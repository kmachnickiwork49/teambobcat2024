using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WetSpot : MonoBehaviour
{
    private Color offColor = new(0, 0, 0, 0);
    [SerializeField] private Color onColor;
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
        spriteRenderer.color = isOn ? onColor : offColor;
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
        startColor = spriteRenderer.color;
        endColor = onVal ? onColor : offColor;
        isOn = onVal;
        elapsedTime = 0f;
    }
}
