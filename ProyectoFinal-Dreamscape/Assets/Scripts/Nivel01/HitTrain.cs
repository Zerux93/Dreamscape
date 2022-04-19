using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitTrain : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform teleportSpot;

    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Player"){
            player.position = teleportSpot.position;
            player.localRotation = teleportSpot.localRotation;
        }
    }

    /*private void OnCollisionEnter(Collision other) {
        if(other.gameObject.name == "LookTrigger"){
            player.position = teleportSpot.position;
        }
    }*/

}
