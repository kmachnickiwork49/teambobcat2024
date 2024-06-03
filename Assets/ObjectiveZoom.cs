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

        float elapsedTime = 0f;
        while (elapsedTime < zoomDuration)
        {
            float t = QuadraticEaseInOut(elapsedTime / zoomDuration);
            Vector3 newPos = Vector3.Lerp(iPos, fPos, t);
            newPos.z = iPos.z;
            transform.position = newPos;
            mainCamera.orthographicSize = iSize + (fSize - iSize) * t;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = new Vector3(fPos.x, fPos.y, iPos.z);
        yield return new WaitForSeconds(pauseDuration);

        elapsedTime = 0f;
        while (elapsedTime < zoomDuration)
        {
            float t = QuadraticEaseInOut(elapsedTime / zoomDuration);
            Vector3 newPos = Vector3.Lerp(fPos, iPos, t);
            newPos.z = iPos.z;
            transform.position = newPos;
            mainCamera.orthographicSize = fSize + (iSize - fSize) * t;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = iPos;

        yield return new WaitForSeconds(pauseDuration);
        StartCoroutine(DoZoom(iPos, targetIdx + 1, iSize, fSize));
    }
     float QuadraticEaseInOut(float t)
     {
        return t < 0.5 ? 2 * t * t : 1 - Mathf.Pow(-2 * t + 2, 2) / 2;
     }
}
