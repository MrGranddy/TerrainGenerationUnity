using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;


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

    }

    // Update is called once per frame
    void Update()
    {
        GenerateTerrain();
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
        mesh.colors = colors;  // Assign the color array to the mesh
        mesh.RecalculateNormals();
    }

}
