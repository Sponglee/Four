using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CartController : MonoBehaviour {

    public CinemachineSmoothPath[] paths;

    public Transform center;
    public CartModelContoller[] carts;

    public int[] currents;
    public GameObject[] cartModels;

    private CinemachineDollyCart selectedDolly;
    private CartModelContoller selectedCart;
    public int selectedIndex;

    public float speed;

    [SerializeField]
    private int checkCurrent;
   
   
 

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
    private int MoveDirection = 0;

    

    private void Start()
    {
        
    }

    // Update is called once per frame
    void Update () {


      

        
        if (Input.GetMouseButtonDown(0) && (IsPointerCast("Cart1") || IsPointerCast("Cart2")))
        {
            if (IsPointerCast("Cart1"))
            {
                selectedIndex = 0;
                selectedCart = carts[0];
                selectedDolly = carts[0].gameObject.GetComponent<CinemachineDollyCart>();
               
            }
            else if (IsPointerCast("Cart2"))
            {
                selectedIndex = 1;
                selectedCart = carts[1];
                selectedDolly = carts[1].gameObject.GetComponent<CinemachineDollyCart>();
            }


            firstClickBool = true;
            //For tracking speed
            firstTouchPosition = Input.mousePosition;
            firstScreenTouch = Camera.main.ScreenToViewportPoint(touchPosition);

            //For tracking direction
            firstCartTouchPosition = Input.mousePosition;
            firstCartTouchPosition.z = 14.4f;
            firstCartTouch = Camera.main.ScreenToWorldPoint(firstCartTouchPosition);

        }


        if(Input.GetMouseButton(0) && firstClickBool)
        {


            angle = GetFirstClickAngle();

            //Check which part of screen coursor points on
            touchPosition = Input.mousePosition;
            screenTouch = Camera.main.ScreenToViewportPoint(touchPosition);



            // when cart reaches its goal - move next but upto checkCurrent and not too far
            if (selectedDolly.m_Position== 3 && MoveDirection == 1)
            {

                if(IsNearCurrent(checkCurrent,selectedCart.Current))
                {
                    selectedDolly.m_Position = 0;
                    selectedCart.Current += 1;
                    Debug.Log("DING " + checkCurrent + ":" + selectedCart.Current);
                }

               
            }
            else if (selectedDolly.m_Position == 0 && MoveDirection == -1)
            {
                if (IsNearCurrent(checkCurrent,selectedCart.Current))
                {
                    Debug.Log("DONG " + checkCurrent + ":" + selectedCart.Current);
                    selectedDolly.m_Position = 3;
                    selectedCart.Current -= 1;
                }

            }

            

            //Check if u clicked on cart first
            //if (firstClickBool)
            //{
                //check where coursor is 
            if (screenTouch.x < 0.5 && screenTouch.y > 0.5)
                checkCurrent = 0;
            else if (screenTouch.x > 0.5 && screenTouch.y > 0.5)
                checkCurrent = 1;
            else if (screenTouch.x > 0.5 && screenTouch.y < 0.5)
                checkCurrent = 2;
            else if (screenTouch.x < 0.5 && screenTouch.y < 0.5)
                checkCurrent = 3;
            //}

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

        }


        if (Input.GetMouseButtonUp(0))
        {
            if (MoveDirection == 0 && (IsPointerCast("Cart1") || IsPointerCast("Cart2")))
            {
                carts[selectedIndex].transform.GetChild(0).Rotate(Vector3.up, 180f);
            }

            MoveDirection = 0;
            firstClickBool = false;
        }

    }


    //Check if u point to nearest current
    private bool IsNearCurrent(int checkCurr, int curr)
    {

        if (checkCurr == 0 && curr == 3)
            return true;
        else if (checkCurr == 3 && curr == 0)
            return true;
        else if (Mathf.Abs(checkCurr - curr) == 1)
            return true;
        else return false;
    }


    // Get angle for mousePosition
    private float GetFirstClickAngle()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 14.4f;

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


    private bool IsPointerCast(string obj)
    {
        RaycastHit hit;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 100f))
        {
            if(hit.transform)
            {
                if (hit.transform.gameObject.CompareTag(obj))
                    return true;
            }
        }
        return false;
    }


   


    
}
