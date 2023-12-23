using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GravBlock : MonoBehaviour
{
    private float rot = 0;

    private void Start()
    {
        rot = gameObject.transform.rotation.eulerAngles.z;
        rot *= Mathf.Deg2Rad;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var FORCE_OF_GRAVITY = Physics.gravity.magnitude;
        var FORCE_OF_GRAVITY2D = Physics2D.gravity.magnitude;
        if (collision.CompareTag("Player"))
        {
            Physics.gravity = new Vector3(Mathf.Cos(rot), Mathf.Sin(rot), 0) * FORCE_OF_GRAVITY;
            Physics2D.gravity = new Vector2(Mathf.Cos(rot), Mathf.Sin(rot)) * FORCE_OF_GRAVITY2D;
            /*switch (rot)
            {
                case 0: Physics.gravity = new Vector3(FORCE_OF_GRAVITY, 0, 0); Physics2D.gravity = new Vector2(FORCE_OF_GRAVITY2D, 0);  break;
                case 270: Physics.gravity = new Vector3(0, -FORCE_OF_GRAVITY, 0); Physics2D.gravity = new Vector2(0, -FORCE_OF_GRAVITY2D); break;
                case 180: Physics.gravity = new Vector3(-FORCE_OF_GRAVITY, 0, 0); Physics2D.gravity = new Vector2(-FORCE_OF_GRAVITY2D, 0); break;
                case 90: Physics.gravity = new Vector3(0, FORCE_OF_GRAVITY, 0); Physics2D.gravity = new Vector2(0, FORCE_OF_GRAVITY2D); break;
            }*/
        }
    }
}