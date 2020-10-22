using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WakeDetailUI : MonoBehaviour {
	GameObject c;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	public void WakeUI(GameObject obj)
	{
		c = obj;
		CardController card = obj.GetComponent<CardController> ();
		transform.Find ("DetailCardUI").gameObject.SetActive (true);
		transform.Find ("DetailCardUI").gameObject.GetComponent<DetailUI>().showUI(card);
	}
	public void CloseUI()
	{
		transform.Find ("DetailCardUI").gameObject.SetActive (false);
		Debug.Log ("Cancel");
	}
	public void callSummon()
	{
		BoardManager b = GameObject.FindGameObjectWithTag ("ChessBoard").GetComponent <BoardManager>();
		b.Summon (c);
		CloseUI ();
		c = null;
	}
}
