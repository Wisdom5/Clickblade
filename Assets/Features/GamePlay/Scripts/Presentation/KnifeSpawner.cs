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
            _knifePoolService.SpawnKnife(
                new Vector3(2.3f, -2.04f, -2.88f),
                new Vector3(0f, 90f, 90f),
                new Vector3(40f, 40f, 40f)
            );
        }

        private void OnDestroy()
        {
            _inputSystemService.InputActions.Player.Tap.performed -= OnTap;
        }
    }
}
