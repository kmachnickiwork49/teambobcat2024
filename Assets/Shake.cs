using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Shake : MonoBehaviour
{
    [SerializeField] float intensity;

    public void DoShake(float shakeTime, Action shakeCallback = null)
    {
        StartCoroutine(DoShakeRoutine(shakeTime, shakeCallback));
    }

    IEnumerator DoShakeRoutine(float shakeTime, Action shakeCallback)
    {
        Vector3 startPosition = transform.position;
        for (float t = 0f; t < shakeTime; t += Time.deltaTime)
        {
            transform.position = startPosition + UnityEngine.Random.insideUnitSphere * intensity;
            yield return null;
        }
        transform.position = startPosition;
        shakeCallback?.Invoke();
    }
}
