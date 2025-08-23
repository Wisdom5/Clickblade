using Features.GamePlay.Scripts.Declaration;
using UnityEngine;

namespace Features.GamePlay.Scripts.Implementation
{
    public class KnifeThrowService : IKnifeThrowService
    {
        public void ThrowKnife(IKnifeView knife, Vector3 direction, float speed, float lifetime)
        {
            Debug.Log($"[KnifeThrowService] Throwing knife with speed: {speed}, lifetime: {lifetime}");

            knife.StartMovement(direction.normalized, speed);

            knife.StartLifetimeTimer(lifetime);
        }

        public void StopKnife(IKnifeView knife)
        {
            Debug.Log("[KnifeThrowService] Stopping knife");

            knife.StopMovement();
            knife.StopLifetimeTimer();
        }
    }
}
