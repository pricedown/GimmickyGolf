using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public static MainCamera instance;
    public bool bigLevel = false;
    public Vector2[] borders = new Vector2[] { Vector2.zero, Vector2.zero};
    public Vector2 gridPos;
    public Vector2 pGridPos;
    private GameObject player;
    private Vector3 camPos;

    private void Start()
    {
        if(instance == null)
        {
            instance = this;
        } else
        {
            Destroy(this);
        }
        player = GameObject.Find("Golf Ball");
        if(bigLevel)
        {
            borders = new Vector2[] { Vector2.zero, Vector2.zero };
            gridPos = transform.position * 4f / 3f;
            foreach (var obj in GameObject.FindGameObjectsWithTag("CameraCornerWall"))
            {
                var pos = obj.transform.localPosition;
                if (pos.x < gridPos.x)
                    borders[0] = pos;
                else
                    borders[1] = pos;
            }
        }
    }
    private void Update()
    {
        if (!bigLevel) return;
        else
        {
            gridPos = transform.position * 3f / 4f;
            pGridPos = player.transform.position * 3f / 4f;
            if (pGridPos.x < borders[0].x + 15.5f)
                camPos.x = borders[0].x + 15.5f;
            else if (pGridPos.x > borders[1].x - 15.5f)
                camPos.x = borders[1].x - 15.5f;
            else
                camPos.x = pGridPos.x;

            if (pGridPos.y < borders[0].y + 8.5f)
                camPos.y = borders[0].y + 8.5f;
            else if (pGridPos.y > borders[1].y - 8.5f)
                camPos.y = borders[1].y - 8.5f;
            else
                camPos.y = pGridPos.y;

            camPos.x = camPos.x * 4 / 3;
            camPos.y = camPos.y * 4 / 3;
            camPos.z = -10;
            transform.position = camPos;
        }
    }

    public Camera GetCamera()
    {
        return GetComponent<Camera>();
    }
}
