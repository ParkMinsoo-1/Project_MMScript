using System.Collections.Generic;

public class StageData
{
    public int ID;
    public int Chapter;
    public float BaseDistance; //기지간의 거리
    public int EnemyBaseHP; //적 기지 체력
    public int EnemyUnit1; 
    public int EnemyUnit2;
    public int EnemyUnit3;
    
    public List<int> firstRewardItemIDs = new();
    public List<int> firstRewardAmounts = new();

    public List<int> repeatRewardItemIDs = new();
    public List<int> repeatRewardAmounts = new();
    
    public string StageName; //스테이지 이름
    public float TeaTime; //웨이브 시작 전 유예시간
    public float ResetTime; //웨이브 길이
    public int EnemyHeroID; //적 영웅 ID
    public string CastleSprite; //적 성 이미지
    public int ActionPoint; //행동력
    public List<string> BGList;

    public int Type ;
    public List<int> GimicID = new();
    public int RaceID ;
    public int BGMName;//브금 번호
}