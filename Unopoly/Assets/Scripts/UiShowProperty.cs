using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using UnityEngine.UI;

public class UiShowProperty : MonoBehaviour
{
    MonopolyNode nodeReference;
    Player playerReference;

    [Header("Buy Property UI")]
    [SerializeField] GameObject propertyUiPanel;
    [SerializeField] TMP_Text propertyNameText;
    [SerializeField] Image colorField;
    [Space]
    [SerializeField] TMP_Text rentPriceText; //without a house
    [SerializeField] TMP_Text oneHouseRentText;
    [SerializeField] TMP_Text twoHouseRentText;
    [SerializeField] TMP_Text threeHouseRentText;
    [SerializeField] TMP_Text fourHouseRentText;
    [SerializeField] TMP_Text hotelRentText;
    [Space]
    [SerializeField] TMP_Text housePriceText;
    [SerializeField] TMP_Text hotelPriceText;
    [Space]
    [SerializeField] Button buyPropertyButton;
    [Space]
    [SerializeField] TMP_Text propertyPriceText;
    [SerializeField] TMP_Text playerMoneyText;

    void OnEnable()
    {
        MonopolyNode.OnShowPropertyBuyPanel += ShowBuyPropertyUi;
    }

    private void OnDisable()
    {
        MonopolyNode.OnShowPropertyBuyPanel -= ShowBuyPropertyUi;
    }

    void Start()
    {
        propertyUiPanel.SetActive(false);
    }

    void ShowBuyPropertyUi(MonopolyNode node, Player currentPlayer)
    {
        nodeReference = node;
        playerReference = currentPlayer;
        //top panel content
        propertyNameText.text = node.name;
        colorField.color = node.propertyColorField.color; 
        //center of the card
        rentPriceText.text = node.baseRent + " RON";
        oneHouseRentText.text = node.rentWithHouses[0] + " RON";
        twoHouseRentText.text = node.rentWithHouses[1] + " RON";
        threeHouseRentText.text = node.rentWithHouses[2] + " RON";
        fourHouseRentText.text = node.rentWithHouses[3] + " RON";
        hotelRentText.text = node.rentWithHouses[4] + " RON";
        //cost of buildings
        housePriceText.text = node.houseCost + " RON";
        hotelPriceText.text = node.houseCost + " RON";
        //bottom bar
        propertyPriceText.text = "Price: " + node.price + " RON";
        playerMoneyText.text = "Your Money: " + currentPlayer.ReadMoney + " RON";
        //buy property button
        if(currentPlayer.CanAffordNode(node.price))
        {
            buyPropertyButton.interactable = true;
        }
        else
        {
            buyPropertyButton.interactable = false;
        }
        //show the panel
        propertyUiPanel.SetActive(true);
    }

    public void BuyPropertyButton()//this is called from the buy button
    {
        //tell the player to buy the property
        playerReference.BuyProperty(nodeReference);
        //mayble close the property card or

        //make the button not interactable anymore
        buyPropertyButton.interactable = false;
    }

    public void ClosePropertyButton()//this is called from the buy button
    {
        //close the panel
        propertyUiPanel.SetActive(false);
        //clear node reference
        nodeReference = null;
        playerReference = null;
    }
}
