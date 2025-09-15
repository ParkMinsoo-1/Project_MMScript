using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpawnButton : MonoBehaviour
{
    public int unitID;
    [HideInInspector] public bool isEnemy;
    [HideInInspector] public bool isHero;

    public Image cooldownOverlay;
    public Button button;
    public TextMeshProUGUI costText;
    public GameObject iconParent;

    private bool initialized = false;
    public UnitStats stats;


    public void Update()
    {
        if (!initialized || unitID == 0 || stats == null) return;

        float remaining = CoolTimeManager.Instance.GetRemainingCooldown(unitID);
        float total = GetCooldown();

        // 🔹 자원 부족한 경우 → fillAmount를 1로 덮음
        bool isAffordable = BattleResourceManager.Instance.currentResource >= stats.Cost;

        if (!isAffordable)
        {
            cooldownOverlay.fillAmount = 1f;
            button.interactable = false;
            return;
        }

        // 🔹 자원 충분한 경우 → 쿨다운 기준으로 fillAmount 설정
        cooldownOverlay.fillAmount = total > 0 ? remaining / total : 0f;
        button.interactable = remaining <= 0f;
    }


    public void InitializeUI()
    {
        if (unitID == 0)
        {
            return;
        }
        stats = BuffManager.ApplyBuff(UnitDataManager.Instance.GetStats(unitID));
        stats = UnitSpawner.Instance.SetGimmick(stats);
        stats = SkillManager.Instance.OnStartBuff(stats);
        if (stats == null)
        {
            Debug.LogWarning("Stats not found for unitID: " + unitID);
            return;
        }

        if (costText != null)
            costText.text = stats.Cost.ToString();

        if (iconParent != null)
        {
            foreach (Transform child in iconParent.transform)
                Destroy(child.gameObject);

            string path = $"Units/{stats.ModelName}";
            var modelPrefab = Resources.Load<GameObject>(path);
            if (modelPrefab != null)
            {
                var iconInstance = Instantiate(modelPrefab, iconParent.transform);
                iconInstance.transform.localPosition = new Vector3(0f, 0f, -10f);
                iconInstance.transform.localRotation = Quaternion.identity;
                iconInstance.transform.localScale = Vector3.one * 150f;

                DestroyImmediate(iconInstance.GetComponentInChildren<Animator>());
                DestroyImmediate(iconInstance.GetComponentInChildren<SPUM_Prefabs>());
            }
            else
            {
                Debug.LogWarning($"모델 프리팹을 찾을 수 없습니다: {path}");
            }
        }

        initialized = true;
    }

    private float GetCooldown()
    {
        return stats != null ? stats.SpawnInterval : 0f;
    }
}
