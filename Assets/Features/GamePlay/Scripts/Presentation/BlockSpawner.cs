using UnityEngine;

namespace Features.GamePlay.Scripts.Presentation
{
    public class BlockSpawner : MonoBehaviour
    {
        [SerializeField]
        private BlockView _blockPrefab;

        private readonly Color32 _colorA = new(237, 106, 90, 255);
        private readonly Color32 _colorB = new(243, 156, 145, 255);

        private readonly int _blockCount = 50;
        private readonly float _stepY = 0.25f;

        private void Start()
        {
            BuildColumn();
        }

        private void BuildColumn()
        {
            ClearColumn();

            for (var i = 0; i < _blockCount; i++)
            {
                var pos = new Vector3(0, i * _stepY, 0);
                var blockView = Instantiate(_blockPrefab, transform);
                blockView.transform.localPosition = pos;

                blockView.Color = i % 2 == 0 ? _colorA : _colorB;
            }
        }

        private void ClearColumn()
        {
            for (var i = transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }
    }
}
