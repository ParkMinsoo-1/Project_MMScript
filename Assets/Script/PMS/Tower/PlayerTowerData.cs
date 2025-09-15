using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTowerData
{
    public Dictionary<int, int> lastClearFloor = new(); // 종족별 마지막 클리어 층
    public Dictionary<int, int> entryCounts = new(); // 종족별 입장 횟수
    public long lastResetTime = 0;
}
