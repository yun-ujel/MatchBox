using UnityEngine;
using UnityEngine.InputSystem;

public static class MiscUtils
{
    public static Vector3 GetMouseWorldPosition()
    {
        return Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
    }
}
