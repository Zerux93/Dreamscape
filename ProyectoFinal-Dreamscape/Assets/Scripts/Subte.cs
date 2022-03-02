using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Subte : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 15f;
    //bobmovement
    [SerializeField] private float bobMovementSpeed = 2f; 
    [SerializeField] private float bobMovementAmount = 0.001f; 

    [Header("Waypoints")]
    [SerializeField] private Transform waypointA;
    [SerializeField] private Transform waypointB;
    [SerializeField] private Transform waypointC;
    public bool canMove = true;

    [Header("Trains")]
    [SerializeField] private Transform trainA;
    [SerializeField] private Transform trainB;

    [Header("Doors")]
    [SerializeField] private Animator[] RDoors;
    [SerializeField] private Animator[] LDoors; 

    [Header("Player")]
    [SerializeField] Transform Player;
    private bool playerHasCrossed = false; 

    private float timePass = 0;
    private float coolDown = 3f;
    private float coolDown2 = 5f; 

    public bool fistStation = false;
    public bool secondStation = false;

    private float timer;

    void Update()
    {
        if(canMove && !fistStation){
            Movement(waypointA);
                if(transform.position.z <= waypointA.transform.position.z){
                    Debug.Log("EstacionA");
                    canMove = false;
                    if(!playerHasCrossed)
                    OpenDoors(LDoors);
            }
        }

        if(canMove && fistStation && !secondStation){
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

    private void Movement(Transform waypoint){
        Vector3 deltaVector = new Vector3(0,0,waypoint.position.z - transform.position.z);
        Vector3 direction = deltaVector.normalized;
        transform.position += direction * speed * Time.deltaTime;
    }

    public void BobMovement(Transform train){
        timer += Time.deltaTime * bobMovementSpeed;
        train.transform.localPosition = new Vector3(
            train.transform.localPosition.x + Mathf.Sin(timer) * bobMovementAmount,
            train.transform.localPosition.y,
            train.transform.localPosition.z
        );
    }

    public void OpenDoors(Animator[] doors){
        foreach(Animator a in doors){
            a.SetBool("Open", true);
        }
    }

    public void CloseDoors(Animator[] doors){
        foreach(Animator a in doors){
            a.SetBool("Open", false);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Player" && playerHasCrossed == false){
            Debug.Log("Entró");
            playerHasCrossed = true;
            CloseDoors(LDoors);
            canMove = true;
            fistStation = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if(other.tag == "Player" && secondStation){
            Debug.Log("Salió");
            playerHasCrossed = false;
        }
    }
}
