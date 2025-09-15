using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerCurrencyEvent
{
    public static Action<int> OnGoldChange;
    public static Action<int> OnTicketChange;
    public static Action<int> OnSpecTicketChange;
    public static Action<int> OnBluePrintChange;
    public static Action<int> OnTributeChange;
    public static Action<int> OnActionPointChange;
    public static Action<int> OnCertiChange;
}
