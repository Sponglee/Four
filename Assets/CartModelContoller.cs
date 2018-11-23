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
    //Keep track of what direction cart was moving for skipping track bug(0-3 position etc)
    public int lastDirection;
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
    }

    private void OnTriggerEnter(Collider other)
    {
        //first cart selected, hits second one(this), which is not moving
        if (gameObject.CompareTag("Cart2") && cartManager.selectedIndex==0 && other.gameObject.CompareTag("Cart1") && !CollidedBool 
                                            && gameObject.GetComponent<Rigidbody>().velocity == new Vector3(0, 0, 0))
        {
            CollidedBool = true;
            Debug.Log("YAY Cart2");
          
           
            MoveOut(cartManager.CartMoveDiretion);
        }
        else if (gameObject.CompareTag("Cart1") && cartManager.selectedIndex == 1 && other.gameObject.CompareTag("Cart2") && !CollidedBool 
                                            && gameObject.GetComponent<Rigidbody>().velocity == new Vector3(0, 0, 0))
        {
            CollidedBool = true;

            Debug.Log("YAY Cart 1");

            MoveOut(cartManager.CartMoveDiretion);
        }
    }


    private void OnTriggerExit(Collider other)
    {

        if (gameObject.CompareTag("Cart2") && cartManager.selectedIndex == 0 && other.gameObject.CompareTag("Cart1") && CollidedBool 
                                                && gameObject.GetComponent<Rigidbody>().velocity == new Vector3(0, 0, 0))
        {
            CollidedBool = false;
            Debug.Log("EXIT 2");
            
            //other.transform.parent.gameObject.GetComponent<CartController>().MoveOut(gameObject, 1);
        }
        else if (gameObject.CompareTag("Cart1") && cartManager.selectedIndex == 1 && other.gameObject.CompareTag("Cart2") && CollidedBool 
                                                && gameObject.GetComponent<Rigidbody>().velocity == new Vector3(0, 0, 0))
        {
            CollidedBool = false;
            Debug.Log("EXIT 1");

            //other.transform.parent.gameObject.GetComponent<CartController>().MoveOut(gameObject, 1);
        }
    }


    private void MoveOut(int direction)
    {
        if(lastDirection != direction)
        {
            tempCart.m_Speed = -tempCart.m_Speed;
            lastDirection = direction;
            Debug.Log("REEE");
            return;
        }
        if(direction == 1)
        {
            Current++;
            //Set path after calculating current
            tempCart.m_Path = paths[current];
            tempCart.m_Position = 0;
            tempCart.m_Speed = 30;

        }
        else if(direction == -1)
        {
            Current--;
            //Set path after calculating current
            tempCart.m_Path = paths[current];
            tempCart.m_Position = 3;
            tempCart.m_Speed = -30;
        }
    }
}
