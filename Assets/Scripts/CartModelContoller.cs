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

  
}