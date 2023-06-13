using UnityEngine;

namespace MatchBox.Box.Capabilities
{

    [RequireComponent(typeof(Rigidbody2D))]
    public class Move : Capability
    {
        #region Parameters
        private Rigidbody2D body;

        private Vector2 navigation;

        #region Serialized

        [Header("Speed Values")]
        [SerializeField] private float maxSpeed;

        [Header("Acceleration")]

        [SerializeField, Range(0f, 100f)] private float groundAcceleration;
        [SerializeField, Range(0f, 100f)] private float groundDeceleration;

        [Space]

        [SerializeField, Range(0f, 100f)] private float airAcceleration;
        [SerializeField, Range(0f, 100f)] private float airDeceleration;
        #endregion

        #region Physics Calculation
        private Vector2 velocity;

        private float acceleration;
        private float maxSpeedChange;

        #endregion

        #endregion
        private void Start()
        {
            body = GetComponent<Rigidbody2D>();

            BoxPlayer.OnNavigateEvent += Navigate;
        }

        private void Navigate(object sender, BoxPlayerHandler.OnInputEventArgs args)
        {
            navigation = args.context.ReadValue<Vector2>();
        }

        private void FixedUpdate()
        {
            velocity = body.velocity;

            acceleration = GetAcceleration();

            maxSpeedChange = acceleration * Time.fixedDeltaTime;
            velocity.x = Mathf.MoveTowards(velocity.x, navigation.x * maxSpeed, maxSpeedChange);

            body.velocity = velocity;
        }

        private float GetAcceleration()
        {
            if ((navigation.x < 0f && velocity.x > 0f) || (navigation.x > 0f && velocity.x < 0f))
            {
                return BoxPlayer.OnGround ? groundDeceleration : airDeceleration;
            }
            else if (navigation.x == 0f)
            {
                return BoxPlayer.OnGround ? groundDeceleration : airDeceleration;
            }

            return BoxPlayer.OnGround ? groundAcceleration : groundDeceleration;
        }
    }
}