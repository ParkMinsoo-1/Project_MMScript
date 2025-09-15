using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PickInfo
{
    public int    ID;                   //유닛 ID
    public string Name;                 //유닛 이름
    public int raceId;                  //유닛 종족
    public string Description;          //유닛 설명
    public bool   IsHero;               //유닛 타입
    public string Uniticon;             //유닛 아이콘
    public int warrant;                 //유닛 증명서 가치.
    public int duplication;             //중복시 재화량
    public bool IsEnemy;                //피아구분
    public string projectile;
}
