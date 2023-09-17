using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    public int width = 500;
    public int height = 500;

    public MapRenderer mapRenderer;
    public RiverGenerator riverGenerator;
    public NoiseSettings mapSettings;

    public float hillHeight = 0.5f, snowHeight = 0.6f, waterHeight = 0.4f;
    //public GameObject townPrefab; // Attach your prefab here through Unity Editor

    public float[,] noiseMap;

    private void Start()
    {
        noiseMap = new float[width, height];
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
            mapRenderer.SetMapSize(width, height);
            PrepareMap();
        }
    }

    public void PrepareMap()
    {
        Debug.Log("Generating map");

        mapRenderer.ClearMap();

        float[,] elevationMap = new float[width, height];
        float[,] decreaseMap = new float[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                elevationMap[x, y] = NoiseHelper.PerlinMap(x, y, mapSettings.elevationOctaves, mapSettings.elevationStartFrequency, mapSettings.elevationPersistance, mapSettings.elevationOffset);
                decreaseMap[x, y] = NoiseHelper.PerlinMap(x, y, mapSettings.decreaseOctaves, mapSettings.decreaseStartFrequency, mapSettings.decreasePersistance, mapSettings.decreaseOffset);
            }
        }

        float[,] nearestMountainMap = NoiseHelper.NearestMountainMap(elevationMap);

        mapRenderer.InitMap(elevationMap, decreaseMap, nearestMountainMap, mapSettings);

        noiseMap = elevationMap;

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


