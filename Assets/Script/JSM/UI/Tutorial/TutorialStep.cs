using UnityEngine;

[System.Serializable]
public class TutorialStep
{
    public string npcName;
    [TextArea] public string dialogue;
    public string triggerEventName;

    public int effectID;
    public bool dialogUp;
    public string highlightTarget;
}
