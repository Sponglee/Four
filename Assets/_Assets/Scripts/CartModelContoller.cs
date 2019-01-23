using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CartModelContoller : MonoBehaviour
{
    //=============================





    //for tracking same color detatch
    public bool sameColorDrop = false;
    //Track level of spawn
    [SerializeField]
    private int currentLevel = -2;
    public int CurrentLevel
    {
        get
        {
            currentLevel = transform.parent.GetSiblingIndex();
            return currentLevel;
        }

        set
        {
            //if(gameObject.CompareTag("Spawn"))
            //{
            //    LevelManager.Instance.Level = value;

            //}

            currentLevel = value;
        }
    }


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
    private CinemachineDollyCart tempCart;
    public CinemachineSmoothPath[] paths;
    public CartManager spawnManager;
    [SerializeField]
    private int cartNumber;

    public Color spawnColor;
    public bool IsLowered = false;

    ////for Direction control
    //public int lastDirection;
    //public float lastDirectionTimer;


    //For collisions
    private float colInertia;

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


    //=========================================

  

    public IEnumerator StopCollided()
    {
       
        yield return new WaitForSeconds(1f);
        collidedBool = false;
    }

  

    private void Start()
    {
        colInertia = LevelManager.Instance.collisionInertia;
        //tempCart = gameObject.transform.parent.GetComponent<CinemachineDollyCart>();
        //cartNumber = tempCart.transform.GetSiblingIndex();
        //int tmp = CurrentLevel;
        currentLevel = transform.parent.GetSiblingIndex();
    }



    //RUNNER
    private void Update()
    {

      

        ////DEbug;
        //colInertia = LevelManager.Instance.collisionInertia;
        //Bring spawn back to center
        if (transform.localPosition.z != 0)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition,
                                                        new Vector3(transform.localPosition.x,
                                                        transform.localPosition.y, colInertia), colInertia);
            //Debug.Log(transform.localPosition.z);

        }

    }



    //private void OnTriggerExit(Collider other)
    //{

    //}


    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("Obstacle"))
        {
            StartCoroutine(LevelManager.Instance.StopCollision());
        }
        else if(other.transform.CompareTag("Spawn"))
        {
           
            LevelManager.Instance.Level = currentLevel;
          
        }
    }


    //Get reference to object hit by ray with tag
    private GameObject GrabRayObj(Collision other, string obj)
    {
        RaycastHit hit;
        Vector3 dir = other.transform.position + new Vector3(0, -100f, -2.5f);


        if (Physics.Raycast(other.transform.position + new Vector3(0, -0.5f, -2.5f), -Vector3.up, out hit))
        {
            Debug.DrawLine(other.transform.position + new Vector3(0, -0.5f, -2.5f), dir, Color.red, 10f);
            if (hit.transform)
            {
                if (hit.transform.gameObject.CompareTag(obj))
                {
                    return hit.transform.gameObject;
                }
                return hit.transform.gameObject;
            }
        }
        return null;




    }




  


}