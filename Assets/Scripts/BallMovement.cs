using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
    public float indicatorDuration = 5f, indicatorStep = 0.1f;
    
    [Header("Runtime")]
    public bool isClicked = false, still;

    public LineRenderer pullbackIndicator, trajectoryIndicator;
    public Vector2 relativeMousePos, storedPos, mousePos, screenSize, shotDirection;
    public float power, portalCD;
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
        if(portalCD > 0)
            portalCD -= Time.deltaTime;

        still = (rb.velocity.magnitude <= 0.05f);
        
        
        if (isClicked && still)
        {
            // Set up for a Stroke
            Vector2 drawback = relativeMousePos - storedPos;
            UpdateShot(drawback);
            
            pullbackIndicator.enabled = true;
            trajectoryIndicator.enabled = true;
            pullbackIndicator.SetPositions(PullbackLine());
            trajectoryIndicator.SetPositions(TrajectoryLine());
        }
        else
        {
            pullbackIndicator.enabled = false;
            //trajectoryIndicator.enabled = false;
        }
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
            if (still) // TODO: add cancelling of action
                rb.AddForce(power * shotDirection);
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
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Portal" && portalCD <= 0)
        {
            collision.GetComponent<PortalController>().Warp(this.gameObject);
            Vector3 diff = (transform.position-collision.transform.position)*collision.GetComponent<PortalController>().offSetMult;
            //transform.position = collision.GetComponent<PortalController>().otherPortal.transform.position+diff;

            portalCD = 0.25f;
        }
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
        // TODO: add the reference
        List<Vector3> points = new List<Vector3>();

        int steps = (int)(indicatorDuration / indicatorStep); //Calculates amount of steps simulation will iterate for
        float _vel = power / rb.mass * Time.fixedDeltaTime; // Velocity = Force / Mass * time

        for (int i = 0; i < 50; ++i) //Iterate a ForLoop over number of Steps
        {
            // Remember f(t) = (x0 + x*t, y0 + y*t - 9.81t²/2)
            // To calculate new Position at each Step... Origin + (LaunchDirection * (LaunchSpeed * Current Step * Length of a Step)
            Vector3 pos = (Vector2)transform.position + (shotDirection * _vel * i * indicatorStep); // Calculate new Vector at flat speed

            pos.y += Physics2D.gravity.y / 2 * Mathf.Pow((i * indicatorStep), 2); // Factor in Gravity, affecting only the Y-Axis (y0 + y*t - 9.81t²/2)
            points.Add(pos); //Add this to the next entry on the list
        }

        return points.ToArray();
    } 
}