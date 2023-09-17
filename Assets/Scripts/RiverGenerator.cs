using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RiverGenerator : MonoBehaviour
{
    public MapGenerator mapGenerator;
    public NoiseSettings riverSettings;
    public Vector2 riverStartPosition;
    public int riverLength = 50;
    public bool bold = true;
    public bool converganceOn = true;

    public void GenerateRivers()
    {
        var result = NoiseHelper.FindLocalMaxima(mapGenerator.noiseMap);
        var toCreate = result.Where(pos => mapGenerator.noiseMap[pos.x, pos.y] > mapGenerator.snowHeight).OrderBy(a => Guid.NewGuid()).Take(5).ToList();
        var waterMinimas = NoiseHelper.FindLocalMinima(mapGenerator.noiseMap);
        waterMinimas = waterMinimas.Where(pos => mapGenerator.noiseMap[pos.x, pos.y] < mapGenerator.waterHeight).OrderBy(pos => mapGenerator.noiseMap[pos.x, pos.y]).Take(20).ToList();
        foreach (var item in toCreate)
        {
            //SetTileTo(item.x, item.y, maxPosTile);
            CreateRiver(item, waterMinimas);
            //return;
        }
        Color grassColor = new Color(0.0f, 1.0f, 0.0f);
        Color snowColor = new Color(1.0f, 1.0f, 1.0f);
        Color dustColor = new Color(0.9f, 0.8f, 0.7f);
        Color pinkColor = new Color(1.0f, 0.0f, 1.0f); // RGB for Pink

        mapGenerator.mapRenderer.SetAreaToColor(grassColor, pinkColor, 10);
        mapGenerator.mapRenderer.SetAreaToColor(snowColor, pinkColor, 5);
        mapGenerator.mapRenderer.SetAreaToColor(dustColor, pinkColor, 10);

    }

    private void CreateRiver(Vector2Int startPosition, List<Vector2Int> waterMinimas)
    {
        PerlinWorm worm;
        if (converganceOn)
        {
            var closestWaterPos = waterMinimas.OrderBy(pos => Vector2.Distance(pos, startPosition)).First();
            worm = new PerlinWorm(riverSettings, startPosition, closestWaterPos);
        }
        else
        {
            worm = new PerlinWorm(riverSettings, startPosition);
        }

        var position = worm.MoveLength(riverLength);
        StartCoroutine(PlaceRiverTile(position));
    }

  IEnumerator PlaceRiverTile(List<Vector2> positons)
{
    foreach (var pos in positons)
    {
        Vector3 scaledPos = new Vector3(pos.x, pos.y, 0);
        var tilePos = mapGenerator.mapRenderer.GetCellPosition(scaledPos);

        if (true){
        if (tilePos.x < 0 || tilePos.x >= mapGenerator.width || tilePos.y < 0 || tilePos.y >= mapGenerator.height)
            break;
        if (true){

        mapGenerator.mapRenderer.SetTileTo((int)tilePos.x, (int)tilePos.y);

        if (bold && mapGenerator.noiseMap[(int)tilePos.x, (int)tilePos.y] < mapGenerator.hillHeight)
        {
            mapGenerator.mapRenderer.SetTileTo((int)(tilePos + Vector3Int.right).x,(int)(tilePos + Vector3Int.right).y);
            mapGenerator.mapRenderer.SetTileTo((int)(tilePos + Vector3Int.left).x,(int)(tilePos + Vector3Int.left).y);
            mapGenerator.mapRenderer.SetTileTo((int)(tilePos + Vector3Int.up).x,(int)(tilePos + Vector3Int.up).y);
            mapGenerator.mapRenderer.SetTileTo((int)(tilePos + Vector3Int.down).x,(int)(tilePos + Vector3Int.down).y);
        }
        }
        }

        yield return new WaitForSeconds(0.0f);
    }
    yield return null;
}

}
