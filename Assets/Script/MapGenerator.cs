using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
public class MapGenerator : MonoBehaviour
{
    public int width;
    public int height;
    public string seed;
    public bool useRanSeed;

    [Range(0, 100)]
    public int ranFillPerc;

    int[,] map;
    int[,] borderdMap;

    private void Start()
    {
        GenerateMap();
        MeshGenerator meshGen = GetComponent<MeshGenerator>();
        meshGen.GenerateMesh(borderdMap, 1);
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GenerateMap();
        }
    }
    void GenerateMap()
    {
        map = new int[width, height];
        ranFillMap();

        for (int i = 0; i < 5; i++)
        {
            //SmoothMap();
        }

        //ProcessMap();

        int borderSize = 0;

        borderdMap = new int[width + borderSize * 2, height + borderSize * 2];

        for (int x = 0; x < borderdMap.GetLength(0); x++)
        {
            for (int y = 0; y < borderdMap.GetLength(1); y++)
            {
                if (x >= borderSize && x < width + borderSize && y >= borderSize && y < height + borderSize)
                {
                    borderdMap[x, y] = map[x - borderSize, y - borderSize];
                }
                else
                {
                    borderdMap[x, y] = 1;
                }
            }
        }

        MeshGenerator meshGen = GetComponent<MeshGenerator>();
        meshGen.GenerateMesh(borderdMap, 1);
    }

    /*void ProcessMap()
    {
        List<List<Coord>> wallRegions = GetRegions(1);

        int wallTresholdSize = 50;

        foreach (List<Coord> wallRegion in wallRegions)
        {
            if (wallRegion.Count < wallTresholdSize)
            {
                foreach (Coord tile in wallRegion)
                {
                    map[tile.tileX, tile.tileY] = 0;
                }
            }
        }

        List<List<Coord>> roomRegions = GetRegions(0);

        int roomTresholdSize = 50;

        List<Room> surviveRooms = new List<Room>();



        foreach (List<Coord> roomRegion in roomRegions)
        {
            if (roomRegion.Count < roomTresholdSize)
            {
                foreach (Coord tile in roomRegion)
                {
                    map[tile.tileX, tile.tileY] = 1;
                }
            }
            else
            {
                surviveRooms.Add(new Room(roomRegion, map));
            }
        }
        surviveRooms.Sort();
        surviveRooms[0].mainRoom = true;
        surviveRooms[0].isAccessible = true;

        connectRooms(surviveRooms);
    }

    void connectRooms(List<Room> allRooms, bool forceAccess = false)
    {

        List<Room> roomListA = new List<Room>();
        List<Room> roomListB = new List<Room>();


        int BestDistance = 0;
        Coord bestTileA = new Coord();
        Coord bestTileB = new Coord();

        Room bestRoomA = new Room();
        Room bestRoomB = new Room();

        bool possConnFound = false;

        if (forceAccess)
        {
            foreach (Room room in allRooms)
            {
                if (room.isAccessible)
                {
                    roomListB.Add(room);
                }
                else
                    roomListA.Add(room);
            }
        }
        else
        {
            roomListA = allRooms;
            roomListB = allRooms;
        }

        foreach (Room a in roomListA)
        {
            if (!forceAccess)
            {

                possConnFound = false;
                if (a.connected.Count > 0)
                {
                    continue;
                }
            }

            foreach (Room b in roomListB)
            {
                if (a == b || a.IsConnected(b))
                    continue;



                for (int tileIndA = 0; tileIndA < a.edgeTiles.Count; tileIndA++)
                {
                    for (int tileIndB = 0; tileIndB < b.edgeTiles.Count; tileIndB++)
                    {
                        Coord tileA = a.edgeTiles[tileIndA];
                        Coord tileB = b.edgeTiles[tileIndB];

                        int distancebetweenRooms = (int)(Mathf.Pow(tileA.tileX - tileB.tileX, 2) + Mathf.Pow(tileA.tileY - tileB.tileY, 2));

                        if (distancebetweenRooms < BestDistance || !possConnFound)
                        {
                            BestDistance = distancebetweenRooms;
                            possConnFound = true;
                            bestTileA = tileA;
                            bestTileB = tileB;
                            bestRoomA = a;
                            bestRoomB = b;
                        }
                    }

                }
            }
            if (possConnFound && !forceAccess)
            {
                CreatePass(bestRoomA, bestRoomB, bestTileA, bestTileB);
            }
        }

        if (possConnFound && forceAccess)
        {
            CreatePass(bestRoomA, bestRoomB, bestTileA, bestTileB);
            connectRooms(allRooms, true);
        }
        if (!forceAccess)
        {
            connectRooms(allRooms, true);
        }
    }

    void CreatePass(Room a, Room b, Coord tileA, Coord tileB)
    {
        Room.ConnectRoom(a, b);


        List<Coord> line = Getline(tileA, tileB);

        foreach (Coord c in line)
        {
            DrawCircle(c, 1);
        }


    }

    void DrawCircle(Coord c, int r)
    {
        for (int x = -r; x <= r; x++)
        {
            for (int y = -r; y <= r; y++)
            {
                if (x * x + y * y <= r * r)
                {
                    int realX = c.tileX + x;
                    int realY = c.tileY + y;
                    if (IsInMapRange(realX, realY))
                    {
                        map[realX, realY] = 0;
                    }
                }
            }
        }
    }*/
    List<Coord> Getline(Coord start, Coord end)
    {
        List<Coord> line = new List<Coord>();
        int x = start.tileX;
        int y = start.tileY;

        int dx = end.tileX - start.tileX;
        int dy = end.tileY - start.tileY;

        bool inverted = false;

        int step = Math.Sign(dx);
        int gradStep = Math.Sign(dy);
        int longest = Mathf.Abs(dx);
        int shortest = Mathf.Abs(dy);

        if (longest < shortest)
        {
            inverted = true;
            longest = Mathf.Abs(dy);
            shortest = Mathf.Abs(dx);

            step = Math.Sign(dy);
            gradStep = Math.Sign(dx);

        }

        int gradAccum = longest / 2;

        for (int i = 0; i < longest; i++)
        {
            line.Add(new Coord(x, y));
            if (inverted)
            {
                y += step;

            }
            else
            {
                x += step;
            }

            gradAccum += shortest;
            if (gradAccum >= longest)
            {
                if (inverted)
                {
                    x += gradStep;
                }
                else
                {
                    y += gradStep;
                }
                gradAccum -= longest;
            }
        }
        return line;
    }
    Vector3 CoordtoWorldPoint(Coord tile)
    {
        return new Vector3(-width / 2 + .5f + tile.tileX, 2, -height / 2 + .5f + tile.tileY);
    }

    List<List<Coord>> GetRegions(int tileType)
    {
        List<List<Coord>> regions = new List<List<Coord>>();
        int[,] mapFlags = new int[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (mapFlags[x, y] == 0 && map[x, y] == tileType)
                {
                    List<Coord> newRegion = GetRegionTiles(x, y);
                    regions.Add(newRegion);

                    foreach (Coord tile in newRegion)
                    {
                        mapFlags[tile.tileX, tile.tileY] = 1;
                    }
                }
            }
        }
        return regions;
    }

    List<Coord> GetRegionTiles(int startX, int startY)
    {
        List<Coord> tiles = new List<Coord>();
        int[,] mapFlags = new int[width, height];
        int tileType = map[startX, startY];

        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(new Coord(startX, startY));
        mapFlags[startX, startY] = 1;

        while (queue.Count > 0)
        {
            Coord tile = queue.Dequeue();
            tiles.Add(tile);

            for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++)
            {
                for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++)
                {
                    if (IsInMapRange(x, y) && (y == tile.tileY || x == tile.tileX))
                    {
                        if (mapFlags[x, y] == 0 && map[x, y] == tileType)
                        {
                            mapFlags[x, y] = 1;
                            queue.Enqueue(new Coord(x, y));
                        }
                    }
                }
            }
        }

        return tiles;
    }
    bool IsInMapRange(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    void ranFillMap()
    {
        int rMax = map.GetUpperBound(0);
        int cMax = map.GetUpperBound(1);
        float placementThreshold = .1f;

        for (int x = 0; x <= rMax; x++)
        {
            for (int y = 0; y <= cMax; y++)
            {
                //1
                if (x == 0 || y == 0 || x == rMax || y == cMax)
                {
                    map[x, y] = 0;
                }

                //2
                else if (x % 2 == 0 && y % 2 == 0)
                {
                    if (UnityEngine.Random.value > placementThreshold)
                    {
                        //3
                        map[x, y] = 1;

                        int a = UnityEngine.Random.value < .5 ? 0 : (UnityEngine.Random.value < .5 ? -1 : 1);
                        int b = a != 0 ? 0 : (UnityEngine.Random.value < .5 ? -1 : 1);
                        map[x + a, y + b] = 1;
                    }
                }
            }
        }
    }


    /* void SmoothMap()
     {
         for (int x = 0; x < width; x++)
         {
             for (int y = 0; y < height; y++)
             {
                 int neighWallTiles = GetSurrWallCount(x, y);
                 if (neighWallTiles > 4)
                 {
                     map[x, y] = 1;

                 }
                 else if (neighWallTiles < 4)
                 {
                     map[x, y] = 0;
                 }
             }
         }
     }*/
    int GetSurrWallCount(int gridX, int gridY)
    {
        int wallCount = 0;
        for (int neighX = gridX - 1; neighX <= gridX + 1; neighX++)
        {
            for (int neighY = gridY - 1; neighY <= gridY + 1; neighY++)
            {
                if (neighX >= 0 && neighX < width && neighY >= 0 && neighY < height)
                {
                    if (neighX != gridX || neighY != gridY)
                    {

                        wallCount += map[neighX, neighY];
                    }

                }
                else
                {
                    wallCount++;
                }
            }
        }
        return wallCount;
    }

    struct Coord
    {
        public int tileX;
        public int tileY;

        public Coord(int x, int y)
        {
            tileX = x;
            tileY = y;
        }
    }

    class Room : IComparable<Room>
    {
        public List<Coord> tiles;
        public List<Coord> edgeTiles;
        public List<Room> connected;
        public int roomSize;
        public bool isAccessible;
        public bool mainRoom;

        public Room()
        {

        }

        public Room(List<Coord> roomTiles, int[,] map)
        {
            tiles = roomTiles;
            roomSize = tiles.Count;
            connected = new List<Room>();

            edgeTiles = new List<Coord>();

            foreach (Coord tile in tiles)
            {
                for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++)
                {
                    for (int y = tile.tileY - 1; y <= tile.tileY; y++)
                    {
                        if (x == tile.tileX || y == tile.tileY)
                        {
                            if (map[x, y] == 1)
                            {
                                edgeTiles.Add(tile);
                            }
                        }


                    }
                }
            }

        }

        public void SetAccessMainRoom()
        {
            if (!isAccessible)
            {
                isAccessible = true;
                foreach (Room conRoom in connected)
                {
                    conRoom.SetAccessMainRoom();
                }
            }
        }

        public int CompareTo(Room otherRoom)
        {
            return otherRoom.roomSize.CompareTo(roomSize);
        }

        public static void ConnectRoom(Room A, Room B)
        {
            if (A.isAccessible)
            {
                B.SetAccessMainRoom();
            }
            else if (B.isAccessible)
            {
                A.SetAccessMainRoom();
            }
            A.connected.Add(B);
            B.connected.Add(A);
        }
        public bool IsConnected(Room other)
        {
            return connected.Contains(other);
        }
    }



}