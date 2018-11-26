using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CartManager : MonoBehaviour {



    public GameObject[] cartPrefabs;
    public CinemachineSmoothPath[] paths;

    public Transform center;
    public CartModelContoller[] carts;

    public int[] currents;
    

    private CinemachineDollyCart selectedDolly;
    private CartModelContoller selectedCart;
    public int selectedIndex;
    public int CartMoveDirection;

    public float speed;

    [SerializeField]
    private int checkCurrent;

    public bool IsTop = false;

    public float angle;
    private Vector3 worldTouch;
    private Vector3 screenTouch;
    private Vector3 touchPosition;

    private Vector3 firstCartTouch;

    private Vector3 firstScreenTouch;
    private Vector3 firstTouchPosition;
    private Vector3 firstCartTouchPosition;
    private float firstTouchAngle;
    private int firstCurrent;
    private bool firstClickBool = false;



    
  


    //To prevent changing direction while moving (-1 - left, 1 - right, 0 - free)
    public int MoveDirection = 0;

    private void Start()
    {
        int spawnRandomizer = Random.Range(1, 4);
        Debug.Log(spawnRandomizer);
        for (int i = 0; i< spawnRandomizer; i++)
        {
            //spawn cart prefab, set random position
            GameObject tmpCart = Instantiate(cartPrefabs[Random.Range(0, 3)], transform);
            tmpCart.transform.GetComponent<CinemachineDollyCart>().m_Path = paths[i];
            //Set current for that cart
            tmpCart.transform.GetChild(0).GetComponent<CartModelContoller>().Current = i;
            //Set track references for that cart
            tmpCart.transform.GetChild(0).GetComponent<CartModelContoller>().paths = paths;
            //set cart reference for manager
            carts[i] = tmpCart.transform.GetChild(0).GetComponent<CartModelContoller>();
        }
    }
    // Update is called once per frame
    void Update () {
        if (transform.parent.GetSiblingIndex() == 0)
            IsTop = true;

        if (Input.GetMouseButtonDown(0) && IsTop && IsPointerCast("Cart"))
        {
            //get reference to target
            GameObject tmpRayHit = GrabRayObj("Cart");
            //add info on selected Index
            selectedIndex = tmpRayHit.transform.parent.GetSiblingIndex();
            selectedCart = tmpRayHit.GetComponent<CartModelContoller>();
            selectedDolly = tmpRayHit.transform.parent.GetComponent<CinemachineDollyCart>();
           
           


            firstClickBool = true;
            //For tracking speed
            firstTouchPosition = Input.mousePosition;
            firstScreenTouch = Camera.main.ScreenToViewportPoint(touchPosition);

            //For tracking direction
            firstCartTouchPosition = Input.mousePosition;
            firstCartTouchPosition.z = 14.4f;
            firstCartTouch = Camera.main.ScreenToWorldPoint(firstCartTouchPosition);
        }

        if(Input.GetMouseButton(0) && IsTop && firstClickBool)
        {       
            angle = GetFirstClickAngle();

            //Check which part of screen coursor points on
            touchPosition = Input.mousePosition;
            screenTouch = Camera.main.ScreenToViewportPoint(touchPosition);

            // when cart reaches its goal - move next but upto checkCurrent and not too far
            if (selectedDolly.m_Position== 3 && MoveDirection == 1)
            {
                if(IsNearCurrent(checkCurrent,selectedCart.Current,MoveDirection))
                {
                    selectedDolly.m_Position = 0;
                    selectedCart.Current += 1;
                    //Set path after calculating current
                    selectedDolly.m_Path = paths[selectedCart.Current]; 
                } 
            }
            else if (selectedDolly.m_Position == 0 && MoveDirection == -1)
            {
                if (IsNearCurrent(checkCurrent,selectedCart.Current,MoveDirection))
                {   
                    selectedDolly.m_Position = 3;
                    selectedCart.Current -= 1;
                    //Set path after calculating current
                    selectedDolly.m_Path = paths[selectedCart.Current];
                }
            } 

            //check where coursor is 
            if (screenTouch.x < 0.5 && screenTouch.y > 0.5)
                checkCurrent = 0;
            else if (screenTouch.x > 0.5 && screenTouch.y > 0.5)
                checkCurrent = 1;
            else if (screenTouch.x > 0.5 && screenTouch.y < 0.5)
                checkCurrent = 2;
            else if (screenTouch.x < 0.5 && screenTouch.y < 0.5)
                checkCurrent = 3;
            
            //For moving right
            if (angle > 10 && MoveDirection != -1)
            {

                MoveDirection = 1;
                selectedDolly.m_Speed = Vector3.Distance(firstScreenTouch, screenTouch) / Time.deltaTime;
                //set min and max speeds
                if (selectedDolly.m_Speed < 8)
                    selectedDolly.m_Speed = 8;
                else if (selectedDolly.m_Speed >= 30)
                    selectedDolly.m_Speed = 30;
            }
            //For moving left
            else if (angle < -10 && MoveDirection != 1)
            {

                MoveDirection = -1;
                selectedDolly.m_Speed = -Vector3.Distance(firstScreenTouch, screenTouch) / Time.deltaTime;
                //set min and max speeds
                if (selectedDolly.m_Speed > -8)
                    selectedDolly.m_Speed = -8;
                else if (selectedDolly.m_Speed <= -30)
                    selectedDolly.m_Speed = -30;
            }

            //Write move direction to cartcontroller
            if (selectedDolly.m_Speed > 0)
                CartMoveDirection = 1;
            else if (selectedDolly.m_Speed < 0)
                CartMoveDirection = -1;
        }

        if (Input.GetMouseButtonUp(0) && IsTop)
        {
            if (MoveDirection == 0 && IsPointerCast("Cart"))
            {
                carts[selectedIndex].transform.Rotate(Vector3.up, 180f);
                carts[selectedIndex].IsLowered = !carts[selectedIndex].IsLowered;
                
                    //carts[selectedIndex].gameObject.GetComponent<BoxCollider>().isTrigger = !carts[selectedIndex].gameObject.GetComponent<BoxCollider>().isTrigger;
            }

            firstClickBool = false;
            MoveDirection = 0;
            //angle = 0;
        }

    }

    //public void SaveLastDirection(int index)
    //{
    //    if (selectedDolly.m_Speed > 0)
    //        carts[index].lastDirection = 1;
    //    else if (selectedDolly.m_Speed < 0)
    //        carts[index].lastDirection = -1;

    //    carts[index].lastDirectionTimer += Time.deltaTime;
    //}

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
        return Mathf.Atan2(Vector3.Dot(Vector3.back, Vector3.Cross(firstCartTouch-center.position, direction)), 
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
            Debug.Log(">"+i+" "  + result.gameObject.tag);
             
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
    private GameObject GrabRayObj(string obj)
    {
        RaycastHit hit;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 100f))
        {
            if (hit.transform)
            {
                if (hit.transform.gameObject.CompareTag(obj))
                {
                    Debug.DrawRay(hit.transform.position, Camera.main.transform.position, Color.red, 5f);
                    return hit.transform.gameObject;
                }
            }
        }
        return null;
    }
}
