using UnityEngine;
using UnityEngine.InputSystem;

namespace MatchBox.Box.Capabilities
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Jump : Capability
    {
        #region Parameters
        private Rigidbody2D body;
        private bool submitButtonDown;

        #region Serialized
        [Header("Force Values")]
        [SerializeField] private float jumpForce;

        [Space]

        [SerializeField, Range(0f, 1f)] private float jumpReleaseMultiplier;

        [Header("Leniency")]
        [SerializeField, Range(0f, 1f)] private float jumpBuffer;
        [SerializeField, Range(0f, 1f)] private float coyoteTime;

        #endregion

        #region Calculations
        private GravityState currentGravityState;
        private Vector2 velocity;

        private bool isRisingFromJump;

        private float jumpBufferLeft;
        private float coyoteTimeLeft;

        private enum GravityState
        {
            neutral,
            upwards,
            downwards
        }

        #endregion

        #endregion

        private void Start()
        {
            body = GetComponent<Rigidbody2D>();

            BoxPlayer.OnSubmitEvent += ReceiveSubmitInput;
        }

        private void ReceiveSubmitInput(object sender, BoxPlayerHandler.OnInputEventArgs args)
        {
            submitButtonDown = args.context.control.IsPressed();

            if (submitButtonDown)
            {
                jumpBufferLeft = jumpBuffer;
            }
        }

        private void Update()
        {
            coyoteTimeLeft -= Time.deltaTime;
            jumpBufferLeft -= Time.deltaTime;

            if (BoxPlayer.OnGround && !isRisingFromJump)
            {
                coyoteTimeLeft = coyoteTime;
            }
        }

        private void FixedUpdate()
        {
            velocity = body.velocity;

            if (jumpBufferLeft > 0f && coyoteTimeLeft > 0f)
            {
                TriggerJump();
            }

            if (body.velocity.y < 0f)
            {
                isRisingFromJump = false;
            }
            else if (isRisingFromJump && !submitButtonDown)
            {
                velocity.y *= jumpReleaseMultiplier;
                isRisingFromJump = false;
            }

            body.velocity = velocity;
        }

        private void TriggerJump()
        {
            velocity.y = jumpForce;

            isRisingFromJump = true;

            coyoteTimeLeft = 0f;
            jumpBufferLeft = 0f;
        }
    }
}
