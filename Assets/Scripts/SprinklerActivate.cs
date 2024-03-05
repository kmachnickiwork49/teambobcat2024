using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SprinklerActivate : MonoBehaviour
{

    [SerializeField] private int negBoundX, negBoundY, posBoundX, posBoundY;
    [SerializeField] private Tilemap my_tm;

    private bool triggered = false;
    void OnMouseDown() {
        Debug.Log("mouse clicked sprinkler activate");
        triggered = true;

        for (int x = negBoundX; x <= posBoundX; x++)
        {
            for (int y = negBoundY; y <= posBoundY; y++)
            {
                Vector3Int cellPosition = new Vector3Int(
        		    x, 
		            y, 
		            0);

                TileBase tile = my_tm.GetTile(cellPosition);

                if (tile != null)
                {
                    my_tm.SetTile(cellPosition, null); // Remove tile at cellPosition
                    Debug.Log("remove " + cellPosition);
                }
            }
        }

        gameObject.GetComponentInChildren<ParticleSystem>().Play();
    }

    public bool getTriggerVal() {
        return triggered;
    }
}
