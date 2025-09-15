using System.Collections;
using UnityEngine;

public class TouchBlocker : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(HideAfterDelay());
    }
    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(3f);
        gameObject.SetActive(false);
    }
}
