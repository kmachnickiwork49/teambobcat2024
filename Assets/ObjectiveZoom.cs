using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveZoom : MonoBehaviour
{
    [SerializeField] float endSize = 2f;
    [SerializeField] float zoomDuration = 3f;
    [SerializeField] float pauseDuration = 2f;
    [SerializeField] List<Transform> targets;
    Vector3 initialPos;
    float initialSize;
    Camera mainCamera;

    void Start()
    {
        initialPos = transform.position;
        mainCamera = Camera.main;
        initialSize = mainCamera.orthographicSize;
        StartCoroutine(DoZoom(initialPos, 0, initialSize, endSize));
    }

    IEnumerator DoZoom(Vector3 iPos, int targetIdx, float iSize, float fSize)
    {
        if (targetIdx == targets.Count) { yield break; }
        Vector3 fPos = targets[targetIdx].position;

        StartCoroutine(CameraUtils.MoveAndZoomCamera(iPos, fPos, iSize, fSize, zoomDuration));
        yield return new WaitForSeconds(zoomDuration);
        
        yield return new WaitForSeconds(pauseDuration);

        StartCoroutine(CameraUtils.MoveAndZoomCamera(fPos, iPos, fSize, iSize, zoomDuration));
        yield return new WaitForSeconds(zoomDuration);

        yield return new WaitForSeconds(pauseDuration);
        StartCoroutine(DoZoom(iPos, targetIdx + 1, iSize, fSize));
    }
}
