using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Hole : MonoBehaviour
{
    public float timeDiff;
    private float storedTime;
    private bool hasCompleted = false;

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
            GameObject player = collision.gameObject;
            LevelManager.instance.HoleComplete(player);
            hasCompleted = true;
        }
    }

}
