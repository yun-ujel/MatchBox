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

        [SerializeField] private Vector2Int gridPosition;

        public event System.EventHandler<OnMoveEventArgs> OnMoveEvent;
        public class OnMoveEventArgs : System.EventArgs
        {
            public OnMoveEventArgs(Vector2Int start, Vector2Int target, Vector3 world)
            {
                startGridPosition = start;
                targetGridPosition = target;

                targetWorldPosition = world;
            }

            public Vector2Int startGridPosition { get; private set; }
            public Vector2Int targetGridPosition { get; private set; }

            public Vector3 targetWorldPosition { get; private set; }
        }

        #endregion

        #region Selection

        bool isSelected;

        public event System.EventHandler<OnSelectEventArgs> OnSelectEvent;
        public class OnSelectEventArgs : System.EventArgs
        {
            public OnSelectEventArgs(bool selected)
            {
                IsSelected = selected;
            }
            public bool IsSelected { get; private set; }
        }

        #endregion

        #endregion

        private void Start()
        {
            grid = gridDisplay.Grid;
        }

        private void Update()
        {
            timeSinceLastMove += Vector2.one * Time.deltaTime;

            ProcessNavigation();
        }
        
        #region Interaction Methods (Swap / Selection)
        private bool TrySwapObjects(Vector3 worldMouseDownPos, Vector3 worldMouseUpPos)
        {
            Vector2Int mouseDown = grid.WorldToGridPosition(worldMouseDownPos);
            Vector2Int mouseUp = grid.WorldToGridPosition(worldMouseUpPos);

            return TrySwapObjects(mouseDown, mouseUp);
        }

        private bool TrySwapObjects(Vector2Int position1, Vector2Int position2)
        {
            if (position1 != position2 && (position1 - position2).sqrMagnitude == 1)
            {
                return gridDisplay.SwapObjects(position1, position2);
            }

            return false;
        }
        private void SetSelected(bool selected)
        {
            isSelected = selected;

            if (selected)
            {
                OnMoveEvent += SwapOnMove;
            }
            else
            {
                OnMoveEvent -= SwapOnMove;
            }

            OnSelectEvent?.Invoke(this, new OnSelectEventArgs(selected));
        }

        private void SwapOnMove(object sender, OnMoveEventArgs args)
        {
            TrySwapObjects(args.startGridPosition, args.targetGridPosition);
            SetSelected(false);
        }
        #endregion

        #region Navigation Methods
        private void SetGridPosition(Vector2Int targetGridPosition)
        {
            if (targetGridPosition != gridPosition && grid.IsGridPositionInbounds(targetGridPosition))
            {
                Vector3 worldPosition = grid.GridToWorldPosition(targetGridPosition, false);

                OnMoveEvent?.Invoke(this, new OnMoveEventArgs(gridPosition, targetGridPosition, worldPosition));

                gridPosition = targetGridPosition;
                transform.position = worldPosition;
            }
        }

        private void Move(Vector2 input)
        {
            #region Set Target Position
            Vector2Int targetPosition = new Vector2Int
            (
                gridPosition.x + Mathf.RoundToInt(input.x),
                gridPosition.y + Mathf.RoundToInt(input.y)
            );

            Vector2Int gridSize = new Vector2Int(grid.Width, grid.Height);

            for (int axis = 0; axis < 2; axis++)
            {
                if (targetPosition[axis] >= gridSize[axis])
                {
                    targetPosition[axis] = 0;
                }
                else if (targetPosition[axis] < 0)
                {
                    targetPosition[axis] = gridSize[axis] - 1;
                }
            }
            #endregion

            SetGridPosition(targetPosition);
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
        #endregion

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
            if (!ctx.performed) { return; }

            if (ctx.control.IsPressed())
            {
                SetSelected(true);
            }
            else
            {
                SetSelected(false);
            }
        }

        public void Submit(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed) { return; }

            if (ctx.control.IsPressed())
            {
                SetSelected(!isSelected);
            }
        }

        public void Cancel(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed) { return; }

            if (ctx.control.IsPressed())
            {
                SetSelected(false);
            }
        }

        public void Navigate(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed) { return; }

            navigation = ctx.ReadValue<Vector2>();
        }
        #endregion



    }
}
