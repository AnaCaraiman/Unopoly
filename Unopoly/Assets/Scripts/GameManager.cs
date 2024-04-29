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
    [Header("Player Info")]
    [SerializeField] GameObject playerInfoPrefab;
    [SerializeField] Transform playerPanel; //for the player info prefabs to become parented to
    [SerializeField] List<GameObject> playerTokenList = new List<GameObject>();

    //rolling dice info
    int[] rolledDice;
    bool rolledADouble;
    public bool RolledDouble => rolledADouble;
    int doubleRollCount;
    //tax poll
    int taxPool = 0;

    //pass over go to get money
    public int GetGoMoney => goMoney;

    //debug
    public bool alwaysRollDouble = false;

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
        rolledDice[1] = Random.Range(1,7);
        Debug.Log("Rolled: " + rolledDice[0] + " and " + rolledDice[1]);


        //debug
        if (alwaysRollDouble)
        {
            rolledDice[0] = 2;
            rolledDice[1] = 2;
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
                doubleRollCount++;
            }
            else if (playerList[currentPlayer].NumturnsInJail >= maxTurnsInJail)
            {
                playerList[currentPlayer].SetOutOfJail();
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
                    rolledADouble = false; //reset
                    return;
                }
            }

        }
        //can we leave jail

        //move anyhow if allowed
        rolledDice[0] = 5;
        rolledDice[1] = 5;
        if (allowedToMove)
        {
            StartCoroutine(DelayBeforeMove(rolledDice[0] + rolledDice[1]));
        }
        else
        {
            //switch player
            Debug.Log("we can not move");
            SwitchPlayer();
        }
        //show or hide

    }

    IEnumerator DelayBeforeMove(int rolledDice)
    {
        yield return new WaitForSeconds(2f);
        //if we are allowed to move -> move
        gameBoard.MovePlayerToken(rolledDice, playerList[currentPlayer]);
        //else we switch player
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
