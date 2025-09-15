using System.Collections.Generic;
[System.Serializable]
public class UnitStats
{
    public int ID;
    public string Name;         //유닛 이름
    public string Description;  //유닛 설명
    public int RaceID;          //종족명
    public bool IsHero;         //영웅유닛
    public bool IsAOE;          //범위공격
    public float AttackRange;   //사거리
    public float Damage;        //데미지
    public float MaxHP;         //최대체력
    public float MoveSpeed;     //이동속도
    public float SpawnInterval; //스폰 쿨타임
    public int Cost;            //소환 코스트/보상 코스트
    public int Hitback;         //넉백 체력의 n-1/n ~ 1/n일때 넉백
    public float PreDelay;      //공격 선딜레이
    public float PostDelay;     //공격 후딜레이
    public string ModelName;    //모델링 이름
    public int AttackType;      //공격 타입 0 근접 2 활 4 마법 (+1은 스킬)
    public float Size;          //유닛 크기
    public List<int> SkillID;   //스킬 id
    public bool isEnemy;        //아군적군 구분용
    public int warrant;         //증명서 수
    public int shopPrice;       //상점 가격
    public List<int> tagId;     //태그id 리스트
    public string projectile;   //투사체
}
