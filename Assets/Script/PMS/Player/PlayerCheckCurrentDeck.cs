using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerCheckCurrentDeck
{
    public static bool HasUnitsInCurrentDeck()
    {
        var deck = PlayerDataManager.Instance.player.currentDeck;
        var normal = deck.GetAllNormalUnit();
        var leader = deck.GetLeaderUnitInDeck();

        return (leader != null) || (normal != null && normal.Count > 0);
    }
}
