using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GravBlock : MonoBehaviour
{
    private float rot = 0;
    public float magnitude = 1f;

    public Vector2 getResultingGravity()
    {
        //var FORCE_OF_GRAVITY = Physics.gravity.magnitude * magnitude;
        var FORCE_OF_GRAVITY2D = Physics2D.gravity.magnitude * magnitude;
        return new Vector2(Mathf.Cos(rot), Mathf.Sin(rot)) * FORCE_OF_GRAVITY2D;
    }

    private void Start()
    {
        rot = gameObject.transform.rotation.eulerAngles.z;
        rot *= Mathf.Deg2Rad;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Physics2D.gravity = getResultingGravity();
            Physics.gravity = Physics2D.gravity;
        }
    }
}