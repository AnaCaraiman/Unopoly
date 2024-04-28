using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
    internal bool CanAffordNode(int price)
    { 
        return money >= price;
    }

    public void BuyProperty(MonopolyNode node)
    {
        money = node.price;
        node.SetOwner(this);
        //update ui
        myInfo.SetPlayerCash(money);
        //set ownership
        myMonopolyNodes.Add(node);
        //sort all nodes by price
        SortPropertiesByPrice();
    
    }

    void SortPropertiesByPrice()
    {        
        myMonopolyNodes.OrderBy(_node => _node.price).ToList();
     }

    internal void PayRent(int rentAmount, Player owner)
    {//don t have enough money
        if (money < rentAmount)
        { 
         //handel insufficent funds > AI
        }
        money-= rentAmount;
       owner.CollectMoney(rentAmount);
        //update ui
        myInfo.SetPlayerCash(money);

    }
    internal void PayMoney(int amount)
    {
        if (money < amount)
        {
            //handel insufficent funds > AI
        }
        money -= amount;
        myInfo.SetPlayerCash(money);
    }

}
