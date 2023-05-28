using UnityEngine;
using Grids;
using UnityEngine.InputSystem;

namespace GoblinBarfight.Grids
{
    public class GridDisplay : MonoBehaviour
    {
        #region Parameters

        #region Grid
        private Grid<GridObject> grid;
        [SerializeField] private GridObjectSettings settings;
        #endregion

        #region Input
        private Vector3 mouseDownPosition;

        private bool mouseDown;
        #endregion

        #endregion

        private void Start()
        {
            GridObject.Settings = settings;

            grid = new Grid<GridObject>(9, 9, 1f, Vector2.one * -4.5f, GridObject.create);
        }
        private void Update()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame && !Mouse.current.leftButton.wasReleasedThisFrame)
            {
                mouseDown = true;
                mouseDownPosition = MiscUtils.GetMouseWorldPosition();
            }

            if (mouseDown && Mouse.current.leftButton.wasReleasedThisFrame)
            {
                TriggerMouseReleased(MiscUtils.GetMouseWorldPosition());
            }
        }

        private void TriggerMouseReleased(Vector3 mouseUpPosition)
        {
            Vector2Int mouseDown = grid.WorldToGridPosition(mouseDownPosition);
            Vector2Int mouseUp = grid.WorldToGridPosition(mouseUpPosition);

            if (mouseDown != mouseUp && (mouseDown - mouseUp).sqrMagnitude == 1)
            {
                SwapObjects(mouseDown, mouseUp);
            }
        }

        private void SwapObjects(int xPos1, int yPos1, int xPos2, int yPos2)
        {
            GridObject object1 = grid.GetObject(xPos1, yPos1);
            GridObject object2 = grid.GetObject(xPos2, yPos2);

            grid.SetObject(xPos1, yPos1, object2); /* Put Object 2 in Object 1's Position */
            grid.SetObject(xPos2, yPos2, object1); /* Put Object 1 in Object 2's Position */

            object2.MoveToPosition(xPos1, yPos1);
            object1.MoveToPosition(xPos2, yPos2);
        }

        private void SwapObjects(Vector2Int pos1, Vector2Int pos2)
        {
            SwapObjects(pos1.x, pos1.y, pos2.x, pos2.y);
        }
    }

}