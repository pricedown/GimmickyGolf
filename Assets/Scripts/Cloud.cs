using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    float speed;
    float screenWidth;
    private void Start()
    {
        speed = Random.Range(0.03f, 0.05f);
        screenWidth = 25f;
    }
    private void FixedUpdate()
    {
        transform.position += new Vector3(speed, 0, 0);
        if(transform.position.x > screenWidth)
        {
            Destroy(gameObject);
        }
    }
}
