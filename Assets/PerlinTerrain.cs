using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using System.Threading;


public class PerlinTerrain : MonoBehaviour
{

    public int width = 256;
    public int height = 256;
    public float scale = 20;

    public int octaves = 6;
    public float persistence = 0.8f;
    public float initialFrequency = 0.06f;
    public float lacunarity = 2.0f;
    public float heightOffset = 0.2f;

    Color brown = new Color(0.647f, 0.165f, 0.165f);


   
    // Start is called before the first frame update
    void Start()
    {
        GenerateTerrain();
        //Thread.Sleep(5000);
        //GenerateRivers();
    }

    // Update is called once per frame
    void Update()
    {
      

    }

    public float[,] GenerateIslandGradientMap(int mapWidth, int mapHeight)
    {
        float[,] gradientMap = new float[mapWidth, mapHeight];
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                float i = x / (float)mapWidth * 2 - 1;
                float j = y / (float)mapHeight * 2 - 1;

                //float value = Mathf.Max(Mathf.Abs(i), Mathf.Abs(j));
                float value = Mathf.Sqrt(Mathf.Pow(i, 2) + Mathf.Pow(j, 2));
                float a = 3;
                float b = 2.2f;

                float islandGradientValue = Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));

                gradientMap[x, y] = islandGradientValue;
            }
        }
        return gradientMap;
    }

    float PerlinNoise(float x, float y, int octaves, float persistence, float frequency, float lacunarity)
    {
        float total = 0;
        float amplitude = 1;
        float maxValue = 0;  // Used for normalizing result to 0.0 - 1.0

        for (int i = 0; i < octaves; i++)
        {
            total += Mathf.PerlinNoise(x * frequency, y * frequency) * amplitude;

            maxValue += amplitude;

            amplitude *= persistence;
            frequency *= lacunarity;
        }

        return total / maxValue;
    }

    public float[,] GeneratePerlinNoiseMap(int mapWidth, int mapHeight)
    {
        float[,] perlinMap = new float[mapWidth, mapHeight];

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                float noise = PerlinNoise(x, y, octaves, persistence, initialFrequency, lacunarity);
                noise = noise - heightOffset;

                perlinMap[x, y] = noise;
                    
            }
        }

        return perlinMap;

    }

    void GenerateTerrain()
    {
        // Create the mesh data
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        Vector3[] vertices = new Vector3[width * height];
        int[] triangles = new int[(width - 1) * (height - 1) * 6];
        Color[] colors = new Color[width * height];  // To store color data based on height

        int tris = 0;
        int verts = 0;

        float[,] gradient = GenerateIslandGradientMap(width, height);
        float[,] noise = GeneratePerlinNoiseMap(width, height);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Use Perlin noise to set the height of the terrain
                float heightValue = (noise[x, y] - gradient[x, y] * 0) * scale;
                Debug.Log($"{heightValue/scale}");
                vertices[verts] = new Vector3(x, heightValue, y);

                // Color based on height
                if (heightValue < 0)
                {
                    colors[verts] = Color.blue;  // Underwater
                }
                else if (heightValue < scale * 0.3f)
                {
                    colors[verts] = Color.green;  // Low lands
                }
                else if (heightValue < scale * 0.7f)
                {
                    colors[verts] = brown;  // Mountains
                }
                else
                {
                    colors[verts] = Color.white;  // High peaks
                }

                verts++;

                if (x < (width - 1) && y < (height - 1))
                {
                    triangles[tris] = x * height + y;
                    triangles[tris + 1] = x * height + y + 1;
                    triangles[tris + 2] = (x + 1) * height + y;

                    triangles[tris + 3] = (x + 1) * height + y;
                    triangles[tris + 4] = x * height + y + 1;
                    triangles[tris + 5] = (x + 1) * height + y + 1;

                    tris += 6;
                }
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        MarkLocalMinimaAndMaxima(vertices, colors,gradient,noise);
        mesh.colors = colors;  // Assign the color array to the mesh
        mesh.RecalculateNormals();
    }


    // A function to mark the local minima and maxima on the terrain
    void MarkLocalMinimaAndMaxima(Vector3[] vertices, Color[] colors, float[,] gradient, float[,] noise)
    {
        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                int currentIndex = x * height + y;
                float currentValue = vertices[currentIndex].y;

                bool isLocalMinima = true;
                bool isLocalMaxima = true;

                // Loop through the neighbors to check for local minima or maxima
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        if (dx == 0 && dy == 0) continue;

                        int neighborIndex = (x + dx) * height + (y + dy);
                        float neighborValue = vertices[neighborIndex].y;

                        if (currentValue >= neighborValue)
                        {
                            isLocalMinima = false;
                        }

                        if (currentValue <= neighborValue)
                        {
                            isLocalMaxima = false;
                        }
                    }
                }
            float heightValue = (noise[x, y] - gradient[x, y] * 0) * scale;

                 // Store the coordinates of local minima and maxima
            if (isLocalMinima && heightValue < 0)  // Check if it's water tile
            {
                colors[currentIndex] = Color.white;  // Mark local minima with black color
            }

            if (isLocalMaxima && heightValue >= scale * 0.7f)  // Check if it's mountain tile
            {
                colors[currentIndex] = Color.black;  // Mark local maxima with red color
            }
            }
        }
    }

}
