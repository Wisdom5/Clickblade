using System;
using UnityEngine;

namespace Features.GamePlay.Scripts.Declaration
{
    public interface IKnifeThrowService
    {
        event Action KnifeStuck;

        void ThrowKnife(IKnifeView knife, Vector3 direction, float speed, float lifetime);
        void StopKnife(IKnifeView knife);
    }
}
