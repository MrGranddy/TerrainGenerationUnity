using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    public MapRenderer mapRenderer;

    public RiverGenerator riverGenerator;
    // public TileBase grassTile, waterTile, hillTile, snowTile, maxPosTile;
    public int mapSize;
    public NoiseSettings mapSettings;
    public float hillHeight = 0.5f,
        snowHeight = 0.6f,
        waterHeight = 0.4f;
    //public GameObject townPrefab; // Attach your prefab here through Unity Editor

    public float[,] noiseMap;

    private void Start()
    {
        noiseMap = new float[mapSize, mapSize];
        // PrepareMap();
        // ShowMinimas();
        // ShowMaximas();
        // riverGenerator.GenerateRivers();
        // riverGenerator.GenerateRivers();

    }

    private void Update()
    {

    }

    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            PrepareMap();
        }
    }

    public void PrepareMap()
    {
        Debug.Log("Generating map");

        mapRenderer.ClearMap();

        float[,] elevationMap = new float[mapSize, mapSize];
        float[,] moistureMap = new float[mapSize, mapSize];

        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                Vector2 noise = NoiseHelper.SumNoise(x, y, mapSettings);

                elevationMap[x, y] = noise.x;
                // elevationMap[x, y] -= islandGradient[x,y];
                moistureMap[x, y] = noise.y;
            }
            noiseMap = elevationMap;
        }

        float[,] nearestMountainMap = NoiseHelper.NearestMountainMap(elevationMap);

        mapRenderer.InitMap(elevationMap, moistureMap, nearestMountainMap);
        ShowMaximas();
        ShowMinimas();
    }

    public void ShowMaximas()
    {
        var result = NoiseHelper.FindLocalMaxima(noiseMap);
        result = result
            .Where(pos => noiseMap[pos.x, pos.y] > snowHeight)
            .OrderBy(pos => noiseMap[pos.x, pos.y])
            .Take(20)
            .ToList();
        foreach (var item in result)
        {
            float maximaHeight = noiseMap[item.x, item.y];
            Debug.Log($"noiseMapVal: {maximaHeight}");
            mapRenderer.SetTileTo(item.x, item.y);
        }
    }

    public void ShowMinimas()
    {
        var result = NoiseHelper.FindLocalMinima(noiseMap);
        result = result
            .Where(pos => noiseMap[pos.x, pos.y] < waterHeight)
            .OrderBy(pos => noiseMap[pos.x, pos.y])
            .Take(20)
            .ToList();
        foreach (var item in result)
        {
            mapRenderer.SetTileTo(item.x, item.y);
        }
    }
}


