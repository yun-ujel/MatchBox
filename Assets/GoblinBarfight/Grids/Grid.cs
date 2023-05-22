using UnityEngine;
using Grid = GoblinBarfight.Grids.Grid;

namespace GoblinBarfight.Grids
{
    public class Grid
    {
        public int Width { get; private set; }

        public int Height { get; private set; }

        private float cellSize;
        private Vector3 originPosition;

        private GridObject[,] gridArray;

        public Grid(int width, int height, float cellSize, Vector3 originPosition)
        {
            Width = width;
            Height = height;

            this.originPosition = originPosition;

            this.cellSize = cellSize;

            #region Initialize Grid
            gridArray = new GridObject[width, height];

            for (int x = 0; x < gridArray.GetLength(0); x++)
            {
                for (int y = 0; y < gridArray.GetLength(1); y++)
                {
                    //gridArray[x, y] = new GridObject();
                }
            }
            #endregion

            #region Draw Debug Lines

            for (int x = 0; x < gridArray.GetLength(0); x++)
            {
                for (int y = 0; y < gridArray.GetLength(1); y++)
                {
                    Debug.DrawLine(GridToWorldPosition(x, y), GridToWorldPosition(x, y + 1), Color.white, 100f);
                    Debug.DrawLine(GridToWorldPosition(x, y), GridToWorldPosition(x + 1, y), Color.white, 100f);
                }
            }
            Debug.DrawLine(GridToWorldPosition(0, height), GridToWorldPosition(width, height), Color.white, 100f);
            Debug.DrawLine(GridToWorldPosition(width, 0), GridToWorldPosition(width, height), Color.white, 100f);
            #endregion
        }

        #region Position Conversion Methods
        public Vector3 GridToWorldPosition(int x, int y, bool bottomLeft = false)
        {
            if (bottomLeft)
            {
                return (new Vector3(x, y) * cellSize) + originPosition;
            }

            return (new Vector3(x, y) * cellSize) + originPosition
                - (Vector3)(0.5f * cellSize * Vector2.one);
        }

        public void WorldToGridPosition(Vector3 worldPosition, out int x, out int y)
        {
            worldPosition -= originPosition;
            x = Mathf.FloorToInt(worldPosition.x / cellSize);
            y = Mathf.FloorToInt(worldPosition.y / cellSize);
        }

        public Vector2Int WorldToGridPosition(Vector3 worldPosition)
        {
            WorldToGridPosition(worldPosition, out int x, out int y);
            return new Vector2Int(x, y);
        }
        #endregion
    }
}
