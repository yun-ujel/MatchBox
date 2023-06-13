using UnityEngine;
using Grids;
using System.Collections.Generic;

namespace MatchBox.Grids.Utilities
{
    public static class GridObjectUtils
    {
        public static GridObjectSettings Settings { get; set; }

        public static GridObject[] FindMatchesInAxis(this Grid<GridObject> grid, int axis, int x, int y, GridObjectType type)
        {
            List<GridObject> matchingObjects = new List<GridObject>();
            List<Vector2Int> potentialMatchPositions = new List<Vector2Int>();

            GridObject gridObject;

            Vector2Int originPosition = new Vector2Int(x, y);
            Vector2Int checkedPosition = originPosition;

            for (int i = 0 - (Settings.RequiredObjectsForMatch - 1); i < Settings.RequiredObjectsForMatch; i++)
            {
                if (i == 0) { continue; }

                checkedPosition[axis] = originPosition[axis] + i;

                gridObject = grid.GetObject(checkedPosition);

                if (!gridObject.MatchesType(type))
                {
                    EvaluateMatches();
                    continue;
                }
                else if (gridObject.MatchesType(type))
                {
                    potentialMatchPositions.Add(checkedPosition);
                    continue;
                }
            }

            EvaluateMatches();

            void EvaluateMatches()
            {
                if (potentialMatchPositions.Count >= Settings.RequiredObjectsForMatch - 1)
                {
                    for (int o = 0; o < potentialMatchPositions.Count; o++)
                    {
                        matchingObjects.Add(grid.GetObject(potentialMatchPositions[o]));
                    }
                }
                potentialMatchPositions.Clear();
            }

            return matchingObjects.ToArray();
        }

        public static List<GridObjectType> FindTypesThatWouldMatchInAxis(this Grid<GridObject> grid, int axis, int x, int y)
        {
            /*Debug.Log($"Checking for types that would create matches in position ( {x}, {y} ), from axis {axis}...");*/
            List<GridObjectType> typesThatWouldMatch = new List<GridObjectType>();
            List<Vector2Int> potentialMatchPositions = new List<Vector2Int>();

            GridObject gridObject;

            Vector2Int originPosition = new Vector2Int(x, y);
            Vector2Int checkedPosition = originPosition;

            GridObjectType gridObjectType = null;

            for (int i = 0 - (Settings.RequiredObjectsForMatch - 1); i < Settings.RequiredObjectsForMatch; i++)
            {
                if (i == 0) { continue; }

                checkedPosition[axis] = originPosition[axis] + i;
                gridObject = grid.GetObject(checkedPosition);

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

                        potentialMatchPositions.Add(checkedPosition);
                        continue;
                    }
                }
                
                if (gridObject.MatchesType(gridObjectType))
                {
                    /*Debug.Log($"{position}: Found Grid Object of matching type {gridObjectType.Name}. Saving.");*/
                    potentialMatchPositions.Add(checkedPosition);
                    continue;
                }
                else
                {
                    /*Debug.Log($"{position}: Streak of {potentialMatchPositions.Count} broken.");*/
                    EvaluateMatches();

                    if (gridObject == null)
                    {
                        /*Debug.Log($"{position}: Couldn't find a Grid Object. Skipping...");*/
                        continue;
                    }
                    else
                    {
                        gridObjectType = gridObject.Type;
                        /*Debug.Log($"{position}: Found valid Grid Object. Saving type {gridObjectType.Name}.");*/

                        potentialMatchPositions.Add(checkedPosition);
                        continue;
                    }
                }
            }

            EvaluateMatches();
            void EvaluateMatches()
            {
                if (potentialMatchPositions.Count >= Settings.RequiredObjectsForMatch - 1)
                {
                    typesThatWouldMatch.Add(gridObjectType);
                }
                potentialMatchPositions.Clear();
            }

            return typesThatWouldMatch;
        }

        public static bool MatchesType(this GridObject gridObject, GridObjectType type)
        {
            if (!Settings.AllowMatchingWithMatchedObjects)
            {
                return gridObject != null && !gridObject.IsMatched && gridObject.Type == type;
            }

            return gridObject != null && gridObject.Type == type;
        }
        
    }
}