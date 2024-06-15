using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using UnityEngine.UI;


public class UiShowUtility : MonoBehaviour
{
    MonopolyNode nodeReference;
    Player playerReference;

    [Header("Buy Railroad UI")]
    [SerializeField] GameObject utilityUiPanel;
    [SerializeField] TMP_Text utilityText;

    [Space]

    [Space]

    [SerializeField] TMP_Text morgagePriceText;
    [Space]
    [SerializeField] Button buyUtilityButton;
    [Space]
    [SerializeField] TMP_Text utilityPriceText;
    [SerializeField] TMP_Text playerMoneyText;

    void OnEnable()
    {
        MonopolyNode.OnShowUtilityBuyPanel += ShowBuyUtilityPanelUI;
    }

    private void OnDisable()
    {
        MonopolyNode.OnShowRailroadBuyPanel -= ShowBuyUtilityPanelUI;
    }

    void Start()
    {
        utilityUiPanel.SetActive(false);
    }

    void ShowBuyUtilityPanelUI(MonopolyNode node, Player currentPlayer)
    {

        nodeReference = node;
        playerReference = currentPlayer;
        //top panel content
        utilityText.text = node.name;
        //cost of buildings

        morgagePriceText.text = node.houseCost + " RON";
        //bottom bar
        utilityPriceText.text = "Price: " + node.price + " RON";
        playerMoneyText.text = "Your Money: " + currentPlayer.ReadMoney + " RON";
        //buy property button
        if (currentPlayer.CanAffordNode(node.price))
        {
            buyUtilityButton.interactable = true;
        }
        else
        {
            buyUtilityButton.interactable = false;
        }
        //show the panel
        utilityUiPanel.SetActive(true);



    }

    public void BuyUtilityButton()//this is called from the buy button
    {
        //tell the player to buy the property
        playerReference.BuyProperty(nodeReference);
        //mayble close the property card or

        //make the button not interactable anymore
        buyUtilityButton.interactable = false;
    }

    public void CloseUtilityButton()//this is called from the buy button
    {
        //close the panel
        utilityUiPanel.SetActive(false);
        //clear node reference
        nodeReference = null;
        playerReference = null;
    }
}
