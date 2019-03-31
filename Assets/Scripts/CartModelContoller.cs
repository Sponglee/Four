using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CartModelContoller : MonoBehaviour
{

    //public int modelCurrent;
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
        }
    }


    private void Start()
    {
        tempCart = gameObject.transform.parent.GetComponent<CinemachineDollyCart>();
        cartNumber = tempCart.transform.GetSiblingIndex();

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
       
        if (/*!CollidedBool &&*/gameObject.CompareTag("Spawn") && (other.gameObject.CompareTag("Cart") || other.gameObject.CompareTag("Bottom") || other.gameObject.CompareTag("Steel")))
        {
            //Debug.Log("COLLISION " + gameObject.name);
            //CollidedBool = true;
            if (gameObject.GetComponent<Renderer>().material.color == other.gameObject.GetComponent<Renderer>().material.color)
            {

                Instantiate(LevelManager.Instance.blankCartPrefab, other.transform.parent.parent);
                //Debug.Log("FIRST SAME COLOR");

                //check if no dollys
                other.transform.parent.parent.GetComponent<CartManager>().CheckCarts();
                //DETACH
                other.transform.parent.SetParent(null);
                //Pop sequence
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
                



                GameObject tmpRay = GrabRayObj(other, "Cart");

                //Second cart below check for color
                if (tmpRay.GetComponent<Renderer>().material.color != gameObject.GetComponent<Renderer>().material.color)
                {
                  
                    //destroy holder if no dollys
                    //Debug.Log("THEN SECOND");
                    transform.parent.parent.GetComponent<CartManager>().CheckCarts();
                    //DETACH
                    //transform.parent.SetParent(null);
                    gameObject.GetComponent<BoxCollider>().isTrigger = true;
                    Rigidbody tmprb = gameObject.GetComponent<Rigidbody>();
                    tmprb.constraints = RigidbodyConstraints.None;
                    tmprb.useGravity = true;
                    tmprb.velocity = new Vector3(0, 0, -50f);
                    tmprb.AddRelativeTorque(new Vector3(1000f, 0, 0));

                }

               
            }
            else if (other.transform.parent != null)
            {
                //get index of levelHolder above
                int levelIndex = other.transform.parent.parent.parent.GetSiblingIndex();
                if (levelIndex >= 1)
                {
                    //Debug.Log("STICK CART");
                    StickCart(other, levelIndex);
                }
                //else
                //{
                //    SceneManager.LoadScene("Main");
                //}
            }
            else if (gameObject.CompareTag("Spawn") &&  other.gameObject.CompareTag("Bottom"))
            {
                int levelIndex = other.transform.parent.parent.GetSiblingIndex();
                if (levelIndex >= 1)
                {
                    StickCart(other, levelIndex);
                }
                //else
                //{

                //    SceneManager.LoadScene("Main");
                //}
            }
          
        }
    
    }


    //Get reference to object hit by ray with tag
    private GameObject GrabRayObj(Collision other, string obj)
    {
        RaycastHit hit;
        Vector3 dir = other.transform.position + new Vector3(0, -100f, -2.5f);


        if (Physics.Raycast(other.transform.position + new Vector3(0, 0, -2.5f), -Vector3.up, out hit))
        {
            Debug.DrawLine(other.transform.position + new Vector3(0, 0, -2.5f), dir, Color.red, 10f);
            if (hit.transform)
            {
                if (hit.transform.gameObject.CompareTag(obj))
                {
                    return hit.transform.gameObject;
                }
                return hit.transform.gameObject;
            }
        }
        return null;




    }


    public void StickCart(Collision other, int levelIndex)
    {
        Debug.Log("STICK CART");
        int newCurrent = other.gameObject.GetComponent<CartModelContoller>().Current;
        //Remove 1 blank
        Destroy(LevelManager.Instance.gameObject.transform.GetChild(levelIndex - 1).GetChild(0).Find("BlankHolder(Clone)").gameObject);
        //spawn cart prefab, set current position
        GameObject tmpCart = Instantiate(LevelManager.Instance.gameObject.transform.GetChild(levelIndex - 1).GetChild(0).GetComponent<CartManager>().cartPrefabs[0], LevelManager.Instance.gameObject.transform.GetChild(levelIndex - 1).GetChild(0).transform);
        //Set material
        tmpCart.transform.GetComponentInChildren<Renderer>().material.color = SpawnManager.Instance.spawnCartManager.spawnMatRandomColor;
        //Set current for that cart
        tmpCart.transform.GetChild(0).GetComponent<CartModelContoller>().Current = newCurrent;
        ////Set track references for that cart
        tmpCart.transform.GetChild(0).GetComponent<CartModelContoller>().paths = LevelManager.Instance.gameObject.transform.GetChild(levelIndex - 1).GetChild(0).GetComponent<CartManager>().paths;
        //set cart reference for manager
        tmpCart.transform.GetComponent<CinemachineDollyCart>().m_Path = tmpCart.transform.GetChild(0).GetComponent<CartModelContoller>().paths[newCurrent];

        //Enable Horizontal Check
        LevelManager.Instance.gameObject.transform.GetChild(levelIndex - 1).GetChild(0).GetComponent<CartManager>().HorizontalCheck(spawnColor);
       
        //LevelManager.Instance.gameObject.transform.GetChild(levelIndex - 1).GetChild(0).GetChild(0).GetComponent<CartManager>().carts[Current] = tmpCart.transform.GetChild(0).GetComponent<CartModelContoller>();

        Destroy(transform.parent.gameObject);
        //Debug.Log(tmpCart.name);

        LevelManager.Instance.LevelMove(levelIndex);
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