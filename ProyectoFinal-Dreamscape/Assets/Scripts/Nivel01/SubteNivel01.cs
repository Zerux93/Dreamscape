using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubteNivel01 : SubteScript
{
    [Header("Waypoints")]
    [SerializeField] private Transform waypointA;
    [SerializeField] private Transform waypointB;

    [Header("Trains")]
    [SerializeField] private Transform trainA;
    [SerializeField] private Transform trainB;

    private bool playerHasEntered = false; 

    private float timePass = 0;
    private float coolDown = 3f;
    private float coolDown2 = 5f; 

    public bool firstStation = false;


    // Update is called once per frame
    void Update(){

        if(canMove && playerHasEntered){
            Movement(waypointA);
                if(transform.position.z <= waypointA.transform.position.z){
                    Debug.Log("EstacionA");
                    canMove = false;                    
                    OpenDoors(RDoors);
            }
        }

        if(!playerHasEntered && firstStation && !canMove){
            timePass += Time.deltaTime;
            if(timePass > coolDown){
                CloseDoors(RDoors);
            }

            if(timePass > coolDown2){
                canMove = true;
                timePass = 0;
            }
        }

        if(canMove && firstStation){
            Movement(waypointB);
            coolDown2 = 10f;
            timePass += Time.deltaTime;
            if(timePass > coolDown2){
                canMove = true;
                Destroy(this.gameObject);
            }
        }

        if(playerHasEntered){
            Player.transform.SetParent(transform);
        } else {
            Player.transform.SetParent(null);
        }

        if(canMove){
            BobMovement(trainA);
        }
    }

    private void FixedUpdate() {
        if(canMove){;
            BobMovement(trainB);
        }
    }


    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Player" && playerHasEntered == false){
            Debug.Log("Entró");
            playerHasEntered = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if(other.tag == "Player"){
            Debug.Log("Salió");
            playerHasEntered = false;
            firstStation = true;
        }
    }
}
