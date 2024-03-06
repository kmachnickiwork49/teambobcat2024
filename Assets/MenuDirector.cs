using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuDirector : MonoBehaviour
{
    public event Action OnIntroComplete;
    [SerializeField] float introWait;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(TriggerEvent(introWait, OnIntroComplete));
    }

    IEnumerator TriggerEvent(float waitTime, Action action) {
        yield return new WaitForSeconds(waitTime);
        action?.Invoke();
        Debug.Log("invoke");
    }
}
