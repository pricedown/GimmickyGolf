using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class BallMovement : MonoBehaviour
{
    // maximum force that can be applied to the ball in a swing
    // 0 means uncapped
    public float maxPower; 
    
    // draw weight of the "bow" that flings the ball
    public float drawForce; 
    
    // trajectory indicator
    // duration: simulated foresight
    // step: roughness of simulation
    public float indicatorDuration = 5f;
    
    [Header("Runtime")]

    public LineRenderer pullbackIndicator, trajectoryIndicator;
    public Vector2 relativeMousePos, storedPos, mousePos, screenSize, shotDirection, previousPos;
    public float power;
    public bool isClicked = false, still, glued = false;
    public float portalCD;
    private Rigidbody2D rb;
    public GameObject cursorIndicatorPrefab;
    public Camera cam;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
        pullbackIndicator = GameObject.Find("Pullback").GetComponent<LineRenderer>();
        trajectoryIndicator = GameObject.Find("Trajectory").GetComponent<LineRenderer>();
        screenSize = new Vector2(Screen.width, Screen.height);
    }
    private void FixedUpdate()
    {
        if (glued) rb.velocity = Vector2.zero;
        if (portalCD > 0) portalCD -= Time.deltaTime;
        still = (rb.velocity.magnitude <= 0.05f);

        if (still)
        {
            if (isClicked)
            {
                // Set up for a Stroke
                Vector2 drawback = relativeMousePos - storedPos;
                UpdateShot(drawback);

                pullbackIndicator.enabled = true;
                trajectoryIndicator.enabled = true;
                pullbackIndicator.SetPositions(PullbackLine());
                trajectoryIndicator.SetPositions(TrajectoryLine());
            }
        } else {
            pullbackIndicator.enabled = false;
            trajectoryIndicator.enabled = false;
            print(shotTime - Time.time);
        }
    }

    public void MousePosition(InputAction.CallbackContext context)
    {
        mousePos = context.ReadValue<Vector2>();
        relativeMousePos = mousePos / screenSize;
    }

    private float shotTime;

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
                if (glued)
                {
                    glued = false;
                    rb.isKinematic = false;
                }
                rb.AddForce(power * shotDirection);
                shotTime = Time.time;
                previousPos = transform.position;
            }// TODO: add cancelling of action
        }
    }
    
    public void UpdateShot(Vector2 drawback)
    {
        shotDirection = -drawback.normalized; // reverse direction (bow physics)
        power = drawForce * drawback.magnitude; // F = draw force constant * draw length

        // clamp 
        power = Mathf.Max(power, 0);
        if (maxPower > 0) power = Mathf.Min(power, maxPower);
    }

    public Vector3[] PullbackLine()
    {
        Vector3[] points = new Vector3[] { cam.ScreenToWorldPoint(mousePos), cam.ScreenToWorldPoint(storedPos * screenSize) };
       
        for (int i = 0; i < 2; i++)
            points[i].z = 0;
        
        return points;
    }
    
    public Vector3[] TrajectoryLine()
    {
        trajectoryIndicator.positionCount = (int)Mathf.Ceil(indicatorDuration / Time.fixedDeltaTime) + 1; // plus one for initial position
        List<Vector3> points = new List<Vector3>();
        
        Vector3 initialVelocity = rb.velocity + shotDirection * ((power / rb.mass) * Time.fixedDeltaTime); // v0 = (F/m) * t 

        Vector3 velocity = initialVelocity;
        points.Add(transform.position);
        for (int i = 0; i < trajectoryIndicator.positionCount; i++)
        {
            Vector3 newPoint = points.Last();
            
            // calculate velocity from accelerations and time
            velocity += Physics.gravity * rb.gravityScale * Time.fixedDeltaTime;  // gravity
            velocity = velocity * Mathf.Clamp01(1f - rb.drag * Time.fixedDeltaTime); // drag
            // calculate position from velocities
            newPoint += (velocity * Time.fixedDeltaTime);
            
            points.Add(newPoint);
        }
        return points.ToArray();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Glue")
        {
            glued = true;
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
        }
        if (collision.gameObject.tag == "Water")
        {
            rb.velocity = Vector2.zero;
            transform.position = previousPos;
        }
    }
}