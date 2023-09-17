using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NoiseHelper
{
    public static Vector2 SumNoise(float x, float y, NoiseSettings noiseSettings)
    {
        float amplitude = 1;
        float frequency = noiseSettings.startFrequency;
        float elevation = 0;
        float moisture = 0;
        float amplitudeSum = 0;

        // Generate elevation noise
        for (int i = 0; i < noiseSettings.octaves; i++)
        {
            elevation += amplitude * Mathf.PerlinNoise(x * frequency, y * frequency);
            amplitudeSum += amplitude;
            amplitude *= noiseSettings.persistance;
            frequency *= 2;
        }
        elevation /= amplitudeSum; // set range back to 0-1

        // Generate moisture noise
        amplitude = 1;
        frequency = noiseSettings.biomeStartFrequency;
        amplitudeSum = 0;
        for (int i = 0; i < noiseSettings.biomeOctaves; i++)
        {
            moisture += amplitude * Mathf.PerlinNoise(1000 + x * frequency, 1000 + y * frequency); // Offsetting to not overlap with elevation noise
            amplitudeSum += amplitude;
            amplitude *= noiseSettings.biomePersistance;
            frequency *= 2;
        }
        moisture /= amplitudeSum; // set range back to 0-1

        return new Vector2(elevation, moisture);
    }

    public static float RangeMap(
        float inputValue,
        float inMin,
        float inMax,
        float outMin,
        float outMax
    )
    {
        return outMin + (inputValue - inMin) * (outMax - outMin) / (inMax - inMin);
    }

    public static List<Vector2Int> FindLocalMaxima(float[,] noiseMap)
    {
        List<Vector2Int> maximas = new List<Vector2Int>();
        for (int x = 0; x < noiseMap.GetLength(0); x++)
        {
            for (int y = 0; y < noiseMap.GetLength(1); y++)
            {
                var noiseVal = noiseMap[x, y];
                if (CheckNeighbours(x, y, noiseMap, (neighbourNoise) => neighbourNoise > noiseVal))
                {
                    // float temp = noiseMap[x,y];
                    // Debug.Log($"local maxima: {temp}");
                    maximas.Add(new Vector2Int(x, y));
                }
            }
        }
        return maximas;
    }

    public static List<Vector2Int> FindLocalMinima(float[,] noiseMap)
    {
        List<Vector2Int> minima = new List<Vector2Int>();
        for (int x = 0; x < noiseMap.GetLength(0); x++)
        {
            for (int y = 0; y < noiseMap.GetLength(1); y++)
            {
                var noiseVal = noiseMap[x, y];
                if (CheckNeighbours(x, y, noiseMap, (neighbourNoise) => neighbourNoise < noiseVal))
                {
                    minima.Add(new Vector2Int(x, y));
                }
            }
        }
        return minima;
    }

    static List<Vector2Int> directions = new List<Vector2Int>
    {
        new Vector2Int(0, 1), //N
        new Vector2Int(1, 1), //NE
        new Vector2Int(1, 0), //E
        new Vector2Int(-1, 1), //SE
        new Vector2Int(-1, 0), //S
        new Vector2Int(-1, -1), //SW
        new Vector2Int(0, -1), //W
        new Vector2Int(1, -1) //NW
    };

    private static bool CheckNeighbours(
        int x,
        int y,
        float[,] noiseMap,
        Func<float, bool> failCondition
    )
    {
        foreach (var dir in directions)
        {
            var newPost = new Vector2Int(x + dir.x, y + dir.y);

            if (
                newPost.x < 0
                || newPost.x >= noiseMap.GetLength(0)
                || newPost.y < 0
                || newPost.y >= noiseMap.GetLength(1)
            )
            {
                continue;
            }

            if (failCondition(noiseMap[x + dir.x, y + dir.y]))
            {
                return false;
            }
        }
        return true;
    }
}
