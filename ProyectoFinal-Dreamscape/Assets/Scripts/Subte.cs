using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Subte : MonoBehaviour
{
    public float speed = 15f;
    private bool canMove = true;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(canMove)
            Movement();
    }

    public void Movement(){
        transform.Translate(speed * Vector3.back * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other) {
        if(other.tag == "StopTrigger")
            canMove = false;
    }
}
