using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float speed;
    public List<Transform> targets = new List<Transform>();
    private List<Vector2> locations = new List<Vector2>();
    public int targetChoice = 0;

    private void Start()
    {
        foreach(Transform t in targets) // Get position of targets set
        {
            locations.Add(t.position); // Add position to list
            Destroy(t.gameObject); // Destroy object
        }
    }
    private void FixedUpdate()
    {
        transform.position = Vector2.MoveTowards(transform.position, locations[targetChoice], speed * Time.deltaTime);
        if ((Vector2)transform.position == locations[targetChoice]) // Check if it has reached its location
        {
            targetChoice++; // Change target
            if(targetChoice >= targets.Capacity)
            {
                targetChoice = 0; // Reset target
            }
        }
    }
}
