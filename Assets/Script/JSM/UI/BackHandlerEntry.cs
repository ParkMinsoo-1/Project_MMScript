public class BackHandlerEntry
{
    public int Priority;
    public System.Func<bool> IsActive;
    public System.Action OnBack;

    public BackHandlerEntry(int priority, System.Func<bool> isActive, System.Action onBack)
    {
        Priority = priority;
        IsActive = isActive;
        OnBack = onBack;
    }
}