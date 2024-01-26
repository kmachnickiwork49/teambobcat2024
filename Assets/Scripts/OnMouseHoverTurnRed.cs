using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnMouseHoverTurnRed : MonoBehaviour
{

void OnMouseOver()
    {
        Debug.Log("hello");
        //gameObject.transform.position = new Vector3(gameObject.transform.position.x + 1, gameObject.transform.position.y, gameObject.transform.position.z);
        gameObject.GetComponent<SpriteRenderer>().color = Color.red;
    }

}
