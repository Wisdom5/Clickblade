using UnityEngine;

namespace Features.GamePlay.Scripts.Declaration
{
    public interface IBlockView
    {
        Transform BlockTransform { get; }
        Color BlockColor { get; set; }
    }
}
