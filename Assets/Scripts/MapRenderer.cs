using UnityEngine;
using System.Collections.Generic;

public class MapRenderer : MonoBehaviour
{
    // Vertex count along x and y
    public int width;
    public int height;

    private int scale = 30;

    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;
    private Color[] colors;

    // Define colors here
    private Color waterColor = new Color(0.0f, 0.0f, 1.0f);
    private Color grassColor = new Color(0.0f, 1.0f, 0.0f);
    private Color hillColor = new Color(0.5f, 0.35f, 0.05f);
    private Color snowColor = new Color(1.0f, 1.0f, 1.0f);

    private Color pinkColor = new Color(1.0f, 0.0f, 1.0f); // RGB for Pink

    private Color tundraColor = new Color(0.8f, 0.8f, 0.75f);
    private Color dryGrassColor = new Color(0.8f, 0.7f, 0.3f);
    private Color sandColor = new Color(0.95f, 0.9f, 0.55f);
    private Color deepWaterColor = new Color(0.0f, 0.0f, 0.5f);
    private Color dustColor = new Color(0.9f, 0.8f, 0.7f);

    public void ClearMap()
    {
        // Do something if needed
    }

    public void InitMap(float[,] elevationMap, float[,] moistureMap, float[,] nearestMountainMap)
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        vertices = new Vector3[width * height];
        triangles = new int[(width - 1) * (height - 1) * 6];
        colors = new Color[width * height];

        // Determine the max distance in nearestMountainMap for normalization
        float maxDistance = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                maxDistance = Mathf.Max(maxDistance, nearestMountainMap[i, j]);
            }
        }

        int tris = 0;
        int verts = 0;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                vertices[verts] = new Vector3(x, elevationMap[x, y] * scale, y);
                float elevation = elevationMap[x, y];
                float moisture = moistureMap[x, y];
                // Normalize the distance to a value between 0 and 1
                float normalizedDistance = nearestMountainMap[x, y] / maxDistance;

                if (normalizedDistance < 0.33f)
                {
                    colors[verts] = Color.Lerp(grassColor, dryGrassColor, normalizedDistance / 0.33f);
                }
                else if (normalizedDistance < 0.66f)
                {
                    colors[verts] = Color.Lerp(dryGrassColor, dustColor, (normalizedDistance - 0.33f) / 0.33f);
                }
                else
                {
                    colors[verts] = Color.Lerp(dustColor, dustColor, (normalizedDistance - 0.66f) / 0.34f);
                }

                if (elevation > 0.5f)
                {
                    colors[verts] = snowColor;
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
        // Debug.Log(vertices[0]);
        // Debug.Log(triangles[0]);
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;

        mesh.RecalculateNormals();
    }

    public void SetTileTo(int x, int y)
    {
        int index = x * width + y;
        colors[index] = waterColor;
        mesh.colors = colors;

        mesh.RecalculateNormals();
    }

    public Vector3 GetCellPosition(Vector3 pos)
    {
        return new Vector3(Mathf.FloorToInt(pos.x), pos.y, Mathf.FloorToInt(pos.z));
    }

    public void SetAreaToColor(Color searchColor, Color newColor, int areaSize)
    {
        List<Vector2Int> suitableAreas = new List<Vector2Int>();

        for (int x = 0; x < width - (areaSize - 1); x++)
        {
            for (int y = 0; y < height - (areaSize - 1); y++)
            {
                bool isSuitableArea = true;

                for (int dx = 0; dx < areaSize; dx++)
                {
                    for (int dy = 0; dy < areaSize; dy++)
                    {
                        int index = (x + dx) * height + (y + dy);
                        if (colors[index] != searchColor)
                        {
                            isSuitableArea = false;
                            break;
                        }
                    }
                    if (!isSuitableArea)
                        break;
                }

                if (isSuitableArea)
                {
                    suitableAreas.Add(new Vector2Int(x, y));
                }
            }
        }

        if (suitableAreas.Count > 0)
        {
            Vector2Int selectedArea = suitableAreas[Random.Range(0, suitableAreas.Count)];

            for (int dx = 0; dx < areaSize; dx++)
            {
                for (int dy = 0; dy < areaSize; dy++)
                {
                    int index = (selectedArea.x + dx) * height + (selectedArea.y + dy);
                    colors[index] = newColor;
                }
            }
        }
    }
}
