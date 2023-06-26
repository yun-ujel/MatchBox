using UnityEngine;
using Grids;
using System.Collections.Generic;
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

        #region Events

        public event System.EventHandler<OnMatchFoundEventArgs> OnMatchFoundEvent;

        public class OnMatchFoundEventArgs : System.EventArgs
        {
            public GridObject[] GridObjects { get; private set; }
            public GridObjectType Type { get; private set; }
            public int XPos { get; private set; }
            public int YPos { get; private set; }

            public OnMatchFoundEventArgs(int x, int y, GridObjectType type, GridObject[] gridObjects)
            {
                XPos = x;
                YPos = y;
                Type = type;
                GridObjects = gridObjects;
            }
        }

        #endregion

        #endregion

        private void Start()
        {
            settings.GridParentTransform = transform;

            GridObject.Settings = settings;
            GridObjectUtils.Settings = settings;

            grid = new Grid<GridObject>(9, 9, 1f, Vector2.one * -4.5f, GridObject.create);
            OnSetGridEvent?.Invoke(this, new SetGridEventArgs { grid = grid });
        }

        #region Grid Interaction Methods

        #region Singular Grid Objects
        public bool SwapObjects(int xPos1, int yPos1, int xPos2, int yPos2)
        {
            GridObject object1 = grid.GetObject(xPos1, yPos1);
            GridObject object2 = grid.GetObject(xPos2, yPos2);

            if ((!object1.IsMatched && !object2.IsMatched) || settings.AllowMovingOfMatchedObjects)
            {
                grid.SetObject(xPos1, yPos1, object2); /* Put Object 2 in Object 1's Position */
                grid.SetObject(xPos2, yPos2, object1); /* Put Object 1 in Object 2's Position */

                MoveGridObject(object2, xPos1, yPos1);
                MoveGridObject(object1, xPos2, yPos2);
                
                return true;
            }
            else
            {
                return false;
            }

            void MoveGridObject(GridObject gridObject, int x, int y)
            {
                gridObject.MoveToPosition(x, y);

                if (FindMatches(x, y, gridObject.Type, out GridObject[] matches, true))
                {
                    for (int i = 0; i < matches.Length; i++)
                    {
                        matches[i].SetMatched(true);
                    }

                    OnMatchFoundEvent?.Invoke(this, new OnMatchFoundEventArgs(x, y, gridObject.Type, matches));
                }
            }
        }

        public bool FindMatches(int x, int y, GridObjectType type, out GridObject[] matchingObjects, bool includeObjectAtOrigin = false)
        {
            List<GridObject> matchingObjectsList = new List<GridObject>();
            matchingObjectsList.AddRange(grid.FindMatchesInAxis(0, x, y, type));
            matchingObjectsList.AddRange(grid.FindMatchesInAxis(1, x, y, type));

            if (matchingObjectsList.Count > 0)
            {
                if (includeObjectAtOrigin)
                {
                    matchingObjectsList.Add(grid.GetObject(x, y));
                }

                matchingObjects = matchingObjectsList.ToArray();
                return true;
            }

            matchingObjects = matchingObjectsList.ToArray();
            return false;
        }

        public bool SwapObjects(Vector2Int pos1, Vector2Int pos2)
        {
            return SwapObjects(pos1.x, pos1.y, pos2.x, pos2.y);
        }
        #endregion


        #region Full Grid
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

        #endregion
        #endregion
    }

}