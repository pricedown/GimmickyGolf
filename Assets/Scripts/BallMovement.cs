using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class BallMovement : MonoBehaviour
{
    // maximum force that can be applied to the ball in a swing
    // 0 means uncapped
    public float maxPower; 
    
    // draw weight of the "bow" that flings the ball
    public float drawForce; 
    
    [Header("Runtime")]
    public bool isClicked = false, still, glued = false;
    public LineRenderer pullbackIndicator;
    public Vector2 relativeMousePos, storedPos, mousePos, screenSize;
    public float power, glueCD = 0;
    private Rigidbody2D rb;
    public GameObject cursorIndicatorPrefab;
    public Camera cam;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
        pullbackIndicator = GameObject.Find("Pullback").GetComponent<LineRenderer>();
        screenSize = new Vector2(Screen.width, Screen.height);
    }
    private void FixedUpdate()
    {
        if (glued)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
        } else if(glueCD > 0)
        {
            glueCD -= Time.deltaTime;
        }
        still = (rb.velocity.magnitude <= 0.05f);
        
        pullbackIndicator.SetPositions(PullbackLine());
        pullbackIndicator.enabled = isClicked && still;
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
            isClicked = true;
            storedPos = relativeMousePos;
        }
        if (context.canceled) // on release
        {
            isClicked = false;
            if (still)
            {
                Vector2 drawback = relativeMousePos - storedPos;
                Shoot(drawback);
            }
        }
    }
    
    public void Shoot(Vector2 drawback)
    {
        if (still)
        {
            if (glued) glueCD = 0.1f;
            glued = false;
            rb.isKinematic = false;
            Vector2 shotDirection = -drawback.normalized; // reverse direction (bow physics)
            power = drawForce * drawback.magnitude; // F = draw force constant * draw length

            // clamp 
            power = Mathf.Max(power, 0);
            if (maxPower > 0) power = Mathf.Min(power, maxPower);

            rb.AddForce(power * shotDirection);
        }
    }

    public Vector3[] PullbackLine()
    {
        Vector3[] points = new Vector3[] { cam.ScreenToWorldPoint(mousePos), cam.ScreenToWorldPoint(storedPos * screenSize) };
       
        for (int i = 0; i < 2; i++)
            points[i].z = 0;
        
        return points;
    }

    public List<Vector3> TrajectoryLine()
    {
        List<Vector3> points = new List<Vector3>();

        return points;
    }
}