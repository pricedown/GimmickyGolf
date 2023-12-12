using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.Serialization;

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

    public float stillVelocity = 0.2f;
    public float stillDuration = 0.2f;
    public bool rollingFlat = false;
    public float rollingDuration = 0.05f;
    public float rollingVelocity = 2f;
    public float lastRolling;

    [Header("Runtime")]

    public int varMagStrength = 35;
    public int magnetStrength;
    public bool magnetisedA = false;
    public bool magnetisedR = false;
    Vector3 magnetPosition;
    public LineRenderer pullbackIndicator, trajectoryIndicator;
    public Vector2 relativeMousePos, storedPos, mousePos, screenSize, shotDirection, previousPos, initialPos, storedGravity2D;
    public float power;
    public bool isClicked = false, still, glued = false;
    public float portalCD;
    public GameObject cursorIndicatorPrefab;
    public Camera cam;
    public int strokeCount = 0;
    private Rigidbody2D rb;
    private CircleCollider2D collider;
    public Color stillColor, moveColor, pullColor;
    public SpriteRenderer ballRender;
    public Vector3 storedGravity3D;
    
    private float lastMoved;
    private float lastParticle;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<CircleCollider2D>();
        cam = Camera.main;
        pullbackIndicator = GameObject.Find("Pullback").GetComponent<LineRenderer>();
        trajectoryIndicator = GameObject.Find("Trajectory").GetComponent<LineRenderer>();
        screenSize = new Vector2(Screen.width, Screen.height);
        initialPos = transform.position;
        lastParticle = Time.time - 0.5f;
        ChangeStrokes(0);
        LevelManager.instance.LoadPlayer();
        ResetGravity();
    }

    private bool isRolling()
    {
        float dotProduct = Vector3.Dot(rb.velocity.normalized, Physics2D.gravity.normalized);
        float threshold = 0.1f;
        return (Mathf.Abs(dotProduct) < threshold && rb.velocity.magnitude <= rollingVelocity);
    }

    private void FixedUpdate()
    {
        // reduce downtime when ball is rolling flat
        if (!isRolling())
            lastRolling = Time.time;
        rollingFlat = (Time.time - lastRolling >= rollingDuration);

        if (rollingFlat)
        {
            rb.velocity *= 0.965f; 
        }

        if (glued)
        {
            rb.velocity = Vector2.zero;
            rb.rotation = 0;
        }
        if (portalCD > 0) portalCD -= Time.deltaTime;
        
        if (rb.velocity.magnitude > stillVelocity)
            lastMoved = Time.time;
        
        still = (Time.time - lastMoved >= stillDuration);

        if (still)
        {
            if (isClicked)
            {
                // Set up for a Stroke
                
                Vector2 drawback = relativeMousePos - storedPos;
                UpdateShot(drawback);
                ballRender.color = pullColor;
                pullbackIndicator.enabled = true;
                trajectoryIndicator.enabled = true;
                pullbackIndicator.SetPositions(PullbackLine());
                trajectoryIndicator.SetPositions(TrajectoryLine());
            }
            else
            {
                ballRender.color = stillColor;
            }
        } else {
            ballRender.color = moveColor;
            pullbackIndicator.enabled = false;
            trajectoryIndicator.enabled = false;
            //print(shotTime - Time.time);
        }

        if (magnetisedA)
        {
            float distance = Vector3.Distance(magnetPosition, transform.position);
            Vector2 targetDirection = (magnetPosition - transform.position).normalized;
            //magnetStrength = (int)(1 / distance * varMagStrength);
            magnetStrength = varMagStrength;
            rb.AddForce(new Vector2(targetDirection.x, targetDirection.y) * magnetStrength);

        }
        if (magnetisedR)
        {
            float distance = Vector3.Distance(magnetPosition, transform.position);
            Vector2 targetDirection = (magnetPosition - transform.position).normalized;
            magnetStrength = (int)(1 / distance * varMagStrength);
            //magnetStrength = 40;
            rb.AddForce(new Vector2(targetDirection.x, targetDirection.y) * (-1 * magnetStrength));
            
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
            if (still && Time.timeScale != 0f && power > 0)
            {
                if (glued)
                {
                    glued = false;
                    rb.isKinematic = false;
                }
                ChangeStrokes(1);
                rb.AddForce(power * shotDirection);
                storedGravity2D = Physics2D.gravity;
                storedGravity3D = Physics.gravity;
                shotTime = Time.time;
                previousPos = transform.position;
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
        if (context.performed && Time.timeScale != 0f)
        {
            transform.position = initialPos;
            Cancel(context);
            ChangeStrokes(-1 * strokeCount);
            rb.velocity = Vector2.zero;
            rb.inertia = 0;
            ResetGravity();
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
            Vector3 lastPoint = points.Last();
            
            // calculate velocity from accelerations and time
            velocity += Physics.gravity * (rb.gravityScale * Time.fixedDeltaTime);  // gravity
            velocity = velocity * Mathf.Clamp01(1f - rb.drag * Time.fixedDeltaTime); // drag
            // calculate position from velocities
            Vector3 newPoint = lastPoint + (velocity * Time.fixedDeltaTime);

            points.Add(newPoint);

            const int margin = 2;
            if (i + margin >= trajectoryIndicator.positionCount)
                continue;
            
            // ?only check if it's a certain distance away
            if (Vector3.Distance(transform.position, newPoint) > 2*collider.radius)
            {
                /*
                if (Physics2D.OverlapCircle(newPoint, collider.radius - 0.3f))
                {
                    trajectoryIndicator.positionCount = i+margin;
                }*/
                // TODO: add filter for triggers in OverlapArea
                
                if (Physics2D.OverlapCircle(newPoint, collider.radius))
                    trajectoryIndicator.positionCount = i+margin;
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
            Physics2D.gravity = storedGravity2D;
            Physics.gravity = storedGravity3D;
            transform.position = previousPos;
            rb.velocity = Vector2.zero;
            rb.inertia = 0;
        }
    }

    public void setTargetA(Vector3 position)
    {
        magnetPosition = position;
        magnetisedA = true;
    }
    public void setTargetR(Vector3 position)
    {
        magnetPosition = position;
        magnetisedR = true;
    }

    private void ChangeStrokes(int changeBy)
    {
        if (Time.timeScale != 0)
        {
            strokeCount += changeBy;
            LevelManager.instance.SetCurrentStrokes(strokeCount);
        }
    }
    public void ResetGravity()
    {
        var FORCE_OF_GRAVITY = Physics.gravity.magnitude;
        var FORCE_OF_GRAVITY2D = Physics2D.gravity.magnitude;
        Physics.gravity = new Vector3(0, -FORCE_OF_GRAVITY, 0);
        Physics2D.gravity = new Vector2(0, -FORCE_OF_GRAVITY2D);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (Time.time - lastParticle > 0.2f)
        {
            transform.GetChild(1).GetChild(3).GetComponent<ParticleSystem>().Play();
            lastParticle = Time.time;
        }
    }
}