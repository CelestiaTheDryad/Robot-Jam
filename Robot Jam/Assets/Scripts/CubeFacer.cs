using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeFacer : MonoBehaviour {

	// Use this for initialization
	void Start () {
        transform.LookAt(new Vector3(0, transform.position.y, 0));
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
