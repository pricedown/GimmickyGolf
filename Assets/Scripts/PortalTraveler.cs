using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalTraveler : MonoBehaviour
{
    float portalCD = 0;

    void FixedUpdate()
    {
        if(portalCD > 0)
        {
            portalCD -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Portal" && portalCD <= 0)
        {
            portalCD = 0.25f;
            collision.GetComponent<PortalController>().Warp(gameObject);
        }
    }
}
