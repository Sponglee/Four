using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    Rigidbody collectableRb;


    [SerializeField]
    private int powerCol = -1;
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

    private void Start()
    {
        collectableRb = transform.GetComponent<Rigidbody>();

        if(PowerCol != -1)
        {
           int PowerColRand = Random.Range(0,100);

            //Shield
            if (PowerColRand > 10 && PowerColRand < 30)
            {
                PowerCol = 0;
            }
            //Magnet
            else if (PowerColRand>=30 && PowerColRand<40)
            {
                PowerCol = 1;
            }
            //Powered Up
            else if(PowerColRand > 30 && PowerColRand < 50)
            {
                PowerCol = 2;
            }


            transform.GetChild(0).GetChild(0).GetChild(PowerCol).gameObject.SetActive(true);
        }

    }

    private void Update()
    {
        if(PowerCol!= -1)
        {
            transform.GetChild(0).Rotate(Vector3.forward, 3f);
        }
        else
        {
            transform.GetChild(0).Rotate(Vector3.forward, 1.5f);
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

}
