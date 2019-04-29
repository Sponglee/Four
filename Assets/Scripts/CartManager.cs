﻿using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CartManager : MonoBehaviour
{



    public GameObject spawnPrefab;
    public GameObject cartHolderPrefab;
    public CinemachineSmoothPath[] paths;

    //For dropping
    public GameObject spawnObject;
    //Reference to spawnManager
    public CartManager spawnManagerRef;
    //For spawn helping
    public List<Color> colorHelper;

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

    private void Start()
    {

        //spawnManagerRef = SpawnManager.Instance.spawnCartManager;
        colorHelper = new List<Color>();
        spawnMatsRef = LevelManager.Instance.spawnMats;


        if (!gameObject.CompareTag("Spawn") && !gameObject.CompareTag("Bottom"))
        {
            int index = 0;


            //***********************************TODO****************************************
            //
            //
            //
            //                  Every 5 (mod 5 != 0) transform.parents check - more one color rows

            //if(transform.parent.GetSiblingIndex()%5 == 0)
            //{
                
                  
            //}

            //currentRequiredPos = transform.parent.GetSiblingIndex() / (LevelManager.Instance.levelCount / 5);
            //Debug.Log(currentRequiredPos + ": " + transform.parent.GetSiblingIndex() + "/" + LevelManager.Instance.levelCount / 5);



            //Debug.Log(":::::" + requiredCart1 + " : " + requiredCart2);
            for (int i = 0; i < carts.Length; i++)
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
                
                if(i == currentRequiredPos)
                {
                    materialRandomizer = 1;
                }
                else if (i == requiredCart2)
                {
                    materialRandomizer = 2/*Random.Range(2,spawnMatsRef.Length)*/;
                }
                else
                {
                    materialRandomizer = Random.Range(0, spawnMatsRef.Length);
                }

                //Debug.Log(">>>>>>>"+materialRandomizer);
                //If randomizer proc or this is a required cart
                if (i == requiredCart1/*currentRequiredPos*/)
                {
                    //spawn cart prefab, set random position
                    GameObject tmpCart = Instantiate(LevelManager.Instance.cartPrefab, transform);
                    //check if it's steel
                  
                    

                    //Set a material
                    tmpCart.transform.GetChild(0).GetComponent<Renderer>().material = spawnMatsRef[materialRandomizer];

                    //Set position and orientation for dolly
                    tmpCart.transform.SetParent(tmpCartHolder.transform);
                    tmpCart.transform.position = tmpCartHolder.transform.position;
                    tmpCart.transform.rotation = tmpCartHolder.transform.rotation;
                    //tmpCart.transform.rotation = tmpCart.transform.rotation * Quaternion.Euler(-180, 0, 90);

                    //Set current for that cart
                    tmpCart.transform.GetChild(0).GetComponent<CartModelContoller>().Current = tmpCartHolder.transform.GetSiblingIndex();
                   
                    //set cart reference for manager
                    carts[index] = tmpCart.transform.GetChild(0).GetComponent<CartModelContoller>();
                   
                }
                else if(i == requiredCart2)
                {
                    //spawn cart prefab, set random position
                    GameObject tmpCart = Instantiate(LevelManager.Instance.cartPrefab, transform);


                    //Set a material
                    tmpCart.transform.GetChild(0).GetComponent<Renderer>().material = spawnMatsRef[materialRandomizer];

                    //Set position and orientation for dolly
                    tmpCart.transform.SetParent(tmpCartHolder.transform);
                    tmpCart.transform.position = tmpCartHolder.transform.position;
                    tmpCart.transform.rotation = tmpCartHolder.transform.rotation;
                    //tmpCart.transform.rotation = tmpCart.transform.rotation * Quaternion.Euler(-180, 0, 90);

                    //Set current for that cart
                    tmpCart.transform.GetChild(0).GetComponent<CartModelContoller>().Current = tmpCartHolder.transform.GetSiblingIndex();

                    //set cart reference for manager
                    carts[index] = tmpCart.transform.GetChild(0).GetComponent<CartModelContoller>();
                }
                else if (materialRandomizer == 0)
                {
                    
                   
                    //spawn cart prefab, set random position
                    GameObject tmpCart = Instantiate(LevelManager.Instance.cartPrefab, transform);
                    //check if it's steel

                    //if (materialRandomizer == 0)
                    //{

                        tmpCart.tag = "Steel";
                        tmpCart.transform.GetChild(0).tag = "Steel";
                    //}

                    //Set a material
                    tmpCart.transform.GetChild(0).GetComponent<Renderer>().material = spawnMatsRef[materialRandomizer];

                    //Set position and orientation for dolly
                    tmpCart.transform.SetParent(tmpCartHolder.transform);
                    tmpCart.transform.position = tmpCartHolder.transform.position;
                    tmpCart.transform.rotation = tmpCartHolder.transform.rotation;
                    //tmpCart.transform.rotation = tmpCart.transform.rotation * Quaternion.Euler(-180, 0, 90);

                    //Set current for that cart
                    tmpCart.transform.GetChild(0).GetComponent<CartModelContoller>().Current = tmpCartHolder.transform.GetSiblingIndex();

                    //set cart reference for manager
                    carts[index] = tmpCart.transform.GetChild(0).GetComponent<CartModelContoller>();

                }
                else
                {

                    //spawn cart prefab, set random position
                    GameObject tmpCart = Instantiate(LevelManager.Instance.cartPrefab, transform);
                    //check if it's steel

                    //if (materialRandomizer == 0)
                    //{

                  

                    //Set a material
                    tmpCart.transform.GetChild(0).GetComponent<Renderer>().material = spawnMatsRef[materialRandomizer];

                    //Set position and orientation for dolly
                    tmpCart.transform.SetParent(tmpCartHolder.transform);
                    tmpCart.transform.position = tmpCartHolder.transform.position;
                    tmpCart.transform.rotation = tmpCartHolder.transform.rotation;
                    //tmpCart.transform.rotation = tmpCart.transform.rotation * Quaternion.Euler(-180, 0, 90);

                    //Set current for that cart
                    tmpCart.transform.GetChild(0).GetComponent<CartModelContoller>().Current = tmpCartHolder.transform.GetSiblingIndex();

                    //set cart reference for manager
                    carts[index] = tmpCart.transform.GetChild(0).GetComponent<CartModelContoller>();
                }
                index++;
            }
        }
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



    ////Check if u point to nearest current
    //private bool IsNearCurrent(int checkCurr, int curr, int moveDir)
    //{

    //    int checking = Mathf.Abs(checkCurr - curr);
    //    if (checking == 1)
    //        return true;
    //    //when moving clockwise check if coursor not at 3 and vice-versa
    //    else if (checking != 1 && moveDir == 1 && checkCurr == 0 && curr == 3)
    //        return true;
    //    else if (checking != 1 && moveDir == -1 && checkCurr == 3 && curr == 0)
    //        return true;
    //    else return false;
    //}

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
        //Debug.Log(">>CHECK CARTS ");
        int cartCount = 0;
        //Debug.Log("++++++++++ " + transform.parent.GetSiblingIndex());
        foreach (Transform child in transform)
        {
            //Debug.Log(child.name);
            if(child.childCount != 0)
            {

                //Debug.Log(">>>>"+child.GetChild(0).name);
                //Check if there's 2 childs
                if (child.childCount >= 2)
                {
                    Debug.Log("<<< DESTROYED");
                    Destroy(child.GetChild(1).gameObject);
                }

                //Cound 1 child for three pop
                if (child.GetChild(0).gameObject.CompareTag("Cart"))
                {
                    cartCount++;
                    //Debug.Log(child.GetChild(0).GetComponent<CartModelContoller>().CurrentLevel);
                }
                
               
            }
            else
            {
                Debug.Log(">>FOUND CART");
                //Add a blank if it's lost
                GameObject tmpBlank = Instantiate(LevelManager.Instance.blankCartPrefab, child);
                tmpBlank.transform.SetSiblingIndex(1);
                tmpBlank.transform.GetChild(0).GetComponent<CartModelContoller>().Current = child.transform.GetSiblingIndex();

                tmpBlank.transform.position = child.transform.position;
                tmpBlank.transform.rotation = child.transform.rotation;
            }

        }

        if (gameObject.CompareTag("Cart") && cartCount == 0)
        {
            ////Debug.Log("HERE");
            //Instantiate(LevelManager.Instance.cylinderPrefab, transform.parent.position + new Vector3(0, 5, -5), Quaternion.identity, LevelManager.Instance.EffectHolder);
            Destroy(transform.parent.gameObject);
            //Get some effects 
            //LevelManager.Instance.RaiseTower();
            //Speed up the game
            SpawnManager.Instance.spawnInterval -= 0.6f / (LevelManager.Instance.levelCount - 2);
            if (SpawnManager.Instance.spawnInterval < 0.4f)
                SpawnManager.Instance.spawnInterval = 0.4f;
        }

        //if (!spawnManagerRef.spawnedBool)
        //{
        //    //SpawnManager.Instance.Spawn();
        //    //Debug.Log("NANI");
        //    //yield break;
        //}

    }

    //Check for more than 3
    public void HorizontalCheck(Color checkColor, int levelIndex)
    {
        //StartCoroutine(StopHorizontalCheck(checkColor, levelIndex));
    }

    //public IEnumerator StopHorizontalCheck(Color checkColor, int levelIndex)
    //{
    //    yield return new WaitForSecondsRealtime(0.15f);
    //    //Debug.Log(">>HORIZONTAL ");
    //    int color = 0;
    //    //Find object named Spawn for reference
    //    //CartManager spawnColorsRef = SpawnManager.Instance.spawnCartManager;
    //    List<GameObject> checkedCarts;
    //    checkedCarts = new List<GameObject>();


    //    for (int i = 0; i < transform.childCount; i++)
    //    {
    //        //Debug.Log("I " + i + " : " + transform.childCount);
    //        if (transform.GetChild(i).GetChild(0).gameObject.CompareTag("Cart"))
    //        {
    //            if (transform.GetChild(i).GetChild(0).GetChild(0).GetComponent<Renderer>().material.color
    //                == spawnColorsRef.spawnMatRandomColor)
    //            {
    //                //Add cart to list
    //                checkedCarts.Add(transform.GetChild(i).GetChild(0).GetChild(0).gameObject);
    //                color++;
    //            }


    //            //Debug.Log("Checking " + i + "|" + transform.GetChild(i).GetSiblingIndex() + " " + checkedDollys.Count);
    //        }
    //    }

    //    if (color >= 3)
    //    {
    //        //Debug.Log("MORE THAN 3");
    //        foreach (GameObject go in checkedCarts)
    //        {
    //            //Get some effects at effect position (1st child)
    //            Instantiate(LevelManager.Instance.threePrefab, go.transform.parent.GetChild(1).position, Quaternion.identity, LevelManager.Instance.EffectHolder);
                
    //            GameObject tmpBlank = Instantiate(LevelManager.Instance.blankCartPrefab, go.transform.parent.parent);
    //            tmpBlank.transform.SetSiblingIndex(1);
    //            tmpBlank.transform.GetChild(0).GetComponent<CartModelContoller>().Current = go.transform.GetSiblingIndex();

    //            tmpBlank.transform.position = go.transform.parent.transform.position;
    //            tmpBlank.transform.rotation = go.transform.parent.transform.rotation;

    //            go.transform.parent.SetParent(null);
    //            go.GetComponent<BoxCollider>().isTrigger = true;
    //            Rigidbody tmprb = go.GetComponent<Rigidbody>();
    //            tmprb.constraints = RigidbodyConstraints.None;
    //            tmprb.useGravity = true;
    //            tmprb.AddRelativeForce(new Vector3(0, 100f, 0));
    //            tmprb.AddRelativeTorque(new Vector3(1000f, 0, 0));
    //            go.tag = "Untagged";

    //            //Debug.Log("CH CRTS");
    //            CheckCarts();

               

    //        }
    //        //SCORE
    //        GameManager.Instance.Score += 5 * color;

    //        checkedCarts.Clear();
    //    }
    //    else
    //    {
    //        //int tmpRange = Random.Range(0, 2);
    //        //if(tmpRange==0)
    //        //{
    //        //    //Rotate the lower level
    //        //    LevelManager.Instance.LevelMove(levelIndex);
    //        //}
    //        //else
    //        //{
    //        //    //Rotate the lower level
    //        //    LevelManager.Instance.LevelMove(levelIndex, true);
    //        //}
            
    //    }



    //    //GetNew spawn ready
    //    if (!spawnManagerRef.spawnedBool)
    //    {
    //        //SpawnManager.Instance.Spawn();
    //        //Debug.Log("NANI");
    //    }
    //}
}