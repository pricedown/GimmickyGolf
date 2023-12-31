using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Hole : MonoBehaviour
{
    public float timeDiff;
    private float storedTime;
    private bool flagRising = false;
    private Vector3 startLocation;
    private GameObject player;

    private void Start()
    {
        startLocation = transform.GetChild(2).GetChild(0).transform.localPosition;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = collision.gameObject;
            flagRising = true;
            storedTime = Time.time; // Get time the ball gets in the hole
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
            flag.transform.localPosition += new Vector3(0, Time.deltaTime * 1.75f, 0);
            if (flag.transform.localPosition.y >= 0.39f)
            {
                flagRising = false;
                flag.transform.localPosition = new Vector3(2.3f, 0.39f, 0);
                LevelManager.instance.HoleComplete(player);
            }
        }
    }
    public void FlagRaise()
    {
        flagRising = true;
        var flag = transform.GetChild(2).GetChild(0).transform;
    }
}
