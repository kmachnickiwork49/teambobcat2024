using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobOrientationGeneric : MonoBehaviour
{
    [SerializeField] AnimatorControllerParameter upLeft;
    [SerializeField] AnimatorControllerParameter downLeft;
    Animator animator;
    [SerializeField] BobReportMovement pathfinding;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        animator.SetBool("movingUp", pathfinding.GetMovingUp());
        animator.SetBool("stopped", pathfinding.GetStopped());
        transform.localScale = new(
            0.1f * (pathfinding.GetMovingRight() ? -1 : 1),
            transform.localScale.y,
            1);
    }

}
