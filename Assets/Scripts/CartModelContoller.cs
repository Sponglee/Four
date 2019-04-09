using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CartModelContoller : MonoBehaviour
{

    //Current for multiple pops
    private int CollidedCurrent = -1;

    //bool for checking multiple pops
    private bool SecondCollision = false;
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

    private CinemachineDollyCart tempCart;
    public CinemachineSmoothPath[] paths;
    public CartManager spawnManager;
    [SerializeField]
    private int cartNumber;

    public Color spawnColor;
    public bool IsLowered = false;

    ////for Direction control
    //public int lastDirection;
    //public float lastDirectionTimer;


    [SerializeField]
    private int current;
    public int Current
    {
        get
        {
            return current;
        }
        set
        {
            current = value;
            //Check if passes 0 and set to value again
            if (current < 0)
                current = 3;
            else if (current > 3)
                current = 0;

            //if(!gameObject.CompareTag("Spawn"))
            //    Debug.Log(name + " : " + current);
          
        }
    }


    private void Start()
    {
        //Current = gameObject.transform.parent.parent.GetSiblingIndex();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Bottom") && gameObject.CompareTag("Cart"))
        {
            Destroy(transform.parent.gameObject);
        }
        else if (other.gameObject.CompareTag("Bottom") && gameObject.CompareTag("Spawn"))
        {
            Destroy(gameObject.transform.parent.gameObject);
        }
    }


    private void OnCollisionEnter(Collision other)
    {
       
        if (!CollidedBool && gameObject.CompareTag("Spawn") && (other.gameObject.CompareTag("Cart") || other.gameObject.CompareTag("Bottom") || other.gameObject.CompareTag("Steel")))
        {
            CollidedBool = true;
            if (gameObject.GetComponent<Renderer>().material.color == other.gameObject.GetComponent<Renderer>().material.color)
            {
                //Check if other is in the same column if secon hit 
                if(SecondCollision && CollidedCurrent != other.transform.GetComponent<CartModelContoller>().Current)
                {
                    //Debug.Log("REE");
                    return;
                }

                //Replace other with blank
                GameObject tmpBlank = Instantiate(LevelManager.Instance.blankCartPrefab);
                Transform tmpBlankParent = other.transform.parent.parent;
                //int otherIndex = other.transform.parent.GetSiblingIndex();

                //remember which level other cart is on 
                int detatchLevel = other.transform.parent.parent.parent.parent.GetSiblingIndex();
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
                Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
                rb.constraints = RigidbodyConstraints.None;
                rb.useGravity = true;
                rb.velocity = new Vector3(Random.Range(-10f, 10f), 50f, -50f);
                rb.AddRelativeTorque(new Vector3(5000f, 0, 0));

                //Get some effects 
                Instantiate(LevelManager.Instance.hitPrefab, gameObject.transform.position + new Vector3(0, 5, -5), Quaternion.identity, LevelManager.Instance.EffectHolder);
                //For pizzaz
                //StartCoroutine(LevelManager.Instance.TiDi(0.05f));
                
                //Second cart below check for color - if not the same - pop Spawn out
                GameObject tmpRay = DownCheckRay(other.transform);
               
                //If there's not same color below
                if (tmpRay != null && tmpRay.CompareTag("Cart") && tmpRay.GetComponent<Renderer>().material.color != gameObject.GetComponent<Renderer>().material.color)
                {
                    //Debug.Log("REEEEETATCH");
                    //destroy holder if no dollys
                    //DETACH
                    transform.parent.SetParent(null);
                    gameObject.GetComponent<BoxCollider>().isTrigger = true;
                    Rigidbody tmprb = gameObject.GetComponent<Rigidbody>();
                    tmprb.constraints = RigidbodyConstraints.None;
                    tmprb.useGravity = true;
                    tmprb.velocity = new Vector3(0, 10f, -50f);
                    tmprb.AddRelativeTorque(new Vector3(1000f, 0, 0));

                }
                //Enable next cart collision
                else if(tmpRay != null && tmpRay.CompareTag("Cart") && tmpRay.GetComponent<Renderer>().material.color == gameObject.GetComponent<Renderer>().material.color)
                {
                    //Debug.Log("NEXT");  
                    CollidedBool = false;
                    CollidedCurrent = other.transform.GetComponent<CartModelContoller>().Current;
                    SecondCollision = true;
                } //Detatch spawn if it hit last level (prevent bottom cart bug)
                //if (tmpRay == null && detatchLevel == LevelManager.Instance.levelCount - 2)
                else
                {
                    //Debug.Log("REEEEETATCH");
                    //destroy holder if no dollys
                    //DETACH
                    transform.parent.SetParent(null);
                    gameObject.GetComponent<BoxCollider>().isTrigger = true;
                    Rigidbody tmprb = gameObject.GetComponent<Rigidbody>();
                    tmprb.constraints = RigidbodyConstraints.None;
                    tmprb.useGravity = true;
                    tmprb.velocity = new Vector3(0, 10f, -50f);
                    tmprb.AddRelativeTorque(new Vector3(1000f, 0, 0));
                }

            }
            //If not the same color
            else if (other.transform.parent != null)
            {
                //If this cart was going to go through same color twice- just pop it without sticking
                if (SecondCollision)
                {
                    //DETACH
                    gameObject.GetComponent<BoxCollider>().isTrigger = true;
                    Rigidbody tmprb = gameObject.GetComponent<Rigidbody>();
                    tmprb.constraints = RigidbodyConstraints.None;
                    tmprb.useGravity = true;
                    tmprb.velocity = new Vector3(0, 0, -50f);
                    tmprb.AddRelativeTorque(new Vector3(1000f, 0, 0));
                    return;
                }

                //get index of levelHolder above
                int levelIndex = other.transform.parent.parent.parent.parent.GetSiblingIndex();
                //Debug.Log("LEVEL " + levelIndex);
                if (levelIndex >= 1)
                {
                    if (false/*stickRay != null  && !ReferenceEquals(gameObject, stickRay)*/)
                    {
                        CollidedBool = false;
                        //SecondCollision = true;
                        return;
                    }
                    else
                    {

                        //Debug.Log("Sticking levelIndex: " + levelIndex);
                        StickCart(other, levelIndex);
                    }

                }
                else
                {
                    FunctionHandler.Instance.OpenGameOver("GAME OVER");
                }
            }
        }
    
    }


    //Get reference to object hit by ray with tag
    private GameObject DownCheckRay(Transform origin, string obj="")
    {
        RaycastHit hit;

        Vector3 dir;
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
            dir = origin.position + new Vector3(0, -20f, -2.5f);
            Debug.DrawLine(origin.position + new Vector3(0, 0, -2.5f), dir, Color.black, 10f);
        //}


        if (Physics.Raycast(origin.position + new Vector3(0, 0, -2.5f), rayDirection, out hit))
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


    public void StickCart(Collision other, int levelIndex)
    {
        int newCurrent = other.gameObject.GetComponent<CartModelContoller>().Current;


      
        //Remove 1 blank
        //Destroy(LevelManager.Instance.gameObject.transform.GetChild(levelIndex - 1).GetChild(0).Find("BlankHolder(Clone)").gameObject);


        //spawn cart prefab
        GameObject tmpCart = Instantiate(LevelManager.Instance.cartPrefab, LevelManager.Instance.gameObject.transform.GetChild(levelIndex - 1).GetChild(0).GetChild(newCurrent).transform);
        //Set material and orientation
        tmpCart.transform.SetSiblingIndex(0);
        tmpCart.transform.GetComponentInChildren<Renderer>().material.color = SpawnManager.Instance.spawnCartManager.spawnMatRandomColor;
        tmpCart.transform.position = LevelManager.Instance.gameObject.transform.GetChild(levelIndex - 1).GetChild(0).GetChild(newCurrent).position;
        tmpCart.transform.rotation = tmpCart.transform.parent.rotation;
        tmpCart.transform.GetChild(0).GetComponent<CartModelContoller>().Current = newCurrent;
        //tmpCart.transform.rotation = tmpCart.transform.rotation * Quaternion.Euler(-180, 0, 90);


        ////Get rid of that position blank
        foreach (Transform child in tmpCart.transform.parent)
        {
            if (child.CompareTag("Blank"))
            {
                Destroy(child.gameObject);
            }
        }

        Destroy(transform.parent.gameObject);






        //****??????????
        //Enable Horizontal Check
 
        LevelManager.Instance.gameObject.transform.GetChild(levelIndex - 1)
                                                        .GetChild(0).GetComponent<CartManager>().HorizontalCheck(spawnColor, levelIndex);



    }


    //private void MoveOut(int direction)
    //{
    //    //foreach(CartModelContoller tempModel in cartManager.carts)
    //    //{

    //    //}
    //    if (GetCartAngle() > 0)
    //    {
    //        if(tempCart.m_Position == 0)
    //        {
    //            tempCart.m_Speed = -tempCart.m_Speed;
    //            return;
    //        }
    //        Current++;
    //        //Set path after calculating current
    //        tempCart.m_Path = paths[current];
    //        tempCart.m_Position = 0;
    //        tempCart.m_Speed = 40;

    //    }
    //    else if (GetCartAngle() < 0)
    //    {
    //        if (tempCart.m_Position == 3)
    //        {
    //            tempCart.m_Speed = -tempCart.m_Speed;
    //            return;
    //        }
    //        Current--;
    //        //Set path after calculating current
    //        tempCart.m_Path = paths[current];
    //        tempCart.m_Position = 3;
    //        tempCart.m_Speed = -40;
    //    }
    //}

    //// Get angle for mousePosition
    //private float GetCartAngle()
    //{
    //    Vector3 selCart = cartManager.carts[cartManager.selectedIndex].transform.position;
    //    Vector3 selDirection = selCart - cartManager.center.position;

    //    Vector3 moveCart = transform.position;
    //    Vector3 direction = moveCart - cartManager.center.position;

    //    //Get angle between mouse coursor and first touch on cart
    //    return Mathf.Atan2(Vector3.Dot(Vector3.back, Vector3.Cross(selDirection, direction)),
    //                                    Vector3.Dot(selDirection, direction)) * Mathf.Rad2Deg;
    //}
}