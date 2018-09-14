using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuMoveClouds : MonoBehaviour {
    float t;
    Vector2 startPos;
    Vector2 destinyPos;

	// Use this for initialization
	void Start () {
        startPos = transform.position;
        destinyPos = new Vector2(2333, transform.position.y);
	}
	
	// Update is called once per frame
	void Update () {
        t += Time.deltaTime / 120;
        transform.position = Vector2.Lerp(startPos, destinyPos, t);
	}
}
