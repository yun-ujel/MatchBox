using UnityEngine;

namespace MatchBox.Box.CollisionChecks
{
    [CreateAssetMenu(fileName = "Ground Check", menuName = "Scriptable Object/Collision Check/Ground Check")]
    public class GroundCheck : CollisionCheck
    {
        public bool OnGround { get; private set; }

        [SerializeField,Range(0f, 1f)] private float minGroundNormalY = 0.9f;
        [Space]
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float raycastDistance = 1f;

        public override void OnCollisionEnter(Collision2D collision)
        {
            EvaluateCollision(collision);
        }

        public override void OnCollisionStay(Collision2D collision)
        {
            EvaluateCollision(collision);
        }

        public override void OnCollisionExit(Collision2D collision)
        {
            OnGround = false;
        }

        private bool EvaluateCollision(Collision2D collision)
        {
            RaycastHit2D hit = Physics2D.Raycast
            (
                collision.otherCollider.transform.position,
                Vector2.down,
                raycastDistance,
                groundLayer
            );

            return hit.normal.y >= minGroundNormalY;
        }
    }
}
