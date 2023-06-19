using UnityEngine;
using UnityEngine.InputSystem;

namespace MatchBox.Box.Capabilities
{
    public class PlayerMovement : Capability
    {
        #region Parameters
        private Rigidbody2D body;

        private Vector2 navigation;
        private float timeSinceLastNavigate;

        #region Serialized
        [Header("Ground Movement")]
        [SerializeField] private float maxRunSpeed;

        [Space]

        [SerializeField, Range(0f, 100f)] private float activeGroundAcceleration;
        [SerializeField, Range(0f, 100f)] private float activeAirAcceleration;
        [SerializeField, Range(0f, 100f)] private float passiveDeceleration;

        [Header("Wall Run / Slide")]
        [SerializeField] private float maxWallrunSpeed;
        [SerializeField, Range(0f, 100f)] private float wallrunAcceleration;

        [Space]

        [SerializeField] private float maxWallslideSpeed;
        [SerializeField, Range(0f, 100f)] private float wallslideDeceleration;

        [Header("Jump")]
        [SerializeField] private float jumpForce;
        [SerializeField, Range(0f, 1f)] private float jumpReleaseMultiplier;

        [Space]

        [SerializeField, Range(0f, 1f)] private float jumpBuffer;
        [SerializeField, Range(0f, 1f)] private float coyoteTime;

        [Header("Wall Jump")]
        [SerializeField] private float wallJumpForce;

        [Space]

        [SerializeField, Range(0f, 1f)] private float wallCoyoteTime;
        [SerializeField, Range(0f, 1f)] private float lockedXVelocityDuration;
        #endregion

        #region Calculations
        private Vector2 velocity;

        #region Wall Run / Slide Calculations
        private bool isWallrunning;
        private float wallDirectionX;

        private float wallPositionX;

        private bool queuedWallUnstick;
        #endregion

        #region Horizontal Movement Calculations
        private float desiredVelocityX;
        #endregion

        #region Jump Calculations
        private bool isRisingFromJump;
        private bool submitButtonDown;

        private float jumpBufferLeft;
        private float coyoteTimeLeft;

        private float wallCoyoteTimeLeft;

        private float lockedXVelocityTimeLeft;
        #endregion

        #endregion
        #endregion

        void Start()
        {
            body = GetComponent<Rigidbody2D>();

            BoxPlayer.OnNavigateEvent += Navigate;
            BoxPlayer.OnLeftClickEvent += LeftClick;
            BoxPlayer.OnSubmitEvent += Submit;

            BoxPlayer.OnTouchWallEvent += TouchWall;
        }

        #region Events

        #region Input Events
        private void Navigate(object sender, BoxPlayerHandler.OnInputEventArgs args)
        {
            navigation = args.context.ReadValue<Vector2>();

            if (navigation.y < 0f)
            {
                body.velocity = new Vector2(0f, body.velocity.y);
            }
            timeSinceLastNavigate = 0f;
        }

        private void LeftClick(object sender, BoxPlayerHandler.OnInputEventArgs args)
        {
            if (!args.context.control.IsPressed()) { return; }
            UnstickFromWall();
        }

        private void Submit(object sender, BoxPlayerHandler.OnInputEventArgs args)
        {
            submitButtonDown = args.context.control.IsPressed();

            if (submitButtonDown)
            {
                jumpBufferLeft = jumpBuffer;
            }
        }
        #endregion

        private void TouchWall(object sender, BoxPlayerHandler.OnCollisionEventArgs args)
        {
            wallDirectionX = args.direction.x;

            wallPositionX = args.collision.GetContact(0).point.x;
        }

        #endregion

        private void Update()
        {
            timeSinceLastNavigate += Time.deltaTime;

            jumpBufferLeft -= Time.deltaTime;
            coyoteTimeLeft -= Time.deltaTime;

            wallCoyoteTimeLeft -= Time.deltaTime;
            lockedXVelocityTimeLeft -= Time.deltaTime;

            if (BoxPlayer.OnWall)
            {
                coyoteTimeLeft = 0f;

                if (!isRisingFromJump)
                {
                    wallCoyoteTimeLeft = wallCoyoteTime;
                }
            }
            else if (BoxPlayer.OnGround)
            {
                wallCoyoteTimeLeft = 0f;

                if (!isRisingFromJump)
                {
                    coyoteTimeLeft = coyoteTime;
                }
            }

            if (!BoxPlayer.OnWall && queuedWallUnstick)
            {
                queuedWallUnstick = false;
            }
        }

        private void FixedUpdate()
        {
            velocity = body.velocity;

            if (lockedXVelocityTimeLeft > 0f)
            {
                desiredVelocityX = -wallDirectionX * maxRunSpeed;
            }
            else
            {
                desiredVelocityX = navigation.x * maxRunSpeed;
            }

            #region Wall Interactions
            if (BoxPlayer.OnWall && MovingTowardsWall())
            {
                // Stick To Wall
                if (!queuedWallUnstick)
                {
                    velocity.x = 0f;

                    body.position = new Vector2
                    (
                        wallPositionX + (transform.localScale.x * 0.5f * -wallDirectionX), 
                        body.position.y
                    );
                }

                // Wall Run
                if (navigation.y > 0f && !isRisingFromJump)
                {
                    velocity.y = Mathf.MoveTowards(Mathf.Max(velocity.y, 0f), maxWallrunSpeed, wallrunAcceleration * Time.fixedDeltaTime);
                    isWallrunning = true;
                }
                else if (isWallrunning)
                {
                    DropOffWall();
                }

                // Smooth towards wallslide speed
                if (velocity.y > maxWallslideSpeed)
                {
                    velocity.y = Mathf.MoveTowards(velocity.y, maxWallslideSpeed, wallslideDeceleration * Time.fixedDeltaTime);
                }
                else if (velocity.y < -maxWallslideSpeed)
                {
                    velocity.y = Mathf.MoveTowards(velocity.y, -maxWallslideSpeed, wallslideDeceleration * Time.fixedDeltaTime);
                }
                else
                {
                    velocity.y = Mathf.Clamp(velocity.y, -maxWallslideSpeed, maxWallslideSpeed);
                }
            }
            #endregion
            else
            {
                if (isWallrunning && !isRisingFromJump)
                {
                    DropOffWall();
                }

                #region Horizontal Movement
                velocity.x = Mathf.MoveTowards
                (
                    velocity.x,
                    desiredVelocityX,
                    GetAcceleration() * Time.fixedDeltaTime
                );
                #endregion
            }

            #region Jump
            if (jumpBufferLeft > 0f)
            {
                if (coyoteTimeLeft > 0f)
                {
                    Jump();
                }

                if (wallCoyoteTimeLeft > 0f)
                {
                    WallJump();
                }
            }

            if (body.velocity.y < 0f)
            {
                isRisingFromJump = false;
            }
            else if (isRisingFromJump && !submitButtonDown)
            {
                Debug.Log("Jump Cancelled");
                velocity.y *= jumpReleaseMultiplier;
                isRisingFromJump = false;
            }
            #endregion

            body.velocity = velocity;
        }

        private void Jump()
        {
            Debug.Log("Jump");
            velocity.y = jumpForce;

            isWallrunning = false;
            isRisingFromJump = true;

            coyoteTimeLeft = 0f;
            wallCoyoteTimeLeft = 0f;

            jumpBufferLeft = 0f;
        }

        private void WallJump()
        {
            Debug.Log("Wall Jump");
            velocity.y = wallJumpForce;

            isWallrunning = false;
            isRisingFromJump = true;

            coyoteTimeLeft = 0f;
            wallCoyoteTimeLeft = 0f;

            jumpBufferLeft = 0f;

            lockedXVelocityTimeLeft = lockedXVelocityDuration;

            queuedWallUnstick = true;

            desiredVelocityX = -wallDirectionX * maxRunSpeed;
            velocity.x = desiredVelocityX;
        }

        private void DropOffWall()
        {
            Debug.Log("Drop Off Wall");
            velocity.y *= 0.2f;
            isWallrunning = false;
        }

        private float GetAcceleration()
        {
            if (navigation.x == 0f && timeSinceLastNavigate > 0.1f)
            {
                return passiveDeceleration;
            }
            else
            {
                return BoxPlayer.OnGround || BoxPlayer.OnWall ? activeGroundAcceleration : activeAirAcceleration;
            }
        }

        private bool MovingTowardsWall()
        {
            return (wallDirectionX > 0f && navigation.x > 0f)
                || (wallDirectionX < 0f && navigation.x < 0f)
                || (navigation.x == 0f && wallDirectionX != 0f);
        }

        public void UnstickFromWall()
        {
            if (!BoxPlayer.OnWall) { return; }

            queuedWallUnstick = true;
            body.velocity += new Vector2(-wallDirectionX, 0f);
        }
    }
}