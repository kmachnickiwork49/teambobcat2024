using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SprinklerActivatePF : MonoBehaviour
{
    [SerializeField] private int range;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private TargetSelection targetSelection;
    [SerializeField] private bool debugMode;
    private GameObject sprinkles;
    private WetSpot wetSpot;

    void Start()
    {
        sprinkles = transform.Find("Sprinkles").gameObject;
        wetSpot = GetComponentInChildren<WetSpot>();
        sprinkles.SetActive(false);
        wetSpot.SetIsOn(false);
    }

    private bool triggered = false;
    void OnMouseDown() {
        Debug.Log("mouse clicked sprinkler activate");
        triggered = !triggered;
        sprinkles.SetActive(triggered);
        wetSpot.SetIsOn(triggered);
		Vector3Int selfCellPosition = tilemap.WorldToCell(transform.position);
        for (int x = -range; x <= range; x++)
        {
            for (int y = -range; y <= range; y++)
            {
                Vector3Int cellPosition = new(
                    selfCellPosition.x + x,
                    selfCellPosition.y + y,
                    selfCellPosition.z);
                TileBase tile = tilemap.GetTile(cellPosition);
                if (tile != null)
                {
                    targetSelection.ModifyForbiddenTile(cellPosition, triggered);
                }
                if (debugMode)
                {
                    TileFlags currentFlags = tilemap.GetTileFlags(cellPosition);
                    tilemap.SetTileFlags(cellPosition, currentFlags & ~TileFlags.LockColor);
                    tilemap.SetColor(cellPosition, triggered ? new Color(0.95f, 0.95f, 1f) : new Color(1f, 1f, 1f));
                }
            }
        }
    }

    public bool getTriggerVal() {
        return triggered;
    }
}
