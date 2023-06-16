using UnityEngine;

namespace MatchBox.Box.Capabilities
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Wallslide : Capability
    {
        #region Parameters
        private Rigidbody2D body;

        private Vector2 navigation;

        #region Serialized
        [Header("Wall Run")]
        [SerializeField] private float maxWallrunSpeed;
        [SerializeField, Range(0f, 100f)] private float wallrunAcceleration;

        [Header("Wall Slide")]
        [SerializeField] private float maxWallslideSpeed;
        #endregion

        #region Calculations
        private Vector2 velocity;

        private float wallDirectionX;

        #endregion
        #endregion

        void Start()
        {
            body = GetComponent<Rigidbody2D>();

            BoxPlayer.OnNavigateEvent += Navigate;
            BoxPlayer.OnTouchWallEvent += TouchWall;
        }

        private void TouchWall(object sender, BoxPlayerHandler.OnCollisionEventArgs args)
        {
            wallDirectionX = args.direction.x;
        }

        private void Navigate(object sender, BoxPlayerHandler.OnInputEventArgs args)
        {
            navigation = args.context.ReadValue<Vector2>();
        }

        private void FixedUpdate()
        {
            if (BoxPlayer.OnWall)
            {
                velocity = body.velocity;
                
                if (MovingTowardsWall())
                {
                    velocity.x = wallDirectionX;
                }

                if (navigation.y > 0f)
                {
                    velocity.y = Mathf.MoveTowards(Mathf.Max(velocity.y, 0f), maxWallslideSpeed, wallrunAcceleration * Time.fixedDeltaTime);
                }
                else
                {
                    velocity.y = Mathf.Max(velocity.y, -maxWallslideSpeed);
                }

                body.velocity = velocity;
            }
        }

        private bool MovingTowardsWall()
        {
            return (wallDirectionX > 0f && navigation.x > 0f)
                || (wallDirectionX < 0f && navigation.x < 0f)
                || (navigation.x == 0f && wallDirectionX != 0f);
        }
    }
}
