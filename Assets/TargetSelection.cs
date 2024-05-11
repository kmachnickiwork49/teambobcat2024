using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TargetSelection : MonoBehaviour
{
    [SerializeField] private int range;
    [SerializeField] private Tilemap tilemap;
    private Vector3Int? selectedTile;
    private List<Vector3Int> candidateTiles;

    private HashSet<Vector3Int> forbiddenTiles = new();
    public event EventHandler onForbiddenChanged;

    void Start()
    {
        candidateTiles = new List<Vector3Int>();
        for (int x = -range; x <= range; x++) {
            for (int y = -range; y <= range; y++) {
                candidateTiles.Add(new Vector3Int(-1,-1,-1)); // placeholder tiles
            }
        }
    }

    public void setNewTilemap(Tilemap tm, List<Vector3Int> forbiddenTiles = default) {
        tilemap = tm;
        candidateTiles = new List<Vector3Int>();
        for (int x = -range; x <= range; x++) {
            for (int y = -range; y <= range; y++) {
                candidateTiles.Add(new Vector3Int(-1,-1,-1)); // placeholder tiles
            }
        }
        this.forbiddenTiles = new HashSet<Vector3Int>(forbiddenTiles);
        selectedTile = GetRandomTile();
        Debug.Log("selectedTile: " + selectedTile + " ");
    }

    public Tilemap GetTilemap()
    {
        return tilemap;
    }

    public void SelectTile(Vector3Int? selected) 
    {
        selectedTile = selected;
    }
    public void ModifyForbiddenTile(Vector3Int tile, bool isForbidden)
    {
        if (isForbidden)
        {
            forbiddenTiles.Add(tile);
        } else
        {
            forbiddenTiles.Remove(tile);
        }
        onForbiddenChanged?.Invoke(this, EventArgs.Empty);
    }

    public HashSet<Vector3Int> GetForbiddenTiles()
    {
        return forbiddenTiles;
    }

    public Vector3Int GetTarget()
    {
        if (selectedTile.HasValue && !forbiddenTiles.Contains(selectedTile.Value)) return selectedTile.Value;

        return GetRandomTile();
    }

    public Vector3Int GetRandomTile() 
    {
        Vector3Int selfCellPosition = tilemap.WorldToCell(transform.position);
        int tilesLen = 0;
        for (int x = -range; x <= range; x++)
        {
            for (int y = -range; y <= range; y++)
            {
                if (x==0 && y==0)
                {
                    continue;
                }
                Vector3Int cellPosition = new(
        		    selfCellPosition.x + x, 
		            selfCellPosition.y + y, 
		            selfCellPosition.z - 1);

                TileBase tile = tilemap.GetTile(cellPosition);

                if (tile != null && !forbiddenTiles.Contains(cellPosition))
                {
                    candidateTiles[tilesLen] = cellPosition;
                    tilesLen += 1;
                }
            }
        }
        Vector3Int chosenTile = candidateTiles[UnityEngine.Random.Range(0, tilesLen)];
        return chosenTile;
    }
}
