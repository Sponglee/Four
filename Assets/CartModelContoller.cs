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
            
            //Set path after calculating current
            tempCart.m_Path = paths[current];
        }
    }




    private void Start()
    {
        tempCart = gameObject.GetComponent<CinemachineDollyCart>();
    }
    //private void OnTriggerEnter(Collider other)
    //{
        
    //    if ((other.gameObject.CompareTag("Cart1")) && !CollidedBool && gameObject.GetComponent<Rigidbody>().velocity == new Vector3(0,0,0))
    //    {
    //        CollidedBool = true;
    //        Debug.Log("YAY " + other.gameObject.transform.parent.gameObject.name);
    //        //CollidedBool = true;
    //        //other.gameObject.transform.parent.gameObject.GetComponent<CartController>().MoveOut(gameObject, 1);
    //    }
    //}


    //private void OnTriggerExit(Collider other)
    //{

    //    if ((other.gameObject.CompareTag("Cart1")) && CollidedBool && gameObject.GetComponent<Rigidbody>().velocity == new Vector3(0, 0, 0))
    //    {
    //        CollidedBool = false;
    //        Debug.Log("EXIT");
    //        //CollidedBool = true;
    //        //other.transform.parent.gameObject.GetComponent<CartController>().MoveOut(gameObject, 1);
    //    }
    //}

}
