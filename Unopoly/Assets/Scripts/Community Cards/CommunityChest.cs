using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using UnityEngine.UI;

public class CommunityChest : MonoBehaviour
{
    [SerializeField] List<SCR_CommunityCard> cards = new List<SCR_CommunityCard>();
    [SerializeField] TMP_Text cardText;
    [SerializeField] GameObject cardHolderBackground;
    [SerializeField] float showTime = 3;
    [SerializeField] Button closeCardButton;

    List<SCR_CommunityCard> cardPool = new List<SCR_CommunityCard>();
    List<SCR_CommunityCard> usedCardPool = new List<SCR_CommunityCard>();
    //current card and player
    SCR_CommunityCard pickedCard;
    Player currentPlayer;
    
    void OnEnable()
    {
        MonopolyNode.OnDrawCommunityCard += Drawcard;
    }

    private void OnDisable()
    {
        MonopolyNode.OnDrawCommunityCard -= Drawcard;
    }

    private void Start()
    {
        cardHolderBackground.SetActive(false);
        //add all the cards to the pool
        cardPool.AddRange(cards);
        //shuffle the cards
        ShuffleCards();
    }

    void ShuffleCards()
    {
        for (int i = 0; i < cardPool.Count; i++)
        {
            int index = Random.Range(0, cardPool.Count);
            SCR_CommunityCard tempCard = cardPool[index];
            cardPool[index] = cardPool[i];
            cardPool[i] = tempCard;
        }
    }

    void Drawcard(Player cardTaker)
    {
        //draw a card
        pickedCard = cardPool[0];
        cardPool.RemoveAt(0);
        usedCardPool.Add(pickedCard);
        if (cardPool.Count == 0)
        {
            //put back all cards
            cardPool.AddRange(usedCardPool);
            usedCardPool.Clear();
            //shuffle all
            ShuffleCards();
        }
        //who is current player
        currentPlayer = cardTaker;
        //show the card
        cardHolderBackground.SetActive(true);
        //fill the card text
        cardText.text = pickedCard.textOnCard;
        //deactivate button if we are an ai
        if (currentPlayer.playerType == Player.PlayerType.AI)
        {
            closeCardButton.interactable = false;
            Invoke("ApplyCardEffect", showTime);
        }
        else
        {
            closeCardButton.interactable = true;
        }
    
    }

    public void ApplyCardEffect()//close button of the card
    {
        bool isMoving = false;
        if(pickedCard.rewardMoney != 0 && !pickedCard.collectFromPlayer)
        {
            currentPlayer.CollectMoney(pickedCard.rewardMoney);
        }
        else if(pickedCard.penalityMoney != 0)
        {
            currentPlayer.PayMoney(pickedCard.penalityMoney);//handle insufficent funds
        }
        else if(pickedCard.moveToBoardIndex != -1)
        {
            isMoving = true;
            //steps to goal
            int currentIndex = MonopolyBoard.instance.route.IndexOf(currentPlayer.MyMonopolyNode);
            int lengthOfBoard = MonopolyBoard.instance.route.Count;
            int stepsToMove = 0;

            if (pickedCard.moveToBoardIndex > currentIndex)
            {
                stepsToMove = pickedCard.moveToBoardIndex - currentIndex;
            }
            else if(pickedCard.moveToBoardIndex < currentIndex)
            {
                stepsToMove = lengthOfBoard - currentIndex + pickedCard.moveToBoardIndex;
            }
            //start to move
            MonopolyBoard.instance.MovePlayerToken(stepsToMove, currentPlayer); 
        }
        else if(pickedCard.collectFromPlayer)
        {
            int totalCollected = 0;
            List<Player> allplayers = GameManager.instance.GetPlayers;

            foreach (var player in allplayers)
            {
                if(player != currentPlayer)
                {
                    //prevent bankruptcy
                    int amount = Mathf.Min(player.ReadMoney, pickedCard.rewardMoney);
                    player.PayMoney(amount);
                    totalCollected += amount;
                }
            }
            currentPlayer.CollectMoney(totalCollected);

        }
        else if(pickedCard.streetRepairs)
        {
            int[] allBuildings = currentPlayer.CountHouseAndHotels();
            int totalCost = pickedCard.streetRepairsHousePrice * allBuildings[0] + pickedCard.streetRepairsHotelPrice * allBuildings[1];
            currentPlayer.PayMoney(totalCost);
        }
        else if(pickedCard.goToJail)
        {
            isMoving = true;
            currentPlayer.GoToJail(MonopolyBoard.instance.route.IndexOf(currentPlayer.MyMonopolyNode));
        }
        else if(pickedCard.jailFreeCard)
        {

        }
        cardHolderBackground.SetActive(false);
        ContinueGame(isMoving);
    }

    void ContinueGame(bool isMoving)
    {
        if(currentPlayer.playerType == Player.PlayerType.AI)
        {
            if(!isMoving && GameManager.instance.RolledDouble)
            {
                GameManager.instance.RollDice();
            }
            else if(!isMoving && !GameManager.instance.RolledDouble)
            {
                GameManager.instance.SwitchPlayer();
            }
        }
        else //human input
        {
            
        }
    }


}
