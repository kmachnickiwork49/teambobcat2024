using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FlickerLight : MonoBehaviour
{
    [SerializeField] Light2D light2D;
    [SerializeField] float offIntensity = 0.1f;
    [SerializeField] float onIntensity = 1f;
    [SerializeField] float minSwitchTime = 0.2f;
    [SerializeField] float maxSwitchTime = 0.5f;

    private bool isOn;
    private float timer;

    // Update is called once per frame
    void Update()
    {
        if (timer <= 0)
        {
            timer = Random.Range(minSwitchTime, maxSwitchTime);
            isOn = !isOn;
            if (isOn) {
                light2D.intensity = onIntensity;
            } else
            {
                light2D.intensity = offIntensity;
            }
        }
        timer -= Time.deltaTime;
    }
}
