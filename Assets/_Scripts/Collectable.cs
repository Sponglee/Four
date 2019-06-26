﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    Rigidbody collectableRb;

    public bool ChestCollectable = false;
    public float chestDuration = 1f;
    private float chestTimer = 0f;

    [SerializeField]
    private int powerCol = -2;
    // -3 - Key PowerCol, -2 - gem PowerUp -1 - gem collectable
    public int PowerCol
    {
        get
        {
            return powerCol;
        }

        set
        {
            powerCol = value;
        }
    }

    private void Awake()
    {
        collectableRb = transform.GetComponent<Rigidbody>();

        
        //RandomizeCollectable();

        chestTimer = 0;
    }
    //private void Start()
    //{
    //    collectableRb = transform.GetComponent<Rigidbody>();

    //    RandomizeCollectable();

    //    chestTimer = 0;
    //    //if(ChestCollectable)
    //    //{
    //    //    transform.localPosition += new Vector3( Random.Range(-1f,1f),Random.Range(-1f,1f),0);
    //    //}
    //}

    private void Update()
    {
        if(!ChestCollectable)
        {
            transform.GetChild(0).Rotate(Vector3.forward, 3f);
        }
        else
        {
            chestTimer += Time.deltaTime;
            if (chestTimer > chestDuration)
            {
                transform.parent.GetChild(1).gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (PowerCol == -1 && other.CompareTag("Magnet"))
        {
            transform.SetParent(SpawnManager.Instance.transform.GetChild(0));
            StartCoroutine(MagnetCollectable());
        }
    }



    private void OnEnable()
    {
        if(PowerCol == -3)
        {
            Debug.Log("ONENABLE KEY");
            if (GameManager.Instance.KeyCount >= 3)
            {
                Debug.Log("DISABLE");
                transform.parent.gameObject.SetActive(false);
            }
        }
       
    }


    private IEnumerator MagnetCollectable()
    {
        Transform ballTrans = BallController.Instance.transform;

        collectableRb.constraints = RigidbodyConstraints.None;
        while (true)
        {
            Debug.DrawLine(collectableRb.transform.position, ballTrans.position, Color.blue);
            collectableRb.velocity = (ballTrans.position + new Vector3(0, Random.Range(0f,10f), 0f) - collectableRb.transform.position) * 5f;
            //Debug.Log(collectableRb.velocity);
            yield return null;
        }
    }

    public void RandomizeCollectable()
    {

        if(PowerCol >= 0)
        {
            transform.GetChild(0).GetChild(0).GetChild(PowerCol).gameObject.SetActive(false);
        }




        if (PowerCol > -3 && PowerCol < 0)
        {
            int PowerColRand = Random.Range(0, 100);

            Debug.Log(">rand " + PowerColRand);


            //Shield
            if (PowerColRand >= 0 && PowerColRand < 20)
            {
                PowerCol = 0;
            }
            //Magnet
            else if (PowerColRand >= 20 && PowerColRand < 30)
            {
                PowerCol = 1;
            }
            //Powered Up
            else if (PowerColRand >= 30 && PowerColRand < 50)
            {
                PowerCol = 2;
            }
            //Gems med
            else if (PowerColRand >= 50 && PowerColRand < 65)
            {
                PowerCol = 4;
            }
            //Gems high
            else if (PowerColRand >= 65 && PowerColRand < 67)
            {
                PowerCol = 5;
            }
            //Gems min
            else if (PowerColRand >= 67 && PowerColRand < 100)
            {
                PowerCol = 3;
            }

            Debug.Log(PowerCol);
            transform.GetChild(0).GetChild(0).GetChild(PowerCol).gameObject.SetActive(true);
        }
    }
}