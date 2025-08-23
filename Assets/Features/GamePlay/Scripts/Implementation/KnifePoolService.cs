using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Features.Firebase.Declaration;
using Features.GamePlay.Scripts.Declaration;
using Features.GamePlay.Scripts.Presentation;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace Features.GamePlay.Scripts.Implementation
{
    public class KnifePoolService : IKnifePoolService, IDisposable
    {
        private readonly Vector3 _spawnPos = new(0, 0, 0);
        private readonly Vector3 _spawnRot = new(0, 90, 90);
        private readonly Vector3 _spawnScale = new(25, 25, 25);
        private readonly float _defaultReturnTime = 5f; //todo get from remote config
        private readonly IKnifeView _knifePrefab;
        private readonly Transform _container;
        private readonly IKnifeThrowService _knifeThrowService;
        private readonly IFirebaseRemoteConfigProvider _firebaseRemoteConfigProvider;
        private readonly bool _collectionChecks = true;
        private readonly bool _disableCollectionChecksInRelease = true;

        private readonly List<IKnifeView> _activeKnives = new();
        private IKnifeView _readyKnife;
        private IObjectPool<IKnifeView> _pool;
        private int _maxPoolSize;
        private readonly int _initialPoolSize;

        public bool HasReadyKnife => _readyKnife != null;

        //todo make maxpoolsize logic(increase pool size or rectreacte knifes from old knifes)
        public KnifePoolService(
            IKnifeView knifePrefab,
            Transform container,
            IKnifeThrowService knifeThrowService,
            IFirebaseRemoteConfigProvider firebaseRemoteConfigProvider,
            int initialPoolSize = 10,
            int maxPoolSize = 50)
        {
            _knifePrefab = knifePrefab;
            _container = container;
            _knifeThrowService = knifeThrowService ?? throw new ArgumentNullException(nameof(knifeThrowService));
            _firebaseRemoteConfigProvider = firebaseRemoteConfigProvider ??
                                            throw new ArgumentNullException(nameof(firebaseRemoteConfigProvider));
            _initialPoolSize = initialPoolSize;
            _maxPoolSize = maxPoolSize;
        }

        public void Initialize()
        {
            PrepareNextKnife().Forget();
            Debug.Log("[KnifePoolService]  Initialized.");
        }

        private IObjectPool<IKnifeView> Pool
        {
            get
            {
                if (_pool == null)
                {
                    var useCollectionChecks = _collectionChecks;

#if !DEVELOPMENT_BUILD && !UNITY_EDITOR
                    if (_disableCollectionChecksInRelease)
                    {
                        useCollectionChecks = false;
                    }
#endif
                    _pool = new ObjectPool<IKnifeView>(
                        CreatePooledItem,
                        OnTakeFromPool,
                        OnReturnedToPool,
                        OnDestroyPoolObject,
                        useCollectionChecks,
                        _initialPoolSize,
                        _maxPoolSize);
                }

                return _pool;
            }
        }

        private IKnifeView GetKnife()
        {
            var knife = TryGetKnifeFromPool();

            if (knife == null)
            {
                Debug.LogError("[KnifePoolService] Failed to get knife from pool. Pool might be exhausted.");
                throw new InvalidOperationException("[KnifePoolService] knife can not be null.");
            }

            _activeKnives.Add(knife);
            return knife;
        }

        public async UniTask<IKnifeView> GetKnifeAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await UniTask.Yield(cancellationToken);

            return GetKnife();
        }

        private IKnifeView TryGetKnifeFromPool()
        {
            try
            {
                var knife = Pool.Get();

                if (knife == null)
                {
                    Debug.LogError("[KnifePoolService] Object pool returned null. Possibly exhausted.");
                }

                return knife;
            }
            catch (InvalidOperationException exception)
            {
                Debug.LogError(
                    $"[KnifePoolService] Knife pool exhausted: {exception.Message}. Consider increasing max pool size.");
                return null;
            }
        }

        public void ReleaseKnife(IKnifeView knife)
        {
            if (knife == null)
            {
                return;
            }

            if (_activeKnives.Remove(knife))
            {
                Pool.Release(knife);
            }
        }

        public void ClearActiveKnives()
        {
            foreach (var knife in _activeKnives)
            {
                if (knife != null)
                {
                    Pool.Release(knife);
                }
            }

            _activeKnives.Clear();
        }

        public void ResetPool()
        {
            ClearActiveKnives();

            if (_pool != null)
            {
                _pool.Clear();
                _pool = null;
            }
        }

        public void SetMaxPoolSize(int size)
        {
            if (size <= _maxPoolSize)
            {
                return;
            }

            _maxPoolSize = size;

            if (_pool != null)
            {
                ClearActiveKnives();
                _pool.Clear();
                _pool = null;
            }
        }

        private IKnifeView CreatePooledItem()
        {
            var knifeObject = ToKnifeView(_knifePrefab);
            if (knifeObject == null)
            {
                throw new InvalidOperationException("Knife prefab cannot be null when creating pooled item.");
            }

            var instance = Object.Instantiate(knifeObject, _container);

            instance.gameObject.SetActive(false);
            instance.Initialize(ReleaseKnife);

            return instance;
        }

        private void OnTakeFromPool(IKnifeView knife)
        {
            var knifeObject = ToKnifeView(knife);
            if (knifeObject == null)
            {
                return;
            }

            knifeObject.transform.SetParent(_container, false);
            knifeObject.gameObject.SetActive(true);
        }

        private void OnReturnedToPool(IKnifeView knife)
        {
            var knifeObject = ToKnifeView(knife);
            if (knifeObject == null)
            {
                return;
            }

            knife.StopLifetimeTimer();

            if (knifeObject.transform.parent != _container)
            {
                knifeObject.transform.SetParent(_container, false);
            }

            knifeObject.gameObject.SetActive(false);
        }

        private void OnDestroyPoolObject(IKnifeView knife)
        {
            var knifeObject = ToKnifeView(knife);
            if (knifeObject == null)
            {
                return;
            }

            Object.Destroy(knifeObject.gameObject);
        }

        public void Dispose()
        {
            ClearActiveKnives();

            _pool?.Clear();
            _pool = null;
        }

        public void SpawnKnife(Vector3 position, Vector3 rotationEuler, Vector3 scale)
        {
            var knife = GetKnife() as KnifeView;
            if (knife == null)
            {
                Debug.LogError("[KnifePoolService] IKnifeView is not KnifeView or null!");
                return;
            }

            var knifeTransform = knife.transform;
            knifeTransform.SetPositionAndRotation(position, Quaternion.Euler(rotationEuler));
            knifeTransform.localScale = scale;
        }

        public void ThrowReadyKnife(Vector3 direction, float speed)
        {
            if (_readyKnife == null)
            {
                Debug.LogWarning("[KnifePoolService] No knife ready to throw!");
                return;
            }

            var thrownKnife = _readyKnife;
            _readyKnife = null;

            _knifeThrowService.ThrowKnife(thrownKnife, direction, speed, _defaultReturnTime);

            PrepareNextKnife().Forget();
        }

        private async UniTaskVoid PrepareNextKnife()
        {
            await UniTask.Yield();
            _readyKnife = await GetKnifeAsync();
            var readyKnifeObject = _readyKnife as KnifeView;

            if (_readyKnife != null && readyKnifeObject != null)
            {
                var knifeTransform = readyKnifeObject.transform;
                knifeTransform.SetLocalPositionAndRotation(_spawnPos, Quaternion.Euler(_spawnRot));
                knifeTransform.localScale = _spawnScale;
            }
        }

        private KnifeView ToKnifeView(IKnifeView knife)
        {
            if (knife == null)
            {
                Debug.LogError("[KnifePoolService] IKnifeView is not KnifeView or null!");
            }

            var knifeObject = knife as KnifeView;

            return knifeObject;
        }
    }
}
