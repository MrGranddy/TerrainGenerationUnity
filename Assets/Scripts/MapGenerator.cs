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
    public float hillHeight = 0.5f, snowHeight = 0.6f, waterHeight = 0.4f;
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
        PrepareMap();
    }

    // public float[,] GenerateIslandGradientMap(int mapWidth, int mapHeight)
    // {
    //     float[,] gradientMap = new float[mapWidth, mapHeight];
    //     for (int x = 0; x < mapWidth; x++)
    //     {
    //         for (int y = 0; y < mapHeight; y++)
    //         {
    //             float i = x / (float)mapWidth * 2 - 1;
    //             float j = y / (float)mapHeight * 2 - 1;

    //             float value = Mathf.Max(Mathf.Abs(i), Mathf.Abs(j));
    //             float a = 3;
    //             float b = 2.2f;

    //             float islandGradientValue = Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));

    //             gradientMap[x, y] = islandGradientValue;
    //         }
    //     }
    //     return gradientMap;
    // }

    public static float[,] GenerateIslandGradientMap(int mapWidth, int mapHeight)
{
    float[,] map = new float[mapWidth,mapHeight];
    for (int x = 0; x < mapWidth; x++)
    {
        for (int y = 0; y < mapHeight; y++)
        {
            // Value between 0 and 1 where * 2 - 1 makes it between -1 and 0
            float i = x / (float)mapWidth * 2 - 1;
            float j = y / (float)mapHeight * 2 - 1;
 
            // Find closest x or y to the edge of the map
            float value = Mathf.Max(Mathf.Abs(i), Mathf.Abs(j));
 
            // Apply a curve graph to have more values around 0 on the edge, and more values >= 3 in the middle
            float a = 3;
            float b = 2.2f;
            float islandGradientValue = Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));
 
            // Apply gradient in the map
            map[x,y] = islandGradientValue;
        }
    }
    return map;
}



    public void PrepareMap()
    {
        Debug.Log("Generating map");
        mapRenderer.ClearMap();
        float[,] islandGradient = GenerateIslandGradientMap(mapSize,mapSize);
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
            mapRenderer.InitMap(elevationMap, moistureMap);
            ShowMaximas();
            ShowMinimas();

    }

    public void ShowMaximas()
    {
        var result = NoiseHelper.FindLocalMaxima(noiseMap);
        result = result.Where(pos => noiseMap[pos.x, pos.y] > snowHeight).OrderBy(pos => noiseMap[pos.x, pos.y]).Take(20).ToList();
        foreach (var item in result)
        {
            float maximaHeight = noiseMap[item.x,item.y];
            Debug.Log($"noiseMapVal: {maximaHeight}");
            mapRenderer.SetTileTo(item.x, item.y);
        }
    }

    public void ShowMinimas()
    {
        var result = NoiseHelper.FindLocalMinima(noiseMap);
        result = result.Where(pos => noiseMap[pos.x, pos.y] < waterHeight).OrderBy(pos => noiseMap[pos.x, pos.y]).Take(20).ToList();
        foreach (var item in result)
        {
            mapRenderer.SetTileTo(item.x, item.y);
        }
    }
}


