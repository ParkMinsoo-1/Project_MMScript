using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldShopInit : MonoBehaviour
{
    public GameObject purchaseUI;

    private void OnDisable()
    {
        purchaseUI.SetActive(false);
    }
}
