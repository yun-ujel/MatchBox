using UnityEngine;
using Grids;

public class GridTesting : MonoBehaviour
{
    private Grid<TestGridObject> grid;

    private void Start()
    {
        grid = new Grid<TestGridObject>(4, 2, 1f, new Vector3(-2f, -1f), TestGridObject.create);
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TestGridObject gridObject = grid.GetObject(MiscUtils.GetMouseWorldPosition());
            if (gridObject != null)
            {
                gridObject.SetValue(gridObject.Value + 1);
            }
        }
    }

    public class TestGridObject
    {
        public static System.Func<Grid<TestGridObject>, int, int, TestGridObject> create = (Grid<TestGridObject> g, int x, int y) => new TestGridObject(g, x, y);

        public int Value { get; private set; }

        #region Grid References
        private Grid<TestGridObject> grid;
        private int x;
        private int y;
        #endregion

        public TestGridObject(Grid<TestGridObject> grid, int x, int y)
        {
            this.grid = grid;
            this.x = x;
            this.y = y;
        }

        public void SetValue(int value)
        {
            Value = value;
            grid.TriggerGridObjectChanged(x, y);
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
