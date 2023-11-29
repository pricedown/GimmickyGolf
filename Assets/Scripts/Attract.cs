using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Attract : MonoBehaviour
{

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var ballScript = collision.GetComponent<BallMovement>();
            ballScript.setTargetA(transform.position);
           
            
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var ballsScript = collision.GetComponent<BallMovement>();
        ballsScript.magnetisedA = false;
    }
}
