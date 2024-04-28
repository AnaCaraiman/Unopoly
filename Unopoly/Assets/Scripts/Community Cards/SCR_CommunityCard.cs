using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Community Card", menuName = "Monopoly/Cards/Community")]
public class SCR_CommunityCard : ScriptableObject
{
    public string textOnCard;
    public int rewardMoney;
    public int penalityMoney;
    public int moveToBoardIndex = -1;
    public bool collectFromPlayer;
    public bool streetRepairs;
    public bool goToJail;
    public bool getFreeJail;
}
