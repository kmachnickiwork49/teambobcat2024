using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ChangeLightDirection : MonoBehaviour
{
    [SerializeField] Light2D light2D;
    [SerializeField] float rotationSpeed = 5f;
    Vector3 lastPos;

    // Start is called before the first frame update
    void Start()
    {
        lastPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 moveDirection = transform.position - lastPos;
        if (moveDirection != Vector3.zero)
        {
            float targetAngle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg - 90;
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
            light2D.transform.rotation = Quaternion.Slerp(light2D.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        lastPos = transform.position;
    }
}
