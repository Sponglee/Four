using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CartModelContoller : MonoBehaviour
{


 

    private CinemachineDollyCart tempCart;
    public CinemachineSmoothPath[] paths;

    public SpawnManager spawnManager;
    [SerializeField]
    private int cartNumber;

    public Color spawnColor;
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

            //if(!gameObject.CompareTag("Spawn"))
            //    Debug.Log(name + " : " + current);
          
        }
    }


    private void Start()
    {
        spawnManager = SpawnManager.Instance;
        //Current = gameObject.transform.parent.parent.GetSiblingIndex();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Bottom") && gameObject.CompareTag("Cart"))
        {
            Destroy(transform.parent.gameObject);
        }
        else if (other.gameObject.CompareTag("Bottom") && gameObject.CompareTag("Spawn"))
        {
            Destroy(gameObject.transform.parent.gameObject);
        }
    }


   

   

    public void StickCart(Collision other, int levelIndex)
    {
        int newCurrent = other.gameObject.GetComponent<CartModelContoller>().Current;


      
        //Remove 1 blank
        //Destroy(LevelManager.Instance.gameObject.transform.GetChild(levelIndex - 1).GetChild(0).Find("BlankHolder(Clone)").gameObject);


        //spawn cart prefab
        GameObject tmpCart = Instantiate(LevelManager.Instance.cartPrefab, LevelManager.Instance.gameObject.transform.GetChild(levelIndex - 1).GetChild(0).GetChild(newCurrent).transform);
        //Set material and orientation
        tmpCart.transform.SetSiblingIndex(0);
        tmpCart.transform.GetComponentInChildren<Renderer>().material.color = SpawnManager.Instance.spawnCartManager.spawnMatRandomColor;
        tmpCart.transform.position = LevelManager.Instance.gameObject.transform.GetChild(levelIndex - 1).GetChild(0).GetChild(newCurrent).position;
        tmpCart.transform.rotation = tmpCart.transform.parent.rotation;
        tmpCart.transform.GetChild(0).GetComponent<CartModelContoller>().Current = newCurrent;
        //tmpCart.transform.rotation = tmpCart.transform.rotation * Quaternion.Euler(-180, 0, 90);


        ////Get rid of that position blank
        foreach (Transform child in tmpCart.transform.parent)
        {
            if (child.CompareTag("Blank"))
            {
                Destroy(child.gameObject);
            }
        }

        Destroy(transform.parent.gameObject);






        //****??????????
        //Enable Horizontal Check
 
        LevelManager.Instance.gameObject.transform.GetChild(levelIndex - 1)
                                                        .GetChild(0).GetComponent<CartManager>().HorizontalCheck(spawnColor, levelIndex);



    }


    //private void MoveOut(int direction)
    //{
    //    //foreach(CartModelContoller tempModel in cartManager.carts)
    //    //{

    //    //}
    //    if (GetCartAngle() > 0)
    //    {
    //        if(tempCart.m_Position == 0)
    //        {
    //            tempCart.m_Speed = -tempCart.m_Speed;
    //            return;
    //        }
    //        Current++;
    //        //Set path after calculating current
    //        tempCart.m_Path = paths[current];
    //        tempCart.m_Position = 0;
    //        tempCart.m_Speed = 40;

    //    }
    //    else if (GetCartAngle() < 0)
    //    {
    //        if (tempCart.m_Position == 3)
    //        {
    //            tempCart.m_Speed = -tempCart.m_Speed;
    //            return;
    //        }
    //        Current--;
    //        //Set path after calculating current
    //        tempCart.m_Path = paths[current];
    //        tempCart.m_Position = 3;
    //        tempCart.m_Speed = -40;
    //    }
    //}

    //// Get angle for mousePosition
    //private float GetCartAngle()
    //{
    //    Vector3 selCart = cartManager.carts[cartManager.selectedIndex].transform.position;
    //    Vector3 selDirection = selCart - cartManager.center.position;

    //    Vector3 moveCart = transform.position;
    //    Vector3 direction = moveCart - cartManager.center.position;

    //    //Get angle between mouse coursor and first touch on cart
    //    return Mathf.Atan2(Vector3.Dot(Vector3.back, Vector3.Cross(selDirection, direction)),
    //                                    Vector3.Dot(selDirection, direction)) * Mathf.Rad2Deg;
    //}
}