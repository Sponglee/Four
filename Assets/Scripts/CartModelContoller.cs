using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CartModelContoller : MonoBehaviour
{
    //Track level of spawn
    public int currentLevel = 0;
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
        if(gameObject.CompareTag("Cart"))
        {
            currentLevel = transform.parent.parent.parent.GetSiblingIndex();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("Bottom") && gameObject.CompareTag("Cart"))
        {
            
            Destroy(transform.parent.gameObject);
        }
        else if(other.gameObject.CompareTag("Bottom") && gameObject.CompareTag("Spawn"))
        {
            //Destroy(gameObject.transform.parent.gameObject);
           
        } 
       
    }


    private void OnCollisionEnter(Collision other)
    {
        //Debug.Log("STICK " + currentLevel + " TO " + other.transform.parent.parent.parent.GetSiblingIndex());
        //if hits something below( or up <- fix this)
        if (gameObject.CompareTag("Spawn") && other.gameObject.CompareTag("Cart") 
            && gameObject.transform.position.y > other.transform.position.y)
        {
           
            if (gameObject.GetComponent<Renderer>().material.color == other.gameObject.GetComponent<Renderer>().material.color)
            {
                
                Instantiate(LevelManager.Instance.blankCartPrefab,other.transform.parent.parent);
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
                rb.AddRelativeTorque(new Vector3(5000f, 0,0));

                //Get some effects 
                Instantiate(LevelManager.Instance.hitPrefab,gameObject.transform.position + new Vector3(0, 5,-5), Quaternion.identity, LevelManager.Instance.EffectHolder);
                //For pizzaz
                //StartCoroutine(LevelManager.Instance.TiDi(0.05f));




                //GameObject tmpRay = GrabRayObj(other, "Cart");
                
                //if (tmpRay.GetComponent<Renderer>().material.color != gameObject.GetComponent<Renderer>().material.color)
                //{
                //    LevelManager.Instance.SpawnInProgress = false;
                //    //destroy holder if no dollys
                //    //Debug.Log("THEN SECOND");
                //    transform.parent.parent.GetComponent<CartManager>().CheckCarts();
                //    //DETACH
                //    //transform.parent.SetParent(null);
                //    //gameObject.GetComponent<BoxCollider>().isTrigger = true;
                //    //Rigidbody tmprb = gameObject.GetComponent<Rigidbody>();
                //    //tmprb.constraints = RigidbodyConstraints.None;
                //    //tmprb.useGravity = true;
                //    //tmprb.velocity = new Vector3(0, 0, -50f);
                //    //tmprb.AddRelativeTorque(new Vector3(1000f, 0, 0));
                  
                //}

            }
            else
            {
                int levelIndex = other.transform.parent.parent.parent.GetSiblingIndex();
                if (levelIndex >= 1)
                {
                    StickCart(other, levelIndex);
                }
            }
            
            //else if (other.transform.parent != null /*&& LevelManager.Instance.SpawnInProgress == true*/)
            //{
            //    //Debug.Log("SIDEBUMP");
            //    ////get index of levelHolder above
            //    int levelIndex = other.transform.parent.parent.parent.GetSiblingIndex();
               

            //   
            //    ////else
            //    ////{
            //    ////    //SceneManager.LoadScene("Main");
            //    ////}
            //}

        }

        if (gameObject.CompareTag("Spawn") && other.gameObject.CompareTag("Cart") && currentLevel == other.gameObject.GetComponent<CartModelContoller>().currentLevel)
        {
            Debug.Log(currentLevel + " : " + other.gameObject.GetComponent<CartModelContoller>().currentLevel);
            MoveOut(gameObject.transform, other.transform, other.transform.parent.parent.parent.GetSiblingIndex());
        }
        if (gameObject.CompareTag("Spawn") && other.gameObject.CompareTag("Bottom"))
        {
            SceneManager.LoadScene("Main");
            int levelIndex = other.transform.parent.parent.parent.GetSiblingIndex();
            if (levelIndex >= 1)
            {
            }
            else
            {

                //SceneManager.LoadScene("Main");
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
      
        int newCurrent = other.gameObject.GetComponent<CartModelContoller>().Current;

        // remember what level it's on currently
        currentLevel = levelIndex -1;


        LevelManager.Instance.SpawnInProgress = false;
        

        
    }


    private void MoveOut( Transform spawn, Transform other, int levelIndex)
    {
        //foreach (Transform tempModel in other.parent.parent)
        //{
            
        //}
        CinemachineDollyCart otherCart = other.parent.GetComponent<CinemachineDollyCart>();
        Debug.Log(GetCartAngle(spawn, other, levelIndex));

        if (GetCartAngle(spawn,other, levelIndex) > 0)
        {
            if (otherCart.m_Position == 0)
            {
                otherCart.m_Speed = -otherCart.m_Speed;
                return;
            }
            int otherCurrent = other.GetComponent<CartModelContoller>().Current++;
            //Set path after calculating current
            otherCart.m_Path = other.GetComponent<CartModelContoller>().paths[otherCurrent];
            otherCart.m_Position = 0;
            otherCart.m_Speed = 40;

        }
        else if (GetCartAngle(spawn, other, levelIndex) < 0)
        {
            if (tempCart.m_Position == 3)
            {
                otherCart.m_Speed = -otherCart.m_Speed;
                return;
            }
            int otherCurrent = other.GetComponent<CartModelContoller>().Current++;
            //Set path after calculating current
            otherCart.m_Path = other.GetComponent<CartModelContoller>().paths[otherCurrent];
            otherCart.m_Position = 3;
            otherCart.m_Speed = -40;

        }
    }

    // Get angle for mousePosition
    private float GetCartAngle(Transform spawn, Transform other, int levelIndex)
    {
        Vector3 spawnDirection = spawn.position - LevelManager.Instance.transform.GetChild(levelIndex).transform.position;
        Vector3 otherDirection = other.parent.position - LevelManager.Instance.transform.GetChild(levelIndex).transform.position;

        Debug.DrawLine(spawn.position,
            LevelManager.Instance.transform.GetChild(levelIndex).transform.position, Color.black, 5f);
        Debug.DrawLine(other.parent.position, 
            LevelManager.Instance.transform.GetChild(levelIndex).transform.position, Color.green,5f);

        //Get angle between mouse coursor and first touch on cart
        return Mathf.Atan2(Vector3.Dot(Vector3.back, Vector3.Cross(spawnDirection, otherDirection)),
                                        Vector3.Dot(spawnDirection, otherDirection)) * Mathf.Rad2Deg;


    }
}
