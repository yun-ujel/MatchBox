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
        #endregion

        #region Calculations
        private Vector2 velocity;

        #region Wall Run / Slide Calculations
        private bool isWallrunning;
        private float wallDirectionX;

        private float wallPositionX;

        private bool queuedWallUnstick;
        #endregion


        #endregion
        #endregion

        void Start()
        {
            body = GetComponent<Rigidbody2D>();

            BoxPlayer.OnNavigateEvent += Navigate;
            BoxPlayer.OnLeftClickEvent += LeftClick;

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

            if (!BoxPlayer.OnWall && queuedWallUnstick)
            {
                queuedWallUnstick = false;
            }
        }

        private void FixedUpdate()
        {
            velocity = body.velocity;

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
                if (navigation.y > 0f)
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
            else
            {
                if (isWallrunning)
                {
                    DropOffWall();
                }

                velocity.x = Mathf.MoveTowards
                (
                    velocity.x,
                    navigation.x * maxRunSpeed,
                    GetAcceleration() * Time.fixedDeltaTime
                );
            }

            body.velocity = velocity;
        }

        private void DropOffWall()
        {
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