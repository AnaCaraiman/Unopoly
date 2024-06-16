using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.UIElements;
using UnityEngine;

using UnityEngine.UI;

public class ManageCardUI : MonoBehaviour
{
    [SerializeField] Image colorField;
    [SerializeField] GameObject[] buildings;
    [SerializeField] GameObject mortgageImage;
    [SerializeField] TMP_Text mortgageValueText;

    [SerializeField] Button mortgageButton, unMortgageButton;
    Player playerReference;
    MonopolyNode nodeReference;


    //Color setColor, int numberOfBuildings, bool isMortgaged, int mortgageValue
    public void SetCard (MonopolyNode node, Player owner)
    {
         nodeReference = node;
        playerReference = owner;
        //SET COLOR
        if (node.propertyColorField != null)
        {
            colorField.color = node.propertyColorField.color;
        }
        else 
        { 
            colorField.color = Color.black;
        }
        //SET BUILDINGS
        if (node.NumberOfHouses < 4)
        {
            for (int i = 0; i < node.NumberOfHouses; i++)
            {
                buildings[i].SetActive(true);
            }

        }
        else
        {
            buildings[4].SetActive(true); 
        }
        //SHOW MORTGAGE Image
        mortgageImage.SetActive(node.IsMortgaged);
        //Text update
        mortgageValueText.text = "Mortgage Value = RON " + node.MortgageValue;
        //Buttons
        mortgageButton.interactable = !node.IsMortgaged;
        unMortgageButton.interactable = node.IsMortgaged;

        


    }

    public void MortgageButton()
    {
        if (nodeReference.IsMortgaged)
        {
            //error message
            return;
        }
      
        playerReference.CollectMoney(nodeReference.MortgageProperty());
        mortgageImage.SetActive(true);
        mortgageButton.interactable = false;
        unMortgageButton.interactable = true;        
        
    }

    public void UnMortgageButton()
    {
        if (!nodeReference.IsMortgaged)
        {
            //error message
            return;
        }

        playerReference.PayMoney(nodeReference.MortgageValue);
        nodeReference.UnMortgageProperty();
        mortgageImage.SetActive(false);
        mortgageButton.interactable = true;
        unMortgageButton.interactable = false;

    }

}
