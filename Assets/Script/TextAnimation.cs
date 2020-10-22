using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextAnimation : MonoBehaviour {
    Animator a;
    bool reset;
    public bool finish;
	// Use this for initialization
	void Start () {
		a = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        if (reset)
        {
            a.Play("Play");
            reset = false;
        }
        else if(a.GetCurrentAnimatorStateInfo(0).IsName("End"))
        {
            this.gameObject.SetActive(false);
            finish = true;
        }
	}
    public bool Play()
    {
        reset = true;
        finish = false;
        return reset;
    }
}
