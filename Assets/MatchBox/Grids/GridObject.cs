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

        private GridObjectBehaviour parent;

        public GridObjectType Type { get; private set; }
        public bool IsMatched { get; private set; }

        #endregion
        public GridObject(Grid<GridObject> grid, int x, int y)
        {
            this.grid = grid;
            this.x = x;
            this.y = y;

            GameObject gameObject = Object.Instantiate(Settings.GridObjectPrefab, grid.GridToWorldPosition(x, y, false), Quaternion.identity);
            gameObject.name = $"( {x}, {y} )";

            parent = gameObject.GetComponent<GridObjectBehaviour>();

            SetType(GenerateType(grid, x, y, out bool failed));
            IsMatched = failed;
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
            
            if (FindMatches(x, y, Type, out GridObject[] matches))
            {
                SetMatched(true);
                for (int i = 0; i < matches.Length; i++)
                {
                    matches[i].SetMatched(true);
                }
            }

            this.x = x;
            this.y = y;
        }

        #region Visuals
        public void SetSprite(bool matched)
        {
            if (matched)
            {
                parent.GetComponent<SpriteRenderer>().sprite = Settings.BSprite;
                return;
            }
            parent.GetComponent<SpriteRenderer>().sprite = Settings.ASprite;
        }
        #endregion

        #endregion

        private GridObjectType GenerateType(Grid<GridObject> grid, int x, int y, out bool failedToAvoidMatch)
        {
            failedToAvoidMatch = false;

            List<GridObjectType> validTypes = new List<GridObjectType>(Settings.types);
            bool allTypesValid = true;

            #region Remove Invalid Types
            if (x >= Settings.RequiredObjectsForMatch - 1)
            {
                GridObjectType initialType = grid.GetObject(x - 1, y).Type;
                bool typeValid = false;
                for (int i = 0 - (Settings.RequiredObjectsForMatch - 1); i < 0; i++)
                {
                    if (!grid.GetObject(x + i, y).MatchesType(initialType))
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
                    if (!grid.GetObject(x, y + i).MatchesType(initialType))
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

        public void SetMatched(bool matched)
        {
            IsMatched = matched;
            SetSprite(matched);
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