using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core
{
    public class InputSystemService : IInputSystemService, IDisposable
    {
        public void Dispose()
        {
            InputActions.Dispose();
        }

        public InputActions InputActions { get; private set; }

        public void Initialize()
        {
            InputActions = new InputActions();
            Enable();
        }

        public void Enable()
        {
            InputActions.Enable();
        }

        public void Disable()
        {
            InputActions.Disable();
        }

        public Vector2 GetPointerPosition()
        {
            return Mouse.current != null
                ? Mouse.current.position.ReadValue()
                : Vector2.zero;
        }
    }
}
