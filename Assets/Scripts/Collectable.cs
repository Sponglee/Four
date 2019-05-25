using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    Rigidbody collectableRb;
    


    private void Start()
    {
        collectableRb = transform.GetComponent<Rigidbody>();
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Magnet"))
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
            collectableRb.velocity = (ballTrans.position - collectableRb.transform.position) * 5f;
            Debug.Log(collectableRb.velocity);
            yield return null;
        }
    }

}
