using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    // movement
    public static float HorizontalInput { get; private set; }
    public static float VerticalInput { get; private set; }
    // view
    public static float MouseInputX { get; private set; }
    public static float MouseInputY { get; private set; }
    public static bool IsViewResetPressed { get; private set; }
    private const KeyCode ViewResetKey = KeyCode.BackQuote;
    // jump
    public static bool IsJumpPressed { get; private set; } // Jump
    // fire
    public static bool IsFirePressed { get; private set; } // Fire1
    // reload
    public static bool IsReloadPressed { get; private set; }
    private const KeyCode ReloadKey = KeyCode.R;
    // gravity
    public static bool IsTargetGravityPressed { get; private set; } // Fire2
    public static float MouseInputWheel { get; private set; }
    // intestatic raction
    public static bool IsInteractPressed { get; private set; }
    private const KeyCode InteractKey = KeyCode.E;

    public void GetInput() {
        HorizontalInput = Input.GetAxisRaw("Horizontal");
        VerticalInput = Input.GetAxisRaw("Vertical");
        
        MouseInputX = Input.GetAxis("Mouse X");
        MouseInputY = Input.GetAxis("Mouse Y");
        IsViewResetPressed = Input.GetKeyDown(ViewResetKey);
        
        IsJumpPressed = Input.GetButtonDown("Jump");

        IsFirePressed = Input.GetButton("Fire1");

        IsReloadPressed = Input.GetKeyDown(ReloadKey);

        IsTargetGravityPressed = Input.GetButton("Fire2");
        MouseInputWheel = Input.GetAxis("Mouse ScrollWheel");

        IsInteractPressed = Input.GetKeyDown(InteractKey);
        Debug.Log("input");
        Debug.Log(HorizontalInput);
    }
}
