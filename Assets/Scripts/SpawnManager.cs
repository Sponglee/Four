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
    public CinemachineVirtualCamera vcamSpeedy;
    public CinemachineVirtualCamera vcamMenu;
    public CinemachineVirtualCamera vcamShop;

    public bool gameMode = true;

   
    // Use this for initialization
    void Start()
    {

        //Grab gameMode 
        gameMode = (PlayerPrefs.GetInt("GameMode", 0) != 0);

       
        Spawn();
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
        //spawnMatRandomColor = LevelManager.Instance.towerMat.color;


        //spawn cart prefab, set random position
        tmpCart = Instantiate(spawnPrefab, transform.position, Quaternion.identity, transform);

        //Set material to spawn
        //tmpCart.GetComponent<Renderer>().material.color = spawnMatRandomColor;

        //spawnObject = tmpCart;

        //Follow camera to a ball
        vcam.m_Follow = tmpCart.transform;
        vcam.m_LookAt = tmpCart.transform;

        vcamSpeedy.m_Follow = tmpCart.transform;
        vcamSpeedy.m_LookAt = tmpCart.transform;


        
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
            cart.transform.SetSiblingIndex(0);

            //rb.constraints &= ~RigidbodyConstraints.FreezePositionY;

            //rb.AddForce(0, -100f, 0);

        }


    }


    public IEnumerator StopMagnet()
    {
        while (transform.GetChild(0).gameObject.activeSelf)
        {
            transform.GetChild(0).position = transform.GetChild(1).position + Vector3.down*5f;
            yield return null;

        }
    }

    ////Drop spawned cart
    //public void DropSpawn(GameObject spawnCart)
    //{
    //    if (spawnCart != null)
    //    {
    //        spawnCart.transform.GetChild(0).GetComponent<Rigidbody>().constraints &= ~RigidbodyConstraints.FreezePositionY;
 


    //    }

    //}






  


}





