using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SearchService;
using UnityEngine;

public class PortalController : MonoBehaviour
{
    public int portalID;
    public GameObject otherPortal;

    private void Start()
    {
        otherPortal = GameObject.FindGameObjectsWithTag("Portal").Where(x => x.GetComponent<PortalController>().portalID == portalID && x != this.gameObject).FirstOrDefault();
    }
}
