using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CartModelContoller : MonoBehaviour
{
    //for tracking same color detatch
    public bool sameColorDrop = false;
    //Track level of spawn
    [SerializeField]
    private int currentLevel = -2;
    public int CurrentLevel
    {
        get
        {
            if (gameObject.CompareTag("Cart"))
            {
                currentLevel = transform.parent.parent.parent.GetSiblingIndex();
            }
            return currentLevel;
        }

        set
        {
            if(gameObject.CompareTag("Spawn"))
            {
                //LevelManager.Instance.Level = value;
                
            }
      
            currentLevel = value;
        }
    }


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
        yield return new WaitForSeconds(1f);
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
        int tmp = CurrentLevel;
      
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Bottom") && gameObject.CompareTag("Cart"))
        {

            Destroy(transform.parent.gameObject);
        }
        else if (other.gameObject.CompareTag("Bottom") && gameObject.CompareTag("Spawn"))
        {
            //Destroy(gameObject.transform.parent.gameObject);

        }

    }


    private void OnCollisionEnter(Collision other)
    {
        //Debug.Log("STICK " + currentLevel + " TO " + other.transform.parent.parent.parent.GetSiblingIndex());
        //if hits something below( or up <- fix this)

        if (gameObject.CompareTag("Cart") && other.gameObject.CompareTag("Cart") 
            && gameObject.transform.position.y - other.transform.position.y > 0.5f)
        {
           
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
                StartCoroutine(LevelManager.Instance.TiDi(0.05f));





            }
            else
            {

                int levelIndex = other.transform.parent.parent.parent.GetSiblingIndex();
                if (sameColorDrop)
                {
                    //Pop sequence
                    gameObject.GetComponent<BoxCollider>().isTrigger = true;
                    Rigidbody rb = gameObject.GetComponent<Rigidbody>();
                    rb.constraints = RigidbodyConstraints.None;
                    rb.useGravity = true;
                    rb.velocity = new Vector3(Random.Range(-10f, 10f), 50f, -50f);
                    rb.AddRelativeTorque(new Vector3(5000f, 0, 0));
                }
                else if (levelIndex >= 1)
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
        //If carts are on the same level
        else if (gameObject.CompareTag("Cart") && other.gameObject.CompareTag("Cart")
        && CurrentLevel == other.gameObject.GetComponent<CartModelContoller>().CurrentLevel
        && !gameObject.GetComponent<CartModelContoller>().CollidedBool
        /*&& !gameObject.gameObject.GetComponent<CartModelContoller>().CollidedBool*/)
        {
            //Debug.Log("CART HIT " + gameObject.transform.GetComponent<CartModelContoller>().current);
            CollidedBool = true;
            MoveOut(gameObject.transform, other.transform, gameObject.transform.GetComponent<CartModelContoller>().currentLevel);
        }
        //Else if spawn is hitting cart and not same level
        else if (gameObject.CompareTag("Spawn") && other.gameObject.CompareTag("Cart")
            && gameObject.transform.position.y - other.transform.position.y > 0.1f
             /*&& CurrentLevel != other.gameObject.GetComponent<CartModelContoller>().CurrentLevel*/)
        {
            GameObject tmpRay = GrabRayObj(other, "Cart");

            CurrentLevel = other.gameObject.GetComponent<CartModelContoller>().CurrentLevel - 1;
            //Debug.Log(other.gameObject.GetComponent<CartModelContoller>().currentLevel + " : " + tmpRay.GetComponent<CartModelContoller>().currentLevel);
            //Drop if lower one same color
            if (tmpRay != null && tmpRay.GetComponent<Renderer>().material.color == other.gameObject.GetComponent<Renderer>().material.color)
            {
                other.gameObject.GetComponent<CartModelContoller>().sameColorDrop = true;
                SpawnManager.Instance.DropCart(other.transform.gameObject);

            }
            ////Drop cart if only there's more than 1 level to move
            //else if (tmpRay != null && tmpRay.GetComponent<CartModelContoller>().currentLevel - other.gameObject.GetComponent<CartModelContoller>().currentLevel > 1)
            //{
            //    //LevelManager.Instance.SpawnInProgress = false;
            //    SpawnManager.Instance.DropCart(other.transform.gameObject);
            //}


        }

        //Detach cart from bottom
        if (gameObject.CompareTag("Cart") && other.gameObject.CompareTag("Bottom"))
        {
            //Debug.Log("REE");
            //DETACH
            transform.parent.SetParent(null);
            //Pop sequence
            gameObject.GetComponent<BoxCollider>().isTrigger = true;
            Rigidbody rb = gameObject.GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.None;
            rb.useGravity = true;
            rb.velocity = new Vector3(Random.Range(-10f, 10f), 50f, -50f);
            rb.AddRelativeTorque(new Vector3(5000f, 0, 0));

            //Get some effects 
            Instantiate(LevelManager.Instance.hitPrefab, other.gameObject.transform.position + new Vector3(0, 5, -5), Quaternion.identity, LevelManager.Instance.EffectHolder);
            //For pizzaz
            StartCoroutine(LevelManager.Instance.TiDi(0.05f));
        }

        //Move cart if spawn push
        if (gameObject.CompareTag("Spawn") && other.gameObject.CompareTag("Cart")
        && CurrentLevel == other.gameObject.GetComponent<CartModelContoller>().CurrentLevel
        && gameObject.transform.position.y - other.transform.position.y <0.1f
        && !gameObject.GetComponent<CartModelContoller>().CollidedBool)
        {
            //Debug.Log("SPAWN MOVE " + currentLevel + " : " + other.gameObject.GetComponent<CartModelContoller>().currentLevel);
            CollidedBool = true;
            MoveOut(gameObject.transform, other.transform, other.transform.parent.parent.parent.GetSiblingIndex());




        }
        //win condition
        if (gameObject.CompareTag("Spawn") && other.gameObject.CompareTag("Bottom"))
        {
            SceneManager.LoadScene("Main");
            //int levelIndex = other.transform.parent.parent.parent.GetSiblingIndex();
            //if (levelIndex >= 1)
            //{
            //}
            //else
            //{

            //    //SceneManager.LoadScene("Main");
            //}
        }
    }


    //Get reference to object hit by ray with tag
    private GameObject GrabRayObj(Collision other, string obj)
    {
        RaycastHit hit;
        Vector3 dir = other.transform.position + new Vector3(0, -100f, -2.5f);


        if (Physics.Raycast(other.transform.position + new Vector3(0, -0.5f, -2.5f), -Vector3.up, out hit))
        {
            Debug.DrawLine(other.transform.position + new Vector3(0, -0.5f, -2.5f), dir, Color.red, 10f);
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
        CurrentLevel = levelIndex - 1;
        //Remove 1 blank
        //Destroy(LevelManager.Instance.gameObject.transform.GetChild(levelIndex - 1).GetChild(0).Find("BlankHolder(Clone)").gameObject);
        //spawn cart prefab, set current position
        //SpawnManager.Instance.Bounce();

        GameObject tmpCart = Instantiate(LevelManager.Instance.gameObject.transform.GetChild(levelIndex - 1)
            .GetChild(0).GetComponent<CartManager>().cartPrefabs[0], LevelManager.Instance.gameObject.transform.GetChild(levelIndex - 1)
            .GetChild(0).transform);
        //Set material
        tmpCart.transform.GetComponentInChildren<Renderer>().material = gameObject.transform.GetComponentInChildren<Renderer>().material;
        //Set current for that cart
        tmpCart.transform.GetChild(0).GetComponent<CartModelContoller>().Current = newCurrent;
        ////Set track references for that cart
        tmpCart.transform.GetChild(0).GetComponent<CartModelContoller>().paths = LevelManager.Instance.gameObject.transform.GetChild(levelIndex - 1).GetChild(0).GetComponent<CartManager>().paths;
        //set cart reference for manager
        tmpCart.transform.GetComponent<CinemachineDollyCart>().m_Path = tmpCart.transform.GetChild(0).GetComponent<CartModelContoller>().paths[newCurrent];
        tmpCart.transform.GetChild(0).GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        SpawnManager.Instance.Bounce();

        //Enable Horizontal Check
        other.transform.parent.parent.GetComponent<CartManager>().HorizontalCheck(spawnColor);
        LevelManager.Instance.SpawnInProgress = false;
        //LevelManager.Instance.gameObject.transform.GetChild(levelIndex - 1).GetChild(0).GetChild(0).GetComponent<CartManager>().carts[Current] = tmpCart.transform.GetChild(0).GetComponent<CartModelContoller>();

        Destroy(transform.parent.gameObject);

        //Debug.Log(tmpCart.name);


    }


    private void MoveOut(Transform spawn, Transform other, int levelIndex)
    {

        CollidedBool = true;

        CinemachineDollyCart otherCart = other.parent.GetComponent<CinemachineDollyCart>();
        //Debug.Log(GetCartAngle(spawn, other, levelIndex) + " : " + other.GetComponent<CartModelContoller>().Current);

        if (GetCartAngle(spawn, other, levelIndex) < 0)
        {

            LevelManager.Instance.LevelRotate(levelIndex, -1);


        }
        else if (GetCartAngle(spawn, other, levelIndex) > 0)
        {
            LevelManager.Instance.LevelRotate(levelIndex, 1);

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
            LevelManager.Instance.transform.GetChild(levelIndex).transform.position, Color.green, 5f);

        //Get angle between mouse coursor and first touch on cart
        return Mathf.Atan2(Vector3.Dot(Vector3.back, Vector3.Cross(spawnDirection, otherDirection)),
                                        Vector3.Dot(spawnDirection, otherDirection)) * Mathf.Rad2Deg;


    }


    //private void MoveOut(Transform spawn, Transform other, int levelIndex)
    //{

        
    //    //other.GetComponent<CartModelContoller>().CollidedBool = true;
        
    //    float cartSpeed = 40;
    //    CinemachineDollyCart otherCart = other.parent.GetComponent<CinemachineDollyCart>();
    //    Debug.Log(GetCartAngle(spawn, other, levelIndex) + " : " + other.GetComponent<CartModelContoller>().Current);
    //    //if (spawn.CompareTag("Spawn"))
    //    //{
    //    //    cartSpeed = 10;
    //    //}
        
    //    if (GetCartAngle(spawn, other, levelIndex) < 0)
    //    {
    //        if (otherCart.m_Position <= 3 && otherCart.m_Position >0)
    //        {
               
    //            otherCart.m_Speed = -cartSpeed;
               
    //            Debug.Log("-: " + other.GetComponent<CartModelContoller>().Current);
    //            return;
    //        }
    //        else if (otherCart.m_Position >= 0 && otherCart.m_Position < 3)
    //        {
    //            other.GetComponent<CartModelContoller>().Current--;
    //            Debug.Log("current: " + other.GetComponent<CartModelContoller>().Current);
    //            //Set path after calculating current
    //            otherCart.m_Path = other.GetComponent<CartModelContoller>().paths[other.GetComponent<CartModelContoller>().Current];
    //            otherCart.m_Position = 3;
    //            otherCart.m_Speed = -cartSpeed;
                
    //            return;
    //        }



    //    }
    //    else if (GetCartAngle(spawn, other, levelIndex) > 0)
    //    {
    //        if (otherCart.m_Position >= 0 && otherCart.m_Position < 3)
    //        {
    //            otherCart.m_Speed = cartSpeed;

    //            Debug.Log("-: " + other.GetComponent<CartModelContoller>().Current);
    //            return;
    //        }
    //        else if (otherCart.m_Position <= 3 && otherCart.m_Position > 0)
    //        {
    //            other.GetComponent<CartModelContoller>().Current++;
    //            //Set path after calculating current
    //            otherCart.m_Path = other.GetComponent<CartModelContoller>().paths[other.GetComponent<CartModelContoller>().Current];
    //            otherCart.m_Position = 0;
    //            otherCart.m_Speed = cartSpeed;
    //            Debug.Log("current: " + other.GetComponent<CartModelContoller>().Current);
    //            return;
    //        }


    //    }
    //}
}