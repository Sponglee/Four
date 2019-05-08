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

    public Transform map;
    public GameObject mapElemRef;
    public Color unlockedMapColor;
    public Color lockedMapColor;
    public Color finishedColor;

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
        GameManager.Instance.menuScoreText.text = GameManager.Instance.Score.ToString();

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
            if(message == "LEVEL COMPLETE")
                StartCoroutine(StopMapProgression());
            Time.timeScale = 0;
        }
        yield return null;
    }


    public IEnumerator StopMapProgression()
    {
        Debug.Log("Opened");
        int tmpRank = PlayerPrefs.GetInt("CurrentRank",1);
        GameObject mapSegment = null;

       

        for (int i = Mathf.Clamp(tmpRank-100,0,tmpRank); i < tmpRank+1; i++)
        {
           
            if (i % 4 == 0)
            {
                mapSegment = Instantiate(mapElemRef, map);
            }
            
            if(mapSegment != null)
            {
                
                if (i != tmpRank)
                {
                    mapSegment.transform.GetChild(i % 4).GetComponent<Image>().color = finishedColor;
                    mapSegment.transform.GetChild(i % 4).GetChild(0).GetComponent<Text>().text = (i + 1).ToString();
                    //Color current level as finished
                    //if (i == tmpRank - 1)
                    //{
                    //    Debug.Log("YEET");
                    //    yield return StopMapPan();
                    //    mapSegment.transform.GetChild(i % 4).GetChild(0).GetComponent<Text>().text = (i + 1).ToString();
                    //    yield return StopColorLerp(mapSegment.transform.GetChild(i % 4), finishedColor);
                    //}
                }
              
                else
                {
                    yield return StopMapPan();
                    //Unlock next level
                    yield return StopColorLerp(mapSegment.transform.GetChild(i % 4), unlockedMapColor);
                    mapSegment.transform.GetChild(i % 4).GetChild(0).GetComponent<Text>().text = (i + 1).ToString();


                }
            }
               
           
          
        }




        yield return null;
    }



    public IEnumerator StopMapPan()
    {
        //yield return new WaitForSeconds(0.2f);
        float elapsed = 0;
        float duration = 5f;
        int tmpRank = PlayerPrefs.GetInt("CurrentRank", 1);
        
        Vector3 startPos = map.transform.localPosition;
        Vector3 endPos;



        if (tmpRank < 12)
            yield break;
        else if(tmpRank % 4 != 0)
        {
            endPos = -Vector3.right * (tmpRank / 4 -2)*100;
        }
        else
        {
            endPos = -Vector3.right * (tmpRank /4 - 2) * 100;

        }

        Debug.Log(endPos);

        map.transform.localPosition = Vector3.zero;

        while (elapsed<duration)
        {
            Debug.Log(map.transform.localPosition.x);
            elapsed += 0.2f;
            map.transform.localPosition = Vector3.Lerp(map.transform.localPosition, 
                        endPos, elapsed / duration);

            //Debug.Log("XXXXX " + (PlayerPrefs.GetInt("CurrentRank", 1) / 4 - 3) * 97f);
            yield return new WaitForEndOfFrame();
        }
    }
    

    //Lerp the color
    private IEnumerator StopColorLerp(Transform target, Color destColor)
    {
        float elapsed = 0;
        float duration = 5f;

        while (elapsed < duration)
        {


            elapsed += 0.1f;


            target.GetComponent<Image>().color
                = Color.Lerp(lockedMapColor,
                destColor, elapsed / duration);

            //Debug.Log(">><<");
            yield return null;
        }
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
