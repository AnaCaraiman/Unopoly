using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.UIElements;

public class MonopolyBoard : MonoBehaviour
{
    public static MonopolyBoard instance;

    public List<MonopolyNode> route = new List<MonopolyNode>();

    [System.Serializable]
    public class NodeSet

    {
        
        public Color setColor = Color.white;
     public List<MonopolyNode> nodesInSetList = new List<MonopolyNode>();
    }
    
    
    public List<NodeSet> nodeSetList= new List<NodeSet>();

    void Awake()
    {
        instance = this;
    }

   void OnValidate()
    {
        route.Clear();
        foreach (Transform node in transform.GetComponentInChildren<Transform>())
        {
            MonopolyNode monopolyNode = node.GetComponent<MonopolyNode>();
            if (monopolyNode != null)
            {
                route.Add(monopolyNode);
            }
        }
        //UpdateAll Nodee Color
       /* for (int i = 0; i < nodeSetList.Count; i++)
        {
            for (int j = 0; j < nodeSetList[i].nodesInSetList.Count; j++)
            {
                nodeSetList[i].nodesInSetList[j].UpdateColorField(nodeSetList[i].setColor);
            }
        }
       */
    }

    void OnDrawGizmos()
    {
        if(route.Count > 0)
        {
            for(int i = 0; i < route.Count; i++)
            {
                Vector3 current = route[i].transform.position;
                Vector3 next = (i + 1 < route.Count) ? route[i + 1].transform.position : current;

                if (route[i] != null)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(current, next);
                }
            }
        }
    }

    public void MovePlayerToken( int steps, Player player)
    {
        StartCoroutine(MovePlayerInSteps(steps, player));
    }

    public void MovePlayerToken(MonopolyNodeType type, Player player)
    {
        int indexOfNextNodeType = -1;
        int indexOnBoard = route.IndexOf(player.MyMonopolyNode); //where is the player
        int startSearchIndex = (indexOnBoard + 1) % route.Count;
        int nodeSearches = 0; //amount of fields searched

        while(indexOfNextNodeType == -1 && nodeSearches<route.Count) 
        {
            if (route[startSearchIndex].monopolyNodeType == type) //found the desired type
            {
                indexOfNextNodeType = startSearchIndex;
            }
            startSearchIndex = (startSearchIndex+1) % route.Count;
            nodeSearches++;
        }
        if(indexOfNextNodeType == -1) //security exit
        {
            Debug.Log("No node found");
            return;
        }
        StartCoroutine(MovePlayerInSteps(nodeSearches, player));
    }

    IEnumerator MovePlayerInSteps(int steps, Player player)
    {
        int stepsLeft = steps;
        GameObject tokenToMove = player.MyToken;
        int indexOnBoard = route.IndexOf(player.MyMonopolyNode);
        bool moveOverGo = false;
        bool isMovingForward = steps >0;
        if(isMovingForward)
        {
            while (stepsLeft > 0)
            {
                indexOnBoard++;
                if (indexOnBoard > route.Count - 1)
                {
                    indexOnBoard = 0;
                    moveOverGo = true;
                }

                //Vector3 startPos = tokenToMove.transform.position;
                Vector3 endPos = route[indexOnBoard].transform.position;
                while (MoveToNextNode(tokenToMove, endPos, 20))
                {
                    yield return null;
                }
                stepsLeft--;
            }
        }
        else
        {
            while (stepsLeft < 0)
            {
                indexOnBoard--;
                if (indexOnBoard < 0)
                {
                    indexOnBoard = route.Count - 1;
                }

                //Vector3 startPos = tokenToMove.transform.position;
                Vector3 endPos = route[indexOnBoard].transform.position;
                while (MoveToNextNode(tokenToMove, endPos, 20))
                {
                    yield return null;
                }
                stepsLeft++;
            }
        }   
        
        if(moveOverGo)
        {
            //player.CollectMoney(GameManager.instance.GetMoney);
        }
        player.SetMyCurrentNode(route[indexOnBoard]);
    }

    bool MoveToNextNode(GameObject tokenToMove, Vector3 endPos, float speed)
    {
        return endPos != (tokenToMove.transform.position = Vector3.MoveTowards(tokenToMove.transform.position,endPos,speed * Time.deltaTime));
    }

    public (List<MonopolyNode> list, bool allsame ) PlayerHasAllNodesofSet(MonopolyNode node)
    {
        bool allsame = false;
        foreach(var nodeSet in nodeSetList)
        {
            if(nodeSet.nodesInSetList.Contains(node))
            {  //linq
                allsame = nodeSet.nodesInSetList.All(_node => _node.Owner == node.Owner);
                if (allsame)
                {
                    return (nodeSet.nodesInSetList, allsame);
                }
               
            }
        }
        return (null, allsame);
    }
}
