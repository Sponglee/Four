using System;
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

    public Slider progresSlider;

    public Image powerFiller;

    public TextMeshProUGUI currText;
    public TextMeshProUGUI nextText;
    public TextMeshProUGUI multiText;

    public GameObject fltText;


    //Fill amount of powerbar
    public float fillRate = 5;
    //Decrease rate for powerFill
    public float powerDecreaseSpeed = 1000;
    public float powerDecreaseAmount = 1000;
    public float powerRestoreRate = 50;
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
            if (powerFill >= 1)
            {
              
                BallController.Instance.PoweredUp = true;
                ComboActive = true;
            }
            else if(powerFill<=0)
            {
                BallController.Instance.PoweredUp = false;
                ComboActive = false;
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
                multiText.transform.parent.gameObject.SetActive(false);
            }
            else
            {
                multiText.transform.parent.gameObject.SetActive(true);
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


    // Start is called before the first frame update
    void Start()
    {
        powerFiller.fillAmount = powerFill;
        bestScore = PlayerPrefs.GetInt("BestScore", 0);
        Score = PlayerPrefs.GetInt("Score", 0);
        //Debug.Log(":" + Score + " :: " + bestScore + ":");

       

        Score = 0;
        LevelProgress = 0;
        CurrentRank = PlayerPrefs.GetInt("CurrentRank", 1);
    }

  
    public void AddScore(int scoreAmount, Color color, Transform origin)
    {
       

        GameObject tmpFltText = Instantiate(fltText, origin.position, Quaternion.identity);
        tmpFltText.transform.GetChild(0).GetChild(0).GetComponent<Text>().color = color;
        tmpFltText.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = string.Format("+{0}", scoreAmount/**comboCount*/);

       

        if (scoreAmount == multiplier )
        {
            Score += Multiplier;
            Multiplier++;
        }
        else
        {
            //Debug.Log("ADD " + scoreAmount + " : " + comboCount + " : " + Multiplier);
            Score += scoreAmount;
        }
    }

    private bool PowerUpDecreasing = false;

    public void GrabCollectable()
    {
       
        PowerFill += 1/fillRate;
        if(powerFill>=1)
        {

            powerFill = 1;
            powerFiller.color = Color.yellow;

            //Fade down
            if (!PowerUpDecreasing)
            {
                BallController.Instance.gameObject.GetComponent<Renderer>().material.color = Color.yellow;
               
                StartCoroutine(StopPoweredUp(500,Time.timeSinceLevelLoad));
            }
            else
            {
                powerDecreaseAmount += powerRestoreRate;
            }
           
        }
    }


    private IEnumerator StopPoweredUp(float fraction, float startTime)
    {
      
        PowerUpDecreasing = true;
        while (powerFill > 0)  
        {
            //Debug.Log(">>>>" + powerDecreaseAmount);
            //Debug.Log(Time.timeSinceLevelLoad + " - " + startTime);
            PowerFill -= (Time.timeSinceLevelLoad- startTime) / powerDecreaseAmount;
            powerFiller.fillAmount = PowerFill;
            yield return null;
        }
        //powerFill = 0;
        powerFiller.color = Color.white;
        Multiplier = 1;
        PowerUpDecreasing = false;
        powerDecreaseAmount = powerDecreaseSpeed;
        BallController.Instance.gameObject.GetComponent<Renderer>().material.color = Color.white;
    }



    public void LevelComplete()
    {
        int tmpLvlCount = PlayerPrefs.GetInt("LevelCount", 15);
        FunctionHandler.Instance.OpenGameOver("LEVEL COMPLETE");
        PlayerPrefs.SetInt("CurrentRank", CurrentRank + 1);
        if (tmpLvlCount <= 500)
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
