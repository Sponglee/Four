using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CartController : MonoBehaviour {

    public Transform center;

    public CinemachineSmoothPath[] paths;
    private CinemachineDollyCart tempCart;
    public float speed;

    [SerializeField]
    private int checkCurrent;
    [SerializeField]
    private int current;
    public int Current
    {
        get
        { return current;}
        set
        {
            current = value;
            if (current < 0)
                current = paths.Length - 1;
            else if (current > paths.Length - 1)
                current = 0;
        }
    }
   
    

  
    public float angle;
    public Vector3 worldTouch;
    public Vector3 screenTouch;
    public Vector3 touchPosition;

    public Vector3 firstCartTouch;

    public Vector3 firstScreenTouch;
    public Vector3 firstTouchPosition;
    public Vector3 firstCartTouchPosition;
    public float firstTouchAngle;
    public int firstCurrent;
    public bool firstClickBool = false;

    //To prevent changing direction while moving (-1 - left, 1 - right, 0 - free)
    public int MoveDirection = 0;

    

    private void Start()
    {
        tempCart = gameObject.GetComponent<CinemachineDollyCart>();
    }

    // Update is called once per frame
    void Update () {
        Debug.DrawLine(firstCartTouch, center.position, Color.red);


        if (Input.GetMouseButtonDown(0) && IsPointerOverUIObject("Cart"))
        {

            firstClickBool = true;
            //For tracking speed
            firstTouchPosition = Input.mousePosition;
            firstScreenTouch = Camera.main.ScreenToViewportPoint(touchPosition);

            //For tracking direction
            firstCartTouchPosition = Input.mousePosition;
            firstCartTouchPosition.z = 14.4f;
            firstCartTouch = Camera.main.ScreenToWorldPoint(firstCartTouchPosition);



            ////grab initial current click
            //if (screenTouch.x < 0.5 && screenTouch.y > 0.5)
            //    firstCurrent = 0;
            //else if (screenTouch.x > 0.5 && screenTouch.y > 0.5)
            //    firstCurrent = 1;
            //else if (screenTouch.x > 0.5 && screenTouch.y < 0.5)
            //    firstCurrent = 2;
            //else if (screenTouch.x < 0.5 && screenTouch.y < 0.5)
            //    firstCurrent = 3;

        }


        if(Input.GetMouseButton(0))
        {


            angle = GetFirstClickAngle();

            //Check which part of screen coursor points on
            touchPosition = Input.mousePosition;
            screenTouch = Camera.main.ScreenToViewportPoint(touchPosition);




            if (tempCart.m_Position == 3 && Current != checkCurrent /*&& IsNearCurrent(checkCurrent,Current) */&& MoveDirection == 1)
            {
                tempCart.m_Position = 0;
                Current += 1;
               
            }
            else if (tempCart.m_Position == 0 && Current != checkCurrent /*&& IsNearCurrent(checkCurrent,Current)*/ && MoveDirection == -1)
            {
                tempCart.m_Position = 3;
                Current -= 1;
                
            }
            tempCart.m_Path = paths[Current];

            if (firstClickBool)
            {
                //check where coursor is 
                if (screenTouch.x < 0.5 && screenTouch.y > 0.5)
                    checkCurrent = 0;
                else if (screenTouch.x > 0.5 && screenTouch.y > 0.5)
                    checkCurrent = 1;
                else if (screenTouch.x > 0.5 && screenTouch.y < 0.5)
                    checkCurrent = 2;
                else if (screenTouch.x < 0.5 && screenTouch.y < 0.5)
                    checkCurrent = 3;
            }


            if (angle > 10 && MoveDirection != -1)
            {
                MoveDirection = 1;
                tempCart.m_Speed = Vector3.Distance(firstScreenTouch, screenTouch) / Time.deltaTime;
            }
            else if (angle < -10 && MoveDirection != 1)
            {
                MoveDirection = -1;
                tempCart.m_Speed = -Vector3.Distance(firstScreenTouch, screenTouch) / Time.deltaTime;
            }

            //if (tempCart.m_Position == 3)
            //{
            //    current = (current + 1) % paths.Length - 1;
            //    tempCart.m_Path = paths[current];
            //    tempCart.m_Position = 0;
            //}
        }


        if(Input.GetMouseButtonUp(0))
        {
            MoveDirection = 0;
            firstClickBool = false;
        }

        //if (Input.GetMouseButtonUp(0))
        //{

        //    tempCart.m_Path = paths[current];
        //    tempCart.m_Speed = 8;

        //}
        //else if (Input.GetMouseButtonUp(1))
        //{

        //    current = current - 1;
        //    if (current < 0)
        //        current = paths.Length-1;

        //    tempCart.m_Path = paths[current];
        //    tempCart.m_Position = 3;
        //    tempCart.m_Speed = -speed;

        //}


    }


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

    // Is touching ui
    public bool IsPointerOverUIObject(string obj)
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        if (results.Count > 0)
            return results[0].gameObject.CompareTag(obj);
        else
            return false;
    }

}
