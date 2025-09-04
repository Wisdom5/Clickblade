using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Features.GamePlay.Scripts.Declaration;
using UnityEngine;

namespace Features.GamePlay.Scripts.Presentation
{
    public class KnifeView : MonoBehaviour, IKnifeView
    {
        [SerializeField]
        private Transform _tip;

        public event Action<IKnifeView, IBlockView> BlockHit;

        public Transform KnifeTransform => transform;
        public GameObject KnifeGameObject => gameObject;

        private Action<IKnifeView> _releaseCallback;
        private CancellationTokenSource _lifetimeCts;

        private bool _isMoving;
        private Vector3 _moveDirection;
        private float _moveSpeed;

        public void Initialize(Action<IKnifeView> releaseCallback)
        {
            _releaseCallback = releaseCallback;
            _isMoving = false;
        }

        public void StartMovement(Vector3 direction, float speed)
        {
            _isMoving = true;
            _moveDirection = direction.normalized;
            _moveSpeed = speed;
        }

        public void StopMovement()
        {
            _isMoving = false;
        }

        public void StartLifetimeTimer(float lifetime)
        {
            StopLifetimeTimer();
            _lifetimeCts = new CancellationTokenSource();
            StartLifetimeTimerAsync(lifetime, _lifetimeCts.Token).Forget();
        }

        public void StopLifetimeTimer()
        {
            _lifetimeCts?.Cancel();
            _lifetimeCts?.Dispose();
            _lifetimeCts = null;
        }

        private async UniTaskVoid StartLifetimeTimerAsync(float lifetime, CancellationToken cancellationToken)
        {
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(lifetime), cancellationToken: cancellationToken);
                if (!cancellationToken.IsCancellationRequested)
                {
                    Debug.Log("[KnifeView] Lifetime expired, returning to pool");
                    _releaseCallback?.Invoke(this);
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        private void Update()
        {
            if (!_isMoving)
            {
                return;
            }

            var distanceThisFrame = _moveSpeed * Time.deltaTime;
            var start = _tip != null ? _tip.position : transform.position;

            if (Physics.Raycast(start, _moveDirection, out var hit, distanceThisFrame))
            {
                transform.position += _moveDirection * hit.distance;

                _isMoving = false;
                StopLifetimeTimer();

                var blockView = hit.collider.GetComponent<IBlockView>();
                if (blockView != null)
                {
                    BlockHit?.Invoke(this, blockView);
                }
            }
            else
            {
                transform.position += _moveDirection * distanceThisFrame;
            }
        }

        public void ResetState()
        {
            _isMoving = false;
            _moveDirection = Vector3.zero;
            _moveSpeed = 0f;
            StopLifetimeTimer();

            BlockHit = null;
        }

        private void OnDestroy()
        {
            StopLifetimeTimer();
        }
    }
}
