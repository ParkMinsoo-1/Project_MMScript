using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPresetDeck : MonoBehaviour
{
    //[SerializeField] private UIPresetSlot[] normalSlots;
    //[SerializeField] private UIPresetSlot leaderSlot;

    //private DeckData tempDeck = new DeckData();

    //private void Awake()
    //{
    //    foreach (var slot in normalSlots)
    //    {
    //        slot.SetParentDeck(this, false);
    //    }
    //    leaderSlot.SetParentDeck(this, true);
    //}

    //public void SetDeck(DeckData deck)
    //{
    //    tempDeck = deck.Clone();
    //    RefreshUI();
    //}

    //public DeckData GetTempDeckData()
    //{
    //    return tempDeck;
    //}

    //public void ClearAll()
    //{
    //    tempDeck = new DeckData();
    //    RefreshUI();
       
    //}

    //public void RefreshUI()
    //{
    //    for (int i = 0; i < normalSlots.Length; i++)
    //    {
    //        if (i < tempDeck.deckList.Count)
    //        {
    //            var stats = UnitDataManager.Instance.GetStats(tempDeck.deckList[i].myUnitID);
    //            normalSlots[i].Setup(stats, this, false);
    //        }
    //        else
    //        {
    //            normalSlots[i].Clear();
    //        }
    //    }

    //    if (tempDeck.leaderUnit != null)
    //    {
    //        var leaderStats = UnitDataManager.Instance.GetStats(tempDeck.leaderUnit.myUnitID);
    //        leaderSlot.Setup(leaderStats, this, true);
    //    }
    //    else
    //    {
    //        leaderSlot.Clear();
    //    }
    //}

    //public bool TryAddUnit(UnitStats stats, bool isLeader)
    //{
    //    if (isLeader)
    //    {
    //        if (tempDeck.SetLeaderUnit(stats.ID))
    //        {
    //            RefreshUI();
    //            return true;
    //        }
    //    }
    //    else
    //    {
    //        if (tempDeck.AddNormalUnit(stats.ID))
    //        {
    //            RefreshUI();
    //            return true;
    //        }
    //    }
    //    return false;
    //}

    //public void RemoveUnit(UnitStats stats)
    //{
    //    tempDeck.RemoveUnit(stats.ID);
    //    RefreshUI();
    //}

    //public bool ContainsUnit(int unitID)
    //{
    //    return tempDeck.Contains(unitID);
    //}
}
