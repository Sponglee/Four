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
    }


    public void CloseGameOver()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Main");
    }

}
