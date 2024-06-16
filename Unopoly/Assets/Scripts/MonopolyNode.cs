using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using UnityEngine.UI;

public enum MonopolyNodeType
{
    Property,
    Utility,
    Railroad,
    Tax,
    Chance,
    CommunityChest,
    Go, 
    Jail,
    FreeParking,
    GoToJail
}

public class MonopolyNode : MonoBehaviour
{
    public MonopolyNodeType monopolyNodeType;
    public Image propertyColorField;
    [Header("Property Name")]
    [SerializeField] internal new string name;
    [SerializeField] TMP_Text nameText;
    [Header("Property Price")]
    public int price;
    public int houseCost;
    [SerializeField] TMP_Text priceText;
    [Header("Property Rent")]
    [SerializeField] bool calculateRentAuto;
    [SerializeField] int currentRent;
    [SerializeField] internal int baseRent;
    [SerializeField] internal List<int> rentWithHouses = new List<int>();
    int numberOfHouses;
    public int NumberOfHouses => numberOfHouses;
    [SerializeField] GameObject[] houses;
    [SerializeField] GameObject hotel;
    [Header("Property Mortgage")]
    [SerializeField] GameObject mortgageImage;
    [SerializeField] GameObject propertyImage;
    [SerializeField] bool isMortgaged;
    [SerializeField] int mortgageValue;
    [Header("Property Owner")]
    [SerializeField] GameObject ownerBar;
    [SerializeField] TMP_Text ownerText;
    Player owner;

    //message system
    public delegate void UpdateMessage(string message);
    public static UpdateMessage OnUpdateMessage;

    public Player Owner => owner;
    public void SetOwner(Player newOwner) 
    {
        owner = newOwner;
        OnOwnerUpdated();
    }



    //drag a community card
    public delegate void DrawCommunityCard(Player player);
    public static DrawCommunityCard OnDrawCommunityCard;
    //drag a chance card
    public delegate void DrawChanceCard(Player player);
    public static DrawChanceCard OnDrawChanceCard;

    //Human Input Panel

    public delegate void ShowHumanPanel(bool activatePanel, bool activateRollDice, bool activateEndTurn, bool hasChanceJailCard, bool hasCommunityJailCard);
    public static ShowHumanPanel OnShowHumanPanel;

    //property buy panel
    public delegate void ShowBuyPropertyBuyPanel(MonopolyNode node, Player player);
    public static ShowBuyPropertyBuyPanel OnShowPropertyBuyPanel;

    //railroad buy panel
    public delegate void ShowRailroadBuyPanel(MonopolyNode node, Player player);
    public static ShowRailroadBuyPanel OnShowRailroadBuyPanel;

    //utility buy panel
    public delegate void ShowUtilityBuyPanel(MonopolyNode node, Player player);
    public static ShowUtilityBuyPanel OnShowUtilityBuyPanel;

    void OnValidate()
    {
        if (nameText != null)
        {
            nameText.text = name; 
        }
        
        //calculation rent
        if (calculateRentAuto)
        {
            if(monopolyNodeType == MonopolyNodeType.Property)
            {
                if (baseRent > 0)
                {
                    price = 3 * (baseRent * 10);
                    //mortgage price
                    mortgageValue = price / 2;
                    rentWithHouses.Clear();

                    rentWithHouses.Add(baseRent * 5);
                    rentWithHouses.Add(baseRent * 5 * 3);
                    rentWithHouses.Add(baseRent * 5 * 9);
                    rentWithHouses.Add(baseRent * 5 * 16);
                    rentWithHouses.Add(baseRent * 5 * 25);
                    
                }
                else if (baseRent <= 0)
                {
                    price = 0;
                    baseRent = 0;
                    rentWithHouses.Clear();
                    mortgageValue = 0;
                }
            }
            if(monopolyNodeType == MonopolyNodeType.Utility) {
                mortgageValue = price / 2;
            }

            if (monopolyNodeType == MonopolyNodeType.Railroad) {
                mortgageValue = price / 2;
            }
        }

        if (priceText != null)
        {
            priceText.text = price + "RON";
        }
        //update the owner
        OnOwnerUpdated();
        UnMortgageProperty();
        //isMortgaged = false;
    }

    public void UpdateColorField(Color color)
    { if (propertyColorField != null) 
        { 
        propertyColorField.color = color; 
        }
    }
    //mortgage content
    public int MortgageProperty()
    {
        isMortgaged = true;
        if (mortgageImage != null)
        {
            mortgageImage.SetActive(true);
        }
        if (propertyImage != null)
        {
            propertyImage.SetActive(false);
        }
        return mortgageValue;
    }

    public void UnMortgageProperty()
    {
        isMortgaged = false;
        if(mortgageImage != null)
        {
            mortgageImage.SetActive(false);
        }
        if(propertyImage != null)
        {
            propertyImage.SetActive(true);
        }
    }

    public bool IsMortgaged => isMortgaged;
    public int MortgageValue => mortgageValue;

    //update owner
    public void OnOwnerUpdated()
    {
        if (ownerBar != null)
        {
            if(owner != null)
            {
                ownerBar.SetActive(true);
                ownerText.text = owner.name;
            }
            else
            {
                ownerBar.SetActive(false);
                ownerText.text = "";
            }
        }
    }

    public void PlayerLandedOnNode(Player player)
    {
        bool playerIsHuman = player.playerType == Player.PlayerType.Human;
        bool continueTurn = true;
        //check for node type

        switch (monopolyNodeType)
        {
            case MonopolyNodeType.Property:
                if (!playerIsHuman)//AI
                {
                    // if it owned && if we not are the owner && if it is not mortgaged
                    if (owner != null && owner != player && !isMortgaged)
                    { //pay rent to somebody


                        //calculate the rent
                        int rentToPay = CalculatePropertyRent();

                        //pay the rent to the owner
                        player.PayRent(rentToPay, owner);

                        //show a message about what happened
                        OnUpdateMessage.Invoke(player.name + " pays rent of: " + rentToPay + " to " + owner.name);
                    }
                    else if (owner == null && player.CanAffordNode(price) )
                    {  
                        // buy th node
                        //Debug.Log("Player can afford the property");
                        OnUpdateMessage.Invoke(player.name + " buys " + this.name);
                        player.BuyProperty(this);
                        OnOwnerUpdated();

                        //show a mesage

                    }
                    else 
                    { //is unowned and cant afford it 
                        
                    }
                }
                else//human
                { 
                    // if it owned && if we not are the owner && if it is not mortgaged
                    if (owner != null && owner != player && !isMortgaged)
                    { //pay rent to somebody
                      //calculate the rent
                      int rentToPay = CalculatePropertyRent();
                        //pay the rent to the owner
                        player.PayRent(rentToPay, owner);
                      //show a message about what happened
                    }
                    else if (owner == null)
                    {   
                        // show buy interface for propwert
                        OnShowPropertyBuyPanel.Invoke(this, player);
                    }
                    else
                    { //is unowned and cant afford it 

                    }

                }
            break;

            case MonopolyNodeType.Utility:
                if (!playerIsHuman)//AI
                {
                    // if it owned && if we not are the owner && if it is not mortgaged
                    if (owner != null && owner != player && !isMortgaged)
                    { //pay rent to somebody


                        //calculate the rent
                        //Debug.Log("Player Might pay rent && owner is:" + owner.playerName);
                        int rentToPay = CalculateUtilityRent();
                        currentRent = rentToPay;

                        //pay the rent to the owner
                        player.PayRent(rentToPay, owner);

                        //show a message about what happened
                        OnUpdateMessage.Invoke(player.name + " pays Utility rent of: " + rentToPay + "to" + owner.name + "for landing on" + name + "node");
                    }
                    else if (owner == null && player.CanAffordNode(price))
                    {   // buy th node

                        //Debug.Log("Player can afford the property");
                        OnUpdateMessage.Invoke(player.name + " buys Utility" + this.name);
                        player.BuyProperty(this);
                        OnOwnerUpdated();

                        //show a mesage

                    }
                    else
                    { //is unowned and cant afford it 

                    }
                }
                else//human
                {
                    // if it owned && if we not are the owner && if it is not mortgaged
                    if (owner != null && owner != player && !isMortgaged)
                    {
                        int rentToPay = CalculateUtilityRent();
                        currentRent = rentToPay;

                        //pay the rent to the owner
                        player.PayRent(rentToPay, owner);
                        //show a message about what happened
                    }
                    else if (owner == null)
                    {
                        
                        // show buy interface for utility
                        OnShowUtilityBuyPanel.Invoke(this, player);



                    }
                    else
                    { //is unowned and cant afford it 

                    }

                }
                break;

            case MonopolyNodeType.Railroad:
                if (!playerIsHuman)//AI
                {
                    // if it owned && if we not are the owner && if it is not mortgaged
                    if (owner != null && owner != player && !isMortgaged)
                    { //pay rent to somebody


                        //calculate the rent
                        int rentToPay = CalculateRailroadRent();
                        currentRent = rentToPay;


                        //pay the rent to the owner
                        player.PayRent(rentToPay, owner);

                        //show a message about what happened
                        OnUpdateMessage.Invoke(player.name + " pays Railroad rent of: " + rentToPay + " to " + owner.name);
                    }
                    else if (owner == null && player.CanAffordNode(price))
                    {   // buy th node

                        //Debug.Log("Player can afford the property");
                        player.BuyProperty(this);
                        OnOwnerUpdated();

                        //show a mesage

                    }
                    else
                    { //is unowned and cant afford it 

                    }
                }
                else//human
                {
                    // if it owned && if we not are the owner && if it is not mortgaged
                    if (owner != null && owner != player && !isMortgaged)
                    { //calculate the rent
                        int rentToPay = CalculateRailroadRent();
                        currentRent = rentToPay;


                        //pay the rent to the owner
                        player.PayRent(rentToPay, owner);
                        //show a message about what happened
                    }
                    else if (owner == null)
                    {
                        // show buy interface for railroad
                        OnShowRailroadBuyPanel.Invoke(this, player);



                    }
                    else
                    { //is unowned and cant afford it 

                    }

                }
                break;
            case MonopolyNodeType.Tax:
                GameManager.instance.AddTaxToPool(price);
                player.PayMoney(price);
                //show a message about what happened
                OnUpdateMessage.Invoke(player.name + " pays tax of: " + price + " to the pool");
                break;
            case MonopolyNodeType.FreeParking:
                int tax = GameManager.instance.GetTaxPool();
                player.CollectMoney(tax);
                //show a message about what happened
                OnUpdateMessage.Invoke(player.name + " collects tax of: " + tax + " from the pool");

                break;
            case MonopolyNodeType.GoToJail:
                int indexOnBoard = MonopolyBoard.instance.route.IndexOf(player.MyMonopolyNode);
                player.GoToJail(indexOnBoard);
                OnUpdateMessage.Invoke(player.name + " has to go to jail");
                continueTurn = false;

                break;
            case MonopolyNodeType.Chance:
                OnDrawChanceCard.Invoke(player);
                continueTurn = false;

                break;
            case MonopolyNodeType.CommunityChest:
                OnDrawCommunityCard.Invoke(player);
                continueTurn = false;
                break;
        }
        //stop here if needed
        if(!continueTurn)
        {
            return;
        }
        
        
        
        //continue
        if(!playerIsHuman)
        {
            //Invoke("ContinueGame", GameManager.instance.SecondsBetweenTurns);
            player.ChangeState(Player.AiStates.TRADING);
        }
        else
        {
            bool canEndTurn = !GameManager.instance.RolledDouble && player.ReadMoney >= 0;
            bool canRollDice = GameManager.instance.RolledDouble && player.ReadMoney >= 0;
            bool jail1 = player.HasChanceJailFreeCard;
            bool jail2 = player.HasCommunityJailFreeCard;
            //SHOW UI
            OnShowHumanPanel.Invoke(true, canRollDice, canEndTurn, jail1, jail2);
        }
    }

//    void ContinueGame()
 //   {
 //       //check if we rolled a double
 //       if (GameManager.instance.RolledDouble)
 //       {
 //           //roll again
 //           GameManager.instance.RollDice();
 //       }
 //       else
 //       {
 //           //not a double, switch player
 //           GameManager.instance.SwitchPlayer();
 //
 //       }
 //   }

    int CalculatePropertyRent()
    {
        switch (numberOfHouses) 
        {
            case 0:
                //check if owner has the full set of this nodes
                var (list,allsame)=MonopolyBoard.instance.PlayerHasAllNodesofSet(this);
                if (allsame)
                {
                    currentRent = baseRent * 2;
                }
                else 
                {
                    currentRent = baseRent; 
                }
                break;
            case 1:
                currentRent = rentWithHouses[0];
                break;
            case 2:
                currentRent = rentWithHouses[1];
                break;
            case 3:
                currentRent = rentWithHouses[2];
                break;
            case 4:
                currentRent = rentWithHouses[3];
                break;
            case 5://hotel
                currentRent = rentWithHouses[4];
                break;
        }
        return currentRent;
    }

    int CalculateUtilityRent()
    {
        int[] lastRolledDice = GameManager.instance.LastRolledDice;
        
        int result = 0;

        var(list, allsame) = MonopolyBoard.instance.PlayerHasAllNodesofSet(this);
        if (allsame)
        {
            result = (lastRolledDice[0] + lastRolledDice[1]) * 10;
        }
        else
        {
            result = (lastRolledDice[0] + lastRolledDice[1]) * 4;
        }

        return result;
    }

   int CalculateRailroadRent()
    {
        int result = 0;
        var (list, allsame) = MonopolyBoard.instance.PlayerHasAllNodesofSet(this);

        int amount = 0;
        foreach (var item in list)
        {
            amount += (item.owner == this.owner) ? 1 : 0;
        }

        result = baseRent * (int)Mathf.Pow(2, amount-1); 

        return result;
    }

    void VisualizeHouses()
    {
        switch(numberOfHouses)
        {
            case 0:
                houses[0].SetActive(false);
                houses[1].SetActive(false);
                houses[2].SetActive(false);
                houses[3].SetActive(false);
                hotel.SetActive(false);
                break;

            case 1:
                houses[0].SetActive(true);
                houses[1].SetActive(false);
                houses[2].SetActive(false);
                houses[3].SetActive(false);
                hotel.SetActive(false);
                break;

            case 2:
                houses[0].SetActive(true);
                houses[1].SetActive(true);
                houses[2].SetActive(false);
                houses[3].SetActive(false);
                hotel.SetActive(false);
                break;

            case 3:
                houses[0].SetActive(true);
                houses[1].SetActive(true);
                houses[2].SetActive(true);
                houses[3].SetActive(false);
                hotel.SetActive(false);
                break;

            case 4:
                houses[0].SetActive(true);
                houses[1].SetActive(true);
                houses[2].SetActive(true);
                houses[3].SetActive(true);
                hotel.SetActive(false);
                break;

            case 5:
                houses[0].SetActive(false);
                houses[1].SetActive(false);
                houses[2].SetActive(false);
                houses[3].SetActive(false);
                hotel.SetActive(true);
                break;

        }
    }

    public void BuildHouseOrHotel()
    {
        if(monopolyNodeType == MonopolyNodeType.Property)
        {
            numberOfHouses++;
            VisualizeHouses();
        }
    }
    public int SellHouseOrHotel()
    {
        if (monopolyNodeType == MonopolyNodeType.Property && numberOfHouses > 0)
        {
            numberOfHouses--;
            VisualizeHouses();
            return houseCost / 2;
        }
        return 0;
    }

    public void ResetNode()
    {
        if(isMortgaged)
        {
            propertyImage.SetActive(true );
            mortgageImage.SetActive(false);
            isMortgaged = false;
        }
        ///reset houses and hotel
        if(monopolyNodeType == MonopolyNodeType.Property)
        {
            numberOfHouses = 0;
            VisualizeHouses();
        }
        //reset the owner

        //remove propery from owner
        owner.RemoveProperty(this);
        owner.name = "";
        owner.ActivateSelector(false);
        owner = null;
        OnOwnerUpdated();
    }

    //TRADING SYSTEM

    //change node owner
    public void ChangeOwner(Player newOwner)
    {
        owner.RemoveProperty(this);
        newOwner.AddProperty(this);
        SetOwner(newOwner);
    }
}
