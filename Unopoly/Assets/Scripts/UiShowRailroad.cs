using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using UnityEngine.UI;

public class UiShowRailroad : MonoBehaviour
{

    MonopolyNode nodeReference;
    Player playerReference;

    [Header("Buy Railroad UI")]
    [SerializeField] GameObject railroadUiPanel;
    [SerializeField] TMP_Text railroadNameText;

    [Space]

    [SerializeField] TMP_Text oneRailroadRentText;
    [SerializeField] TMP_Text twoRailroadRentText;
    [SerializeField] TMP_Text threeRailroadRentText;
    [SerializeField] TMP_Text fourRailroadRentText;

    [Space]

    [SerializeField] TMP_Text morgagePriceText;
    [Space]
    [SerializeField] Button buyRailRoadButton;
    [Space]
    [SerializeField] TMP_Text propertyPriceText;
    [SerializeField] TMP_Text playerMoneyText;

    void OnEnable()
    {
        MonopolyNode.OnShowRailroadBuyPanel += ShowBuyRailroadPanelUI;
    }

    private void OnDisable()
    {
        MonopolyNode.OnShowRailroadBuyPanel -= ShowBuyRailroadPanelUI;
    }
    void ShowBuyRailroadPanelUI(MonopolyNode node, Player currentPlayer)
    {

        nodeReference = node;
        playerReference = currentPlayer;
        //top panel content
        railroadNameText.text = node.name;
        // colorField.color = node.propertyColorField.color;
        //center of the card
        //result = baseRent * (int)Mathf.Pow(2, amount-1); 

        oneRailroadRentText.text = node.baseRent * (int)Mathf.Pow(2, 1 - 1) + " RON";
        twoRailroadRentText.text = node.baseRent * (int)Mathf.Pow(2, 2 - 1) + " RON";
        threeRailroadRentText.text = node.baseRent * (int)Mathf.Pow(2, 3 - 1) + " RON";
        fourRailroadRentText.text = node.baseRent * (int)Mathf.Pow(2, 4 - 1) + " RON";

        //cost of buildings

        morgagePriceText.text = node.houseCost + " RON";
        //bottom bar
        propertyPriceText.text = "Price: " + node.price + " RON";
        playerMoneyText.text = "Your Money: " + currentPlayer.ReadMoney + " RON";
        //buy property button
        if (currentPlayer.CanAffordNode(node.price))
        {
            buyRailRoadButton.interactable = true;
        }
        else
        {
            buyRailRoadButton.interactable = false;
        }
        //show the panel
        railroadUiPanel.SetActive(true);



    }

    void Start()
    {
        railroadUiPanel.SetActive(false);
    }
    public void BuyRailroadButton()//this is called from the buy button
    {
        //tell the player to buy the property
        playerReference.BuyProperty(nodeReference);
        //mayble close the property card or

        //make the button not interactable anymore
        buyRailRoadButton.interactable = false;
    }

    public void CloseRailroadButton()//this is called from the buy button
    {
        //close the panel
        railroadUiPanel.SetActive(false);
        //clear node reference
        nodeReference = null;
        playerReference = null;
    }

}