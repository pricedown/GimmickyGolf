using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine;

public class PortalController : MonoBehaviour
{
    public int portalID;
    public GameObject otherPortal;
    public Vector2 offSetMult;

    [System.Obsolete]
    private void Start()
    {
        gameObject.GetComponentInChildren<ParticleSystem>().emissionRate = 25 * transform.localScale.y;
        gameObject.GetComponentInChildren<ParticleSystem>().startColor = GetComponent<SpriteRenderer>().color;
        otherPortal = GameObject.FindGameObjectsWithTag("Portal").Where(x => x.GetComponent<PortalController>().portalID == portalID && x != this.gameObject).FirstOrDefault();
    }
    public void Warp(GameObject target)
    {
        var otherRot = otherPortal.transform.rotation.eulerAngles.z*Mathf.PI/180;
        var thisRot = transform.rotation.eulerAngles.z*Mathf.PI/180;
        var xDifference = transform.position.x - target.transform.position.x;
        var yDifference = transform.position.y - target.transform.position.y;
        var angle = Mathf.Atan2(yDifference, xDifference)-thisRot;
        var newAngle = otherRot - angle + Mathf.PI;
        var dist = Vector2.Distance(target.transform.position, transform.position);
        dist *= otherPortal.transform.localScale.y / transform.localScale.y;
        var newPosition = new Vector2(otherPortal.transform.position.x + dist*Mathf.Cos(newAngle), otherPortal.transform.position.y + dist*Mathf.Sin(newAngle));
        target.transform.position = newPosition;
        var diff = -thisRot +otherRot - Mathf.PI;
        var rb = target.GetComponent<Rigidbody2D>();
        var newVelo = new Vector2(rb.velocity.x*Mathf.Cos(diff)-rb.velocity.y*Mathf.Sin(diff),rb.velocity.x*Mathf.Sin(diff)+rb.velocity.y*Mathf.Cos(diff));
        rb.velocity = newVelo;
    }
}
