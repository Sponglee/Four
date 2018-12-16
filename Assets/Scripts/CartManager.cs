using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CartManager : MonoBehaviour {


  
    public GameObject[] cartPrefabs;
    public CinemachineSmoothPath[] paths;

    //For dropping
    public GameObject spawnObject;
    //Reference to spawnManager
    public CartManager spawnManager;
    public bool spawnedBool = false;
    public Transform center;
    public CartModelContoller[] carts;

    public int[] currents;
    

    private CinemachineDollyCart selectedDolly;
    private CartModelContoller selectedCart;
    public int selectedIndex;
    public int CartMoveDirection;

    public float speed;
    private Vector3 firstCartTouch;
    
    //To prevent changing direction while moving (-1 - left, 1 - right, 0 - free)
    public int MoveDirection = 0;

    //For nextSpawn carts
    public Color spawnMatRandomColor;
    public int spawnMatRandomizer;
    public float spawnTimer;
    public float spawnDuration = 0.5f;

    public Image canvasIdentifier;
    public Material[] spawnMats;

    //for RaiseTower check if no carts
    public bool NoDollysBool = false;

    //For spawn helping
    public List<Color> colorHelper;



    private void Start()
    {
        colorHelper = new List<Color>();
        spawnManager = GameObject.Find("Spawn").transform.GetChild(0).GetChild(0).GetComponent<CartManager>();

        if (gameObject.CompareTag("Spawn"))
        {
            //spawnTimer = spawnDuration;
            //set random spawn color
            spawnMatRandomizer = Random.Range(0,4);
            canvasIdentifier.color = spawnMats[spawnMatRandomizer].color;
            Spawn();
            
        }
      


        if (gameObject.CompareTag("Cart"))
        {
          
         
           
            int index = 0;
            for (int i = 0; i < 4; i++)
            {
                int spawnRandomizer = Random.Range(0, 100);
                int materialRandomizer = Random.Range(0, spawnMats.Length);
                if(spawnRandomizer<=60)
                {
                    //spawn cart prefab, set random position
                    GameObject tmpCart = Instantiate(cartPrefabs[0], transform);
                    tmpCart.transform.GetChild(0).GetComponent<Renderer>().material = spawnMats[materialRandomizer];
                    tmpCart.transform.GetComponent<CinemachineDollyCart>().m_Path = paths[i % 4];
                    //Set current for that cart
                    tmpCart.transform.GetChild(0).GetComponent<CartModelContoller>().Current = i % 4;
                    //Set track references for that cart
                    tmpCart.transform.GetChild(0).GetComponent<CartModelContoller>().paths = paths;
                    //set cart reference for manager
                    carts[index] = tmpCart.transform.GetChild(0).GetComponent<CartModelContoller>();
                    index++;

                }
                else
                {
                    //spawn cart prefab, set random position
                    GameObject tmpCart = Instantiate(LevelManager.Instance.blankCartPrefab, transform);
                    tmpCart.transform.GetComponent<CinemachineDollyCart>().m_Path = paths[i % 4];
                    //Set current for that cart
                    
                    index++;
                }
                
            }


        }
       

    }


    // Update is called once per frame
    void Update()
    {

        if (gameObject.CompareTag("Spawn") && Input.GetMouseButtonUp(0) 
            && !LevelManager.Instance.RotationProgress && !LevelManager.Instance.SpawnInProgress /*&& spawnTimer <= 0*/)
        {
            GameObject tmpRayCart = GrabSpawnObj(transform, "Cart");
            if (tmpRayCart != null && tmpRayCart.GetComponent<Renderer>().material.color != spawnMats[spawnMatRandomizer].color
                && tmpRayCart.transform.parent.parent.childCount >= 4 && tmpRayCart.transform.parent.parent.parent.GetSiblingIndex() == 0)
            {
                //Debug.Log("NOT SAME ");
            }
            else 
            {
                DropSpawn(spawnObject);
                LevelManager.Instance.SpawnInProgress = true;
                spawnedBool = false;
            }


            
           
            
            
            ////Reset spawn cooldown
            //spawnTimer = spawnDuration;
        }

    
        //if (gameObject.CompareTag("Spawn") && spawnTimer > 0)
        //{
        //    spawnTimer -= Time.fixedUnscaledDeltaTime;
        //}
    }


    //Spawn new cart
    public void Spawn()
    {
        spawnedBool = true;
        //spawn cart prefab, set random position
        GameObject tmpCart = Instantiate(cartPrefabs[0], transform);
        spawnObject = tmpCart;
        //Set material to spawn
        tmpCart.transform.GetChild(0).GetComponent<Renderer>().material = spawnMats[spawnMatRandomizer];
        tmpCart.transform.GetComponent<CinemachineDollyCart>().m_Path = paths[2];
        //Set current for that cart
        tmpCart.transform.GetChild(0).GetComponent<CartModelContoller>().Current = 2;
        //set material number
        tmpCart.transform.GetChild(0).GetComponent<CartModelContoller>().spawnNumber = spawnMatRandomizer;
        //Set track references for that cart
        tmpCart.transform.GetChild(0).GetComponent<CartModelContoller>().paths = paths;
        //set cart reference for manager
        carts[0] = tmpCart.transform.GetChild(0).GetComponent<CartModelContoller>();
        //Set parent of Level manager
        //tmpCart.transform.SetParent(LevelManager.Instance.transform);
    }



    public void DropSpawn(GameObject spawnCart)
    {
        spawnCart.transform.GetChild(0).GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        //set color of next spawn
        spawnMatRandomizer = Random.Range(0, cartPrefabs.Length);
        canvasIdentifier.color = spawnMats[spawnMatRandomizer].color;
      
    }

    //Check if u point to nearest current
    private bool IsNearCurrent(int checkCurr, int curr, int moveDir)
    {

        int checking = Mathf.Abs(checkCurr - curr);
        if (checking == 1)
            return true;
        //when moving clockwise check if coursor not at 3 and vice-versa
        else if (checking != 1 && moveDir == 1 && checkCurr == 0 && curr == 3)
            return true;
        else if (checking != 1 && moveDir == -1 && checkCurr ==3  && curr == 0)
            return true;
        else return false;
    }

    // Get angle for mousePosition
    private float GetFirstClickAngle()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 20f;

        Vector3 screenPos = Camera.main.ScreenToWorldPoint(mousePos);
        
        Vector3 direction = screenPos - center.position;
        
        Debug.DrawLine(screenPos,center.position,Color.blue);
      
        //Get angle between mouse coursor and first touch on cart
        return Mathf.Atan2(Vector3.Dot(Vector3.up, Vector3.Cross(firstCartTouch-center.position, direction)), 
                                        Vector3.Dot(firstCartTouch- center.position, direction)) * Mathf.Rad2Deg;
    }



    // Is touching tag
    private bool IsPointerOverUIObject(string obj)
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        int i = 0;
        foreach ( RaycastResult result in results)
        {
            i++;
            //Debug.Log(">"+i+" "  + result.gameObject.tag);
             
        }

        if (results.Count > 0)
            return results[0].gameObject.CompareTag(obj);
        else
            return false;
    }

    //check if obj was hit by ray with tag
    private bool IsPointerCast(string obj)
    {
        RaycastHit hit;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
       
        if (Physics.Raycast(ray, out hit, 100f))
        {
            if(hit.transform)
            {
                if (hit.transform.gameObject.CompareTag(obj))
                {
                    //Debug.DrawRay(hit.transform.position, Camera.main.transform.position,Color.red,5f);
                    return true;
                }
            }
        }
        return false;
    }

    //Get reference to object hit by ray with tag
    private GameObject GrabObj(string obj)
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
       

        if (Physics.Raycast(ray, out hit, 100f))
        {
            if (hit.transform)
            {
                if (hit.transform.gameObject.CompareTag(obj))
                {
                  
                    return hit.transform.gameObject;
                }
            }
        }
        return null;
    }

    //Get reference to object hit by ray with tag
    private GameObject GrabSpawnObj(Transform origin, string obj)
    {
        RaycastHit hit;
        Vector3 dir = origin.position + new Vector3(0, -100f, 0f);

            Debug.DrawLine(origin.position, dir, Color.red, 10f);

        if (Physics.Raycast(origin.position, -Vector3.up, out hit))
        {
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

    //Check to RaiseTower
    public void CheckCarts()
    {
        StartCoroutine(StopCheckCarts());
    }

    public IEnumerator StopCheckCarts()
    {
        yield return new WaitForSeconds(0.1f);
       
        int cartCount = 0;
        foreach (Transform child in transform)
        {
            if (child.gameObject.CompareTag("Cart"))
            {
                cartCount++;
            }
        }

        if (cartCount == 0)
        {
            Instantiate(LevelManager.Instance.cylinderPrefab, transform.parent.position + new Vector3(0, 5, -5), Quaternion.identity, LevelManager.Instance.EffectHolder);
            Destroy(transform.parent.gameObject);
            //Get some effects 
            LevelManager.Instance.RaiseTower();
        }
        //GetNew spawn ready
        if(!spawnManager.spawnedBool)
        {

            spawnManager.Spawn();
            Debug.Log("NANI");

        }
        
    }
    
    //Check for more than 3
    public void HorizontalCheck(int checkNumber)
    {
        StartCoroutine(StopHorizontalCheck(checkNumber));
    }

    public IEnumerator StopHorizontalCheck(int checkNumber)
    {
        yield return new WaitForSecondsRealtime(0.05f);
        int color = 0;
        //Find object named Spawn for reference
        GameObject spawnColorsRef = GameObject.Find("Spawn");
        List<GameObject> checkedDollys;
        checkedDollys = new List<GameObject>();

        
        for (int i = 0; i < transform.childCount; i++)
        {
            //Debug.Log("I " + i + " : " + transform.childCount);
            if (transform.GetChild(i).gameObject.CompareTag("Cart"))
            {
                if(transform.GetChild(i).GetChild(0).GetComponent<Renderer>().material.color 
                    == spawnColorsRef.transform.GetChild(0).GetChild(0).GetComponent<CartManager>().spawnMats[checkNumber].color)
                {
                    checkedDollys.Add(transform.GetChild(i).GetChild(0).gameObject);
                    color++;
                }
              

            //Debug.Log("Checking "+ i + "|" + transform.GetChild(i).GetSiblingIndex() + " " + checkedDollys.Count);
            }
        }

        if(color>=3)
        {
            //Debug.Log("MORE THAN 3");
            foreach (GameObject go in checkedDollys)
            {
                //Get some effects at effect position (1 child)
                Instantiate(LevelManager.Instance.threePrefab, go.transform.parent.GetChild(1).position, Quaternion.identity, LevelManager.Instance.EffectHolder);
                go.transform.parent.SetParent(null);

                go.GetComponent<BoxCollider>().isTrigger = true;
                Rigidbody tmprb = go.GetComponent<Rigidbody>();
                tmprb.constraints = RigidbodyConstraints.None;
                tmprb.useGravity = true;
                tmprb.AddRelativeForce(new Vector3(0, 100f, 0));
                tmprb.AddRelativeTorque(new Vector3(1000f, 0, 0));



                Instantiate(LevelManager.Instance.blankCartPrefab, transform);
                CheckCarts();
            }
        }
        //GetNew spawn ready

        spawnManager.Spawn();

    }
}
