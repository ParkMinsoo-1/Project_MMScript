using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

// 내가 가지고 있는 유닛 리스트. 인벤토리와 같은 역할. 뽑기 진행시 유닛을 추가 시킬 곳.
public class MyUnitList 
{
    //private static MyUnitList instance;
    //public static MyUnitList Instance
    //{
    //    get
    //    {
    //        if(instance == null)
    //        {
    //            instance = new MyUnitList();
    //        }
    //        return instance;
    //    }
    //}
    
    //private List<int> myList = new();

    //public bool AddUnit (int id) // 유닛 추가 메서드. 유닛의 동일한 아이디 확인해서 bool 값 반환.
    //{
    //    if (myList.Contains(id))
    //    {
    //        Debug.Log("이미 동일한 유닛이 존재합니다.");
    //        return false;
    //    }

        
    //    myList.Add(id);
        
    //    return true;
    //}

    //public bool HasUnit(int id) // 보유중인지 체크
    //{
    //    return myList.Contains(id);

    //}

    //public List<UnitStats> GetAllNormalUnit() // 현재 보유 유닛 리스트에서 일반 유닛만 리스트화 하기. UI 필터 적용. 추가 기능 더 필요함.
    //{
    //    return myList.Select(id => UnitDataManager.Instance.GetStats(id))
    //        .Where(stat => stat != null && !stat.IsHero).ToList()
    //        .ToList();
    //}

    //public List<UnitStats> GetAllLeaderUnit() // 현재 보유 유닛 리스트에서 리더 유닛만 리스트화 하기.
    //{
    //    return myList.Select(id => UnitDataManager.Instance.GetStats(id))
    //        .Where(stat => stat != null && stat.IsHero).ToList()
    //        .ToList();
    //}

    //public List<UnitStats> GetAllUnit() // 현재 보유 중인 모든 유닛 리스트 반환.
    //{
    //    return myList.Select(id => UnitDataManager.Instance.GetStats(id))
    //        .Where(stat => stat != null)
    //        .ToList();
    //}

    /// <summary>
    /// 현재 게임을 껐다가 켰을 때 이 전의 데이터를 저장해야 할 필요가 있을까? 굳이? 없어도 되는 함수긴 함.
    /// </summary>
    //public void SaveMyList() // 나의 보유 유닛 리스트 저장. 동일한 키값으로 이전의 값을 계속 덮어 씌우는 것이기 때문에, 뽑기 후에 진행.
    //{
    //    string json = JsonUtility.ToJson(this);
    //    PlayerPrefs.SetString("MyUnitList", json);
    //}

    //public static MyUnitList LoadMyList() 
    //{
    //    string json = PlayerPrefs.GetString("MyUnitList", "");
    //    if (string.IsNullOrEmpty(json)) return new MyUnitList();
    //    return JsonUtility.FromJson<MyUnitList>(json);
    //}
}
