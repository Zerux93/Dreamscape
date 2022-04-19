using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Subte : SubteScript
{
    [Header("Waypoints")]
    [SerializeField] private Transform waypointA;
    [SerializeField] private Transform waypointB;
    [SerializeField] private Transform waypointC;

    [Header("Trains")]
    [SerializeField] private Transform trainA;
    [SerializeField] private Transform trainB;

    private bool playerHasCrossed = false; 

    private float timePass = 0;
    private float coolDown = 3f;
    private float coolDown2 = 5f; 

    public bool firstStation = false;
    public bool secondStation = false;

    void Update()
    {
        if(canMove && !firstStation){
            Movement(waypointA);
                if(transform.position.z <= waypointA.transform.position.z){
                    Debug.Log("EstacionA");
                    canMove = false;
                    if(!playerHasCrossed)
                    OpenDoors(LDoors);
            }
        }

        if(canMove && firstStation && !secondStation){
            Movement(waypointB);
            if(transform.position.z >= waypointB.transform.position.z){  
                Debug.Log("EstacionB");                  
                canMove = false;
                secondStation = true;
                OpenDoors(LDoors);
            }
        }

        if(!playerHasCrossed && secondStation && !canMove){
            timePass += Time.deltaTime;
            if(timePass > coolDown){
                CloseDoors(LDoors);
            }

            if(timePass > coolDown2){
                canMove = true;
                timePass = 0;
            }
        }

        if(canMove && secondStation){
            Debug.Log("Estacion C");
            Movement(waypointC);
            coolDown2 = 10f;
            timePass += Time.deltaTime;
            if(timePass > coolDown2){
                canMove = true;
                Destroy(this.gameObject);
            }
        }

        if(playerHasCrossed){
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
        if(other.tag == "Player" && playerHasCrossed == false){
            Debug.Log("Entró");
            playerHasCrossed = true;
            CloseDoors(LDoors);
            canMove = true;
            firstStation = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if(other.tag == "Player" && secondStation){
            Debug.Log("Salió");
            playerHasCrossed = false;
        }
    }
}
