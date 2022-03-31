using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessing : MonoBehaviour
{
    public Volume volume;
    [SerializeField]private float intensityValue = 0;

    private Vignette vignette;


    private void Update() {
        if(volume.profile.TryGet<Vignette>(out vignette)){
            vignette.intensity.value = intensityValue;
        }
    }
}
