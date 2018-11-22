using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CartController : MonoBehaviour {

    public Transform center;

    public CinemachineSmoothPath[] paths;
    private CinemachineDollyCart tempCart;
    public float speed;

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

            if (value != current)
            {
                current = value;

             
                tempCart.m_Position = 0;
                if(true)
                {
                    tempCart.m_Path = paths[current];
                    tempCart.m_Speed = Vector3.Distance(firstScreenTouch, screenTouch) / Time.deltaTime;
                }
            }
            //else if (current == value && SwipeManager.Instance.Direction != SwipeDirection.None)
            //{
            //    current = value;


            //    tempCart.m_Position = 0;
            //    if (true)
            //    {
            //        tempCart.m_Path = paths[current];
            //        tempCart.m_Speed = -Vector3.Distance(firstScreenTouch, screenTouch) / Time.deltaTime;
            //    }
            //}
            else current = value;
        }
    }

  

    public Vector3 screenTouch;
    public Vector3 touchPosition;
    public Vector3 firstScreenTouch;
    public Vector3 firstTouchPosition;
    public float firstTouchAngle;
    public int firstCurrent;

    private void Start()
    {
        tempCart = gameObject.GetComponent<CinemachineDollyCart>();
    }

    // Update is called once per frame
    void Update () {

        if(Input.GetMouseButtonDown(0))
        {

            firstTouchPosition = Input.mousePosition;
            firstScreenTouch = Camera.main.ScreenToViewportPoint(touchPosition);
            firstTouchAngle = GetFirstClickAngle();

            //grab initial current click
            if (screenTouch.x < 0.5 && screenTouch.y > 0.5)
                firstCurrent = 0;
            else if (screenTouch.x > 0.5 && screenTouch.y > 0.5)
                firstCurrent = 1;
            else if (screenTouch.x > 0.5 && screenTouch.y < 0.5)
                firstCurrent = 2;
            else if (screenTouch.x < 0.5 && screenTouch.y < 0.5)
                firstCurrent = 3;

        }


        if(Input.GetMouseButton(0))
        {
            

            touchPosition = Input.mousePosition;
            screenTouch = Camera.main.ScreenToViewportPoint(touchPosition);


            //check where coursor is 
            if (screenTouch.x < 0.5 && screenTouch.y > 0.5)
                Current = 0;
            else if (screenTouch.x > 0.5 && screenTouch.y > 0.5)
                Current = 1;
            else if (screenTouch.x > 0.5 && screenTouch.y < 0.5)
                Current = 2;
            else if (screenTouch.x < 0.5 && screenTouch.y < 0.5)
                Current = 3;

           
            
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

    // Get angle for mousePosition
    private float GetFirstClickAngle()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10;

        Vector3 screenPos = Camera.main.ScreenToWorldPoint(mousePos);
        Vector3 direction = screenPos - center.position;

        return Mathf.Atan2(Vector3.Dot(Vector3.back, Vector3.Cross(center.up, direction)), Vector3.Dot(center.up, direction)) * Mathf.Rad2Deg;
    }


}
