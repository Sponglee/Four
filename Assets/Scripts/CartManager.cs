using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CartManager : MonoBehaviour
{



    public GameObject[] cartPrefabs;
    public CinemachineSmoothPath[] paths;

    //For dropping
    public GameObject spawnObject;
    //Reference to spawnManager
    public CartManager spawnManagerRef;
    //For spawn helping
    public List<Color> colorHelper;

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
    public float spawnTimer;
    public float spawnDuration = 0.5f;

    public Image canvasIdentifier;
    public Material[] spawnMats;

    //for RaiseTower check if no carts
    public bool NoDollysBool = false;




    private void Start()
    {

        spawnManagerRef = SpawnManager.Instance.spawnCartManager;
        colorHelper = new List<Color>();




        if (gameObject.CompareTag("Cart") )
        {
            int index = 0;
            for (int i = 0; i < carts.Length; i++)
            {
                int spawnRandomizer = Random.Range(0, 100);
                int materialRandomizer = Random.Range(0, spawnMats.Length);
                if (spawnRandomizer <= 60)
                {
                    //spawn cart prefab, set random position
                    GameObject tmpCart = Instantiate(cartPrefabs[0], transform);
                    if(materialRandomizer == spawnMats.Length - 1)
                    {
                        tmpCart.tag = "Steel";
                        tmpCart.transform.GetChild(0).tag = "Steel";
                    }
                    tmpCart.transform.GetChild(0).GetComponent<Renderer>().material = spawnMats[materialRandomizer];
                    tmpCart.transform.GetComponent<CinemachineDollyCart>().m_Path = paths[i % paths.Length];
                    //Set current for that cart
                    tmpCart.transform.GetChild(0).GetComponent<CartModelContoller>().Current = i % paths.Length;
                    //Set track references for that cart
                    tmpCart.transform.GetChild(0).GetComponent<CartModelContoller>().paths = paths;
                    //set cart reference for manager
                    carts[index] = tmpCart.transform.GetChild(0).GetComponent<CartModelContoller>();
                   
                }
                else
                {
                    //spawn blank prefab, set random position
                    GameObject tmpCart = Instantiate(LevelManager.Instance.blankCartPrefab, transform);
                    //Set current for that blank
                    tmpCart.transform.GetChild(0).GetComponent<CartModelContoller>().Current = i % paths.Length;

                    //Set track references for that cart
                    tmpCart.transform.GetComponent<CinemachineDollyCart>().m_Path = paths[i % paths.Length];
                    tmpCart.transform.GetChild(0).GetComponent<CartModelContoller>().paths = paths;
                    //set cart reference for manager
                    carts[index] = tmpCart.transform.GetChild(0).GetComponent<CartModelContoller>();
                }
                index++;
            }
        }
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
        else if (checking != 1 && moveDir == -1 && checkCurr == 3 && curr == 0)
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

        Debug.DrawLine(screenPos, center.position, Color.blue);

        //Get angle between mouse coursor and first touch on cart
        return Mathf.Atan2(Vector3.Dot(Vector3.up, Vector3.Cross(firstCartTouch - center.position, direction)),
                                        Vector3.Dot(firstCartTouch - center.position, direction)) * Mathf.Rad2Deg;
    }



    // Is touching tag
    private bool IsPointerOverUIObject(string obj)
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        int i = 0;
        foreach (RaycastResult result in results)
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
            if (hit.transform)
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
                //Debug.Log(child.GetChild(0).GetComponent<CartModelContoller>().CurrentLevel);
               
            }
        }

        if (gameObject.CompareTag("Cart") && cartCount == 0)
        {
            Instantiate(LevelManager.Instance.cylinderPrefab, transform.parent.position + new Vector3(0, 5, -5), Quaternion.identity, LevelManager.Instance.EffectHolder);
            Destroy(transform.parent.gameObject);
            //Get some effects 
            LevelManager.Instance.RaiseTower();
        }
        else if (!spawnManagerRef.spawnedBool)
        {
            SpawnManager.Instance.Spawn();
            //Debug.Log("NANI");
            //yield break;
        }

    }

    //Check for more than 3
    public void HorizontalCheck(Color checkColor, int levelIndex)
    {
        StartCoroutine(StopHorizontalCheck(checkColor, levelIndex));
    }

    public IEnumerator StopHorizontalCheck(Color checkColor, int levelIndex)
    {
        yield return new WaitForSecondsRealtime(0.05f);
        int color = 0;
        //Find object named Spawn for reference
        CartManager spawnColorsRef = SpawnManager.Instance.spawnCartManager;
        List<GameObject> checkedDollys;
        checkedDollys = new List<GameObject>();


        for (int i = 0; i < transform.childCount; i++)
        {
            //Debug.Log("I " + i + " : " + transform.childCount);
            if (transform.GetChild(i).gameObject.CompareTag("Cart"))
            {
                if (transform.GetChild(i).GetChild(0).GetComponent<Renderer>().material.color
                    == spawnColorsRef.spawnMatRandomColor)
                {
                    checkedDollys.Add(transform.GetChild(i).GetChild(0).gameObject);
                    color++;
                }


                //Debug.Log("Checking " + i + "|" + transform.GetChild(i).GetSiblingIndex() + " " + checkedDollys.Count);
            }
        }

        if (color >= 3)
        {
            //Debug.Log("MORE THAN 3");
            foreach (GameObject go in checkedDollys)
            {
                //Get some effects at effect position (1 child)
                Instantiate(LevelManager.Instance.threePrefab, go.transform.parent.GetChild(1).position, Quaternion.identity, LevelManager.Instance.EffectHolder);
                
                GameObject tmpBlank = Instantiate(LevelManager.Instance.blankCartPrefab, transform);
                tmpBlank.transform.GetChild(0).GetComponent<CartModelContoller>().Current = go.transform.GetSiblingIndex();



                go.transform.parent.SetParent(null);
                go.GetComponent<BoxCollider>().isTrigger = true;
                Rigidbody tmprb = go.GetComponent<Rigidbody>();
                tmprb.constraints = RigidbodyConstraints.None;
                tmprb.useGravity = true;
                tmprb.AddRelativeForce(new Vector3(0, 100f, 0));
                tmprb.AddRelativeTorque(new Vector3(1000f, 0, 0));
                go.tag = "Untagged";


                CheckCarts();
            }
        }
        else
        {
            //Rotate the lower level
            LevelManager.Instance.LevelMove(levelIndex);
        }



        //GetNew spawn ready
        if (!spawnManagerRef.spawnedBool)
        {
            SpawnManager.Instance.Spawn();
            //Debug.Log("NANI");
        }
    }
}