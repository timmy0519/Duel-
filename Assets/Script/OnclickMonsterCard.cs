 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnclickMonsterCard : MonoBehaviour {


	// Use this for initialization
	public void ReturnMonsterPrefab(GameObject obj) {
		GameObject ChessBoard = gameObject.transform.GetChild(0).gameObject;
		Debug.Log("hivmdkslvmdslkvmdslv") ;
        int Monster_ID = obj.GetComponent<CardController>().getID();
		switch (Monster_ID)
		{
			case 0: 
				ChessBoard.GetComponent<BoardManager>().GetMonsterName("Cha_Knight");
				break;
			case 1:
				ChessBoard.GetComponent<BoardManager>().GetMonsterName("Cha_Slime");
				break;
			case 2:
				ChessBoard.GetComponent<BoardManager>().GetMonsterName("StoneMonster");
				break;
			default:
				break;
		}
	}
	
}
