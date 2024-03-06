using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuTreehouse : MonoBehaviour
{
    [SerializeField] MenuDirector menuDirector;
    [SerializeField] float fadeDuration;
    SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        menuDirector.OnIntroComplete += SwitchToInterior;
    }

    void SwitchToInterior() {
        StartCoroutine(FadeOut());
        menuDirector.OnIntroComplete -= SwitchToInterior;
    }

    IEnumerator FadeOut()
    {
        for (float t = 0f; t < fadeDuration; t += Time.deltaTime)
        {
            float normalizedTime = t / fadeDuration;
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1 - normalizedTime);
            yield return null;
        }
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0);
    }
}
