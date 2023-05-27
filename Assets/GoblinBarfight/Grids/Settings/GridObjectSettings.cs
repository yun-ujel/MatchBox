using UnityEngine;

namespace GoblinBarfight.Grids
{
    [CreateAssetMenu(fileName = "Grid Object Settings", menuName = "Scriptable Object/Grid Object Settings")]
    public class GridObjectSettings : ScriptableObject
    {
        [field: SerializeField] public GameObject GridObjectPrefab { get; set; }
        public GridObjectType[] types;
    }
}
