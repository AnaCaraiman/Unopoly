using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

using TMPro;

public class ManageUI : MonoBehaviour
{
    public static ManageUI instance;

    [SerializeField] GameObject managepanel; //toshow and hide the manage panel
    [SerializeField] Transform propertyGrid; //to parent sets to it
    [SerializeField] GameObject propertySetPrefab; //

    Player playerReference; 
    List<GameObject> propertyPrefabs = new List<GameObject>();
    [SerializeField] TMP_Text yourMoneyText;
    [SerializeField] TMP_Text systemMessageText;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        managepanel.SetActive(false);
    }
    public void OpenManager()//call from button

    {

        playerReference = GameManager.instance.GetCurrentPlayer;
        CreateProperties();
        
        managepanel.SetActive(true);
        UpdateMoneyText();

    }

    public void CloseManager()//call from button
    {
        managepanel.SetActive(false);
        ClearProperties();
    }
    void ClearProperties()
    {
        for(int i = propertyPrefabs.Count - 1; i >= 0; i--)
        {
            Destroy(propertyPrefabs[i]);
        }
        propertyPrefabs.Clear();
    }
    void CreateProperties()
    {
        //Ger all nodes as node sets
        List<MonopolyNode> processedSet = null;
        foreach (var node in playerReference.GetMonopolyNodes)
        {
            var (list, allsame) = MonopolyBoard.instance.PlayerHasAllNodesofSet(node);
            List<MonopolyNode> nodeSet = new List<MonopolyNode>();
            nodeSet.AddRange(list);
            if (nodeSet != null && list != processedSet)
            {  //update processed first
                processedSet = list;

                nodeSet.RemoveAll(n => n.Owner != playerReference);

                //create prefab with all nodes owned by player
                GameObject newPropertyset = Instantiate(propertySetPrefab, propertyGrid, false);
                newPropertyset.GetComponent<ManagePropertyUI>().SetProperty(nodeSet, playerReference);

                propertyPrefabs.Add(newPropertyset);



            }
        }
    }
    public void UpdateMoneyText()
    {
        string showMoney = (playerReference.ReadMoney >= 0)? "<color=green>RON" + playerReference.ReadMoney:"<color=red>RON" + playerReference.ReadMoney;
        yourMoneyText.text = "<color=black>Your Money:</color> " + showMoney;
    }
    public void UpdateSystemMessage(string message)
    {
        systemMessageText.text = message;
    }
    public void AutoHandleFunds()
    {
        if(playerReference.ReadMoney > 0)
        {
            UpdateSystemMessage("You don't need to do that, you have enough money!");
            return;
        }
        playerReference.HandleInsuficientFunds(Mathf.Abs(playerReference.ReadMoney));
        //UPDATE THE UI
        ClearProperties();
        CreateProperties();
        //UPDATE SYSTEM MESSAGE
        UpdateMoneyText();
    }
}
