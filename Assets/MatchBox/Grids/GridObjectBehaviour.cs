using UnityEngine;

namespace MatchBox.Grids
{
    [RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
    public class GridObjectBehaviour : MonoBehaviour
    {
        #region Parameters

        private GridObject child;
        private SpriteRenderer spriteRenderer;
        private BoxCollider2D boxCollider;

        #region Moving
        private Vector3 targetPosition;
        private Vector3 velocity;

        private bool isMoving;

        private float smoothTime;
        #endregion

        #endregion

        private void Awake()
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }
            if (boxCollider == null)
            {
                boxCollider = GetComponent<BoxCollider2D>();
            }
        }

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
            child.OnCollapseEvent += Collapse;
        }

        private void Collapse(object sender, GridObject.OnCollapseEventArgs args)
        {
            boxCollider.enabled = args.IsCollapsed && args.IsMatched;
            spriteRenderer.forceRenderingOff = args.IsCollapsed && !args.IsMatched;
        }

        private void OnMove(object sender, GridObject.OnMoveEventArgs args)
        {
            smoothTime = args.SmoothTime;

            gameObject.name = $"( {args.TargetGridPositionX}, {args.TargetGridPositionY} )";
            MoveToPosition(args.TargetWorldPosition);
        }

        private void OnUpdateVisual(object sender, GridObject.OnUpdateVisualEventArgs args)
        {
            spriteRenderer.sprite = args.IsMatched ? args.Type.MatchedSprite : args.Type.DefaultSprite;
            spriteRenderer.color = args.Type.Color;
        }
    }
}