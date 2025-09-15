using UnityEngine;

[System.Serializable]
public class Item

{
    public int ID;                 //아이템 ID
    public string Name;            //아이템 이름
    public int Cost;               //아이템 가격
    public string Description;     //아이템 설명
    public int DailyBuy;          //아이템 구매 횟수 
    public string ItemIcon;        //아이템 아이콘 
}