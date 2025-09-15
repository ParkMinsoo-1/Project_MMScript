using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QuestType
{
    Daily,
    Weekly
}
public enum ConditionType
{
    UseActionPoint,
    Looting,
    Login,
    Recruit,
    Tower,
    MainChapter
}
public enum RewardType
{
    Gold,
    Ticket,
    BluePrint
}
public class QuestData
{
    public int ID;
    public string Title;
    public QuestType Type;
    public ConditionType ConditionType;
    public int ConditionValue;
    public RewardType RewardType;
    public int RewardValue;
    public int Order;
}
