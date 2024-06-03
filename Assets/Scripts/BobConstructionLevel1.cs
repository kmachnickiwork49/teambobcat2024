using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class BobConstructionLevel1 : MonoBehaviour
{
    [SerializeField] private BobOrientationGeneric bobOrientationGeneric;
    [SerializeField] private TargetSelection targetSelection;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Pathfinder pathfinder;
    [SerializeField] private CementMixer[] myCMs;
    [SerializeField] private Vector3Int[] toDel;
    [SerializeField] private bool debugMode1 = false;
    [SerializeField] private Tile debugTile;
    [SerializeField] private GameObject level1plank;
    private bool enteredHardhatAnim = false;
    private bool doneHardhatAnim = false;
    private float hardHatTimer = 0.0f;
    private bool doneWinCon = false;
        void Start() {
        //targetSelection.setNewTilemap(tilemap);
        // PROBLEM WITH setNewTilemap -> Clears forbiddenTiles, and order of Start() calls matters
        // Easy solution -> remove setNewTilemap in start, rely on SerializeField
        targetSelection.SelectTile(new Vector3Int(-8,3,0));
        //Debug.Log(targetSelection.GetTarget());
        enteredHardhatAnim = false;
        hardHatTimer = 0.0f;
    }
    
    // LEVEL 1
    // X and Y BOUNDS: 14 length, 5 width
    // Y_MAX = 3
    // Y_MIN = -10
    // X_MAX = -8
    // X_MIN = -12

    void Update() {
        //targetSelection.SelectTile(new Vector3Int(-4,2,0));
        //targetSelection.SelectTile(new Vector3Int(-4,3,0));
        if (!enteredHardhatAnim && pathfinder.IsCloseTo(transform.position, tilemap.GetCellCenterWorld(new Vector3Int(-10,3,0)), 0.002f) == true) {
            //pathfinder.SetActive(false);
            //targetSelection.SetActive(false);
            //bobOrientationGeneric.SetActive(false);
            //Debug.Log("trigger animation");
            hardHatTimer = 0.0f;
            enteredHardhatAnim = true;
        }
        if (enteredHardhatAnim) {
            hardHatTimer += Time.deltaTime;
        }
        if (enteredHardhatAnim == false || hardHatTimer < 3.0f) {
            if (enteredHardhatAnim) {
                //Debug.Log(hardHatTimer);
                transform.position = tilemap.GetCellCenterWorld(new Vector3Int(-10,3,1));
                targetSelection.SetRange(2);
            }
            return;
        }
        // Done with hard hat animation
        //Debug.Log("done with hard hat anim");
        if (doneHardhatAnim == false) {
            // Do this once
            foreach (CementMixer cm in myCMs) {
                cm.setCanPour(true);
                cm.activateGlow();
            }
            foreach (Vector3Int tile in toDel) {
                targetSelection.ModifyForbiddenTile(tile, true);
                if (debugMode1) {
                    tilemap.SetTile(tile, debugTile);
                }
            }
            level1plank.SetActive(false);
        }
        doneHardhatAnim = true;

        //targetSelection.SelectTile(new Vector3Int(-8,3,0));
        //Debug.Log(transform.position);
        //if (tilemap.GetCellCenterWorld(transform.position).Y < -8.0f) {
        if (
            pathfinder.IsCloseTo(transform.position, tilemap.GetCellCenterWorld(new Vector3Int(-8,-8,1))) ||
            pathfinder.IsCloseTo(transform.position, tilemap.GetCellCenterWorld(new Vector3Int(-9,-8,1))) ||
            pathfinder.IsCloseTo(transform.position, tilemap.GetCellCenterWorld(new Vector3Int(-10,-8,1))) ||
            pathfinder.IsCloseTo(transform.position, tilemap.GetCellCenterWorld(new Vector3Int(-11,-8,1))) ||
            pathfinder.IsCloseTo(transform.position, tilemap.GetCellCenterWorld(new Vector3Int(-12,-8,1)))
        ) {
            Debug.Log("win! go next puzzle");
            if (!doneWinCon) {
                StartCoroutine(GoConstructionLevelTwo());
                doneWinCon = true;
            }
        }
    }

    IEnumerator GoConstructionLevelTwo() { 
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene("ConstructionLevelTwo");
    }
}
