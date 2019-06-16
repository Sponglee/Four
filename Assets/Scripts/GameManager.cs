
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{




    public GameObject chestPrefab;
    public Transform chestReference;
    public Transform chestSpawnPoint;
    


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



    [SerializeField]
    private int skinAvailability;
    public int SkinAvailability
    {
        get
        {
            return skinAvailability;
        }

        set
        {
            skinAvailability = value;
            PlayerPrefs.SetInt("SkinAvailability", skinAvailability);
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

    //Keys for the chest
    [SerializeField]
    private int keyCount=0;
    public int KeyCount
    {
        get
        {
            return keyCount;
        }

        set
        {
            keyCount = value;
            if(value  > 0)
            {
               
                //chestReference.transform.GetChild(0).GetComponent<ChestController>().key.SetActive(true);
            }
            PlayerPrefs.SetInt("KeyCount", value);
        }
    }


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
                if (shieldCount>1)
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
                if (magnetCount>1)
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
                //Fade out the button
                powerPanel.GetChild(2).GetChild(2).gameObject.SetActive(false);

                //Set powerCol multiplier
                if (poweredUpCount>1)
                {
                    powerPanel.GetChild(2).GetChild(1).gameObject.SetActive(true);
                    powerPanel.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = poweredUpCount.ToString();
                }
                else
                {
                    powerPanel.GetChild(2).GetChild(1).gameObject.SetActive(false);
                }
            }
            else
            {
                //Fade in the button
                powerPanel.GetChild(2).GetChild(2).gameObject.SetActive(true);
            }
            PlayerPrefs.SetInt("PoweredUpCount", poweredUpCount);
        }
    }


  



    //////////////////////////////////////

    private void Awake()
    {
        Gems = PlayerPrefs.GetInt("Gems", 0);

        KeyCount = PlayerPrefs.GetInt("KeyCount", 0);

        CurrentRank = PlayerPrefs.GetInt("CurrentRank", 1);
        //Check bought skins
        SkinAvailability = PlayerPrefs.GetInt("SkinAvailability", 1);

    }
    // Start is called before the first frame update
    void Start()
    {
       


      
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
        Debug.Log(power);
        //Not gem
        if(power != -1)
        {
            switch (power)
            {
                //Shield
                case 0:
                    {
                            ShieldCount++;
                        //if(BallController.Instance.Shielded)
                        //else
                        //    BallController.Instance.Shielded = true;
                    }
                    break;
                //Magnet
                case 1:
                    {
                            MagnetCount++;
                        //if(BallController.Instance.Magnet)
                        //else
                        //    BallController.Instance.Magnet = true;
                    }
                    break;
                //PoweredUp
                case 2:
                    {
                            PoweredUpCount++;
                        //if (BallController.Instance.PoweredUp)
                        //{
                        //}
                        //else
                        //    BallController.Instance.PoweredUp = true;
                    }
                    break;
                //Gems min
                case 3:
                    {
                        Gems += 10;
                        //StartCoroutine(StopGems(10));
                    }
                    break;
                //Gems mid
                case 4:
                    {
                        Gems += 25;
                        //StartCoroutine(StopGems(25));
                    }
                    break;
                //Gems high
                case 5:
                    {
                        Gems += 100;
                        //StartCoroutine(StopGems(100));
                    }
                    break;
                default:
                    break;
            }

            Debug.Log(power);
           
        }
        //Coll is gem
        else
        {
            Gems++;
        }

       
    }


    public IEnumerator StopGems(int amount)
    {
        yield return new WaitForSeconds(0.5f);
        Gems += amount;
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

    public void ResetAnims()
    {
        //chestReference.transform.GetComponentInChildren<ChestController>().chestAnim.Rebind();
    }

    public void LevelComplete()
    {
        int tmpLvlCount = PlayerPrefs.GetInt("LevelCount", 50);
        FunctionHandler.Instance.OpenGameOver(string.Format("LEVEL {0} COMPLETE",PlayerPrefs.GetInt("CurrentRank",1)));
        
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


   
    public void ChestSpawn()
    {
        StartCoroutine(StopChestSpawn());
    }


    public void ChestDespawn()
    {
        foreach (Transform child in chestReference)
        {
            Destroy(child.gameObject);
        }
    }


    public IEnumerator StopChestSpawn()
    {
        float chestSpawnPointOffset = 0;
        int keyNumber = PlayerPrefs.GetInt("KeyCount", 0);
        for (int i = 0; i < keyNumber; i++)
        {
            chestSpawnPoint.localPosition = Vector3.zero;
            //Middle row
            if (i % 3 == 0)
            {
                if (i!=0)
                {
                    chestSpawnPointOffset += 200f;
                    if (chestSpawnPointOffset == 1400f)
                        chestSpawnPointOffset = 0;
                }
                chestSpawnPoint.localPosition = new Vector3(0, 0,  chestSpawnPointOffset);
            }
            //Right
            else if (i % 3 == 1)
            { 
                chestSpawnPoint.localPosition = new Vector3(185f, 0, +100 + chestSpawnPointOffset);
            }
            //Left
            else if(i%3 == 2)
            {
                
                chestSpawnPoint.localPosition = new Vector3(-185f, 0, +100f + chestSpawnPointOffset);
            }
          
            Instantiate(chestPrefab, chestSpawnPoint.position, Quaternion.identity, chestReference);
            yield return new WaitForSeconds(0.2f);
        }
    }
}
