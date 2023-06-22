using UnityEngine;
using UnityEngine.InputSystem;

namespace MatchBox.Box.Capabilities
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Smash : Capability
    {
        private Vector3 worldPointerPosition;

        private Vector3 lastSmashCenter;
        private Vector2 smashDirection;

        private Rigidbody2D body;

        [SerializeField] private float smashDistance;
        [SerializeField] private float smashRadius;

        [Space]

        [SerializeField] private float smashStrength;

        private void Start()
        {
            body = GetComponent<Rigidbody2D>();

            BoxPlayer.OnPointEvent += OnPoint;
            BoxPlayer.OnLeftClickEvent += OnLeftClick;
        }

        private void OnLeftClick(object sender, BoxPlayerHandler.OnInputEventArgs args)
        {
            if (!args.context.control.IsPressed()) { return; }

            smashDirection = (worldPointerPosition - transform.position).normalized;
            lastSmashCenter = transform.position + (Vector3)(smashDirection * smashDistance);

            body.velocity -= smashDirection * smashStrength;
        }

        private void Update()
        {

        }

        private void OnDrawGizmos()
        {
            UnityEditor.Handles.DrawWireDisc(lastSmashCenter, Vector3.forward, smashRadius);
        }

        private void OnPoint(object sender, BoxPlayerHandler.OnInputEventArgs args)
        {
            worldPointerPosition = Camera.main.ScreenToWorldPoint(args.context.ReadValue<Vector2>());
            worldPointerPosition.z = 0f;
        }
    }

}