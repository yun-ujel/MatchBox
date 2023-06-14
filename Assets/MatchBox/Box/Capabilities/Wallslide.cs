using UnityEngine;

namespace MatchBox.Box.Capabilities
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Wallslide : Capability
    {
        private Rigidbody2D body;
        private Vector2 velocity;

        private Vector2 navigation;

        private float wallDirection;

        [Header("Wall Run")]
        [SerializeField] private float maxWallrunSpeed;
        [SerializeField, Range(0f, 100f)] private float wallrunAcceleration;

        [Header("Wall Slide")]
        [SerializeField] private float maxWallslideSpeed;

        void Start()
        {
            body = GetComponent<Rigidbody2D>();

            BoxPlayer.OnNavigateEvent += Navigate;
            BoxPlayer.OnTouchWallEvent += TouchWall;
        }

        private void TouchWall(object sender, BoxPlayerHandler.OnCollisionEventArgs args)
        {
            wallDirection = args.direction.x;
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
                    velocity.x = wallDirection;
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
            return (wallDirection > 0f && navigation.x > 0f)
                || (wallDirection < 0f && navigation.x < 0f)
                || (navigation.x == 0f && wallDirection != 0f);
        }
    }
}
