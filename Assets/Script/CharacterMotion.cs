using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CharacterMotion : MonoBehaviour {
    private bool summon;
    private int curDir; //  global direction
    private bool moving;
    private bool startMove;
	private bool startBattle;
    private int dir;
    private CardController cardInfo;
    private int currentHp;
    private int currentAttack;
    bool arrived;
    Vector3 destination = new Vector3();
	bool playBattleAnimation;
	bool moveBack;
	bool EnemyAnimation;
    private int playerIndex; // different player move different direction related to Character
	int enemyMonsIndexInList ;
    private BoardManager board;
    private TextMesh HpDisplay;
    private TextMesh AttackDisplay;
    private GameObject info;
	private AudioSource[] audio;// 0 :damage 1: walk
    // Use this for initialization
    void Start () {
		audio = new AudioSource[2];
		audio [0] = GameObject.FindGameObjectWithTag ("audio").transform.Find ("Hit").GetComponent<AudioSource> ();
		audio[1] = GameObject.FindGameObjectWithTag ("audio").transform.Find ("Walk").GetComponent<AudioSource> ();
        summon = false;
        cardInfo = gameObject.GetComponent<CardController>();
        info = transform.Find("Info").gameObject;
        Debug.Log(info.name);
        HpDisplay = transform.Find("Info/hp").GetComponent<TextMesh>();
        AttackDisplay = transform.Find("Info/attack").GetComponent<TextMesh>();
        Debug.Log("hp: " + currentHp + " , att : " + currentAttack);
        enemyMonsIndexInList = -1;
        board = GameObject.FindGameObjectWithTag("ChessBoard").GetComponent<BoardManager>();
        startMove = false;
		startBattle = false;
        if (Vector3.Dot(transform.forward.normalized, Vector3.forward.normalized) == 1) //up
        {
            curDir = 0;
        }
        else if (Vector3.Dot(transform.forward.normalized, Vector3.forward.normalized) == -1) // down
        {
            curDir = 1;
        }
        else if (Vector3.Dot(transform.forward.normalized, Vector3.right.normalized) == 1) //right
        {
            curDir = 3;
        }
        else if (Vector3.Dot(transform.forward.normalized, Vector3.right.normalized) == -1)  //left
        {
            curDir = 2;
        }
	}

    // Update is called once per frame
    private void FixedUpdate()
    {
        arrived = false;
        // bool startMove
        movingState();
        // bool startBattle
        battleState();  
    }
    public void summonMons()
    {
        if (summon) return;
        summon = true;
        info.SetActive(true);
        currentAttack = cardInfo.getAttackData();
        currentHp = cardInfo.getHpData();
    }
    void Update () {
        info.transform.rotation = Quaternion.identity;
        playerIndex = board.playerIndex;

        AttackDisplay.text = currentAttack.ToString();
        HpDisplay.text = currentHp.ToString();

        //full hp->green , damaged ->red
        if (currentHp == cardInfo.getHpData())
        {
            HpDisplay.color = new Color(35f/255f, 185f/255f, 50f/255f, 255f/255f);
        }
        else
            HpDisplay.color = new Color(240f / 255f, 30f / 255f, 30f / 255f, 255f / 255f);
     //   Debug.Log(HpDisplay.color);
        checkCondition();
    }
	public bool TurnDirection(int dir)
	{
        Debug.Log("turn");
		if (dir == 0) {
			transform.rotation = Quaternion.LookRotation (Vector3.forward);
			curDir = 0;
		} else if (dir == 1) {
			transform.rotation = Quaternion.LookRotation (Vector3.forward * -1);
			curDir = 1;
		} else if (dir == 2) {
			transform.rotation = Quaternion.LookRotation (Vector3.right * -1);
			curDir = 2;
		} else if (dir == 3) {
			transform.rotation = Quaternion.LookRotation (Vector3.right);
			curDir = 3;
		}
        return true;
	}
    //return current hp
    public int damageHp(int damage)
    {
        if (currentHp < damage)
            currentHp = 0;
        else
            currentHp -= damage;

        return currentHp;
    }
    // if hp is 0, destroy now , need to add more animation later
    public void checkCondition()
    {
        if(currentHp<=0 && summon)
        {
            board.placedMonsters[(playerIndex + 1) % 2].Remove(gameObject);
            Destroy(gameObject, 1.4f);
        }
    }
    bool normalAttack = true;
    //monster's special effect or normal attack to damage enemy, doen't include castle
    //only normalAttack here. may add addition function later
    public void damagePhase(GameObject enemyObj)
    {
        CharacterMotion enemy = enemyObj.GetComponent<CharacterMotion>();
        if (normalAttack)
            enemy.damageHp(currentAttack);
    }
	public void BattleAnimation(int dir, bool AttackEnemy)
	{
        // if already in battle, return
		if (moveBack || playBattleAnimation || startBattle)
			return;
        //reset the bool we need in battle 
		playBattleAnimation = false;
		moveBack = false;
		startBattle = true;
		moving = false;
		EnemyAnimation = AttackEnemy;
		this.curDir = dir;
		enemyMonsIndexInList = board.findEnemy (board.selectMoveCharacter);
	}

    public void MoveCharacter(int localDir)  // input dir need to be change global -> localdir input by arrow keys
    {
        Debug.Log("move character");
        if(!startMove)
        {
        	startMove = true;
            moving = false;
            dir = localDir;
        }

        /*Vector3 destination = new Vector3();
            if(dir  == curDir) // don't need to rotate
            {
                 if (!moving)
                 {
                    destination = transform.position + transform.forward;
                    moving = true;
                    Debug.Log(transform.position + " && " + destination);
                    Debug.Log("curDir" +curDir);
                 }
                 else if(destination != null)
                 {
                    if( !Vector3.Equals(  transform.position , destination))
                    {
                        Vector3 to = Vector3.MoveTowards(transform.position, destination, Time.deltaTime);
                        transform.Translate(to);
                         Debug.Log(to);
                    }
                    else
                    {
                        moving = false;
                        destination = transform.position;
                    }
                 }
           
            }
            else  // need to rotate
            {
                if (dir == 0)
                {
                    transform.forward = Vector3.RotateTowards(this.transform.forward, Vector3.forward, 300, 0.5f);
                    curDir = 0;
                }
                else if (dir == 1)
                {
                    transform.forward = Vector3.RotateTowards(this.transform.forward, Vector3.forward * -1, 300, 0.5f);
                    curDir = 1;
                }
                else if (dir == 2)
                {
                    transform.forward = Vector3.RotateTowards(this.transform.forward, Vector3.right * -1, 300, 0.5f);
                    curDir = 2;
                }
                else if (dir == 3)
                {
                    transform.forward = Vector3.RotateTowards(this.transform.forward, Vector3.right, 300, 0.5f);
                    curDir = 3;
                }
            }
        
        GetComponent<Animator>().Play("Walk");*/
    }
	public int ReturnDir()
	{
		return curDir;
	}
    public void setPlayerIndex(int id)
    {
        playerIndex = id;
    }
    Vector3 beforeMove;
    private void battleState()
    {
        if (startBattle)
        {
            // first move 
            //Debug.Log(moveBack+" && " + playBattleAnimation);
            if (!moveBack && !playBattleAnimation)
            {
                if (true)
                { // don't need to rotate

                    if (!moving)
                    { // move to half way between character
                        Debug.Log("want to move");
                        beforeMove = transform.position;
                        destination = transform.position + transform.forward.normalized * 0.5f;
                        moving = true;
                    }
                    else
                    {
                        if (Vector3.Distance(transform.position, destination) > 0.01f)
                        {
                            Vector3 to = Vector3.MoveTowards(transform.position, destination, 5 * Time.deltaTime);
                            transform.position = to;
                        }
                        else
                        {
                            arrived = true;
                            moving = false;
                            playBattleAnimation = true;
                            destination = transform.position;
                        }
                    }
                } /*else if (!moving) {  // need to rotate
					Debug.Log ("want to rotate");
					if (dir == 0) {
						transform.rotation = Quaternion.LookRotation (Vector3.forward);
						curDir = 0;
					} else if (dir == 1) {
						transform.rotation = Quaternion.LookRotation (Vector3.forward * -1);
						curDir = 1;
					} else if (dir == 2) {
						transform.rotation = Quaternion.LookRotation (Vector3.right * -1);
						curDir = 2;
					} else if (dir == 3) {
						transform.rotation = Quaternion.LookRotation (Vector3.right);
						curDir = 3;
					}
				}*/
                GetComponent<Animator>().Play("Walk");
            }
            else if (playBattleAnimation)
            {
                Animator enemy;
                if (EnemyAnimation)
                {
                    enemy = board.placedMonsters[(board.playerIndex + 1) % 2][enemyMonsIndexInList].GetComponent<Animator>();
                    damagePhase(board.placedMonsters[(board.playerIndex + 1) % 2][enemyMonsIndexInList]);
                    GameObject e = board.placedMonsters[(board.playerIndex + 1) % 2][enemyMonsIndexInList];
                    board.monsPosTable[(int)e.transform.position.x, (int)e.transform.position.z] = false;
                    enemy.Play("Damaged");
                }
				audio [0].Play ();
                GetComponent<Animator>().Play("Attack");
                playBattleAnimation = false;
                moveBack = true;
                moving = false;
            }
            else if (moveBack)
            {
                if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("End"))
                {
                    if (!moving)
                    {
                        destination = beforeMove;
                        moving = true;
                    }
                    else
                    {
                        if (Vector3.Distance(transform.position, destination) > 0.01f)
                        {
                            Vector3 to = Vector3.MoveTowards(transform.position, destination, 5 * Time.deltaTime);
                            transform.position = to;
                        }
                        else
                        {
                            arrived = true;
                            moving = false;
                            destination = transform.position;
                            moveBack = false;
                            startBattle = false;
                            // end battle animation 
                        }
                    }
                }
                enemyMonsIndexInList = -1;
              
                board.BattleEnd = true;
            }
        }
    }
    private void movingState()
    {
        if (startMove)
        {
            if (dir == curDir)
            { // don't need to rotate
                if (!moving)
                {
                    Debug.Log("want to move");

                    destination = transform.position + transform.forward.normalized;
                    moving = true;
					audio [1].Play ();
                }
                else
                {
                    if (Vector3.Distance(transform.position, destination) > 0.01f)
                    {
                        board.monsPosTable[(int)transform.position.x, (int)transform.position.z] = false;
                        Vector3 to = Vector3.MoveTowards(transform.position, destination, 10 * Time.deltaTime);
                        transform.position = to;
                    }
                    else
                    {
                        board.monsPosTable[(int)transform.position.x, (int)transform.position.z] = true;
                        arrived = true;
                        board.steps[playerIndex]--;
                        if (playerIndex == 1 && board.MoveMonsEnd)
                        {
                            AIcontroller ai = GameObject.FindGameObjectWithTag("AI").GetComponent<AIcontroller>();
                            ai.RotateBattleDir();
                        }
                        startMove = false;
                        destination = transform.position;
                    }
                }

            }
            else if (!moving)
            {  // need to rotate
                Debug.Log("want to rotate");
                if (dir == 0)
                {
                    transform.rotation = Quaternion.LookRotation(Vector3.forward);
                    curDir = 0;
                }
                else if (dir == 1)
                {
                    transform.rotation = Quaternion.LookRotation(Vector3.forward * -1);
                    curDir = 1;
                }
                else if (dir == 2)
                {
                    transform.rotation = Quaternion.LookRotation(Vector3.right * -1);
                    curDir = 2;
                }
                else if (dir == 3)
                {
                    transform.rotation = Quaternion.LookRotation(Vector3.right);
                    curDir = 3;
                }
            }
            GetComponent<Animator>().Play("Walk");
        }
    }
}
