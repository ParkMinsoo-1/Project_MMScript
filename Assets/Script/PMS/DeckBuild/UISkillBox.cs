using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISkillBox : MonoBehaviour
{
    [SerializeField] private Image activeIcon;
    [SerializeField] private Image passiveIcon;

    [SerializeField] private Sprite activeSprite;
    [SerializeField] private Sprite passiveSprite;

    [SerializeField] private Button activeBtn;
    [SerializeField] private Button passiveBtn;
    [SerializeField] private UISkillInfo skillInfo;

    private SkillData activeSkill;
    private SkillData passiveSkill;

    public void SetSkill(UnitStats data)
    {
        Hide();

        activeSkill = null;
        passiveSkill = null;

        foreach (int skill in data.SkillID)
        {
            SkillData skillData = SkillDataManager.Instance.GetSkillData(skill);

            if (skillData == null)
            {
                Debug.LogWarning($"스킬 ID {skill} 에 대한 데이터를 찾을 수 없습니다!");
                continue;
            }

            Debug.Log($"스킬 ID {skill} / 이름: {skillData.Name} / 타입: {skillData.SkillType}");

            if (skillData.SkillType == "Active")
            {
                activeSkill = skillData;
                activeIcon.sprite = activeSprite;
                activeBtn.onClick.AddListener(OnClickActiveSkill);
            }
            else if (skillData.SkillType == "Passive")
            {
                passiveSkill = skillData;
                passiveIcon.sprite = passiveSprite;
                passiveBtn.onClick.AddListener(OnClickPassiveSkill);
            }
        }
    }

    private void OnClickActiveSkill()
    {
        if(activeSkill != null)
        {
            skillInfo.ShowSkillInfo(activeSkill);
        }
        else
        {
            Debug.Log("액티브 스킬데이터가 없습니다.");
        }
    }

    private void OnClickPassiveSkill()
    {
        if (passiveSkill != null)
        {
            skillInfo.ShowSkillInfo(passiveSkill);
        }
        else
        {
            Debug.Log("패시브 스킬데이터가 없습니다.");
        }
    }

    public void Hide()
    {
        skillInfo.HideSkillInfo();
    }
}
