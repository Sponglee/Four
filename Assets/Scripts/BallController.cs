using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallController : Singleton<BallController>
{
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
            currentLevel = value;
            GameManager.Instance.LevelProgress = (float)(currentLevel) / LevelManager.Instance.levelCount;
          


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

            forcePush = value;
            if(value == true)
            {
                GameObject otherTrans = DownCheckRay(transform, "");
                Debug.Log(otherTrans.name);

                if (/*otherTrans.gameObject.CompareTag("Cart") ||*/ otherTrans.gameObject.CompareTag("Steel"))
                {
                    //Сheck if cart is close to push it out if needed
                    Debug.Log("BUMP " + CurrentLevel + " ::: " + otherTrans.GetComponent<CartModelContoller>().LevelIndex);
                    if (ForcePush && (otherTrans.GetComponent<CartModelContoller>().LevelIndex - CurrentLevel <= 2))
                    {
                      
                        PushDown(otherTrans.transform, otherTrans.GetComponent<CartModelContoller>().LevelIndex);
                    }
                }
            }
            else
            {
                //gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
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
            collidedBool = value;
            StartCoroutine(StopCollided());

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
    private float forceMultiplier = 10;
    [SerializeField]
    public float comboMultiplier = 1;
    [SerializeField]
    private float forceTreshold = 3f;
    [SerializeField]
    private Vector3 downVelocity = Vector3.down * 0.1f;
    public bool MenuOpened = false;


    [SerializeField]
    public /*static*/ bool Move = false;
    [SerializeField] float jumpStrength = 100;
    [SerializeField] float gravityForce = 10;



    LevelManager level;
    Rigidbody rb;
    float nextBallPosToJump;
    int skippedCounter = 0;
    float vel;

    public bool TapToStart = false;

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

            poweredUp = value;
            if(value == true)
            {

              
            }
        }

   
     
    }


    void Start()
    {
        

        downVelocity = new Vector3(0, -PlayerPrefs.GetFloat("Speed", 0.3f), 0);

        rb = GetComponent<Rigidbody>();
        level = FindObjectOfType<LevelManager>();

        nextBallPosToJump = -level.spawnOffset /*+ GetComponent<SphereCollider>().bounds.size.y / 2*/ + level.spawnOffsetStep / 2;

        //Debug.Log(nextBallPosToJump);
        LevelManager.Instance.ballRef = this;

        //gameObject.GetComponent<Renderer>().material.color = LevelManager.Instance.spawnMats[0].color;
    }

   
    private void Update()
    {
        if(Input.GetMouseButtonDown(2))
        {

            //PoweredUp = true;
            //GameManager.Instance.ComboActive = true;
            GameManager.Instance.LevelComplete();
        }
        else if(Input.GetMouseButtonDown(1))
        {
            StartCoroutine(FunctionHandler.Instance.StopMapPan());
        }
        

        if (TapToStart && !MenuOpened)
        {
            if (PoweredUp)
            {

               
                SpawnManager.Instance.vcamSpeedy.m_Priority = 11;
                comboMultiplier -= Time.deltaTime;
                comboMultiplier = Mathf.Clamp(comboMultiplier,2.5f,3f);
                //Debug.Log(comboMultiplier);
               
            }
            else
            {
               
                SpawnManager.Instance.vcamSpeedy.m_Priority = 9;
             

            }

            if (forceMultiplier >= forceTreshold)
            {
                if (!ForcePush)
                {
                    ForcePush = true;

                }


            }
            else
            {
                ForcePush = false;
            }

            //Move
            forceMultiplier += 1;
            forceMultiplier = Mathf.Clamp(forceMultiplier, 0, forceTreshold);
            transform.parent.position += downVelocity * forceMultiplier * comboMultiplier;


            //FailSafe for a ball
            if (rb.velocity != Vector3.zero)
            {
                rb.velocity = Vector3.zero;
            }
            if (transform.localPosition.y > 0)
            {
                transform.localPosition = Vector3.zero;
            }


           

        }
        else if(!TapToStart)
        {
            if(Input.GetMouseButtonDown(0))
            {
                TapToStart = true;
                GameManager.Instance.tapText.gameObject.SetActive(false);
                //PoweredUp = true;
              
            }
        }


    }




    [SerializeField]
    private bool WarningCheck = false;
    //Check what level ball is currently on
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Level"))
        {
            //Debug.Log(other.name);
            CurrentLevel = other.transform.parent.parent.GetSiblingIndex();
        }
        else if (other.gameObject.CompareTag("CartTrigger") && ForcePush)
        {
            //if(gameObject.GetComponent<Renderer>().material.color != other.transform.parent.GetComponent<Renderer>().material.color)
            //    WarningCheck = true;
        }
        else if(other.gameObject.CompareTag("Collectable"))
        {
            Destroy(other.gameObject);
            GameManager.Instance.GrabCollectable();
            if(PoweredUp)
            {
                comboMultiplier += 0.3f;
            }
            Instantiate(LevelManager.Instance.threePrefab, other.transform.position, Quaternion.identity);
        }
           
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("CartTrigger"))
        {
            //if(gameObject.GetComponent<Renderer>().material.color != other.transform.parent.GetComponent<Renderer>().material.color)
            //    WarningCheck = false;
        }
    }




    //Process a collision
    private void OnCollisionEnter(Collision other)
    {
        //Debug.Log("COLLIDED " + other.gameObject.name);
        //Collision with steel carts or cart carts that are to the left or to the right
        //if (/*!CollidedBool && */(other.gameObject.CompareTag("Steel") || (other.gameObject.CompareTag("Cart")) && gameObject.GetComponent<Renderer>().material != other.gameObject.GetComponent<Renderer>().material))
        //{
        //    CollidedBool = true;
        //    if (other.transform.parent.parent != null && CurrentLevel == other.transform.parent.parent.parent.parent.GetSiblingIndex())
        //    {
        //        //Debug.Log("Ball " + transform.position);
        //        //Debug.Log("Cart " + other.transform.position);
        //        if (other.transform.position.x >= transform.position.x)
        //        {
        //            //LevelManager.Instance.LevelMove(CurrentLevel, true);
        //        }
        //        else
        //        {
        //            //LevelManager.Instance.LevelMove(CurrentLevel, false);
        //        }
        //        return;
        //    }
              
        //}



        //Debug.Log("ENTER " + gameObject.name + " >>> " + other.gameObject.name);
        if (/*other.gameObject.CompareTag("Cart")  || */other.gameObject.CompareTag("Steel"))
        {
            if (ForcePush /*&& !CollidedBool*/)
            {
                //Debug.Log(other + "COLLISION");
                //Debug.Log(other.transform.name + "Transform");


                PushDown(other.transform, other.transform.GetComponent<CartModelContoller>().LevelIndex);


            }
        }
        else if (other.gameObject.CompareTag("Danger"))
        {
            if(!PoweredUp)
            {
                FunctionHandler.Instance.OpenGameOver("GAME OVER");
                
            }
            else
                PushDown(other.transform, other.transform.GetComponent<CartModelContoller>().LevelIndex);
        }
        else if (other.gameObject.CompareTag("Bottom"))
        {
            //Add more levels for progression
            GameManager.Instance.LevelComplete();
        }
    }


    public void PushDown(Transform other, int siblingIndex)
    {
       
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

           
            //Second cart pop sequence
            other.gameObject.GetComponent<BoxCollider>().isTrigger = true;
            other.gameObject.GetComponent<CapsuleCollider>().enabled = false;
            Rigidbody rb = other.transform.GetChild(1).GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.None;
            rb.useGravity = true;
            rb.velocity = new Vector3(Random.Range(-50f, -30f), 80f, -50f);
            rb.AddRelativeTorque(new Vector3(500f, 20f, 0));


            rb = other.transform.GetChild(0).GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.None;
            rb.useGravity = true;
            rb.velocity = new Vector3(Random.Range(50f, 30f), 80f, -50f);
            rb.AddRelativeTorque(new Vector3(500f, 20f, 0));


            //Get some effects 
            Instantiate(LevelManager.Instance.hitPrefab, gameObject.transform.position + new Vector3(0, 5, -5), Quaternion.identity, LevelManager.Instance.EffectHolder);


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
                GameManager.Instance.AddScore(1, Color.grey, transform.GetChild(0));
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
        float TotalTime = 1f;
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


    //Get reference to object hit by ray with tag
    private GameObject DownCheckRay(Transform origin, string obj = "")
    {
        RaycastHit hit;

        Vector3 dir;
        Vector3 offsetOrigin;
        Vector3 rayDirection;

        //Offset origin to get center of a cart
        offsetOrigin = origin.position /*+ new Vector3(0, -2.51f, 0)*/;
        //lowerEnd of debug line
        dir = origin.position + new Vector3(0, -15f, 0);
        Debug.DrawLine(offsetOrigin, dir, Color.black, 10f);
        //}


        if (Physics.Raycast(offsetOrigin, dir, out hit))
        {

            if (hit.transform)
            {
                //Return any tag object if ""
                if (obj == "")
                    return hit.transform.gameObject;
                //Return only objects with obj tag
                if (hit.transform.gameObject.CompareTag(obj))
                {
                    return hit.transform.gameObject;
                }

            }
        }
        return null;

    }

}
