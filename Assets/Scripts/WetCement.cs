using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WetCement : MonoBehaviour
{

    private GameObject my_bob;
    [SerializeField] private Sprite alt_form;

    void Start() {
        my_bob = GameObject.Find("Bob");
    }

    void Update() {
        if (my_bob.transform.position.x < transform.position.x + 0.1f
            && my_bob.transform.position.x > transform.position.x - 0.1f
            && my_bob.transform.position.y < transform.position.y + 0.1f
            && my_bob.transform.position.y > transform.position.y - 0.1f) {
                this.GetComponent<SpriteRenderer>().sprite = alt_form;
            }
    }
}
