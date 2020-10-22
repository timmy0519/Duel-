using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopTextController : MonoBehaviour {
    Transform[] child;
    List<GameObject> selectAnimation;
    

    //assume that if we have a set of animation need to activate
    //FIFO to play the animations


	// Use this for initialization
	void Start () {
        selectAnimation = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {
        if(selectAnimation.Count > 0)
        {
            if(selectAnimation[0].activeSelf == false)  // end of animation
            {
                selectAnimation.RemoveAt(0);
                if(selectAnimation.Count>0)
                {
                    selectAnimation[0].SetActive(true);
                    selectAnimation[0].GetComponent<TextAnimation>().Play();
                }

            }
        }
	}
    public bool Activate(string s)
    {
        foreach(Transform child in this.transform)
        {
            if(child.name.Equals(s))
            {
                if (selectAnimation.Count == 0) //play first animation
                {
                    child.gameObject.SetActive(true);
                    child.gameObject.GetComponent<TextAnimation>().Play();
                }
                    
                selectAnimation.Add(child.gameObject);           
            }
            
        }
        return false;
    }
}
