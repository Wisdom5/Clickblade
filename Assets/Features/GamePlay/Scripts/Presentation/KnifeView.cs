using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Features.GamePlay.Scripts.Declaration;
using UnityEngine;

namespace Features.GamePlay.Scripts.Presentation
{
    public class KnifeView : MonoBehaviour, IKnifeView
    {
        private Action<IKnifeView> _readyReturnToPool;
        private CancellationTokenSource _lifetimeCts;

        private Vector3 _moveDirection;
        private float _moveSpeed;
        private bool _isMoving;

        public Transform Transform => transform;

        public void Initialize(Action<IKnifeView> readyReturnToPool)
        {
            Debug.Log("[KnifeView] Initialize KnifeView");
            _readyReturnToPool = readyReturnToPool;
        }

        public void StartMovement(Vector3 direction, float speed)
        {
            _moveDirection = direction;
            _moveSpeed = speed;
            _isMoving = true;
        }

        public void StopMovement()
        {
            _isMoving = false;
        }

        public void StartLifetimeTimer(float lifetime)
        {
            StopLifetimeTimer();
            _lifetimeCts = new CancellationTokenSource();
            ReturnAfterDelayAsync(lifetime, _lifetimeCts.Token).Forget();
        }

        public void StopLifetimeTimer()
        {
            _lifetimeCts?.Cancel();
            _lifetimeCts?.Dispose();
            _lifetimeCts = null;
        }

        private void Update()
        {
            if (_isMoving)
            {
                transform.position += _moveDirection * (_moveSpeed * Time.deltaTime);
            }
        }

        private void ReadyReturnToPool()
        {
            StopMovement();
            StopLifetimeTimer();
            _readyReturnToPool?.Invoke(this);
        }

        private async UniTask ReturnAfterDelayAsync(float delay, CancellationToken cancellationToken)
        {
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: cancellationToken);

                if (!cancellationToken.IsCancellationRequested)
                {
                    ReadyReturnToPool();
                }
            }
            catch (OperationCanceledException)
            {
                Debug.LogWarning("[KnifeView] Cancelled ReturnAfterDelayAsync.");
            }
        }

        private void OnDestroy()
        {
            StopLifetimeTimer();
        }
    }
}
