using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BobState
{
    PATHFIND,
    SHAKE
}

public class BobBrain : MonoBehaviour
{
    [SerializeField] Shake shake;
    [SerializeField] float workerRange;
    [SerializeField] float shakeDuration;
    [SerializeField] MoveTowards moveTowardsStart;
    [SerializeField] Pathfinder pathFinder;
    BobState state;

    private void OnDrawGizmos()
    {
        RangeUtils.VisualizeRange(transform, workerRange);
    }

    private void Start()
    {
        InPathfind();
    }

    private void Update()
    {
        switch (state)
        {
            case BobState.PATHFIND:
                GameObject[] workers = GameObject.FindGameObjectsWithTag("Scary");
                foreach (GameObject worker in workers)
                {
                   if (RangeUtils.TargetInRange(transform, worker.transform, workerRange))
                   {
                       OutPathfind();
                       InShake();
                       break;
                   }
                }
                break;
            case BobState.SHAKE:
                break;
            default:
                break;
        }
    }

    void InPathfind()
    {
        state = BobState.PATHFIND;
        pathFinder.enabled = true;
    }
    void OutPathfind()
    {
        pathFinder.enabled = false;
    }

    void InShake()
    {
        state = BobState.SHAKE;
        shake.DoShake(shakeDuration, () =>
        {
            moveTowardsStart.Enable((_) => { InPathfind(); });
        });
    }
}
