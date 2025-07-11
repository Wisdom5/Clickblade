using System;
using Cysharp.Threading.Tasks;
using Features.Firebase.Declaration;
using Features.Firebase.Implementation;
using UnityEngine;
using VContainer.Unity;

namespace Core.Bootstrap
{
    public class GameBootstrapper : IStartable
    {
        private readonly IFirebaseService _firebaseService;
        private readonly RemoteConfigService _remoteConfigService;

        // Конструктор для инъекции зависимостей
        public GameBootstrapper(IFirebaseService firebaseService, RemoteConfigService remoteConfigService)
        {
            _firebaseService = firebaseService;
            _remoteConfigService = remoteConfigService;
        }

        public void Start()
        {
            InitializeAsync().Forget();
        }

        private async UniTask InitializeAsync()
        {
            try
            {
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
