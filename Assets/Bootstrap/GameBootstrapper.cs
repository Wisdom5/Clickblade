using System;
using System.Threading;
using Core;
using Cysharp.Threading.Tasks;
using Features.Firebase.Declaration;
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
                #region Infrastructure Layer

                _performanceService.Initialize();
                _inputSystemService.Initialize();
                await _firebaseService.InitializeAsync();

                #endregion

                #region Configuration Layer

                await _firebaseRemoteConfigProvider.Initialize(CancellationToken.None); //todo create cts

                #endregion

                #region Services Layer

                //Services Layer

                #endregion

                #region Domain Logic Layer

                _knifePoolService.Initialize();

                #endregion

                #region Presentation Layer

                //Presentation Layer

                #endregion

                Debug.Log("[GameBootstrapper] Initialized.");
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }
    }
}
