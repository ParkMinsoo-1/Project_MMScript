using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIGimmickInfo : MonoBehaviour
{
    public static UIGimmickInfo Instance;

    [SerializeField] private GameObject root;
    [SerializeField] private TextMeshProUGUI gimmickText;
    [SerializeField] private GameObject backGround;

    private BackHandlerEntry entry;

    private void Awake()
    {
        Instance = this;
        root.SetActive(false);
        entry = new BackHandlerEntry(
           priority: 30,
           isActive: () => gameObject.activeInHierarchy,
           onBack: () => gameObject.SetActive(false)
       );
        BackHandlerManager.Instance.Register(entry);
    }

    public void Open(List<string> gimmickNames)
    {
        gimmickText.text = string.Join("\n", gimmickNames);
        root.SetActive(true);
        backGround.SetActive(true);
    }

    public void Close()
    {
        root.SetActive(false);
        backGround.SetActive(false);
    }

    public bool IsOpen()
    {
        return root.activeSelf;
    }
}
