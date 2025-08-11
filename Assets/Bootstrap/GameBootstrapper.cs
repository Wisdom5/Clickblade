using System;
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
        private readonly IInputSystemService _inputSystemService;
        private readonly IKnifePoolService _knifePoolService;
        private readonly RemoteConfigService _remoteConfigService;

        public GameBootstrapper(
            IFirebaseService firebaseService,
            RemoteConfigService remoteConfigService,
            IInputSystemService inputService,
            IKnifePoolService knifePoolService)
        {
            _firebaseService = firebaseService;
            _remoteConfigService = remoteConfigService;
            _inputSystemService = inputService;
            _knifePoolService = knifePoolService;
        }

        public void Start()
        {
            InitializeAsync().Forget();
        }

        private async UniTask InitializeAsync()
        {
            try
            {
                _inputSystemService.Initialize();
                _knifePoolService.Initialize();
                await _firebaseService.InitializeAsync();
                await _remoteConfigService.InitializeAsync();

                Debug.Log("[GameBootstrapper] Initialized.");
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }
    }
}
