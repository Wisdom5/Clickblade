using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
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
        private readonly Vector3 _spawnScale = new(40, 40, 40);
        private readonly float _defaultReturnTime = 5f; //todo get from remote config
        private readonly KnifeView _knifePrefab;
        private readonly Transform _container;
        private readonly bool _collectionChecks = true;
        private readonly bool _disableCollectionChecksInRelease = true;

        private readonly List<KnifeView> _activeKnives = new();
        private KnifeView _readyKnife;
        private IObjectPool<KnifeView> _pool;
        private int _maxPoolSize;
        private readonly int _initialPoolSize;

        public bool HasReadyKnife => _readyKnife != null;

        //todo make maxpoolsize logic(increase pool size or rectreacte knifes from old knifes)
        public KnifePoolService(
            KnifeView knifePrefab,
            Transform container,
            int initialPoolSize = 10,
            int maxPoolSize = 50)
        {
            _knifePrefab = knifePrefab;
            _container = container;
            _initialPoolSize = initialPoolSize;
            _maxPoolSize = maxPoolSize;
        }

        public void Initialize()
        {
            PrepareNextKnife().Forget();
            Debug.Log("[KnifePoolService]  Initialized.");
        }

        private IObjectPool<KnifeView> Pool
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
                    _pool = new ObjectPool<KnifeView>(
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

        private KnifeView GetKnife()
        {
            var knife = TryGetKnifeFromPool();

            if (knife == null)
            {
                Debug.LogError("[KnifePoolService] Failed to get knife from pool. Pool might be exhausted.");
                return null;
            }

            _activeKnives.Add(knife);
            return knife;
        }

        public async UniTask<KnifeView> GetKnifeAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await UniTask.Yield(cancellationToken);

            return GetKnife();
        }

        private KnifeView TryGetKnifeFromPool()
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

        public void ReleaseKnife(KnifeView knife)
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

        private KnifeView CreatePooledItem()
        {
            var instance = Object.Instantiate(_knifePrefab, _container);

            instance.gameObject.SetActive(false);
            instance.Initialize(ReleaseKnife);

            return instance;
        }

        private void OnTakeFromPool(KnifeView knife)
        {
            knife.transform.SetParent(_container, false);
            knife.gameObject.SetActive(true);
        }

        private void OnReturnedToPool(KnifeView knife)
        {
            knife.StopReturnTimer();

            if (knife.transform.parent != _container)
            {
                knife.transform.SetParent(_container, false);
            }

            knife.gameObject.SetActive(false);
        }

        private void OnDestroyPoolObject(KnifeView knife)
        {
            if (knife != null)
            {
                Object.Destroy(knife.gameObject);
            }
        }

        public void Dispose()
        {
            ClearActiveKnives();

            _pool?.Clear();
            _pool = null;
        }

        public void SpawnKnife(Vector3 position, Vector3 rotationEuler, Vector3 scale)
        {
            var knife = GetKnife();
            if (knife == null)
            {
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

            thrownKnife.Throw(direction, speed, _defaultReturnTime);

            PrepareNextKnife().Forget();
        }

        private async UniTaskVoid PrepareNextKnife()
        {
            await UniTask.Yield();
            _readyKnife = await GetKnifeAsync();

            if (_readyKnife != null)
            {
                var knifeTransform = _readyKnife.transform;
                knifeTransform.SetLocalPositionAndRotation(_spawnPos, Quaternion.Euler(_spawnRot));
                knifeTransform.localScale = _spawnScale;
            }
        }
    }
}
