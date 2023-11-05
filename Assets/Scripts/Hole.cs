using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hole : MonoBehaviour
{
    public float timeDiff;
    private float storedTime;
    private bool hasCompleted = false;
    public GameObject holeCompleteText;

    private void Start()
    {
        holeCompleteText.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        storedTime = Time.time; // Get time the ball gets in the hole
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(Time.time - storedTime > timeDiff && !hasCompleted) // Check if the ball has been in the hole for long enough
        {
            HoleComplete();
        }
    }
    private void HoleComplete()
    {
        holeCompleteText.SetActive(true); // Show the text saying the hole has been completed
        hasCompleted = true;
        Time.timeScale = 0f; // Stop the scene
    }
}
