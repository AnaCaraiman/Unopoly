using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UIElements;
using JetBrains.Annotations;

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
    public string name;
    int money;
    MonopolyNode currentnode;
    bool isInJail;
    int numTurnsInJail = 0;
    [SerializeField] GameObject myToken;
    [SerializeField] List<MonopolyNode> myMonopolyNodes = new List<MonopolyNode>();
    public List<MonopolyNode> GetMonopolyNodes => myMonopolyNodes;

    bool hasChanceJailFreeCard, hasCommunityJailFreeCard;

    public bool HasChanceJailFreeCard => hasChanceJailFreeCard;
    public bool HasCommunityJailFreeCard => hasCommunityJailFreeCard;

    //PlayerInfo
    PlayerInfo myInfo;

    //AI
    int aiMoneySavity = 300;

    //AI states
    public enum AiStates
    {
        IDLE,
        TRADING
    }

    public AiStates aiState;

    //Human Input Panel
    public delegate void ShowHumanPanel(bool activatePanel, bool activateRollDice, bool activateEndTurn, bool hasChanceJailCard, bool hasCommunityJailCard);
    public static ShowHumanPanel OnShowHumanPanel;

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
        myInfo.SetPlayerNameAndCash(name, money);
        myToken = token;
        myInfo.ActivateArrow(false);
    }

    public void SetMyCurrentNode(MonopolyNode newNode)
    {
        currentnode = newNode;
        newNode.PlayerLandedOnNode(this);

        //ai player
        if(playerType == PlayerType.AI)
        {
            //CHECH IF I CAN BUY A HOUSE
            ChecckIfPlayerHasASet();
            //check for umortgaged properties
            UnmortgageProperties();
            //check if he could trade
            //TradingSystem.instance.FindMissingProperty(this);

        } 
    }

    public void CollectMoney(int amount)
    {
        money += amount;
        myInfo.SetPlayerCash(money);

        if (playerType == PlayerType.HUMAN && GameManager.instance.GetCurrentPlayer == this)
        {
            bool canEndTurn = !GameManager.instance.RolledADouble && ReadMoney >= 0 && GameManager.instance.HasRolledDice;
            bool canRollDice = (GameManager.instance.RolledADouble && ReadMoney >= 0) || (!GameManager.instance.HasRolledDice && ReadMoney >= 0);
            //SHOW UI
            OnShowHumanPanel.Invoke(true, canRollDice, canEndTurn, hasChanceJailFreeCard, hasCommunityJailFreeCard);
        }
    }
    internal bool CanAffordNode(int price)
    { 
        return money >= price;
    }

    public void BuyProperty(MonopolyNode node)
    {
        money -= node.price;
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
        myMonopolyNodes = myMonopolyNodes.OrderBy(_node => _node.price).ToList();
     }

    internal void PayRent(int rentAmount, Player owner)
    {//don t have enough money
        if (money < rentAmount)
        {
            if (playerType == PlayerType.AI)
            {
                //handel insufficent funds > AI
                HandleInsuficientFunds(rentAmount);
            }
            else
            {
                //disable human tunr and roll dice
                OnShowHumanPanel.Invoke(true, false, false, hasChanceJailFreeCard, hasCommunityJailFreeCard);

            }
        }
    
        money -= rentAmount;
        owner.CollectMoney(rentAmount);
        //update ui
        myInfo.SetPlayerCash(money);

    }
    internal void PayMoney(int amount)
    {
        if (money < amount)
        {
            if (playerType == PlayerType.AI)
            {
                //handel insufficent funds > AI
                HandleInsuficientFunds(amount);
            }
            else
            {
                //disable human turn and roll dice
                OnShowHumanPanel.Invoke(true, false, false);
            
            }
        }
        money -= amount;
        myInfo.SetPlayerCash(money);

        if (playerType == PlayerType.Human && GameManager.instance.GetCurrentPlayer == this)
        {
            bool canEndTurn = !GameManager.instance.RolledDouble && ReadMoney >= 0;
            bool canRollDice = GameManager.instance.RolledDouble && ReadMoney >= 0;
            //SHOW UI
            OnShowHumanPanel.Invoke(true, canRollDice, canEndTurn, hasChanceJailFreeCard, hasCommunityJailFreeCard);
        }
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
                houses += node.NumberOfHouses;
            }
            else
            {
                hotels += 1;
            }
        }

        int[] allBuildings = new int[] { houses, hotels };
        return allBuildings;
    }

    //handle insufficent funds

    public void HandleInsuficientFunds(int amountToPay)
    {
        int housesToSell = 0; //houses to sell
        int allHouses = 0;
        int propertiesToMortgage = 0;
        int allPropertiesToMortgage = 0;

        //count all houses 
        foreach (var node in myMonopolyNodes)
        {
            allHouses += node.NumberOfHouses;
        }

        //loop through all properties and try to sell as much as needed
        while (money < amountToPay && allHouses > 0)
        {
            foreach (var node in myMonopolyNodes)
            {
                housesToSell = node.NumberOfHouses;
                if (housesToSell > 0)
                {
                    CollectMoney(node.SellHouseOrHotel());
                    allHouses--;
                    if (money >= amountToPay)
                    {
                        return;
                    }
                }
            }
        }
        //mortgage 
        foreach (var node in myMonopolyNodes)
        {
            allPropertiesToMortgage += (!node.IsMortgaged) ? 1 : 0;
        }
        //loop through all properties and try to mortgage as much as needed
        while (money < amountToPay && propertiesToMortgage > 0)
        {
            foreach (var node in myMonopolyNodes)
            {
                propertiesToMortgage = (!node.IsMortgaged) ? 1 : 0;
                if (propertiesToMortgage > 0)
                {
                    CollectMoney(node.MortgageProperty());
                    allPropertiesToMortgage--;
                    if (money >= amountToPay)
                    {
                        return;
                    }
                }
            }
        }
        if(playerType == PlayerType.AI)
        {
            Bankrupt();
        }

    }

    //bankrupt - game over
    void Bankrupt()
    {
        //take out of the game

        //send a message to the message system
        OnUpdateMessage?.Invoke(name + "<color = red> is bankrupt! </color>");
        //clear all what the player owns
        for(int i = myMonopolyNodes.Count-1 ; i >= 0; i--)
        {
            myMonopolyNodes[i].ResetNode();
        }

        if(hasChanceFreeCard)
        {
            ChanceField.instance.AddBackJailFreeCard();
        }

        if(hasCommunityFreeCard)
        {
            CommunityChest.instance.AddBackJailFreeCard();
        }

        //remove the player
        GameManager.instance.RemovePlayer(this);
    }


    void UnmortgageProperties()
    {
        //for ai
        foreach (var node in myMonopolyNodes)
        {
            if (node.IsMortgaged)
            {
                int cost = node.MortgageValue + (int)(node.MortgageValue * 0.1f);  //10% interest
                if( money >= aiMoneySavity + cost)
                {
                    PayMoney(cost);
                    node.UnMortgageProperty();
                }
            }
        }
    }

        //chech if a player has a property set
    void ChecckIfPlayerHasASet()
    {
        //call it only once per set
        List<MonopolyNode> processedSet = null;
        //store and compare


        foreach (var node in myMonopolyNodes)
        {
            var (list, allSame) = MonopolyBoard.instance.PlayerHasAllNodesofSet(node);

            if (!allSame)
            {
                continue;
            }

            List<MonopolyNode> nodeSet = list;
            if (nodeSet != null && nodeSet != processedSet)
            {
                bool hasMorgagedNode = nodeSet.Any(node => node.IsMortgaged) ? true : false;
                if (!hasMorgagedNode)
                {
                    if (nodeSet[0].monopolyNodeType == MonopolyNodeType.Property)
                    {
                        //we could build a house on the set
                        BuildHouseOrHotelEvenly(nodeSet);
                        //update process set
                        processedSet = nodeSet;
                    }
                }
            }
        }
    }

    internal void BuildHouseOrHotelEvenly(List<MonopolyNode> nodesToBuildOn)
    {
        int minHouses = int.MaxValue;
        int maxHouses = int.MinValue;
        //get min and max numbers of houses currently on the propertys
        foreach (var node in nodesToBuildOn)
        {
            int numOfHouses = node.NumberOfHouses;
            if (numOfHouses < minHouses)
            {
                minHouses = numOfHouses;
            }
            if (numOfHouses > maxHouses && numOfHouses < 5)
            {
                maxHouses = numOfHouses;
            }
        }

        foreach (var node in nodesToBuildOn)
        {
            if (node.NumberOfHouses == minHouses && node.NumberOfHouses < 5 && CanAffordHouse(node.houseCost))
            {
                node.BuildHouseOrHotel();
                PayMoney(node.houseCost);
                break;
            }
        }
    }
    internal void SellHouseEvenly(List<MonopolyNode> nodesToSellFrom)
    {
        int minHouses = int.MaxValue;
        bool houseSold = false;
        foreach (var node in nodesToSellFrom)
        {  
            minHouses = Mathf.Min(minHouses, node.NumberOfHouses);
        
        }
        //sell house
        for (int i=nodesToSellFrom.Count-1; i>=0; i--)
        {
            if (nodesToSellFrom[i].NumberOfHouses > minHouses)
            {
               CollectMoney( nodesToSellFrom[i].SellHouseOrHotel());
               houseSold = true;
               break;
            }

        }
        if(!houseSold)
        {
            CollectMoney(nodesToSellFrom[nodesToSellFrom.Count-1].SellHouseOrHotel());
        }

    }


   internal bool CanAffordHouse(int price)
    {
        if (playerType == PlayerType.AI)
        {
            return (money - aiMoneySavity) >= price;
        }

        return money >= price;  //human only
    }

    public void ActivateSelector(bool active)
    {
        myInfo.ActivateArrow(active);
    }

    //remove and add nodes
    public void AddProperty(MonopolyNode node)
    {
        myMonopolyNodes.Add(node);
        SortPropertiesByPrice();
    }
    public void RemoveProperty(MonopolyNode node)
    {
        myMonopolyNodes.Remove(node);
        SortPropertiesByPrice();
    }

    public void ChangeState(AiStates state)
    {
        if (playerType == PlayerType.Human)
        {
            return;
        }

        aiState = state;
        switch (aiState)
        {
            case AiStates.IDLE:
                {
                    //continue game
                    GameManager.instance.Continue();
                }
                break;
            case AiStates.TRADING:
                {
                    //hold until continue
                    TradingSystem.instance.FindMissingProperty(this);
                }
                break;
        }

        //jail free card
        public void AddChanceJailFreeCard()
        {
            hasChanceJailFreeCard = true;
        }

        public void AddCommunityJailFreeCard()
        {
            hasCommunityJailFreeCard = true;
        }

        public void UseCommunityJailFreeCard()//jail2
        {
            if(!IsInJail)
            {
                return;
            }
            hasCommunityJailFreeCard = false;
            SetOutOfJail();
            CommunityChest.instance.AddBackJailFreeCard();
            OnUpdateMessage.Invoke(name + " used a Jail Free Card.");
        }

        public void UseChanceJailFreeCard()//jail1
        {
            if (!IsInJail)
            {
                return;
            }
            hasChanceJailFreeCard = false;
            SetOutOfJail();
            ChanceField.instance.AddBackJailFreeCard();
            OnUpdateMessage.Invoke(name + " used a Jail Free Card.");
        }
    }
