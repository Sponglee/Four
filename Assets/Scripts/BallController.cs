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


    private void Start()
    {
        LevelManager.Instance.ballRef = this;
    }

    [SerializeField]
    private float forceMultiplier = 13;
    private void Update()
    {
        if (Input.GetMouseButton(0)
           && !LevelManager.Instance.RotationProgress
               && !LevelManager.Instance.SpawnInProgress
                   && !LevelManager.Instance.LevelMoveProgress && SwipeManager.Instance.IsSwiping(SwipeDirection.None)/*&& spawnTimer <= 0*/)
        {
            
            ForcePush = true;
            forceMultiplier += 1;
            forceMultiplier = Mathf.Clamp(forceMultiplier, 0,30);
            gameObject.GetComponent<Rigidbody>().velocity = -Vector3.up * forceMultiplier;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            forceMultiplier = 13;
            //LevelManager.Instance.StartLevelMove(CurrentLevel);
            if (SpawnManager.Instance.gameMode)
            {
                ForcePush = false;
                //gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * 50);
            }
            else
            {
                ForcePush = false;
                //gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * 50);
            }
        }
    }

    //Check what level ball is currently on
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Level"))
        {
            //Debug.Log(other.name);
            CurrentLevel = other.transform.parent.parent.GetSiblingIndex();
        }
    }

    //Process a collision
    private void OnCollisionEnter(Collision other)
    {
        //Debug.Log("COLLIDED " + this.tag);
        //Collision with steel carts or cart carts that are to the left or to the right
        if (other.gameObject.CompareTag("Steel") || (other.gameObject.CompareTag("Cart") && gameObject.GetComponent<Renderer>().material != other.gameObject.GetComponent<Renderer>().material))
        {
            if (CurrentLevel == other.transform.parent.parent.parent.parent.GetSiblingIndex())
            {
                Debug.Log("Ball " + transform.position);
                Debug.Log("Cart " + other.transform.position);
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
            if (ForcePush && !CollidedBool)
            {

                PushDown(other);
            }
        }
        else if (other.gameObject.CompareTag("Bottom"))
        {
            FunctionHandler.Instance.OpenGameOver("YOU WIN");
        }
    }


    public void PushDown(Collision other)
    {
       
        //Debug.Log("SHIKARI");
        CollidedBool = true;
        if (gameObject.GetComponent<Renderer>().material.color == other.gameObject.GetComponent<Renderer>().material.color)
        {
            ForcePush = false;
            //Check if other is in the same column if secon hit 
            if (SecondCollision && CollidedCurrent != other.transform.GetComponent<CartModelContoller>().Current)
            {
                //Debug.Log("REE");
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
            rb.velocity = new Vector3(Random.Range(-20f, 20f), 50f, -50f);
            rb.AddRelativeTorque(new Vector3(5000f, 0, 0));
            //Get some effects 
            Instantiate(LevelManager.Instance.hitPrefab, gameObject.transform.position + new Vector3(0, 5, -5), Quaternion.identity, LevelManager.Instance.EffectHolder);

            //For pizzaz
            //StartCoroutine(LevelManager.Instance.TiDi(0.05f));

            //SCORE
            GameManager.Instance.AddScore(1, gameObject.GetComponent<Renderer>().material.color, gameObject.transform);
            //Second cart below check for color - if not the same - pop Spawn out
            GameObject tmpRay = DownCheckRay(other.transform);

            if (tmpRay != null && tmpRay.CompareTag("Cart") && tmpRay.GetComponent<Renderer>().material.color == gameObject.GetComponent<Renderer>().material.color)
            {
                //Debug.Log("NEXT");
                CollidedBool = false;
                CollidedCurrent = other.transform.GetComponent<CartModelContoller>().Current;
               //SecondCollision = true;
            }


        }
        //If not the same color
        else if (other.transform.parent != null && other.gameObject.CompareTag("Cart"))
        {
            //CollidedBool = false;
            if(ForcePush)
            {
                gameObject.GetComponent<Renderer>().material = other.gameObject.GetComponent<Renderer>().material;
                ForcePush = false;
                PushDown(other);
                gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            }


        }
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
