using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BackHandlerManager : MonoBehaviour
{
    public static BackHandlerManager Instance { get; private set; }

    private readonly List<BackHandlerEntry> entries = new();
    private float backLockedUntil = 0f;

    private bool isBackEnabled = true; // 🔸 명시적 제어 플래그

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (!isBackEnabled || Time.unscaledTime < backLockedUntil)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            var validEntry = entries
                .Where(e => e.IsActive != null && e.IsActive())
                .OrderByDescending(e => e.Priority)
                .FirstOrDefault();

            validEntry?.OnBack?.Invoke();
        }
    }

    public void Register(BackHandlerEntry entry)
    {
        if (!entries.Contains(entry))
            entries.Add(entry);
    }

    public void Unregister(BackHandlerEntry entry)
    {
        entries.Remove(entry);
    }

    public void LockBackFor(float duration)
    {
        backLockedUntil = Time.unscaledTime + duration;
    }

    public void SetBackEnabled(bool enabled)
    {
        isBackEnabled = enabled;
    }

    public bool IsLocked => Time.unscaledTime < backLockedUntil;
    public bool IsBackEnabled => isBackEnabled;
}
