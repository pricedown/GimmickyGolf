using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GravBlock : MonoBehaviour
{
    public int rot = 1;
    public GameObject arrowPrefab;

    private void Start()
    {
        Vector2 scaleMod = new Vector2(2 / transform.localScale.x, 2 / transform.localScale.y);
        Vector2 startPos = new Vector2(-1 + scaleMod.x/2, -1 + scaleMod.y/2);
        for(int i = 0; i < transform.localScale.x; i++)
        {
            for(int j = 0; j < transform.localScale.y; j++)
            {
                var obj = Instantiate(arrowPrefab, transform);
                obj.transform.localPosition = new Vector2(startPos.x + i * scaleMod.x, startPos.y + j * scaleMod.y);
                obj.transform.localScale = 2*Vector2.one/transform.localScale;
                obj.name = "i: " + i + ", j: " + j;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var FORCE_OF_GRAVITY = Physics.gravity.magnitude;
        var FORCE_OF_GRAVITY2D = Physics2D.gravity.magnitude;
        if (collision.CompareTag("Player"))
        {
            switch(rot)
            {
                case 1: Physics.gravity = new Vector3(FORCE_OF_GRAVITY, 0, 0); Physics2D.gravity = new Vector2(FORCE_OF_GRAVITY2D, 0);  break;
                case 2: Physics.gravity = new Vector3(0, -FORCE_OF_GRAVITY, 0); Physics2D.gravity = new Vector2(0, -FORCE_OF_GRAVITY2D); break;
                case 3: Physics.gravity = new Vector3(-FORCE_OF_GRAVITY, 0, 0); Physics2D.gravity = new Vector2(-FORCE_OF_GRAVITY2D, 0); break;
                case 4: Physics.gravity = new Vector3(0, FORCE_OF_GRAVITY, 0); Physics2D.gravity = new Vector2(0, FORCE_OF_GRAVITY2D); break;
            }
        }
    }
}