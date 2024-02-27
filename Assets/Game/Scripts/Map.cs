using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Map : MonoBehaviour
{
    public static Map main;

    [SerializeField] private Tilemap FloorMap;
    [SerializeField] private Tilemap WallMap;
    [SerializeField] private Tilemap ShadowMap;
    [SerializeField] private TileCustomData dark;
    private List<Vector3Int> lightSourcesPos;

    // Start is called before the first frame update
    void Start()
    {
        if (main == null)
        {
            main = this;
        }
        else
            throw new System.Exception("There can be only one map");

        lightSourcesPos = new List<Vector3Int>();

        int startX = WallMap.cellBounds.xMin;
        int startY = WallMap.cellBounds.yMin;
        int mapWidth = WallMap.cellBounds.xMax;
        int mapHeight = WallMap.cellBounds.yMax;

        for(int x = startX; x < mapWidth; x++)
        {
            for (int y = startY; y < mapHeight; y++)
            {
                TileCustomData wallTile = WallMap.GetTile<TileCustomData>(new Vector3Int(x, y, 0));
                TileCustomData floorTile = FloorMap.GetTile<TileCustomData>(new Vector3Int(x, y, 0));
                if (wallTile != null)
                {
                    if(wallTile.sourceLightLevel > 0)
                    {
                        TileCustomData light = wallTile.copy();
                        light.sprite = null;
                        ShadowMap.SetTile(new Vector3Int(x, y, 0), light);
                        lightSourcesPos.Add(new Vector3Int(x, y, 0));
                    }
                    else
                    {
                        ShadowMap.SetTile(new Vector3Int(x, y, 0), dark);
                    }
                }
                else if(floorTile != null)
                {
                    if (floorTile.sourceLightLevel> 0)
                    {
                        TileCustomData light = floorTile.copy();
                        light.sprite = null;
                        ShadowMap.SetTile(new Vector3Int(x, y, 0), light);
                        lightSourcesPos.Add(new Vector3Int(x, y, 0));
                    }
                    else
                    {
                        ShadowMap.SetTile(new Vector3Int(x, y, 0), dark);
                    }
                }
                else
                {
                    ShadowMap.SetTile(new Vector3Int(x, y, 0), dark);
                }
            }
        }
        UpdateLightMap();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UpdateNeighbours(Vector3Int tilePos, int lightStrength)
    {
        lightStrength--;
        if(lightStrength == 0)
        {
            return;
        }
        TileCustomData currentTile = ShadowMap.GetTile<TileCustomData>(tilePos);
        if (currentTile.lightLevel < lightStrength)
        {
            currentTile.lightLevel = lightStrength;
            currentTile.sprite = null;
            ShadowMap.SetTile(tilePos, currentTile);
            Debug.Log(tilePos);
        }
        else
        {
            //If current tile has higher or the same light level, no need for updating it
            return;
        }

        UpdateNeighbours(tilePos + new Vector3Int(1, 0, 0), lightStrength);
        UpdateNeighbours(tilePos + new Vector3Int(-1, 0, 0), lightStrength);
        UpdateNeighbours(tilePos + new Vector3Int(0, 1, 0), lightStrength);
        UpdateNeighbours(tilePos + new Vector3Int(0, -1, 0), lightStrength);
    }
    
    public void UpdateLightMap()
    {
        foreach(Vector3Int pos in lightSourcesPos)
        {
            UpdateNeighbours(pos, ShadowMap.GetTile<TileCustomData>(pos).sourceLightLevel);
        }
    }
}
