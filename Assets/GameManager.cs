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
            Debug.Log("<< " + score);
            if(value>=bestScore)
            {
                bestScore = value;
                PlayerPrefs.SetInt("BestScore", value);
                Debug.Log(bestScore + ">>" + score);
                bestText.gameObject.SetActive(false);
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
        Score = PlayerPrefs.GetInt("Score",0);
        Debug.Log(":" + Score + " :: " + bestScore + ":");

        if(score >= bestScore)
        {
            bestText.gameObject.SetActive(false);
           
        }
        else
        {
            bestText.text = bestScore.ToString();
        }

        LevelProgress = 0;
        CurrentRank = PlayerPrefs.GetInt("CurrentRank", 1);
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}


    public bool ComboActive = false;

    public void AddScore(int score)
    {

    }
}
