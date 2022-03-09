using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckManager : MonoBehaviour
{
    
    [SerializeField] private GameObject[] chekpoint;

    public List<string> listCheckpoints = new List<string>();

    // Start is called before the first frame update
    void Update()
    {

    }
    
    public void AddCheckpoint(GameObject g){
        listCheckpoints.Add(g.name);
        foreach(string s in listCheckpoints){
            Debug.Log(s);
        }
    }


}
