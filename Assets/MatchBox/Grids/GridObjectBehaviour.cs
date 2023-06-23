using UnityEngine;

namespace MatchBox.Grids
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class GridObjectBehaviour : MonoBehaviour
    {
        #region Parameters

        private GridObject child;
        private SpriteRenderer spriteRenderer;

        #region Moving
        private Vector3 targetPosition;
        private Vector3 velocity;

        private bool isMoving;

        private float smoothTime;
        #endregion

        #endregion

        private void FixedUpdate()
        {
            if (isMoving)
            {
                if (transform.position != targetPosition)
                {
                    transform.position = Vector3.SmoothDamp
                    (
                        transform.position,
                        targetPosition,
                        ref velocity,
                        smoothTime,
                        Mathf.Infinity,
                        Time.fixedDeltaTime
                    );
                }
                else
                {
                    isMoving = false;
                }
            }
        }

        private void MoveToPosition(Vector3 position)
        {
            targetPosition = position;
            isMoving = true;
        }

        public void SetChild(GridObject child)
        {
            this.child = child;

            child.OnMoveEvent += OnMove;
            child.OnUpdateVisualEvent += OnUpdateVisual;
        }

        private void OnMove(object sender, GridObject.OnMoveEventArgs args)
        {
            smoothTime = args.SmoothTime;

            gameObject.name = $"( {args.TargetGridPositionX}, {args.TargetGridPositionY} )";
            MoveToPosition(args.TargetWorldPosition);
        }

        private void OnUpdateVisual(object sender, GridObject.OnUpdateVisualEventArgs args)
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }

            spriteRenderer.sprite = args.IsMatched ? args.Type.MatchedSprite : args.Type.DefaultSprite;
            spriteRenderer.color = args.Type.Color;
        }
    }
}