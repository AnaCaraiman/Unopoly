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
    MonopolyNode currentnode;
    bool isInJail;
    int numTurnsInJail = 0;
    [SerializeField] GameObject myToken;
    [SerializeField] List<MonopolyNode> myMonopolyNodes = new List<MonopolyNode>();

    //PlayerInfo
    PlayerInfo myInfo;

    //AI
    int aiMoneySavity = 300;

    //return info
    public bool IsInJail => isInJail;
    public GameObject MyToken => myToken;
    public MonopolyNode MyMonopolyNode => currentnode;
    public int ReadMoney => money;

    //message system
    public delegate void UpdateMessage(string message);
    public static UpdateMessage OnUpdateMessage;

    public void Init(MonopolyNode startNode, int startMoney, PlayerInfo info, GameObject token)
    {
        currentnode = startNode;
        money = startMoney;
        myInfo = info;
        myInfo.SetPlayerNameAndCash(playerName, money);
        myToken = token;
    }

    public void SetMyCurrentNode(MonopolyNode node)
    {
        currentnode = node;
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


    public void GoToJail(int indexOnBoard)
    {
        isInJail = true;
        //reposistion player
        //myToken.transform.position = MonopolyBoard.instance.route[10].transform.position;
        //currentnode = MonopolyBoard.instance.route[10];
        MonopolyBoard.instance.MovePlayerToken(CalculateDistanceFromJail(indexOnBoard), this);
        GameManager.instance.ResetRolledDouble();
    }

    public void SetOutOfJail()
    {
        isInJail = false;
        numTurnsInJail = 0;
    }

    int CalculateDistanceFromJail(int indexOnBoard)
    { 
        int result = 0;
        int indexOfJail = 10;
        if(indexOnBoard > indexOfJail)
        {
            result = (indexOnBoard - indexOfJail) * -1;
        }
        else
        {
            result = (indexOfJail - indexOnBoard);
        }
        return result;

    }

    public int NumturnsInJail => numTurnsInJail;

    public void IncreaseNumTurnsInJail()
    {
        numTurnsInJail++;
    }

    //street repairs
    public int[] CountHouseAndHotels()
    {
        int houses = 0;
        int hotels = 0;

        foreach (var node in myMonopolyNodes)
        {
            if (node.NumberOfHouses != 5)
            {
                houses += node.NumberOfHouses
            }
            else
            {
                hotels += 1;
            }
        }

        int[] allBuildings = new int[] { houses, hotels };
        return allBuildings;
    }
}
