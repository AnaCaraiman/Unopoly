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
    int numberOfHouses;
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
            if(ownerName != "")
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

        //check for node type

        switch (monopolyNodeType)
        {
            case MonopolyNodeType.Property:
                if (!playerIsHuman)//AI
                {
                    // if it owned && if we not are the owner && if it is not mortgaged
                    if (owner.name != "" && owner != currentPlayer && !isMortgaged)
                    { //pay rent to somebody

                      //calculate the rent
                      int rentToPay = CalculatePropertyRent();

                      //pay the rent to the owner
                      //show a message about what happened
                    }
                    else if (owner.name == "" /*&& if can afford*/ )
                    {// buy th node

                        //show a mesage

                    }
                    else 
                    { //is unowned and cant afford it 
                        
                    }
                }
                else//human
                { 
                    // if it owned && if we not are the owner && if it is not mortgaged
                    if (owner.name != "" && owner != currentPlayer && !isMortgaged)
                    { //pay rent to somebody
                      //calculate the rent
                      //pay the rent to the owner
                      //show a message about what happened
                    }
                    else if (owner.name == "")
                    {   
                        // show buy interface for propwert

                     

                    }
                    else
                    { //is unowned and cant afford it 

                    }

                }
            break;

            case MonopolyNodeType.Utility:

                break;
            case MonopolyNodeType.Railroad:

                break;
            case MonopolyNodeType.Tax:

                break;
            case MonopolyNodeType.FreeParking:

                break;
            case MonopolyNodeType.GoToJail:

                break;
            case MonopolyNodeType.Chance:

                break;
            case MonopolyNodeType.CommunityChest:

                break;
        }
        
        
        
        
        //continue
        if(!playerIsHuman)
        {
            Invoke("ContinueGame", 2f);
        }
        else
        {

        }
    }

    void ContinueGame()
    {
        GameManager.instance.SwitchPlayer();
    }

    int CalculatePropertyRent()
    {
        switch (numberOfHouses) 
        {
            case 0:
                //check if owner has the full set of this nodes
                bool allsame = true;
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
}
