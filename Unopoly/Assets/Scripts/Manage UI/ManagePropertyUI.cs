using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

public class ManagePropertyUI : MonoBehaviour
{
    [SerializeField] Transform cardHolder;//Horizontal layout 
    [SerializeField] GameObject cardPrefab;
    [SerializeField] Button buyHouseButton, sellHouseButton;
    [SerializeField] TMP_Text buyHousePriceText, sellHousePriceText;
    Player playerReference;

    List<MonopolyNode> nodesInSet= new List<MonopolyNode>();
    List<GameObject> cardInSet = new List<GameObject>();

    //THIS Property is only for 1 specific card set
    public void SetProperty(List<MonopolyNode> nodes, Player owner)
    {
        playerReference = owner;
        nodesInSet.AddRange(nodes);
        for (int i = 0; i < nodesInSet.Count; i++)
        {
           GameObject newCard = Instantiate(cardPrefab, cardHolder, false);
            ManageCardUI manageCardUi= newCard.GetComponent<ManageCardUI>();
            cardInSet.Add(newCard);
            manageCardUi.SetCard(nodesInSet[i],owner);

        }
        var (list, allsame) = MonopolyBoard.instance.PlayerHasAllNodesofSet(nodesInSet[0]);
        buyHouseButton.interactable = allsame;
        sellHouseButton.interactable = allsame;
    }



    public void BuyHouseButton()
    {
        if (playerReference.CanAffordHouse(nodesInSet[0].houseCost))
        {
            playerReference.BuildHouseOrHotelEvenly(nodesInSet);
        }
        else { 
        //CANT AFFORD House-SYSTEM MESSAGE for the player 
        }
    }
    public void SellHouseButton()
    {    //maybe check if player has houses to sell
        playerReference.SellHouseEvenly(nodesInSet);
        //UPDATE MONEY Text - in manage ui 

    }
}
