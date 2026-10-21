using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

// Mouse ve dokunmatik ekranı tek yerden okur (yeni ve eski giriş sistemiyle çalışır).
public static class GameInput
{
#if ENABLE_INPUT_SYSTEM
    static Pointer P => Pointer.current;
    public static bool PressedThisFrame => P != null && P.press.wasPressedThisFrame;
    public static bool IsPressed => P != null && P.press.isPressed;
    public static bool ReleasedThisFrame => P != null && P.press.wasReleasedThisFrame;
    public static Vector2 Position => P != null ? P.position.ReadValue() : Vector2.zero;
#else
    public static bool PressedThisFrame => Input.GetMouseButtonDown(0);
    public static bool IsPressed => Input.GetMouseButton(0);
    public static bool ReleasedThisFrame => Input.GetMouseButtonUp(0);
    public static Vector2 Position => Input.mousePosition;
#endif

    public static bool OverUI => Hud.IsOverUI(Position);
}
