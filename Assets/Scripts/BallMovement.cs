using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BallMovement : MonoBehaviour
{
    // maximum force that can be applied to the ball in a swing, 0 means uncapped
    public float maxPower; 
    
    // draw weight constant of the "bow" that flings the ball
    public float drawForce; 
    
    [Header("Runtime")]
    public bool isClicked = false;
    public Vector2 relativeMousePos, storedPos, mousePos, screenSize;
    public float power;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        screenSize = new Vector2(Screen.width, Screen.height);
    }

    public void MousePosition(InputAction.CallbackContext context)
    {
        mousePos = context.ReadValue<Vector2>();
        relativeMousePos = mousePos / screenSize;
    }

    public void Click(InputAction.CallbackContext context)
    {
        if (context.performed) // on click
        {
            storedPos = relativeMousePos;
            isClicked = true;
        }
        if (context.canceled) // on release
        {
            Vector2 drawback = relativeMousePos - storedPos;
            Shoot(drawback);
            isClicked = false;
        }
    }
    public void Shoot(Vector2 drawback)
    {
        Vector2 shotDirection = -drawback.normalized; // reverse direction (bow physics)
        power = drawForce * drawback.magnitude; // F = draw force constant * draw length

        // clamp 
        power = Mathf.Max(power, 0);
        if (maxPower > 0) power = Mathf.Min(power, maxPower);
        
        rb.AddForce(power * shotDirection);
    }
}