using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BobConstructionLevel1 : MonoBehaviour
{
    [SerializeField] private BobOrientationGeneric bobOrientationGeneric;
    [SerializeField] private TargetSelection targetSelection;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Pathfinder pathfinder;
    void Start() {
        targetSelection.setNewTilemap(tilemap);
        targetSelection.SelectTile(new Vector3Int(-9,2,0));
        Debug.Log(targetSelection.GetTarget());
    }
    
    void Update() {

    }
}
