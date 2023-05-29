using UnityEngine;
using Grids;
using System.Collections.Generic;

namespace GoblinBarfight.Grids.Utilities
{
    public static class GridObjectUtils
    {
        public static GridObject[] FindMatchesInAxis(this Grid<GridObject> grid, int axis, int x, int y, GridObjectType type, int requiredForMatch)
        {
            List<GridObject> matchingObjects = new List<GridObject>();
            List<Vector2Int> potentialMatchPositions = new List<Vector2Int>();

            GridObject gridObject;

            for (int i = 0 - (requiredForMatch - 1); i < requiredForMatch; i++)
            {
                gridObject = grid.GetObject(AddToAxis(axis, x, y, i));

                if (!IsGridObjectOfType(gridObject, type))
                {
                    if (potentialMatchPositions.Count >= requiredForMatch)
                    {
                        for (int o = 0; o < potentialMatchPositions.Count; o++)
                        {
                            matchingObjects.Add(grid.GetObject(potentialMatchPositions[o]));
                        }
                    }
                    potentialMatchPositions.Clear();
                    continue;
                }
                else if (IsGridObjectOfType(gridObject, type))
                {
                    potentialMatchPositions.Add(AddToAxis(axis, x, y, i));
                    continue;
                }
            }

            if (potentialMatchPositions.Count >= requiredForMatch)
            {
                for (int o = 0; o < potentialMatchPositions.Count; o++)
                {
                    matchingObjects.Add(grid.GetObject(potentialMatchPositions[o]));
                }
            }

            return matchingObjects.ToArray();
        }

        public static List<GridObjectType> FindTypesThatWouldMatchInAxis(this Grid<GridObject> grid, int axis, int x, int y, int requiredForMatch)
        {
            /*Debug.Log($"Checking for types that would create matches in position ( {x}, {y} ), from axis {axis}...");*/
            List<GridObjectType> typesThatWouldMatch = new List<GridObjectType>();
            List<Vector2Int> potentialMatchPositions = new List<Vector2Int>();

            GridObject gridObject;
            Vector2Int position;
            GridObjectType gridObjectType = null;

            for (int i = 0 - (requiredForMatch - 1); i < requiredForMatch; i++)
            {
                if (i == 0)
                {
                    continue;
                }

                position = AddToAxis(axis, x, y, i);
                gridObject = grid.GetObject(position);

                if (gridObjectType == null)
                {
                    if (gridObject == null)
                    {
                        /*Debug.Log($"{position}: Couldn't find a Grid Object. Skipping...");*/
                        continue;
                    }
                    else
                    {
                        gridObjectType = gridObject.Type;
                        /*Debug.Log($"{position}: Found valid Grid Object. Saving type {gridObjectType.Name}.");*/

                        potentialMatchPositions.Add(position);
                        continue;
                    }
                }
                
                if (IsGridObjectOfType(gridObject, gridObjectType))
                {
                    /*Debug.Log($"{position}: Found Grid Object of matching type {gridObjectType.Name}. Saving.");*/
                    potentialMatchPositions.Add(position);
                    continue;
                }
                else
                {
                    /*Debug.Log($"{position}: Streak of {potentialMatchPositions.Count} broken.");*/
                    if (potentialMatchPositions.Count >= requiredForMatch - 1)
                    {
                        typesThatWouldMatch.Add(gridObjectType);
                    }
                    potentialMatchPositions.Clear();

                    if (gridObject == null)
                    {
                        /*Debug.Log($"{position}: Couldn't find a Grid Object. Skipping...");*/
                        continue;
                    }
                    else
                    {
                        gridObjectType = gridObject.Type;
                        /*Debug.Log($"{position}: Found valid Grid Object. Saving type {gridObjectType.Name}.");*/

                        potentialMatchPositions.Add(position);
                        continue;
                    }
                }
            }

            if (potentialMatchPositions.Count >= requiredForMatch - 1)
            {
                typesThatWouldMatch.Add(gridObjectType);
            }

            return typesThatWouldMatch;
        }

        private static Vector2Int AddToAxis(int axis, int x, int y, int add)
        {
            if (axis == 0)
            {
                return new Vector2Int(x + add, y);
            }
            return new Vector2Int(x, y + add);
        }

        public static bool IsGridObjectOfType(GridObject gridObject, GridObjectType type)
        {
            return gridObject != null && gridObject.Type == type;
        }
        
    }
}