using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallController : Singleton<BallController>
{

    public Animator BallAnim;
    public int comboIndex = 1;

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
            levelManager.transform.GetChild((currentLevel + 65) % levelManager.transform.childCount).gameObject.SetActive(true);

            if (currentLevel>20)
            {
                levelManager.transform.GetChild(currentLevel - 20).gameObject.SetActive(false);
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
                comboIndex = 0;
                //GameObject otherTrans = DownCheckRay(transform, "Cart");

                //PushDown(otherTrans.transform, otherTrans.GetComponent<CartModelContoller>().LevelIndex);

                BallAnim.SetTrigger("Bump");
                //for (int i = 2; i < Random.Range(0,10); i++)
                //{
                //    int randomDir = Random.Range(0, 2);

                //    if (randomDir == 1)
                //    {
                //        StartCoroutine(LevelManager.Instance.transform.GetChild(CurrentLevel + i).GetChild(0).GetComponent<CartManager>().StopLevelRotator());

                //    }
                //    else
                //    {
                //        StartCoroutine(LevelManager.Instance.transform.GetChild(CurrentLevel + i).GetChild(0).GetComponent<CartManager>().StopLevelRotator(true));

                //    }

                //}
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
    public float comboMultiplier = 1;
    [SerializeField]
    private float forceTreshold = 3f;
    [SerializeField]
    private Vector3 downVelocity = Vector3.down;
    public bool MenuOpened = false;


    [SerializeField]
    public /*static*/ bool Move = false;
    [SerializeField] float jumpStrength = 100;
    [SerializeField] float gravityForce = 10;



    LevelManager levelManager;
    Rigidbody rb;
    float nextBallPosToJump;
    int skippedCounter = 0;
    float vel;

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
                rb.constraints &= ~RigidbodyConstraints.FreezePositionY;
                GameManager.Instance.tapText.gameObject.SetActive(false );

                //StartCoroutine(LevelManager.Instance.StopLevelRotator());
            }
            else if (value == false && tapToStart == true)
            {
                rb.constraints = RigidbodyConstraints.FreezeAll;
                GameManager.Instance.tapText.gameObject.SetActive(true);
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
                //Enable trigger
                transform.GetComponent<BoxCollider>().isTrigger = true;
                BallAnim.SetBool("Fall", true);
                poweredUpVFX.SetActive(true);

                Debug.Log("SPEED " + rb.velocity);
                rb.velocity = Vector3.down * 72f;

                //RemoveCartBelow(2);
                CollidedBool = false;
                poweredUp = value;
            }
            else if (collidedBool == true && value == false)
            {
                //Disable trigger
                transform.GetComponent<BoxCollider>().isTrigger = false;
                BallAnim.SetBool("Fall", false);
                poweredUpVFX.SetActive(false);
            }
            else if (value == false)
            {
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
            }
            else if(shielded == true && value == false)
            {
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
               StartCoroutine(magnetHolder.parent.GetComponent<SpawnManager>().StopMagnet());
            }
            else if(value == false && magnet == true)
            {
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

        transform.SetSiblingIndex(1);

        magnetHolder = SpawnManager.Instance.transform.GetChild(0);


        downVelocity = new Vector3(0, -PlayerPrefs.GetFloat("Speed", 0.3f), 0);

        rb = GetComponent<Rigidbody>();
        levelManager = FindObjectOfType<LevelManager>();

        nextBallPosToJump = -levelManager.spawnOffset /*+ GetComponent<SphereCollider>().bounds.size.y / 2*/ + levelManager.spawnOffsetStep / 2;

        //Debug.Log(nextBallPosToJump);
        levelManager.ballRef = this;

        //gameObject.GetComponent<Renderer>().material.color = LevelManager.Instance.spawnMats[0].color;


        
        StartCoroutine(levelManager.StopLevelRotator());
    }

   
    private void Update()
    {

        if(!collidedBool && !poweredUp && TapToStart)
        {

            comboMultiplier = Mathf.Clamp(comboMultiplier + Time.deltaTime, 1, 3f);
            //powerUpSpeed += Time.deltaTime;

            //Enable and fill powerFiller
            GameManager.Instance.powerFiller.gameObject.SetActive(true);
            GameManager.Instance.powerFiller.fillAmount = (comboMultiplier)/3f;


            if(comboMultiplier == 3f)
            {
                PoweredUp = true;
            }
        }
        else if(!poweredUp)
        {
            comboMultiplier = Mathf.Clamp(comboMultiplier - Time.deltaTime*2f, 1, 3f);
            GameManager.Instance.powerFiller.fillAmount = (comboMultiplier) / 3f;
            //Enable and fill powerFiller
            if (comboMultiplier == 1)
                GameManager.Instance.powerFiller.gameObject.SetActive(false);
        }
        else
        {
            comboMultiplier = 3f;
            GameManager.Instance.powerFiller.gameObject.SetActive(false);
        }


        if (Input.GetMouseButtonDown(1))
        {
           
            //comboMultiplier = 3;
            //PoweredUp = true;
            //GameManager.Instance.ComboActive = true;
            //GameManager.Instance.LevelComplete();
        }
        else if (Input.GetMouseButtonDown(2))
        {
            GameManager.Instance.KeyCount++;
            
        }


        if (TapToStart && !MenuOpened)
        {
            //Launch powerup
            //REPLACE THIS WITH A BUTTON PRESS





            if (PoweredUp)
            {


                SpawnManager.Instance.vcamSpeedy.m_Priority = 11;
                //comboMultiplier -= Time.deltaTime;
                //comboMultiplier = Mathf.Clamp(comboMultiplier, 2.5f, 3f);
                ////Debug.Log(comboMultiplier);

            }
            else
            {

                SpawnManager.Instance.vcamSpeedy.m_Priority = 9;


            }




            if (forceMultiplier >= forceTreshold)
            {
                if (!ForcePush)
                {
                    transform.GetChild(2).GetComponent<Renderer>().material.color = Color.yellow;
                   
                    ForcePush = true;
                    //rb.velocity = downVelocity * forceMultiplier * comboMultiplier * 10f;
                }


            }
            //else if(!CollidedBool)
            //{
            //    //transform.GetChild(2).GetComponent<Renderer>().material.color = Color.white;
            //    ForcePush = false;
                
            //    rb.velocity = downVelocity /** comboMultiplier*/ * 100f;

            //}
            //else
            //{
            //    //transform.GetChild(2).GetComponent<Renderer>().material.color = Color.white;
            //    ForcePush = false;

            //    rb.velocity =Vector3.zero;

            //}



            //if (Input.GetMouseButton(0) && !PoweredUp)
            //{
            //    //Move
            //    forceMultiplier += 1.5f;
            //    forceMultiplier = Mathf.Clamp(forceMultiplier, 0, forceTreshold);

            //}
            //else
            //{
                //forceMultiplier = 1f;
                //forceMultiplier = Mathf.Clamp(forceMultiplier, 10, forceTreshold);
            //}


            


            ////FailSafe for a ball
            //if (rb.velocity != Vector3.zero)
            //{
            //    rb.velocity = Vector3.zero;
            //}
            //if (transform.localPosition.y > 0)
            //{
            //    transform.localPosition = Vector3.zero;
            //}




        }
        else if(!TapToStart && !MenuOpened)
        {
            //rb.velocity = Vector3.zero;

            if (Input.GetMouseButtonDown(0))
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




    [SerializeField]
    private bool WarningCheck = false;
 


    //Rays to move or not
    public void CheckMovement()
    {
        //Grab obj below
        GameObject otherTrans = DownCheckRay(transform, "Cart");


       
        if (otherTrans != null && otherTrans.gameObject.CompareTag("Cart"))
        {
            //Debug.Log(">>> " + CurrentLevel + " : " + otherTrans.transform.parent.parent.parent.parent.GetSiblingIndex());
            if (Mathf.Abs(otherTrans.transform.parent.parent.parent.parent.GetSiblingIndex() - CurrentLevel) > 1 )
            {
                CollidedBool = false;
            }
            else
            {
                if (BallController.Instance.CollidedBool)
                    BallController.Instance.BallAnim.SetTrigger("Bump");
            }
        }
        else
        {
            //Debug.Log("NULL");

            CollidedBool = false;
        }


       
    }



    //private void OnTriggerExit(Collider other)
    //{
    //    if(!PoweredUp && other.gameObject.CompareTag("Steel"))
    //    {
    //        //Debug.Log(other + "COLLISION");
    //        CollidedBool = false;   
    //    }
    //}

    //private void OnTriggerStay(Collider other)
    //{
    //    //Debug.Log("ENTER " + gameObject.name + " >>> " + other.gameObject.name);
    //    if (!PoweredUp && other.gameObject.CompareTag("Steel"))
    //    {
    //        CollidedBool = true;
    //    }
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Level"))
        {

            //Debug.Log(other.name);
            CurrentLevel = other.transform.parent.parent.GetSiblingIndex();
        }
        else if (other.gameObject.CompareTag("Bottom"))
        {
            //Add more levels for progression
            transform.GetComponent<BoxCollider>().isTrigger = false;
            comboMultiplier = 1;
            GameManager.Instance.LevelComplete();
        }
        else if (other.gameObject.CompareTag("Collectable"))
        {

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
            GameManager.Instance.KeyCount++;
            Instantiate(levelManager.smokePrefab, transform.position, Quaternion.identity);
            Destroy(other.gameObject);
        }
        else if (other.gameObject.CompareTag("PowerCol"))
        {
            //GameManager.Instance.KeyCount++;
            GameManager.Instance.GrabCollectable(other.gameObject.GetComponent<Collectable>().PowerCol, other.transform);
            Instantiate(levelManager.smokePrefab, transform.position, Quaternion.identity);
            Destroy(other.gameObject);
        }
        if (PoweredUp && other.gameObject.CompareTag("Cart"))
        {
            PushDown(other.transform, other.transform.GetComponent<CartModelContoller>().LevelIndex);
        }
        else if (other.gameObject.CompareTag("Danger"))
        {
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
       
        //if(PoweredUp)
        //    GameManager.Instance.PowerFill -= 15f / 200f; 
        //Debug.Log("SHIKARI");
        //CollidedBool = true;
        if (true/*gameObject.GetComponent<Renderer>().material.color == other.gameObject.GetComponent<Renderer>().material.color*/)
        {
          
            //ForcePush = false;
            //Check if other is in the same column if secon hit 
            if (SecondCollision && CollidedCurrent != other.transform.GetComponent<CartModelContoller>().Current)
            {
                Debug.Log("REE");
                return;
            }

            //other.SetParent(transform.GetChild(0));
           
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
                StartCoroutine(StopColor(other.transform.GetChild(0).GetChild(0).GetComponent<Renderer>(), Color.yellow));
                StartCoroutine(StopColor(other.transform.GetChild(1).GetChild(0).GetComponent<Renderer>(), Color.yellow));


            }
            else
            {
                //SCORE
             
                StartCoroutine(StopColor(other.transform.GetChild(0).GetChild(0).GetComponent<Renderer>(), Color.white));
                StartCoroutine(StopColor(other.transform.GetChild(1).GetChild(0).GetComponent<Renderer>(), Color.white));
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

}
