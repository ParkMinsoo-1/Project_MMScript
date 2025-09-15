  using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopInit : MonoBehaviour
{
    public GameObject goldStore;
    public GameObject certStore;
    public GameObject purchaseUI;

    private void OnDisable()
    {
        goldStore.SetActive(true);
        certStore.SetActive(false);
        purchaseUI.SetActive(false);
    }
}