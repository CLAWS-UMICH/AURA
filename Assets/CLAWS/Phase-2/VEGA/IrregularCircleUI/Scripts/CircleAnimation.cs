using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleAnimation : MonoBehaviour {

    public GameObject[] animObjects;
	public float speed = 50f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		foreach(GameObject go in animObjects)
        {
            Vector3 angle = go.transform.eulerAngles;

            angle.z += Time.deltaTime * speed;

            go.transform.eulerAngles = angle;
        }
	}
}
