using System;
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
    [SerializeField] private TileLight dark;
    [SerializeField] private LayerMask barrier;
    [SerializeField] private LineRenderer beam;
    private List<Vector3Int> lightSourcesPos;
    private List<Laser> laserSources;
    private List<Laser> updatedLasers;
    private GridLayout gridLayout;

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
        laserSources = new List<Laser>();
        updatedLasers = new List<Laser>();
        gridLayout = gameObject.GetComponent<GridLayout>();

        int startX = WallMap.cellBounds.xMin;
        int startY = WallMap.cellBounds.yMin;
        int mapWidth = WallMap.cellBounds.xMax;
        int mapHeight = WallMap.cellBounds.yMax;
        /*
        //For debugging purposes
        dark = Instantiate(dark);
        dark.sprite = null;
        //
        */
        for (int x = startX; x < mapWidth; x++)
        {
            for (int y = startY; y < mapHeight; y++)
            {
                TileLightExtension wallTile = WallMap.GetTile<TileLightExtension>(new Vector3Int(x, y, 0));
                TileLightExtension floorTile = FloorMap.GetTile<TileLightExtension>(new Vector3Int(x, y, 0));
                if (wallTile != null)
                {
                    if(wallTile.sourceLightLevel > 0)
                    {
                        TileLightExtension light = Instantiate(wallTile);
                        light.sprite = null;
                        ShadowMap.SetTile(new Vector3Int(x, y, 0), light);
                        lightSourcesPos.Add(new Vector3Int(x, y, 0));
                    }
                    else
                    {
                        ShadowMap.SetTile(new Vector3Int(x, y, 0), Instantiate(dark));
                    }
                }
                else if(floorTile != null)
                {
                    if (floorTile.sourceLightLevel> 0)
                    {
                        TileLightExtension light = Instantiate(floorTile);
                        light.sprite = null;
                        ShadowMap.SetTile(new Vector3Int(x, y, 0), light);
                        lightSourcesPos.Add(new Vector3Int(x, y, 0));
                    }
                    else
                    {
                        ShadowMap.SetTile(new Vector3Int(x, y, 0), Instantiate(dark));
                    }
                }
                else
                {
                    ShadowMap.SetTile(new Vector3Int(x, y, 0), Instantiate(dark));
                }
            }
        }
        UpdateLightMap();
        FindLaserSources();
        LoadLaserLight();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UpdateNeighbours(Vector3Int tilePos, int lightStrength)
    {
        TileLight currentTile = ShadowMap.GetTile<TileLight>(tilePos);
        if (currentTile == null)
        {
            return;
        }

        if (currentTile.lightLevel < lightStrength)
        {
            currentTile.lightLevel = lightStrength;
            currentTile.sprite = null;
            ShadowMap.SetTile(tilePos, currentTile);
        }
        else
        {
            //If current tile has higher or the same light level, no need for updating it or any of the neighbours
            return;
        }
        ShadowMap.RefreshTile(tilePos);

        lightStrength--;
        if(lightStrength == 0)
        {
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
            UpdateNeighbours(pos, ShadowMap.GetTile<TileLightExtension>(pos).sourceLightLevel);
        }
    }
    
    public void FindLaserSources()
    {
        foreach(Vector3Int pos in lightSourcesPos)
        {
            Direction.directions facing = ShadowMap.GetTile<TileLightExtension>(pos).facing;
            if (facing != Direction.directions.None)
            {
                Vector2 startPos = new Vector2(pos.x + 0.5f, pos.y + 0.5f);
                Vector2 direction = Direction.getDirectionVector(facing);
                Laser l = new Laser(barrier, startPos, direction, Instantiate(beam));
                l.castRayTo(startPos, direction);
                laserSources.Add(l);
            }
        }
    }

    private bool CCW(Vector2 a, Vector2 b, Vector2 c)
    {
        return (c.y - a.y) * (b.x - a.x) > (b.y - a.y) * (c.x - a.x);
    }

    private bool intersect(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
    {
        return CCW(a, c, d) != CCW(b, c, d) && CCW(a, b, c) != CCW(a, b, d);
    }

    public void SetUpdateLaserList(Vector3 player, Vector3 direction)
    {
        foreach (Laser laser in laserSources)
        {
            Vector2 a = player;
            Vector2 b = (Vector2)player + new Vector2(direction.x * ShadowMap.cellBounds.size.x, direction.y * ShadowMap.cellBounds.size.y);
            Vector2 c = laser.getPosition();
            Vector2 d = (Vector2)laser.getPosition() + new Vector2(laser.getRotation().x * ShadowMap.cellBounds.size.x, laser.getRotation().y * ShadowMap.cellBounds.size.y);
            
            if(intersect(a,b,c,d))
            {
                updatedLasers.Add(laser);
            }
        }
    }

    public void updateLasers()
    {
        foreach( Laser laser in updatedLasers)
        {
            laser.castRayTo(laser.getPosition(), laser.getRotation());
        }
    }

    public void clearUpdateList()
    {
        updatedLasers.Clear();

        ResetMap();
        UpdateLightMap();
        LoadLaserLight();
    }

    private void ResetMap()
    {
        int startX = ShadowMap.cellBounds.xMin;
        int startY = ShadowMap.cellBounds.yMin;
        int mapWidth = ShadowMap.cellBounds.xMax;
        int mapHeight = ShadowMap.cellBounds.yMax;
        for (int x = startX; x < mapWidth; x++)
        {
            for (int y = startY; y < mapHeight; y++)
            {
                TileLightExtension TLE = ShadowMap.GetTile<TileLightExtension>(new Vector3Int(x, y, 0));
                if(TLE == null)
                {
                    ShadowMap.SetTile(new Vector3Int(x, y, 0), Instantiate(dark));
                    continue;
                }

                if (TLE.sourceLightLevel == 0)
                {
                    ShadowMap.SetTile(new Vector3Int(x, y, 0), Instantiate(dark));
                }
                else
                {
                    TLE.lightLevel = 0;
                }
            }
        }
        ShadowMap.RefreshAllTiles();
    }

    private void LoadLaserLight()
    {
        foreach (Laser laser in laserSources)
        {
            LineRenderer LR = laser.getBeam();
            for(int i = 0; i < LR.positionCount - 1; i++)
            {
                float x1 = MathF.Round(LR.GetPosition(i).x * 10) / 10;
                float x2 = MathF.Round(LR.GetPosition(i + 1).x * 10) / 10;
                float y1 = MathF.Round(LR.GetPosition(i).y * 10) / 10;
                float y2 = MathF.Round(LR.GetPosition(i + 1).y * 10) / 10;
                float z1 = MathF.Round(LR.GetPosition(i).z * 10) / 10;
                float z2 = MathF.Round(LR.GetPosition(i + 1).z * 10) / 10;
                
                Vector3Int startPoint = gridLayout.WorldToCell(new Vector3(x1, y1, z1));
                Vector3Int endPoint = gridLayout.WorldToCell(new Vector3(x2, y2, z2));
                if(y1 - y2 == 0)
                {
                    if(x1 > x2)
                    {
                        Vector3Int temp = startPoint;
                        startPoint = endPoint;
                        endPoint = temp;
                    }
                    for(int x = startPoint.x + 1; x <= endPoint.x; x++)
                    {
                        UpdateNeighbours(new Vector3Int(x, startPoint.y, startPoint.z), 2);
                    }
                }
                else
                {
                    if (y1 > y2)
                    {
                        Vector3Int temp = startPoint;
                        startPoint = endPoint;
                        endPoint = temp;
                    }
                    for (int y = startPoint.y + 1; y <= endPoint.y; y++)
                    {
                        UpdateNeighbours(new Vector3Int(startPoint.x, y, startPoint.z), 2);
                    }
                }
            }
        }
    }
}
