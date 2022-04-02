using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TunnelVolume : MonoBehaviour
{
    //[SerializeField] public GameObject tunnel;
    [SerializeField] public Volume volume;

    [SerializeField] public GameObject Player;
    [SerializeField] public Transform teleportSpot;
    [SerializeField] public Transform endPoint;

    private PlayerController playerController;
    bool playerHasEnter = false;
    

    ChromaticAberration chromaticAberration;
    LensDistortion lensDistortion;
    Bloom bloom;

    float moveDirectionZ;
    float clampMove;
    float initialDistance;
    float currentDistance;

    float smoothStepPosition;

    void Start()
    {
        

        volume.profile.TryGet<ChromaticAberration>(out chromaticAberration);
        volume.profile.TryGet<LensDistortion>(out lensDistortion);
        volume.profile.TryGet<Bloom>(out bloom);

        playerController = Player.GetComponent<PlayerController>();

        initialDistance = endPoint.position.z - teleportSpot.position.z;

    }

    void Update()
    {
        moveDirectionZ = playerController.moveDirection.z;
        clampMove = Mathf.Clamp(moveDirectionZ, -smoothStepPosition,smoothStepPosition);
        
        //Debug.Log(clampMove);


        if(playerHasEnter){
            currentDistance = (endPoint.position.z - Player.transform.position.z);

            smoothStepPosition = 1 - (currentDistance / initialDistance);

            //Debug.Log(smoothStepPosition);

            playerController.walkSpeed = 3+(3 * (10*smoothStepPosition));

            playerController.canSprint = false;
        }

    }

    private void FixedUpdate() {
        lensDistortion.intensity.value = clampMove;
        //Debug.Log(lensDistortion.intensity.value);
        chromaticAberration.intensity.value = smoothStepPosition;
        bloom.intensity.value = 10*smoothStepPosition;
        bloom.scatter.value = 0.3f + smoothStepPosition;
    }

    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Player"){
            playerHasEnter = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if(other.tag == "Player"){
            playerHasEnter = false;
            playerController.walkSpeed = 3;
            playerController.canSprint = true;
        }
    }
}
