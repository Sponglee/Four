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

            //Put dolly to a proper index
            transform.parent.SetSiblingIndex(Current);

        }
    }


    private void Start()
    {
        //tempCart = gameObject.transform.parent.GetComponent<CinemachineDollyCart>();
        cartNumber = gameObject.transform.parent.GetSiblingIndex();


        //if(gameObject.CompareTag("Blank"))
        //{
        //    //Set track references for that cart
        //    paths = transform.parent.parent.GetComponent<CartManager>().paths;
        //}

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

            ////Second cart below check for color - if not the same - pop first cart outwards
            //GameObject stickDebRay = GrabRayObj(transform);

            //Debug.Log(" >< " + ReferenceEquals(other.gameObject, stickDebRay) + " >>> " + stickDebRay.GetComponent<Renderer>().material);



            //Debug.Log(" >< " + ReferenceEquals(other.gameObject, gameObject));
            //Debug.Log("COLLISION (" + gameObject.transform.position + "):" + other.transform.position.y);
            CollidedBool = true;
            if (gameObject.GetComponent<Renderer>().material.color == other.gameObject.GetComponent<Renderer>().material.color)
            {
                //Check if other is in the same column if secon hit 
                if(SecondCollision && CollidedCurrent != other.transform.GetComponent<CartModelContoller>().Current)
                {
                    Debug.Log("REE");
                    return;
                }

                //Debug.Log("FIRST SAME COLOR");


                //Replace other with blank
                //DETACH and set new sibling indexes
                GameObject tmpBlank = Instantiate(LevelManager.Instance.blankCartPrefab);
                Transform tmpBlankParent = other.transform.parent.parent;
                int otherIndex = other.transform.parent.GetSiblingIndex();
                
                other.transform.parent.SetParent(null);

                tmpBlank.transform.SetParent(tmpBlankParent);
                tmpBlank.transform.GetChild(0).GetComponent<CartModelContoller>().Current = otherIndex;

                Debug.Log("BLANK FIRST " + tmpBlank.transform.parent.parent.parent.GetSiblingIndex());
                tmpBlank.transform.parent.parent.GetComponent<CartManager>().CheckCarts();

                //check if no dollys
                //Debug.Log(tmpBlank.transform.GetChild(0).GetComponent<CartModelContoller>().Current + " >>>>> " + other.transform.GetComponent<CartModelContoller>().Current);





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
                GameObject tmpRay = GrabRayObj(other.transform, "Cart");
                if (tmpRay != null && tmpRay.GetComponent<Renderer>().material.color != gameObject.GetComponent<Renderer>().material.color)
                {

                    //Debug.Log("KILL SPAWN");
                    //transform.parent.parent.parent.GetComponent<CartManager>().CheckCarts();

                    //destroy holder if no dollys
                    //DETACH
                    transform.parent.SetParent(null);
                    gameObject.GetComponent<BoxCollider>().isTrigger = true;
                    Rigidbody tmprb = gameObject.GetComponent<Rigidbody>();
                    tmprb.constraints = RigidbodyConstraints.None;
                    tmprb.useGravity = true;
                    tmprb.velocity = new Vector3(0, 0, -50f);
                    tmprb.AddRelativeTorque(new Vector3(1000f, 0, 0));

                }
                //Enable next cart collision
                else
                {
                    CollidedBool = false;
                    CollidedCurrent = other.transform.GetComponent<CartModelContoller>().Current;
                    SecondCollision = true;
                }

               
            }
            //If not the same color
            else if (other.transform.parent != null)
            {
                //If this cart was going to go through same color twice- just pop it without sticking
                if (SecondCollision)
                {
                    ////destroy holder if no dollys
                    //Debug.Log("THEN SECOND");
                    //transform.parent.parent.parent.GetComponent<CartManager>().CheckCarts();

                    ////Replace other with blank
                    ////DETACH and set new sibling indexes
                    //GameObject tmpBlank = Instantiate(LevelManager.Instance.blankCartPrefab);
                    //other.transform.parent.parent.GetComponent<CartManager>().CheckCarts();
                    //tmpBlank.transform.SetParent(other.transform.parent.parent);
                    //int otherIndex = other.transform.parent.GetSiblingIndex();
                    //other.transform.parent.SetParent(null);
                    //tmpBlank.transform.GetChild(0).GetComponent<CartModelContoller>().Current = otherIndex;



                    gameObject.GetComponent<BoxCollider>().isTrigger = true;
                    Rigidbody tmprb = gameObject.GetComponent<Rigidbody>();
                    tmprb.constraints = RigidbodyConstraints.None;
                    tmprb.useGravity = true;
                    tmprb.velocity = new Vector3(0, 0, -50f);
                    tmprb.AddRelativeTorque(new Vector3(1000f, 0, 0));
                    return;
                }

                //get index of levelHolder above
                int levelIndex = other.transform.parent.parent.parent.GetSiblingIndex();
                if (levelIndex >= 1)
                {

                    //Debug.Log("STICK CART " + levelIndex);

                    //Second cart below check for color - if not the same - pop first cart outwards
                    //Debug.Log(other.transform.position + " >>> " + transform.position);
                 
                    if (false/*stickRay != null  && !ReferenceEquals(gameObject, stickRay)*/)
                    {
                        CollidedBool = false;
                        //SecondCollision = true;
                        return;
                    }
                    else
                    {
                        
                        //Debug.Log("Sticking levelIndex: " + levelIndex);
                        //////////////StickCart(other, levelIndex);
                    }
                   
                }
                else
                {
                    FunctionHandler.Instance.OpenGameOver("GAME OVER");
                }
            }
            //else if (gameObject.CompareTag("Spawn") &&  other.gameObject.CompareTag("Bottom"))
            //{
            //    //If this cart was going to go through same color - just pop it without sticking
            //    if (SecondCollision)
            //    {
            //        //destroy holder if no dollys
            //        //Debug.Log("THEN SECOND");
            //        transform.parent.parent.GetComponent<CartManager>().CheckCarts();
            //        //DETACH
            //        //transform.parent.SetParent(null);
            //        gameObject.GetComponent<BoxCollider>().isTrigger = true;
            //        Rigidbody tmprb = gameObject.GetComponent<Rigidbody>();
            //        tmprb.constraints = RigidbodyConstraints.None;
            //        tmprb.useGravity = true;
            //        tmprb.velocity = new Vector3(0, 0, -50f);
            //        tmprb.AddRelativeTorque(new Vector3(1000f, 0, 0));
            //        return;
            //    }

            //    int levelIndex = other.transform.parent.parent.GetSiblingIndex();
            //    if (levelIndex >= 1)
            //    {
            //        StickCart(other, levelIndex);
            //    }
            //    else
            //    {

            //        FunctionHandler.Instance.OpenGameOver("GAME OVER");
            //    }
            //}
          
        }
    
    }


    //Get reference to object hit by ray with tag
    private GameObject GrabRayObj(Transform origin, string obj="")
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
                ////Return any tag object if ""
                //if (obj == "")
                //    return hit.transform.gameObject;
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


        Debug.Log("STICK CART " + newCurrent);
        //Remove 1 blank
        //Destroy(LevelManager.Instance.gameObject.transform.GetChild(levelIndex - 1).GetChild(0).Find("BlankHolder(Clone)").gameObject);


        //spawn cart prefab, set current position
        GameObject tmpCart = Instantiate(LevelManager.Instance.gameObject.transform.GetChild(levelIndex - 1).GetChild(0).GetComponent<CartManager>().cartPrefabs[0], LevelManager.Instance.gameObject.transform.GetChild(levelIndex - 1).GetChild(0).transform);
        //Set material
        tmpCart.transform.GetComponentInChildren<Renderer>().material.color = SpawnManager.Instance.spawnCartManager.spawnMatRandomColor;

        //Get rid of that position blank
        Destroy(LevelManager.Instance.gameObject.transform.GetChild(levelIndex - 1).GetChild(0).GetChild(newCurrent).gameObject);

        //Set current for that cart
        tmpCart.transform.GetChild(0).GetComponent<CartModelContoller>().Current = newCurrent;
        ////Set track references for that cart
        tmpCart.transform.GetChild(0).GetComponent<CartModelContoller>().paths = LevelManager.Instance.gameObject.transform.GetChild(levelIndex - 1).GetChild(0).GetComponent<CartManager>().paths;
        //set cart reference for manager
        tmpCart.transform.GetComponent<CinemachineDollyCart>().m_Path = tmpCart.transform.GetChild(0).GetComponent<CartModelContoller>().paths[newCurrent];


        //****??????????
        //Enable Horizontal Check
        LevelManager.Instance.gameObject.transform.GetChild(levelIndex - 1).GetChild(0).GetComponent<CartManager>().HorizontalCheck(spawnColor, levelIndex);
       
        //LevelManager.Instance.gameObject.transform.GetChild(levelIndex - 1).GetChild(0).GetChild(0).GetComponent<CartManager>().carts[Current] = tmpCart.transform.GetChild(0).GetComponent<CartModelContoller>();

        Destroy(transform.parent.gameObject);
        //Debug.Log(tmpCart.name);

     
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