using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UITween;
enum battlePhase { IDLE, DRAW, PLACE_BLOCK, PLACE_MONSTER,DESTROY_CHAR, CAMERA_MOVE_SUMMON, MOVE_MONSTER, CHOOSE_MOVE_MONSTER, BATTLE, CHANGE_PLAYER , GAMEOVER }
public class BoardManager : MonoBehaviour
{
	private AudioSource summon;
	private float nextmoveTime = 0;
    private float moveTime = 0.2f;
    bool destroyEnd;
    public bool placeBlockFind;
    private AIcontroller AI;
    public bool validBoarder;
    public bool besideBlocks;
    int enemyPlayerIndex;
    private Text RoundDisplay;
    public GameObject selectMoveCharacter;
    public GameObject[] Castle;
    public Material redMaterial;
    public Material[] playerBlockMat;
    public Camera MonsterCamera;
    public Camera MainCamera;
    private Material originMat;
    private int timerForWrongPlacement = -1;
    private int[] BoarderForPlacingBlocks; // index 0 for place from 0, index 1 for place from size-1
    private const float TILE_SIZE = 1.0f;
    private const float TILE_offset = 0.5f;  // to the center
    public int BOARD_SIZE = 12;
    public int select_X = -1; // not be select -1
    public int select_Y = -1;
    public bool[,] chessTable;
    public bool[,] monsPosTable;
    public string sel_monster;
    List<LineDrawer> LinesX;
    List<LineDrawer> LinesZ;
    private GameObject LinesParent;
    private GameObject MonsterOnCamera;
    private GameObject CardUIController;
    private battlePhase curPhase;
    public Material[] BlueMons;
    public Material[] RedMons;
    //  private int phase = 0; // phase 0: 0, phase 1: 90,  phase 2: 180,  phase 3: 270,   (y rotation)
    //shape
    private GameObject n_PlaceObject;
    public bool selectShape = false;
    public int selectIndex = -1;
    public GameObject[] Floors;
    public int chooseMoveMonsIndex;
    // monster
    private bool selectMonster = false;
    private int selectMonsIndex = -1;
    public GameObject[] Monsters;
    private List<CardController> allCards;
    //placedObject List
    public List<GameObject>[] placedBlocks;
    public List<GameObject>[] placedMonsters;
    bool checkb = false;
    bool firstPlace = false;
    private bool CardUIControllerOpen = false;
    public bool drawCard;
    public bool placeBlockEnd;
    public bool placeMonsEnd;
    public bool ChooseMoveMonsEnd;
    public bool MoveMonsEnd;
    public bool BattleEnd;
    public bool cameraMoveSummonEnd;
    public bool AskToDestroy;
    public bool WantToDestroy = false;
    public bool NotToDestroy = false;
    public bool DecideToDestroy = false;
    PopTextController PopTextList;
    // Which player , here assume that only has 0,1 two player  育榮看這/////////////////////////////////////////////////////////
    public int playerIndex = 0;
    public int[] steps;
    public int Round; // 回合
    public Text StepDisplay;
    public GameObject selMonsFlag;
    public int[] monsLimit;
	List<KeyValuePair<int, int>> movingPath;
    // Use this for initialization
    public float getTileOffset()
    {
        return TILE_offset;
    }
    // Use this for initialization
    public int getBattlePhase()
    {
        return (int)curPhase;
    }
    public int getBorder(int player)
    {
        return BoarderForPlacingBlocks[player];
    }
    void Start()
    {
		summon = GameObject.FindGameObjectWithTag("audio").transform.Find("Summon").GetComponent<AudioSource> ();
        monsLimit = new int[2];
        monsLimit[0] = 1;
        monsLimit[1] = 1;
        AI = GameObject.FindGameObjectWithTag("AI").GetComponent<AIcontroller>(); ;
        selMonsFlag = GameObject.FindGameObjectWithTag("ChooseFlag");
        RoundDisplay = GameObject.FindGameObjectWithTag("RoundText").GetComponent<Text>();
        StepDisplay = GameObject.FindGameObjectWithTag("Steps").GetComponent<Text>();
        CardUIController = GameObject.FindGameObjectWithTag("CardUIControll");
        CardUIController.GetComponent<EasyTween>().OpenCloseObjectAnimation();
        Round = 1;
        //SpawnLine();
        PopTextList = GameObject.FindGameObjectWithTag("PopTextControll").GetComponent<PopTextController>();
        placedBlocks = new List<GameObject>[2];
        placedBlocks[0] = new List<GameObject>();
        placedBlocks[1] = new List<GameObject>();
        placedMonsters = new List<GameObject>[2];
        placedMonsters[0] = new List<GameObject>();
        placedMonsters[1] = new List<GameObject>();
        chessTable = new bool[BOARD_SIZE, BOARD_SIZE];
        monsPosTable = new bool[BOARD_SIZE, BOARD_SIZE];
        BoarderForPlacingBlocks = new int[2];
        BoarderForPlacingBlocks[0] = 0;
        BoarderForPlacingBlocks[1] = BOARD_SIZE - 1;
        steps = new int[2];
        steps[0] = 2 + Round;
        steps[1] = 1 + Round;
        playerIndex = 1;
        allCards = new List<CardController>();
        GameObject ca = GameObject.FindGameObjectWithTag("CardList");
        foreach (Transform child in ca.transform)
        {
            allCards.Add(child.GetComponent<CardController>());
        }
        for (int i = 0; i < 15; i++)
        {
            AI.cards.Add(allCards[Random.Range(0, allCards.Count)]);
        }
        Debug.Log(AI.cards.Count);
    }
    bool waitManager;
    // Update is called once per frame
    public void stateControll()
    {
		if (curPhase == battlePhase.IDLE) { // start game
			changeBattlePhase ((int)battlePhase.DRAW);
			steps [playerIndex] += Round;
			drawCard = false;
		} else if (curPhase == battlePhase.DRAW) {
			//open cardUI
			if (playerIndex == 0 && !CardUIControllerOpen) {
				CardUIController.GetComponent<EasyTween> ().OpenCloseObjectAnimation ();
				CardUIControllerOpen = true;
			}
			if (drawCard) {
				changeBattlePhase ((int)battlePhase.PLACE_BLOCK);
				placeBlockEnd = false;
			}
		} else if (curPhase == battlePhase.PLACE_BLOCK) {

			destroyEnd = false;
			ChooseMoveMonsEnd = false;
			if (placeBlockEnd) {
				if (playerIndex == 1)
					Debug.Log ("Count" + placedMonsters [playerIndex].Count);
				//can't be 0
				if (placedMonsters [playerIndex].Count > monsLimit [playerIndex]) {
					Debug.Log ("ddwdw");
					AskToDestroy = true;
					if (WantToDestroy || playerIndex == 1) {
						AskToDestroy = false;
						changeBattlePhase ((int)battlePhase.DESTROY_CHAR);
					} else if (NotToDestroy) {
						AskToDestroy = false;
						changeBattlePhase ((int)battlePhase.CHOOSE_MOVE_MONSTER);
					}
				} else
					changeBattlePhase ((int)battlePhase.PLACE_MONSTER);
				placeMonsEnd = false;
			}
			if (playerIndex == 1 && !waitManager)
				return;
		} else if (curPhase == battlePhase.DESTROY_CHAR) {
			if (destroyEnd) {
				changeBattlePhase ((int)battlePhase.PLACE_MONSTER);
				placeMonsEnd = false;
				WantToDestroy = false;
				NotToDestroy = false;
				DecideToDestroy = false;
			}
		} else if (curPhase == battlePhase.PLACE_MONSTER) {
			if (placeMonsEnd) {
				enemyPlayerIndex = -1;
				changeBattlePhase ((int)battlePhase.CAMERA_MOVE_SUMMON);
				cameraMoveSummonEnd = false;
			}
		} else if (curPhase == battlePhase.CAMERA_MOVE_SUMMON) {

			if (cameraMoveSummonEnd) {
				summon.Stop ();
				changeBattlePhase ((int)battlePhase.CHOOSE_MOVE_MONSTER);
				ChooseMoveMonsEnd = false;
			}
		} else if (curPhase == battlePhase.CHOOSE_MOVE_MONSTER) {

			if (ChooseMoveMonsEnd ) {
				changeBattlePhase ((int)battlePhase.MOVE_MONSTER);
				MoveMonsEnd = false;
			}
			movingPath = null;
		} else if (curPhase == battlePhase.MOVE_MONSTER) {
			if (steps [playerIndex] == 0)
				MoveMonsEnd = true;
			if (MoveMonsEnd) {
				if (CheckBattleHappen ()) {
					if (playerIndex == 1) {
						if (AI.RotateBattleDir ())
							changeBattlePhase ((int)battlePhase.BATTLE);
					} else
						changeBattlePhase ((int)battlePhase.BATTLE);
				} else {
					changeBattlePhase ((int)battlePhase.CHANGE_PLAYER);
					BattleEnd = false;
				}
				BattleEnd = false;
			}
		} else if (curPhase == battlePhase.BATTLE) {
			MoveMonsEnd = false;
			if (BattleEnd) {
				BattleEnd = false;
				changeBattlePhase ((int)battlePhase.CHANGE_PLAYER);
			}
		} else if (curPhase == battlePhase.CHANGE_PLAYER) {
			Round++;
			changeBattlePhase ((int)battlePhase.IDLE);
		} else if (curPhase == battlePhase.GAMEOVER) {
			Debug.Log ("ggggggggg");
		}
    }
    public void Update()
    {
        waitManager = AI.waitManager;
        RoundDisplay.text = "Round  " + Round;
        StepDisplay.text = "Steps remained: " + steps[playerIndex];
        stateControll();

        UpdateSelect();

        //border check



        DrawBoard();

    }
    Vector2 temp = new Vector2();
    public bool TryToMove(int dir)
    {
        Vector3 pos = selectMoveCharacter.transform.position;
        Debug.Log("try to move :  " + pos + ", dir: " + dir);
        if (curPhase != battlePhase.MOVE_MONSTER || selectMoveCharacter == null) return false;
        // 4 directions  0: up 1:down 2:left 3:right

        if (dir == 0 && selectMoveCharacter.transform.position.z <= BOARD_SIZE - 1 && 
            chessTable[(int)pos.x, (int)pos.z + 1] && !monsPosTable[(int)pos.x, (int)pos.z + 1])//up
        {
            if (chessTable[(int)pos.x, (int)pos.z + 1] && !monsPosTable[(int)pos.x, (int)pos.z + 1])
            {
                Debug.Log("monsPosTable : " + monsPosTable[(int)pos.x, (int)pos.z + 1]);
                selectMoveCharacter.GetComponent<CharacterMotion>().MoveCharacter(dir);
                return true;
            }
        }
        else if (dir == 1 && selectMoveCharacter.transform.position.z >= 1 &&
            chessTable[(int)pos.x, (int)pos.z - 1] && !monsPosTable[(int)pos.x, (int)pos.z - 1])//down
        {
            if (chessTable[(int)pos.x, (int)pos.z - 1] && !monsPosTable[(int)pos.x, (int)pos.z - 1])
            {
                Debug.Log("monsPosTable : " + monsPosTable[(int)pos.x, (int)pos.z - 1]);
                selectMoveCharacter.GetComponent<CharacterMotion>().MoveCharacter(dir);
                return true;
            }
        }
        else if (dir == 2 && selectMoveCharacter.transform.position.x >= 1&&
           chessTable[(int)pos.x - 1, (int)pos.z] && !monsPosTable[(int)pos.x - 1, (int)pos.z])//left
        {
            if (chessTable[(int)pos.x - 1, (int)pos.z] && !monsPosTable[(int)pos.x - 1, (int)pos.z])
            {
                Debug.Log("monsPosTable : " + monsPosTable[(int)pos.x - 1, (int)pos.z]);
                selectMoveCharacter.GetComponent<CharacterMotion>().MoveCharacter(dir);
                return true;
            }
        }
        else if (dir == 3 && selectMoveCharacter.transform.position.x <= BOARD_SIZE - 1 &&
           chessTable[(int)pos.x + 1, (int)pos.z] && !monsPosTable[(int)pos.x + 1, (int)pos.z])//right
        {
            if (chessTable[(int)pos.x + 1, (int)pos.z] && !monsPosTable[(int)pos.x + 1, (int)pos.z])
            {
                Debug.Log("monsPosTable : " + monsPosTable[(int)pos.x + 1, (int)pos.z]);
                selectMoveCharacter.GetComponent<CharacterMotion>().MoveCharacter(dir);
                return true;
            }
        }
        return false;
    }
    public bool CheckBattleHappen()
    {
        if (curPhase != battlePhase.MOVE_MONSTER || selectMoveCharacter == null)
            return false;
        //Monster Cases
        enemyPlayerIndex = (playerIndex + 1) % 2;
        Vector3 selectMoveCharacterPos = selectMoveCharacter.transform.position;
        for (int i = 0; i < placedMonsters[(enemyPlayerIndex)].Count; i++)
        {
            GameObject mons = placedMonsters[enemyPlayerIndex][i];
            if ((int)mons.transform.position.x == (int)selectMoveCharacterPos.x && (int)mons.transform.position.z == (int)selectMoveCharacterPos.z + 1)
                return true;
        }
        for (int i = 0; i < placedMonsters[enemyPlayerIndex].Count; i++)
        {
            GameObject mons = placedMonsters[enemyPlayerIndex][i];
            if ((int)mons.transform.position.x == (int)selectMoveCharacterPos.x && (int)mons.transform.position.z == (int)selectMoveCharacterPos.z - 1)
                return true;
        }
        for (int i = 0; i < placedMonsters[enemyPlayerIndex].Count; i++)
        {
            GameObject mons = placedMonsters[enemyPlayerIndex][i];
            if ((int)mons.transform.position.x == (int)selectMoveCharacterPos.x - 1 && (int)mons.transform.position.z == (int)selectMoveCharacterPos.z)
                return true;
        }
        for (int i = 0; i < placedMonsters[enemyPlayerIndex].Count; i++)
        {
            GameObject mons = placedMonsters[enemyPlayerIndex][i];
            if ((int)mons.transform.position.x == (int)selectMoveCharacterPos.x + 1 && (int)mons.transform.position.z == (int)selectMoveCharacterPos.z)
                return true;
        }
        //Debug.Log("this will happen");
        //Castle Cases
        if ((int)selectMoveCharacterPos.x == 5 || (int)selectMoveCharacterPos.x == 6 || (int)selectMoveCharacterPos.x == 7)
        {
            int EnemyCastleBorder = (playerIndex == 0) ? BOARD_SIZE - 1 : 0;
            if ((int)selectMoveCharacterPos.z == EnemyCastleBorder)
            {
                return true;
            }
        }

        //no battle happen
        Debug.Log("jump to change player!");
        return false;
    }

    //find enemy in enemy's queue, if not find , return -1
    public int findEnemy(GameObject from)
    {
        enemyPlayerIndex = (playerIndex + 1) % 2;
        Vector3 fromPos = from.transform.position;
        int dir = from.GetComponent<CharacterMotion>().ReturnDir();
        switch (dir)
        {
            case 0:
                for (int i = 0; i < placedMonsters[(enemyPlayerIndex)].Count; i++)
                {
                    GameObject mons = placedMonsters[enemyPlayerIndex][i];
                    if ((int)mons.transform.position.x == (int)fromPos.x && (int)mons.transform.position.z == (int)fromPos.z + 1)
                        return i;
                }
                break;
            case 1:
                for (int i = 0; i < placedMonsters[enemyPlayerIndex].Count; i++)
                {
                    GameObject mons = placedMonsters[enemyPlayerIndex][i];
                    if ((int)mons.transform.position.x == (int)fromPos.x && (int)mons.transform.position.z == (int)fromPos.z - 1)
                        return i;
                }
                break;
            case 2:
                for (int i = 0; i < placedMonsters[enemyPlayerIndex].Count; i++)
                {
                    GameObject mons = placedMonsters[enemyPlayerIndex][i];
                    if ((int)mons.transform.position.x == (int)fromPos.x - 1 && (int)mons.transform.position.z == (int)fromPos.z)
                        return i;
                }
                break;
            case 3:
                for (int i = 0; i < placedMonsters[enemyPlayerIndex].Count; i++)
                {
                    GameObject mons = placedMonsters[enemyPlayerIndex][i];
                    if ((int)mons.transform.position.x == (int)fromPos.x + 1 && (int)mons.transform.position.z == (int)fromPos.z)
                        return i;
                }
                break;
            default:
                break;
        }
        return -1;
    }
    public bool TryToBattle()
    {

        Vector3 pos = selectMoveCharacter.transform.position;
        if (curPhase != battlePhase.BATTLE || selectMoveCharacter == null) return false;
        // 4 directions  0: up 1:down 2:left 3:right

        int MonsterDir = selectMoveCharacter.GetComponent<CharacterMotion>().ReturnDir();

        //Damage To Castle
        if (
            (int)selectMoveCharacter.transform.position.x == 5 ||
            (int)selectMoveCharacter.transform.position.x == 6 ||
            (int)selectMoveCharacter.transform.position.x == 7
          )
        {
            int EnemyCastleBorder = (playerIndex == 0) ? BOARD_SIZE - 1 : 0;
            int EnemyCastleDirection = (playerIndex == 0) ? 0 : 1;
            if ((int)selectMoveCharacter.transform.position.z == EnemyCastleBorder && MonsterDir == EnemyCastleDirection)
            {
                
                selectMoveCharacter.GetComponent<CharacterMotion>().BattleAnimation(playerIndex, false);
                Castle[enemyPlayerIndex].GetComponent<CastleLife>().getDamage(enemyPlayerIndex);
            }
        }

        //Damage to Monster
        int enemyMonsIndexInList = findEnemy(selectMoveCharacter);
        Debug.Log(enemyMonsIndexInList + "enemy");
        if (enemyMonsIndexInList == -1)
            return false;

        Debug.Log("dir:  " + MonsterDir);
    //ss    Debug.Log("table:  " + monsPosTable[(int)pos.x, (int)pos.z + 1] + monsPosTable[(int)pos.x, (int)pos.z - 1] + monsPosTable[(int)pos.x - 1, (int)pos.z] + monsPosTable[(int)pos.x + 1, (int)pos.z]);
        // Detect enemy monster  !!! [TODO] need to identify whether it is enemy's monster !!!
        if (selectMoveCharacter.transform.position.z <= BOARD_SIZE - 1 && MonsterDir == 0 &&
            chessTable[(int)pos.x, (int)pos.z + 1] && monsPosTable[(int)pos.x, (int)pos.z + 1])//up
        {
            if (chessTable[(int)pos.x, (int)pos.z + 1] && monsPosTable[(int)pos.x, (int)pos.z + 1])
            {
                selectMoveCharacter.GetComponent<CharacterMotion>().BattleAnimation(0, true);
                return true;
            }
        }
        else if (selectMoveCharacter.transform.position.z >= 1 && MonsterDir == 1 &&
            chessTable[(int)pos.x, (int)pos.z - 1] && monsPosTable[(int)pos.x, (int)pos.z - 1])//down
        {
            if (chessTable[(int)pos.x, (int)pos.z - 1] && monsPosTable[(int)pos.x, (int)pos.z - 1])
            {
                selectMoveCharacter.GetComponent<CharacterMotion>().BattleAnimation(1, true);
                return true;
            }
        }
        else if (selectMoveCharacter.transform.position.x >= 1 && MonsterDir == 2&&
           chessTable[(int)pos.x - 1, (int)pos.z] && monsPosTable[(int)pos.x - 1, (int)pos.z])//left
        {
            if (chessTable[(int)pos.x - 1, (int)pos.z] && monsPosTable[(int)pos.x - 1, (int)pos.z])
            {
                selectMoveCharacter.GetComponent<CharacterMotion>().BattleAnimation(2, true);
                return true;
            }
        }
        else if (selectMoveCharacter.transform.position.x <= BOARD_SIZE - 1 && MonsterDir == 3 &&
            chessTable[(int)pos.x + 1, (int)pos.z] && monsPosTable[(int)pos.x + 1, (int)pos.z])//right
        {
            if (chessTable[(int)pos.x + 1, (int)pos.z] && monsPosTable[(int)pos.x + 1, (int)pos.z])
            {
                selectMoveCharacter.GetComponent<CharacterMotion>().BattleAnimation(3, true);
                return true;
            }
        }

        return false;
    }
    public void DrawBoard()
    {

      //  DrawLine();
        if (curPhase == battlePhase.PLACE_BLOCK || curPhase == battlePhase.PLACE_MONSTER)
        {
            if ((selectShape) || (selectMonster))
            {
                if (n_PlaceObject == null && (selectShape || selectMonster)) // Instantiate select item
                {
                    if (selectShape)
                    {
                        //hide cardUI
                        if (playerIndex == 0 && CardUIControllerOpen) 
                        {
                            CardUIController.GetComponent<EasyTween>().OpenCloseObjectAnimation();
                            CardUIControllerOpen = false;
                        }
                        n_PlaceObject = Instantiate(Floors[selectIndex], new Vector3(select_X + TILE_offset, 0, select_Y + TILE_offset), Quaternion.identity) as GameObject;
                        n_PlaceObject.transform.SetParent(transform);
                        n_PlaceObject.layer = LayerMask.NameToLayer("Floor");
                        foreach (Transform child in n_PlaceObject.transform)
                        {
                            child.gameObject.GetComponent<MeshRenderer>().material = playerBlockMat[playerIndex];
                        }
                    }
                    else if (selectMonster)
                    {
                        n_PlaceObject = Instantiate(Monsters[selectMonsIndex], new Vector3(select_X + TILE_offset, 0, select_Y + TILE_offset), Quaternion.identity) as GameObject;
                        n_PlaceObject.transform.SetParent(transform);
						if (playerIndex == 1)
							n_PlaceObject.transform.rotation = Quaternion.LookRotation (n_PlaceObject.transform.forward * -1);
                        n_PlaceObject.layer = LayerMask.NameToLayer("Monster");
                        ChangeMaskMonster(n_PlaceObject);
                    }
                }
                else if (n_PlaceObject != null)  //update n_PlaceObject
                {
                    //we use n_PlaceObject's position to detect boarder ,so move the object first
                    n_PlaceObject.transform.position = new Vector3(select_X + TILE_offset, 0, select_Y + TILE_offset);

                    // if not valid , change the object to transparent , not Setactive(false)
                    //AI change phase
                    // if (curPhase == battlePhase.PLACE_BLOCK && playerIndex == 0)
                    // {
                    //     if (!checkb)
                    //     {
                    //         foreach (Transform child in n_PlaceObject.transform)
                    //         {
                    //             MeshRenderer m = child.gameObject.GetComponent<MeshRenderer>();
                    //             m.enabled = false;
                    //         }
                    //     }
                    //     else
                    //     {
                    //         foreach (Transform child in n_PlaceObject.transform)
                    //         {
                    //             MeshRenderer m = child.gameObject.GetComponent<MeshRenderer>();
                    //             m.enabled = true;
                    //         }
                    //     }
                    // }
                    if (selectShape)
                    {
                        //  Debug.Log(selectIndex);
                        if (n_PlaceObject)
                            checkb = CheckBlockBorder(n_PlaceObject);
                        else
                            checkb = CheckBlockBorder(Floors[selectIndex]); //default option
                    }
                    else if (selectMonster)
                    {
                        //  Debug.Log(selectIndex);
                        if (n_PlaceObject)
                            checkb = CheckBlockBorder(n_PlaceObject);
                        else
                            checkb = CheckBlockBorder(Monsters[selectMonsIndex]); //default option
                    }
                    // if not valid , change the object to transparent , not Setactive(false)
                    PlaceItem();
                }
            }
        }
        else if (curPhase == battlePhase.CAMERA_MOVE_SUMMON)
        {
            MainCamera.enabled = false;
            MonsterCamera.enabled = true;
            MoveCameraOnPlaceMonster(MonsterOnCamera);
        }


    }
    void ChangeMaskMonster(GameObject obj)
    {
        string Blue = "_Blue";
        string Red = "_Red";
        string Mask = (playerIndex == 0) ? Blue : Red;
        Material[] MonsArray = (playerIndex == 0) ? BlueMons : RedMons;
        Material[] mat;

        string k = "Clone)";
        string trimName = obj.name.TrimEnd(k.ToCharArray());
        k = "(";
        trimName = trimName.TrimEnd(k.ToCharArray());

        //Debug.Log(trimName + " , " + obj.name);
        k = (" (Instance)");
        // playerIndex : 0 Blue
        switch (trimName)
        {
            case "Knight":
                //Get origin material
                mat = obj.transform.Find("Group Locator").Find("Knight").GetComponent<SkinnedMeshRenderer>().materials;
                // find material to replace
                for (int i = 0; i < mat.Length; i++)
                {
                    string ins = "Instance)";
                    string Origin = mat[i].name.TrimEnd(ins.ToCharArray());
                    ins = " (";
                    Origin = Origin.TrimEnd(ins.ToCharArray());
                    //Debug.Log (Origin);
                    foreach (Material m in MonsArray)
                    {
                        //Debug.Log (Origin + Mask);
                        string st = Origin + Mask;
                        if (m.name.Equals(st))
                        {
                            mat[i] = m;
                            obj.transform.Find("Group Locator").Find("Knight").GetComponent<SkinnedMeshRenderer>().materials = mat;
                            //Debug.Log("change: " + mat[i].name);
                            break;
                        }
                    }
                }
                break;
            case "FireBall":
                //Get origin material
                mat = obj.transform.Find("StoneMonster").GetComponent<SkinnedMeshRenderer>().materials;
                // find material to replace
                for (int i = 0; i < mat.Length; i++)
                {
                    //bool fin = false;
                    string Origin = mat[i].name.TrimEnd(k.ToCharArray());
                    foreach (Material m in MonsArray)
                    {
                        string st = Origin + Mask;
                        if (m.name.Equals(st))
                        {
                            //replace
                            mat[i] = m;
                            obj.transform.Find("StoneMonster").GetComponent<SkinnedMeshRenderer>().materials = mat;
                            break;
                        }
                    }
                }
                break;
            case "Slime":
                //Get origin material
                mat = obj.transform.Find("ModelSlime").GetComponent<SkinnedMeshRenderer>().materials;
                // find material to replace
                for (int i = 0; i < mat.Length; i++)
                {
                    //bool fin = false;
                    string Origin = mat[i].name.TrimEnd(k.ToCharArray());
                    foreach (Material m in MonsArray)
                    {
                        string st = Origin + Mask;
                        if (m.name.Equals(st))
                        {
                            mat[i] = m;
                            obj.transform.Find("ModelSlime").GetComponent<SkinnedMeshRenderer>().materials = mat;
                            Debug.Log("ori2:/" + mat[i].name + "/to:/" + m.name + "/");
                            break;
                        }
                    }
                }
                break;
            default:
                break;
        }
    }
    public GameObject getnPlaceObj()
    {
        return n_PlaceObject;
    }
    public void UpdateSelect() // place and select
    {
        if (curPhase == battlePhase.CHOOSE_MOVE_MONSTER)
        {
            //Debug.Log(chooseMoveMonsIndex);
            if( placedMonsters[playerIndex].Count==1)
            {
                selectMoveCharacter = placedMonsters[playerIndex][0];
                chooseMoveMonsIndex = 0;
                ChooseMoveMonsEnd = true;
            }
            selectMoveCharacter = placedMonsters[playerIndex][chooseMoveMonsIndex];
            if (selMonsFlag.activeSelf == false)
                selMonsFlag.SetActive(true);
            GameObject s = placedMonsters[playerIndex][chooseMoveMonsIndex];
            selMonsFlag.transform.position = s.transform.position + Vector3.up * 2;
        }
        else if (curPhase == battlePhase.DESTROY_CHAR)
        {
            Debug.Log("seleeee");
            if (playerIndex == 1)
            {
                int t = Random.Range(0, placedMonsters[1].Count);
                selectMoveCharacter = placedMonsters[playerIndex][t];
                if (selMonsFlag.activeSelf == false)
                    selMonsFlag.SetActive(true);
                GameObject ss = placedMonsters[playerIndex][t];
                selMonsFlag.transform.position = ss.transform.position + Vector3.up * 2;
				placedMonsters[playerIndex][t].tag = "Todestroy";
                destroyMons(selectMoveCharacter, playerIndex);
            }
            else
            {
                selectMoveCharacter = placedMonsters[playerIndex][chooseMoveMonsIndex];
                if (selMonsFlag.activeSelf == false)
                    selMonsFlag.SetActive(true);
                GameObject s = placedMonsters[playerIndex][chooseMoveMonsIndex];
                selMonsFlag.transform.position = s.transform.position + Vector3.up * 2;
                if (DecideToDestroy)
                {
                    placedMonsters[playerIndex][chooseMoveMonsIndex].tag = "Todestroy";
                    destroyMons(selectMoveCharacter, playerIndex);
                }
            }

        }
        else if (curPhase == battlePhase.MOVE_MONSTER)
        {
            selMonsFlag.transform.position = placedMonsters[playerIndex][chooseMoveMonsIndex].transform.position + Vector3.up * 2;
        }
        if (Input.GetMouseButtonDown(2) && (curPhase == battlePhase.PLACE_BLOCK || curPhase == battlePhase.PLACE_MONSTER))  //Cancel sellect
        {
            selectIndex = -1;
            selectShape = false;
            if (n_PlaceObject != null)
            {
                Destroy(n_PlaceObject);
                n_PlaceObject = null;
            }
        }
        //position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (playerIndex == 0) // AI need update by itself
        {
            if (Physics.Raycast(ray, out hit, 100.0f, LayerMask.GetMask("Board")))
            {
                // Debug.Log((int)hit.point.x + " , " + (int)hit.point.z);
                select_X = (int)hit.point.x;
                select_Y = (int)hit.point.z;
            }
            else
            {
                select_X = -1;
                select_Y = -1;
            }
        }

        //select shape;
        if (selectIndex != -1 && curPhase == battlePhase.PLACE_BLOCK)
            selectShape = true;
        ToSelectMons();
        // select at UIfunction 
        //phaseControl();

    }
    void DrawLine()
    {
        for (int i = 0; i < BOARD_SIZE + 1; i++)
        {
            LinesX[i].DrawLineInGameView(Vector3.right * i, Vector3.right * i + Vector3.forward * BOARD_SIZE, Color.blue);
            LinesZ[i].DrawLineInGameView(Vector3.forward * i, Vector3.forward * i + Vector3.right * BOARD_SIZE, Color.blue);
            /* Debug.DrawLine(Vector3.right * i, Vector3.right * i + Vector3.forward * BOARD_SIZE);
             Debug.DrawLine(Vector3.forward * i, Vector3.forward * i + Vector3.right * BOARD_SIZE);*/
        }
    }
    public void destroyMons(GameObject mons, int player)
    {
        if (!mons) return;
        for (int i = 0; i < placedMonsters[player].Count; i++)
        {
            if (placedMonsters[player][i] == mons  && placedMonsters[player][i].tag == "Todestroy")
            {
                Debug.Log(placedMonsters[player][i].transform.position);

                monsPosTable[(int)placedMonsters[player][i].transform.position.x, (int)placedMonsters[player][i].transform.position.z] = false;
                //Debug.Log(playerIndex + "  destroy pos: " + (int)selectMoveCharacter.transform.position.x+" , " + (int)selectMoveCharacter.transform.position.z );
                placedMonsters[playerIndex].RemoveAt(i);
                Destroy(mons, 0.2f);
                destroyEnd = true;
                return;
            }
        }
        Debug.Log("don't find destroy obj");
    }
    void SpawnLine()
    {
        LinesParent = new GameObject("lineparent");
        LinesX = new List<LineDrawer>();
        LinesZ = new List<LineDrawer>();
        for (int i = 0; i < BOARD_SIZE + 1; i++)
        {
            LineDrawer x = new LineDrawer(0.2f);
            LineDrawer y = new LineDrawer(0.2f);

            LinesX.Add(x);
            LinesZ.Add(y);
            x.Setparent(LinesParent.transform);
            y.Setparent(LinesParent.transform);
        }
    }
    public void phaseControl()
    {
        if (n_PlaceObject != null && selectShape)
        {
            n_PlaceObject.transform.Rotate(n_PlaceObject.transform.up, 90.0f);
        }
    }

    public void ToSelectMons()
    {
        if (curPhase != battlePhase.PLACE_MONSTER) return;
        if (sel_monster.Length == 0) return;
        if (Input.GetMouseButton(0) || playerIndex == 1)
        {
            //Debug.Log("Select Monster");
            selectMonster = true;
            for (int i = 0; i < Monsters.Length; i++)
            {
                //Debug.Log(Monsters[i].transform.name);
                if (sel_monster.ToLower() == (Monsters[i].transform.name).ToLower())
                {
                    selectMonsIndex = i;
                    break;
                }
            }
            if (selectMonsIndex == -1)
            {
                Debug.Log("not find monster");
                selectMonster = false;
                selectMonsIndex = -1;
            }
        }
    }

    private void PlaceItem()
    {
        if (timerForWrongPlacement > 0)
            timerForWrongPlacement--;
        // recover material from wrong placement
        if (timerForWrongPlacement == 0)
        {
            foreach (Transform child in n_PlaceObject.transform)
            {
                MeshRenderer l = child.GetComponent<MeshRenderer>();
                l.material = Instantiate(originMat);
                string k = "(Clone) (Instance)";
                l.material.name = l.material.name.Trim(k.ToCharArray());
                // Debug.Log(l.material.name);
            }
            timerForWrongPlacement = -10;
            originMat = null;
        }
        //try to put down item
        float updateBoarder = BoarderForPlacingBlocks[playerIndex];
        float max = -1;
        float min = -1;
        //  checkb = CheckBlockBorder(n_PlaceObject);
        if (playerIndex == 1 && AI.waitManager && curPhase == battlePhase.PLACE_BLOCK && checkb && UpdateTable(n_PlaceObject, false))
        {
            updateBoarder = BoarderForPlacingBlocks[playerIndex];
            Debug.Log("checking");
            foreach (Transform child in n_PlaceObject.transform)
            {
                if (max == -1 || min == -1)
                {
                    max = child.position.z;
                    min = child.position.z;
                }
                else
                {
                    if (max < child.position.z)
                    {
                        max = child.position.z;
                    }
                    if (min > child.position.z)
                    {
                        min = child.position.z;
                    }
                }
            }
            foreach (Transform child in n_PlaceObject.transform)
            {
                if ((int)max >= BoarderForPlacingBlocks[1])
                {
                    validBoarder = true;
                    //place beside blocks
                    if (((int)child.position.x > 0 && chessTable[(int)child.position.x - 1, (int)child.position.z] && BlockNeighborCheck((int)child.position.x - 1, (int)child.position.z, 1)) ||
                        ((int)child.position.x < BOARD_SIZE - 1 && chessTable[(int)child.position.x + 1, (int)child.position.z] && BlockNeighborCheck((int)child.position.x + 1, (int)child.position.z, 1)) ||
                        ((int)child.position.z > 0 && chessTable[(int)child.position.x, (int)child.position.z - 1] && BlockNeighborCheck((int)child.position.x, (int)child.position.z - 1, 1)) ||
                        ((int)child.position.z < BOARD_SIZE - 1 && chessTable[(int)child.position.x, (int)child.position.z + 1] && BlockNeighborCheck((int)child.position.x, (int)child.position.z + 1, 1)) ||
                         BoarderForPlacingBlocks[1] == BOARD_SIZE - 1)
                        besideBlocks = true;
                    else
                        besideBlocks = false;
                }
                else
                {
                    validBoarder = false;
                    besideBlocks = false;
                }

                //check next


                if (validBoarder && besideBlocks && n_PlaceObject != null)
                {
                    placeBlockFind = true;
                    if (AI.submitDecision)
                    {
                        if (min <= BoarderForPlacingBlocks[1] && validBoarder && (updateBoarder >= min))
                            updateBoarder = min - 1;
                        foreach (Transform childt in n_PlaceObject.transform)
                        {
                            MeshRenderer m = childt.gameObject.GetComponent<MeshRenderer>();
                            m.enabled = true;
                        }

                        UpdateTable(n_PlaceObject, true);
                        BoarderForPlacingBlocks[playerIndex] = (int)updateBoarder;
                        placedBlocks[playerIndex].Add(n_PlaceObject);
                        n_PlaceObject = null;
                        selectShape = false;
                        selectIndex = -1;
                        placeBlockEnd = true;
                        Debug.Log(sel_monster);
                    }
                    else
                    {
                        AI.updatePlaceBlock((int)min);
                        if (placeBlockFind)
                            Debug.Log(n_PlaceObject.transform.rotation.eulerAngles);
                    }
                    // UpdateTable(n_PlaceObject, true);
                    // BoarderForPlacingBlocks[playerIndex] = (int)updateBoarder;
                    // placedBlocks[playerIndex].Add(n_PlaceObject);
                    // n_PlaceObject = null;
                    // selectShape = false;
                    // selectIndex = -1;
                    // placeBlockEnd = true;
                    // Debug.Log(sel_monster);
                }
                else
                    WrongPlacement(n_PlaceObject);

                //  AI.waitManager = false;
            }
        }
        //AI place monster
        if (playerIndex == 1 && AI.waitManager && curPhase == battlePhase.PLACE_MONSTER && checkb && UpdateTable(n_PlaceObject, false))
        {
            // n_PlaceObject.GetComponent<Renderer>().enabled = !n_PlaceObject.GetComponent<Renderer>().enabled;
            placedMonsters[playerIndex].Add(n_PlaceObject);
            n_PlaceObject.GetComponent<CharacterMotion>().summonMons();
            selectMonster = false;
            selectMonsIndex = -1;
            UpdateTable(n_PlaceObject, true);
            MonsterOnCamera = n_PlaceObject;
            n_PlaceObject = null;
            placeMonsEnd = true;
        }
        else if (playerIndex == 1 && AI.waitManager && n_PlaceObject != null)
            WrongPlacement(n_PlaceObject);

        AI.waitManager = false;
        if (Input.GetMouseButtonDown(0))
        {
            // Assume that we will change battlePhase right after we place items
            if (curPhase == battlePhase.PLACE_BLOCK && checkb && UpdateTable(n_PlaceObject, false))
            {
                validBoarder = false;
                besideBlocks = false;
                updateBoarder = BoarderForPlacingBlocks[playerIndex];
                max = -1;
                min = -1;
                foreach (Transform child in n_PlaceObject.transform)
                {
                    if (max == -1 || min == -1)
                    {
                        max = child.position.z;
                        min = child.position.z;
                    }
                    else
                    {
                        if (max < child.position.z)
                        {
                            max = child.position.z;
                        }
                        if (min > child.position.z)
                        {
                            min = child.position.z;
                        }
                    }

                }
                foreach (Transform child in n_PlaceObject.transform)
                {

                    switch (playerIndex)
                    {

                        case 0:
                            //if one blocks in inside the area, then this item is valid
                            if ((int)min <= BoarderForPlacingBlocks[0])
                            {
                                validBoarder = true;
                                //place beside blocks or first place will set (besideBlocks) to true
                                if (((int)child.position.x > 0 && chessTable[(int)child.position.x - 1, (int)child.position.z] && BlockNeighborCheck((int)child.position.x - 1, (int)child.position.z, 0)) ||
                                    ((int)child.position.x < BOARD_SIZE - 1 && chessTable[(int)child.position.x + 1, (int)child.position.z] && BlockNeighborCheck((int)child.position.x + 1, (int)child.position.z, 0)) ||
                                    ((int)child.position.z > 0 && chessTable[(int)child.position.x, (int)child.position.z - 1] && BlockNeighborCheck((int)child.position.x, (int)child.position.z - 1, 0)) ||
                                    ((int)child.position.z < BOARD_SIZE - 1 && chessTable[(int)child.position.x, (int)child.position.z + 1] && BlockNeighborCheck((int)child.position.x, (int)child.position.z + 1, 0)) ||
                                     BoarderForPlacingBlocks[0] == 0)
                                    besideBlocks = true;
                            }

                            Debug.Log(BoarderForPlacingBlocks[0]);
                            break;
                        case 1:
                            /*
                            if (AI.waitNextPlaceBlock)
                            {
                                if ((int)max >= BoarderForPlacingBlocks[1])
                                {
                                    validBoarder = true;
                                    //place beside blocks
                                    if (((int)child.position.x > 0 && chessTable[(int)child.position.x - 1, (int)child.position.z] && BlockNeighborCheck((int)child.position.x - 1, (int)child.position.z, 1)) ||
                                        ((int)child.position.x < BOARD_SIZE - 1 && chessTable[(int)child.position.x + 1, (int)child.position.z] && BlockNeighborCheck((int)child.position.x + 1, (int)child.position.z, 1)) ||
                                        ((int)child.position.z > 0 && chessTable[(int)child.position.x, (int)child.position.z - 1] && BlockNeighborCheck((int)child.position.x, (int)child.position.z - 1, 1)) ||
                                        ((int)child.position.z < BOARD_SIZE - 1 && chessTable[(int)child.position.x, (int)child.position.z + 1] && BlockNeighborCheck((int)child.position.x, (int)child.position.z + 1, 1)) ||
                                         BoarderForPlacingBlocks[1] == BOARD_SIZE - 1)
                                        besideBlocks = true;
                                }
                                //check next
                                AI.waitNextPlaceBlock = false;
                                if (child.position.z <= BoarderForPlacingBlocks[1] && validBoarder && (updateBoarder >= child.position.z || updateBoarder == -1))
                                    updateBoarder = child.position.z - 1;    
                            }break;
                            */
                            break;
                        default:
                            break;
                    }
                }
                Debug.Log("valid : " + validBoarder + " , beside : " + besideBlocks);
                if (validBoarder && besideBlocks)
                {
                    if (max >= BoarderForPlacingBlocks[0] && validBoarder && (updateBoarder <= max))
                        updateBoarder = max + 1;
                    UpdateTable(n_PlaceObject, true);

                    BoarderForPlacingBlocks[playerIndex] = (int)updateBoarder;
                    placedBlocks[playerIndex].Add(n_PlaceObject);
                    n_PlaceObject = null;
                    selectShape = false;
                    selectIndex = -1;
                    placeBlockEnd = true;
                    Debug.Log(sel_monster);
                }
                else
                    WrongPlacement(n_PlaceObject);
            }
            if (curPhase == battlePhase.PLACE_MONSTER && checkb && UpdateTable(n_PlaceObject, false))
            {
				foreach (GameObject block in placedBlocks[1]) {
					foreach (Transform child in block.transform) {
						if ((int)child.transform.position.x == (int)n_PlaceObject.transform.position.x &&
						   (int)child.transform.position.z == (int)n_PlaceObject.transform.position.z)
							return;
					}
				}
                placedMonsters[playerIndex].Add(n_PlaceObject);
                n_PlaceObject.GetComponent<CharacterMotion>().summonMons();
                //selectMoveCharacter = n_PlaceObject;
                //temp = new Vector2(selectMoveCharacter.transform.position.x, selectMoveCharacter.transform.position.z);  // keeping initial position to selectMonster
                selectMonster = false;
                selectMonsIndex = -1;
                UpdateTable(n_PlaceObject, true);
                MonsterOnCamera = n_PlaceObject;
                n_PlaceObject = null;
                placeMonsEnd = true;
            }
            else if (n_PlaceObject != null)
                WrongPlacement(n_PlaceObject);
        }
    }
    public bool BlockNeighborCheck(int X_position, int Z_position, int whichPlayer)
    {
        foreach (GameObject Blocks in placedBlocks[whichPlayer])
        {
            foreach (Transform child in Blocks.transform)
            {
                if ((int)child.position.x == X_position && (int)child.position.z == Z_position)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void GetMonsterName(string name)
    {
        sel_monster = name;
    }

    private bool CheckBlockBorder(GameObject obj)//check if block is in the border
    {
        if (obj == null) return false;
        if (curPhase == battlePhase.PLACE_BLOCK)
        {
            foreach (Transform child in obj.transform)
            {
                //Debug.Log(child.position.x);
                if (child.position.x < 0f || child.position.z < 0f || child.position.x > (float)BOARD_SIZE || child.position.z > (float)BOARD_SIZE)
                    return false;
            }
        }
        else if (curPhase == battlePhase.PLACE_MONSTER)
            if (obj.transform.position.x < 0 || obj.transform.position.z < 0 || obj.transform.position.x > BOARD_SIZE || obj.transform.position.z > BOARD_SIZE)
                return false;

        return true;
    }

    // check and replace table if(set)
    private bool UpdateTable(GameObject obj, bool set)
    {
        //  if (!obj) return false;
        if (curPhase == battlePhase.PLACE_BLOCK)
        {
            bool b = true;
            //get children of blocks
            foreach (Transform child in obj.transform)
            {
                if (chessTable[(int)child.position.x, (int)child.position.z])
                {
                    b = false;
                    break;
                }
            }
            if (!b)
                return false;
            if (set)
            {
                foreach (Transform child in obj.transform)
                {
                    chessTable[(int)child.position.x, (int)child.position.z] = true;
                }
            }

        }
        else if (curPhase == battlePhase.PLACE_MONSTER)
        {
            if (!chessTable[(int)obj.transform.position.x, (int)obj.transform.position.z] || monsPosTable[(int)obj.transform.position.x, (int)obj.transform.position.z])
                return false;
            if (set)
                monsPosTable[(int)obj.transform.position.x, (int)obj.transform.position.z] = true;
        }
        return true;
    }

    private void MoveCameraOnPlaceMonster(GameObject placed_monster)
    {
        float dist = Vector3.Distance(placed_monster.transform.position, MonsterCamera.transform.position);
        MonsterCamera.transform.LookAt(placed_monster.transform.position);
        if (dist > 3.0f)
        {
            MonsterCamera.transform.Translate(MonsterCamera.transform.forward * Time.deltaTime * 6.0f);
        }
        if (dist <= 5.0f)
        {
            MonsterCamera.transform.RotateAround(placed_monster.transform.position, new Vector3(0, 1, 0), 32.0f * Time.deltaTime);
        }
        if (Input.GetMouseButtonDown(0) || (playerIndex == 1 && dist < 3.5f)/*for AI auto switch camera*/)
        {
            MonsterOnCamera = null;
            MainCamera.enabled = true;
            MonsterCamera.enabled = false;
            cameraMoveSummonEnd = true;
            MonsterCamera.transform.position = MainCamera.transform.position;
            MonsterCamera.transform.rotation = MainCamera.transform.rotation;
        }

    }

    //change color and set timer
    private void WrongPlacement(GameObject obj)
    {
        return;
        if (curPhase == battlePhase.PLACE_BLOCK)
        {
            Material t;
            t = obj.transform.GetChild(0).GetComponent<MeshRenderer>().material;
            string k = t.name.ToString();
            string c = " (Instance)";
            k = k.TrimEnd(c.ToCharArray());

            string s = "Material/" + k;

            originMat = Resources.Load(s, typeof(Material)) as Material;
            originMat = Instantiate(originMat) as Material;


            foreach (Transform child in obj.transform)
            {
                MeshRenderer l = child.GetComponent<MeshRenderer>();
                l.material = redMaterial;
            }

            timerForWrongPlacement = 10;
        }
        else if (curPhase == battlePhase.PLACE_MONSTER)
        {

        }
    }

    //choose next move monster
    public void ChangeMoveMons()
    {
        chooseMoveMonsIndex = (chooseMoveMonsIndex + 1) % placedMonsters[playerIndex].Count;
    }

    //summon monster into board
    public void Summon(GameObject obj)
    {
        CardController c;
        c = obj.GetComponent<CardController>();
        selectIndex = (int)c.getBlock();
        sel_monster = c.cardName;
        Debug.Log(sel_monster);
        Destroy(obj); // delete card ---->may have problem
    }


    //change phase and play animation
    public void changeBattlePhase(int next)
    {
        if (Time.time > nextmoveTime)
        {
            //nextmoveTime = Time.time + moveTime;
            //  moveCharactor();
        }
        battlePhase nextPhase = (battlePhase)next;
		if (nextPhase == battlePhase.MOVE_MONSTER) {
			if (playerIndex == 0)
				PopTextList.Activate ("MoveMonster");
			curPhase = (battlePhase)next;
		} else if (nextPhase == battlePhase.BATTLE) {
			if (playerIndex == 0)
				PopTextList.Activate ("Battle");
			curPhase = (battlePhase)next;
		} else if (nextPhase == battlePhase.PLACE_BLOCK) {
			if (playerIndex == 0)
				PopTextList.Activate ("PlaceBlock");
			selMonsFlag.SetActive (false);
			curPhase = (battlePhase)next;
		} else if (nextPhase == battlePhase.PLACE_MONSTER) {
            selMonsFlag.SetActive(false);
			if (playerIndex == 0)
				PopTextList.Activate ("PlaceMonster");
			curPhase = (battlePhase)next;
		} else if (nextPhase == battlePhase.CAMERA_MOVE_SUMMON) {
			summon.Play ();
			Debug.Log (summon.isPlaying);
			curPhase = (battlePhase)next;
		} else if (nextPhase == battlePhase.CHOOSE_MOVE_MONSTER) {
			if (playerIndex == 0)
				PopTextList.Activate ("ChooseMoveMonster");
			selMonsFlag.SetActive (true);
			chooseMoveMonsIndex = 0;
			curPhase = (battlePhase)next;
		} else if (nextPhase == battlePhase.CHANGE_PLAYER) {
			if (playerIndex == 0)
				PopTextList.Activate ("OpponentTurn");
			selMonsFlag.SetActive (false);
			playerIndex = (playerIndex + 1) % 2;
			curPhase = (battlePhase)next;
		} else if (nextPhase == battlePhase.DRAW) {
			if (playerIndex == 0) {
				PopTextList.Activate ("YourTurn");
				PopTextList.Activate ("Draw");
			}
			curPhase = (battlePhase)next;
		} else if (nextPhase == battlePhase.DESTROY_CHAR) {
			curPhase = (battlePhase)next;
		} else if (nextPhase == battlePhase.IDLE) {
			curPhase = (battlePhase)next;
		} else if (nextPhase == battlePhase.GAMEOVER) {
			
			curPhase = (battlePhase)next;
		}
    }
}
