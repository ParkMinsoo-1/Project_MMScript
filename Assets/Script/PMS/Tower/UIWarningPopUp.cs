using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWarningPopUp : MonoBehaviour
{
    private Coroutine hideCoroutine;

    private void OnEnable()
    {
        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);

        hideCoroutine = StartCoroutine(HideRoutine());
        SFXManager.Instance.PlaySFX(6);
    }

    private IEnumerator HideRoutine()
    {
        yield return new WaitForSeconds(1f);  
        gameObject.SetActive(false);
        hideCoroutine = null;
    }

}
