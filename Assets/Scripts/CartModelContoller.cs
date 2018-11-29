using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CartModelContoller : MonoBehaviour
{

    //public int modelCurrent;
    private bool collidedBool = false;
    public bool CollidedBool
    {
        get
        {
            return collidedBool;
        }

        set
        {
            collidedBool = value;
            StartCoroutine(StopCollided());

        }
    }

    public IEnumerator StopCollided()
    {
        yield return new WaitForSeconds(0.3f);
        collidedBool = false;
    }

    private CinemachineDollyCart tempCart;
    public CinemachineSmoothPath[] paths;
    public CartManager cartManager;
    [SerializeField]
    private int cartNumber;

    public bool IsLowered = false;

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
        tempCart = gameObject.transform.parent.GetComponent<CinemachineDollyCart>();
        cartNumber = tempCart.transform.GetSiblingIndex();
        
    }


    private void OnCollisionEnter(Collision other)
    {
        if(gameObject.CompareTag("Spawn") && other.gameObject.CompareTag("Cart"))
        {



            Destroy(transform.parent.gameObject);
            Destroy(other.transform.parent.gameObject);





        }

    }
    private void OnTriggerEnter(Collider other)
    {
        ////////first cart selected, hits second one(this), which is not moving
        //if (cartManager.selectedIndex != cartNumber && !collidedBool)
        //{
        //    
        //    //MoveOut(cartManager.CartMoveDirection);
        //    CollidedBool = true;
        //}

            Debug.Log(gameObject.tag);
        ////first cart selected, hits second one(this), which is not moving
        //if (cartManager.selectedIndex != cartNumber && !collidedBool)
        //{
        //    CollidedBool = true;
        //    MoveOut(cartManager.CartMoveDirection);
        //}
       
    }



    private void MoveOut(int direction)
    {
        //foreach(CartModelContoller tempModel in cartManager.carts)
        //{

        //}
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
        else if (GetCartAngle() < 0)
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
