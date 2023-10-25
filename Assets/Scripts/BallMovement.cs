using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BallMovement : MonoBehaviour
{
    public Vector2 relativeMousePos, storedPos, mousePos, screenSize;
    public float maxPower, powerMultiplier, power;
    private Rigidbody2D rb;
    public bool isClicked = false;

    private void Start()
    {
        screenSize = new Vector2(Screen.width, Screen.height);
        rb = GetComponent<Rigidbody2D>();
    }

    public void MousePosition(InputAction.CallbackContext context)
    {
        mousePos = context.ReadValue<Vector2>();
        relativeMousePos = mousePos / screenSize;
    }

    public void Click(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isClicked = true;
            storedPos = relativeMousePos;
        }
        if(context.canceled)
        {
            power = Mathf.Clamp((relativeMousePos - storedPos).sqrMagnitude * powerMultiplier, 0, maxPower);
            Vector2 shotDirection = (storedPos - relativeMousePos).normalized;
            Shoot(power, shotDirection);
            isClicked = false;
        }
    }
    public void Shoot(float power, Vector2 direction)
    {
        rb.AddForce(direction * power);
    }
}