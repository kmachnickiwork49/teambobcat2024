using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TutorialMirrorScript : MonoBehaviour
{
    
    [SerializeField] private Tilemap my_tm;
    [SerializeField] private TileBase lighted_square;
    private int angleIndex = 0;
    void OnMouseDown() {
        Debug.Log("mouse clicked on mirror");
        int[] my_angles = {0, 15, 30, 45, 60};

        if (angleIndex < 4) {
            angleIndex++;
        }

        my_tm.ClearAllTiles();
        my_tm.SetTile(new Vector3Int(9+angleIndex-1,5,0), lighted_square);

        int curr_angle = my_angles[angleIndex];
        if (angleIndex == 0) {
            gameObject.transform.rotation = new Quaternion(0,0,0,1);
        } else if (angleIndex == 1) {
            gameObject.transform.rotation = new Quaternion(0,0.130526155f,0,0.991444886f);
        } else if (angleIndex == 2) {
            gameObject.transform.rotation = new Quaternion(0,0.258819103f,0,0.965925813f);
        } else if (angleIndex == 3) {
            gameObject.transform.rotation = new Quaternion(0,0.382683426f,0,0.923879564f);
        } else if (angleIndex == 4) {
            gameObject.transform.rotation = new Quaternion(0,0.5f,0,0.866025388f);
        }

    }

}
