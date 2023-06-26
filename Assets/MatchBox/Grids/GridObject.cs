using UnityEngine;
using Grids;
using System.Collections.Generic;
using System.Linq;

namespace MatchBox.Grids
{
    using Utilities;
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

        public GridObjectType Type { get; private set; }
        public bool IsMatched { get; private set; }

        #region Events

        public System.EventHandler<OnUpdateVisualEventArgs> OnUpdateVisualEvent;
        public System.EventHandler<OnMoveEventArgs> OnMoveEvent;

        public class OnUpdateVisualEventArgs : System.EventArgs
        {
            public GridObjectType Type { get; private set; }
            public bool IsMatched { get; private set; }

            public OnUpdateVisualEventArgs(GridObjectType type, bool isMatched)
            {
                Type = type;
                IsMatched = isMatched;
            }
        }

        public class OnMoveEventArgs : System.EventArgs
        {
            public Vector3 TargetWorldPosition { get; private set; }

            public int TargetGridPositionX { get; private set; }
            public int TargetGridPositionY { get; private set; }

            public float SmoothTime { get; private set; }

            public OnMoveEventArgs(Vector3 targetWorldPosition, int x, int y, float smoothTime = 0.04f)
            {
                TargetWorldPosition = targetWorldPosition;

                TargetGridPositionX = x;
                TargetGridPositionY = y;

                SmoothTime = smoothTime;
            }
        }
        #endregion

        #endregion
        public GridObject(Grid<GridObject> grid, int x, int y)
        {
            this.grid = grid;
            this.x = x;
            this.y = y;

            GameObject gameObject = Object.Instantiate(Settings.GridObjectPrefab, Vector3.zero, Quaternion.identity);

            if (Settings.GridParentTransform != null)
            {
                gameObject.transform.parent = Settings.GridParentTransform;
            }
            gameObject.GetComponent<GridObjectBehaviour>().SetChild(this);

            SetType(GenerateType(grid, x, y, out bool failed));
            IsMatched = failed;

            OnMoveEvent?.Invoke(this, new OnMoveEventArgs(grid.GridToWorldPosition(x, y, false), x, y));
            OnUpdateVisualEvent?.Invoke(this, new OnUpdateVisualEventArgs(Type, IsMatched));
        }

        public override string ToString()
        {
            string matched = IsMatched ? ", Matched" : ", Unmatched";
            return $"( {x}, {y} ), Type: {Type.Name}" + matched;
        }

        #region Event Triggering Methods
        private void SetType(GridObjectType type)
        {
            Type = type;

            OnUpdateVisualEvent?.Invoke(this, new OnUpdateVisualEventArgs(Type, IsMatched));
        }

        public void SetMatched(bool matched)
        {
            IsMatched = matched;
            OnUpdateVisualEvent?.Invoke(this, new OnUpdateVisualEventArgs(Type, IsMatched));
        }

        public void MoveVisual(Vector3 position, float smoothTime = 0.04f)
        {
            OnMoveEvent?.Invoke(this, new OnMoveEventArgs(position, x, y, smoothTime));
        }

        public void MoveToPosition(int x, int y)
        {
            OnMoveEvent?.Invoke(this, new OnMoveEventArgs(grid.GridToWorldPosition(x, y, false), x, y));

            this.x = x;
            this.y = y;
        }

        #endregion

        private GridObjectType GenerateType(Grid<GridObject> grid, int x, int y, out bool failedToAvoidMatch)
        {
            failedToAvoidMatch = false;

            List<GridObjectType> validTypes = new List<GridObjectType>(Settings.types);
            bool allTypesValid = true;

            #region Remove Invalid Types
            Vector2Int originPosition = new Vector2Int(x, y);

            for (int axis = 0; axis < 2; axis++)
            {
                if (originPosition[axis] >= Settings.RequiredObjectsForMatch - 1)
                {
                    Vector2Int checkedPosition = new Vector2Int(x, y);
                    checkedPosition[axis] -= 1;

                    GridObjectType initialType = grid.GetObject(checkedPosition).Type;
                    bool initialTypeCreatesMatch = true;

                    for (int i = 0 - (Settings.RequiredObjectsForMatch - 1); i < -1; i++)
                    {
                        checkedPosition[axis] = originPosition[axis] + i;
                        if (!grid.GetObject(checkedPosition).MatchesType(initialType))
                        {
                            initialTypeCreatesMatch = false;
                            break;
                        }
                    }
                    if (initialTypeCreatesMatch)
                    {
                        //Debug.Log($"Grid Object at ( {x}, {y} ) has removed {initialType.Name} from Selectable Types");
                        _ = validTypes.Remove(initialType);
                        allTypesValid = false;
                    }
                }
            }
            #endregion

            #region Select Type
            if (validTypes.Count < 1)
            {
                failedToAvoidMatch = true;
                return Settings.types[0];
            }

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

        public bool FindMatches(int x, int y, GridObjectType type, out GridObject[] matchingObjects)
        {
            List<GridObject> matchingObjectsList = new List<GridObject>();
            matchingObjectsList.AddRange(grid.FindMatchesInAxis(0, x, y, type));
            matchingObjectsList.AddRange(grid.FindMatchesInAxis(1, x, y, type));

            matchingObjects = matchingObjectsList.ToArray();

            if (matchingObjectsList.Count > 0)
            {
                return true;
            }

            return false;
        }

        public void Regenerate()
        {
            #region Calculate Type

            #region Remove Types That Would Match
            List<GridObjectType> matchingTypes = grid.FindTypesThatWouldMatchInAxis(0, x, y);
            matchingTypes.AddRange(grid.FindTypesThatWouldMatchInAxis(1, x, y));

            GridObjectType[] validTypes = Settings.types.Except(matchingTypes).ToArray();
            #region Select Type
            if (validTypes.Length < 1)
            {
                SetType(Settings.types[0]);
                Debug.Log($"( {x}, {y} ): Failed to find a type that wouldn't match");
            }

            int selection = Random.Range(0, validTypes.Length);

            SetType(validTypes[selection]);
            #endregion

            #endregion

            #endregion
        }
    }
}
