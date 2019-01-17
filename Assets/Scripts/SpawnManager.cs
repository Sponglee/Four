using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : Singleton<SpawnManager>
{

    public CartManager spawnCartManager;
    // Use this for initialization
    void Start()
    {
        spawnCartManager = transform.GetChild(0).GetComponent<CartManager>();

        Spawn();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonUp(0)
            && !LevelManager.Instance.RotationProgress 
                && !LevelManager.Instance.SpawnInProgress 
                    && !LevelManager.Instance.LevelMoveProgress /*&& spawnTimer <= 0*/)
        {
            GameObject tmpRayCart = GrabSpawnObj(transform, "Cart");
            if (tmpRayCart != null && tmpRayCart.GetComponent<Renderer>().material.color != spawnCartManager.spawnMatRandomColor
                && tmpRayCart.transform.parent.parent.childCount >= 4 && tmpRayCart.transform.parent.parent.parent.GetSiblingIndex() == 0)
            {
                //Debug.Log("NOT SAME ");
            }
            else
            {
                DropSpawn(spawnCartManager.spawnObject);
                LevelManager.Instance.SpawnInProgress = true;
                spawnCartManager.spawnedBool = false;
            }






            ////Reset spawn cooldown
            //spawnTimer = spawnDuration;
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
        GameObject spawnCheck = GrabSpawnObj(transform, "Cart");
        if (spawnCheck != null)
        {
            //GAME OVER CHECK
            if (LevelManager.Instance.transform.GetChild(0).GetChild(0).CompareTag("Bottom"))
            {
                Debug.Log("GAMEOVER");
                yield break;
            }
            //Debug.Log(spawnCheck.tag);
            else if (spawnCheck && spawnCheck.transform.parent.parent.parent.GetSiblingIndex() == 0)
            {
                if (LevelManager.Instance.transform.GetChild(0).GetChild(0).CompareTag("Cart") || LevelManager.Instance.transform.GetChild(0).GetChild(0).CompareTag("Steel"))
                {
                    //First level Helper
                    if (CheckDollyCount(LevelManager.Instance.transform.GetChild(0).GetChild(0)) == 4)
                    {
                        foreach (Transform dolly in LevelManager.Instance.transform.GetChild(0).GetChild(0))
                        {
                            Debug.Log("HELPER");
                            if (!dolly.CompareTag("Steel"))
                            {
                                spawnCartManager.colorHelper.Add(dolly.GetChild(0).GetComponent<Renderer>().material.color);
                            }


                        }
                        spawnCartManager.spawnMatRandomColor = spawnCartManager.colorHelper[Random.Range(0, spawnCartManager.colorHelper.Count)];
                        spawnCartManager.canvasIdentifier.color = spawnCartManager.spawnMatRandomColor;
                    }
                    else
                    {
                        spawnCartManager.spawnMatRandomColor = spawnCartManager.spawnMats[Random.Range(0, spawnCartManager.spawnMats.Length)].color;
                        spawnCartManager.canvasIdentifier.color = spawnCartManager.spawnMatRandomColor;

                    }
                }



            }
            //Last level helper
            else if (LevelManager.Instance.transform.GetChild(1).GetChild(0).CompareTag("Bottom"))
            {
                foreach (Transform dolly in LevelManager.Instance.transform.GetChild(0).GetChild(0))
                {
                    Debug.Log(dolly.gameObject.name + " HELPER");
                    if (!dolly.CompareTag("Blank") && !dolly.CompareTag("Steel"))
                    {
                        spawnCartManager.colorHelper.Add(dolly.GetChild(0).GetComponent<Renderer>().material.color);

                    }

                }
                spawnCartManager.spawnMatRandomColor = spawnCartManager.colorHelper[Random.Range(0, spawnCartManager.colorHelper.Count)];
                spawnCartManager.canvasIdentifier.color = spawnCartManager.spawnMatRandomColor;
            }
            else
            {
                spawnCartManager.spawnMatRandomColor = spawnCartManager.spawnMats[Random.Range(0, spawnCartManager.spawnMats.Length)].color;
                spawnCartManager.canvasIdentifier.color = spawnCartManager.spawnMatRandomColor;
            }
        }
        else
        {
            spawnCartManager.spawnMatRandomColor = spawnCartManager.spawnMats[Random.Range(0, spawnCartManager.spawnMats.Length)].color;
            spawnCartManager.canvasIdentifier.color = spawnCartManager.spawnMatRandomColor;
        }





       
        //spawn cart prefab, set random position
        GameObject tmpCart = Instantiate(spawnCartManager.cartPrefabs[0], spawnCartManager.transform);
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


    //Get reference to object hit by ray with tag
    private GameObject GrabSpawnObj(Transform origin, string obj)
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
    //Drop spawned cart
    public void DropSpawn(GameObject spawnCart)
    {
        if (spawnCart != null)
        {
            spawnCart.transform.GetChild(0).GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        }


    }
}



