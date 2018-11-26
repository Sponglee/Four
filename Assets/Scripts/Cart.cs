using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cart : MonoBehaviour {

    public Transform[] targets;
    public float speed;

    public int current;

	
	
	void Update ()
    {
		if(transform.position != targets[current].position)
        {
            Vector3 pos = Vector3.MoveTowards(transform.position, targets[current].position, speed);
            GetComponent<Rigidbody>().MovePosition(pos);
        }
        else
        {
            current = (current + 1) % targets.Length;
        }
	}
}
