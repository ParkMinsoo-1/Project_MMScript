using System.Collections.Generic;
using UnityEngine;

public static class BuffManager
{
    public static void InitBuffs()
    {
        PlayerDataManager.Instance.player.buildingBuffs.Clear();

        foreach (int buildingID in BuildManager.Instance.buildingDict.Keys)
        {
            UnitStats buff = new UnitStats
            {
                ID = buildingID,
                RaceID = BuildManager.Instance.buildingDict[buildingID].raceId,
                AttackRange = 1f,
                Damage = 1f,
                MaxHP = 1f,
                MoveSpeed = 1f,
                SpawnInterval = 1f,
                Cost = 1,
                Hitback = 1,
                PreDelay = 1f,
                PostDelay = 1f,
                AttackType = 1,
                Size = 1f,
                tagId = BuildManager.Instance.buildingDict[buildingID].raceIDList,
            };

            PlayerDataManager.Instance.player.buildingBuffs[buildingID] = buff;
        }
    }

    public static UnitStats GetBuff(UnitStats baseStats)
    {
        UnitStats totalBuff = new UnitStats
        {
            // 초기값은 모두 1로 설정
            AttackRange = 1f,
            Damage = 1f,
            MaxHP = 1f,
            MoveSpeed = 1f,
            SpawnInterval = 1f,
            Cost = 1,
            Hitback = 1,
            PreDelay = 1f,
            PostDelay = 1f,
            AttackType = 1,
            Size = 1f
        };

        var buildingBuffs = PlayerDataManager.Instance.player.buildingBuffs;

        foreach (var buff in buildingBuffs.Values)
        {
            bool matchRace = buff.RaceID == baseStats.RaceID;
            bool matchTag = buff.tagId != null && baseStats.tagId != null &&
                            buff.tagId.Exists(tag => baseStats.tagId.Contains(tag));

            if (matchRace || matchTag)
            {
                ApplyBuff(ref totalBuff, buff);
            }
        }

        return totalBuff;
    }


    public static UnitStats ApplyBuff(UnitStats baseStats)//곱연산
    {
        var buff = GetBuff(baseStats);
        if (buff == null) return baseStats;

        return new UnitStats
        {
            ID = baseStats.ID,
            Name = baseStats.Name,
            Description = baseStats.Description,
            RaceID = baseStats.RaceID,
            IsHero = baseStats.IsHero,
            IsAOE = baseStats.IsAOE,
            AttackRange = baseStats.AttackRange * buff.AttackRange,
            Damage = baseStats.Damage * buff.Damage,
            MaxHP = baseStats.MaxHP * buff.MaxHP,
            MoveSpeed = baseStats.MoveSpeed * buff.MoveSpeed,
            SpawnInterval = baseStats.SpawnInterval * buff.SpawnInterval,
            Cost = Mathf.RoundToInt(baseStats.Cost * buff.Cost),
            Hitback = Mathf.RoundToInt(baseStats.Hitback * buff.Hitback),
            PreDelay = baseStats.PreDelay * buff.PreDelay,
            PostDelay = baseStats.PostDelay * buff.PostDelay,
            ModelName = baseStats.ModelName,
            AttackType = baseStats.AttackType,
            Size = baseStats.Size * buff.Size,
            SkillID = baseStats.SkillID,
            projectile = baseStats.projectile,
        };
    }
    private static void ApplyBuff(ref UnitStats total, UnitStats add)//합연산
    {
        total.AttackRange += add.AttackRange - 1f;
        total.Damage += add.Damage - 1f;
        total.MaxHP += add.MaxHP - 1f;
        total.MoveSpeed += add.MoveSpeed - 1f;
        total.SpawnInterval += add.SpawnInterval - 1f;
        total.Cost += add.Cost - 1;
        total.Hitback += add.Hitback - 1;
        total.PreDelay += add.PreDelay - 1f;
        total.PostDelay += add.PostDelay - 1f;
        total.AttackType += add.AttackType - 1;
        total.Size += add.Size - 1f;
    }

    public static void UpdateBuffStat(int buildingID, List<int> statIndex, float value)
    {
        if (!PlayerDataManager.Instance.player.buildingBuffs.ContainsKey(buildingID))
        {
            Debug.LogWarning($"Race ID {buildingID}에 대한 버프가 존재하지 않습니다.");
            return;
        }
        var buff = PlayerDataManager.Instance.player.buildingBuffs[buildingID];
        value *= 0.01f;
        foreach( var i in statIndex ) 
        {
            switch (i)
            {
                case 0: buff.Damage += value; break;
                case 1: buff.MaxHP += value; break;
                case 2: buff.MoveSpeed += value; break;
                case 3: buff.AttackRange += value; break;
                case 4: buff.SpawnInterval += value; break;
                case 5: buff.Cost += Mathf.RoundToInt(value); break;
                case 6: buff.Hitback += Mathf.RoundToInt(value); break;
                case 7: buff.PreDelay += value; break;
                case 8: buff.PostDelay += value; break;
                case 9: buff.Size += value; break;
                default:
                    Debug.LogWarning($"알 수 없는 statIndex: {statIndex}");
                    return;
            }
        }
        PlayerDataManager.Instance.player.buildingBuffs[buildingID] = buff;
    }
}
