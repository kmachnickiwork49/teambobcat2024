using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuButton : MonoBehaviour
{
    [SerializeField] MenuDirector menuDirector;
    [SerializeField] float fadeDuration;
    TMP_Text titleText;
    
    void Start()
    {
        titleText = GetComponent<TMP_Text>();
        titleText.color = new Color(titleText.color.r, titleText.color.g, titleText.color.b, 0);
        menuDirector.OnIntroComplete.AddListener(Appear);
    }

    void Appear()
    {
        StartCoroutine(AppearTransition());
        menuDirector.OnIntroComplete.AddListener(Appear);
    }

    IEnumerator AppearTransition() {
        yield return new WaitForSeconds(1);
        for (float t = 0f; t < fadeDuration; t += Time.deltaTime)
        {
            float normalizedTime = t / fadeDuration;
            titleText.color = new Color(titleText.color.r, titleText.color.g, titleText.color.b, normalizedTime);
            yield return null;
        }
        titleText.color = new Color(titleText.color.r, titleText.color.g, titleText.color.b, 1);
    }
}
