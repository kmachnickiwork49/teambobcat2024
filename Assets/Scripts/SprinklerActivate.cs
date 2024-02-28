using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SprinklerActivate : MonoBehaviour
{
    [SerializeField] private int range;
    [SerializeField] private Tilemap tilemap;
    private GameObject sprinkles;

    void Start()
    {
        sprinkles = transform.Find("Sprinkles").gameObject;
        sprinkles.SetActive(false);
    }

    private bool triggered = false;
    void OnMouseDown() {
        Debug.Log("mouse clicked sprinkler activate");
        triggered = !triggered;
        sprinkles.SetActive(triggered);
		Vector3Int selfCellPosition = tilemap.WorldToCell(transform.position);
        for (int x = -range; x <= range; x++)
        {
            for (int y = -range; y <= range; y++)
            {
                Vector3Int cellPosition = new(
                    selfCellPosition.x + x,
                    selfCellPosition.y + y,
                    selfCellPosition.z);
                TileFlags currentFlags = tilemap.GetTileFlags(cellPosition);
                tilemap.SetTileFlags(cellPosition, currentFlags & ~TileFlags.LockColor);
                //Debug.Log(tilemap.GetTile(cellPosition));
                tilemap.SetColor(cellPosition, triggered ? new Color(0.95f, 0.95f, 1f) : new Color(1f, 1f, 1f));
            }
        }
    }

    public bool getTriggerVal() {
        return triggered;
    }
}
