using System.Xml.Linq;
using TMPro;
using UnityEngine;

public class BattleResourceManager : MonoBehaviour
{
    public static BattleResourceManager Instance { get; private set; }

    public TMP_Text moneyText;

    public float currentResource = 0f;
    public int maxResource;
    public int resourcePerSecond;

    public int walletLevel = 1;


    [Header("지갑 설정")]
    [Tooltip("지갑 최대 레벨 (최대 업그레이드 가능 단계)")]
    public int maxWalletLevel = 8;

    [Tooltip("지갑 레벨업 기본 비용")]
    public int basicLevelUpCost = 50;

    [Tooltip("레벨 업그레이드마다 증가하는 비용")]
    public int levelUpBaseCost = 50;

    [Tooltip("1레벨 최대 자원")]
    public int basicWallet = 100;

    [Tooltip("레벨업 시 최대 보유 자원량 증가치")]
    public int increaseWallet = 50;

    [Tooltip("1레벨 자원 획득 속도")]
    public int basicEarn = 5;

    [Tooltip("레벨업 시 자원 획득 속도 증가치")]
    public int increaseEarn = 2;

    private int walletLevelUpCost;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
        walletLevelUpCost = basicLevelUpCost;
        maxResource = basicWallet;
        resourcePerSecond = basicEarn;
}

void FixedUpdate()
    {
        // 실시간 자원 증가
        currentResource += resourcePerSecond * Time.deltaTime;
        currentResource = Mathf.Min(currentResource, maxResource);

        // 실시간 UI 갱신
        if (moneyText != null)
        {
            moneyText.text = Mathf.FloorToInt(currentResource).ToString()+ " / " + maxResource.ToString();
        }
    }

    public void Add(int amount)
    {
        currentResource = Mathf.Min(currentResource + amount, maxResource);
    }

    public bool Spend(int amount)
    {
        if (currentResource < amount) return false;
        currentResource -= amount;
        return true;
    }

    public int GetLevelUpCost()
    {
        return walletLevelUpCost;
    }

    public bool CanLevelUp() => walletLevel < maxWalletLevel;

    public void LevelUp()
    {
        if (!CanLevelUp()) return;

        int cost = GetLevelUpCost();
        if (!Spend(cost)) return;

        walletLevel++;
        maxResource += increaseWallet;
        resourcePerSecond += increaseEarn;
        walletLevelUpCost += levelUpBaseCost;
    }
}
