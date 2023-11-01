using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hole : MonoBehaviour
{
    public float timeDiff;
    private float storedTime;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        storedTime = Time.time;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(Time.time - storedTime > timeDiff)
        {
            print("HoleComplete");
        }
    }
}
