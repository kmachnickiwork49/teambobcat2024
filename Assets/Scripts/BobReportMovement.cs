using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobReportMovement : MonoBehaviour
{

    private bool movingUp;
    private bool movingRight;
    private bool isStopped;

    void Start() {
        movingUp = false;
        movingRight = false;
        isStopped = false;
    }

    public bool GetMovingUp()
    {
        return movingUp;
    }

    public bool GetStopped() {
        return isStopped;
    }

    public bool GetMovingRight()
    {
        return movingRight;
    }

    public void SetMovingUp(bool mu)
    {
        movingUp = mu;
    }

    public void SetStopped(bool st) {
        isStopped = st;
    }

    public void SetMovingRight(bool mr)
    {
        movingRight = mr;
    }

}
