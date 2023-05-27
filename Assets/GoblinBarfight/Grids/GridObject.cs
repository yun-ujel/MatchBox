using UnityEngine;
using Grids;

namespace GoblinBarfight.Grids
{
    public class GridObject
    {
        #region Parameters
        public static System.Func<Grid<GridObject>, int, int, GridObject> create = (Grid<GridObject> g, int x, int y) => new GridObject(g, x, y);

        public static GameObject GridObjectPrefab { get; set; }

        #region Grid Position
        private Grid<GridObject> grid;
        private int x;
        private int y;
        #endregion

        private GridObjectBehaviour parent;

        #endregion
        public GridObject(Grid<GridObject> grid, int x, int y)
        {
            this.grid = grid;
            this.x = x;
            this.y = y;

            GameObject gameObject = Object.Instantiate(GridObjectPrefab, grid.GridToWorldPosition(x, y, false), Quaternion.identity);
            gameObject.name = $"( {x}, {y} )";

            parent = gameObject.GetComponent<GridObjectBehaviour>();
        }

        public void MoveToPosition(int x, int y)
        {
            parent.MoveToPosition(grid.GridToWorldPosition(x, y, false));
            parent.gameObject.name = $"( {x}, {y} )";

            this.x = x;
            this.y = y;
        }

        public void GetPosition(out int x, out int y)
        {
            x = this.x;
            y = this.y;
        }
    }
}