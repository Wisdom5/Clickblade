using Core;
using Features.Firebase.Declaration;
using Features.Firebase.Implementation;
using Features.GamePlay.Scripts.Declaration;
using Features.GamePlay.Scripts.Implementation;
using Features.GamePlay.Scripts.Presentation;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Bootstrap
{
    public class GameLifetimeScope : LifetimeScope
    {
        [SerializeField]
        private KnifeView _knifePrefab;
        [SerializeField]
        private Transform _knifeContainer;
        [SerializeField]
        private int _initialPoolSize = 10;
        [SerializeField]
        private int _maxPoolSize = 50;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<IInputSystemService, InputSystemService>(Lifetime.Singleton);
            builder.Register<IFirebaseService, FirebaseService>(Lifetime.Singleton);
            builder.Register<RemoteConfigService>(Lifetime.Singleton);

            builder.Register<IKnifePoolService>(_ =>
                    new KnifePoolService(_knifePrefab, _knifeContainer, _initialPoolSize, _maxPoolSize),
                Lifetime.Singleton);

            builder.RegisterComponentInHierarchy<KnifeSpawner>();

            builder.RegisterEntryPoint<GameBootstrapper>();

            Debug.Log("[GameLifetimeScope] GameLifetimeScope Initialized.");
        }
    }
}
