using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    [Header("Game Over/ Win Info")]
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] TMP_Text winnerNameText;
    [Header("Dice")]
    [SerializeField] Dice _dice1;
    [SerializeField] Dice _dice2;

    //rolling dice info
    List<int> rolledDice;
    bool rolledADouble;
    public bool RolledDouble => rolledADouble;
    public void ResetRolledDouble() => rolledADouble = false;
    int doubleRollCount;
    bool hasRolledDice;
    public bool HasRolledDice => hasRolledDice;

    //tax poll
    int taxPool = 0;

    //pass over go to get money
    public int GetGoMoney => goMoney;
    public float SecondsBetweenTurns => secondsBetweenTurns;
    public List<Player> GetPlayers => playerList;
    public Player GetCurrentPlayer => playerList[currentPlayer];

    //message system
    public delegate void UpdateMessage(string message);
    public static UpdateMessage OnUpdateMessage;

    //Human Input Panel

    public delegate void ShowHumanPanel(bool activatePanel, bool activateRollDice, bool activateEndTurn, bool hasChanceJailCard, bool hasCommunityJailCard);
    public static ShowHumanPanel OnShowHumanPanel;

    //debug
    public bool alwaysDoubleRoll = false;


    [SerializeField] bool forceDiceRoll = false;
    [SerializeField] int dice1;
    [SerializeField] int dice2;



    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        currentPlayer = Random.Range(0, playerList.Count);
        gameOverPanel.SetActive(false);
        Initialize();
        if (playerList[currentPlayer].playerType == Player.PlayerType.AI)
        {
            //RollDice();
            RollPhysicalDice();
        }
        else
        {
            //show the roll dice button
            OnShowHumanPanel.Invoke(true, true, false, false, false);
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
        playerList[currentPlayer].ActivateSelector(true);

        if (playerList[currentPlayer].playerType == Player.PlayerType.Human)
        {
            bool jail1 = playerList[currentPlayer].HasChanceJailFreeCard;
            bool jail2 = playerList[currentPlayer].HasCommunityJailFreeCard;
            OnShowHumanPanel.Invoke(true, true, false, jail1, jail2);
        }
        else
        {
            bool jail1 = playerList[currentPlayer].HasChanceJailFreeCard;
            bool jail2 = playerList[currentPlayer].HasCommunityJailFreeCard;
            OnShowHumanPanel.Invoke(false, false, false, jail1, jail2);
        }
    }

    void RollPhysicalDice()
    {
        CheckForJailFree();
        rolledDice.Clear();
        _dice1.RollDice();
        _dice2.RollDice();
    }

    void CheckForJailFree()
    {
        //jail free card
        if (playerList[currentPlayer].IsInJail && playerList[currentPlayer].playerType == Player.PlayerType.AI)
        {
            if (playerList[currentPlayer].HasChanceJailFreeCard)
            {
                playerList[currentPlayer].UseChanceJailFreeCard();
            }
            else if (playerList[currentPlayer].HasCommunityJailFreeCard)
            {
                playerList[currentPlayer].UseCommunityJailFreeCard();
            }

        }
    }

    public void ReportDiceRolled(int diceValue)
    {
        rolledDice.Add(diceValue);
        if(rolledDice.Count == 2)
        {
            RollDice();
        }
    }
    
    public void  RollDice() //press button from human or auto ai
    {
        bool allowedToMove = true;
        hasRolledDice = true;

        //reset last roll
        //rolledDice = new int[2];
        //any roll dice and store the value
        //rolledDice[0] = Random.Range(1,7);
        //rolledDice[1] = Random.Range(1, 7);


        Debug.Log("Rolled: " + rolledDice[0] + " and " + rolledDice[1]);

        //if(alwaysDoubleRoll)
        //{
            //rolledDice[0] = 1;
            //rolledDice[1] = 1;
        //}
        //if(forceDiceRoll)
        //{   rolledDice[0] = dice1;
            //rolledDice[1] = dice2;
              
        //}



        //chance for doubles
        rolledADouble = rolledDice[0] == rolledDice[1];
        //throw 3 times in a row -> jail time -> end turn

        //is in jail already
        if (playerList[currentPlayer].IsInJail)
        if (playerList[currentPlayer].IsInJail)
        {
            playerList[currentPlayer].IncreaseNumTurnsInJail();

            //check if we rolled a double
            if (rolledADouble)
            {
                //get out of jail
                playerList[currentPlayer].SetOutOfJail();
                OnUpdateMessage.Invoke(playerList[currentPlayer].name + " <color=red>can leave jail</color>, because a double was rolled");
                doubleRollCount++;
            }
            else if (playerList[currentPlayer].NumturnsInJail >= maxTurnsInJail)
            {
                playerList[currentPlayer].SetOutOfJail();
                OnUpdateMessage.Invoke(playerList[currentPlayer].name + " <color=red>has been released from jail</color>, because they have been in jail for too long");
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
                    OnUpdateMessage.Invoke(playerList[currentPlayer].name + " has been sent to jail for rolling 3 doubles in a row");
                    rolledADouble = false; //reset
                    return;
                }
            }

        }
        //can we leave jail

        //move anyhow if allowed
        if (allowedToMove)
        {
            OnUpdateMessage.Invoke(playerList[currentPlayer].name + " has rolled " + rolledDice[0] + " & " + rolledDice[1]);
            StartCoroutine(DelayBeforeMove(rolledDice[0] + rolledDice[1]));
        }
        else
        {
            //switch player
            OnUpdateMessage.Invoke(playerList[currentPlayer].name + " has to stay in jail");
            StartCoroutine(DelayBetweenSwitchPlayer());
        }


        //show or hide
        if (playerList[currentPlayer].playerType == Player.PlayerType.Human)
        {
            bool jail1 = playerList[currentPlayer].HasChanceJailFreeCard;
            bool jail2 = playerList[currentPlayer].HasCommunityJailFreeCard;
            OnShowHumanPanel.Invoke(true, false, false, jail1, jail2); 
        }


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

        //reset dice
        hasRolledDice = false;

        doubleRollCount = 0;

        if (currentPlayer >= playerList.Count)
        {
            currentPlayer = 0;
        }

        DeactivateArrow();
        playerList[currentPlayer].ActivateSelector(true);

        if (playerList[currentPlayer].playerType == Player.PlayerType.AI)
        {
            //RollDice();
            RollPhysicalDice();
            OnShowHumanPanel.Invoke(false, false, false, false, false);
        }
        else //human
        {
            bool jail1 = playerList[currentPlayer].HasChanceJailFreeCard;
            bool jail2 = playerList[currentPlayer].HasCommunityJailFreeCard;
            OnShowHumanPanel.Invoke(true, true, false, jail1, jail2);
        }
    }

    public List<int> LastRolledDice => rolledDice;

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

    public void RemovePlayer(Player player)
    {
        playerList.Remove(player);
        //check - game over
        CheckForGameOver();
    }

    void CheckForGameOver()
    {
        if(playerList.Count == 1)
        {
            //we have a winner
            Debug.Log(playerList[0].name + " has won the game!");
            OnUpdateMessage.Invoke(playerList[0].name + " has won the game!");
            //stop the game loop

            //show ui
            gameOverPanel.SetActive(true);
            winnerNameText.text = playerList[0].name;
        }
    }

    void DeactivateArrow()
    {
        foreach (var player in playerList)
        {
            player.ActivateSelector(false);
        }
    }

    //continue game
    public void Continue()
    {
        if (playerList.Count > 1)
        {
            Invoke("ContinueGame", SecondsBetweenTurns);
        }
    }


    void ContinueGame()
    {
        //check if we rolled a double
        if (RolledDouble)
        {
            //roll again
            //RollDice();
            RollPhysicalDice();
        }
        else
        {
            //switch player
            SwitchPlayer();

        }
    }

    public void HumanBankrupt()
    {
        playerList[currentPlayer].Bankrupt();
    }

    //jail free cards buttons
    public void UseJail1Card()
    {
        playerList[currentPlayer].UseChanceJailFreeCard();
    }

    public void UseJail2Card()
    {
        playerList[currentPlayer].UseCommunityJailFreeCard();
    }
}
