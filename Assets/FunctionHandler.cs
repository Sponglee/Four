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

    private void Start()
    {
        Time.timeScale = 1;
    }

    public void OpenGameOver(string message)
    {

        StartCoroutine(StopOpenGameOver(message));


    }

    public IEnumerator StopOpenGameOver(string message)
    {
        
        GameManager.Instance.bestText.text = GameManager.Instance.bestScore.ToString();
        

        //if there's no message - mid game open or close menu
        if (message == "")
        {
            Time.timeScale = 0;
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
            LevelManager.Instance.EffectHolder.gameObject.SetActive(false);
            //Activate Menu screen
            menuCanvas.SetActive(true);
            //Set message
            menuCanvas.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = message;

            //Disable menu button if game over or win
            menuButton.SetActive(false);
            //yield return new WaitForSeconds(0.21f);
            Time.timeScale = 0;

        }
        yield return null;
    }


    public void CloseGameOver(bool menuClose = false)
    {
       
        //If menu is already open
        if (menuCanvas.activeSelf)
        {
            

            //GameOver menu close
            if (!menuClose)
            {
                Time.timeScale = 1;

              
                SceneManager.LoadScene("Main");
            }
            else
            {
                Time.timeScale = 1;
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
        PlayerPrefs.SetInt("CurrentRank", Convert.ToInt32(levels.text));
        SceneManager.LoadScene("Main");
    }

    public void SetSpeed(InputField speed)
    {
        PlayerPrefs.SetFloat("Speed", float.Parse(speed.text));
        SceneManager.LoadScene("Main");
    }


    public void ResetData()
    {
        PlayerPrefs.DeleteAll();
    }
}
