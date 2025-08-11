using Core;
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

        [Inject]
        public void Construct(IInputSystemService inputSystemService, IKnifePoolService knifePoolService)
        {
            _inputSystemService = inputSystemService;
            _knifePoolService = knifePoolService;
        }

        private void Start()
        {
            _inputSystemService.InputActions.Player.Tap.performed += OnTap;
        }

        private void OnTap(InputAction.CallbackContext ctx)
        {
            if (_knifePoolService.HasReadyKnife)
            {
                _knifePoolService.ThrowReadyKnife(Vector3.left, 20f);
            }
        }

        private void OnDestroy()
        {
            _inputSystemService.InputActions.Player.Tap.performed -= OnTap;
        }
    }
}
