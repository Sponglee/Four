using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FunctionHandler : Singleton<FunctionHandler>
{
    public GameObject menuCanvas;


   
    public void OpenGameOver(string message)
    {
        menuCanvas.SetActive(true);
        menuCanvas.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = message;
        Time.timeScale = 0;

        if(message == "")
        {
            menuCanvas.transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
            menuCanvas.transform.GetChild(0).GetChild(3).gameObject.SetActive(true);
        }
    }


    public void CloseGameOver(bool menuClose = false)
    {
        Time.timeScale = 1;
        if(!menuClose)    
            SceneManager.LoadScene("Main");
        else
        {
            menuCanvas.SetActive(false);
            menuCanvas.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
            menuCanvas.transform.GetChild(0).GetChild(3).gameObject.SetActive(false);
        }
    }


    public void SwitchMode()
    {
        SpawnManager.Instance.gameMode = !SpawnManager.Instance.gameMode;
    }
}
