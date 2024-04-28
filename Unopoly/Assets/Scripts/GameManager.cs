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
    int doubleRollCount;

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
        //reset last roll
        rolledDice = new int[2];
        //any roll dice and store the value
        rolledDice[0] = Random.Range(1, 7);
        rolledDice[1] = Random.Range(1, 7);
        Debug.Log("Rolled: " + rolledDice[0] + " and " + rolledDice[1]);
        //chance for doubles
        rolledADouble = rolledDice[0] == rolledDice[1];
        //throw 3 times in a row -> jail time -> end turn

        //is in jail already

        //can we leave jail

        //move anyhow if allowed
        StartCoroutine(DelayBeforeMove(rolledDice[0] + rolledDice[1]));
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

}
