using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class MenuDirector : MonoBehaviour
{
    public UnityEvent OnIntroComplete;
    [SerializeField] float introWait;
    [SerializeField] Canvas creditsCanvas;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(TriggerEvent(introWait, OnIntroComplete));
    }

    IEnumerator TriggerEvent(float waitTime, UnityEvent action) { 
        yield return new WaitForSeconds(waitTime);
        action?.Invoke();
    }

    public void Play() { 
        SceneManager.LoadSceneAsync("TutorialLevelWinterDemo");
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void SetShowCredits(bool value)
    {
        creditsCanvas.gameObject.SetActive(value);
    }
}
