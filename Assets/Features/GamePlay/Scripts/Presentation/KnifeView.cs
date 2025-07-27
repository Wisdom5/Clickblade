using Core;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace Features.GamePlay.Scripts.Presentation
{
    public class KnifeView : MonoBehaviour
    {
        private IInputSystemService _inputSystemService;

        [Inject]
        public void Construct(IInputSystemService inputSystemService)
        {
            _inputSystemService = inputSystemService;
        }

        private void Start()
        {
            _inputSystemService.InputActions.Player.Tap.performed += OnTap;
        }

        private void OnTap(InputAction.CallbackContext ctx)
        {
            Debug.Log("Tapped!!!");
        }

        private void OnDestroy()
        {
            _inputSystemService.InputActions.Player.Tap.performed -= OnTap;
        }
    }
}
