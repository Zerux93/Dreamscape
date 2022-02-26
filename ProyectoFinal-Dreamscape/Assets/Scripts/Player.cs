using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement")]    
    [SerializeField] float speed = 3f;

    [Header("Look Parameters")]
    [SerializeField, Range(1,10)] private float lookSpeedX = 2.0f;
    [SerializeField, Range(1,10)] private float lookSpeedY = 2.0f;
    [SerializeField, Range(1,180)] private float upperLookLimit = 80.0f;
    [SerializeField, Range(1,180)] private float lowerLookLimit = 80.0f;

    [SerializeField] private float gravity = 30.0f;

    //Movement
    private Vector2 currentInput;
    private Vector3 moveDirection;    
    float xAxis;

    public Camera playerCamera;

    private CharacterController characterController;

    private float rotationX = 0;


 


    void Update()
    {
        Move();
        MouseLook();
        
        
        
    }

    void Awake() {
        characterController = GetComponent<CharacterController>();
    }

    public void Move(){
        currentInput = new Vector2(speed * Input.GetAxis("Vertical"),speed * Input.GetAxis("Horizontal"));
        moveDirection = (transform.TransformDirection(Vector3.forward)*currentInput.x)
         + (transform.TransformDirection(Vector3.right) * currentInput.y);        

        characterController.Move(moveDirection * Time.deltaTime);
    }

    public void MouseLook(){        
        rotationX -= Input.GetAxis("Mouse Y") * lookSpeedY;
        rotationX = Mathf.Clamp(rotationX, -upperLookLimit, lowerLookLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeedX, 0);
    }






}
