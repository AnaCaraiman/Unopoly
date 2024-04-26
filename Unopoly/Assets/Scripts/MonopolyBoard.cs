using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonopolyBoard : MonoBehaviour
{

    [SerializeField] List<MonopolyNode> route = new List<MonopolyNode>();

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
}
