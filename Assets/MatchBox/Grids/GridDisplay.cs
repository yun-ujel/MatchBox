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

        public Grid<GridObject> Grid { get; private set; }

        [SerializeField] private GridObjectSettings settings;

        private List<GridObject> matchedGridObjects = new List<GridObject>();
        #endregion

        #region Events
        public class OnMatchFoundEventArgs : System.EventArgs
        {
            public GridObject[] ObjectsInMatch { get; private set; }
            public GridObject[] NewlyMatchedObjects { get; private set; }
            public GridObjectType Type { get; private set; }
            public int XPos { get; private set; }
            public int YPos { get; private set; }

            public OnMatchFoundEventArgs(int x, int y, GridObjectType type, GridObject[] objectsInMatch, GridObject[] newlyMatchedObjects)
            {
                XPos = x;
                YPos = y;
                Type = type;
                ObjectsInMatch = objectsInMatch;
                NewlyMatchedObjects = newlyMatchedObjects;
            }
        }
        public class OnGridCollapseEventArgs : System.EventArgs
        {
            public bool IsCollapsed { get; private set; }

            public OnGridCollapseEventArgs(bool isCollapsed)
            {
                IsCollapsed = isCollapsed;
            }
        }

        public class OnGridObjectRegeneratedEventArgs : System.EventArgs
        {
            public OnGridObjectRegeneratedEventArgs()
            {

            }
        }

        public event System.EventHandler<OnMatchFoundEventArgs> OnMatchFoundEvent;

        public event System.EventHandler<OnGridCollapseEventArgs> OnGridCollapseEvent;

        public event System.EventHandler<OnGridObjectRegeneratedEventArgs> OnGridObjectRegeneratedEvent;

        #endregion

        #endregion

        private void Awake()
        {
            settings.GridParentTransform = transform;

            GridObject.Settings = settings;
            GridObjectUtils.Settings = settings;

            Grid = new Grid<GridObject>(9, 9, 1f, Vector2.one * -4.5f, GridObject.create);
        }

        private void AddToMatchedObjects(GridObject[] add, out GridObject[] newObjects)
        {
            List<GridObject> newObjectsList = new List<GridObject>();

            for (int i = 0; i < add.Length; i++)
            {
                if (matchedGridObjects.Contains(add[i]))
                {
                    continue;
                }
                newObjectsList.Add(add[i]);
                matchedGridObjects.Add(add[i]);
            }

            if (newObjectsList.Count == 0)
            {
                newObjects = null;
                return;
            }

            newObjects = newObjectsList.ToArray();
            return;
        }

        #region Grid Interaction Methods

        #region Singular Grid Objects
        public bool SwapObjects(int xPos1, int yPos1, int xPos2, int yPos2)
        {
            GridObject object1 = Grid.GetObject(xPos1, yPos1);
            GridObject object2 = Grid.GetObject(xPos2, yPos2);

            if ((!object1.IsMatched && !object2.IsMatched) || settings.AllowMovingOfMatchedObjects)
            {
                Grid.SetObject(xPos1, yPos1, object2); /* Put Object 2 in Object 1's Position */
                Grid.SetObject(xPos2, yPos2, object1); /* Put Object 1 in Object 2's Position */

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

                    AddToMatchedObjects(matches, out GridObject[] newMatches);

                    OnMatchFoundEvent?.Invoke(this, new OnMatchFoundEventArgs(x, y, gridObject.Type, matches, newMatches));
                }
            }
        }

        public bool FindMatches(int x, int y, GridObjectType type, out GridObject[] matchingObjects, bool includeObjectAtOrigin = false)
        {
            List<GridObject> matchingObjectsList = new List<GridObject>();
            matchingObjectsList.AddRange(Grid.FindMatchesInAxis(0, x, y, type));
            matchingObjectsList.AddRange(Grid.FindMatchesInAxis(1, x, y, type));

            if (matchingObjectsList.Count > 0)
            {
                if (includeObjectAtOrigin)
                {
                    matchingObjectsList.Add(Grid.GetObject(x, y));
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

        public void RegenerateGridObjects(params GridObject[] gridObjects)
        {
            for (int i = 0; i < gridObjects.Length; i++)
            {
                gridObjects[i]?.Regenerate();
            }

            OnGridObjectRegeneratedEvent?.Invoke(this, new OnGridObjectRegeneratedEventArgs());
        }
        #endregion


        #region Full Grid
        public void RegenerateGrid()
        {
            GridObject[,] gridCopy = new GridObject[Grid.Width, Grid.Height];

            for (int x = 0; x < Grid.Width; x++)
            {
                for (int y = 0; y < Grid.Height; y++)
                {
                    gridCopy[x, y] = Grid.GetObject(x, y);
                    if (!gridCopy[x, y].IsMatched)
                    {
                        Grid.SetObject(x, y, null);
                    }
                }
            }

            for (int x = 0; x < Grid.Width; x++)
            {
                for (int y = 0; y < Grid.Height; y++)
                {
                    if (gridCopy[x, y].IsMatched)
                    {
                        continue;
                    }
                    gridCopy[x, y].Regenerate();
                    Grid.SetObject(x, y, gridCopy[x, y]);
                }
            }

            OnGridObjectRegeneratedEvent?.Invoke(this, new OnGridObjectRegeneratedEventArgs());
        }

        public void CollapseGrid(bool collapsed = true)
        {
            for (int x = 0; x < Grid.Width; x++)
            {
                for (int y = 0; y < Grid.Height; y++)
                {
                    GridObject gridObject = Grid.GetObject(x, y);
                    gridObject.SetParentCollapsed(collapsed);
                }
            }

            OnGridCollapseEvent?.Invoke(this, new OnGridCollapseEventArgs(collapsed));
        }

        public void RestoreGrid()
        {
            RegenerateGrid();
            CollapseGrid(false);
        }

        #endregion
        #endregion
    }

}