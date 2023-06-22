using UnityEngine;

namespace MatchBox.Box.Capabilities
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class GravityMultiplier : Capability
    {
        private Rigidbody2D body;

        [Header("Gravity Values")]
        [SerializeField] private float upwardsGravityScale;
        [SerializeField] private float downwardsGravityScale;

        [Space]

        [SerializeField] private float defaultGravityScale;

        [Space]

        [SerializeField] private bool useDefaultGravityOnWalls;

        private void Start()
        {
            body = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            if (body.velocity.y == 0f || (useDefaultGravityOnWalls && BoxPlayer.OnWall))
            {
                body.gravityScale = defaultGravityScale;
            }
            else if (body.velocity.y > 0f)
            {
                body.gravityScale = upwardsGravityScale;
            }
            else if (body.velocity.y < 0f)
            {
                body.gravityScale = downwardsGravityScale;
            }
        }
    }
}
