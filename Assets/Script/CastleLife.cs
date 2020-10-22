using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CastleLife : MonoBehaviour {
	const int lifePoint = 3;
	public int CastleIndex;
	public int life = lifePoint;
	public GameObject bang;
	public GameObject ruin;
	public GameObject[] Heart;
	private GameObject ChessBoard;
	public GameObject Campfire;
	public GameObject WinLoseUI;
	public GameObject WinLoseText;
	public GameObject SettingButtonController;
	public float targetTime = 1.0f ;
	private float nextUpdate = 0.0f;
	private bool GameOver = false;
	// Use this for initialization
	void Start () {
		ChessBoard = GameObject.Find("ChessBoard");
		WinLoseUI.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	public void getDamage(int EnemyIndex){
		if(CastleIndex == EnemyIndex)
		{
			Debug.Log("Damage!!!");
			//Instantiate(bang, transform.position, Quaternion.identity);
			damageHeart();
			life--;
		}
		if(life == 0)
		{
			Debug.Log("castle ruin");
			GameOver = true;
			ChessBoard.GetComponent<BoardManager> ().changeBattlePhase ((int)battlePhase.GAMEOVER);
			WinLoseUI.SetActive(true);
			if(CastleIndex == 1) 
				WinLoseText.GetComponent<Text>().text = "YOU WIN !";
			else
				WinLoseText.GetComponent<Text>().text = "YOU LOSE !";				
			WinLoseUI.GetComponent<EasyTween>().OpenCloseObjectAnimation();
			SettingButtonController.GetComponent<SettingButtConstroller>().open = true;
			SettingButtonController.GetComponent<SettingButtConstroller>().SettingMenuUI.GetComponent<EasyTween>().OpenCloseObjectAnimation();
		}		
	}
	private void damageHeart()
	{
		if(life>0)
		{
			GetComponent<AudioSource>().Play();
			int index = life - 1;
			onFire ();
			GameObject now = Heart [index];
			Destroy (now, 0.5f);
		}
	}
	private void onFire()
	{
		Campfire.SetActive (true);
	}

}
