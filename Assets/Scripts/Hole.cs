using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Hole : MonoBehaviour
{
    public float timeDiff;
    private float storedTime;
    private bool hasCompleted = false;
    private GameObject player;
    public GameObject holeCompleteText;
    public TextMeshProUGUI strokeText;

    private void Start()
    {
        holeCompleteText.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            storedTime = Time.time; // Get time the ball gets in the hole
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(Time.time - storedTime > timeDiff && !hasCompleted && collision.CompareTag("Player")) // Check if the ball has been in the hole for long enough
        {
            player = collision.gameObject;
            HoleComplete();
        }
    }
    private void HoleComplete()
    {
        holeCompleteText.SetActive(true); // Show the text saying the hole has been completed
        int strokes = player.GetComponent<BallMovement>().strokeCount;
        strokeText.text = "Strokes: " + strokes;
        hasCompleted = true;
        Time.timeScale = 0f; // Stop the scene
    }
}
