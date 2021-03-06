using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public bool CanMove{ get; private set; } = true;
    private bool IsSprinting => canSprint && Input.GetKey(sprintKey);
    private bool ShouldJump => Input.GetKeyDown(jumpKey) && characterController.isGrounded;
    private bool ShouldCrouch => Input.GetKeyDown(crouchKey) && !duringCrouchAnimation && characterController.isGrounded;
    private bool isSwimming = false;

    [Header("Functional Options")]
    [SerializeField] public bool canWalk = true;
    [SerializeField] public bool canSprint = true;
    [SerializeField] private bool canJump = true;
    [SerializeField] private bool canCrouch = true;
    [SerializeField] private bool canUseHeadbob = true;
    [SerializeField] private bool willSlideOnSlopes = true;
    [SerializeField] private bool canZoom = true;
    [SerializeField] private bool canInteract = true;
    [SerializeField] private bool useFootsteps = true;
    [SerializeField] public bool canSwim = false;



    [Header("Controls")]
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;
    [SerializeField] private KeyCode zoomKey = KeyCode.Mouse1;
    [SerializeField] private KeyCode interactionKey = KeyCode.Mouse0;


    [Header("Movement Parameters")]
    [SerializeField] public float walkSpeed = 3.0f;
    [SerializeField] public float sprintSpeed = 6.0f;  
    [SerializeField] private float crouchSpeed = 1.5f;  
    [SerializeField] private float slopeSpeed = 8f;  
    [SerializeField] private float swimmingSpeed = 5f;


    [Header("Look Parameters")]
    [SerializeField, Range(1,10)] private float lookSpeedX = 2.0f;
    [SerializeField, Range(1,10)] private float lookSpeedY = 2.0f;
    [SerializeField, Range(1,180)] private float upperLookLimit = 80.0f;
    [SerializeField, Range(1,180)] private float lowerLookLimit = 80.0f;

    [Header("Jumping Parameters")]
    [SerializeField] public float jumpForce = 8.0f;
    [SerializeField] public float gravity = 30.0f;
    [SerializeField] private float swimGravity = 5f;

    [Header("Crouch Parameters")]    
    [SerializeField] private float crouchHeight = 0.5f;
    [SerializeField] private float standingHeight = 2f;
    [SerializeField] private float timeToCrouch = 0.25f;
    [SerializeField] private Vector3 crouchingCenter = new Vector3(0,0.5f,0);
    [SerializeField] private Vector3 standingCenter = new Vector3(0,0,0);
    private bool isCrouching;
    private bool duringCrouchAnimation;    

    [Header("Headbob Parameters")]
    [SerializeField] private float walkBobSpeed = 14f;
    [SerializeField] private float walkBobAmount = 0.05f;
    [SerializeField] private float sprintBobSpeed = 18f;
    [SerializeField] private float sprintBobAmount = 0.1f;
    [SerializeField] private float crouchBobSpeed = 8f;
    [SerializeField] private float crouchBobAmount = 0.025f;
    private float defaultYPos = 0;
    private float timer;

    [Header("Zoom Parameters")]
    [SerializeField] private float timeToZoom = 0.3f;
    [SerializeField] private float zoomFOV = 30f;
    private float defaultFOV;
    private Coroutine zoomRoutine;

    [Header("Footsteps Parameters")]
    [SerializeField] private float baseStepSpeed = 0.5f;
    [SerializeField] private float crouchStepMultiplier = 1.5f;
    [SerializeField] private float sprintStepMultiplier = 0.6f;
    [SerializeField] private AudioSource footstepAudioSource = default;
    [SerializeField] private AudioClip[] woodClip = default;
    [SerializeField] private AudioClip[] metalClip = default;
    [SerializeField] private AudioClip[] tileClip = default;
    private float footStepTimer = 0;
    private float GetCurrentOffset => isCrouching ? baseStepSpeed * crouchStepMultiplier :
                                      IsSprinting ? baseStepSpeed * sprintStepMultiplier :
                                      baseStepSpeed;


    //Sliding Parameters
    private Vector3 hitPointNormal;
    private bool isSliding{
        get{
            //Debug.DrawRay(transform.position, Vector3.down, Color.red);
            if(characterController.isGrounded && Physics.Raycast(transform.position, Vector3.down, out RaycastHit slopeHit, 4f)){
                hitPointNormal = slopeHit.normal;
                return Vector3.Angle(hitPointNormal, Vector3.up) > characterController.slopeLimit;
            }else{
                return false;
            }
        }
    }

    [Header("Interactions")]
    [SerializeField] private Vector3 interactionRayPoint = default;
    [SerializeField] private float interactionDistance = default;
    [SerializeField] private LayerMask interactionLayer = default;
    private Interactable currentInteractable;

    private Camera playerCamera;
    private CharacterController characterController;

    public Vector3 moveDirection;
    private Vector2 currentInput;

    private float rotationX = 0;

    public static PlayerController instance;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        
        playerCamera = GetComponentInChildren<Camera>();
        characterController = GetComponent<CharacterController>();
        defaultYPos = playerCamera.transform.localPosition.y;
        defaultFOV = playerCamera.fieldOfView;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;        
    }

    // Update is called once per frame
    void Update()
    {
        HandleMouseLook();

        if(canZoom)
            HandleZoom();

        if(canInteract){
            HandleIntectationCheck();
            HandleInteractionInput(); 
        }  

        if (CanMove && canWalk){

            HandleMovementInput();         

            if(canJump)
                HandleJump();

            if(canCrouch)
                HandleCrouch();

            if(canUseHeadbob)
                HandleHeadBob();

            if(useFootsteps)
                HandleFootsteps();
                

            ApplyFinalMovements();
        }

        if(canSwim){
            HandleMovementInput();
            HandleMouseLook();
            HandleSwimmingMovements();
        }

        //Debug.Log(moveDirection.z);
        
    }

    //Keyword controll
    private void HandleMovementInput(){
        currentInput = new Vector2((isCrouching ? crouchSpeed : IsSprinting ? sprintSpeed : isSwimming ? swimmingSpeed : walkSpeed) * Input.GetAxis("Vertical"),
                                   (isCrouching ? crouchSpeed : IsSprinting ? sprintSpeed : isSwimming ? swimmingSpeed : walkSpeed) * Input.GetAxis("Horizontal"));

        float moveDirectionY = moveDirection.y;
        moveDirection = (transform.TransformDirection(Vector3.forward)*currentInput.x)
         + (transform.TransformDirection(Vector3.right) * currentInput.y);

        moveDirection.y = moveDirectionY;
    }

    private void HandleMouseLook(){
        rotationX -= Input.GetAxis("Mouse Y") * lookSpeedY;
        rotationX = Mathf.Clamp(rotationX, -upperLookLimit, lowerLookLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeedX, 0);
    }

    private void HandleJump(){
        if(ShouldJump)
            moveDirection.y = jumpForce;
    }

    private void HandleCrouch(){
        if(ShouldCrouch)
            StartCoroutine(CrouchStand());
    }

    private void HandleHeadBob(){
        if(!characterController.isGrounded) return;

        if(Mathf.Abs(moveDirection.x) > 0.1f || Mathf.Abs(moveDirection.z) > 0.1f ){
            timer += Time.deltaTime * (isCrouching ? crouchBobSpeed : IsSprinting ? sprintBobSpeed : walkBobSpeed);
            playerCamera.transform.localPosition = new Vector3(
                playerCamera.transform.localPosition.x,
                defaultYPos + Mathf.Sin(timer) * (isCrouching ? crouchBobAmount : IsSprinting ? sprintBobAmount : walkBobAmount),
                playerCamera.transform.localPosition.z);
        }
    }

    private void HandleZoom(){
        if(Input.GetKeyDown(zoomKey)){
            if(zoomRoutine != null){
                StopCoroutine(zoomRoutine);
                zoomRoutine = null;
            }
            zoomRoutine = StartCoroutine(ToggleZoom(true));
        }

        if(Input.GetKeyUp(zoomKey)){
            if(zoomRoutine != null){
                StopCoroutine(zoomRoutine);
                zoomRoutine = null;
            }
            zoomRoutine = StartCoroutine(ToggleZoom(false));
        }
    }

    private void HandleIntectationCheck(){
        if(Physics.Raycast(playerCamera.ViewportPointToRay(interactionRayPoint),
            out RaycastHit hit, interactionDistance))
        {
            if(hit.collider.gameObject.layer == 3 && (currentInteractable == null ||
                hit.collider.gameObject.GetInstanceID() != currentInteractable.GetInstanceID())){
                    
                    hit.collider.TryGetComponent(out currentInteractable);

                if(currentInteractable){
                    currentInteractable.OnFocus();
                }
            }
        } else if(currentInteractable){
            currentInteractable.OnLoseFocus();
            currentInteractable = null;
        }
    }

    private void HandleInteractionInput(){
        if(Input.GetKeyDown(interactionKey) && currentInteractable != null
            && Physics.Raycast(playerCamera.ViewportPointToRay(interactionRayPoint),
            out RaycastHit hit, interactionDistance, interactionLayer))
        {
            currentInteractable.OnInteract();
        }
    }

    private void HandleFootsteps(){
        if(!characterController.isGrounded) return;
        if(currentInput == Vector2.zero) return;

        footStepTimer -= Time.deltaTime;

        if(footStepTimer <= 0){
            if(Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 5)){
                //Debug.DrawRay(transform.position, Vector3.down, Color.red);
                switch(hit.collider.tag){
                    case "Footsteps/WOOD":
                        footstepAudioSource.PlayOneShot(woodClip[Random.Range(0, woodClip.Length-1)]);
                        Debug.Log("WOOD");
                        break;
                    case "Footsteps/METAL":
                        footstepAudioSource.PlayOneShot(metalClip[Random.Range(0, metalClip.Length-1)]);
                        Debug.Log("METAL");
                        break;
                    case "Footsteps/TILE":
                        footstepAudioSource.PlayOneShot(tileClip[Random.Range(0, tileClip.Length-1)]);
                        Debug.Log("TILE");
                        break;
                    default:
                        footstepAudioSource.PlayOneShot(tileClip[Random.Range(0, tileClip.Length-1)]);
                        //Debug.Log("DEFAULT");
                        break;
                }
            }
            footStepTimer = GetCurrentOffset;
        }
    }

    private void ApplyFinalMovements(){

        if (characterController.velocity.y < -1 && characterController.isGrounded)
        moveDirection.y = 0;

        if(willSlideOnSlopes && isSliding)
            moveDirection += new Vector3(hitPointNormal.x, -hitPointNormal.y, hitPointNormal.z)
                * slopeSpeed;

        if(!characterController.isGrounded){
            moveDirection.y -= gravity * Time.deltaTime;
        }

        characterController.Move(moveDirection * Time.deltaTime);
    }

    private void HandleSwimmingMovements(){

        isSwimming = true;

        if (characterController.velocity.y < -1)
        moveDirection.y = 0;

        if(!characterController.isGrounded){
            moveDirection.y -= swimGravity * Time.deltaTime;
        }

        characterController.Move(moveDirection * Time.deltaTime);

        if(Input.GetKeyDown(jumpKey))
            moveDirection.y = jumpForce;

        //if(Input.GetKeyDown(sprintKey))

    }


    private IEnumerator CrouchStand(){

        if(isCrouching && Physics.Raycast(playerCamera.transform.position, Vector3.up, 1f))
            yield break;

        duringCrouchAnimation = true;

        float timeElapsed = 0;
        float targetHeight = isCrouching ? standingHeight : crouchHeight;
        float currentHeight = characterController.height;
        Vector3 targetCenter = isCrouching ? standingCenter : crouchingCenter;
        Vector3 currentCenter = characterController.center;

        while(timeElapsed < timeToCrouch){
            characterController.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed/timeToCrouch);
            characterController.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed/timeToCrouch);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        characterController.height = targetHeight;
        characterController.center = targetCenter;

        isCrouching = !isCrouching;

        duringCrouchAnimation = false;
    }

    private IEnumerator ToggleZoom(bool isEnter){
        float targetFOV = isEnter ? zoomFOV : defaultFOV;
        float startingFOV = playerCamera.fieldOfView;
        float timeElapsed = 0;

        while(timeElapsed < timeToZoom){
            playerCamera.fieldOfView = Mathf.Lerp(startingFOV, targetFOV, timeElapsed/timeToZoom);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        playerCamera.fieldOfView = targetFOV;
        zoomRoutine = null;
    }
}
