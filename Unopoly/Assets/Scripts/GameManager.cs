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


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Initialize();
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

}
