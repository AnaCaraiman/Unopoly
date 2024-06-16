using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ManageUI : MonoBehaviour
{
    [SerializeField] GameObject managepanel; //toshow and hide the manage panel
    [SerializeField] Transform propertyGrid; //to parent sets to it
    [SerializeField] GameObject propertySetPrefab; //

    Player playerReference; 
    List<GameObject> propertyPrefabs = new List<GameObject>();

    void Start()
    {
        managepanel.SetActive(false);
    }
    public void OpenManager()//call from button

    {

        playerReference = GameManager.instance.GetCurrentPlayer;
        //Ger all nodes as node sets
        List<MonopolyNode> processedSet = null;
        foreach ( var node in playerReference.GetMonopolyNodes)
        {
            var (list,allsame) = MonopolyBoard.instance.PlayerHasAllNodesofSet(node);
            List<MonopolyNode> nodeSet = list;
            if (nodeSet != null && nodeSet != processedSet)
            {  //update processed first
                processedSet = nodeSet;

                nodeSet.RemoveAll(n => n.Owner != playerReference);

                //create prefab with all nodes owned by player
                GameObject newPropertyset = Instantiate(propertySetPrefab, propertyGrid, false);
                newPropertyset.GetComponent<ManagePropertyUI>().SetProperty(nodeSet, playerReference);

                propertyPrefabs.Add(newPropertyset);
                


            }
        }
        
        managepanel.SetActive(true);

    }

    public void CloseManager()//call from button
    {
        managepanel.SetActive(false);
    }
}
