using Core;
using Features.Firebase.Declaration;
using Features.GamePlay.Scripts.Declaration;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace Features.GamePlay.Scripts.Presentation
{
    public class KnifeSpawner : MonoBehaviour
    {
        private IInputSystemService _inputSystemService;
        private IKnifePoolService _knifePoolService;
        private IFirebaseRemoteConfigProvider _remoteConfigProvider;

        [Inject]
        public void Construct(IInputSystemService inputSystemService, IKnifePoolService knifePoolService,
            IFirebaseRemoteConfigProvider remoteConfigProvider)
        {
            _inputSystemService = inputSystemService;
            _knifePoolService = knifePoolService;
            _remoteConfigProvider = remoteConfigProvider;
        }

        private void Start()
        {
            _inputSystemService.InputActions.Player.Tap.performed += OnTap;
        }

        private void OnTap(InputAction.CallbackContext ctx)
        {
            if (_knifePoolService.HasReadyKnife)
            {
                _knifePoolService.ThrowReadyKnife(Vector3.left, _remoteConfigProvider.Config.KnifeFlySpeed);
            }
        }

        private void OnDestroy()
        {
            _inputSystemService.InputActions.Player.Tap.performed -= OnTap;
        }
    }
}
