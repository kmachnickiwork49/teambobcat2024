using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCamera : MonoBehaviour
{
    [SerializeField] MenuDirector menuDirector;
    [SerializeField] float zoomDuration;
    [SerializeField] float startSize = 5f;
    [SerializeField] float endSize = 3.5f;
    [SerializeField] float startY = 0f;
    [SerializeField] float endY = -0.6f;

    void Start()
    {
        Camera.main.orthographicSize = startSize;
        transform.position = new(transform.position.x, startY, transform.position.z);
        menuDirector.OnIntroComplete += Zoom;
    }

    void Zoom() {
        StartCoroutine(ZoomTransition());
        menuDirector.OnIntroComplete -= Zoom;
    }

    IEnumerator ZoomTransition() { 
        for (float t = 0f; t < zoomDuration; t += Time.deltaTime)
        {
            Camera.main.orthographicSize -= (startSize - endSize) * Time.deltaTime / zoomDuration;
            transform.position -= new Vector3(0, (startY - endY) * Time.deltaTime / zoomDuration, 0);
            yield return null;
        }
        Camera.main.orthographicSize = endSize;
        transform.position = new(transform.position.x,  endY, transform.position.z);
    }
}
