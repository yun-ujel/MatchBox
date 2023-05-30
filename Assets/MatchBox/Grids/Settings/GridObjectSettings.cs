using UnityEngine;

namespace MatchBox.Grids
{
    [CreateAssetMenu(fileName = "Grid Object Settings", menuName = "Scriptable Object/Grid Object Settings")]
    public class GridObjectSettings : ScriptableObject
    {
        [field: SerializeField] public GameObject GridObjectPrefab { get; set; }

        [Header("Gameplay")]
        public GridObjectType[] types;
        [field: SerializeField] public int RequiredObjectsForMatch { get; set; } = 3;

        [field: Space, SerializeField] public bool AllowMatchingWithMatchedObjects { get; set; } = true;
        [field: SerializeField] public bool AllowMovingOfMatchedObjects { get; set; } = false;

        [field: Header("Sprites")]
        [field: SerializeField] public Sprite ASprite { get; set; }
        [field: SerializeField] public Sprite BSprite { get; set; }
    }
}
