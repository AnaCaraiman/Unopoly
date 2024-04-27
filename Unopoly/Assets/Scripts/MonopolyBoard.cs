using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.UIElements;

public class MonopolyBoard : MonoBehaviour
{

    public List<MonopolyNode> route = new List<MonopolyNode>();

    [System.Serializable]
    public class Node
    
    
    [SerializeField] List<>



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

    IEnumerator MovePlayerInSteps(int steps, Player player)
    {
        int stepsLeft = steps;
        GameObject tokenToMove = player.MyToken;
        int indexOnBoard = route.IndexOf(player.MyMonopolyNode);
        bool moveOverGo = false;
        while (stepsLeft>0)
        {
            indexOnBoard++;
            if (indexOnBoard > route.Count + 1)
            {
                indexOnBoard = 0;
                moveOverGo = true;
            }

            Vector3 startPos = tokenToMove.transform.position;
            Vector3 endPos = route[indexOnBoard].transform.position;
            while(MoveToNextNode(tokenToMove,endPos,20))
            {
                yield return null;
            }
            stepsLeft--;
        }
        if(moveOverGo)
        {
            //player.CollectMoney(GameManager.instance.GetMoney);
        }
        player.SetMyCurrentNode(route[indexOnBoard]);
    }

    bool MoveToNextNode(GameObject tokenToMove, Vector3 endPos, float speed)
    {
        return endPos != (tokenToMove.transform.position = Vector3.MoveTowards(tokenToMove.transform.position,endPos,speed + Time.deltaTime));
    }
}
