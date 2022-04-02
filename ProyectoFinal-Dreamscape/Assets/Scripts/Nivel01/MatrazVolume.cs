using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MatrazVolume : MonoBehaviour
{
    [SerializeField] public Volume volume;

    [SerializeField] public GameObject Player;
    private PlayerController playerController;

    [SerializeField] private float waveMovementSpeed = 4f; 
    [SerializeField] private float waveMovementAmount = 0.45f; 

    LensDistortion lensDistortion;

    bool playerInside = false;

    private float timer;
    private float timePass = 0;
    private float coolDown = 3f;
    private float coolDown2 = 5f; 
    private float bobCount;
    


    // Start is called before the first frame update
    void Start()
    {
        playerController = Player.GetComponent<PlayerController>();

        volume.profile.TryGet<LensDistortion>(out lensDistortion);
    }

    // Update is called once per frame
    void Update()
    {
        if(playerInside)
            WaveMovement();
    }

    public void WaveMovement(){
        timer += Time.deltaTime * waveMovementSpeed;
        bobCount = 0+ Mathf.Sin(timer) * waveMovementAmount;
        Debug.Log(bobCount);

    }

    private void FixedUpdate() {
        lensDistortion.intensity.value = bobCount;
    }

    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Player"){
            playerController.canWalk = false;
            playerController.canSwim = true;

            playerInside = true;
        }
    }

}
