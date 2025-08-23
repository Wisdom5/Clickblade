using System;
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
        private int _maxPoolSize = 50;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<PerformanceService>(Lifetime.Singleton);
            builder.Register<IInputSystemService, InputSystemService>(Lifetime.Singleton);
            builder.Register<IFirebaseService, FirebaseService>(Lifetime.Singleton);
            builder.Register<RemoteConfigService>(Lifetime.Singleton);

            builder.Register<IKnifeThrowService, KnifeThrowService>(Lifetime.Singleton);

            builder.Register(container =>
                {
                    var knifeThrowService = container.Resolve<IKnifeThrowService>();
                    return new KnifePoolService(
                        _knifePrefab,
                        _knifeContainer,
                        knifeThrowService,
                        10,
                        _maxPoolSize);
                }, Lifetime.Singleton)
                .As<IKnifePoolService>()
                .As<IDisposable>();

            builder.RegisterComponentInHierarchy<KnifeSpawner>();

            builder.RegisterEntryPoint<GameBootstrapper>();

            Debug.Log("[GameLifetimeScope] GameLifetimeScope Initialized.");
        }
    }
}
