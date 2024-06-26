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
    [SerializeField] private float pourPeriod = 2.0f;
    private bool beenClicked = false;
    private bool startPour = false;
    private float pourTimer;
    private bool donePour1 = false;
    private bool canPour = false;
    [SerializeField] private GameObject my_glow;
    [SerializeField] private GameObject wet_cement_tile_prefab;
    // Start is called before the first frame update
    void Start()
    {
        beenClicked = false;
        startPour = false;
        donePour1 = false;
        pourTimer = 0.0f;
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
        if (beenClicked && !startPour && canPour) {
            startPour = true;
            pourTimer = 0.0f;
            //Debug.Log("update registers beenClicked");
        }
        if (startPour && canPour) {
            if (donePour1 == false) {
                /*
                pourTimer = pourTimer + Time.deltaTime;
                Debug.Log("pouring" + pourTimer);
                if (pourTimer >= 3.0f) {
                    foreach (Vector3Int tile in tilesToCover) {
                        targetSelection.ModifyForbiddenTile(tile, true);
                        if (debugMode) {
                            tilemap.SetTile(tile, debugTile);
                        }
                    }
                    Debug.Log("donePour1");
                    donePour1 = true;
                }
                */
                StartCoroutine(PourRoutine(tilesToCover));
                donePour1 = true;
            }
        }
    }

    void OnMouseDown() {
        beenClicked = true;
        //Debug.Log("Start pouring");
    }

    IEnumerator PourRoutine(Vector3Int[] tilesToCover)
    {
            foreach (Vector3Int tile in tilesToCover) {
                Debug.Log("poured");
                // Change to Bob wants to walk on wet cement
                targetSelection.ModifyForbiddenTile(tile, false);
                if (debugMode) {
                    tilemap.SetTile(tile, debugTile);
                }
                Instantiate(wet_cement_tile_prefab, tilemap.GetCellCenterWorld(tile), Quaternion.identity);
                yield return new WaitForSeconds(pourPeriod);
            }
    }

    public void setCanPour(bool a) {
        canPour = a;
    }

    public void activateGlow() {
        my_glow.SetActive(true);
    }
}
