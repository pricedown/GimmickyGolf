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

    public LineRenderer pullbackIndicator, trajectoryIndicator;
    public Vector2 relativeMousePos, storedPos, mousePos, screenSize, shotDirection;
    public float power;
    public bool isClicked = false, still, glued = false;
    public float portalCD, glueCD = 0;
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


        if (still)
        {
            if (glued) glueCD = 0.1f;
            glued = false;
            rb.isKinematic = false;
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

    public Vector3[] PullbackLine()
    {
        Vector3[] points = new Vector3[] { cam.ScreenToWorldPoint(mousePos), cam.ScreenToWorldPoint(storedPos * screenSize) };
       
        for (int i = 0; i < 2; i++)
            points[i].z = 0;
        
        return points;
    }
    public Vector3[] TrajectoryLine()
    {
        List<Vector3> points = new List<Vector3>();
        Vector3 initialVelocity = shotDirection * ((power / rb.mass) * Time.fixedDeltaTime); // v0 = (F/m) * t 
        
        // remember, every tick:
        // rb.velocity *= Mathf.Clamp01(1f - rb.drag * Time.fixedDeltaTime);
        // so how do we account for rb.drag?
        
        for (float deltaTime = 0; deltaTime < indicatorDuration; deltaTime += indicatorStep)
        {
            Vector3 pos = transform.position + (initialVelocity * deltaTime);
            pos.y += 0.5f * Physics2D.gravity.y * Mathf.Pow(deltaTime, 2);
            points.Add(pos);
        }
        return points.ToArray();
    }
}