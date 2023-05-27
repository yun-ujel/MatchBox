using UnityEngine;
using Grids;

namespace GoblinBarfight.Grids
{
    public class GridDisplay : MonoBehaviour
    {
        private Grid<GridObject> grid;
        [SerializeField] private GridObjectSettings settings;

        private void Start()
        {
            GridObject.Settings = settings;

            grid = new Grid<GridObject>(9, 9, 1f, Vector2.one * -4.5f, GridObject.create);
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SwapObjects(0, 0, 1, 0);
            }
        }

        private void SwapObjects(int xPos1, int yPos1, int xPos2, int yPos2)
        {
            GridObject object1 = grid.GetObject(xPos1, yPos1);
            GridObject object2 = grid.GetObject(xPos2, yPos2);
            
            object1.MoveToPosition(xPos2, yPos2);
            object2.MoveToPosition(xPos1, yPos1);

            grid.SetObject(xPos1, yPos1, object2);
            grid.SetObject(xPos2, yPos2, object1);
        }

        private void SwapObjects(Vector2Int pos1, Vector2Int pos2)
        {
            SwapObjects(pos1.x, pos1.y, pos2.x, pos2.y);
        }
    }

}