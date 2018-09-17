﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Camera.main != null)
        {
            //transform.LookAt(Camera.main.transform);
            transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);

        }
    }
}