using System;
using System.Threading;
using Core;
using Cysharp.Threading.Tasks;
using Features.Firebase.Declaration;
using Features.Firebase.Implementation;
using Features.GamePlay.Scripts.Declaration;
using UnityEngine;
using VContainer.Unity;

namespace Bootstrap
{
    public class GameBootstrapper : IStartable
    {
        private readonly IFirebaseService _firebaseService;
        private readonly IFirebaseRemoteConfigProvider _firebaseRemoteConfigProvider;
        private readonly IInputSystemService _inputSystemService;
        private readonly IKnifePoolService _knifePoolService;
        private readonly PerformanceService _performanceService;

        public GameBootstrapper(
            IFirebaseService firebaseService,
            IFirebaseRemoteConfigProvider firebaseRemoteConfigProvider,
            IInputSystemService inputService,
            IKnifePoolService knifePoolService,
            PerformanceService performanceService)
        {
            _firebaseService = firebaseService;
            _inputSystemService = inputService;
            _knifePoolService = knifePoolService;
            _performanceService = performanceService;
            _firebaseRemoteConfigProvider = firebaseRemoteConfigProvider;
        }

        public void Start()
        {
            InitializeAsync().Forget();
        }

        private async UniTask InitializeAsync()
        {
            try
            {
                _performanceService.Initialize();
                _inputSystemService.Initialize();
                _knifePoolService.Initialize();
                await _firebaseService.InitializeAsync();
                await _firebaseRemoteConfigProvider.Initialize(CancellationToken.None);//todo create cts

                Debug.Log("[GameBootstrapper] Initialized.");
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }
    }
}
