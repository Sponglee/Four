using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : Singleton<SpawnManager>
{

    public CartManager spawnCartManager;


    public CinemachineVirtualCamera vCam;
    // Use this for initialization
    void Start()
    {
        spawnCartManager = transform.GetChild(0).GetComponent<CartManager>();

        //Spawn();
    }

    // Update is called once per frame
    void Update()
    {

        //if (Input.GetMouseButtonUp(0)
        //    && !LevelManager.Instance.RotationProgress /*&& !LevelManager.Instance.SpawnInProgress && spawnTimer <= 0*/)
        //{
        //    //GameObject tmpRayCart = GrabSpawnObj(transform, "Cart");
        //    //if (tmpRayCart != null && tmpRayCart.GetComponent<Renderer>().material.color != spawnCartManager.spawnMatRandomColor
        //    //    && tmpRayCart.transform.parent.parent.childCount >= 4 && tmpRayCart.transform.parent.parent.parent.GetSiblingIndex() == 0)
        //    //{
        //    //    //Debug.Log("NOT SAME ");
        //    //}
        //    //else
        //    //{
        //    //    DropSpawn(spawnCartManager.spawnObject);
        //    //    
        //    //spawnCartManager.spawnedBool = false;
        //    //  
        //    //}
        //    //LevelManager.Instance.SpawnInProgress = true;
           



        //    ////Reset spawn cooldown
        //    //spawnTimer = spawnDuration;
        //}

    }



    //Spawn new cart
    public void Spawn()
    {

        
        ////spawn cart prefab, set random position
        GameObject tmpCart = Instantiate(spawnCartManager.cartPrefabs[0], spawnCartManager.transform);

        ////Set reference to spawn
        spawnCartManager.spawnObject = tmpCart;

      
        ////Set camera target
        vCam.m_LookAt = tmpCart.transform.GetChild(0);
        vCam.m_Follow = tmpCart.transform.GetChild(0);
    }

    //Get reference to object hit by ray with tag
    private GameObject GrabSpawnObj(Transform origin, string obj = "")
    {
        RaycastHit hit;
        Vector3 dir = origin.position + new Vector3(0, -100f, 0f);

        Debug.DrawLine(origin.position, dir, Color.red, 10f);

        if (Physics.Raycast(origin.position, -Vector3.up, out hit))
        {
            if (hit.transform)
            {
                //if (hit.transform.gameObject.CompareTag(obj))
                //{
                //    return hit.transform.gameObject;
                //}
                return hit.transform.gameObject;
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
            if (origin.GetChild(i).gameObject.CompareTag("Cart"))
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

            rb.constraints = RigidbodyConstraints.None;
            rb.constraints = RigidbodyConstraints.FreezePositionX;
            rb.constraints = RigidbodyConstraints.FreezePositionZ;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            rb.AddForce(0, -100f, 0);

        }


    }


    public void Bounce()
    {
        Debug.Log("BOUNCE");
        Rigidbody rb = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Rigidbody>();
        rb.velocity = new Vector3(0, 0, 0);
        rb.AddForce(0, 16f, 0);
    }
}