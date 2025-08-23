using System;
using Cysharp.Threading.Tasks;

namespace Utils
{
    public static class UniTaskExtensions
    {
        public static async UniTask<T> WithTimeout<T>(this UniTask<T> task, int milliSeconds)
        {
            var timeoutTask = UniTask.Delay(milliSeconds).ContinueWith(() => default(T));
            var (winArgumentIndex, result1, result2) = await UniTask.WhenAny(task, timeoutTask);

            if (winArgumentIndex == 0)
            {
                return result1;
            }

            throw new TimeoutException("The operation has timed out.");
        }

        public static async UniTask WithTimeout(this UniTask task, int milliSeconds)
        {
            var timeoutTask = UniTask.Delay(milliSeconds);
            var winArgumentIndex = await UniTask.WhenAny(task, timeoutTask);

            if (winArgumentIndex == 0)
            {
                await task;
            }
            else
            {
                throw new TimeoutException("The operation has timed out.");
            }
        }
    }
}
