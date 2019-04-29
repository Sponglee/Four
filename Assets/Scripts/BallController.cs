using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : Singleton<BallController>
{
    ///FROM CARTMODELCONTROLLER 
    /// 

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
    public float forceTreshold = 20f;
    public Vector3 downVelocity = Vector3.down * 13f;



    [SerializeField]
    public /*static*/ bool Move = false;
    [SerializeField] float jumpStrength = 100;
    [SerializeField] float gravityForce = 10;

    LevelManager level;
    Rigidbody rb;
    float nextBallPosToJump;
    int skippedCounter = 0;
    float vel;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        level = FindObjectOfType<LevelManager>();

        nextBallPosToJump = -level.spawnOffset /*+ GetComponent<SphereCollider>().bounds.size.y / 2*/ + level.spawnOffsetStep / 2;

        Debug.Log(nextBallPosToJump);
        LevelManager.Instance.ballRef = this;
    }

   
    private void Update()
    {
        if (Input.GetMouseButton(0)
           //&& !LevelManager.Instance.RotationProgress
           //    && !LevelManager.Instance.SpawnInProgress
           //        && !LevelManager.Instance.LevelMoveProgress 
           && SwipeManager.Instance.IsSwiping(SwipeDirection.None)/*&& spawnTimer <= 0*/)
        {
            
            
            forceMultiplier += 1;
            forceMultiplier = Mathf.Clamp(forceMultiplier, 0,50);
            gameObject.GetComponent<Rigidbody>().velocity = -Vector3.up * forceMultiplier;
            if(forceMultiplier>= forceTreshold)
            {
                ForcePush = true;
            }
            else
            {
                ForcePush = false;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            forceMultiplier = 10;
            ForcePush = false;
            gameObject.GetComponent<Rigidbody>().velocity = downVelocity;
        }



    }





    void FixedUpdate()
    {
        if (!Move)
            return;

        vel = -gravityForce * Time.deltaTime;

        float overlap = nextBallPosToJump - (transform.position.y + vel);
        if (overlap >= 0)
        {
            transform.Translate(Vector3.up * (vel + overlap));
            CheckCollision();
        }
        transform.Translate(Vector3.up * vel);
    }


    void CheckCollision()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, level.spawnOffsetStep / 2,
            LayerMask.GetMask("Circles")))
        {
            if (hit.collider.CompareTag("Cart"))
            {
                if (skippedCounter >= 2)
                {
                    //// TODO: Apply good-looking break force.
                    //if (hit.collider.transform.parent.CompareTag("Cylinder Object"))
                    //{
                    //    Destroy(hit.collider.gameObject);
                    //}
                    //else
                    //{
                    //    Destroy(hit.collider.transform.parent.gameObject);
                    //}
                }

                skippedCounter = 0;
                Jump();
                Debug.Log("Good.");
            }
            //else if (hit.collider.CompareTag("Bad"))
            //{
            //    Debug.Log("END GAME.");
            //    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            //}
            //else if (hit.collider.CompareTag("Finish"))
            //{
            //    Debug.Log("YOU WON.");
            //    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            //}
            else
            {
                Debug.LogWarning("COLLIDED WITH UNKNOWN OBJECT.");
            }
        }
        else
        {
            ++skippedCounter;
            nextBallPosToJump -= level.spawnOffsetStep;
        }
    }

    void Jump()
    {
        Debug.Log("Jump");
        vel = jumpStrength;
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
        if (other.gameObject.CompareTag("CartTrigger") && ForcePush)
        {
            if(gameObject.GetComponent<Renderer>().material.color != other.transform.parent.GetComponent<Renderer>().material.color)
                WarningCheck = true;
        }
           
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("CartTrigger"))
        {
            if(gameObject.GetComponent<Renderer>().material.color != other.transform.parent.GetComponent<Renderer>().material.color)
                WarningCheck = false;
        }
    }




    //Process a collision
    private void OnCollisionEnter(Collision other)
    {
        //Debug.Log("COLLIDED " + other.gameObject.name);
        //Collision with steel carts or cart carts that are to the left or to the right
        if ((other.gameObject.CompareTag("Steel") || (other.gameObject.CompareTag("Cart")) && gameObject.GetComponent<Renderer>().material != other.gameObject.GetComponent<Renderer>().material))
        {
            if (other.transform.parent.parent != null && CurrentLevel == other.transform.parent.parent.parent.parent.GetSiblingIndex())
            {
                //Debug.Log("Ball " + transform.position);
                //Debug.Log("Cart " + other.transform.position);
                if (other.transform.position.x >= transform.position.x)
                {
                    LevelManager.Instance.LevelMove(CurrentLevel, true);
                }
                else
                {
                    LevelManager.Instance.LevelMove(CurrentLevel, false);
                }
                return;
            }
              
        }
        //Debug.Log("ENTER " + gameObject.name + " >>> " + other.gameObject.name);
        if (other.gameObject.CompareTag("Cart")  || other.gameObject.CompareTag("Steel"))
        {
            if (ForcePush /*&& !CollidedBool*/)
            {

                PushDown(other, other.transform.parent.parent.parent.parent.GetSiblingIndex());
            }
        }
        else if (other.gameObject.CompareTag("Bottom"))
        {
            FunctionHandler.Instance.OpenGameOver("YOU WIN");
            PlayerPrefs.SetInt("LevelCount", PlayerPrefs.GetInt("LevelCount", 15) + 5);
        }
    }


    public void PushDown(Collision other, int siblingIndex)
    {
       
        //Debug.Log("SHIKARI");
        CollidedBool = true;
        if (gameObject.GetComponent<Renderer>().material.color == other.gameObject.GetComponent<Renderer>().material.color)
        {
            ForcePush = false;
            //Check if other is in the same column if secon hit 
            if (SecondCollision && CollidedCurrent != other.transform.GetComponent<CartModelContoller>().Current)
            {
                Debug.Log("REE");
                return;
            }

            //Replace other with blank
            GameObject tmpBlank = Instantiate(LevelManager.Instance.blankCartPrefab);
            //Debug.Log(other.transform.parent.parent.name);
            Transform tmpBlankParent = other.transform.parent.parent;
        
            //DETACH and set new sibling indexes
            other.transform.parent.SetParent(null);

            //Set blank orientation
            tmpBlank.transform.SetParent(tmpBlankParent);
            tmpBlank.transform.GetChild(0).GetComponent<CartModelContoller>().Current = tmpBlankParent.GetSiblingIndex();
            tmpBlank.transform.position = other.transform.parent.position;
            tmpBlank.transform.rotation = other.transform.parent.rotation;
            //Debug.Log("BLANK FIRST " + tmpBlank.transform.parent.parent.parent.GetSiblingIndex());
            tmpBlank.transform.parent.parent.GetComponent<CartManager>().CheckCarts();



            //Second cart pop sequence
            other.gameObject.GetComponent<BoxCollider>().isTrigger = true;
            other.gameObject.GetComponent<CapsuleCollider>().enabled = false;
            Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.None;
            rb.useGravity = true;

            //if(siblingIndex%2 == 0)
            //    rb.velocity = new Vector3(Random.Range(-150f, -50f), 50f, -50f);
            //else
            //    rb.velocity = new Vector3(Random.Range(150f, 50f), 50f, -50f);


            int detatchDir = Random.Range(0, 2);

            if(detatchDir == 0)
                rb.velocity = new Vector3(Random.Range(-50f, -30f), 80f, -50f);
            else
                rb.velocity = new Vector3(Random.Range(50f, 30f), 80f, -50f);



            rb.AddRelativeTorque(new Vector3(500f, 20f, 0));
            //Get some effects 
            Instantiate(LevelManager.Instance.hitPrefab, gameObject.transform.position + new Vector3(0, 5, -5), Quaternion.identity, LevelManager.Instance.EffectHolder);

            //For pizzaz
            //StartCoroutine(LevelManager.Instance.TiDi(0.05f));

            //SCORE
            GameManager.Instance.AddScore(1, gameObject.GetComponent<Renderer>().material.color, transform.GetChild(0));
            //Second cart below check for color - if not the same - pop Spawn out
            GameObject tmpRay = DownCheckRay(other.transform);

            if (tmpRay != null && tmpRay.CompareTag("Cart") && tmpRay.GetComponent<Renderer>().material.color == gameObject.GetComponent<Renderer>().material.color)
            {
                //Debug.Log("NEXT");
                CollidedBool = false;
                CollidedCurrent = other.transform.GetComponent<CartModelContoller>().Current;
               //SecondCollision = true;
            }
            //else if(tmpRay != null && tmpRay.CompareTag("Cart") && tmpRay.GetComponent<Renderer>().material.color != gameObject.GetComponent<Renderer>().material.color)
            //{
            //    Debug.Log("HEREERERERERERERER");
            //    ForcePush = false;
            //    forceMultiplier = 5;
            //}


        }
        //If not the same color
        else if (other.transform.parent != null && other.gameObject.CompareTag("Cart"))
        {
           
            //CollidedBool = false;
            if(ForcePush)
            {
              

                gameObject.GetComponent<Rigidbody>().velocity = downVelocity;
                forceMultiplier = 5;
                ForcePush = false;
                //Debug.Log("POINK " + ForcePush);
                
                if (!warning)
                {
                    StopCoroutine(StopColor(other.gameObject.GetComponent<Renderer>(), Color.red));
                    StartCoroutine(StopColor(other.gameObject.GetComponent<Renderer>(), Color.red));
                }
                else
                {
                    //ForcePush = false;
                    gameObject.GetComponent<Renderer>().material = other.gameObject.GetComponent<Renderer>().material;

                    PushDown(other, other.transform.parent.parent.parent.parent.GetSiblingIndex());
                    //FunctionHandler.Instance.OpenGameOver("GAME OVER");
                }

                    
                       
               
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
        //Debug.Log(render.material.color + " >> " + initColor);
       

        float ElapsedTime = 0.0f;
        float TotalTime = 0.3f;
        while (ElapsedTime < TotalTime)
        {
            ElapsedTime += Time.deltaTime;
            render.material.color = Color.Lerp(color, initColor, (ElapsedTime / TotalTime));

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

        ////Shoot ray up
        //if (higherOrigin)
        //{
        //    rayDirection = -Vector3.up;
        //    dir = origin.position + new Vector3(0, 20, -2.5f);
        //    Debug.DrawLine(origin.position + new Vector3(0, 0, -2.5f), dir, Color.green, 10f);
        //}
        //else
        //{
        rayDirection = -Vector3.up;
        //Offset origin to get center of a cart
        offsetOrigin = origin.TransformPoint(origin.localPosition + new Vector3(0, 2.5f, 0));
        //lowerEnd of debug line
        dir = offsetOrigin + new Vector3(0, -20f, 0);
        Debug.DrawLine(origin.TransformPoint(origin.localPosition + new Vector3(0, 2.5f, 0)), dir, Color.black, 10f);
        //}


        if (Physics.Raycast(offsetOrigin, rayDirection, out hit))
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
