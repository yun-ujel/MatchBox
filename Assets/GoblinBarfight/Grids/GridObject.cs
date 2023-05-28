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
        private void SetType(GridObjectType type)
        {
            Type = type;

            parent.GetComponent<SpriteRenderer>().color = type.Color;
        }

        public void MoveToPosition(int x, int y)
        {
            parent.MoveToPosition(grid.GridToWorldPosition(x, y, false));
            parent.gameObject.name = $"( {x}, {y} )";

            GridObject[] matches = FindMatches(x, y);
            if (matches.Length > 0)
            {
                SetSprite(false);
                for (int i = 0; i < matches.Length; i++)
                {
                    matches[i].SetSprite(false);
                }
            }

            this.x = x;
            this.y = y;
        }

        public void SetSprite(bool a)
        {
            if (a)
            {
                parent.GetComponent<SpriteRenderer>().sprite = Settings.aSprite;
                return;
            }
            parent.GetComponent<SpriteRenderer>().sprite = Settings.bSprite;
        }
        #endregion

        /*private bool IsMatchInAxis(int axis, int x, int y)
        {
            if (axis > 1 || axis < 0)
            {
                return false;
            }

            int counter = 0;
            #region Check X Axis
            if (axis == 0)
            {
                for (int i = -2; i < 3; i++)
                {
                    if (counter > 1)
                    {
                        break;
                    }
                    if (i == 0)
                    {
                        continue;
                    }

                    if (grid.GetObject(x + i, y) == null)
                    {
                        counter = 0;
                        continue;
                    }
                    else if (grid.GetObject(x + i, y).Type == Type)
                    {
                        counter++;
                        continue;
                    }
                    counter = 0;
                }

                return counter > 1;
            }
            #endregion
            #region Check Y Axis
            for (int i = -2; i < 3; i++)
            {
                if (counter > 1)
                {
                    break;
                }
                if (i == 0)
                {
                    continue;
                }

                if (grid.GetObject(x, y + i) == null)
                {
                    counter = 0;
                    continue;
                }
                else if (grid.GetObject(x, y + i).Type == Type)
                {
                    counter++;
                    continue;
                }
                counter = 0;
            }

            return counter > 1;
            #endregion
        }
        */

        public GridObject[] FindMatches(int x, int y)
        {
            List<GridObject> match3Objects = new List<GridObject>();
            List<Vector2Int> matchingObjectPositions = new List<Vector2Int>();

            GridObject gridObject;

            #region Check X Axis
            for (int i = -2; i < 3; i++)
            {
                gridObject = grid.GetObject(x + i, y);

                if (i == 0)
                {
                    continue;
                }

                if (gridObject == null || gridObject.Type != Type)
                {
                    if (matchingObjectPositions.Count >= 2)
                    {
                        for (int o = 0; o < matchingObjectPositions.Count; o++)
                        {
                            match3Objects.Add(grid.GetObject(matchingObjectPositions[o]));
                        }
                    }
                    matchingObjectPositions.Clear();
                    continue;
                }
                else if (gridObject.Type == Type)
                {
                    matchingObjectPositions.Add(new Vector2Int(x + i, y));
                    continue;
                }
            }

            if (matchingObjectPositions.Count >= 2)
            {
                for (int o = 0; o < matchingObjectPositions.Count; o++)
                {
                    match3Objects.Add(grid.GetObject(matchingObjectPositions[o]));
                }
            }
            #endregion

            matchingObjectPositions.Clear();

            #region Check Y Axis
            for (int i = -2; i < 3; i++)
            {
                gridObject = grid.GetObject(x, y + i);

                if (i == 0)
                {
                    continue;
                }

                if (gridObject == null || gridObject.Type != Type)
                {
                    if (matchingObjectPositions.Count >= 2)
                    {
                        for (int o = 0; o < matchingObjectPositions.Count; o++)
                        {
                            match3Objects.Add(grid.GetObject(matchingObjectPositions[o]));
                        }
                    }
                    matchingObjectPositions.Clear();
                    continue;
                }
                else if (gridObject.Type == Type)
                {
                    matchingObjectPositions.Add(new Vector2Int(x, y + i));
                    continue;
                }
            }

            if (matchingObjectPositions.Count >= 2)
            {
                for (int o = 0; o < matchingObjectPositions.Count; o++)
                {
                    match3Objects.Add(grid.GetObject(matchingObjectPositions[o]));
                }
            }
            #endregion

            return match3Objects.ToArray();
        }
    }
}
