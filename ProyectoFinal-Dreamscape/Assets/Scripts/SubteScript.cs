using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubteScript : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 15f;
    //bobmovement
    public float bobMovementSpeed = 2f; 
    public float bobMovementAmount = 0.001f; 

    public bool canMove = true;

    [Header("Doors")]
    public Animator[] RDoors;
    public Animator[] LDoors; 

    [Header("Player")]
    public Transform Player;

    public float timer;

    public void Movement(Transform waypoint){
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

}
