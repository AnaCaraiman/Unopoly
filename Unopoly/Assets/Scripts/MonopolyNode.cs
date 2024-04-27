using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

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
    [Header("Property Name")]
    [SerializeField] internal new string name;
    [SerializeField] TMP_Text nameText;
    [Header("Property Price")]
    public int price;
    [SerializeField] TMP_Text priceText;
    [Header("Property Rent")]
    [SerializeField] bool calculateRentAuto;
    [SerializeField] int currentRent;
    [SerializeField] internal int baseRent;
    [SerializeField] internal int[] rentWithHouses;
    [Header("Property Mortgage")]
    [SerializeField] GameObject mortgageImage;
    [SerializeField] GameObject propertyImage;
    [SerializeField] bool isMortgaged;
    [SerializeField] int mortgageValue;
    [Header("Property Owner")]
    public Player owner;
    [SerializeField] GameObject ownerBar;
    [SerializeField] TMP_Text ownerText;

    private void OnValidate()
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
                if(baseRent > 0)
                {
                    price = 3 * (baseRent * 10);
                    //mortgage price
                    mortgageValue = price / 2;
                    rentWithHouses = new int[]
                    {
                        baseRent * 5,
                        baseRent * 5 *3,
                        baseRent * 5 * 9,
                        baseRent * 5 * 16,
                        baseRent * 5 * 25,
                    };
                }
            }
            if(monopolyNodeType == MonopolyNodeType.Utility) {
                mortgageValue = price / 2;
            }

            if (monopolyNodeType == MonopolyNodeType.Utility) {
                mortgageValue = price / 2;
            }
        }

        if (priceText != null)
        {
            priceText.text = price + "RON";
        }
        //update the owner
        OnOwnerUpdated();
        MortgageProperty();
        //isMortgaged = false;
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
            if(owner.playerName != "")
            {
                ownerBar.SetActive(true);
                ownerText.text = owner.playerName;
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

        if(!playerIsHuman)
        {
            Invoke("ContinueGame", 2f);
        }
        else
        {
            //show ui
        }
    }

    void ContinueGame()
    {
        GameManager.instance.SwitchPlayer();
    }
}
