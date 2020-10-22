using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UITween;


public class PlayerControll : MonoBehaviour
{
    public GameObject StaticButtonObject_up;
    public GameObject StaticButtonObject_down;
    public GameObject UpButtonController;
    public GameObject DownButtonController;
    private bool ShowButton = false;
    private Text ButtonText_up;
    private Text ButtonText_down;
    GameObject moveCharacter;
    // Use this for initialization
    void Start()
    {
        ButtonText_up   =  StaticButtonObject_up.GetComponent<Text>();
        ButtonText_down =  StaticButtonObject_down.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        GameObject b = GameObject.FindWithTag("ChessBoard");
        GameObject g = GameObject.FindWithTag("Hand");
        GameObject moveCharacter = b.GetComponent<BoardManager>().selectMoveCharacter;
        if(b.GetComponent<BoardManager>().playerIndex == 0)
        {
            if ((battlePhase)b.GetComponent<BoardManager>().getBattlePhase() == battlePhase.DRAW)
                {
                    ButtonText_up.text = "Draw Card!";
                }
            if ((battlePhase)b.GetComponent<BoardManager>().getBattlePhase() == battlePhase.CHOOSE_MOVE_MONSTER||
                (battlePhase)b.GetComponent<BoardManager>().getBattlePhase() == battlePhase.DESTROY_CHAR &&b.GetComponent<BoardManager>().playerIndex == 0)
                {
                    ButtonText_up.text = "Next Monster";
                }
            if ((battlePhase)b.GetComponent<BoardManager>().getBattlePhase() == battlePhase.MOVE_MONSTER)
                {
                    ButtonText_up.text = "Stop";
                }
            if ((battlePhase)b.GetComponent<BoardManager>().getBattlePhase() == battlePhase.BATTLE)
                {
                    ButtonText_up.text = "Attack!";
                }
            if ((battlePhase)b.GetComponent<BoardManager>().getBattlePhase() == battlePhase.PLACE_BLOCK && b.GetComponent<BoardManager>().AskToDestroy)
                {
                    ButtonText_up.text = "Destroy";
                }
            
            
            /*Down Button */
            if ((battlePhase)b.GetComponent<BoardManager>().getBattlePhase() == battlePhase.CHOOSE_MOVE_MONSTER)
                {
                    ButtonText_down.text = "OK!";
                }
            if ((battlePhase)b.GetComponent<BoardManager>().getBattlePhase() == battlePhase.PLACE_BLOCK && b.GetComponent<BoardManager>().AskToDestroy)
                {
                    ButtonText_down.text = "Not to Summon";
                }
            if ((battlePhase)b.GetComponent<BoardManager>().getBattlePhase() == battlePhase.DESTROY_CHAR && b.GetComponent<BoardManager>().WantToDestroy)
                {
                    ButtonText_down.text = "Confirm";
                }
        }
        if(ShowButton == true && b.GetComponent<BoardManager>().playerIndex == 1)
        {
            UpButtonController.GetComponent<EasyTween>().OpenCloseObjectAnimation();
            DownButtonController.GetComponent<EasyTween>().OpenCloseObjectAnimation();
            ShowButton = false;
        }
        if(ShowButton == false && b.GetComponent<BoardManager>().playerIndex == 0)
        {
            UpButtonController.GetComponent<EasyTween>().OpenCloseObjectAnimation();
            DownButtonController.GetComponent<EasyTween>().OpenCloseObjectAnimation();
            ShowButton = true;            
        }
        // 
        // if (Input.GetKeyDown(KeyCode.C))
        // {
        //     if ((battlePhase)b.GetComponent<BoardManager>().getBattlePhase() == battlePhase.DRAW)
        //     {
        //         g.GetComponent<DropZone>().AddCard((int)Random.Range(0, 3));
        //         b.GetComponent<BoardManager>().drawCard = true;
        //     }
        // }
        /*arrow*/
        if (Input.GetKeyDown(KeyCode.W))
        {
            if ((battlePhase)b.GetComponent<BoardManager>().getBattlePhase() == battlePhase.BATTLE)
            {
                moveCharacter.GetComponent<CharacterMotion>().TurnDirection(0);
            }
            else
                b.GetComponent<BoardManager>().TryToMove(0);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            if ((battlePhase)b.GetComponent<BoardManager>().getBattlePhase() == battlePhase.BATTLE)
            {
                moveCharacter.GetComponent<CharacterMotion>().TurnDirection(1);
            }
            else
                b.GetComponent<BoardManager>().TryToMove(1);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            if ((battlePhase)b.GetComponent<BoardManager>().getBattlePhase() == battlePhase.BATTLE)
            {
                moveCharacter.GetComponent<CharacterMotion>().TurnDirection(2);
            }
            else
                b.GetComponent<BoardManager>().TryToMove(2);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            if ((battlePhase)b.GetComponent<BoardManager>().getBattlePhase() == battlePhase.BATTLE)
            {
                moveCharacter.GetComponent<CharacterMotion>().TurnDirection(3);
            }
            else
                b.GetComponent<BoardManager>().TryToMove(3);
        }
        else if(Input.GetKeyDown(KeyCode.Space))
        {
            b.GetComponent<BoardManager>().phaseControl();
        }
        /*else */
        // else if (Input.GetKeyDown(KeyCode.P))
        // {
        //     if ((battlePhase)b.GetComponent<BoardManager>().getBattlePhase() == battlePhase.CHOOSE_MOVE_MONSTER||
        //        (battlePhase)b.GetComponent<BoardManager>().getBattlePhase() == battlePhase.DESTROY_CHAR &&b.GetComponent<BoardManager>().playerIndex == 0)
        //         b.GetComponent<BoardManager>().ChangeMoveMons();
        // }
        // else if (Input.GetKeyDown(KeyCode.O))
        // {
        //     if ((battlePhase)b.GetComponent<BoardManager>().getBattlePhase() == battlePhase.CHOOSE_MOVE_MONSTER)
        //         b.GetComponent<BoardManager>().ChooseMoveMonsEnd = true;
        // }
        else if (Input.GetKeyDown(KeyCode.Y))
        {
            if ((battlePhase)b.GetComponent<BoardManager>().getBattlePhase() == battlePhase.MOVE_MONSTER)
                b.GetComponent<BoardManager>().MoveMonsEnd = true;
        }
        // else if (Input.GetKeyDown(KeyCode.B))
        // {
        //     if ((battlePhase)b.GetComponent<BoardManager>().getBattlePhase() == battlePhase.BATTLE)
        //         b.GetComponent<BoardManager>().TryToBattle();
        // }
        // else if(Input.GetKeyDown(KeyCode.X))
        // {
        //     if ((battlePhase)b.GetComponent<BoardManager>().getBattlePhase() == battlePhase.DESTROY_CHAR)
        //         b.GetComponent<BoardManager>().WantToDestroy = true;
        // }
        // else if(Input.GetKeyDown(KeyCode.Z))
        // {
        //     if ((battlePhase)b.GetComponent<BoardManager>().getBattlePhase() == battlePhase.DESTROY_CHAR)
        //         b.GetComponent<BoardManager>().NotToDestroy = true;
        // }
        // else if(Input.GetKeyDown(KeyCode.C))
        // {
        //     if ((battlePhase)b.GetComponent<BoardManager>().getBattlePhase() == battlePhase.DESTROY_CHAR)
        //         b.GetComponent<BoardManager>().DecideToDestroy = true;
        // }
        // else if (Input.GetKeyDown(KeyCode.V))
        // {
        //     b.GetComponent<BoardManager>().BattleEnd = true;
        // }
        // if (Input.GetKeyDown(KeyCode.A))
        // {
        //     GameObject t = GameObject.Find("PopTextControll");
        //     t.GetComponent<PopTextController>().Activate("YourTurn");
        // }
        // else if (Input.GetKeyDown(KeyCode.S))
        // {
        //     GameObject t = GameObject.Find("PopTextControll");
        //     t.GetComponent<PopTextController>().Activate("OpponentTurn");
        // }
    }
    public void PhaseButtonClick_up()
    {
        GameObject b = GameObject.FindWithTag("ChessBoard");
        GameObject g = GameObject.FindWithTag("Hand");
        if ((battlePhase)b.GetComponent<BoardManager>().getBattlePhase() == battlePhase.DRAW)
        {
            g.GetComponent<DropZone>().AddCard((int)Random.Range(0, 3));
            b.GetComponent<BoardManager>().drawCard = true;
        }
        if ((battlePhase)b.GetComponent<BoardManager>().getBattlePhase() == battlePhase.CHOOSE_MOVE_MONSTER||
            (battlePhase)b.GetComponent<BoardManager>().getBattlePhase() == battlePhase.DESTROY_CHAR &&b.GetComponent<BoardManager>().playerIndex == 0)
            {
                b.GetComponent<BoardManager>().ChangeMoveMons();
            }
        if ((battlePhase)b.GetComponent<BoardManager>().getBattlePhase() == battlePhase.MOVE_MONSTER)
            {
                b.GetComponent<BoardManager>().MoveMonsEnd = true;
            }
        if ((battlePhase)b.GetComponent<BoardManager>().getBattlePhase() == battlePhase.BATTLE)
            {
                b.GetComponent<BoardManager>().TryToBattle();
            }
        if ((battlePhase)b.GetComponent<BoardManager>().getBattlePhase() == battlePhase.PLACE_BLOCK && b.GetComponent<BoardManager>().AskToDestroy)
            {
                b.GetComponent<BoardManager>().WantToDestroy = true;
            }
    }
    public void PhaseButtonClick_down(){
        GameObject b = GameObject.FindWithTag("ChessBoard");
        GameObject g = GameObject.FindWithTag("Hand");
        
        if ((battlePhase)b.GetComponent<BoardManager>().getBattlePhase() == battlePhase.CHOOSE_MOVE_MONSTER)
            {
                b.GetComponent<BoardManager>().ChooseMoveMonsEnd = true;
            }
        if ((battlePhase)b.GetComponent<BoardManager>().getBattlePhase() == battlePhase.PLACE_BLOCK && b.GetComponent<BoardManager>().AskToDestroy)
            {
                b.GetComponent<BoardManager>().NotToDestroy = true;
            }
        if ((battlePhase)b.GetComponent<BoardManager>().getBattlePhase() == battlePhase.DESTROY_CHAR && b.GetComponent<BoardManager>().WantToDestroy)
            {
                b.GetComponent<BoardManager>().DecideToDestroy = true;
            }
    }
}
