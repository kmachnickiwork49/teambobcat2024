using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StomachMeter : MonoBehaviour
{
    [SerializeField] GameObject stomachMeter;
    [SerializeField] Transform indicator;
    [SerializeField] float minRotation;
    [SerializeField] float maxRotation;

    public void SetSatiation(float ratio)
    {
        indicator.eulerAngles = new Vector3(0, 0, minRotation + (maxRotation - minRotation) * ratio);
    }

    public void SetShow(bool show)
    {
        stomachMeter.SetActive(show);
    }
}
