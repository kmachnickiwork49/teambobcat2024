using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CameraUtils
{
    public static IEnumerator MoveAndZoomCamera(
        Vector3 iPos, Vector3 fPos, 
        float iSize, float fSize, 
        float duration)
    {
        Camera cam = Camera.main;
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float t = QuadraticEaseInOut(elapsedTime / duration);
            Vector3 newPos = Vector3.Lerp(iPos, fPos, t);
            newPos.z = -10;
            cam.transform.position = newPos;
            cam.orthographicSize = iSize + (fSize - iSize) * t;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        cam.transform.position = new(fPos.x, fPos.y, -10);
        cam.orthographicSize = fSize;
    }

     public static float QuadraticEaseInOut(float t)
     {
        return t < 0.5 ? 2 * t * t : 1 - Mathf.Pow(-2 * t + 2, 2) / 2;
     }
}
