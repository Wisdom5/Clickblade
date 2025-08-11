using System;
using UnityEngine;

namespace Features.GamePlay.Scripts.Declaration
{
    public interface IKnifeView
    {
        void Initialize(Action<IKnifeView> readyReturnToPool);
        void StopReturnTimer();
        void Throw(Vector3 direction, float speed, float lifeTime);
    }
}
