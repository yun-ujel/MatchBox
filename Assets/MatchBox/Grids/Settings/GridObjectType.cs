using UnityEngine;

namespace MatchBox.Grids
{
    [System.Serializable]
    public class GridObjectType
    {
        [field: SerializeField] public string Name { get; set; }
        [field: SerializeField] public Color Color { get; set; }
    }

}