using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTutorialData", menuName = "Tutorial/TutorialData")]
public class TutorialData : ScriptableObject
{
    public List<TutorialStep> steps;
}
