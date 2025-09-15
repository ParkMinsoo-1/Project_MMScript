using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GimmickManager : MonoBehaviour
{
    public static GimmickManager Instance;

    // 기믹 데이터 저장용
    public Dictionary<int, GimmickData> gimmickDict = new();
    public GameObject banPanel;
    public Button heroBtn;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    public void ApplyGimmick(int id)
    {
        if (!GimmickDataManager.Instance.gimmickDict.TryGetValue(id, out var data))
        {
            Debug.LogWarning($"[GimmickManager] ID {id} 기믹을 찾을 수 없습니다.");
            return;
        }
        switch (data.Name)
        {
            case "TimeLimit": SetTimeLimit(data.EffectValue); break;
            case "UnitResummonCooldownUp": SetUnitResummonCooldownUp(data.EffectValue); break;
            case "UnitSummonCostUp": SetUnitSummonCostUp((int)data.EffectValue); break;
            case "LeaderUnitSummonBan": SetLeaderUnitSummonBan(); break;
        }
    }

    private void SetTimeLimit(float value)
    {
        WaveManager.Instance.Timer.SetActive(true);
        BattleManager.Instance.timer.timeLimit = value;
        Debug.Log("시간제한!");
    }

    private void SetUnitResummonCooldownUp(float value)
    {
        WaveManager.Instance.SetUnitResummonCooldownUp(value);
        Debug.Log("쿨증!");
    }

    private void SetUnitSummonCostUp(int value)
    {
        WaveManager.Instance.SetUnitSummonCostUp(value);
        Debug.Log("코증!");
    }

    private void SetLeaderUnitSummonBan()
    {
        Debug.Log("리더 밴!");
        banPanel.SetActive(true);
        heroBtn.interactable = false;
        // 제한 처리 로직 필요 시 여기에 구현
    }
}
