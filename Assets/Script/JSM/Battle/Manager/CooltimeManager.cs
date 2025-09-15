using UnityEngine;
using System.Collections.Generic;

public class CoolTimeManager : MonoBehaviour
{
    public static CoolTimeManager Instance { get; private set; }

    private readonly Dictionary<int, float> cooldowns = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public bool CanSpawn(int unitID)
    {
        return !cooldowns.ContainsKey(unitID) || Time.time >= cooldowns[unitID];
    }

    public void SetCooldown(int unitID, float cooldownDuration)
    {
        cooldowns[unitID] = Time.time + cooldownDuration;
    }

    public float GetRemainingCooldown(int unitID)
    {
        if (!cooldowns.ContainsKey(unitID)) return 0f;
        return Mathf.Max(0f, cooldowns[unitID] - Time.time);
    }

    public void ClearAllCooldowns()
    {
        cooldowns.Clear();
    }
}
