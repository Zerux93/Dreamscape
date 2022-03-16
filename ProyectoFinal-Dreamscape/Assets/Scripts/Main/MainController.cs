using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainController : MonoBehaviour
{
    public void StartGame(){
        SceneManager.LoadScene("01Estacion");
    }

    public void ExitGame(){
        Debug.Log("QUIT");
        Application.Quit();
    }
}
