using Features.GamePlay.Scripts.Declaration;
using UnityEngine;

namespace Features.GamePlay.Scripts.Presentation
{
    public class BlockView : MonoBehaviour, IBlockView
    {
        [SerializeField]
        private MeshRenderer _meshRenderer;
        [SerializeField]
        private BoxCollider _collider;

        public Transform BlockTransform => transform;

        public Color BlockColor
        {
            get => _meshRenderer.material.color;
            set => _meshRenderer.material.color = value;
        }
    }
}
