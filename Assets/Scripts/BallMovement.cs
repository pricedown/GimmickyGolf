using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BallMovement : MonoBehaviour
{
    public Vector2 mousePos;
    public bool isClicked = false;

    public void MousePosition(InputAction.CallbackContext context)
    {
        mousePos = context.ReadValue<Vector2>();
    }

    public void Click(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isClicked = true;
        }
        else
        {
            isClicked = false;
        }
    }
}