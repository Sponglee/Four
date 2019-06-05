﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    public TextMeshProUGUI tapText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI bestText;
    public TextMeshProUGUI multiText;
    public TextMeshProUGUI menuScoreText;
    public TextMeshProUGUI gemsText;

    public Transform powerPanel;

    public GameObject popUp;


    public Slider progresSlider;
    public Image powerFiller;

    public TextMeshProUGUI currText;
    public TextMeshProUGUI nextText;

    public GameObject multiButton;
    public GameObject fltText;

    private int gems;

    public int Gems
    {
        get
        {
            return gems;
        }

        set
        {
            gems = value;
            gemsText.text = gems.ToString();
            PlayerPrefs.SetInt("Gems", gems);
        }
    }

   




    //Fill amount of powerbar
    public float fillRate = 5;
    //Decrease rate for powerFill
    
    public float powerDecreaseAmount;
    public float powerRestoreRate;
    [SerializeField]
    private float powerFill = 0;
    public float PowerFill
    {
        get
        {
            return powerFill;
        }

        set
        {
            //powerFill = Mathf.Clamp(0f,1f,value);
                powerFill = value;
                powerFiller.fillAmount = powerFill;

           if(powerFill > 0 && powerFill<1)
                Instance.powerFiller.gameObject.SetActive(true);


            if (powerFill >= 1 && BallController.Instance.PowerUpTrigger == false)
            {
                BallController.Instance.PowerUpTrigger = true;
                powerFiller.fillAmount = 0;
                //powerFill = 0.9f;

                //powerFiller.color = Color.yellow;
                //ComboActive = true;
            }
          

        }
    }



    public int bestScore;
    private int score;
    public int Score
    {
        get
        {
            return score;
        }

        set
        {
            score = value;
            scoreText.text = value.ToString();
            PlayerPrefs.SetInt("Score", value);
            //Debug.Log("<< " + score);
            if (value>=bestScore)
            {
                bestScore = value;
                PlayerPrefs.SetInt("BestScore", value);
                menuScoreText.gameObject.SetActive(false);
            }
        }
    }


    public bool ComboActive = false;
    public Color comboColor;
    public int comboCount = 1;
    private int multiplier = 1;
    public int Multiplier
    {
        get
        {
            return multiplier;
        }

        set
        {
            multiplier = value;
            if(multiplier == 1)
            {
                multiText.transform.gameObject.SetActive(false);
            }
            else
            {
                multiText.transform.gameObject.SetActive(true);
                multiText.text = string.Format("x{0}", value.ToString());
            }

           
        }
    }




    private float levelProgress;
    public float LevelProgress
    {
        get
        {
            return levelProgress;
        }

        set
        {
            levelProgress = value;
            progresSlider.value = value;
            if(progresSlider.value>=1)
            {
                
                
                progresSlider.value = 0;
            }
        }
    }


    private int currentRank;
    public int CurrentRank
    {
        get
        {
            return currentRank;
        }

        set
        {
           
            currentRank = value;
            
            currText.text = value.ToString();
            nextText.text = (value + 1).ToString();
        }
    }
    //POWER UPS
    ///////////////////////////////////////////////
    [SerializeField]
    private int shieldCount = 0;
    public int ShieldCount
    {
        get
        {
            return shieldCount;
        }

        set
        {
            shieldCount = value;
            if (shieldCount > 0)
            {
                //Fade out the button
                powerPanel.GetChild(0).GetChild(2).gameObject.SetActive(false);
                //Set powerCol multiplier
                if(shieldCount>1)
                {
                    powerPanel.GetChild(0).GetChild(1).gameObject.SetActive(true);
                    powerPanel.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = shieldCount.ToString();
                }
                else
                {
                    powerPanel.GetChild(0).GetChild(1).gameObject.SetActive(false);
                }
            }
            else
            {
                //Fade in the button
                powerPanel.GetChild(0).GetChild(2).gameObject.SetActive(true);
            }
            PlayerPrefs.SetInt("ShieldCount", shieldCount);
        }
    }

    [SerializeField]
    private int magnetCount = 0;
    public int MagnetCount
    {
        get
        {
            return magnetCount;
        }

        set
        {
            magnetCount = value;
            if (magnetCount > 0)
            {
                //Fade out the button
                powerPanel.GetChild(1).GetChild(2).gameObject.SetActive(false);
                //Set powerCol multiplier
                if(magnetCount>1)
                {
                    powerPanel.GetChild(1).GetChild(1).gameObject.SetActive(true);
                    powerPanel.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = magnetCount.ToString();
                }
                else
                {
                    powerPanel.GetChild(1).GetChild(1).gameObject.SetActive(false);
                }
            }
            else
            {
                //Fade in the buttton
                powerPanel.GetChild(1).GetChild(2).gameObject.SetActive(true);
            }
            PlayerPrefs.SetInt("MagnetCount", magnetCount);
        }
    }


    [SerializeField]
    private int poweredUpCount = 0;
    public int PoweredUpCount
    {
        get
        {
            return poweredUpCount;
        }

        set
        {
            poweredUpCount = value;
            if (poweredUpCount > 0)
            {
                ////Fade out the button
                ////powerPanel.GetChild(2).GetChild(2).gameObject.SetActive(false);
                //Set powerCol multiplier
                if(poweredUpCount>1)
                {
                    //powerPanel.GetChild(2).GetChild(1).gameObject.SetActive(true);
                    powerPanel.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = poweredUpCount.ToString();
                }
                else
                {
                    //powerPanel.GetChild(2).GetChild(1).gameObject.SetActive(false);
                }
            }
            else
            {
                //Fade in the button
                //powerPanel.GetChild(2).GetChild(2).gameObject.SetActive(true);
            }
            PlayerPrefs.SetInt("PoweredUpCount", poweredUpCount);
        }
    }


    //////////////////////////////////////

    private void Awake()
    {
        CurrentRank = PlayerPrefs.GetInt("CurrentRank", 1);

    }
    // Start is called before the first frame update
    void Start()
    {
        Gems = PlayerPrefs.GetInt("Gems", 0);

        //Initialize powerUps
        ShieldCount = PlayerPrefs.GetInt("ShieldCount", 0);
        MagnetCount = PlayerPrefs.GetInt("MagnetCount", 0);
        PoweredUpCount = PlayerPrefs.GetInt("PoweredUpCount", 0);

        //PowerFill = 0;

        bestScore = PlayerPrefs.GetInt("BestScore", 0);
        Score = PlayerPrefs.GetInt("Score", 0);
       
        //Debug.Log(":" + Score + " :: " + bestScore + ":");

       

  
        LevelProgress = 0;
      
    }

  
    public void AddScore(int scoreAmount, Color color, Transform origin)
    {
       

        //GameObject tmpFltText = Instantiate(fltText, origin.position, Quaternion.identity);
        //tmpFltText.transform.GetChild(0).GetChild(0).GetComponent<Text>().color = color;
        //tmpFltText.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = string.Format("+{0}", scoreAmount/**comboCount*/);

       

        if (scoreAmount == multiplier && scoreAmount != 1)
        {
            Score += Multiplier;
            
        }
        else
        {
            //Debug.Log("ADD " + scoreAmount + " : " + comboCount + " : " + Multiplier);
            Score += scoreAmount;
        }
    }

    private bool PowerUpDecreasing = false;

    public void GrabCollectable(int power = -1, Transform powerColTrans = null)
    {

        if(power != -1)
        {
            switch (power)
            {
                //Shield
                case 0:
                    {
                        if(BallController.Instance.Shielded)
                            ShieldCount++;
                        else
                            BallController.Instance.Shielded = true;
                    }
                    break;
                //Magnet
                case 1:
                    {
                        if(BallController.Instance.Magnet)
                            MagnetCount++;
                        else
                            BallController.Instance.Magnet = true;
                    }
                    break;
                //PoweredUp
                case 2:
                    {
                        if (BallController.Instance.PoweredUp)
                        {
                            //PoweredUpCount++;
                        }
                        else
                            BallController.Instance.PoweredUp = true;
                    }
                    break;
                default:
                    break;
            }
           
        }
        else
        {
            Gems++;
        }

       
    }





    public IEnumerator StopPoweredUp(float fraction, float startTime, float powerDecreaseSpeed)
    {

      

        PowerUpDecreasing = true;

        while (powerFill > 0)  
        {
            if (powerFill <= 0.1f)
            {
                //BallController.Instance.comboMultiplier = 1f;
                yield return new WaitForSeconds(0.02f);
            }
            else
                yield return null;

            if (ComboActive)
            {

                //PowerFill -= 10f / (powerDecreaseSpeed);
            }
            else
            {
                //PowerFill -= 10f / (powerDecreaseSpeed);

            }

          

        }

        BallController.Instance.PoweredUp = false;
        BallController.Instance.comboMultiplier = 1f;

        //PowerFill = 0;
        //powerFiller.color = Color.white;
        Multiplier = 1;
        PowerUpDecreasing = false;
        powerDecreaseAmount = powerDecreaseSpeed;
        BallController.Instance.transform.GetChild(2).GetComponent<Renderer>().material.color = Color.white;


    }



    public void LevelComplete()
    {
        int tmpLvlCount = PlayerPrefs.GetInt("LevelCount", 50);
        FunctionHandler.Instance.OpenGameOver(String.Format("LEVEL {0} COMPLETE",PlayerPrefs.GetInt("CurrentRank",1)-1));
        PlayerPrefs.SetInt("CurrentRank", CurrentRank + 1);
        if (tmpLvlCount <= 400)
            PlayerPrefs.SetInt("LevelCount", tmpLvlCount + 5);
        else
        {

            if (PlayerPrefs.GetInt("CurrentRank", 1) % 10 == 0)
            {
                tmpLvlCount++;
                PlayerPrefs.SetInt("LevelCount", tmpLvlCount);
            }
        }
    }
}
