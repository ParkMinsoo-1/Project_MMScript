using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIPresetSlot : MonoBehaviour
{
    //[SerializeField] Button preset1;
    private PresetMover presetMover;

    private void Awake()
    {
        presetMover = GetComponent<PresetMover>();
    }


    
}