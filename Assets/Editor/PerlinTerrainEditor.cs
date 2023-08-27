using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PerlinTerrain))]
public class PerlinTerrainEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector
        DrawDefaultInspector();

        PerlinTerrain myScript = (PerlinTerrain)target;
        if (GUILayout.Button("Generate Terrain"))
        {
            myScript.GenerateTerrain();
        }
    }
}