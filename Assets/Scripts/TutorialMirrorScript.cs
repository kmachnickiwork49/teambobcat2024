using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TutorialMirrorScript : MonoBehaviour
{
    
    [SerializeField] private Tilemap my_tm;
    [SerializeField] private TileBase lighted_square;
    private int angleIndex = 0;

    [SerializeField] private TargetSelection targetSelection;
    [SerializeField] private int range;

    [SerializeField] GameObject wall_crash;

    private Vector3Int my_targ;

    public Vector3Int GetMyTarg() {
        if (angleIndex == 0) { return targetSelection.GetRandomTile(); }
        return my_targ;
    }

    void OnMouseDown() {
        Debug.Log("mouse clicked on mirror");
        int[] my_angles = {0, 15, 30, 45, 60, 75};

        if (angleIndex < 5) {
            angleIndex++;
        }

        // Old
        //my_tm.ClearAllTiles();
        //my_tm.SetTile(new Vector3Int(9+angleIndex-1,5,0), lighted_square);

        // New
        Vector3Int selfCellPosition = my_tm.WorldToCell(targetSelection.transform.position);
        for (int x = -range; x <= range; x++)
        {
            for (int y = -range; y <= range; y++)
            {
                Vector3Int cellPosition = new(
                    selfCellPosition.x + x,
                    selfCellPosition.y + y,
                    selfCellPosition.z);
                TileBase tile = my_tm.GetTile(cellPosition);
                if (tile != null)
                {
                    targetSelection.ModifyForbiddenTile(cellPosition, true);
                }
            }
        }
        targetSelection.ModifyForbiddenTile(new Vector3Int(9+angleIndex-1,5,0), false); // Now whenever call targetSelection.GetRandomTile() should be target
        targetSelection.SelectTile(new Vector3Int(9+angleIndex-1,5,0)); 
        my_targ = new Vector3Int(9+angleIndex-1,5,0);

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
        } else if (angleIndex == 5) {
            targetSelection.ModifyForbiddenTile(new Vector3Int(13,5,0), false);
            wall_crash.SetActive(true);
        }

    }

}
