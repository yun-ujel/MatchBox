using UnityEngine;
using UnityEngine.InputSystem;
using Grids;
using UnityEngine.InputSystem.Controls;

namespace MatchBox.Grids
{
    public class GridSelect : MonoBehaviour
    {
        #region Parameters
        private Grid<GridObject> grid;

        [SerializeField] private GridDisplay gridDisplay;

        #region Navigation
        [Header("Navigation")]

        [SerializeField] private float moveRepeatDelay = 0.5f;
        [SerializeField] private float moveRepeatRate = 0.1f;

        private Vector2 timeSinceLastMove;
        private Vector2Int consecutiveMoves;

        private Vector2 navigation;

        private Vector2Int gridPosition;
        #endregion

        #region Display
        [Header("Display")]
        [SerializeField] private float positionSmoothTime;

        private Vector3 targetWorldPosition;
        private Vector3 smoothDampVelocity;
        #endregion

        #endregion

        private void Start()
        {
            gridDisplay.OnSetGridEvent += SetGrid;
        }

        private void SetGrid(object sender, GridDisplay.SetGridEventArgs args)
        {
            grid = args.grid;
        }

        private void TrySwapObjects(Vector3 worldMouseDownPos, Vector3 worldMouseUpPos)
        {
            Vector2Int mouseDown = grid.WorldToGridPosition(worldMouseDownPos);
            Vector2Int mouseUp = grid.WorldToGridPosition(worldMouseUpPos);

            if (mouseDown != mouseUp && (mouseDown - mouseUp).sqrMagnitude == 1)
            {
                gridDisplay.SwapObjects(mouseDown, mouseUp);
            }
        }

        private void SetGridPosition(Vector2Int targetGridPosition)
        {
            if (targetGridPosition != gridPosition && grid.IsGridPositionInbounds(targetGridPosition))
            {
                gridPosition = targetGridPosition;
                targetWorldPosition = grid.GridToWorldPosition(gridPosition, false);
            }
        }

        #region Input Methods
        public void Point(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed) { return; }

            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(ctx.ReadValue<Vector2>());
            Vector2Int targetGridPosition = grid.WorldToGridPosition(mousePosition);
            
            SetGridPosition(targetGridPosition);
        }

        public void LeftClick(InputAction.CallbackContext ctx)
        {
            
        }

        public void Submit(InputAction.CallbackContext ctx)
        {

        }

        public void Cancel(InputAction.CallbackContext ctx)
        {

        }

        public void Navigate(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed) { return; }

            navigation = ctx.ReadValue<Vector2>();
        }
        #endregion

        private void Move(Vector2 input)
        {
            #region Set Target Position
            Vector2Int targetPosition = new Vector2Int
            (
                gridPosition.x + Mathf.RoundToInt(input.x),
                gridPosition.y + Mathf.RoundToInt(input.y)
            );

            Vector2Int gridSize = new Vector2Int(grid.Width, grid.Height);
            bool looped = false;

            for (int axis = 0; axis < 2; axis++)
            {
                if (targetPosition[axis] >= gridSize[axis])
                {
                    targetPosition[axis] = 0;
                    looped = true;
                }
                else if (targetPosition[axis] < 0)
                {
                    targetPosition[axis] = gridSize[axis] - 1;
                    looped = true;
                }
            }
            #endregion

            SetGridPosition(targetPosition);

            if (looped)
            {
                transform.position = targetWorldPosition;
            }
        }

        private void Update()
        {
            timeSinceLastMove += Vector2.one * Time.deltaTime;

            ProcessNavigation();

            transform.position = Vector3.SmoothDamp
            (
                transform.position,
                targetWorldPosition,
                ref smoothDampVelocity,
                positionSmoothTime
            );
        }

        private void ProcessNavigation()
        {
            for (int axis = 0; axis < 2; axis++)
            {
                Vector2 oneAxisNavigation = Vector2.zero;
                oneAxisNavigation[axis] = navigation[axis];

                if (navigation[axis] != 0f)
                {
                    if (consecutiveMoves[axis] == 0)
                    {
                        ProcessMove(axis, oneAxisNavigation);
                    }
                    else if (consecutiveMoves[axis] > 0)
                    {
                        if (timeSinceLastMove[axis] > moveRepeatDelay && consecutiveMoves[axis] == 1)
                        {
                            ProcessMove(axis, oneAxisNavigation);
                        }
                        else if (timeSinceLastMove[axis] > moveRepeatRate && consecutiveMoves[axis] > 1)
                        {
                            ProcessMove(axis, oneAxisNavigation);
                        }
                    }
                }
                else
                {
                    consecutiveMoves[axis] = 0;
                }
            }

            void ProcessMove(int axis, Vector2 oneAxisNavigation)
            {
                Move(oneAxisNavigation);

                timeSinceLastMove[axis] = 0f;
                consecutiveMoves[axis]++;
            }
        }
    }
}
