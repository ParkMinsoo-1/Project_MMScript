using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    protected Camera cam;
    private Coroutine routine;
    public bool IsFocusing { get; private set; } = false;

    private void Awake() => cam = Camera.main;

    public void FocusOn(Transform target, float zoom)
    {
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(MoveAndZoom(target.position, zoom));
    }

    private IEnumerator MoveAndZoom(Vector3 pos, float zoom)
    {
        Debug.Log(pos);
        IsFocusing = true;
        Vector3 start = cam.transform.position;
        float startZoom = cam.orthographicSize;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime;
            cam.transform.position = Vector3.Lerp(start, new Vector3(pos.x, pos.y, start.z), t);
            cam.orthographicSize = Mathf.Lerp(startZoom, zoom, t);
            yield return null;
        }
    }
}
