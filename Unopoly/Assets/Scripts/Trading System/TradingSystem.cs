using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TradingSystem : MonoBehaviour
{
    public static TradingSystem instance;

    //message system
    public delegate void UpdateMessage(string message);
    public static UpdateMessage OnUpdateMessage;

    void Awake()
    {
        instance = this;
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
                if(hasMostOfSet >= 2)
                {
                    requestedNode = list.Find(n => n.Owner != currentPlayer && n.Owner != null);
                    //make offer
                    MakeTradeDecision(currentPlayer, requestedNode.Owner, requestedNode);
                    break;
                }
            }

        }
    }
    //make trade decision
    void MakeTradeDecision(Player currentPlayer, Player nodeOwner, MonopolyNode requestedNode)
    {
        //trade with money if possible
        if(currentPlayer.ReadMoney >= CalculateValueOfNode(requestedNode))
        {
            //trade with money only

            //make decision
            MakeTradeOffer(currentPlayer, nodeOwner, requestedNode, null, CalculateValueOfNode(requestedNode), 0);
            return;
        }
        //find incomplete set but exclude set with current node
        foreach (var node in currentPlayer.GetMonopolyNodes)
        {
            var checkedSet = MonopolyBoard.instance.PlayerHasAllNodesofSet(node).list;
            if(checkedSet.Contains(requestedNode)) 
            {
                //stop checking
                continue;
            }
            if(checkedSet.Count(n => n.Owner == currentPlayer) == 1)//valid node check
            {
                if(CalculateValueOfNode(node) + currentPlayer.ReadMoney >= requestedNode.price)
                {
                    int difference = CalculateValueOfNode(requestedNode) - CalculateValueOfNode(node);
                    if(difference >= 0)
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
                    break;
                }
            }
        }

    }

    //make trade offer
    void MakeTradeOffer(Player currentPlayer, Player nodeOwner, MonopolyNode requestedNode, MonopolyNode offerNode, int offeredMoney, int requestedMoney)
    {
        if(nodeOwner.playerType == Player.PlayerType.AI)
        {
            ConsiderTradeOffer(currentPlayer, nodeOwner, requestedNode, offerNode, offeredMoney, requestedMoney);
        }
        else if(nodeOwner.playerType == Player.PlayerType.Human) 
        {
            //show UI for human
        }
    }

    //consider trade offer
    void ConsiderTradeOffer(Player currentPlayer, Player nodeOwner, MonopolyNode requestedNode, MonopolyNode offerNode, int offeredMoney, int requestedMoney)
    {
        int valueOfTheTrade = (CalculateValueOfNode(requestedNode) + requestedMoney) - (CalculateValueOfNode(offerNode) + offeredMoney);
        //sell a node for money only
        if(requestedNode == null && offerNode != null && requestedMoney <= nodeOwner.ReadMoney/3)
        {
            Trade(currentPlayer, nodeOwner, requestedNode, offerNode, offeredMoney, requestedMoney);
            return;
        }
        //normal trade
        if(valueOfTheTrade <= 0)
        {
            //trade the node is valid
            Trade(currentPlayer, nodeOwner, requestedNode, offerNode, offeredMoney, requestedMoney);
        }
        else
        {
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
        if(requestedNode != null)
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
        else if(offerNode != null && requestedNode == null)
          {
            currentPlayer.CollectMoney(requestedMoney);
            nodeOwner.PayMoney(requestedMoney);
            offerNode.ChangeOwner(nodeOwner);
            OnUpdateMessage.Invoke(currentPlayer.name + " sold " + offerNode.name + " to " + nodeOwner.name + " for " + requestedMoney);
          }
    }
}
