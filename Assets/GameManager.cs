using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI bestText;

    public Slider progresSlider;

    public TextMeshProUGUI currText;
    public TextMeshProUGUI nextText;
    public TextMeshProUGUI multiText;

    public GameObject fltText;

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
            if(value>=bestScore)
            {
                bestScore = value;
                PlayerPrefs.SetInt("BestScore", value);
                Debug.Log(bestScore + ">>" + score);
                bestText.gameObject.SetActive(false);
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
                CurrentRank++;
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
            PlayerPrefs.SetInt("CurrentRank", value);
            currText.text = value.ToString();
            nextText.text = (value + 1).ToString();
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        bestScore = PlayerPrefs.GetInt("BestScore", 0);
        Score = PlayerPrefs.GetInt("Score", 0);
        //Debug.Log(":" + Score + " :: " + bestScore + ":");

        if (score >= bestScore)
        {
            bestText.gameObject.SetActive(false);
           
        }
        else
        {
            bestText.text = bestScore.ToString();
        }

        Score = 0;
        LevelProgress = 0;
        CurrentRank = PlayerPrefs.GetInt("CurrentRank", 1);
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}



    public void AddScore(int scoreAmount, Color color, Transform origin)
    {
        if(!ComboActive)
        {
            comboCount = 1;
            Multiplier = 1;
            ComboActive = true;
            comboColor = color;
        }
        else
        {
            if(comboColor == color)
            {
                //Debug.Log("SAMECOLOR");
                comboCount++;
                Multiplier++;
            }
            else
            {
                comboColor = color;
                comboCount= 1;
                Multiplier = 1;
            }
        }

        GameObject tmpFltText = Instantiate(fltText, origin.position, Quaternion.identity);
        tmpFltText.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = string.Format("+{0}", scoreAmount/**comboCount*/);

        //Debug.Log("ADD " + scoreAmount + " : " + comboCount + " : " + Multiplier);
        Score += scoreAmount/**comboCount*Multiplier*/;
    }
}
