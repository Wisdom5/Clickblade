using UnityEngine;

namespace Features.GamePlay.Scripts.Declaration
{
    public interface IKnifePoolService
    {
        bool HasReadyKnife { get; }

        void SpawnKnife(Vector3 position, Vector3 rotationEuler, Vector3 scale);
        void ThrowReadyKnife(Vector3 direction, float speed);
        void SetMaxPoolSize(int size);
        void ClearActiveKnives();
        void ResetPool();
        void Initialize();
    }
}
