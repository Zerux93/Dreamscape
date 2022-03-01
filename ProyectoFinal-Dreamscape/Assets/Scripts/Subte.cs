using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Subte : MonoBehaviour
{
    [SerializeField] private float speed = 15f;
    private bool canMove = true;

    [SerializeField] private Transform trainA;
    [SerializeField] private Transform trainB;

    [SerializeField] private Animator[] RDoors;
    [SerializeField] private Animator[] LDoors;  

    public Transform Player;

    //bobmovement
    [SerializeField] private float bobMovementSpeed = 2f; 
    [SerializeField] private float bobMovementAmount = 0.001f; 
    //private float TrainAdefaultYPos = 0;
    //private float TrainBdefaultYPos = 0;

    public float timePass = 0;
    public float coolDown = 5f; 
    private bool playerHasCrossed = false; 

    Vector3 forwardDir = Vector3.forward;
    Vector3 backDir = Vector3.back;

    bool fistStation = false;
    bool secondStation = false;
    bool isMoving = false;

    float timer;



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

        if(canMove){
            BobMovement(trainA);
        }
    }

    private void FixedUpdate() {
        if(canMove){;
            BobMovement(trainB);
        }
    }

    void Awake() {
        //TrainAdefaultYPos = trainA.transform.localPosition.y;
        //TrainBdefaultYPos = trainA.transform.localPosition.y;
    }

    public void Movement(Vector3 dir){
        transform.Translate(speed * dir * Time.deltaTime);
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
}
