using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BaseCastle : MonoBehaviour
{
    public bool isEnemy;

    private int maxHP = 1000;
    private int currentHP;

    public TMP_Text hpText;
    public Image hpGauge;

    public Collider2D hitCollider;

    private bool isDestroyed = false;

    public SpriteRenderer sprite;

    private void Start()
    {
        Debug.Log(isEnemy);
        SetMaxHP();
        currentHP = maxHP;
        UpdateUI();
        sprite.sprite = isEnemy ? Resources.Load<Sprite>($"Sprites/{WaveManager.Instance.currentStage.CastleSprite}") : Resources.Load<Sprite>("Sprites/Castle_Ally");
    }
    public void SetMaxHP()
    {
        maxHP = isEnemy?WaveManager.Instance.currentStage.EnemyBaseHP : 500; // 적 기지 : 아군 기지
    }
    public void TakeDamage(int amount)
    {
        if (isDestroyed) return;

        if (currentHP == maxHP&&isEnemy)//처음 성이 맞아서 데미지를 입을때만 실행
            WaveManager.Instance.TriggerWave();

        currentHP -= amount;
        if (currentHP <= 0)
        {
            currentHP = 0;
            OnDestroyed();
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        if (hpText != null)
            hpText.text = $"{currentHP} / {maxHP}";

        if (hpGauge != null)
            hpGauge.fillAmount = (float)currentHP / maxHP;
    }

    void OnDestroyed()
    {
        isDestroyed = true;

        // 유닛 통과 가능하게 충돌 제거
        if (hitCollider != null)
            hitCollider.enabled = false;

        Debug.Log(isEnemy ? "완전 승리!!" : "패배..");
        // 게임 종료 처리
        BattleManager.Instance.OnBaseDestroyed(isEnemy);
    }
}
