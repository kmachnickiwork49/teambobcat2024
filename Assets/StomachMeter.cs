using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StomachMeter : MonoBehaviour
{
    [SerializeField] GameObject target;
    [SerializeField] Transform indicator;
    [SerializeField] float minRotation;
    [SerializeField] float maxRotation;
    [SerializeField] float depleteTime = 1f;
    [SerializeField] float fillTime = 3f;

    bool isFilling = false;
    private void OnMouseDown()
    {
        //target.SetActive(!target.active);
        if (isFilling)
        {
            StartCoroutine(Rotate(0, 1, fillTime));
        } else
        {
            StartCoroutine(Rotate(1, 0, depleteTime));
        }
        isFilling = !isFilling;
    }

    IEnumerator Rotate(float iRatio, float fRatio, float duration) {
        float r = iRatio;
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            r -= (iRatio - fRatio) * Time.deltaTime / duration;
            indicator.eulerAngles = new Vector3(0, 0, minRotation + (maxRotation - minRotation) * r);
            yield return null;
        }
        //indicator.eulerAngles = new Vector3(0, 0, minRotation + (maxRotation - minRotation) * fRatio);
    }

}
