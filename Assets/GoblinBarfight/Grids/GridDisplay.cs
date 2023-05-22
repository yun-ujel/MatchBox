using UnityEngine;

namespace GoblinBarfight.Grids
{
    public class GridDisplay : MonoBehaviour
    {
        private Grid grid;
        [SerializeField] private GameObject gridObject;

        private void Start()
        {
            grid = new Grid(5, 5, 1.5f, Vector2.one * -3.75f);

            for (int x = 0; x < grid.Width; x++)
            {
                for (int y = 0; y < grid.Width; y++)
                {
                    _ = Instantiate(gridObject, grid.GridToWorldPosition(x, y, true), Quaternion.identity, transform);
                }
            }
        }
    }
}
