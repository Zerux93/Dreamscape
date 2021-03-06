using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    public Transform player;
    public Transform teleportSpot;

    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Player"){
            player.position = teleportSpot.position;
        }
    }
}
