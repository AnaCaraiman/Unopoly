using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Player
{
    public enum PlayerType
    {
        Human,
        AI
    }
    //Human
    public PlayerType playerType;
    public string playerName;
    int money;
    MonopolyNode currentNode;
    bool isInJail;
    int numberOfTurnsInJail;
    [SerializeField] GameObject myToken;
    [SerializeField] List<MonopolyNode> myMonopolyNodes = new List<MonopolyNode>();

    //PlayerInfo
    PlayerInfo myInfo;

    //AI
    int aiMoneySavity = 300;

    //return info
    public bool IsInJail => isInJail;
    public GameObject MyToken => myToken;
    public MonopolyNode MyMonopolyNode => currentNode;

    public void Init(MonopolyNode startNode, int startMoney, PlayerInfo info, GameObject token)
    {
        currentNode = startNode;
        money = startMoney;
        myInfo = info;
        myInfo.SetPlayerNameAndCash(playerName, money);
        myToken = token;
    }

    public void SetMyCurrentNode(MonopolyNode node)
    {
        currentNode = node;
        node.PlayerLandedOnNode(this);
    }

    public void CollectMoney(int amount)
    {
        money += amount;
        myInfo.SetPlayerCash(money);
    }

}
