using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/** Swipe direction */
public enum SwipeDirection
{
    None = 0,
    Left = 1,
    Right = 2,
    Up = 4,
    Down = 8,
}



public class SwipeManager : Singleton<SwipeManager>
{


    public SwipeDirection Direction { set; get; }


    private Vector3 touchPosition;
    private Vector3 screenTouch;



    private float swipeResistance = 5f;


    public bool SwipeC = true;
    public bool swipeValue = false;


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {


        Direction = SwipeDirection.None;
        if (Input.GetMouseButtonDown(0))
        {
            touchPosition = Input.mousePosition;
            screenTouch = Camera.main.ScreenToViewportPoint(touchPosition);
        }

        if (Input.GetMouseButton(0))
        {
            Vector2 deltaSwipe = touchPosition - Input.mousePosition;




            if (Mathf.Abs(deltaSwipe.x) > Mathf.Abs(deltaSwipe.y) && Mathf.Abs(deltaSwipe.x) > swipeResistance)
            {
                if (screenTouch.y >= 0.5)
                {
                    Direction |= (deltaSwipe.x < 0) ? SwipeDirection.Left : SwipeDirection.Right;
                }
                else
                    Direction |= (deltaSwipe.x < 0) ? SwipeDirection.Right : SwipeDirection.Left;
            }
            else if (Mathf.Abs(deltaSwipe.y) > Mathf.Abs(deltaSwipe.x) && Mathf.Abs(deltaSwipe.y) > swipeResistance)
            {
                if (screenTouch.x >= 0.5)
                {
                    Direction |= (deltaSwipe.y < 0) ? SwipeDirection.Up : SwipeDirection.Down;
                }
                else
                    Direction |= (deltaSwipe.y < 0) ? SwipeDirection.Up : SwipeDirection.Down;
            }
            else
            {

                Direction |= SwipeDirection.None;
            }
            //Debug.Log(Direction);
        }

    }
    public bool IsSwiping(SwipeDirection dir)
    {

        return (Direction & dir) == dir;
    }


    public void SwipeChange()
    {
        swipeValue = !swipeValue;

    }
}