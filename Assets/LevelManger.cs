using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManger : MonoBehaviour {


    public GameObject levelPrefab;
    public int spawnOffset = 0;
	
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonDown(2))
        {
            GameObject tmpSpawn = Instantiate(levelPrefab, transform);
            tmpSpawn.transform.position += new Vector3(0,-spawnOffset,0);
            spawnOffset += 5;
        }
	}
}
