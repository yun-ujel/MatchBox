using UnityEngine;
using UnityEngine.InputSystem;

public static class MiscUtils
{
    public static Vector3 GetMouseWorldPosition()
    {
        return Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
    }

    public static Vector2 GetWorldSize(this Camera camera)
    {
        // Returns the Camera's Orthographic size converted to world units.

        float screenToWorldHeight = 2 * camera.orthographicSize;
        float screenToWorldWidth = screenToWorldHeight * camera.aspect;

        //Debug.Log("Calculated World Size: " + new Vector2(screenToWorldWidth, screenToWorldHeight));

        return new Vector2(screenToWorldWidth, screenToWorldHeight);
    }
}
