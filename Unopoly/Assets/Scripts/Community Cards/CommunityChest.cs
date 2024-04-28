using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class CommunityChest : MonoBehaviour
{
    [SerializeField] List<SCR_CommunityCard> cards = new List<SCR_CommunityCard>();
    [SerializeField] TMP_Text cardText;
    [SerializeField] GameObject cardHolderBackground;
    [SerializeField] float showTime = 3;
    [SerializeField] float moveDelay = 0.5f;

    [SerializeField] List<SCR_CommunityCard> cardPool = new List<SCR_CommunityCard>();

    void OnEnable()
    {
        MonopolyNode.OnDrawCommunityCard += Drawcard;
    }

    private void OnDisable()
    {
        MonopolyNode.OnDrawCommunityCard += Drawcard;
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
        SCR_CommunityCard pickedCard = cardPool[0];
        //show the card
        cardHolderBackground.SetActive(true);
        //fill the card text
        cardText.text = pickedCard.textOnCard;
        //apply the card effect

    }

}
