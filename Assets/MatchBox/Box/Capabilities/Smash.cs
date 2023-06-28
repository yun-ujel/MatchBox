using UnityEngine;
using UnityEngine.InputSystem;
using Grids;
using System.Collections.Generic;

namespace MatchBox.Box.Capabilities
{
    using Grids;

    [RequireComponent(typeof(Rigidbody2D))]
    public class Smash : Capability
    {
        #region Parameters
        private Grid<GridObject> grid;

        private Vector3 worldPointerPosition;
        private Vector2 lastSmashCenter;

        private Vector2 smashDirection;

        private Rigidbody2D body;

        #region Serialized
        [SerializeField] private GridDisplay gridDisplay;

        [Header("Smash Values")]
        [SerializeField] private float smashDistance;
        [SerializeField] private float smashRadius;

        [Space]

        [SerializeField] private float smashStrength;

        #endregion
        #endregion

        private void Start()
        {
            body = GetComponent<Rigidbody2D>();

            BoxPlayer.OnPointEvent += OnPoint;
            BoxPlayer.OnLeftClickEvent += OnLeftClick;
        }

        #region Input Events
        private void OnLeftClick(object sender, BoxPlayerHandler.OnInputEventArgs args)
        {
            if (!args.context.control.IsPressed()) { return; }

            DoSmash();
        }

        private void OnPoint(object sender, BoxPlayerHandler.OnInputEventArgs args)
        {
            worldPointerPosition = Camera.main.ScreenToWorldPoint(args.context.ReadValue<Vector2>());
            worldPointerPosition.z = 0f;
        }
        #endregion

        private void DoSmash()
        {
            smashDirection = (worldPointerPosition - transform.position).normalized;
            lastSmashCenter = (Vector2)transform.position + (smashDirection * smashDistance);

            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(lastSmashCenter, smashRadius);

            if (hitColliders != null && hitColliders.Length > 0)
            {
                body.velocity -= smashDirection * smashStrength;

                GridObject[] hitObjects = GetGridObjectsHit();

                if (hitObjects != null)
                {
                    for (int i = 0; i < hitObjects.Length; i++)
                    {
                        hitObjects[i].Regenerate();
                    }
                }
            }
        }

        private GridObject[] GetGridObjectsHit()
        {
            if (grid == null)
            {
                grid = gridDisplay.Grid;
            }

            Vector2 smashWorldPosMax = lastSmashCenter + (Vector2.one * smashRadius);
            Vector2 smashWorldPosMin = lastSmashCenter + (Vector2.one * -smashRadius);

            Debug.Log($"World Smash Pos Max X: {smashWorldPosMax.x} / {grid.CellSize} = {smashWorldPosMax.x / grid.CellSize}");
            Debug.Log($"World Smash Pos Max X: {smashWorldPosMax.y} / {grid.CellSize} = {smashWorldPosMax.y / grid.CellSize}");

            Debug.DrawLine(smashWorldPosMin, smashWorldPosMax, Color.red, 1f);

            Vector2Int smashGridPosMax = grid.WorldToGridPosition(smashWorldPosMax);
            Vector2Int smashGridPosMin = grid.WorldToGridPosition(smashWorldPosMin);

            if (grid.GetObject(smashWorldPosMax) != null || grid.GetObject(smashWorldPosMin) != null)
            {
                List<GridObject> hitObjects = new List<GridObject>();

                for (int x = smashGridPosMin.x; x <= smashGridPosMax.x; x++)
                {
                    for (int y = smashGridPosMin.y; y < smashGridPosMax.y; y++)
                    {
                        Debug.Log($"Hitting Grid Object at ( {x}, {y} )");

                        GridObject gridObject = grid.GetObject(x, y);

                        if (gridObject == null)
                        {
                            Debug.Log($"Grid Object at ( {x}, {y} ) doesn't exist. continuing...");

                            continue;
                        }

                        if (gridObject != null && gridObject.IsMatched)
                        {
                            Debug.Log($"Grid Object at ( {x}, {y} ) successfully hit");

                            hitObjects.Add(gridObject);
                        }
                        else
                        {
                            Debug.Log($"Grid Object at ( {x}, {y} ) was not matched. continuing...");
                        }
                    }
                }

                return hitObjects.ToArray();
            }
            else
            {
                Debug.Log("No valid objects hit");

                return null;
            }
        }

        #region Debug
        private void OnDrawGizmos()
        {
            UnityEditor.Handles.DrawWireDisc(lastSmashCenter, Vector3.forward, smashRadius);
        }
        #endregion
    }
}