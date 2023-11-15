using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using Unity.VisualScripting;

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
    public Vector2 relativeMousePos, storedPos, mousePos, screenSize, shotDirection, previousPos, initialPos;
    public float power;
    public bool isClicked = false, still, glued = false;
    public float portalCD;
    public GameObject cursorIndicatorPrefab;
    public Camera cam;
    public int strokeCount = 0;
    private Rigidbody2D rb;
    private CircleCollider2D collider;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<CircleCollider2D>();
        cam = Camera.main;
        pullbackIndicator = GameObject.Find("Pullback").GetComponent<LineRenderer>();
        trajectoryIndicator = GameObject.Find("Trajectory").GetComponent<LineRenderer>();
        screenSize = new Vector2(Screen.width, Screen.height);
        initialPos = transform.position;
        ChangeStrokes(0);
        LevelManager.instance.LoadPlayer();
    }
    private void FixedUpdate()
    {
        if (glued) rb.velocity = Vector2.zero;
        if (portalCD > 0) portalCD -= Time.deltaTime;
        still = (rb.velocity.magnitude <= 0.09f);

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
            //print(shotTime - Time.time);
        }
    }

    public void MousePosition(InputAction.CallbackContext context)
    {
        mousePos = context.ReadValue<Vector2>();
        relativeMousePos = mousePos / screenSize;
    }

    public float shotTime;
    
    public void Click(InputAction.CallbackContext context)
    {
        if (context.performed) // on click
        {
            isClicked = true;
            storedPos = relativeMousePos;
        }
        if (context.canceled && isClicked) // on release
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
                ChangeStrokes(1);
            }// TODO: add cancelling of action
        }
    }
    public void Cancel(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isClicked = false;
            pullbackIndicator.enabled = false;
            trajectoryIndicator.enabled = false;
        }
    }

    public void ResetPos(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            transform.position = initialPos;
            Cancel(context);
            ChangeStrokes(-1 * strokeCount);
            rb.velocity = Vector2.zero;
            rb.inertia = 0;
        }
    }
    public void Pause(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            Cancel(context);
            LevelManager.instance.PauseUnpause();
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
        points.Capacity = trajectoryIndicator.positionCount;
        
        Vector3 initialVelocity = rb.velocity + shotDirection * ((power / rb.mass) * Time.fixedDeltaTime); // v0 = (F/m) * t 

        Vector3 velocity = initialVelocity;
        points.Add(transform.position);
        
        for (int i = 0; i < trajectoryIndicator.positionCount; i++)
        {
            Vector3 newPoint = points.Last();
            
            // calculate velocity from accelerations and time
            velocity += Physics.gravity * (rb.gravityScale * Time.fixedDeltaTime);  // gravity
            velocity = velocity * Mathf.Clamp01(1f - rb.drag * Time.fixedDeltaTime); // drag
            // calculate position from velocities
            newPoint += (velocity * Time.fixedDeltaTime);

            points.Add(newPoint);

            const int margin = 2;
            if (i + margin >= trajectoryIndicator.positionCount)
                continue;
            if (Vector3.Distance(transform.position, newPoint) > 2*collider.radius)
            {
                if (Physics2D.OverlapCircle(newPoint, collider.radius - 0.3f))
                {
                    trajectoryIndicator.positionCount = i+margin;
                }
            }
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
            if(Mathf.Abs((previousPos - initialPos).magnitude) < 1f)
            {
                ChangeStrokes(-1 * strokeCount);
            }
            transform.position = previousPos;
            rb.velocity = Vector2.zero;
            rb.inertia = 0;
        }
    }

    private void ChangeStrokes(int changeBy)
    {
        if (Time.timeScale != 0)
        {
            strokeCount += changeBy;
            LevelManager.instance.SetCurrentStrokes(strokeCount);
        }
    }
}