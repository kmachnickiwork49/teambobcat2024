using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using UnityEngine.Rendering.Universal;

[System.Serializable]
public class TilemapInfo
{
    public List<Vector3Int> forbiddenTiles;
    public Tilemap tilemap;
}

[System.Serializable]
public class ListWrapper
{
     public List<SpriteRenderer> values;
}

public class TilemapBrain : MonoBehaviour
{
    [SerializeField] List<TilemapInfo> tilemapInfos;
    [SerializeField] List<Tilemap> triggerZoneStarts;
    [SerializeField] List<Vector3> transitionPaths;
    [SerializeField] List<Transform> cameraTransforms;
    [SerializeField] List<float> cameraZooms;
    [SerializeField] float cameraMoveDuration = 2f;
    [SerializeField] float transitionMoveSpeed = 2f;
    [SerializeField] BobBrain bobBrain;
    [SerializeField] Light2D bobLight;
    [SerializeField] Light2D globalLight;
    [SerializeField] float lightFadeTime = 2f;
    [SerializeField] float minGlobalLightIntensity = 0.15f;
    [SerializeField] FlickerLight bobFlickerLight;
    [SerializeField] TargetSelection bobTargetSelection;
    [SerializeField] Pathfinder bobPathfinder;
    [SerializeField] Transform bobTransform;
    [SerializeField] int mapIdx = 0;
    [SerializeField] List<ListWrapper> srs = new();
    bool inTransition = false;

    private void OnDrawGizmos()
    {
        int idx = 0;
        foreach(Vector3 path in transitionPaths) {
            Gizmos.DrawRay(triggerZoneStarts[idx].transform.position, transitionPaths[idx]);
            idx++;
        }

        foreach (TilemapInfo tmi in tilemapInfos)
        {
            Tilemap tm = tmi.tilemap;
            foreach (Vector3Int tile in tmi.forbiddenTiles)
            {
                RangeUtils.VisualizePosition(tm.GetCellCenterWorld(tile));
            }
        }
    }

    void Start()
    {
        bobFlickerLight.enabled = false;
        bobLight.intensity = 0;
        globalLight.intensity = 1;
        Camera cam = Camera.main;
        StartCoroutine(CameraUtils.MoveAndZoomCamera(
            cam.transform.position, cameraTransforms[mapIdx].position,
            cam.orthographicSize, cameraZooms[mapIdx], 0.1f));
    }

    // Update is called once per frame
    void Update()
    {
        if (mapIdx >= tilemapInfos.Count - 1)
        {
            return;
        }
        Vector3Int bobTile = triggerZoneStarts[mapIdx].WorldToCell(bobTransform.position - new Vector3(0, 0, 1.0f));
        if (!inTransition && triggerZoneStarts[mapIdx].HasTile(bobTile))
        {
            if (mapIdx == 0) bobBrain.DisableJackhammers();
            if (mapIdx == 1) StartCoroutine(InitiateBobLight());
            inTransition = true;
            // entered next tilemap
            foreach (SpriteRenderer sr in srs[mapIdx].values)
            {
                StartCoroutine(ChangeTopLevelOpacity(1f, 0.05f, cameraMoveDuration, sr));
            }
            StartCoroutine(CameraUtils.MoveAndZoomCamera(
                cameraTransforms[mapIdx].position, cameraTransforms[mapIdx + 1].position,
                cameraZooms[mapIdx], cameraZooms[mapIdx + 1], 
                cameraMoveDuration));
            StartCoroutine(MoveToNextTilemap(transitionPaths[mapIdx], () =>
            {
                if (mapIdx == 1) ActivateBobLight();
                TilemapInfo tmi = tilemapInfos[mapIdx + 1];
                bobTargetSelection.setNewTilemap(tmi.tilemap, tmi.forbiddenTiles);
                mapIdx += 1;
                inTransition = false;
                bobPathfinder.Reset();
            }));
        }
    }

    IEnumerator InitiateBobLight()
    { 
        bobFlickerLight.enabled = true;
        for (float t = 0f; t < lightFadeTime; t += Time.deltaTime)
        {
            float normT = t / lightFadeTime;
            globalLight.intensity = 1 - (1 - minGlobalLightIntensity) * normT;
            yield return null;
        }
        globalLight.intensity = minGlobalLightIntensity;
    }

    void ActivateBobLight()
    {
        bobFlickerLight.enabled = false;
        bobLight.intensity = 1;
    }

    IEnumerator MoveToNextTilemap(Vector3 path, Action moveFinishedCB)
    {
        Vector3 startPos = bobTransform.position;
        Vector3 endPos = path + startPos;
        float travelTime = path.magnitude / transitionMoveSpeed;
        float elapsedTime = 0;

        while (elapsedTime < travelTime)
        {
            float progress = elapsedTime / travelTime;
            bobTransform.position = Vector3.Lerp(startPos, endPos, progress);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        bobTransform.position = endPos;
        moveFinishedCB?.Invoke();
    }

    IEnumerator ChangeTopLevelOpacity(float iAlpha, float fAlpha, float duration, SpriteRenderer sr)
    {
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            float normalizedTime = t / duration;
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, iAlpha + (fAlpha - iAlpha) * normalizedTime);
            yield return null;
        }
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, fAlpha);
    }
}
