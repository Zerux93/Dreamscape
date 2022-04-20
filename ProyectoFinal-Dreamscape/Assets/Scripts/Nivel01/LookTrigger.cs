using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookTrigger : Interactable
{

    [SerializeField] Transform subte;
    [SerializeField] Transform waypoint;
    [SerializeField] private float speed = 50f;
    private bool canMove = false;

    public override void OnFocus()
    {
        Debug.Log("MIRO");
        print("Looking at");
        canMove = true;        
    }

    private void Update() {

        if(canMove)
            Movement(waypoint);
    }

    private void Movement(Transform waypoint){
        Vector3 deltaVector = new Vector3(0,0,waypoint.position.z - subte.transform.position.z);
        Vector3 direction = deltaVector.normalized;
        subte.transform.position += direction * speed * Time.deltaTime;
    }

    public override void OnInteract(){

    }

    public override void OnLoseFocus()
    {

    }  
}
