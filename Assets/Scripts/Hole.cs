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
        storedTime = Time.time;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(Time.time - storedTime > timeDiff && !hasCompleted)
        {
            HoleComplete();
        }
    }
    private void HoleComplete()
    {
        holeCompleteText.SetActive(true);
        hasCompleted = true;
        Time.timeScale = 0f;
    }
}
