using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;
using System.Linq;

public class ManagePropertyUI : MonoBehaviour
{
    [SerializeField] Transform cardHolder;//Horizontal layout 
    [SerializeField] GameObject cardPrefab;
    [SerializeField] Button buyHouseButton, sellHouseButton;
    [SerializeField] TMP_Text buyHousePriceText, sellHousePriceText;
    Player playerReference;

    List<MonopolyNode> nodesInSet = new List<MonopolyNode>();
    List<GameObject> cardsInSet = new List<GameObject>();
    [SerializeField] GameObject buttonBox;

    //THIS Property is only for 1 specific card set
    public void SetProperty(List<MonopolyNode> nodes, Player owner)
    {
        playerReference = owner;
        nodesInSet.AddRange(nodes);
        for (int i = 0; i < nodesInSet.Count; i++)
        {
            GameObject newCard = Instantiate(cardPrefab, cardHolder, false);
            ManageCardUI manageCardUi = newCard.GetComponent<ManageCardUI>();
            cardsInSet.Add(newCard);
            manageCardUi.SetCard(nodesInSet[i], owner, this);

        }
        var (list, allsame) = MonopolyBoard.instance.PlayerHasAllNodesofSet(nodesInSet[0]);
        Debug.Log(allsame + " allsame");
        buyHouseButton.interactable = allsame && CheckIfBuyAllowed();
        sellHouseButton.interactable = CheckIfSellAllowed();

        buyHousePriceText.text = "- " + nodesInSet[0].houseCost + " RON";
        sellHousePriceText.text = "+ " + nodesInSet[0].houseCost / 2 + " RON";
        if (nodes[0].monopolyNodeType != MonopolyNodeType.Property)
        {
            buttonBox.SetActive(false);
        }
    }



    public void BuyHouseButton()
    {
        if (!CheckIfBuyAllowed())
        {
            //error message
            string message = "One or more properties are mortgaged, you can't build a house!";
            ManageUI.instance.UpdateSystemMessage(message);
            return;
        }
        if (playerReference.CanAffordHouse(nodesInSet[0].houseCost))
        {
            playerReference.BuildHouseOrHotelEvenly(nodesInSet);
            //UPDATE MONEY Text - in manage ui
            UpdateHouseVisuals();
            string message = "You build a house.";
            ManageUI.instance.UpdateSystemMessage(message);
        }
        else
        {
            string message = "You don't have enough money!";
            ManageUI.instance.UpdateSystemMessage(message);
            //CANT AFFORD House-SYSTEM MESSAGE for the player 
        }
        sellHouseButton.interactable = CheckIfSellAllowed();
        ManageUI.instance.UpdateMoneyText();
    }
    public void SellHouseButton()
    {    //maybe check if player has houses to sell
        playerReference.SellHouseEvenly(nodesInSet);
        //UPDATE MONEY Text - in manage ui 
        UpdateHouseVisuals();
        sellHouseButton.interactable = CheckIfSellAllowed();
        ManageUI.instance.UpdateMoneyText();
    }

    public bool CheckIfSellAllowed()
    {
        if (nodesInSet.Any(n => n.NumberOfHouses > 0))
        {
            return true;
        }
        return false;
    }

    bool CheckIfBuyAllowed()
    {
        if (nodesInSet.Any(n => n.IsMortgaged == true))
        {
            return false;
        }
        return true;
    }

    public bool CheckIfMortgageAllowed()
    {
        if (nodesInSet.Any(n => n.NumberOfHouses > 0))
        {
            return false;
        }
        return true;
    }

    void UpdateHouseVisuals()
    {
        foreach (var item in cardsInSet)
        {
            cardHolder.GetComponent<ManageCardUI>().ShowBuildings();
        }
    }
}
