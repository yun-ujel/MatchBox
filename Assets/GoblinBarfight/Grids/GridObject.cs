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
            if (x >= Settings.RequiredObjectsForMatch - 1)
            {
                GridObjectType initialType = grid.GetObject(x - 1, y).Type;
                bool typeValid = false;
                for (int i = 0 - (Settings.RequiredObjectsForMatch - 1); i < 0; i++)
                {
                    if (!IsGridObjectOfType(grid.GetObject(x + i, y), initialType))
                    {
                        typeValid = true;
                        break;
                    }
                }
                if (!typeValid)
                {
                    //Debug.Log($"Grid Object at ( {x}, {y} ) has removed {initialType.Name} from Selectable Types");
                    _ = validTypes.Remove(initialType);
                    allTypesValid = false;
                }
            }

            if (y >= Settings.RequiredObjectsForMatch - 1)
            {
                GridObjectType initialType = grid.GetObject(x, y - 1).Type;
                bool typeValid = false;
                for (int i = 0 - (Settings.RequiredObjectsForMatch - 1); i < -1; i++)
                {
                    if (!IsGridObjectOfType(grid.GetObject(x, y + i), initialType))
                    {
                        typeValid = true;
                        break;
                    }
                }
                if (!typeValid)
                {
                    //Debug.Log($"Grid Object at ( {x}, {y} ) has removed {initialType.Name} from Selectable Types");
                    _ = validTypes.Remove(initialType);
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
            //Debug.Log($"Grid Object at ( {x}, {y} ) has selected type {validTypes[selection].Name}");
            return validTypes[selection];
            #endregion
        }

        #region Parent Reference Methods
        private void SetType(GridObjectType type)
        {
            this.Type = type;

            parent.GetComponent<SpriteRenderer>().color = type.Color;
        }

        public void MoveToPosition(int x, int y)
        {
            parent.MoveToPosition(grid.GridToWorldPosition(x, y, false));
            parent.gameObject.name = $"( {x}, {y} )";

            _ = FindMatches(x, y, Type, out GridObject[] matches);
            
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

        #region Utility Methods
        public bool FindMatches(int x, int y, GridObjectType type, out GridObject[] matchingObjects)
        {
            List<GridObject> matchingObjectsList = new List<GridObject>();
            matchingObjectsList.AddRange(FindMatchesInAxis(0, x, y, type));
            matchingObjectsList.AddRange(FindMatchesInAxis(1, x, y, type));

            matchingObjects = matchingObjectsList.ToArray();

            if (matchingObjectsList.Count > 0)
            {
                return true;
            }

            return false;
        }

        private GridObject[] FindMatchesInAxis(int axis, int x, int y, GridObjectType type)
        {
            #region Set Multipliers
            int xMultiplier = 0;
            int yMultiplier = 0;
            if (axis == 0) { xMultiplier = 1; }
            else if (axis == 1) { yMultiplier = 1; }
            #endregion

            List<GridObject> match3Objects = new List<GridObject>();
            List<Vector2Int> matchingObjectPositions = new List<Vector2Int>();

            GridObject gridObject;

            for (int i = 0 - (Settings.RequiredObjectsForMatch - 1); i < Settings.RequiredObjectsForMatch; i++)
            {
                gridObject = grid.GetObject(x + (i * xMultiplier), y + (i * yMultiplier));

                if (!IsGridObjectOfType(gridObject, type))
                {
                    if (matchingObjectPositions.Count >= Settings.RequiredObjectsForMatch)
                    {
                        for (int o = 0; o < matchingObjectPositions.Count; o++)
                        {
                            match3Objects.Add(grid.GetObject(matchingObjectPositions[o]));
                        }
                    }
                    matchingObjectPositions.Clear();
                    continue;
                }
                else if (IsGridObjectOfType(gridObject, type))
                {
                    matchingObjectPositions.Add(new Vector2Int(x + (i * xMultiplier), y + (i * yMultiplier)));
                    continue;
                }
            }

            if (matchingObjectPositions.Count >= Settings.RequiredObjectsForMatch)
            {
                for (int o = 0; o < matchingObjectPositions.Count; o++)
                {
                    match3Objects.Add(grid.GetObject(matchingObjectPositions[o]));
                }
            }

            return match3Objects.ToArray();
        }

        private bool IsGridObjectOfType(GridObject gridObject, GridObjectType type)
        {
            if (gridObject == null)
            {
                return false;
            }
            return gridObject.Type == type;
        }
        #endregion
    }
}
