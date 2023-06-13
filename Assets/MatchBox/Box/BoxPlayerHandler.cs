using UnityEngine;
using UnityEngine.InputSystem;

namespace MatchBox.Box
{
    public class BoxPlayerHandler : MonoBehaviour
    {
        #region Collision Checks
        [Header("Collision Checking")]
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private CollisionCheckValues collisionCheckValues;

        [field: SerializeField] public bool OnGround { get; private set; }
        [field: SerializeField] public bool OnWall { get; private set; }

        private Vector2 wallDirection;

        public event System.EventHandler<OnCollisionEventArgs> OnTouchGroundEvent;
        public event System.EventHandler<OnCollisionEventArgs> OnTouchWallEvent;

        #region Classes
        [System.Serializable]
        private class CollisionCheckValues
        {
            [Header("Ground")]
            [SerializeField, Range(0f, 1f)] private float minGroundNormalY = 0.9f;
            [SerializeField, Range(0f, 1f)] private float maxGroundNormalY = 1f;

            public bool IsGround(float normalY)
            {
                return normalY >= minGroundNormalY && normalY <= maxGroundNormalY;
            }

            [Header("Wall")]
            [SerializeField, Range(0f, 1f)] private float minWallNormalX = 0.9f;
            [SerializeField, Range(0f, 1f)] private float maxWallNormalX = 1f;

            public bool IsWall(float normalX)
            {
                float abs = Mathf.Abs(normalX);
                return abs >= minWallNormalX && abs <= maxWallNormalX;
            }
        }
        public class OnCollisionEventArgs : System.EventArgs
        {
            public Vector2 direction;

            public OnCollisionEventArgs(Vector2 direction)
            {
                this.direction = direction;
            }
        }
        #endregion

        private void OnCollisionEnter2D(Collision2D collision)
        {
            bool wasOnGround = OnGround;
            bool wasOnWall = OnWall;

            EvaluateCollision(collision);
            
            if (OnGround && !wasOnGround)
            {
                OnTouchGroundEvent?.Invoke(this, new OnCollisionEventArgs(Vector2.down));
            }
            if (OnWall && !wasOnWall)
            {
                OnTouchWallEvent?.Invoke(this, new OnCollisionEventArgs(wallDirection));
            }
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            EvaluateCollision(collision);
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            OnGround = false;
            OnWall = false;
        }

        private void EvaluateCollision(Collision2D collision)
        {
            int contactsOnLeft = 0;
            int contactsOnRight = 0;

            for (int i = 0; i < collision.contactCount; i++)
            {
                Vector2 contactNormal = collision.GetContact(i).normal;
                OnGround |= collisionCheckValues.IsGround(contactNormal.y);
                OnWall |= collisionCheckValues.IsWall(contactNormal.x);

                Vector2 direction = collision.GetContact(i).point - (Vector2)collision.otherCollider.transform.position;

                if (direction.x > 0)
                {
                    contactsOnRight++;
                }
                else if (direction.x < 0)
                {
                    contactsOnLeft++;
                }
            }
            
            if (contactsOnLeft < contactsOnRight)
            {
                wallDirection.x = 1f;
            }
            else if (contactsOnLeft > contactsOnRight)
            {
                wallDirection.x = -1f;
            }
            else
            {
                wallDirection.x = 0f;
            }
        }
        #endregion

        #region Input
        public class OnInputEventArgs : System.EventArgs
        {
            public InputAction.CallbackContext context;

            public OnInputEventArgs(InputAction.CallbackContext ctx)
            {
                context = ctx;
            }
        }

        public event System.EventHandler<OnInputEventArgs> OnLeftClickEvent;
        public event System.EventHandler<OnInputEventArgs> OnPointEvent;
        public event System.EventHandler<OnInputEventArgs> OnSubmitEvent;
        public event System.EventHandler<OnInputEventArgs> OnCancelEvent;
        public event System.EventHandler<OnInputEventArgs> OnNavigateEvent;

        public void LeftClick(InputAction.CallbackContext ctx)
        {
            TriggerInputEvent(ctx, OnLeftClickEvent);
        }

        public void Point(InputAction.CallbackContext ctx)
        {
            TriggerInputEvent(ctx, OnPointEvent);
        }

        public void Submit(InputAction.CallbackContext ctx)
        {
            TriggerInputEvent(ctx, OnSubmitEvent);
        }

        public void Cancel(InputAction.CallbackContext ctx)
        {
            TriggerInputEvent(ctx, OnCancelEvent);
        }

        public void Navigate(InputAction.CallbackContext ctx)
        {
            TriggerInputEvent(ctx, OnNavigateEvent);
        }

        private void TriggerInputEvent(InputAction.CallbackContext ctx, System.EventHandler<OnInputEventArgs> Event)
        {
            if (!ctx.performed) { return; }
            Event?.Invoke(this, new OnInputEventArgs(ctx));
        }
        #endregion
    }
}