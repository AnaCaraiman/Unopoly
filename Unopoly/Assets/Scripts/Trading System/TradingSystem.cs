using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.UI;

public class TradingSystem : MonoBehaviour
{
    public static TradingSystem instance;

    [SerializeField] GameObject cardPrefab;
    [SerializeField] GameObject tradePanel;
    [SerializeField] GameObject resultPanel;
    [SerializeField] TMP_Text resultMessageText;
    [Header("LEFT SIDE")]
    [SerializeField] TMP_Text leftOffererNameText;
    [SerializeField] Transform leftCardGrid;
    [SerializeField] ToggleGroup leftToggleGroup; //to toggle the card selection
    [SerializeField] TMP_Text leftYourMoneyText;
    [SerializeField] TMP_Text leftOfferMoneyText;
    [SerializeField] Slider leftMoneySlider;
    List<GameObject> leftCardPrefabList = new List<GameObject>();
    Player leftPlayerReferene;

    [Header("MIDDLE")]
    [SerializeField] Transform buttonGrid;
    [SerializeField] GameObject playerButtonPrefab;
    List<GameObject> playerButtonList = new List<GameObject>();

    [Header("RIGHT SIDE")]
    [SerializeField] TMP_Text rightOffererNameText;
    [SerializeField] Transform rightCardGrid;
    [SerializeField] ToggleGroup rightToggleGroup; //to toggle the card selection
    [SerializeField] TMP_Text rightYourMoneyText;
    [SerializeField] TMP_Text rightOfferMoneyText;
    [SerializeField] Slider rightMoneySlider;
    List<GameObject> rightCardPrefabList = new List<GameObject>();
    Player rightPlayerReferene;

    [Header("Trade Offer")]
    [SerializeField] GameObject tradeOfferPanel;
    [SerializeField] TMP_Text leftMessageText, rightMessageText, leftMoneyText, rightMoneyText;
    [SerializeField] GameObject leftCard, rightCard;
    [SerializeField] Image leftColorField, rightColorField;
    [SerializeField] Image leftPropImage, rightPropImage;
    [SerializeField] Sprite houseSprite, railroadSprite, utilitySprite;


    //store the offer for human
    Player currentPlayer, nodeOwner;
    MonopolyNode requestedNode, offerNode;
    int offeredMoney, requestedMoney;

    //message system
    public delegate void UpdateMessage(string message);
    public static UpdateMessage OnUpdateMessage;

    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        tradePanel.SetActive(false);
        resultPanel.SetActive(false);
        tradeOfferPanel.SetActive(false);
    }
    //find missing properties in set
    public void FindMissingProperty(Player currentPlayer)
    {
        List<MonopolyNode> processedSet = null;
        MonopolyNode requestedNode = null;
        foreach (var node in currentPlayer.GetMonopolyNodes)
        {
            var (list, allSame) = MonopolyBoard.instance.PlayerHasAllNodesofSet(node);
            List<MonopolyNode> nodeSet = new List<MonopolyNode>();
            nodeSet.AddRange(list);
            //ckeck if all have been purchased
            bool notAllPurchased = list.Any(n => n.Owner == null);

            //ai owns this full set already
            if (allSame || processedSet == list || notAllPurchased)
            {
                processedSet = list;
                continue;
            }
            //find the owned by other player
            if (list.Count == 2)
            {
                requestedNode = list.Find(n => n.Owner != currentPlayer && n.Owner != null);
                if (requestedNode != null)
                {
                    //make offer
                    MakeTradeDecision(currentPlayer, requestedNode.Owner, requestedNode);
                    break;
                }
            }
            if (list.Count >= 3)
            {
                int hasMostOfSet = list.Count(n => n.Owner == currentPlayer);
                if (hasMostOfSet >= 2)
                {
                    requestedNode = list.Find(n => n.Owner != currentPlayer && n.Owner != null);
                    //make offer
                    MakeTradeDecision(currentPlayer, requestedNode.Owner, requestedNode);
                    break;
                }
            }

        }
        //continue if nothing is found
        if (requestedNode == null)
        {
            currentPlayer.ChangeState(Player.AiStates.IDLE);
        }
    }
    //make trade decision
    void MakeTradeDecision(Player currentPlayer, Player nodeOwner, MonopolyNode requestedNode)
    {
        //trade with money if possible
        if (currentPlayer.ReadMoney >= CalculateValueOfNode(requestedNode))
        {
            //trade with money only

            //make decision
            MakeTradeOffer(currentPlayer, nodeOwner, requestedNode, null, CalculateValueOfNode(requestedNode), 0);
            return;
        }

        bool foundDecision = false;

        //find incomplete set but exclude set with current node
        foreach (var node in currentPlayer.GetMonopolyNodes)
        {
            var checkedSet = MonopolyBoard.instance.PlayerHasAllNodesofSet(node).list;
            if (checkedSet.Contains(requestedNode))
            {
                //stop checking
                continue;
            }
            if (checkedSet.Count(n => n.Owner == currentPlayer) == 1)//valid node check
            {
                if (CalculateValueOfNode(node) + currentPlayer.ReadMoney >= requestedNode.price)
                {
                    int difference = CalculateValueOfNode(requestedNode) - CalculateValueOfNode(node);
                    if (difference >= 0)
                    {
                        //make offer
                        MakeTradeOffer(currentPlayer, nodeOwner, requestedNode, node, difference, 0);
                        break;
                    }
                    else
                    {
                        MakeTradeOffer(currentPlayer, nodeOwner, requestedNode, node, 0, Mathf.Abs(difference));
                    }
                    //make offer
                    foundDecision = true;
                    break;
                }
            }
        }
        if (!foundDecision)
        {
            currentPlayer.ChangeState(Player.AiStates.IDLE);
        }

    }

    //make trade offer
    void MakeTradeOffer(Player currentPlayer, Player nodeOwner, MonopolyNode requestedNode, MonopolyNode offerNode, int offeredMoney, int requestedMoney)
    {
        if (nodeOwner.playerType == Player.PlayerType.AI)
        {
            ConsiderTradeOffer(currentPlayer, nodeOwner, requestedNode, offerNode, offeredMoney, requestedMoney);
        }
        else if (nodeOwner.playerType == Player.PlayerType.Human)
        {
            //show UI for human
            ShowTradeOfferPanel(currentPlayer, nodeOwner, requestedNode, offerNode, offeredMoney, requestedMoney);
        }
    }

    //consider trade offer
    void ConsiderTradeOffer(Player currentPlayer, Player nodeOwner, MonopolyNode requestedNode, MonopolyNode offerNode, int offeredMoney, int requestedMoney)
    {
        int valueOfTheTrade = (CalculateValueOfNode(requestedNode) + requestedMoney) - (CalculateValueOfNode(offerNode) + offeredMoney);
        //sell a node for money only
        if (requestedNode == null && offerNode != null && requestedMoney <= nodeOwner.ReadMoney / 3 && MonopolyBoard.instance.PlayerHasAllNodesofSet(requestedNode).allSame)
        {
            Trade(currentPlayer, nodeOwner, requestedNode, offerNode, offeredMoney, requestedMoney);
            if (currentPlayer.playerType == Player.PlayerType.Human)
            {
                TradeResult(true);
            }
            return;
        }
        //normal trade
        if (valueOfTheTrade <= 0 && MonopolyBoard.instance.PlayerHasAllNodesofSet(requestedNode).allSame)
        {
            //trade the node is valid
            Trade(currentPlayer, nodeOwner, requestedNode, offerNode, offeredMoney, requestedMoney);
            if (currentPlayer.playerType == Player.PlayerType.Human)
            {
                TradeResult(true);
            }
        }
        else
        {
            if (currentPlayer.playerType == Player.PlayerType.Human)
            {
                TradeResult(false);
            }
            //debug line or tell player it is rejected
            Debug.Log("AI rejected trade offer");
        }
    }

    //calculate the value of node
    int CalculateValueOfNode(MonopolyNode requestedNode)
    {
        int value = 0;
        if (requestedNode != null)
        {
            if (requestedNode.monopolyNodeType == MonopolyNodeType.Property)
            {
                value = requestedNode.price + requestedNode.NumberOfHouses * requestedNode.houseCost;
            }
            else
            {
                value = requestedNode.price;
            }
            return value;
        }
        return value;

    }

    //trade the node
    void Trade(Player currentPlayer, Player nodeOwner, MonopolyNode requestedNode, MonopolyNode offerNode, int offeredMoney, int requestedMoney)
    {
        if (requestedNode != null)
        {
            currentPlayer.PayMoney(offeredMoney);
            requestedNode.ChangeOwner(currentPlayer);

            nodeOwner.CollectMoney(offeredMoney);
            nodeOwner.PayMoney(requestedMoney);

            if (offerNode != null)
            {
                offerNode.ChangeOwner(nodeOwner);
            }

            string offeredNodeName = (offerNode != null) ? " & " + offerNode.name : "";
            OnUpdateMessage.Invoke(currentPlayer.name + " traded " + requestedNode.name + " for " + offeredMoney + offeredNodeName + " to " + nodeOwner.name);
        }
        else if (offerNode != null && requestedNode == null)
        {
            currentPlayer.CollectMoney(requestedMoney);
            nodeOwner.PayMoney(requestedMoney);
            offerNode.ChangeOwner(nodeOwner);
            OnUpdateMessage.Invoke(currentPlayer.name + " sold " + offerNode.name + " to " + nodeOwner.name + " for " + requestedMoney);
        }

        //hide UI for human only
        CloseTradePanel();
        if (currentPlayer.playerType == Player.PlayerType.AI)
        {
            currentPlayer.ChangeState(Player.AiStates.IDLE);
        }
    }

    //current player
    public void CreateLeftPanel()
    {
        leftOffererNameText.text = leftPlayerReferene.name;
        List<MonopolyNode> referenceNodes = leftPlayerReferene.GetMonopolyNodes;

        for (int i = 0; i < referenceNodes.Count; i++)
        {
            GameObject tradeCard = Instantiate(cardPrefab, leftCardGrid, false);
            //set actual card
            tradeCard.GetComponent<TradePropertyCard>().SetTradeCard(leftPlayerReferene.GetMonopolyNodes[i], leftToggleGroup);
            leftCardPrefabList.Add(tradeCard);
        }
        leftYourMoneyText.text = "Your Money: " + leftPlayerReferene.ReadMoney;
        //set up money slider
        leftMoneySlider.maxValue = leftPlayerReferene.ReadMoney;
        leftMoneySlider.value = 0;
        UpdateLeftSlider(leftMoneySlider.value);

        //reset old content
        tradePanel.SetActive(true);
    }

    public void UpdateLeftSlider(float value)
    {
        leftOfferMoneyText.text = "Offered Money: " + leftMoneySlider.value;
    }

    public void UpdateRightSlider(float value)
    {
        rightOfferMoneyText.text = "Requested Money: " + rightMoneySlider.value;
    }

    public void CloseTradePanel()
    {
        tradePanel.SetActive(false);
        ClearAll();
    }

    public void OpenTradePanel(Player currentPlayer)
    {
        leftPlayerReferene = GameManager.instance.GetCurrentPlayer;
        rightOffererNameText.text = "Select a Player";

        CreateLeftPanel();
        CreateMiddleButtons();
    }

    //selected player
    public void ShowRightPlayer(Player player)
    {
        rightPlayerReferene = player;
        //reset current content
        ClearRightPanel();

        //show right player of above player
        rightOffererNameText.text = rightPlayerReferene.name;
        List<MonopolyNode> referenceNodes = rightPlayerReferene.GetMonopolyNodes;

        for (int i = 0; i < referenceNodes.Count; i++)
        {
            GameObject tradeCard = Instantiate(cardPrefab, rightCardGrid, false);
            tradeCard.GetComponent<TradePropertyCard>().SetTradeCard(rightPlayerReferene.GetMonopolyNodes[i], rightToggleGroup);
            //set actual card
            rightCardPrefabList.Add(tradeCard);
        }
        rightYourMoneyText.text = "Your Money: " + rightPlayerReferene.ReadMoney;
        rightMoneySlider.maxValue = rightPlayerReferene.ReadMoney;
        rightMoneySlider.value = 0;
        UpdateRightSlider(rightMoneySlider.value);

        //update the money and the slider
    }

    //set up middle
    void CreateMiddleButtons()
    {
        //clear content
        for (int i = playerButtonList.Count - 1; i >= 0; i--)
        {
            Destroy(playerButtonList[i]);
        }
        playerButtonList.Clear();

        //loop through all players and
        List<Player> allPlayers = new List<Player>();
        allPlayers.AddRange(GameManager.instance.GetPlayers);
        allPlayers.Remove(leftPlayerReferene);

        //add buttons 
        foreach (var player in allPlayers)
        {
            GameObject newPlayerButton = Instantiate(playerButtonPrefab, buttonGrid, false);
            newPlayerButton.GetComponent<TradePlayerButton>().SetPlayer(player);
            playerButtonList.Add(newPlayerButton);
        }
    }

    void ClearAll()
    {
        rightOffererNameText.text = "Select a Player";
        rightYourMoneyText.text = "Your Money: 0 RON";
        rightMoneySlider.maxValue = 0;
        rightMoneySlider.value = 0;
        UpdateRightSlider(rightMoneySlider.value);
        //clear middle buttons
        for (int i = playerButtonList.Count - 1; i >= 0; i--)
        {
            Destroy(playerButtonList[i]);
        }
        playerButtonList.Clear();

        //clear left side
        for (int i = leftCardPrefabList.Count - 1; i >= 0; i--)
        {
            Destroy(leftCardPrefabList[i]);
        }
        leftCardPrefabList.Clear();

        //clear right side
        for (int i = rightCardPrefabList.Count - 1; i >= 0; i--)
        {
            Destroy(rightCardPrefabList[i]);
        }
        rightCardPrefabList.Clear();
    }

    void ClearRightPanel()
    {
        //clear right side
        for (int i = rightCardPrefabList.Count - 1; i >= 0; i--)
        {
            Destroy(rightCardPrefabList[i]);
        }
        rightCardPrefabList.Clear();

        //reset slider
        rightMoneySlider.maxValue = 0;
        rightMoneySlider.value = 0;
        UpdateRightSlider(rightMoneySlider.value);
    }

    //make offer
    public void MakeOfferButton()
    {
        MonopolyNode requestedNode = null;
        MonopolyNode offeredNode = null;
        if (rightPlayerReferene == null)
        {
            //error message - no player to trade with
            return;
        }

        Toggle offeredToggle = leftToggleGroup.ActiveToggles().FirstOrDefault();
        if (offeredToggle != null)
        {
            offeredNode = offeredToggle.GetComponentInParent<TradePropertyCard>().Node();
        }

        Toggle requestedToggle = rightToggleGroup.ActiveToggles().FirstOrDefault();
        if (requestedToggle != null)
        {
            requestedNode = requestedToggle.GetComponentInParent<TradePropertyCard>().Node();
        }

        MakeTradeOffer(leftPlayerReferene, rightPlayerReferene, requestedNode, offeredNode, (int)leftMoneySlider.value, (int)rightMoneySlider.value);
    }

    //trade resut
    void TradeResult(bool accepted)
    {
        if (accepted)
        {
            resultMessageText.text = rightPlayerReferene.name + "<b><color=green> accepted </color></b>" + "the trade";
        }
        else
        {
            resultMessageText.text = rightPlayerReferene.name + "<b><color=red> rejected </color></b>" + "the trade";
        }
        resultPanel.SetActive(true);
    }

    //trade offer panel
    void ShowTradeOfferPanel(Player _currentPlayer, Player _nodeOwner, MonopolyNode _requestedNode, MonopolyNode _offerNode, int _offeredMoney, int _requestedMoney)
    {
        //fill the offer content
        currentPlayer = _currentPlayer;
        nodeOwner = _nodeOwner;
        requestedNode = _requestedNode;
        offerNode = _offerNode;
        offeredMoney = _offeredMoney;
        requestedMoney = _requestedMoney;

        //show panel content
        tradeOfferPanel.SetActive(true);
        leftMessageText.text = currentPlayer.name + " offers:";
        rightMessageText.text = "For " + nodeOwner.name + "'s:";
        leftMoneyText.text = "+ " + offeredMoney + " RON";
        rightMoneyText.text = "+ " + requestedMoney + " RON";
        leftCard.SetActive(offerNode != null ? true : false);
        rightCard.SetActive(requestedNode != null ? true : false);

        if (leftCard.activeInHierarchy)
        {
            leftColorField.color = (offerNode.propertyColorField != null) ? offerNode.propertyColorField.color : Color.black; // Access the color property
            switch (offerNode.monopolyNodeType)
            {
                case MonopolyNodeType.Property:
                    leftPropImage.sprite = houseSprite;
                    leftPropImage.color = Color.red;
                    break;
                case MonopolyNodeType.Railroad:
                    leftPropImage.sprite = railroadSprite;
                    leftPropImage.color = Color.white;
                    break;
                case MonopolyNodeType.Utility:
                    leftPropImage.sprite = utilitySprite;
                    leftPropImage.color = Color.black;
                    break;
            }
        }

        if (rightCard.activeInHierarchy)
        {
            rightColorField.color = (requestedNode.propertyColorField != null) ? requestedNode.propertyColorField.color : Color.black; // Access the color property
            switch (requestedNode.monopolyNodeType)
            {
                case MonopolyNodeType.Property:
                    rightPropImage.sprite = houseSprite;
                    rightPropImage.color = Color.red;
                    break;
                case MonopolyNodeType.Railroad:
                    rightPropImage.sprite = railroadSprite;
                    rightPropImage.color = Color.white;
                    break;
                case MonopolyNodeType.Utility:
                    rightPropImage.sprite = utilitySprite;
                    rightPropImage.color = Color.black; 
                    break;
            }
        }
    }

        public void AcceptOffer()
    {
        Trade(currentPlayer, nodeOwner, requestedNode, offerNode, offeredMoney, requestedMoney);
        ResetOffer();
    }

    public void RejectOffer()
    {
        currentPlayer.ChangeState(Player.AiStates.IDLE);
        ResetOffer();
    }

    void ResetOffer()
    {
        currentPlayer = null;
        nodeOwner = null;
        requestedNode = null;
        offerNode = null;
        offeredMoney = 0;
        requestedMoney = 0;
    }
}
