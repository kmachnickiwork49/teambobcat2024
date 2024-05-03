using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CementMixer : MonoBehaviour
{
    // LEVEL 1
    // X and Y BOUNDS: 14 length, 5 width
    // Y_MAX = 3
    // Y_MIN = -10
    // X_MAX = -8
    // X_MIN = -12
    [SerializeField] private Vector3Int[] tilesToCover;
    [SerializeField] private Vector3Int myCoveredTile;
    [SerializeField] private TargetSelection targetSelection;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Tile debugTile;
    [SerializeField] private bool debugMode = false;
    private bool beenClicked = false;
    private bool startPour = false;
    private float pourTimer = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        beenClicked = false;
        startPour = false;
        // Have to make sure myCoveredTile is in correct spot
        // Use z = 0
        //targetSelection.ModifyForbiddenTile(myCoveredTile, true);
        targetSelection.ModifyForbiddenTile(tilemap.WorldToCell(transform.position), true);
        if (debugMode) {
            tilemap.SetTile(myCoveredTile, debugTile);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (beenClicked) {
            startPour = true;
            pourTimer = 0.0f;
        }
        if (startPour) {
            pourTimer += Time.deltaTime;
            if (pourTimer >= 3.0f) {
                foreach (Vector3Int tile in tilesToCover) {
                    targetSelection.ModifyForbiddenTile(tile, true);
                }
            }
        }
    }

    void OnMouseDown() {
        beenClicked = true;
        Debug.Log("Start pouring");
    }
}
