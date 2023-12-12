using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Hole : MonoBehaviour
{
    public float timeDiff;
    private float storedTime;
    private bool hasCompleted = false;
    private bool flagRising = false;
    private Vector3 startLocation;

    private void Start()
    {
        startLocation = transform.GetChild(2).GetChild(0).transform.localPosition;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            flagRising = true;
            storedTime = Time.time; // Get time the ball gets in the hole
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        //Debug.Log(Time.time - storedTime > timeDiff);
        /*if(Time.time - storedTime > timeDiff && !hasCompleted && collision.CompareTag("Player")) // Check if the ball has been in the hole for long enough
        {
            GameObject player = collision.gameObject;
            LevelManager.instance.HoleComplete(player);
            hasCompleted = true;
        }*/
        if(collision.CompareTag("Player") && collision.GetComponent<BallMovement>().still)
        {
            GameObject player = collision.gameObject;
            LevelManager.instance.HoleComplete(player);
            hasCompleted = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            transform.GetChild(2).GetChild(0).transform.localPosition = startLocation;
            flagRising = false;
        }
    }
    private void Update()
    {
        if(flagRising)
        {
            var flag = transform.GetChild(2).GetChild(0).transform;
            flag.transform.localPosition += new Vector3(0, Time.deltaTime, 0);
            if (flag.transform.localPosition.y >= 0.39f)
            {
                flagRising = false;
                flag.transform.localPosition = new Vector3(2.3f, 0.39f, 0);
            }
        }
    }
    public void FlagRaise()
    {
        flagRising = true;
        var flag = transform.GetChild(2).GetChild(0).transform;
    }
}
