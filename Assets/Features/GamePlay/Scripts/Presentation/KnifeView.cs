using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Features.GamePlay.Scripts.Presentation
{
    public class KnifeView : MonoBehaviour
    {
        private Action<KnifeView> _onReturnToPool;
        private CancellationTokenSource _returnCts;

        public void Initialize(Action<KnifeView> onReturnToPool)
        {
            _onReturnToPool = onReturnToPool;
        }

        public void StartReturnTimer(float delay = 5f)
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

        private async UniTask ReturnAfterDelayAsync(float delay, CancellationToken cancellationToken)
        {
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: cancellationToken);

                if (!cancellationToken.IsCancellationRequested)
                {
                    ReturnToPool();
                }
            }
            catch (OperationCanceledException)
            {
                Debug.LogWarning("[KnifeView] Operation ReturnAfterDelayAsync cancelled.");
            }
        }

        public void ReturnToPool()
        {
            StopReturnTimer();
            _onReturnToPool?.Invoke(this);
        }

        private void OnDisable()
        {
            StopReturnTimer();
        }

        private void OnDestroy()
        {
            StopReturnTimer();
        }
    }
}
