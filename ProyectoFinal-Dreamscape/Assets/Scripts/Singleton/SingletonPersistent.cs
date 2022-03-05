using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonPersistent<T> : MonoBehaviour where T : MonoBehaviour
{

    public static T instance { get; private set; }

    protected virtual void Awake(){
        if(instance == null){
            instance = (T)FindObjectOfType(typeof(T));
            DontDestroyOnLoad(gameObject);
        }else{
            Destroy(gameObject);
        }
    }
}