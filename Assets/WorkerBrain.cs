using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerBrain : MonoBehaviour
{
    [SerializeField] StomachMeter stomachMeter;
    [SerializeField] MoveTowards moveTowardsTarget;
    [SerializeField] MoveTowards moveTowardsStart;
    [SerializeField] GameObject thoughtBubble;
    [SerializeField] GameObject stomachIcon;
    [SerializeField] GameObject eyeIcon;
    [SerializeField] GameObject confuseIcon;
    [SerializeField] GameObject fullIcon;
    [SerializeField] private float depleteTime = 1f;
    [SerializeField] private float fillTime = 3f;
    private float satiation = 1.0f;
    private float? thoughtBubbleTimer = 0f;

    private void Start()
    {
        gameObject.tag = "Scary";
        HideIcons(true);   
    }

    private void Update()
    {
        if (!thoughtBubbleTimer.HasValue) return;
        if (thoughtBubble.activeSelf && thoughtBubbleTimer <= 0f)
        {
            HideIcons(true);
        }

        if (thoughtBubbleTimer > 0f) thoughtBubbleTimer -= Time.deltaTime;
    }

    private void OnMouseDown()
    {
        StartCoroutine(ChangeSatiation(1, 0, depleteTime, () => 
        { 
            stomachMeter.SetShow(false); 
            if (moveTowardsTarget.TargetInRange())
            {
                gameObject.tag = "Untagged";
                SetIcon(eyeIcon);
                moveTowardsStart.Disable();
                moveTowardsTarget.Enable((truckReached) =>
                {
                    if (!truckReached)
                    {
                        SetIcon(confuseIcon, 2);
                        moveTowardsStart.Enable((_) => { gameObject.tag = "Scary"; });
                    } else
                    {
                        StartCoroutine(ChangeSatiation(0, 1, fillTime, () => { 
                            SetIcon(fullIcon, 3);
                            moveTowardsStart.Enable((_) => { gameObject.tag = "Scary"; }); 
                        }));
                    }
                });
            } else
            {
                SetIcon(confuseIcon, 2);
            }
        }));
    }

    void SetIcon(GameObject icon, float? expireTime = null)
    {
        if (!thoughtBubble.activeSelf) thoughtBubble.SetActive(true);
        HideIcons(false);
        icon.SetActive(true);
        thoughtBubbleTimer = expireTime;
    }

    void HideIcons(bool hideBubble = false)
    {
        if (hideBubble) thoughtBubble.SetActive(false);
        stomachMeter.SetShow(false);
        eyeIcon.SetActive(false);
        confuseIcon.SetActive(false);
        fullIcon.SetActive(false);
    }

    IEnumerator ChangeSatiation(float iRatio, float fRatio, float duration, Action onComplete = null)
    {
        SetIcon(stomachIcon);
        satiation = iRatio;
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            satiation -= (iRatio - fRatio) * Time.deltaTime / duration;
            stomachMeter.SetSatiation(satiation);
            yield return null;
        }
        satiation = fRatio;
        onComplete?.Invoke();
    }
}
