using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoints : MonoBehaviour
{
    public CheckManager checkpointManager;
    public Material activeCheckpointMat;

    private bool playerHasPassed = false;
    private Renderer rend;

    private void Start() {
        rend = GetComponent<Renderer>();
    }

    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Player" && !playerHasPassed){
            checkpointManager.AddCheckpoint(this.gameObject);
            rend.sharedMaterial = activeCheckpointMat;
            playerHasPassed = true;
        }
    }
}
