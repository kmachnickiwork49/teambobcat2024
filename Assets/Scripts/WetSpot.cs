using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WetSpot : MonoBehaviour
{
    private Color offColor = new(0, 0, 0, 0);
    [SerializeField] private Color onColor = new();
    [SerializeField] private float transitionTime = 1.0f;
    private SpriteRenderer spriteRenderer;
    private float elapsedTime = 0f;
    private bool isOn = false;
    private Color startColor;
    private Color endColor;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) { print("couldnt get spriteRenderer in WetSpot"); }
        spriteRenderer.color = isOn ? onColor : offColor;
        SetIsOn(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (elapsedTime > transitionTime)
        {
            return;
        }
        elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedTime / transitionTime);
        spriteRenderer.color = Color.Lerp(startColor, endColor, t);
    }

    public void SetIsOn(bool onVal)
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); // To prevent error caused by timing constraints
        startColor = spriteRenderer.color;
        endColor = onVal ? onColor : offColor;
        //endColor = onVal ? offColor : onColor;
        isOn = onVal;
        elapsedTime = 0f;
    }
}