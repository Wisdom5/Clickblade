using System;
using UnityEngine;

namespace Features.GamePlay.Scripts.Declaration
{
    public interface IKnifeView
    {
        Transform KnifeTransform { get; }
        GameObject KnifeGameObject { get; }

        event Action<IKnifeView, IBlockView> BlockHit;
        
        void Initialize(Action<IKnifeView> releaseCallback);
        void StartMovement(Vector3 direction, float speed);
        void StopMovement();
        void StartLifetimeTimer(float lifetime);
        void StopLifetimeTimer();
        void ResetState();
    }
}
