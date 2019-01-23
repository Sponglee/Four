using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cart : MonoBehaviour {

    public Transform[] targets;
    public float speed;

    public int current;

    //RUNNER
    private void Start()
    {
        current = transform.parent.parent.GetSiblingIndex();
    }

    //void Update ()
    //   {
    //	if(transform.position != targets[current].position)
    //       {
    //           Vector3 pos = Vector3.MoveTowards(transform.position, targets[current].position, speed);
    //           GetComponent<Rigidbody>().MovePosition(pos);
    //       }
    //       else
    //       {
    //           current = (current + 1) % targets.Length;
    //       }
    //}


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Spawn"))
        {

            LevelManager.Instance.Level = current;

        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boundary") && LevelManager.Instance.Level == current && current != 0)
        {
            Destroy(gameObject);
        }
    }
}
