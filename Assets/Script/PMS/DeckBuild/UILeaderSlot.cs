using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILeaderSlot : MonoBehaviour
{
    [SerializeField] GameObject leaderPannel;
    [SerializeField] GameObject blocker;
    public void OnClickLeader()
    {
        blocker.SetActive(true);
        leaderPannel.SetActive(true);
        UILeaderUnitPannel.instance.Init();
    }
}
