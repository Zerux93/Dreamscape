using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Immobilizer : MonoBehaviour
{
    [SerializeField] public GameObject Player;
    private PlayerController playerController;

    private void Start() {
        playerController = Player.GetComponent<PlayerController>();
    }

    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Player"){
            playerController.canWalk = false;
            Debug.Log("coso");
        }
    }
}
