using UnityEngine;
using UnityEngine.InputSystem;

namespace MatchBox.Box.Capabilities
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Jump))]
    public class WallJump : Capability
    {
        #region Parameters
        private Rigidbody2D body;

        private Vector2 navigation;

        #region Serialized
        [Header("Force Values")]
        [SerializeField] private Vector2 walljumpClimb;
        [SerializeField] private Vector2 walljumpBounce;

        [Header("Leniency")]
        [SerializeField, Range(0f, 1f)] private float jumpBuffer;
        [SerializeField, Range(0f, 1f)] private float coyoteTime;

        #endregion

        #region Calculations
        private float jumpBufferLeft;
        private float coyoteTimeLeft;

        private float wallDirectionX;

        private Vector2 velocity;

        #endregion

        #region Events
        public event System.EventHandler<OnWallJumpEventArgs> OnWallJumpEvent;

        public class OnWallJumpEventArgs : System.EventArgs
        {
            public Vector2 force;

            public OnWallJumpEventArgs(Vector2 force)
            {
                this.force = force;
            }
        }

        #endregion

        #endregion

        void Start()
        {
            body = GetComponent<Rigidbody2D>();

            BoxPlayer.OnSubmitEvent += ReceiveSubmitInput;
            BoxPlayer.OnNavigateEvent += Navigate;

            BoxPlayer.OnTouchWallEvent += TouchWall;
        }

        private void TouchWall(object sender, BoxPlayerHandler.OnCollisionEventArgs args)
        {
            wallDirectionX = args.direction.x;
        }

        private void ReceiveSubmitInput(object sender, BoxPlayerHandler.OnInputEventArgs args)
        {
            if (args.context.control.IsPressed() && !BoxPlayer.OnGround)
            {
                jumpBufferLeft = jumpBuffer;
            }
        }

        private void Navigate(object sender, BoxPlayerHandler.OnInputEventArgs args)
        {
            navigation = args.context.ReadValue<Vector2>();
        }

        private void Update()
        {
            coyoteTimeLeft -= Time.deltaTime;
            jumpBufferLeft -= Time.deltaTime;

            if (BoxPlayer.OnGround)
            {
                jumpBufferLeft = 0f;
                coyoteTimeLeft = 0f;
            }

            if (BoxPlayer.OnWall)
            {
                coyoteTimeLeft = coyoteTime;
            }
        }
        private void FixedUpdate()
        {
            velocity = body.velocity;

            if (jumpBufferLeft > 0f && coyoteTimeLeft > 0f)
            {
                TriggerJump(CalculateJumpForce());
            }

            body.velocity = velocity;
        }

        private Vector2 CalculateJumpForce()
        {
            if (MovingTowardsWall())
            {
                Vector2 climb = walljumpClimb;
                climb.x *= -wallDirectionX;
                
                return climb;
            }
            else
            {
                Vector2 bounce = walljumpBounce;
                bounce.x *= -wallDirectionX;

                return bounce;
            }
        }

        private bool MovingTowardsWall()
        {
            return (wallDirectionX > 0f && navigation.x > 0f)
                || (wallDirectionX < 0f && navigation.x < 0f)
                || (navigation.x == 0f && wallDirectionX != 0f);
        }

        public void TriggerJump(Vector2 force)
        {
            Debug.Log($"Jumping with force {force}");

            velocity.y = force.y;
            velocity.x += force.x;

            coyoteTimeLeft = 0f;
            jumpBufferLeft = 0f;

            OnWallJumpEvent?.Invoke(this, new OnWallJumpEventArgs(force));
        }
    }
}
