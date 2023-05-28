using UnityEngine;
using TMPro;

namespace Grids
{
    using Utilities;

    public class Grid<TGridObject>
    {
        public event System.EventHandler<OnGridValueChangedEventArgs> OnGridValueChanged;
        public class OnGridValueChangedEventArgs : System.EventArgs
        {
            public int x;
            public int y;
        }

        public int Width { get; private set; }
        public int Height { get; private set; }

        private Vector3 originPosition;
        private float cellSize;

        private TGridObject[,] gridArray;

        private TextMeshPro[,] debugTextArray;
        private bool drawDebugTextArray;

        public Grid(int width, int height, float cellSize, Vector3 originPosition, System.Func<Grid<TGridObject>, int, int, TGridObject> createGridObject)
        {
            Width = width;
            Height = height;

            this.originPosition = originPosition;
            this.cellSize = cellSize;

            #region Initialize Grid
            gridArray = new TGridObject[width, height];

            for (int x = 0; x < gridArray.GetLength(0); x++)
            {
                for (int y = 0; y < gridArray.GetLength(1); y++)
                {
                    gridArray[x, y] = createGridObject(this, x, y);
                }
            }
            #endregion

            #region Draw Debug Text Array
            if (drawDebugTextArray)
            {
                debugTextArray = new TextMeshPro[width, height];
            }

            for (int x = 0; x < gridArray.GetLength(0); x++)
            {
                for (int y = 0; y < gridArray.GetLength(1); y++)
                {
                    if (drawDebugTextArray)
                    {
                        debugTextArray[x, y] = GridUtils.CreateWorldText(gridArray[x, y]?.ToString(), null, GridToWorldPosition(x, y) + (Vector3)(0.5f * cellSize * Vector2.one));
                    }
                    Debug.DrawLine(GridToWorldPosition(x, y), GridToWorldPosition(x, y + 1), Color.white, 100f);
                    Debug.DrawLine(GridToWorldPosition(x, y), GridToWorldPosition(x + 1, y), Color.white, 100f);
                }
            }
            Debug.DrawLine(GridToWorldPosition(0, height), GridToWorldPosition(width, height), Color.white, 100f);
            Debug.DrawLine(GridToWorldPosition(width, 0), GridToWorldPosition(width, height), Color.white, 100f);

            if (drawDebugTextArray)
            {
                OnGridValueChanged += (object sender, OnGridValueChangedEventArgs eventArgs) =>
                {
                    debugTextArray[eventArgs.x, eventArgs.y].text = gridArray[eventArgs.x, eventArgs.y]?.ToString();
                };
            }
            #endregion
        }

        #region Position Conversion Methods
        public Vector3 GridToWorldPosition(int x, int y, bool bottomLeft = true)
        {
            if (bottomLeft)
            {
                return (new Vector3(x, y) * cellSize) + originPosition;
            }
            return (new Vector3(x, y) * cellSize) + originPosition + (Vector3)(0.5f * cellSize * Vector2.one);
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

        #region Set Methods
        public void SetObject(int x, int y, TGridObject value)
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height)
            {
                gridArray[x, y] = value;
            }

            if (OnGridValueChanged != null) { OnGridValueChanged(this, new OnGridValueChangedEventArgs { x = x, y = y }); }
        }

        public void SetObject(Vector3 worldPosition, TGridObject value)
        {
            int x, y;
            WorldToGridPosition(worldPosition, out x, out y);

            SetObject(x, y, value);
        }
        #endregion

        #region Get Methods
        public TGridObject GetObject(int x, int y)
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height)
            {
                return gridArray[x, y];
            }

            return default;
        }

        public TGridObject GetObject(Vector2Int position)
        {
            return GetObject(position.x, position.y);
        }

        public TGridObject GetObject(Vector3 worldPosition)
        {
            int x, y;
            WorldToGridPosition(worldPosition, out x, out y);
            return GetObject(x, y);
        }
        #endregion

        public void TriggerGridObjectChanged(int x, int y)
        {
            if (OnGridValueChanged != null) { OnGridValueChanged(this, new OnGridValueChangedEventArgs { x = x, y = y }); }
        }
    }
}
