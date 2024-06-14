using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] MonopolyBoard gameBoard;
    [SerializeField] List<Player> playerList = new List<Player>();
    [SerializeField] int currentPlayer;
    [Header("Global Game Settings")]
    [SerializeField] int maxTurnsInJail = 3; //how long a player can stay in jail
    [SerializeField] int startingMoney = 1500;
    [SerializeField] int goMoney = 500;
    [SerializeField] float secondsBetweenTurns = 3;
    [Header("Player Info")]
    [SerializeField] GameObject playerInfoPrefab;
    [SerializeField] Transform playerPanel; //for the player info prefabs to become parented to
    [SerializeField] List<GameObject> playerTokenList = new List<GameObject>();


    //rolling dice info
    int[] rolledDice;
    bool rolledADouble;
    public bool RolledDouble => rolledADouble;
    public void ResetRolledDouble() => rolledADouble = false;
    int doubleRollCount;
    //tax poll
    int taxPool = 0;

    //pass over go to get money
    public int GetGoMoney => goMoney;
    public float SecondsBetweenTurns => secondsBetweenTurns;
    public List<Player> GetPlayers => playerList;

    //message system
    public delegate void UpdateMessage(string message);
    public static UpdateMessage OnUpdateMessage;

    //debug
    public bool alwaysDoubleRoll = false;
    

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Initialize();
        if (playerList[currentPlayer].playerType == Player.PlayerType.AI)
        {
            RollDice();
        }
        else
        {
            //show the roll dice button
        }
    }

    void Initialize()
    {
        //initialize the players
        for (int i = 0; i < playerList.Count; i++)
        {
            GameObject infoObject = Instantiate(playerInfoPrefab, playerPanel, false);
            PlayerInfo info = infoObject.GetComponent<PlayerInfo>();

            //randomize the player token
            int randIndex = Random.Range(0, playerTokenList.Count);
            //initialize the player
            GameObject newToken = Instantiate(playerTokenList[randIndex], gameBoard.route[0].transform.position, Quaternion.identity);

            playerList[i].Init(gameBoard.route[0], startingMoney, info, newToken);
        }
    }

    public void  RollDice() //press button from human or auto ai
    {
        bool allowedToMove = true;
        //reset last roll
        rolledDice = new int[2];
        //any roll dice and store the value
        rolledDice[0] = Random.Range(1,7);
        rolledDice[1] = Random.Range(1, 7);


        Debug.Log("Rolled: " + rolledDice[0] + " and " + rolledDice[1]);

        if(alwaysDoubleRoll)
        {
            rolledDice[0] = 1;
            rolledDice[1] = 1;
        }



        //chance for doubles
        rolledADouble = rolledDice[0] == rolledDice[1];
        //throw 3 times in a row -> jail time -> end turn

        //is in jail already
        if (playerList[currentPlayer].IsInJail)
        {
            playerList[currentPlayer].IncreaseNumTurnsInJail();

            //check if we rolled a double
            if (rolledADouble)
            {
                //get out of jail
                playerList[currentPlayer].SetOutOfJail();
                OnUpdateMessage.Invoke(playerList[currentPlayer].playerName + " <color=red>can leave jail</color>, because a double was rolled");
                doubleRollCount++;
            }
            else if (playerList[currentPlayer].NumturnsInJail >= maxTurnsInJail)
            {
                playerList[currentPlayer].SetOutOfJail();
                OnUpdateMessage.Invoke(playerList[currentPlayer].playerName + " <color=red>has been released from jail</color>, because they have been in jail for too long");
            }
            else 
            { 
                allowedToMove = false;
            }
        }
        else //not in jail
        {
            //reset double rolles
            if(!rolledADouble)
            {
                doubleRollCount = 0;
            }
            else
            { 
                doubleRollCount++;
                if(doubleRollCount >= 3)
                {
                    //move to jail
                    int indexOnBoard = MonopolyBoard.instance.route.IndexOf(playerList[currentPlayer].MyMonopolyNode);
                    playerList[currentPlayer].GoToJail(indexOnBoard);
                    OnUpdateMessage.Invoke(playerList[currentPlayer].playerName + " has been sent to jail for rolling 3 doubles in a row");
                    rolledADouble = false; //reset
                    return;
                }
            }

        }
        //can we leave jail

        //move anyhow if allowed
        if (allowedToMove)
        {
            OnUpdateMessage.Invoke(playerList[currentPlayer].playerName + " has rolled " + rolledDice[0] + " & " + rolledDice[1]);
            StartCoroutine(DelayBeforeMove(rolledDice[0] + rolledDice[1]));
        }
        else
        {
            //switch player
            OnUpdateMessage.Invoke(playerList[currentPlayer].playerName + " has to stay in jail");
            StartCoroutine(DelayBetweenSwitchPlayer());
        }
        //show or hide

    }

    IEnumerator DelayBeforeMove(int rolledDice)
    {
        yield return new WaitForSeconds(secondsBetweenTurns);
        //if we are allowed to move -> move
        gameBoard.MovePlayerToken(rolledDice, playerList[currentPlayer]);
        //else we switch player
    }

    IEnumerator DelayBetweenSwitchPlayer()
    {
        yield return new WaitForSeconds(secondsBetweenTurns);
        SwitchPlayer();
    }

    public void SwitchPlayer()
    {
        currentPlayer++;

        doubleRollCount = 0;

        if (currentPlayer >= playerList.Count)
        {
            currentPlayer = 0;
        }
        if (playerList[currentPlayer].playerType == Player.PlayerType.AI)
        {
            RollDice();
        }
        else
        {
            //show the roll dice button
        }
    }

    public int[] LastRolledDice => rolledDice;

    public void AddTaxToPool(int amount)
    {
        taxPool += amount;
    }

    public int GetTaxPool()
    {
        //return the tax pool and reset it
        int currentTaxCollected = taxPool;
        taxPool = 0;
        return currentTaxCollected;
    }
}
