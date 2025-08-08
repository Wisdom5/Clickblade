using UnityEngine;

namespace Features.GamePlay.Scripts.Declaration
{
    public interface IKnifePoolService
    {
        void SpawnKnife(Vector3 position, Vector3 rotationEuler, Vector3 scale);
        void SetMaxPoolSize(int size);
        void ClearActiveKnives();
        void ResetPool();
    }
}
