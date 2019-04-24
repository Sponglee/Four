using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : Singleton<SpawnManager>
{
    public Color nextSpawnColor;
    public Color spawnMatRandomColor;
    public GameObject spawnPrefab;

    public float spawnInterval = 2;
    public float spawnTime = 0;

    //Reference to spawn cart
    public GameObject tmpCart;

    public CinemachineVirtualCamera vcam;

    public bool gameMode = true;

   
    // Use this for initialization
    void Start()
    {

        //Grab gameMode 
        gameMode = (PlayerPrefs.GetInt("GameMode", 0) != 0);

       
        Spawn();
    }

    // Update is called once per frame
    void Update()
    {

     

    }



    //Spawn new cart
    public void Spawn()
    {
        StartCoroutine(StopSpawn());
        
    }




    public IEnumerator StopSpawn()
    {
       

      
        yield return new WaitForSeconds(0.3f);


        //initialize Spawn Color
        spawnMatRandomColor = LevelManager.Instance.spawnMats[1].color;


        //spawn cart prefab, set random position
        tmpCart = Instantiate(spawnPrefab, transform.position, Quaternion.identity);

        //Set material to spawn
        tmpCart.GetComponent<Renderer>().material.color = spawnMatRandomColor;

        //spawnObject = tmpCart;

        //Follow camera to a ball
        vcam.m_Follow = tmpCart.transform;
        vcam.m_LookAt = tmpCart.transform;
    }

    
   
  

    //GrabSpawnObj reload for spawn check only
    public GameObject ScanCarts(Transform origin, string obj, bool SpawnGrab)
    {
        RaycastHit hit;
        Vector3 dir = origin.position + new Vector3(0, -100f, -3f);

        Debug.DrawLine(origin.position + new Vector3(0, -3f, -3f), dir, Color.red, 10f);

        if (Physics.Raycast(origin.position + new Vector3(0, -3f, -3f), -Vector3.up, out hit))
        {
            if (hit.transform)
            {
                if (hit.transform.gameObject.CompareTag(obj))
                {
                    return hit.transform.gameObject;
                }
            }
        }
        return null;

    }


    //Check how many carts on the level
    public int CheckDollyCount(Transform origin)
    {
        int dollyCount = 0;
        for (int i = 0; i < origin.childCount; i++)
        {
            //Debug.Log("I " + i + " : " + transform.childCount);
            if (origin.GetChild(i).gameObject.CompareTag("Cart") || origin.GetChild(i).gameObject.CompareTag("Steel"))
            {
                dollyCount++;
            }
        }
        return dollyCount;
    }

    //Drop spawned cart
    public void DropCart(GameObject cart)
    {
        if (cart != null)
        {

            Rigidbody rb = cart.GetComponent<Rigidbody>();
            //DETACH
            //cart.transform.parent.parent.GetComponent<CartManager>().CheckCarts();
            cart.transform.parent.SetParent(transform);

            rb.constraints &= ~RigidbodyConstraints.FreezePositionY;
           
            rb.AddForce(0, -100f, 0);

        }


    }
    //Drop spawned cart
    public void DropSpawn(GameObject spawnCart)
    {
        if (spawnCart != null)
        {
            spawnCart.transform.GetChild(0).GetComponent<Rigidbody>().constraints &= ~RigidbodyConstraints.FreezePositionY;
 


        }

    }






  


}





