using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CartModelContoller : MonoBehaviour
{

    //public int modelCurrent;
    private bool CollidedBool = false;
    private CinemachineDollyCart tempCart;
    public CinemachineSmoothPath[] paths;
    public CartManager cartManager;
    private int cartNumber;


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
        tempCart = gameObject.GetComponent<CinemachineDollyCart>();
        if (gameObject.CompareTag("Cart0"))
            cartNumber = 0;
        else if (gameObject.CompareTag("Cart1"))
            cartNumber = 1;
    }

    private void OnTriggerEnter(Collider other)
    {
        //first cart selected, hits second one(this), which is not moving
        if (gameObject.CompareTag("Cart1") && cartManager.selectedIndex==0 && other.gameObject.CompareTag("Cart0") && !CollidedBool)
        {
            CollidedBool = true;
            MoveOut(cartManager.CartMoveDirection);
        }
        else if (gameObject.CompareTag("Cart0") && cartManager.selectedIndex == 1 && other.gameObject.CompareTag("Cart1") && !CollidedBool)                           
        {
            CollidedBool = true;
            MoveOut(cartManager.CartMoveDirection);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (gameObject.CompareTag("Cart1") && cartManager.selectedIndex == 0 && other.gameObject.CompareTag("Cart0") && CollidedBool 
                                                && gameObject.GetComponent<Rigidbody>().velocity == new Vector3(0, 0, 0))
        {
            CollidedBool = false;     
        }
        else if (gameObject.CompareTag("Cart0") && cartManager.selectedIndex == 1 && other.gameObject.CompareTag("Cart1") && CollidedBool 
                                                && gameObject.GetComponent<Rigidbody>().velocity == new Vector3(0, 0, 0))
        {
            CollidedBool = false;
        }
    }

    private void MoveOut(int direction)
    {
        if (GetCartAngle() > 0)
        {
            if(tempCart.m_Position == 0)
            {
                tempCart.m_Speed = -tempCart.m_Speed;
                return;
            }
            Current++;
            //Set path after calculating current
            tempCart.m_Path = paths[current];
            tempCart.m_Position = 0;
            tempCart.m_Speed = 40;

        }
        else if(GetCartAngle() < 0)
        {
            if (tempCart.m_Position == 3)
            {
                tempCart.m_Speed = -tempCart.m_Speed;
                return;
            }
            Current--;
            //Set path after calculating current
            tempCart.m_Path = paths[current];
            tempCart.m_Position = 3;
            tempCart.m_Speed = -40;
        }
    }

    // Get angle for mousePosition
    private float GetCartAngle()
    {
        Vector3 selCart = cartManager.carts[cartManager.selectedIndex].transform.position;
        Vector3 selDirection = selCart - cartManager.center.position;

        Vector3 moveCart = transform.position;
        Vector3 direction = moveCart - cartManager.center.position;

        //Get angle between mouse coursor and first touch on cart
        return Mathf.Atan2(Vector3.Dot(Vector3.back, Vector3.Cross(selDirection, direction)),
                                        Vector3.Dot(selDirection, direction)) * Mathf.Rad2Deg;
    }
}
