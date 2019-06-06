using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FunctionHandler : Singleton<FunctionHandler>
{

    public GameObject menuCam;
    public GameObject shopCam;

    public GameObject menuCanvas;
    public GameObject canvas;


    public GameObject menuButton;

    public Transform map;
    public GameObject mapElemRef;
    public Color unlockedMapColor;
    public Color lockedMapColor;
    public Color finishedColor;

    public bool LevelCompleteInProgress = false;

 

    private void Start()
    {
        Time.timeScale = 1;
        menuCam = SpawnManager.Instance.vcamMenu.gameObject;
        shopCam = SpawnManager.Instance.vcamShop.gameObject;
    }

    public void OpenGameOver(string message)
    {
        BallController.Instance.TapToStart = false;
        canvas.SetActive(false);
        BallController.Instance.MenuOpened = true;
        StartCoroutine(StopOpenGameOver(message));

       

    }

    public void CloseGameOver(bool menuClose = false)
    {
        
        //Enable effectHolder
        LevelManager.Instance.EffectHolder.gameObject.SetActive(true);
        canvas.SetActive(true);
        menuCam.SetActive(false);
        BallController.Instance.MenuOpened = false;
        //If menu is already open
        if (menuCanvas.activeSelf)
        {


            //GameOver menu close
            if (!menuClose)
            {
                //Time.timeScale = 1;


                SceneManager.LoadScene("Main");
            }
            else
            {

                //Time.timeScale = 1;
                //Disable menu screen
                menuCanvas.SetActive(false);
                menuCanvas.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
                //Enable menu button if game over or win
                menuButton.SetActive(true);


                
                BallController.Instance.RemoveCartBelow(15);
                GameManager.Instance.tapText.gameObject.SetActive(true);
            }
        }


    }

    public void ToggleShop()
    {
        if(shopCam.activeSelf)
        {
            shopCam.SetActive(false);
        }
        else
        {
            shopCam.SetActive(true);
        }
    }

    public IEnumerator StopOpenGameOver(string message)
    {
        
        GameManager.Instance.bestText.text = GameManager.Instance.bestScore.ToString();
        GameManager.Instance.menuScoreText.text = GameManager.Instance.Score.ToString();

        //if there's no message - mid game open or close menu
        if (message == "")
        {
            
            //Close menu if it's open and midgame
            if (menuCam.activeSelf)
            {
                CloseGameOver(true);
            }
            else
            {
                //Activate Menu screen
                menuCam.SetActive(true);

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
            //Disable fltText
            LevelManager.Instance.EffectHolder.gameObject.SetActive(false);
            //Activate Menu screen
            menuCam.SetActive(true);

            //Set message
            menuCanvas.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = message;

            //Disable menu button if game over or win
            menuButton.SetActive(false);
            //yield return new WaitForSeconds(0.21f);
            if(message != "GAME OVER")
            {
                menuCanvas.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
                if(!LevelCompleteInProgress)
                    StartCoroutine(StopMapProgression());
            }
            else
            {
                GameManager.Instance.Score = 0;
                PlayerPrefs.SetInt("Score", 0);
                //Time.timeScale = 0;
            }
          
        }
        yield return null;
    }


    public IEnumerator StopMapProgression()
    {
        LevelCompleteInProgress = true;
        //Debug.Log("STOPMAP");
        int tmpRank = PlayerPrefs.GetInt("CurrentRank",1);
        GameObject mapSegment = null;


        Transform lastSegment = null;
        Transform nextSegment = null;

        for (int i = Mathf.Clamp(tmpRank-100,0,tmpRank); i < tmpRank+1; i++)
        {
           

            if (i % 4 == 0)
            {
                //Debug.Log("NOW "+i + "(" + tmpRank + ")");
                mapSegment = Instantiate(mapElemRef, map);
                

            }

           

            if (mapSegment != null)
            {
               
                //Debug.Log("AND NOW" + i + "(" + tmpRank + ")");
                if (i != tmpRank)
                { 
                    
                    mapSegment.transform.GetChild(i % 4).GetComponent<Image>().color = finishedColor;
                    mapSegment.transform.GetChild(i % 4).GetChild(0).GetComponent<Text>().text = (i + 1).ToString();

                    if(i<tmpRank)
                    {
                       
                        //enable checkmark
                        //mapSegment.transform.GetChild(i % 4).GetChild(0).GetChild(0).gameObject.SetActive(true);
                     

                        //Disable gems icon
                        if (mapSegment.transform.GetChild(i%4).GetChild(0).childCount>1)
                        {
                            mapSegment.transform.GetChild(i % 4).GetChild(0).GetChild(1).gameObject.SetActive(false);
                           
                        }

                        if (i != tmpRank && i != tmpRank -1)
                        {
                            //Enable tower graphics
                            mapSegment.transform.GetChild(i % 4).GetChild(1).gameObject.SetActive(true);
                        }

                        //Get lastSegment
                        if (i == tmpRank -1)
                        {
                            mapSegment.transform.GetChild(i % 4).GetChild(0).GetChild(0).GetComponent<Image>().color = Color.clear;
                            lastSegment = mapSegment.transform.GetChild(i % 4);

                            //Disable model for later anim
                            mapSegment.transform.GetChild(i % 4).GetChild(1).GetChild(0).gameObject.SetActive(false);

                        }
                     
                      
                    }
                   
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
                  
                    //Unlock next level
                    nextSegment = mapSegment.transform.GetChild(i % 4);
                   
                    mapSegment.transform.GetChild(i % 4).GetChild(0).GetComponent<Text>().text = (i + 1).ToString();


                }

            }

          
          
        }

        yield return StopMapPan(lastSegment);
        //yield return StopColorLerp(lastSegment.GetChild(0).GetChild(0), unlockedMapColor);
        yield return StopColorLerp(nextSegment, finishedColor);

        yield return null;
    }

  

    public IEnumerator StopMapPan(Transform lastSegment = null)
    {
        //yield return new WaitForSeconds(0.2f);
        float elapsed = 0;
        float duration = 5f;
        int tmpRank = PlayerPrefs.GetInt("CurrentRank", 1);
        
        Vector3 startPos = map.transform.localPosition;
        Vector3 endPos;



        if (tmpRank < 12)
        {
            //Animation here
            lastSegment.GetChild(1).gameObject.SetActive(true);
            lastSegment.GetChild(1).GetComponent<Animator>().SetTrigger("Pan");
            //Enable model after anim
            lastSegment.GetChild(1).GetChild(0).gameObject.SetActive(true);
            yield return new WaitForSeconds(1f);
            yield break;
        }

        else if(tmpRank % 4 != 0)
        {
            endPos = -Vector3.right * (tmpRank / 4 -2)*100;
        }
        else
        {
            endPos = -Vector3.right * (tmpRank /4 - 2) * 100;

        }

        //Debug.Log(endPos);

        map.transform.localPosition = Vector3.zero;

        while (elapsed<duration)
        {
            //Debug.Log(map.transform.localPosition.x);
            elapsed += 0.2f;
            map.transform.localPosition = Vector3.Lerp(map.transform.localPosition, 
                        endPos, elapsed / duration);

            //Debug.Log("XXXXX " + (PlayerPrefs.GetInt("CurrentRank", 1) / 4 - 3) * 97f);
            yield return new WaitForEndOfFrame();
        }

        //Animation here
        lastSegment.GetChild(1).gameObject.SetActive(true);
        lastSegment.GetChild(1).GetComponent<Animator>().SetTrigger("Pan");
        //Enable model after anim
        lastSegment.GetChild(1).GetChild(0).gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
    }


    //Lerp the color
    private IEnumerator StopColorLerp(Transform target, Color destColor)
    {
        float elapsed = 0;
        float duration = 2f;

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


   public void StartPowerUp(int index)
    {
        switch (index)
        {
            //Shield
            case 0:
                {
                    if (!BallController.Instance.Shielded && GameManager.Instance.ShieldCount > 0)
                    {
                        GameManager.Instance.ShieldCount--;
                        BallController.Instance.Shielded = true;
                    }
                }
                break;
           
            //Magnet
            case 1:
                {
                    if (!BallController.Instance.Magnet && GameManager.Instance.MagnetCount>0)
                    {
                        GameManager.Instance.MagnetCount--;
                        BallController.Instance.Magnet = true;
                    }
                }
                break;
            //PoweredUp
            case 2:
                {
                    if (!BallController.Instance.PoweredUp && GameManager.Instance.PoweredUpCount > 0)
                    {
                        GameManager.Instance.PoweredUpCount--;
                        BallController.Instance.PowerUpTrigger = false;
                       
                        BallController.Instance.PoweredUp = true;


                    }
                }
                break;

            default:
                break;
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
