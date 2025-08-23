using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Features.Firebase.Declaration
{
    public interface IFirebaseRemoteConfigProvider
    {
        event Action Updated;

        IRemoteConfig Config { get; }

        UniTask Initialize(CancellationToken cancellationToken);
    }
}
