using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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
    [SerializeField] TargetSelection targetSelection;
    [SerializeField] Pathfinder pathFinder;
    BobState state;
    bool jackhammersDisabled = false;

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
                if (jackhammersDisabled) break;
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

    public void DisableJackhammers()
    {
        jackhammersDisabled = true;
    }
}
