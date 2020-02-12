using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//using UnityEngine.Advertisments;

public class FunctionHandler : Singleton<FunctionHandler>
{
    public Transform resumeReference;
    public Transform restartReference;


    public GameObject menuCam;
    public GameObject windowCam;

    public GameObject shopHolder;
    public Transform shopElements;

    public GameObject chestHolder;
    public GameObject menuCanvas;
    public GameObject canvasUI;


    public GameObject menuButton;

    public Transform map;
    public GameObject mapElemRef;
    public Color unlockedMapColor;
    public Color lockedMapColor;
    public Color finishedColor;

    public bool LevelCompleteInProgress = false;
    public bool GameOverInProgress = false;

    private void Start()
    {
        Time.timeScale = 1;
        menuCam = SpawnManager.Instance.vcamMenu.gameObject;
        windowCam = SpawnManager.Instance.vcamShop.gameObject;


        //Check volume 
        int volToggle = PlayerPrefs.GetInt("VolumeMute", 0);

        if (volToggle == 1)
        {
            Debug.Log("MUTED??");

            volumeMuted = true;
            AudioManager.Instance.VolumeMute(volumeMuted);
            volumeRef.GetChild(0).gameObject.SetActive(!volumeMuted);
            volumeRef.GetChild(1).gameObject.SetActive(volumeMuted);
        }
        else
        {
            volumeMuted = false;
            AudioManager.Instance.VolumeMute(volumeMuted);
            volumeRef.GetChild(0).gameObject.SetActive(!volumeMuted);
            volumeRef.GetChild(1).gameObject.SetActive(volumeMuted);
        }
    }

    public void OpenGameOver(string message)
    {
        AudioManager.Instance.PlaySound("MenuSwoop");
        AudioManager.Instance.StopSound("Wind");
        AudioManager.Instance.StopSound("FireTrail");





        BallController.Instance.TapToStart = false;
        canvasUI.SetActive(false);
        BallController.Instance.MenuOpened = true;
        StartCoroutine(StopOpenGameOver(message));
        
       

    }


    public void CloseGameOver(bool menuClose = false)
    {
      
        StartCoroutine(StopCloseGameOver(menuClose));
    }
    public IEnumerator StopCloseGameOver(bool menuClose = false)
    {
        LevelCompleteInProgress = false;
        //Enable effectHolder
        LevelManager.Instance.EffectHolder.gameObject.SetActive(true);
       
        BallController.Instance.MenuOpened = false;

        //If menu is already open
        if (menuCam.activeSelf)
        {
          
            //GameOver menu close
            if (!menuClose)
            {
                //Time.timeScale = 1;


                SceneManager.LoadScene("Main");
            }
            //MidGame Close menu
            else
            {
                
                if (GameOverInProgress)
                {
                    //VOODOO TEST
                    if (GameManager.Instance.Gems >= 100)
                    {
                        GameManager.Instance.Gems -= 100;

                        //Close menu
                        yield return StartCoroutine(CloseGameOverAction());
                    }
                    else
                    {
                        AudioManager.Instance.PlaySound("No");
                      
                    }
                    //VOODOO TEST
                }
                else
                {
                    //Close menu
                    yield return StartCoroutine(CloseGameOverAction());



                    BallController.Instance.RemoveCartBelow(15);

                    GameManager.Instance.tapObject.gameObject.SetActive(true);
                }
             
            }
        }


    }


    public IEnumerator CloseGameOverAction()
    {
        AudioManager.Instance.PlaySound("MenuSwoop");
        menuCam.SetActive(false);
        canvasUI.SetActive(true);
        GameOverInProgress = false;
        //Time.timeScale = 1;
        menuButton.SetActive(true);
        //menuCanvas.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
        //Disable menu screen
        yield return new WaitForSeconds(0.21f);
        menuCanvas.SetActive(false);
        BallController.Instance.RemoveCartBelow(15);
        GameManager.Instance.tapObject.gameObject.SetActive(true);
    }



    public void ToggleMenuWindow(int targetIndex)
    {
        GameObject target = null;
        Debug.Log(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> " + PlayerPrefs.GetInt("KeyCount",0));
        switch (targetIndex)
        {
            //shop window
            case 0:
                target = shopHolder;
                break;
            //chest window
            case 1:
                {
                    target = chestHolder;
             
                }
                break;
            case 2:
                break;
            default:
                break;
        }


        if(windowCam.activeSelf)
        {
            GameManager.Instance.ResetAnims();
            StartCoroutine(StopWindow(target));
            if (target == chestHolder)
            {
                //GameManager.Instance.ChestDespawn();
                if(GameManager.Instance.KeyCount==0)
                {
                    //Enable buttons 
                    menuCanvas.transform.GetChild(3).gameObject.SetActive(false);
                    menuCanvas.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);


                }

            }
        }
        else
        {

            windowCam.SetActive(true);
            target.SetActive(true);
            if(target == chestHolder)
            {
                if(GameManager.Instance.ChestSpawnedBool == false)
                    GameManager.Instance.ChestSpawn();
            }
        }
        AudioManager.Instance.PlaySound("MenuSwoop");
    }

    public IEnumerator StopWindow(GameObject target)
    {
        //yield return new WaitForSeconds(0.61f);
       
        windowCam.SetActive(false);
        yield return new WaitForSeconds(0.21f);

        target.SetActive(false);
    }



    public IEnumerator StopOpenGameOver(string message)
    {
       //Move menu to camera and enable it
        menuCanvas.transform.parent.position = new Vector3(menuCanvas.transform.parent.position.x, BallController.Instance.transform.position.y + 200f, menuCanvas.transform.parent.position.z);
        menuCanvas.SetActive(true);
        GameManager.Instance.bestText.text = GameManager.Instance.bestScore.ToString();
        GameManager.Instance.menuScoreText.text = GameManager.Instance.Score.ToString();
      

        //if there's no message - mid game open or close menu
        if (message == "PAUSE")
        {

            //Set button icons for midgame pause
            resumeReference.GetChild(0).gameObject.SetActive(false);
            resumeReference.GetChild(1).gameObject.SetActive(true);
            
            restartReference.GetChild(0).gameObject.SetActive(false);
            restartReference.GetChild(1).gameObject.SetActive(true);

            //Close menu if it's open and midgame
            if (menuCam.activeSelf)
            {
                CloseGameOver(true);
            }
            else
            {
                //Activate Menu screen
                menuCam.SetActive(true);
                yield return new WaitForSeconds(0.4f);

                //Set message
                menuCanvas.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = message;

                //Enable continue

                menuCanvas.transform.GetChild(0).GetChild(0).GetChild(1).gameObject.SetActive(true);

                //if (SpawnManager.Instance.gameMode)
                //{
                //    menuCanvas.transform.GetChild(0).GetChild(2).GetChild(1).GetComponent<Image>().color = Color.red;
                //}
                //else
                //{
                //    menuCanvas.transform.GetChild(0).GetChild(2).GetChild(1).GetComponent<Image>().color = Color.white;
                //}


            }
        }
        else if (message != "PAUSE")
        {
            if (!LevelCompleteInProgress)
            {
                LevelCompleteInProgress = true;
                //Disable fltText
                LevelManager.Instance.EffectHolder.gameObject.SetActive(false);
                //Activate Menu screen
                menuCam.SetActive(true);
                yield return new WaitForSeconds(0.4f);


                //Enable message and set it
                //menuCanvas.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
                menuCanvas.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
                menuCanvas.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = message;

                //Disable menu button if game over or win
                menuButton.SetActive(false);
                //yield return new WaitForSeconds(0.21f);

                //Level Complete
                if (message != "GAME OVER")
                {

                    //Set button icons for gameOver
                    resumeReference.GetChild(0).gameObject.SetActive(false);
                    resumeReference.GetChild(1).gameObject.SetActive(true);
                    restartReference.GetChild(0).gameObject.SetActive(true);
                    restartReference.GetChild(1).gameObject.SetActive(false);



                    
                    //disable continue
                    menuCanvas.transform.GetChild(0).GetChild(0).GetChild(1).gameObject.SetActive(false);

                   
                    //Enable message and set it
                    //menuCanvas.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
                    menuCanvas.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
                    menuCanvas.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = message;



                    if (GameManager.Instance.KeyCount >= 3)
                    {
                        //disable continue
                        menuCanvas.transform.GetChild(3).gameObject.SetActive(true);
                        //disable buttons
                        menuCanvas.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
                    }


                        yield return StartCoroutine(StopMapProgression());
                 
                }
                else
                {
                    GameOverInProgress = true;
                    resumeReference.gameObject.SetActive(false);

                    //Set button icons for gameOver
                    resumeReference.GetChild(0).gameObject.SetActive(true);
                    resumeReference.GetChild(1).gameObject.SetActive(false);
                    restartReference.GetChild(0).gameObject.SetActive(false);
                    restartReference.GetChild(1).gameObject.SetActive(true);





                    //Enable Lower message and set it
                    menuCanvas.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
                    menuCanvas.transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
                    menuCanvas.transform.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>().text = message;

                    //Animate timeout
                    StartCoroutine(TimeOutButton(resumeReference, restartReference));

                    //Set message
                    menuCanvas.transform.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>().text = string.Format("LEVEL {0}\n {1}%", PlayerPrefs.GetInt("CurrentRank",1),
                        ((int)((float)BallController.Instance.CurrentLevel/(float)LevelManager.Instance.levelCount*100f)).ToString());

                    //GameManager.Instance.Score = 0;
                    PlayerPrefs.SetInt("Score", 0);
                    //Time.timeScale = 0;

                 
                }

               
            }

           
   

        }

        yield return null;
    }



    public IEnumerator TimeOutButton(Transform bounceRef, Transform hideRef)
    {
        //make it bounce
        bounceRef.gameObject.SetActive(true);


       

        bounceRef.GetComponent<Animator>().SetTrigger("Bounce");
        Debug.Log("BOUNCE " + bounceRef.GetComponent<Animator>().isActiveAndEnabled);

        yield return null;
       

        yield return new WaitForSeconds(1.8f);

        hideRef.gameObject.SetActive(true);

       
    }




    public IEnumerator StopMapProgression()
    {
        //LevelCompleteInProgress = true;
        //Debug.Log("STOPMAP");
        int tmpRank = PlayerPrefs.GetInt("CurrentRank",1);
        Debug.Log("RANK " + tmpRank);
        GameObject mapSegment = null;


        Transform lastSegment = null;
        Transform nextSegment = null;

        for (int i = Mathf.Clamp(tmpRank-100,0,tmpRank); i < tmpRank+1; i++)
        {
           

            if (i % 4 == 0)
            {
                //Debug.Log("NOW " + i + "(" + tmpRank + ")");
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
                        mapSegment.transform.GetChild(i % 4).GetChild(0).GetChild(0).gameObject.SetActive(true);


                        //Disable gems icon
                        if (mapSegment.transform.GetChild(i%4).GetChild(0).childCount>1)
                        {
                            mapSegment.transform.GetChild(i % 4).GetChild(0).GetChild(1).gameObject.SetActive(false);
                           
                        }

                        if (i != tmpRank && i != tmpRank -1)
                        {
                            //Enable tower graphics
                            mapSegment.transform.GetChild(i % 4).GetChild(0).gameObject.SetActive(true);
                        }

                        //Get lastSegment
                        if (i == tmpRank -1)
                        {
                            //mapSegment.transform.GetChild(i % 4).GetChild(0).GetChild(0).GetComponent<Image>().color = Color.clear;
                            lastSegment = mapSegment.transform.GetChild(i % 4);

                            //Disable model for later anim
                            mapSegment.transform.GetChild(i % 4).GetChild(1).GetChild(0).gameObject.SetActive(false);
                            //enable checkmark fir later anim
                            mapSegment.transform.GetChild(i % 4).GetChild(0).GetChild(0).gameObject.SetActive(false);
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

        //Add a rank
        PlayerPrefs.SetInt("CurrentRank", tmpRank + 1);
        //AudioManager.Instance.PlaySound("MenuSmash");
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



        if (tmpRank < 9)
        {
            //Animation here
            lastSegment.GetChild(1).gameObject.SetActive(true);
            lastSegment.GetChild(1).GetComponent<Animator>().SetTrigger("Pan");
            //Enable model after anim
            lastSegment.GetChild(0).gameObject.SetActive(true);
            //enable checkmark
            lastSegment.GetChild(0).GetChild(0).gameObject.SetActive(false);
            yield return new WaitForSeconds(1f);
            yield break;
        }

        else if(tmpRank % 4 != 0)
        {
            endPos = -Vector3.right * (tmpRank / 4 )*150f;
        }
        else
        {
            endPos = -Vector3.right * (tmpRank /4) * 150f;

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
        lastSegment.GetChild(1).gameObject.SetActive(true);
        //enable checkmark
        lastSegment.GetChild(0).GetChild(0).gameObject.SetActive(false);
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
                    else if (BallController.Instance.PoweredUp && GameManager.Instance.PoweredUpCount > 0)
                    {
                        GameManager.Instance.PoweredUpCount--;
                        BallController.Instance.comboMultiplier = 3f;
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



    public void ApplySkin(Transform skinElem = null)
    {
        int skinIndex = skinElem.GetSiblingIndex();
        int lastSkin = PlayerPrefs.GetInt("Skin", 0);
        //Check if skin is in availability (bit flag)
        if ((GameManager.Instance.SkinAvailability & 1 << skinIndex) == 1 << skinIndex)
        {

            //If it's bought - select it
            PlayerPrefs.SetInt("Skin", skinIndex);
            skinElem.GetComponent<ThemeButtonUpdater>().UpdateThemeButton(lastSkin);

            SpawnManager.Instance.Spawn();



        }
        else
        {
            //Getskin's Cost
            int cost = skinElem.GetComponent<ThemeButtonUpdater>().itemCost;

            if (GameManager.Instance.Gems >= cost)
            {
                GameManager.Instance.Gems -= cost;

                //bitshift index for memorizing unlocks
                GameManager.Instance.SkinAvailability += 1 << skinIndex;

                PlayerPrefs.SetInt("Skin", skinIndex);


                //If it's bought - select it
                PlayerPrefs.SetInt("Skin", skinIndex);
                skinElem.GetComponent<ThemeButtonUpdater>().UpdateThemeButton(lastSkin);

                SpawnManager.Instance.Spawn();
                AudioManager.Instance.PlaySound("Chest");
            }
            else
            {
                AudioManager.Instance.PlaySound("No");
                Debug.Log("YOU DONT HAVE THE SKIN. BUY IT? " + cost);
            }
        }

    }




    public void MoreKeys()
    {
        if(GameManager.Instance.KeyCount<3 && GameManager.Instance.ChestOpenedCount <9)
        {
            GameManager.Instance.KeyCount += 3;
          


            //Disable back button and back button
            chestHolder.transform.GetChild(1).GetChild(2).GetChild(0).gameObject.SetActive(false);
            chestHolder.transform.GetChild(1).GetChild(2).GetChild(1).gameObject.SetActive(false);
        }
    }



    public void BuySkin()
    {
        
        StartCoroutine(StopBuySkin());
        
    }


    public IEnumerator StopBuySkin()
    {

        List<Transform> skinsToBuy = new List<Transform>();

        foreach (Transform skin in shopElements)
        {


            //Check if skin is in availability (bit flag)
            if ((GameManager.Instance.SkinAvailability & 1 << skin.GetSiblingIndex()) == 1 << skin.GetSiblingIndex())
            {

                continue;
            }
            else
            {
                skinsToBuy.Add(skin);
            }
        }

        int lastStep = -1;
        int randomStep = -1;
        for (int i = 0; i < 5; i++)
        {
            while (skinsToBuy.Count>1 && lastStep == randomStep)
            {
                randomStep = UnityEngine.Random.Range(0, skinsToBuy.Count);
            }
            
            StartCoroutine(StopColorLerp(skinsToBuy[randomStep].GetChild(0), Color.white));
            yield return new WaitForSeconds(1f);
            lastStep = randomStep;

        }

        ApplySkin(skinsToBuy[randomStep]);
    }




    public bool volumeMuted = false;
    public Transform volumeRef;

    public void MuteSound(Transform reference)
    {
        volumeMuted = !volumeMuted;
        AudioManager.Instance.VolumeMute(volumeMuted);
        volumeRef.GetChild(0).gameObject.SetActive(!volumeMuted);
        volumeRef.GetChild(1).gameObject.SetActive(volumeMuted);

    }




}
