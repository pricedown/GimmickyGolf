using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlueBlock : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<BallMovement>() != null && collision.gameObject.GetComponent<BallMovement>().glueCD <= 0)
        {
            collision.gameObject.GetComponent<BallMovement>().glued = true;
        }

    }
}
