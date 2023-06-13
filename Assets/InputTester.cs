using UnityEngine;
using UnityEngine.InputSystem;

public class InputTester : MonoBehaviour
{
    private Vector2 inputMovement;

    private void FixedUpdate()
    {
        transform.position += (Vector3)(inputMovement * 2f * Time.fixedDeltaTime);
    }

    public void Move(InputAction.CallbackContext ctx)
    {
        inputMovement = ctx.ReadValue<Vector2>();
    }

    public void Jump(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) { return; }

        Debug.Log("Jump");
    }
}
