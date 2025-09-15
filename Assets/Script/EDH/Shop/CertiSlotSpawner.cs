using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CertiSlotSpawner : MonoBehaviour
{
    [SerializeField] private GameObject CertiSlot;
    [SerializeField] private Transform Content;


    void OnEnable()
    {
        ShowCertiUnitCard();
    }

    public void ShowCertiUnitCard()
    {
        foreach (Transform child in Content)
        {
            Destroy(child.gameObject);
        }

        var PicklistDo = PickUpListLoader.Instance.GetAllPickList().Values.ToList();

        List<PickInfo> Alliance = new();
        foreach (PickInfo pickInfo in PicklistDo)
        {
            if (!pickInfo.IsEnemy)
            {
                Alliance.Add(pickInfo);
            }
            Debug.Log(Alliance.Count + "존재");
        }
                                                                                                                                                                                                
        foreach (var pick in Alliance)
        {
            CreateCard(pick, Content);
        }
    }

    private void CreateCard(PickInfo pickInfo, Transform parent)
    {
        GameObject go = Instantiate(CertiSlot, parent);
        CertiSlot slot = go.GetComponent<CertiSlot>();
        slot.init(pickInfo);
    }
}
