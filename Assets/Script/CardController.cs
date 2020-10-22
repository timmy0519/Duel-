using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardController : MonoBehaviour {
    // name, step, image, description, single block, level 
    public enum Blocks { stick, square, bat};
    public enum CardName { Knight,Slime,FireBall};
    private int id;
    public string cardName;
    public Sprite image;
    private int step; 
    private string description;
    private Blocks block; // store index from 0
    private int level;
	private int hp;
	private int attack;
	private int defense;
    public int getID()
    {
        return id;
    }
    public int getStep()
    {
        return step;
    }
    public Blocks getBlock()
    {
        return block;
    }
    public string getDescription()
    {
        return description;
    }
    public int getLevel()
    {
        return level;
    }
    public Sprite getSprite()
    {
        return image;
    }


    // Use this for initialization
    void Start () {
        id = -1; // not initiate
		if (id == -1)
		{
			switch (cardName.ToLower())
			{
			case "knight":
				step = 2;
				block = Blocks.stick;
				level = 3;
				description = " Knight_Ting.";
				id = 0;
				hp = 5;
				attack = 1;
				defense = 3;
				break;
			case "slime":
				step = 3;
				block = Blocks.square;
				level = 3;
				description = " Slime line lime nine.";
				id = 1;
				hp = 2;
				attack = 2;
				defense = 2;
				break;
			case "fireball":
				step = 4;
				block = Blocks.bat;
				level = 3;
				description = "Fire BALL!!";
				id = 2;
				hp = 3;
				attack = 5;
				defense = 1;
				break;
			default:
				id = -1;
				break;
			}
			if (image == null)
				Debug.Log("miss image");
			if (cardName == null)
				Debug.Log("miss name");
		}
	}
	public int getHpData()
	{
		return hp;
	}
	public int getAttackData()
	{
		return attack;
	}
	public int getDefenseData()
	{
		return defense;
	}

    // Update is called once per frame
    void Update()
    {
       
    }
       

}
