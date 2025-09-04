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
        private readonly float _defaultKnifeLifetime = 5f;

        private readonly float _heightStep = 0.25f;
        private IInputSystemService _inputSystemService;
        private IKnifePoolService _knifePoolService;
        private IKnifeThrowService _knifeThrowService;
        private IFirebaseRemoteConfigProvider _remoteConfigProvider;

        [Inject]
        public void Construct(
            IInputSystemService inputSystemService,
            IKnifePoolService knifePoolService,
            IKnifeThrowService knifeThrowService,
            IFirebaseRemoteConfigProvider remoteConfigProvider)
        {
            _inputSystemService = inputSystemService;
            _knifePoolService = knifePoolService;
            _knifeThrowService = knifeThrowService;
            _remoteConfigProvider = remoteConfigProvider;
        }

        private void Start()
        {
            _knifeThrowService.KnifeStuck += OnKnifeStuckInBlock;
            _inputSystemService.InputActions.Player.Tap.performed += OnTap;
        }

        private void OnKnifeStuckInBlock()
        {
            transform.position += new Vector3(0, _heightStep, 0);

            Debug.Log($"[KnifeSpawner] Moved up to {transform.position.y}");
        }

        private void OnTap(InputAction.CallbackContext callbackContext)
        {
            if (_knifePoolService.HasReadyKnife)
            {
                var readyKnife = _knifePoolService.GetReadyKnife();
                if (readyKnife != null)
                {
                    var throwDirection = Vector3.left;
                    var throwSpeed = _remoteConfigProvider.Config.KnifeFlySpeed;

                    _knifeThrowService.ThrowKnife(readyKnife, throwDirection, throwSpeed, _defaultKnifeLifetime);
                }
            }
        }

        private void OnDestroy()
        {
            _knifeThrowService.KnifeStuck -= OnKnifeStuckInBlock;
            _inputSystemService.InputActions.Player.Tap.performed -= OnTap;
        }
    }
}
