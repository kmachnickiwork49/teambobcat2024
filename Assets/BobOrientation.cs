using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobOrientation : MonoBehaviour
{
    [SerializeField] Sprite upLeftSprite;
    [SerializeField] Sprite downLeftSprite;
    [SerializeField] TutorialLevelPathfindingWinterDemo pathfinding;
    SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        sr.sprite = pathfinding.GetMovingUp() ? upLeftSprite : downLeftSprite; 
        transform.localScale = new(
            0.1f * (pathfinding.GetMovingRight() ? -1 : 1),
            transform.localScale.y,
            1);
    }
}
