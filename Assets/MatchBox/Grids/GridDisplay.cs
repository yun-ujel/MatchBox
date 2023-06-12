using UnityEngine;
using Grids;
using UnityEngine.InputSystem;

namespace MatchBox.Grids
{
    using Utilities;

    public class GridDisplay : MonoBehaviour
    {
        #region Parameters

        #region Grid

        private Grid<GridObject> grid;

        [SerializeField] private GridObjectSettings settings;

        public class SetGridEventArgs : System.EventArgs
        {
            public Grid<GridObject> grid;
        }
        public event System.EventHandler<SetGridEventArgs> OnSetGridEvent;

        #endregion

        #endregion

        private void Start()
        {
            GridObject.Settings = settings;
            GridObjectUtils.Settings = settings;

            grid = new Grid<GridObject>(9, 9, 1f, Vector2.one * -4.5f, GridObject.create);
            OnSetGridEvent?.Invoke(this, new SetGridEventArgs { grid = grid });
        }

        public void SwapObjects(int xPos1, int yPos1, int xPos2, int yPos2)
        {
            GridObject object1 = grid.GetObject(xPos1, yPos1);
            GridObject object2 = grid.GetObject(xPos2, yPos2);

            if ((!object1.IsMatched && !object2.IsMatched) || settings.AllowMovingOfMatchedObjects)
            {
                grid.SetObject(xPos1, yPos1, object2); /* Put Object 2 in Object 1's Position */
                grid.SetObject(xPos2, yPos2, object1); /* Put Object 1 in Object 2's Position */

                object2.MoveToPosition(xPos1, yPos1);
                object1.MoveToPosition(xPos2, yPos2);
            }
        }

        public void SwapObjects(Vector2Int pos1, Vector2Int pos2)
        {
            SwapObjects(pos1.x, pos1.y, pos2.x, pos2.y);
        }

        public void RegenerateGrid()
        {
            GridObject[,] gridCopy = new GridObject[grid.Width, grid.Height];

            for (int x = 0; x < grid.Width; x++)
            {
                for (int y = 0; y < grid.Height; y++)
                {
                    gridCopy[x, y] = grid.GetObject(x, y);
                    if (!gridCopy[x, y].IsMatched)
                    {
                        grid.SetObject(x, y, null);
                    }
                }
            }

            for (int x = 0; x < grid.Width; x++)
            {
                for (int y = 0; y < grid.Height; y++)
                {
                    if (gridCopy[x, y].IsMatched)
                    {
                        continue;
                    }
                    gridCopy[x, y].Regenerate();
                    grid.SetObject(x, y, gridCopy[x, y]);
                }
            }
        }
    }

}