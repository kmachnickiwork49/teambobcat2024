using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Title : MonoBehaviour
{
    [SerializeField] MenuDirector menuDirector;
    [SerializeField] float fadeDuration;
    TMP_Text titleText;
    
    void Start()
    {
        titleText = GetComponent<TMP_Text>();
        menuDirector.OnIntroComplete += Vanish;
    }

    void Vanish()
    {
        StartCoroutine(VanishTransition());
        menuDirector.OnIntroComplete -= Vanish;
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
