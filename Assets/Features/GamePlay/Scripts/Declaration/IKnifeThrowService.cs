using UnityEngine;

namespace Features.GamePlay.Scripts.Declaration
{
    public interface IKnifeThrowService
    {
        void ThrowKnife(IKnifeView knife, Vector3 direction, float speed, float lifetime);
        void StopKnife(IKnifeView knife);
    }
}
