using System;
using UnityEngine;

namespace Features.GamePlay.Scripts.Declaration
{
    public interface IKnifeView
    {
        void Initialize(Action<IKnifeView> readyReturnToPool);
        void StartMovement(Vector3 direction, float speed);
        void StopMovement();
        void StartLifetimeTimer(float lifetime);
        void StopLifetimeTimer();
        Transform Transform { get; }
    }
}
