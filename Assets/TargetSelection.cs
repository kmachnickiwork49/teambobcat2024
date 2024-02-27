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

    void Start()
    {
        candidateTiles = new List<Vector3Int>();
        for (int x = -range; x <= range; x++) {
            for (int y = -range; y <= range; y++) {
                candidateTiles.Add(new Vector3Int(-1,-1,-1));
            }
        }
    }

    public void SelectTile(Vector3Int selected) 
    {
        selectedTile = selected;
    }

    public Vector3Int GetTarget()
    {
        if (selectedTile.HasValue) return selectedTile.Value;

        return GetRandomTile();
    }

    Vector3Int GetRandomTile() 
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

                if (tile != null)
                {
                    candidateTiles[tilesLen] = cellPosition;
                    tilesLen += 1;
                }
            }
        }
        Vector3Int chosenTile = candidateTiles[Random.Range(0, tilesLen)];
        return chosenTile;
    }
}
