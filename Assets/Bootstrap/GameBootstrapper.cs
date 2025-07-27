using System;
using Core;
using Cysharp.Threading.Tasks;
using Features.Firebase.Declaration;
using Features.Firebase.Implementation;
using UnityEngine;
using VContainer.Unity;

namespace Bootstrap
{
    public class GameBootstrapper : IStartable
    {
        private readonly IFirebaseService _firebaseService;
        private readonly RemoteConfigService _remoteConfigService;
        private readonly IInputSystemService _inputSystemService;

        public GameBootstrapper(IFirebaseService firebaseService, RemoteConfigService remoteConfigService,
            IInputSystemService inputService)
        {
            _firebaseService = firebaseService;
            _remoteConfigService = remoteConfigService;
            _inputSystemService = inputService;
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
                await _firebaseService.InitializeAsync();
                await _remoteConfigService.InitializeAsync();

                Debug.Log("Bootstrap complete");
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }
    }
}
