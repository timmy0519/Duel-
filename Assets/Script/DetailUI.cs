using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class DetailUI : MonoBehaviour {
	public Sprite[] blocks;
	Image monster;
	Image block;
	TextMeshProUGUI title;
	TextMeshProUGUI description;
	TextMeshProUGUI attackData;
	TextMeshProUGUI hpData;
//	TextMeshProUGUI defenseData;
	GameObject SummonButton;
	GameObject Board;
	int playerIndex;
	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

	public void showUI(CardController card)
	{

		Board = GameObject.FindGameObjectWithTag ("ChessBoard");
		playerIndex = Board.GetComponent<BoardManager> ().playerIndex;
		SummonButton = GameObject.Find ("SummonButton");
		monster = transform.Find ("Card/MonsterImage").GetComponent<Image> ();
		block = transform.Find ("Card/BlockImage").GetComponent<Image> ();
		description = transform.Find ("Card/CardInfo/Card Description").GetComponent<TextMeshProUGUI> ();
		attackData = transform.Find ("Card/CardInfo/Attackdata").GetComponent<TextMeshProUGUI> ();
		hpData = transform.Find ("Card/CardInfo/Hpdata").GetComponent<TextMeshProUGUI> ();
		//defenseData = transform.Find ("Card/CardInfo/Defensedata").GetComponent<TextMeshProUGUI> ();
		title = transform.Find ("Card/CardTitle").GetComponent<TextMeshProUGUI> ();


		Debug.Log (monster.name);
		monster.sprite = card.getSprite();
		block.sprite = blocks [card.getID()];
		attackData.SetText (card.getAttackData ().ToString());
		hpData.SetText ((string)card.getHpData ().ToString());
		//defenseData.SetText( (string)card.getDefenseData ().ToString());
		description.SetText( (string)card.getDescription ().Clone());
		title.SetText( card.cardName);
		/*if (playerIndex == 1) {
			SummonButton.SetActive (false);
		}else
			SummonButton.SetActive (true);*/
	}
}
