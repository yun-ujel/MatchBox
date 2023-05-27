using UnityEngine;
using Grids;
using System.Collections.Generic;

namespace GoblinBarfight.Grids
{
    public class GridObject
    {
        #region Parameters

        #region Static
        public static System.Func<Grid<GridObject>, int, int, GridObject> create = (Grid<GridObject> g, int x, int y) => new GridObject(g, x, y);
        public static GridObjectSettings Settings { get; set; }
        #endregion

        #region Grid Position
        private Grid<GridObject> grid;
        private int x;
        private int y;
        #endregion

        private GridObjectBehaviour parent;

        public GridObjectType Type { get; private set; }

        #endregion
        public GridObject(Grid<GridObject> grid, int x, int y)
        {
            this.grid = grid;
            this.x = x;
            this.y = y;

            GameObject gameObject = Object.Instantiate(Settings.GridObjectPrefab, grid.GridToWorldPosition(x, y, false), Quaternion.identity);
            gameObject.name = $"( {x}, {y} )";

            parent = gameObject.GetComponent<GridObjectBehaviour>();

            SetType(CalculateParentType(grid, x, y));
        }

        private GridObjectType CalculateParentType(Grid<GridObject> grid, int x, int y)
        {
            List<GridObjectType> validTypes = new List<GridObjectType>(Settings.types);
            bool allTypesValid = true;

            #region Remove Invalid Types
            if (x >= 2)
            {
                /* Check Two Previous Grid Spaces to avoid Matches */
                if (grid.GetObject(x - 1, y).Type == grid.GetObject(x - 2, y).Type)
                {
                    _ = validTypes.Remove(grid.GetObject(x - 1, y).Type);
                    allTypesValid = false;
                }
            }

            if (y >= 2)
            {
                /* Check Two Previous Grid Spaces to avoid Matches */
                if (grid.GetObject(x, y - 1).Type == grid.GetObject(x, y - 2).Type)
                {
                    _ = validTypes.Remove(grid.GetObject(x, y - 1).Type);
                    allTypesValid = false;
                }
            }
            #endregion

            #region Select Type
            int selection = Random.Range(0, Settings.types.Length);

            if (allTypesValid)
            {
                return Settings.types[selection];
            }

            selection = Random.Range(0, validTypes.Count);

            return validTypes[selection];
            #endregion
        }

        #region Parent Reference Methods
        public void SetType(GridObjectType type)
        {
            Type = type;

            parent.GetComponent<SpriteRenderer>().color = type.Color;
        }

        public void MoveToPosition(int x, int y)
        {
            parent.MoveToPosition(grid.GridToWorldPosition(x, y, false));
            parent.gameObject.name = $"( {x}, {y} )";

            this.x = x;
            this.y = y;
        }
        #endregion
    }
}