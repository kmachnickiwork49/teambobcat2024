using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTowards : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float maxSpeed = 2f;
    [SerializeField] float acceleration = 2f;
    [SerializeField] float followDistance = 0.001f;
    [SerializeField] float range;
    [SerializeField] bool debugMode = true;

    [SerializeField] BobReportMovement brm;

    float currentSpeed;
    bool isEnabled = false;
    Action<bool> onStop;

    private void OnDrawGizmos()
    {
        if (!debugMode) return;
        RangeUtils.VisualizeRange(transform, range);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isEnabled) return;
        bool followDistanceReached = RangeUtils.TargetInRange(transform, target, followDistance);
        if (!TargetInRange() || followDistanceReached)
        {
            currentSpeed = 0;
            isEnabled = false;
            if (onStop != null) onStop?.Invoke(followDistanceReached);
            return;
        }

        float desiredSpeed = Mathf.Min(maxSpeed, currentSpeed + acceleration * Time.deltaTime);
        Vector3 direction = (new Vector3(target.position.x, target.position.y, transform.position.z)  - transform.position);
        Vector3 movement = desiredSpeed * Time.deltaTime * direction.normalized;
        transform.position += movement;
        currentSpeed = desiredSpeed;

        bool movingRight = direction.x > 0;
        bool movingUp = direction.y > 0;
        if (brm != null) {
            brm.SetMovingRight(movingRight);
            brm.SetMovingUp(movingUp);
            brm.SetStopped(false);
        }
    }

    public bool TargetInRange()
    {
        return RangeUtils.TargetInRange(transform, target, range);
    }

    // callback with bool indicating if target reached
    public void Enable(Action<bool> onStopCallback = null)
    {
        isEnabled = true;
        onStop = onStopCallback;
    }

    public void Disable()
    {
        isEnabled = false;
    }
}
