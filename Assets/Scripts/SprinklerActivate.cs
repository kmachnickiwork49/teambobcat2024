using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprinklerActivate : MonoBehaviour
{

    private bool triggered = false;
    void OnMouseDown() {
        Debug.Log("mouse clicked sprinkler activate");
        triggered = true;
    }

    public bool getTriggerVal() {
        return triggered;
    }
}
