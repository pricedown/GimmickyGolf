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
    
    [Header("Runtime")]
    public bool isClicked = false, still;
    public LineRenderer pullbackIndicator;
    public Vector2 relativeMousePos, storedPos, mousePos, screenSize;
    public float power, portalCD;
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
        if(portalCD > 0)
            portalCD -= Time.deltaTime;

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
            Vector2 shotDirection = -drawback.normalized; // reverse direction (bow physics)
            power = drawForce * drawback.magnitude; // F = draw force constant * draw length

            // clamp 
            power = Mathf.Max(power, 0);
            if (maxPower > 0) power = Mathf.Min(power, maxPower);

            rb.AddForce(power * shotDirection);
        }
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

    public List<Vector3> TrajectoryLine()
    {
        List<Vector3> points = new List<Vector3>();

        return points;
    }
}