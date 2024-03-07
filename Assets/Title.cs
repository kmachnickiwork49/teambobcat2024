using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Title : MonoBehaviour
{
    [SerializeField] MenuDirector menuDirector;
    [SerializeField] float appearDuration;
    [SerializeField] float fadeDuration;
    TMP_Text titleText;
    
    void Start()
    {
        titleText = GetComponent<TMP_Text>();
        StartCoroutine(AppearTransition());
        menuDirector.OnIntroComplete.AddListener(Vanish);
    }

    void Vanish()
    {
        StartCoroutine(VanishTransition());
        menuDirector.OnIntroComplete.RemoveListener(Vanish);
    }

    IEnumerator AppearTransition() { 
        for (float t = 0f; t < appearDuration; t += Time.deltaTime)
        {
            float normalizedTime = t / appearDuration;
            titleText.color = new Color(titleText.color.r, titleText.color.g, titleText.color.b, normalizedTime);
            yield return null;
        }
        titleText.color = new Color(titleText.color.r, titleText.color.g, titleText.color.b, 1);
    }

    IEnumerator VanishTransition() { 
        for (float t = 0f; t < fadeDuration; t += Time.deltaTime)
        {
            float normalizedTime = t / fadeDuration;
            titleText.color = new Color(titleText.color.r, titleText.color.g, titleText.color.b, 1 - normalizedTime);
            yield return null;
        }
        titleText.color = new Color(titleText.color.r, titleText.color.g, titleText.color.b, 0);
    }
}
