using Core;
using Features.Firebase.Declaration;
using Features.Firebase.Implementation;
using Features.GamePlay.Scripts.Presentation;
using VContainer;
using VContainer.Unity;

namespace Bootstrap
{
    public class GameLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<IInputSystemService, InputSystemService>(Lifetime.Singleton);
            builder.Register<IFirebaseService, FirebaseService>(Lifetime.Singleton);
            builder.Register<RemoteConfigService>(Lifetime.Singleton);

            builder.RegisterComponentInHierarchy<KnifeView>();

            builder.RegisterEntryPoint<GameBootstrapper>();
        }
    }
}
