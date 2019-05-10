using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CartManager : MonoBehaviour
{



    public GameObject spawnPrefab;
    public GameObject cartHolderPrefab;



   
   

    public bool spawnedBool = false;
    public bool spawnInProgress = false;

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
    public Material[] spawnMatsRef;
    public Image canvasIdentifier;


    //for RaiseTower check if no carts
    public bool NoDollysBool = false;


    int requiredCart1;
    int requiredCart2;


    int currentRequiredPos;

    int cartsLength;

    float angStep = 5f;

    private void Start()
    {


        cartsLength = LevelManager.Instance.cartCount;
        carts = new CartModelContoller[cartsLength];


        spawnMatsRef = LevelManager.Instance.spawnMats;

        requiredCart1 = Random.Range(0, cartsLength);

        transform.parent.GetChild(1).GetChild(0).GetComponent<Renderer>().material.color = spawnMatsRef[1].color;

       

        if (!gameObject.CompareTag("Spawn") && !gameObject.CompareTag("Bottom"))
        {
            int index = 0;



            //Debug.Log(":::::" + requiredCart1 + " : " + requiredCart2);
            for (int i = 0; i < cartsLength; i++)
            {

                //Set up cart holder
                int a = 360 / LevelManager.Instance.cartCount * i;
                Vector3 cartHolderPos = RandomCircle(transform.position,0f, a);

                GameObject tmpCartHolder = Instantiate(cartHolderPrefab,cartHolderPos,Quaternion.identity, transform);

                //Set place and orientation for blank holder
                tmpCartHolder.name = i.ToString();
                tmpCartHolder.transform.rotation = tmpCartHolder.transform.rotation * Quaternion.Euler(0, a, 0);

                int spawnRandomizer = Random.Range(0, 100);
                //cart mats + steel

                int materialRandomizer;
                //Check if it's required Cart- same color as the ball (don't include steel mat)
                //materialRandomizer = i == requiredCart1 ? 1 : Random.Range(0,spawnMatsRef.Length);



                //if (i == requiredCart1)
                //{
                //    materialRandomizer = 1;
                //    continue;
                //}
                //else
                //{
                    materialRandomizer = Random.Range(0, spawnMatsRef.Length);
                //}

         
                //If randomizer proc or this is a required cart
                if (spawnRandomizer <= 30 && spawnRandomizer>5 && i == transform.parent.GetSiblingIndex()%cartsLength)
                {

                   
                    materialRandomizer = 0;

                    //spawn cart prefab, set random position
                    GameObject tmpCart = Instantiate(LevelManager.Instance.cartPrefab, transform);
                    //check if it's steel

                    //if ()
                    //{

                    tmpCart.tag = "Steel";
                    tmpCart.transform.GetChild(0).tag = "Steel";
                    //}

                    //Set a material
                    tmpCart.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Renderer>().material = spawnMatsRef[0];
                    tmpCart.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Renderer>().material = spawnMatsRef[0];

                    //Set position and orientation for dolly
                    tmpCart.transform.SetParent(tmpCartHolder.transform);
                    tmpCart.transform.position = tmpCartHolder.transform.position;
                    tmpCart.transform.rotation = tmpCartHolder.transform.rotation;
                    //tmpCart.transform.rotation = tmpCart.transform.rotation * Quaternion.Euler(-180, 0, 90);

                    //Set Current
                    tmpCart.transform.GetChild(0).GetComponent<CartModelContoller>().Current = tmpCartHolder.transform.GetSiblingIndex();
                    tmpCart.transform.GetChild(0).GetComponent<CartModelContoller>().LevelIndex = transform.parent.GetSiblingIndex();
                    //set cart reference for manager
                    carts[index] = tmpCart.transform.GetChild(0).GetComponent<CartModelContoller>();

                }
                else if(spawnRandomizer<=3 && transform.parent.GetSiblingIndex()>5)
                {
                    materialRandomizer = 0;

                    //spawn cart prefab, set random position
                    GameObject tmpCart = Instantiate(LevelManager.Instance.dangerPrefab, transform);
                    //check if it's steel

                    //if ()
                    //{

                    tmpCart.tag = "Danger";
                    tmpCart.transform.GetChild(0).tag = "Danger";
                    //}

                    ////Set a material
                    //tmpCart.transform.GetChild(0).GetComponent<Renderer>().material = spawnMatsRef[0];

                    //Set position and orientation for dolly
                    tmpCart.transform.SetParent(tmpCartHolder.transform);
                    tmpCart.transform.position = tmpCartHolder.transform.position;
                    tmpCart.transform.rotation = tmpCartHolder.transform.rotation;
                    //tmpCart.transform.rotation = tmpCart.transform.rotation * Quaternion.Euler(-180, 0, 90);

                    //Set Current
                    tmpCart.transform.GetChild(0).GetComponent<CartModelContoller>().Current = tmpCartHolder.transform.GetSiblingIndex();
                    tmpCart.transform.GetChild(0).GetComponent<CartModelContoller>().LevelIndex = transform.parent.GetSiblingIndex();
                    //set cart reference for manager
                    carts[index] = tmpCart.transform.GetChild(0).GetComponent<CartModelContoller>();
                }
                else if (spawnRandomizer > 30 && spawnRandomizer <= 40 && PlayerPrefs.GetInt("CurrentRank",1)>3 && transform.parent.GetSiblingIndex()%3==0)
                {
                    //spawn cart prefab, set random position
                    GameObject tmpCart = Instantiate(LevelManager.Instance.collectablePrefab, transform);
                    //check if it's steel

                    //if (materialRandomizer == 0)
                    //{

                    tmpCart.tag = "Collectable";
                    tmpCart.transform.GetChild(0).tag = "Collectable";
                    //}

                    ////Set a material
                    //tmpCart.transform.GetChild(0).GetComponent<Renderer>().material = spawnMatsRef[materialRandomizer];

                    //Set position and orientation for dolly
                    tmpCart.transform.SetParent(tmpCartHolder.transform);
                    tmpCart.transform.position = tmpCartHolder.transform.position;
                    tmpCart.transform.rotation = tmpCartHolder.transform.rotation;
                    //tmpCart.transform.rotation = tmpCart.transform.rotation * Quaternion.Euler(-180, 0, 90);

                    //Set current for that cart
                    //tmpCart.transform.GetChild(0).GetComponent<CartModelContoller>().Current = tmpCartHolder.transform.GetSiblingIndex();

                    //set cart reference for manager
                    //carts[index] = tmpCart.transform.GetChild(0).GetComponent<CartModelContoller>();
                }
              
                index++;
            }

        }


        transform.parent.eulerAngles = new Vector3(0, angStep * transform.parent.GetSiblingIndex(), 0);

    }


    //Build circle for spots
    public Vector3 RandomCircle(Vector3 center, float radius, int a)
    {
        //Debug.Log(a);
        float ang = a;
        Vector3 pos;
        pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.z = center.z + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        pos.y = center.y;
        return pos;
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


    
}