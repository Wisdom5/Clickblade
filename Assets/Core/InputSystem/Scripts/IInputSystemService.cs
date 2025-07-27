using UnityEngine;

namespace Core
{
    public interface IInputSystemService
    {
        InputActions InputActions { get; }

        void Initialize();
        void Enable();
        void Disable();
        Vector2 GetPointerPosition();
    }
}
