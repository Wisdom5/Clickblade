using Features.Firebase.Declaration;
using Features.Firebase.Implementation;
using VContainer;
using VContainer.Unity;

namespace Core.Bootstrap
{
    public class GameLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<IFirebaseService, FirebaseService>(Lifetime.Singleton);
            builder.Register<RemoteConfigService>(Lifetime.Singleton);
            
            builder.RegisterEntryPoint<GameBootstrapper>();
        }
    }
}
