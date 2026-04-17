using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Random = UnityEngine.Random;

public class TextureInMaterial : MonoBehaviour
{
    [SerializeField] Renderer myRend;
    [Tooltip("size of single tile, width/height")]
    [SerializeField] Vector2 tiling;
    [Tooltip("width/height")]
    [SerializeField] Vector2Int dim;

    [SerializeField] bool randomizeTile;
    [SerializeField, HideIf(nameof(randomizeTile))] Vector2Int targetTile;
    [SerializeField] List<Vector2Int> tilesToIgnore = new List<Vector2Int>();

    void Start()
    {
        SpawnMe();
    }

    [Button]
    void TestSpawn()
    {
        SpawnMe();
    }

    void SpawnMe()
    {
        Material mat = myRend.material;
        mat.mainTextureScale = new Vector2(tiling.x, tiling.y);
        Vector2Int visibleTile = new Vector2Int(Random.Range(0, dim.x), Random.Range(0, dim.y));
        if (randomizeTile)
        {
            while (tilesToIgnore.Contains(visibleTile))
            {
                visibleTile = new Vector2Int(Random.Range(0, dim.x), Random.Range(0, dim.y));
            }
        }
        else
        {
            visibleTile = targetTile;
        }
        targetTile = visibleTile;
        mat.mainTextureOffset = new Vector2(visibleTile.x * tiling.x, visibleTile.y * tiling.y);

    }
}
