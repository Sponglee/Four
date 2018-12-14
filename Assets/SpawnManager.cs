using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : Singleton<SpawnManager> {

    public CartManager spawnCartManager;

    public CinemachineVirtualCamera vCam;
	// Use this for initialization
	void Start () {
        spawnCartManager = transform.GetChild(0).GetComponent<CartManager>();

        Spawn();
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetMouseButtonUp(0)
            && !LevelManager.Instance.RotationProgress && !LevelManager.Instance.SpawnInProgress /*&& spawnTimer <= 0*/)
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
        spawnCartManager.colorHelper.Clear();
        //set random spawn color\
        GameObject spawnCheck = GrabSpawnObj(transform);
        //if there's something with tag cart that ray hit
        if (spawnCheck != null && spawnCheck.CompareTag("Cart"))
        {
            Debug.Log(spawnCheck.tag);
            //if ray obj is first level
            if (spawnCheck.transform.parent.parent.parent.GetSiblingIndex() == 0)
            {
                //if it's full
                if (CheckDollyCount(LevelManager.Instance.transform.GetChild(0).GetChild(0)) == 4)
                {
                    //Pick a color from ones on top level
                    foreach (Transform dolly in LevelManager.Instance.transform.GetChild(0).GetChild(0))
                    {
                        Debug.Log(dolly.GetChild(0).GetComponent<Renderer>().material.color);
                        spawnCartManager.colorHelper.Add(dolly.GetChild(0).GetComponent<Renderer>().material.color);
                        spawnCartManager.spawnMatRandomColor = spawnCartManager.colorHelper[Random.Range(0, spawnCartManager.colorHelper.Count)];
                        spawnCartManager.canvasIdentifier.color = spawnCartManager.spawnMatRandomColor;
                    }
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
        }
        else if(spawnCheck != null && spawnCheck.CompareTag("Cart") || spawnCheck.CompareTag("Bottom"))
        {
            spawnCartManager.spawnMatRandomColor = spawnCartManager.spawnMats[Random.Range(0, spawnCartManager.spawnMats.Length)].color;
            spawnCartManager.canvasIdentifier.color = spawnCartManager.spawnMatRandomColor;
        }

        else
        {
            spawnCartManager.spawnMatRandomColor = spawnCartManager.spawnMats[Random.Range(0, spawnCartManager.spawnMats.Length)].color;
            spawnCartManager.canvasIdentifier.color = spawnCartManager.spawnMatRandomColor;
        }





        spawnCartManager.spawnedBool = true;
        //spawn cart prefab, set random position
        GameObject tmpCart = Instantiate(spawnCartManager.cartPrefabs[0], spawnCartManager.transform);
        spawnCartManager.spawnObject = tmpCart;
        //Set material to spawn
        tmpCart.transform.GetComponent<Renderer>().material.color = spawnCartManager.spawnMatRandomColor;
        //Set current for that cart
        tmpCart.transform.GetComponent<CinemachineDollyCart>().m_Path = spawnCartManager.paths[2];
        //Set dolly position
        tmpCart.transform.GetChild(0).GetComponent<CartModelContoller>().Current = 2;
        //set material number
        tmpCart.transform.GetChild(0).GetComponent<CartModelContoller>().spawnColor = spawnCartManager.spawnMatRandomColor;
        //Set track references for that cart
        tmpCart.transform.GetChild(0).GetComponent<CartModelContoller>().paths = spawnCartManager.paths;
        //set cart reference for manager
        spawnCartManager.carts[0] = tmpCart.transform.GetChild(0).GetComponent<CartModelContoller>();
        //Set parent of Level manager
        //tmpCart.transform.SetParent(LevelManager.Instance.transform);
        //Set camera target
        vCam.m_LookAt = tmpCart.transform;
        vCam.m_Follow = tmpCart.transform;
    }

    //Get reference to object hit by ray with tag
    private GameObject GrabSpawnObj(Transform origin, string obj="")
    {
        RaycastHit hit;
        Vector3 dir = origin.position + new Vector3(0, -100f, -3f);

        Debug.DrawLine(origin.position  + new Vector3(0,0,-3f), dir, Color.red, 10f);

        if (Physics.Raycast(origin.position + new Vector3(0, 0, -3f), -Vector3.up, out hit))
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
    public void DropSpawn(GameObject spawnCart)
    {
        if (spawnCart != null)
        {
            spawnCart.transform.GetChild(0).GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        }


    }
}
