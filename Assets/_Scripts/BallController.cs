using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameAnalyticsSDK;


public class BallController : Singleton<BallController>
{
    private Skybox skyReference;
    public Animator BallAnim;

    private Color pushColor;
    public int comboIndex = 1;
    public bool PowerUpResetBool = false;

    public float comboMultiplier = 1;
    public float comboDecreaseRate = 0.2f;

    ///FROM CARTMODELCONTROLLER 
    /// 
    [SerializeField]
    private int currentLevel = -2;
    public int CurrentLevel
    {
        get
        {
            return currentLevel;
        }

        set
        {
            
            if(!CollidedBool)
                comboIndex = Mathf.Clamp(comboIndex+1,0,1);

            currentLevel = value;
            GameManager.Instance.LevelProgress = (float)(currentLevel) / levelManager.levelCount;
            GameManager.Instance.AddScore(comboIndex, Color.grey, transform.GetChild(1));
            
            //OPTIMIZATION
            levelManager.transform.GetChild((currentLevel + 35) % levelManager.transform.childCount).gameObject.SetActive(true);

            if (currentLevel>20)
            {
                levelManager.transform.GetChild(currentLevel - 10).gameObject.SetActive(false);
            }

            //Decrease PoweredUP
            if (!collidedBool && !PoweredUp && TapToStart)
            {
                if(!PowerUpResetBool)
                {
                    //Increase combo while freefall
                    StartCoroutine(ChangePowerFill(0.4f));
                }
               


                //Enable and fill powerFiller
                GameManager.Instance.powerFiller.transform.parent.gameObject.SetActive(true);
                GameManager.Instance.powerFiller.fillAmount = (comboMultiplier) / 3f;

                //Debug.Log("?????????????????? " + comboMultiplier);
                if(comboMultiplier >= 1f && comboMultiplier <= 1.1f)
                {
                    AudioManager.Instance.PlaySound("Wind");
                }
                else if (comboMultiplier == 3f)
                {
                    PoweredUp = true;
                }
            }

            //Decrease on PoweredUp 
            else if (PoweredUp)
            {
                if (currentBallRank % 4 != 0 && !collidedBool && poweredUp && TapToStart)
                {
                   
                    //Enable and fill powerFiller
                    GameManager.Instance.powerFiller.transform.parent.gameObject.SetActive(true);
                    GameManager.Instance.powerFiller.fillAmount = (comboMultiplier) / 3f;


                    if (comboMultiplier == 0f)
                    {
                        PoweredUp = false;
                    }
                }
                else
                {
                    comboMultiplier = 3f;
                    GameManager.Instance.powerFiller.transform.parent.gameObject.SetActive(false);
                }
            }
        }
    }

    [SerializeField]
    private bool forcePush = false;
    public bool ForcePush
    {
        get
        {
            return forcePush;
        }

        set
        {

            if(value == true && forcePush == false)
            {
               
             
                //GameManager.Instance.PowerFill -= 15f / (GameManager.Instance.powerDecreaseSpeed);
                //rb.velocity = downVelocity * forceMultiplier * comboMultiplier * 10f;
            }
            else if(value == false && forcePush == true)
            {
                //gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
            forcePush = value;
        }
    }


    
    [SerializeField]
    //bool for checking multiple pops
    private bool SecondCollision = false;
    [SerializeField]
    private bool collidedBool = false;
    public bool CollidedBool
    {
        get
        {
            return collidedBool;
        }

        set
        {
            if(collidedBool == false && value == true)
            {
                //Reset to prevent power up second time while falling
                if(PowerUpResetBool)
                {
                    PowerUpResetBool = false;
                }

                comboIndex = 0;
                BallAnim.SetTrigger("Bump");
                AudioManager.Instance.PlaySound("Bump");
                AudioManager.Instance.StopSound("Wind");
                Instantiate(LevelManager.Instance.mpoofPrefab, transform.position, Quaternion.identity);
              
            }
            else if (collidedBool == true && value == false)
            {
                rigidBody.velocity = Vector3.down * gravityForce;
            }
            collidedBool = value;
        }
    }

  

    public IEnumerator StopCollided()
    {
        yield return new WaitForSeconds(0.3f);
        collidedBool = false;
    }
    //Current for multiple pops
    private int CollidedCurrent = -1;

    [SerializeField]
    private float powerUpSpeed = 0;
    [SerializeField]
    private float forceMultiplier = 10;
   
  

    [SerializeField]
    private float forceTreshold = 3f;
    [SerializeField]
    private Vector3 downVelocity = Vector3.down;
    public bool MenuOpened = false;


    [SerializeField]
    public /*static*/ bool Move = false;
    [SerializeField] float jumpStrength = 100;
    [SerializeField] float gravityForce = 10;

    private int currentBallRank = -1;

    LevelManager levelManager;
    private Rigidbody rigidBody;
    private float nextBallPosToJump;
    private int skippedCounter = 0;
    private float vel;

    [SerializeField]
    private bool tapToStart = false;

    public bool TapToStart
    {
        get
        {
            return tapToStart;
        }

        set
        {

            if (value == true && tapToStart == false)
            {
                //let go
                rigidBody.constraints &= ~RigidbodyConstraints.FreezePositionY;
                GameManager.Instance.tapObject.gameObject.SetActive(false);

                if(PoweredUp)
                {
                    AudioManager.Instance.PlaySound("FireTrail");
                    rigidBody.velocity = Vector3.down * 2f * gravityForce;
                }
                else
                {
                    rigidBody.velocity = Vector3.down * gravityForce;
                }

                  
                //StartCoroutine(LevelManager.Instance.StopLevelRotator());
            }
            else if (value == false && tapToStart == true)
            {
                
                rigidBody.constraints = RigidbodyConstraints.FreezeAll;
                GameManager.Instance.tapObject.gameObject.SetActive(true);
            }
           
            tapToStart = value;
        }
    }


    [SerializeField]
    private bool powerUpTrigger = false;
    public bool PowerUpTrigger
    {
        get
        {
            return powerUpTrigger;
        }

        set
        {


            if (value == true && powerUpTrigger == false && !PoweredUp)
            {
                GameManager.Instance.multiButton.SetActive(true);
              
                //StartCoroutine(StopLevelRotator());
                powerUpTrigger = value;
            }
            else if (value == false && powerUpTrigger == true)
            {
               
                Debug.Log("TOOT");
                //RemoveCartBelow(5);
                powerUpTrigger = value;
            }
            else
                return;
            
        }

   
     
    }


    public GameObject poweredUpVFX;
    [SerializeField]
    private bool poweredUp = false;
    public bool PoweredUp
    {
        get
        {
            return poweredUp;
        }

        set
        {
            if(value == true && poweredUp == false)
            {
                PowerUpResetBool = true;
                AudioManager.Instance.StopSound("Wind");
                AudioManager.Instance.PlaySound("FireUp");
                AudioManager.Instance.PlaySound("FireTrail");
                //Enable trigger
                transform.GetComponent<BoxCollider>().isTrigger = true;
                BallAnim.SetBool("Fall", true);

                //Instantiate powerup
                poweredUpVFX.SetActive(true);
                //Turn magnet on
                Instantiate(LevelManager.Instance.poweredUpPrefab, poweredUpVFX.transform);

                //Debug.Log("SPEED " + rb.velocity);
                rigidBody.velocity = Vector3.down * 2f * gravityForce;
                //rb.useGravity = false;

                //RemoveCartBelow(2);
                CollidedBool = false;
                poweredUp = value;

                
            }
            else if (poweredUp == true && value == false)
            {
                //Disable trigger
                transform.GetComponent<BoxCollider>().isTrigger = false;
                BallAnim.SetBool("Fall", false);

                //Disable PowerUp
                if (poweredUpVFX.transform.childCount > 0)
                    Destroy(poweredUpVFX.transform.GetChild(1).gameObject);

                poweredUpVFX.SetActive(false);

                //rigidBody.useGravity = true;
            }

            //Disable everything and sound
            if (value == false)
            {
                AudioManager.Instance.StopSound("FireTrail");
                GameManager.Instance.Multiplier = 1;
                //Disable trigger
                transform.GetComponent<BoxCollider>().isTrigger = false;
                BallAnim.SetBool("Fall", false);
                poweredUpVFX.SetActive(false);
            }
            poweredUp = value;
        }
    }

    public GameObject shieldVFX;
    [SerializeField]
    private bool shielded = false;
    public bool Shielded
    {
        get
        {
            return shielded;
        }

        set
        {
            if(shielded == false && value == true)
            {
                shieldVFX.SetActive(true);
                //Turn magnet on
                Instantiate(LevelManager.Instance.shieldedPrefab, shieldVFX.transform);
            }
            else if(shielded == true && value == false)
            {
                if(shieldVFX.transform.childCount>0)
                Destroy(shieldVFX.transform.GetChild(0).gameObject);
                shieldVFX.SetActive(false);
            }
            shielded = value;
        }
    }


    public Transform magnetHolder;
    public GameObject magnetVFX;

    [SerializeField]
    private bool magnet = false;
    public bool Magnet
    {
        get
        {
            return magnet;
        }

        set
        {
            if(value == true && magnet == false)
            {
               magnetHolder.gameObject.SetActive(true);
               magnetVFX.SetActive(true);
                //Turn magnet on
               Instantiate(LevelManager.Instance.electroMagnetPrefab, magnetVFX.transform);
               StartCoroutine(magnetHolder.parent.GetComponent<SpawnManager>().StopMagnet());
            }
            else if(value == false && magnet == true)
            {
                //Turn magnet off
                Destroy(magnetVFX.transform.GetChild(0).gameObject);
                magnetVFX.SetActive(false);
                magnetHolder.gameObject.SetActive(false);
            }
            magnet = value;
        }
    }





    public void RemoveCartBelow(int range)
    {
        GameObject otherTrans = DownCheckRay(transform, "Cart");
        //Debug.Log(">>>>" + otherTrans.name);

        if (otherTrans.gameObject != null && /*otherTrans.gameObject.CompareTag("Cart") || */otherTrans.gameObject.CompareTag("Danger"))
        {
            Debug.Log("BUMP " + CurrentLevel + " ::: " + otherTrans.GetComponent<CartModelContoller>().LevelIndex);
            //Сheck if cart is close to push it out if needed
            if (otherTrans.GetComponent<CartModelContoller>().LevelIndex - CurrentLevel <= range)
            {

                PushDown(otherTrans.transform, otherTrans.GetComponent<CartModelContoller>().LevelIndex);
            }
            
        }
    }


   

    public IEnumerator StopLevelTurn(Transform target, float duration, float angle)
    {
        float elapsed = 0;
        Vector3 startEul = target.localEulerAngles;
        Vector3 destEul = startEul + new Vector3(0, angle, 0);

        while ( target != null && Mathf.Abs(target.localEulerAngles.y - destEul.y) >= 0.05f)
        {
            target.localEulerAngles = Vector3.Lerp(startEul, destEul , elapsed / duration);
            //Debug.Log(">R> " + target.eulerAngles);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    void Start()
    {
        pushColor = LevelManager.Instance.ballPushColor;
        currentBallRank = PlayerPrefs.GetInt("CurrentRank", 1);
        transform.SetSiblingIndex(1);

        magnetHolder = SpawnManager.Instance.transform.GetChild(0);

        downVelocity = new Vector3(0, -PlayerPrefs.GetFloat("Speed", 0.3f), 0);

        rigidBody = GetComponent<Rigidbody>();
        levelManager = FindObjectOfType<LevelManager>();

        nextBallPosToJump = -levelManager.spawnOffset /*+ GetComponent<SphereCollider>().bounds.size.y / 2*/ + levelManager.spawnOffsetStep / 2;

        levelManager.ballRef = this;

        StartCoroutine(levelManager.StopLevelRotator());
    }

    private void Update()
    {
        if (TapToStart && !MenuOpened)
        {
            if (PoweredUp)
            {
                SpawnManager.Instance.vcamSpeedy.m_Priority = 11;
            }
            else
            {
                SpawnManager.Instance.vcamSpeedy.m_Priority = 9;
            }
        }
        else if (!TapToStart && !MenuOpened)
        {
            //rb.velocity = Vector3.zero;

            if (Input.GetMouseButtonDown(0) && !FunctionHandler.Instance.GameOverInProgress)
            {
                TapToStart = true;

                //PoweredUp = true;

            }
        }
        else
        {
            //rb.velocity = Vector3.zero;
        }
    }

    private void FixedUpdate()
    {

        if (!PoweredUp && collidedBool && TapToStart)
        {
            //Reset power Up on collided
            comboMultiplier = Mathf.Clamp(comboMultiplier - Time.deltaTime * 5f, 0, 5f);
            GameManager.Instance.powerFiller.fillAmount = (comboMultiplier) / 3f;
            //Enable and fill powerFiller
            if (comboMultiplier == 0)
                GameManager.Instance.powerFiller.transform.parent.gameObject.SetActive(false);
        }

        ///DEBUG/////////////////////////////////////////////////

        if (Input.GetMouseButtonDown(1))
        {
            GameManager.Instance.LevelComplete();
            //comboMultiplier = 3;
            //PoweredUp = true;
            //GameManager.Instance.ComboActive = true;
            //GameManager.Instance.LevelComplete();
        }
        else if (Input.GetMouseButtonDown(2))
        {
            GameManager.Instance.KeyCount++;
            BallController.Instance.PoweredUp = true;
            comboMultiplier = 3f;
        }

        ///DEBUG/////////////////////////////////////////////////

       
    }




    [SerializeField] private bool WarningCheck = false;
 


    //Rays to move or not
    public void CheckMovement()
    {
        //Grab obj below
        GameObject otherTrans = DownCheckRay(transform, "Cart");

        if (otherTrans != null && otherTrans.gameObject.CompareTag("Cart"))
        {
            if (Mathf.Abs(otherTrans.transform.parent.parent.parent.parent.GetSiblingIndex() - CurrentLevel) > 1 )
            {
                Debug.Log(">");
                //CollidedBool = true;
                CollidedBool = false;
            }
            else
            {
                if (BallController.Instance.CollidedBool)
                {
                    BallController.Instance.BallAnim.SetTrigger("Bump");
                }
            }
        }
        else
        {
            CollidedBool = false;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Level"))
        {
            CurrentLevel = other.transform.parent.parent.GetSiblingIndex();
        }
        else if (other.gameObject.CompareTag("Bottom"))
        {
            Instantiate(levelManager.finishPrefab, transform.position, Quaternion.identity, transform.GetChild(1));
            BallAnim.SetTrigger("Bump");
            AudioManager.Instance.PlaySound("End");

            PoweredUp = false;
            AudioManager.Instance.StopSound("FireTrail");
            AudioManager.Instance.PlaySound("Finish");


            //Add more levels for progression
            transform.GetComponent<BoxCollider>().isTrigger = false;
            comboMultiplier = 1;
            //GA 
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, Application.version, string.Format("level0{0}",currentBallRank), GameManager.Instance.Gems);

            GameManager.Instance.LevelComplete();

        }
        else if (other.gameObject.CompareTag("Collectable"))
        {
            AudioManager.Instance.PlaySound("Gem");
            Destroy(other.gameObject);
            GameManager.Instance.GrabCollectable();
            if (PoweredUp)
            {
                //comboMultiplier += 0.3f;
            }
            Instantiate(levelManager.poofPrefab, other.transform.position, Quaternion.identity);
        }
        else if (other.gameObject.CompareTag("Chest"))
        {
            AudioManager.Instance.PlaySound("Key");
            GameManager.Instance.KeyCount++;
            Instantiate(levelManager.smokePrefab, transform.position, Quaternion.identity);
            Destroy(other.gameObject);
        }
        else if (other.gameObject.CompareTag("PowerCol"))
        {
            Instantiate(levelManager.smokePrefab, transform.position, Quaternion.identity);
            Destroy(other.gameObject);
        }
        if (PoweredUp && other.gameObject.CompareTag("Cart"))
        {
            AudioManager.Instance.PlaySound("Hit");
            PushDown(other.transform, other.transform.GetComponent<CartModelContoller>().LevelIndex);
        }
        else if (other.gameObject.CompareTag("Danger"))
        {
            AudioManager.Instance.PlaySound("Danger");
            //LevelMove danger if on same level
            if (other.transform.parent.parent != null && CurrentLevel >= other.transform.parent.parent.parent.parent.GetSiblingIndex() && !other.transform.GetComponent<CartModelContoller>().Moving)
            {

                Debug.Log(CurrentLevel + " : : : : " + other.transform.parent.parent.parent.parent.GetSiblingIndex());
                if (other.transform.position.x >= transform.position.x)
                {
                    other.transform.GetComponent<CartModelContoller>().Moving = true;
                    levelManager.LevelMove(CurrentLevel, true);
                }
                else
                {
                    other.transform.GetComponent<CartModelContoller>().Moving = true;
                    levelManager.LevelMove(CurrentLevel, false);
                }
                return;

            }
            else
            {
                
                //8888888888888888888//
                if (Shielded)
                {
                    PushDown(other.transform, other.transform.GetComponent<CartModelContoller>().LevelIndex);
                    Shielded = false;
                }
                else if (!PoweredUp)
                {
                    if (!other.transform.GetComponent<CartModelContoller>().Moving)
                    {
                        AudioManager.Instance.StopSound("FireTrail");
                        //AudioManager.Instance.PlaySound("End");
                        //GA 
                        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, Application.version, string.Format("level0{0}", currentBallRank), GameManager.Instance.Gems);

                        FunctionHandler.Instance.OpenGameOver("GAME OVER");
                        TapToStart = false;
                        forceMultiplier = 1;
                        PushDown(other.transform, other.transform.GetComponent<CartModelContoller>().LevelIndex);
                    }

                }
                else
                {

                    PushDown(other.transform, other.transform.GetComponent<CartModelContoller>().LevelIndex);
                    PoweredUp = false;
                  
                    comboMultiplier = 1;
                    //GameManager.Instance.PowerFill = 0;
                }
            }


        }

    }
    //Process a collision
    private void OnCollisionEnter(Collision other)
    {

       
        //Debug.Log("COLLIDED " + other.gameObject.name);
        //Collision with steel carts or cart carts that are to the left or to the right
        if (!PoweredUp && other.gameObject.CompareTag("Cart"))
        {
            //LevelMove cart if on same level
            if (other.transform.parent.parent != null && CurrentLevel == other.transform.parent.parent.parent.parent.GetSiblingIndex() && !other.transform.GetComponent<CartModelContoller>().Moving)
            {

                if (other.transform.position.x >= transform.position.x )
                {
                    other.transform.GetComponent<CartModelContoller>().Moving = true;
                    levelManager.LevelMove(CurrentLevel, true, true);
                }
                else
                {
                    other.transform.GetComponent<CartModelContoller>().Moving = true;
                    levelManager.LevelMove(CurrentLevel, false,true);
                }
                return;
            }

        }

        if (!PoweredUp && other.gameObject.CompareTag("Cart"))
        {
            if (true /*&& !CollidedBool*/)
            {
                //Debug.Log(other + "COLLISION");
                //Debug.Log(other.transform.name + "Transform");

                CollidedBool = true;
            }


        }


        //else if (other.gameObject.CompareTag("CartTrigger") && ForcePush)
        //{
        //    //if(gameObject.GetComponent<Renderer>().material.color != other.transform.parent.GetComponent<Renderer>().material.color)
        //    //    WarningCheck = true;
        //}




    }




    public void PushDown(Transform other, int siblingIndex)
    {
        //Decrease Power on collision
        if (PoweredUp)
        {
            
            comboMultiplier = Mathf.Clamp(comboMultiplier - comboDecreaseRate, 0, 3f);
            GameManager.Instance.powerFiller.fillAmount = (comboMultiplier) / 3f;
        }
          
        if (true)
        {

            //Check if other is in the same column if secon hit 
            if (SecondCollision && CollidedCurrent != other.transform.GetComponent<CartModelContoller>().Current)
            {
                Debug.Log("REE");
                return;
            }

            //Second cart pop sequence  
            other.gameObject.GetComponent<BoxCollider>().tag = "Untagged";
            other.gameObject.GetComponent<BoxCollider>().isTrigger = true;
            Rigidbody rb = other.transform.GetChild(1).GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.None;
            rb.useGravity = true;
            rb.velocity = new Vector3(Random.Range(-5f, -20f), Random.Range(1f, 10f), -10f);
            rb.AddRelativeTorque(new Vector3(Random.Range(-50f, -10f), Random.Range(0f,20f), -50f));


            rb = other.transform.GetChild(0).GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.None;
            rb.useGravity = true;
            rb.velocity = new Vector3(Random.Range(5f, 20f), Random.Range(1f,10f), -10f);
            rb.AddRelativeTorque(new Vector3(Random.Range(-50f,-10f), Random.Range(-20f, 0f), 50f));


            //SetBoxCollider to trigger to avoid stucking
            other.transform.GetComponent<BoxCollider>().isTrigger = true;

            //Get some effects 
            Instantiate(levelManager.mpoofPrefab, gameObject.transform.position + new Vector3(0, 5, -5), Quaternion.identity, levelManager.EffectHolder);


            if(PoweredUp)
            {
                GameManager.Instance.Multiplier++;
                GameManager.Instance.AddScore(GameManager.Instance.Multiplier, Color.yellow, transform.GetChild(1));
                StartCoroutine(StopColor(other.transform.GetChild(0).GetChild(0).GetComponent<Renderer>(), pushColor));
                StartCoroutine(StopColor(other.transform.GetChild(1).GetChild(0).GetComponent<Renderer>(), pushColor));

                //comboMultiplier = Mathf.Clamp(comboMultiplier - 0.5f, 0, 3f);
                //GameManager.Instance.powerFiller.fillAmount = (comboMultiplier) / 3f;

            }
            else
            {
                //SCORE
               
                StartCoroutine(StopColor(other.transform.GetChild(0).GetChild(0).GetComponent<Renderer>(), pushColor));
                StartCoroutine(StopColor(other.transform.GetChild(1).GetChild(0).GetComponent<Renderer>(), pushColor));
            }
           
        }
      
    }

    [SerializeField]
    private bool warning = false;
    private IEnumerator StopColor(Renderer render, Color color)
    {
        warning = true;
        Color initColor = render.material.color;
        render.material.color = color;
       


        float ElapsedTime = 0.0f;
        float TotalTime = 0.5f;
        while (ElapsedTime < TotalTime)
        {
            render.material.color = Color.Lerp(color, initColor, (ElapsedTime / TotalTime));
            ElapsedTime += Time.fixedDeltaTime;
            //Debug.Log(render.gameObject.name + " >> " + initColor.r);
            //if (ElapsedTime / TotalTime >= 0.5f)

            yield return null;
        }

        warning = false;
        
    }


    //Get reference to  2ND object  UNDER BALL hit by ray with tag
    private GameObject DownCheckRay(Transform origin, string obj = "")
    {
        RaycastHit hit;

        Vector3 dir;
        Vector3 offsetOrigin;
        Vector3 rayDirection;

        //Offset origin to get center of a cart
        offsetOrigin = origin.position + new Vector3(0, 3.51f, 0);
        //lowerEnd of debug line
        dir = origin.position + new Vector3(0, -15f, 0);
        Debug.DrawLine(offsetOrigin, dir, Color.black, 10f);
        //}


        var hits = Physics.RaycastAll(offsetOrigin, dir);

        

        if (hits.Length > 0)
        {

            foreach (var hitElem in hits)
            {
                //Return any tag object if ""
                //if (obj == "")
                //    return hitElem.transform.gameObject;
                //Return only objects with obj tag
                if (hitElem.transform.gameObject.CompareTag(obj))
                {
                    return hitElem.transform.gameObject;
                }

            }
        }
        return null;

    }



    //For poweredUp fill on currentlevel change
    public IEnumerator ChangePowerFill( float rate)
    {
        for (int i = 0; i < 5; i++)
        {
            comboMultiplier = Mathf.Clamp(comboMultiplier + rate /5f, 0, 3f);
            GameManager.Instance.powerFiller.fillAmount = (comboMultiplier) / 3f;
            yield return new WaitForEndOfFrame();
        }
    }

}
