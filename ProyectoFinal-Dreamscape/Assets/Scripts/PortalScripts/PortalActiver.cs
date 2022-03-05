using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalActiver : MonoBehaviour
{
    public GameObject cameraB;
    public GameObject PortalA;


    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Player"){
            cameraB.SetActive(true);
            PortalA.SetActive(true);
        }
    }
}
