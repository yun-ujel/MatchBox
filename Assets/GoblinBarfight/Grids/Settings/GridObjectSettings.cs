using UnityEngine;

namespace GoblinBarfight.Grids
{
    [CreateAssetMenu(fileName = "Grid Object Settings", menuName = "Scriptable Object/Grid Object Settings")]
    public class GridObjectSettings : ScriptableObject
    {
        [field: SerializeField] public GameObject GridObjectPrefab { get; set; }

        [Header("Gameplay")]
        public GridObjectType[] types;
        [field: SerializeField] public int RequiredObjectsForMatch { get; set; } = 3;

        [field: Header("Sprites")]
        [field: SerializeField] public Sprite aSprite { get; set; }
        [field: SerializeField] public Sprite bSprite { get; set; }
    }
}
