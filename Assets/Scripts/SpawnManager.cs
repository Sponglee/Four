using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : Singleton<SpawnManager>
{
    public Color nextSpawnColor;
    public CartManager spawnCartManager;

    public float spawnInterval = 2;

    public float spawnTime = 0;

    // Use this for initialization
    void Start()
    {
        spawnCartManager = transform.GetChild(0).GetComponent<CartManager>();

        Spawn();
    }

    // Update is called once per frame
    void Update()
    {

        //if (Input.GetMouseButtonUp(0)
        //    && !LevelManager.Instance.RotationProgress 
        //        && !LevelManager.Instance.SpawnInProgress 
        //            && !LevelManager.Instance.LevelMoveProgress /*&& spawnTimer <= 0*/)
        //{
        //    GameObject tmpRayCart = ScanCarts(transform, "Cart", true);
        //    if (tmpRayCart != null && tmpRayCart.GetComponent<Renderer>().material.color != spawnCartManager.spawnMatRandomColor
        //        && tmpRayCart.transform.parent.parent.childCount >= 4 && tmpRayCart.transform.parent.parent.parent.GetSiblingIndex() == 0)
        //    {
        //        //Debug.Log("NOT SAME ");
        //    }
        //    else
        //    {
        //        DropSpawn(spawnCartManager.spawnObject);
        //        LevelManager.Instance.SpawnInProgress = true;
        //        spawnCartManager.spawnedBool = false;
        //    }



                


        //    ////Reset spawn cooldown
        //    //spawnTimer = spawnDuration;
        //}

        if(spawnCartManager.spawnedBool)
            spawnTime += Time.deltaTime;

        if(spawnTime>= spawnInterval )
        {
            if(Input.GetMouseButtonDown(1))
            {
                DropSpawn(spawnCartManager.spawnObject);

                spawnCartManager.spawnedBool = false;

                spawnTime = 0;
            }
           
        }
        
    }

    

    //Spawn new cart
    public void Spawn()
    {
        StartCoroutine(StopSpawn());
        
    }




    public IEnumerator StopSpawn()
    {
       
        spawnCartManager.spawnedBool = true;
        yield return new WaitForSeconds(0.3f);
        spawnCartManager.colorHelper.Clear();
        //set random spawn color\
        List<GameObject> spawnChecks = new List<GameObject>();
        spawnChecks = LevelManager.Instance.ScanCarts(LevelManager.Instance.transform, "Cart");

        //Debug.Log(spawnChecks.Count);
        //Debug.Log(spawnCheck.name);
        if (spawnChecks != null)
        {
           
            //GAME OVER CHECK
            if (LevelManager.Instance.transform.GetChild(0).GetChild(0).CompareTag("Bottom"))
            {
                FunctionHandler.Instance.OpenGameOver("YOU WIN");
                yield break;
            }
            //Last level helper
            else if (LevelManager.Instance.transform.GetChild(1).GetChild(0).CompareTag("Bottom"))
            {
                //Debug.Log("LAST LEVEL HELPER");
                foreach (Transform dolly in LevelManager.Instance.transform.GetChild(0).GetChild(0))
                {
                    
                    if (!dolly.GetChild(0).CompareTag("Blank") && !dolly.GetChild(0).CompareTag("Steel"))
                    {
                        //Debug.Log(dolly.GetChild(0).GetChild(0).name + " HELPER");
                        //Grab material from cart
                        spawnCartManager.colorHelper.Add(dolly.GetChild(0).GetChild(0).GetComponent<Renderer>().material.color);

                    }

                }
                spawnCartManager.spawnMatRandomColor = spawnCartManager.colorHelper[Random.Range(0, spawnCartManager.colorHelper.Count)];
                spawnCartManager.canvasIdentifier.color = spawnCartManager.spawnMatRandomColor;
            }
            
            else 
            {
                //Debug.Log(" YEEEE " + spawnChecks.Count);
                foreach (GameObject spawnCheck in spawnChecks)
                {
                    //Debug.Log("POINK " + spawnCheck.name);
                    if (spawnCheck/* && spawnCheck.transform.parent.parent.parent.GetSiblingIndex() == 0*/)
                    {
                        //Debug.Log("FIRST LEVEL");
                        //if (LevelManager.Instance.transform.GetChild(0).GetChild(0).CompareTag("Cart") || LevelManager.Instance.transform.GetChild(0).GetChild(0).CompareTag("Steel"))
                        if (spawnCheck.CompareTag("Cart") /*|| spawnCheck.CompareTag("Steel")*/)
                        {
                            //Debug.Log("HELPER ENABLED");
                            //First level Helper
                            if (true /*CheckDollyCount(LevelManager.Instance.transform.GetChild(0).GetChild(0)) >= 0*/)
                            {
                                //Debug.Log("4 dollys");
                                //foreach (Transform dolly in LevelManager.Instance.transform.GetChild(0).GetChild(0))
                                //{
                                    //Debug.Log("HELPER");
                                    if (spawnCheck.CompareTag("Cart"))
                                    {
                                        //**************************************
                                        spawnCartManager.colorHelper.Add(spawnCheck.GetComponent<Renderer>().material.color);
                                    }
                                    else
                                    {
                                        //spawnCartManager.colorHelper.Add(spawnCartManager.colorHelper[spawnCartManager.colorHelper.Count-1]);
                                    }


                                //}
                              
                            }
                           
                        }
                    }
                }

                //If colorHelper was populated - use it to spawn
                if(spawnCartManager.colorHelper.Count == 0)
                {
                    //Debug.Log("RANDOM COLOR");
                    spawnCartManager.spawnMatRandomColor = spawnCartManager.spawnMats[Random.Range(0, spawnCartManager.spawnMats.Length)].color;
                    spawnCartManager.canvasIdentifier.color = spawnCartManager.spawnMatRandomColor;
                }
                else
                {
                    spawnCartManager.spawnMatRandomColor = spawnCartManager.colorHelper[Random.Range(0, spawnCartManager.colorHelper.Count)];
                    spawnCartManager.canvasIdentifier.color = spawnCartManager.spawnMatRandomColor;
                }


            }
           


        }







        //spawn cart prefab, set random position
        GameObject tmpCart = Instantiate(spawnCartManager.spawnPrefab, spawnCartManager.transform.GetChild(0));
        spawnCartManager.spawnObject = tmpCart;
        //Set material to spawn
        tmpCart.transform.GetChild(0).GetComponent<Renderer>().material.color = spawnCartManager.spawnMatRandomColor;
        tmpCart.transform.GetComponent<CinemachineDollyCart>().m_Path = spawnCartManager.paths[2];
        //Set current for that cart
        tmpCart.transform.GetChild(0).GetComponent<CartModelContoller>().Current = 2;
        //set material number
        tmpCart.transform.GetChild(0).GetComponent<CartModelContoller>().spawnColor = spawnCartManager.spawnMatRandomColor;
        //Set track references for that cart
        tmpCart.transform.GetChild(0).GetComponent<CartModelContoller>().paths = spawnCartManager.paths;
        //set cart reference for manager
        spawnCartManager.carts[0] = tmpCart.transform.GetChild(0).GetComponent<CartModelContoller>();
        //Set parent of Level manager
        //tmpCart.transform.SetParent(LevelManager.Instance.transform);
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

            rb.constraints = RigidbodyConstraints.None;
            rb.constraints = RigidbodyConstraints.FreezePositionX;
            rb.constraints = RigidbodyConstraints.FreezePositionZ;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            rb.AddForce(0, -100f, 0);

        }


    }
    //Drop spawned cart
    public void DropSpawn(GameObject spawnCart)
    {
        if (spawnCart != null)
        {
            spawnCart.transform.GetChild(0).GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        }


    }
}



