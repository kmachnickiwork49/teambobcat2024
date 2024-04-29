using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : MonoBehaviour
{
    [SerializeField] List<Transform> patrolPoints;
    [SerializeField] float maxSpeed = 2f;
    [SerializeField] float acceleration = 2f;
    [SerializeField] float stopTime;
    [SerializeField] float deccelerateDistance = 1f;
    [SerializeField] float closeDistance = 0.1f;
    bool isStopped = false;
    bool isTraversalReversed = false;
    int pointIdx = 0;
    float currentSpeed;

    private void Start()
    {
        currentSpeed = maxSpeed;       
    }

    private void Update()
    {
        if (isStopped) return;
        if (RangeUtils.TargetInRange(transform, patrolPoints[pointIdx], closeDistance))
        {
            pointIdx += isTraversalReversed ? 1 : -1;
            if (pointIdx == patrolPoints.Count || pointIdx == -1)
            {
                isTraversalReversed = !isTraversalReversed;
                pointIdx += isTraversalReversed ? 2: -2;
            }
            StartCoroutine(DoStop());
        }

        Vector3 direction = (patrolPoints[pointIdx].position - transform.position).normalized;
        float desiredSpeed;
        if (RangeUtils.TargetInRange(transform, patrolPoints[pointIdx], deccelerateDistance))
        {
            desiredSpeed = Mathf.Max(0.1f, currentSpeed - acceleration * Time.deltaTime);
        } else
        {
            desiredSpeed = Mathf.Min(maxSpeed, currentSpeed + acceleration * Time.deltaTime);
        }
        Vector3 movement = desiredSpeed * Time.deltaTime * direction;
        transform.position += movement;
        currentSpeed = desiredSpeed;
    }

    IEnumerator DoStop()
    {
        currentSpeed = 0;
        isStopped = true;
        yield return new WaitForSeconds(stopTime);
        isStopped = false;
    }
}
