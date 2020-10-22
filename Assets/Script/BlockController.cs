using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockController : MonoBehaviour {

    Material[] mats;
	// Use this for initialization
	void Start () {
        //mats = GetComponent<Renderer>().materials;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    void changeColor()
    {
        foreach(Transform child in transform)
        {
            mats = child.GetComponent<Renderer>().materials;
             
        }
    }
}
