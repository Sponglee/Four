using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FunctionHandler : Singleton<FunctionHandler>
{
    public GameObject menuCanvas;
    public GameObject menuButton;


   
    public void OpenGameOver(string message)
    {
     
       
        //if there's no message - mid game open or close menu
        if (message == "")
        {
           
            //Close menu if it's open and midgame
            if (menuCanvas.activeSelf)
            {
                CloseGameOver(true);
            }
            else
            {
                //Activate Menu screen
                menuCanvas.SetActive(true);
                //Set message
                menuCanvas.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = message;
                Time.timeScale = 0;


                menuCanvas.transform.GetChild(0).GetChild(2).gameObject.SetActive(true);

                if (SpawnManager.Instance.gameMode)
                {
                    menuCanvas.transform.GetChild(0).GetChild(2).GetChild(1).GetComponent<Image>().color = Color.red;
                }
                else
                {
                    menuCanvas.transform.GetChild(0).GetChild(2).GetChild(1).GetComponent<Image>().color = Color.white;
                }
            }
        }
        else if (message != "")
        {
            //Activate Menu screen
            menuCanvas.SetActive(true);
            //Set message
            menuCanvas.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = message;
            Time.timeScale = 0;

            //Disable menu button if game over or win
            menuButton.SetActive(false);
        }
        
       
    }


    public void CloseGameOver(bool menuClose = false)
    {
        //If menu is already open
        if(menuCanvas.activeSelf)
        {
            Time.timeScale = 1;

            //GameOver menu close
            if (!menuClose)
                SceneManager.LoadScene("Main");
            else
            { 
                //Disable menu screen
                menuCanvas.SetActive(false);
                menuCanvas.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
                
            }
        }
       
       
    }


    public void SwitchMode()
    {
        SpawnManager.Instance.gameMode = !SpawnManager.Instance.gameMode;
        PlayerPrefs.SetInt("GameMode", (SpawnManager.Instance.gameMode ? 1 : 0));


        //*****
        //PlayerPrefs.SetInt("Name", (yourBool ? 1 : 0));
        //yourBool = (PlayerPrefs.GetInt("Name") != 0);
        //
    }


    public void SetLevelCount (InputField levels)
    {
        PlayerPrefs.SetInt("LevelCount", Convert.ToInt32(levels.text));
        SceneManager.LoadScene("Main");
    }
}
