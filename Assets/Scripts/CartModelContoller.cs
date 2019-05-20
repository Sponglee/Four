using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CartModelContoller : MonoBehaviour
{


 

    private CinemachineDollyCart tempCart;


    [SerializeField]
    private int levelIndex;
    public int LevelIndex
    {
        get
        {
            return levelIndex;
        }

        set
        {
            levelIndex = value;
        }
    }


    public SpawnManager spawnManager;
    [SerializeField]
    private int cartNumber;

    public Color spawnColor;
    private bool moving = false;
    public bool Moving
    {
        get
        {
            return moving;
        }

        set
        {
            moving = value;
            StartCoroutine(StopMovingBool());
        }
    }



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
                current = LevelManager.Instance.cartCount-1;
            else if (current > LevelManager.Instance.cartCount-1)
                current = 0;

        }
    }

 

    private void Start()
    {
        spawnManager = SpawnManager.Instance;
      
        //Current = gameObject.transform.parent.parent.GetSiblingIndex();
    }

    private void OnTriggerEnter(Collider other)
    {
      
        if (other.gameObject.CompareTag("Bottom"))
        {
            //Debug.Log("REEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE");
            Destroy(transform.parent.gameObject);
        }
       
    }

    private IEnumerator StopMovingBool()
    {
        yield return new WaitForSecondsRealtime(0.25f);
        moving = false;
    }
  
}