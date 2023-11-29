using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Repel : MonoBehaviour
{

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var ballScript = collision.GetComponent<BallMovement>();
            ballScript.setTargetR(transform.position);
           
            
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var ballsScript = collision.GetComponent<BallMovement>();
        ballsScript.magnetisedR = false;
    }
}
