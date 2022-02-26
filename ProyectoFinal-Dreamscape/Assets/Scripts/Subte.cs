using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Subte : MonoBehaviour
{
    public float speed = 15f;
    private bool canMove = true;

    [SerializeField] private Animator[] RDoors;
    [SerializeField] private Animator[] LDoors;  

    public GameObject Player;

    public float timePass = 0;
    public float coolDown = 5f; 
    private bool playerHasCrossed = false; 

    Vector3 forwardDir = Vector3.forward;
    Vector3 backDir = Vector3.back;

    bool fistStation = false;
    bool secondStation = false;



    void Start()
    {
        
    }


    void Update()
    {
        if(canMove && fistStation == false){
            Movement(backDir);
                if(transform.position.z <= 46f){
                    canMove = false;
                    if(playerHasCrossed == false)
                    OpenDoors(LDoors);
            }
        }

        if(canMove && fistStation == true){
            Movement(forwardDir);
                if(transform.position.z >= 432f){
                    canMove = false;
                    if(playerHasCrossed == true)
                    OpenDoors(LDoors);
            }
        }

        if(playerHasCrossed){
            Player.transform.SetParent(transform);
        } else {
            Player.transform.SetParent(null);
        }
            




        /*if(transform.position.z <= 46f){
            timePass += Time.deltaTime;
            canMove = false;

            if(playerHasCrossed == false)
                OpenDoors(LDoors);
            
            if(timePass > coolDown){
                CloseDoors(LDoors);
                playerHasCrossed = true;
            }
        }*/
    }

    public void Movement(Vector3 dir){
        transform.Translate(speed * dir * Time.deltaTime);
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
}
