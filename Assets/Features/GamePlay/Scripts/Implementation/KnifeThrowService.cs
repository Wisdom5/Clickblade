using System;
using Features.GamePlay.Scripts.Declaration;
using UnityEngine;

namespace Features.GamePlay.Scripts.Implementation
{
    public class KnifeThrowService : IKnifeThrowService
    {
        public event Action KnifeStuck;

        public void ThrowKnife(IKnifeView knife, Vector3 direction, float speed, float lifetime)
        {
            knife.BlockHit += OnKnifeHitBlock;

            knife.StartMovement(direction.normalized, speed);
            knife.StartLifetimeTimer(lifetime);

            Debug.Log($"[KnifeThrowService] Throwing knife with speed: {speed}, lifetime: {lifetime}");
        }

        public void StopKnife(IKnifeView knife)
        {
            knife.StopMovement();
            knife.StopLifetimeTimer();

            knife.BlockHit -= OnKnifeHitBlock;

            Debug.Log("[KnifeThrowService] Knife stopped");
        }

        private void OnKnifeHitBlock(IKnifeView knife, IBlockView block)
        {
            Debug.Log("[KnifeThrowService] Knife hit block - sticking to it");

            knife.StopMovement();
            knife.StopLifetimeTimer();

            var knifeTransform = knife.KnifeTransform;
            var blockTransform = block.BlockTransform;

            if (knifeTransform != null && blockTransform != null)
            {
                knifeTransform.SetParent(blockTransform, true);
                Debug.Log("[KnifeThrowService] Knife stuck to block");
            }

            KnifeStuck?.Invoke();
            knife.BlockHit -= OnKnifeHitBlock;
        }
    }
}
