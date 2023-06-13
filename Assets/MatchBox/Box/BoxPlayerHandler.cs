using UnityEngine;
using UnityEngine.InputSystem;

namespace MatchBox.Box
{
    using CollisionChecks;
    public class BoxPlayerHandler : MonoBehaviour
    {
        #region Collision Checks
        [SerializeField] private CollisionCheck[] collisionChecks;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            foreach(CollisionCheck collisionCheck in collisionChecks)
            {
                collisionCheck.OnCollisionEnter(collision);
            }
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            foreach (CollisionCheck collisionCheck in collisionChecks)
            {
                collisionCheck.OnCollisionStay(collision);
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            foreach (CollisionCheck collisionCheck in collisionChecks)
            {
                collisionCheck.OnCollisionExit(collision);
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