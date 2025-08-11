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
        private CancellationTokenSource _returnCts;

        private Vector3 _moveDirection;
        private float _moveSpeed;
        private bool _isFlying;

        public void Initialize(Action<IKnifeView> readyReturnToPool)
        {
            _readyReturnToPool = readyReturnToPool;
        }

        private void StartReturnTimer(float delay = 5f)
        {
            StopReturnTimer();
            _returnCts = new CancellationTokenSource();
            ReturnAfterDelayAsync(delay, _returnCts.Token).Forget();
        }

        public void StopReturnTimer()
        {
            _returnCts?.Cancel();
            _returnCts?.Dispose();
            _returnCts = null;
        }

        public void Throw(Vector3 direction, float speed, float lifeTime)
        {
            _moveDirection = direction.normalized;
            _moveSpeed = speed;
            _isFlying = true;

            StartReturnTimer(lifeTime);
        }

        private void Update()
        {
            if (_isFlying)
            {
                transform.position += _moveDirection * (_moveSpeed * Time.deltaTime);
            }
        }

        private void ReadyReturnToPool()
        {
            _isFlying = false;
            StopReturnTimer();
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
    }
}
