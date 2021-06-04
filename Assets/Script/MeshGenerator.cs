using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MeshGenerator : MonoBehaviour
{

    public SquareGrid squareGrid;
    public MeshFilter walls;
    public MeshFilter cave;
    public bool is2d = false;
    List<Vector3> vertices;
    List<int> triangles;
    Dictionary<int, List<Triangle>> triDictionary = new Dictionary<int, List<Triangle>>();
    List<List<int>> outlines = new List<List<int>>();
    HashSet<int> checkedVert = new HashSet<int>();




    public void GenerateMesh(int[,] map, float squareSize)
    {
        triDictionary.Clear();
        outlines.Clear();
        checkedVert.Clear();

        squareGrid = new SquareGrid(map, squareSize);

        vertices = new List<Vector3>();
        triangles = new List<int>();

        for (int x = 0; x < squareGrid.squares.GetLength(0); x++)
        {
            for (int y = 0; y < squareGrid.squares.GetLength(1); y++)
            {
                TriangulateSquare(squareGrid.squares[x, y]);
            }
        }

        Mesh mesh = new Mesh();
        cave.mesh = mesh;

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        int tileAmount = 10;
        Vector2[] uvs = new Vector2[vertices.Count];
        for(int i = 0; i < vertices.Count; i++)
        {
            float percentX = Mathf.InverseLerp(-map.GetLength(0) / 2 * squareSize, map.GetLength(0) / 2 * squareSize, vertices[i].x) *tileAmount;
            float percentY = Mathf.InverseLerp(-map.GetLength(0) / 2 * squareSize, map.GetLength(0) / 2 * squareSize, vertices[i].y)*tileAmount;
            uvs[i] = new Vector2(percentX, percentY);
        }
        mesh.uv = uvs;

        if (is2d)
        {
            Generate2dColliders();
        }
        else
        {
            CreateWallMesh();
        }
        
    }

    void CreateWallMesh()
    {
        CalcMeshOutLine();
        List<Vector3> wallVerts = new List<Vector3>();
        List<int> wallTri = new List<int>();
        Mesh WallMesh = new Mesh();
        float wallHeight = 5;

        foreach (List<int> outline in outlines)
        {
            for (int i = 0; i < outline.Count - 1; i++)
            {
                int startInd = wallVerts.Count;
                wallVerts.Add(vertices[outline[i]]); //left
                wallVerts.Add(vertices[outline[i+1]]); //right
                wallVerts.Add(vertices[outline[i]] - Vector3.up *wallHeight);//bottom left
                wallVerts.Add(vertices[outline[i + 1]]-Vector3.up*wallHeight);//bottom right

                wallTri.Add(startInd + 0);
                wallTri.Add(startInd + 2);
                wallTri.Add(startInd + 3);

                wallTri.Add(startInd + 3);
                wallTri.Add(startInd + 1);
                wallTri.Add(startInd + 0);




            }
        }
        WallMesh.vertices = wallVerts.ToArray();
        WallMesh.triangles = wallTri.ToArray();
        walls.mesh = WallMesh;

        MeshCollider wallCollider = walls.gameObject.AddComponent<MeshCollider>();
        wallCollider.sharedMesh = WallMesh;
        
    }
    void Generate2dColliders()
    {
        EdgeCollider2D[] currentColliders = walls.gameObject.GetComponents<EdgeCollider2D>();
        for(int i = 0; i<currentColliders.Length;i++)
        {
            Destroy(currentColliders[i]);
        }
        CalcMeshOutLine();

        foreach(List<int> outline in outlines)
        {
            EdgeCollider2D edgeColl = gameObject.AddComponent<EdgeCollider2D>();
            Vector2[] edgePoints = new Vector2[outline.Count];
            for(int i = 0; i<outline.Count;i++)
            {
                edgePoints[i] = new Vector2(vertices[outline[i]].x, vertices[outline[i]].z);
            }
            edgeColl.points = edgePoints;
        }

    }

    void TriangulateSquare(Square square)
    {
        switch (square.config)
        {
            case 0:
                break;

            // 1point
            case 1:
                MeshFromPoints(square.centerLeft, square.centerBottom, square.bottomLeft);
                break;
            case 2:
                MeshFromPoints(square.bottomRight, square.centerBottom, square.centerRight);
                break;
            case 4:
                MeshFromPoints(square.topRight, square.centerRight, square.centerTop);
                break;
            case 8:
                MeshFromPoints(square.topLeft, square.centerTop, square.centerLeft);
                break;

            //2 points
            case 3:
                MeshFromPoints(square.centerRight, square.bottomRight, square.bottomLeft, square.centerLeft);
                break;
            case 6:
                MeshFromPoints(square.centerTop, square.topRight, square.bottomRight, square.centerBottom);
                break;
            case 9:
                MeshFromPoints(square.topLeft, square.centerTop, square.centerBottom, square.bottomLeft);
                break;
            case 12:
                MeshFromPoints(square.topLeft, square.topRight, square.centerRight, square.centerLeft);
                break;
            case 5:
                MeshFromPoints(square.centerTop, square.topRight, square.centerRight, square.centerBottom, square.bottomLeft, square.centerLeft);
                break;
            case 10:
                MeshFromPoints(square.topLeft, square.centerTop, square.centerRight, square.bottomRight, square.centerBottom, square.centerLeft);
                break;

            //3 point

            case 7:
                MeshFromPoints(square.centerTop, square.topRight, square.bottomRight, square.bottomLeft, square.centerLeft);
                break;
            case 11:
                MeshFromPoints(square.topLeft, square.centerTop, square.centerRight, square.bottomRight, square.bottomLeft);
                break;
            case 13:
                MeshFromPoints(square.topLeft, square.topRight, square.centerRight, square.centerBottom, square.bottomLeft);
                break;
            case 14:
                MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.centerBottom, square.centerLeft);
                break;

            //4 point

            case 15:

                MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.bottomLeft);
                checkedVert.Add(square.topLeft.vertexInd);
                checkedVert.Add(square.topRight.vertexInd);
                checkedVert.Add(square.bottomRight.vertexInd);
                checkedVert.Add(square.bottomLeft.vertexInd);
                break;




        }

    }

    void MeshFromPoints(params Node[] points)
    {
        AssignVertices(points);
        if (points.Length >= 3)
        {
            CreateTriangle(points[0], points[1], points[2]);
        }
        if (points.Length >= 4)
        {
            CreateTriangle(points[0], points[2], points[3]);
        }
        if (points.Length >= 5)
        {
            CreateTriangle(points[0], points[3], points[4]);
        }
        if (points.Length >= 6)
        {
            CreateTriangle(points[0], points[4], points[5]);
        }
    }
    void AssignVertices(Node[] points)
    {
        for (int i = 0; i < points.Length; i++)
        {
            if (points[i].vertexInd == -1)
            {
                points[i].vertexInd = vertices.Count;
                vertices.Add(points[i].pos);
            }
        }
    }

    void CreateTriangle(Node a, Node b, Node c)
    {
        triangles.Add(a.vertexInd);
        triangles.Add(b.vertexInd);
        triangles.Add(c.vertexInd);

        Triangle tri = new Triangle(a.vertexInd, b.vertexInd, c.vertexInd);

        AddTriToDic(tri.vertexIndA, tri);
        AddTriToDic(tri.vertexIndB, tri);
        AddTriToDic(tri.vertexIndC, tri);

    }

    void AddTriToDic(int vertexIndKey, Triangle triangle)
    {
        if (triDictionary.ContainsKey(vertexIndKey))
        {
            triDictionary[vertexIndKey].Add(triangle);
        }
        else
        {
            List<Triangle> triangleList = new List<Triangle>();
            triangleList.Add(triangle);
            triDictionary.Add(vertexIndKey, triangleList);
        }
    }

    void CalcMeshOutLine()
    {

        for (int vertexIndex = 0; vertexIndex < vertices.Count; vertexIndex++)
        {
            if (!checkedVert.Contains(vertexIndex))
            {
                int newOutvert = GetConOutVer(vertexIndex);
                if(newOutvert !=-1)
                {
                    checkedVert.Add(vertexIndex);

                    List<int> newOutline = new List<int>();

                    newOutline.Add(vertexIndex);
                    outlines.Add(newOutline);
                    FollowOutLine(newOutvert, outlines.Count - 1);
                    outlines[outlines.Count - 1].Add(vertexIndex);
                }
            }
            
        }
        
                
    }

    void FollowOutLine(int vertInd, int outlineInd)
    {
        outlines[outlineInd].Add(vertInd);
        checkedVert.Add(vertInd);
        int nextVertInd = GetConOutVer(vertInd);

        if(nextVertInd!=-1)
        {
            FollowOutLine(nextVertInd, outlineInd);
        }
    }

      

    int GetConOutVer(int vertexInd)
    {
        List<Triangle> triContVert = triDictionary[vertexInd];

        for(int i = 0; i < triContVert.Count; i++)
        {
            Triangle tri = triContVert[i];

            for(int j = 0; j<3; j++)
            {
                int vertexB = tri[j];

                if (vertexB != vertexInd && !checkedVert.Contains(vertexB))
                {

                    if (IsOutLineEdg(vertexInd, vertexB))
                    {
                        return vertexB;
                    }
                }
            }
        }
        return -1;
    }

    bool IsOutLineEdg(int vertexA, int vertexB)
    {
        List<Triangle> triContVerA = triDictionary[vertexA];
        int sharedTriCount = 0;

        for(int i = 0; i < triContVerA.Count; i++)
        {
            if(triContVerA[i].Contains(vertexB))
            {
                sharedTriCount++;
            }
            if(sharedTriCount > 1)
            {
                break;
            }
        }
        return sharedTriCount == 1;
    }

    struct Triangle
    {
        public int vertexIndA;
        public int vertexIndB;
        public int vertexIndC;

        int[] vertices;

        public Triangle(int a, int b, int c)
        {
            vertexIndA = a;
            vertexIndB = b;
            vertexIndC = c;

            vertices = new int[3];
            vertices[0] = a;
            vertices[1] = b;
            vertices[2] = c;
        }

        public int this[int i]
        {
            get { return vertices[i]; }
            }

        public bool Contains(int vertInd)
        {
            return vertInd == vertexIndA || vertInd == vertexIndB || vertInd == vertexIndC;
        }
    }

    public class SquareGrid
    {
        public Square[,] squares;

        public SquareGrid(int[,]map, float squareSize)
        {
            int nodeCountX = map.GetLength(0);
            int nodeCountY = map.GetLength(1);
            float mapWidth = nodeCountX * squareSize;
            float mapHeight = nodeCountY * squareSize;

            ControlNode[,] controlNodes = new ControlNode[nodeCountX, nodeCountY];

            for(int x = 0; x < nodeCountX; x++)
            {
                for(int y=0; y <nodeCountY; y++)
                {
                    Vector3 pos = new Vector3(-mapWidth / 2 + x * squareSize + squareSize / 2, 0, -mapHeight + y * squareSize + squareSize / 2);
                    controlNodes[x, y] = new ControlNode(pos, map[x, y] == 1, squareSize);
                }
            }

            squares = new Square[nodeCountX - 1, nodeCountY - 1];
            for (int x = 0; x < nodeCountX-1; x++)
            {
                for (int y = 0; y < nodeCountY-1; y++)
                {
                    squares[x, y] = new Square(controlNodes[x, y + 1], controlNodes[x + 1, y + 1], controlNodes[x + 1, y], controlNodes[x, y]);
                }
            }

        }


    }

    public class Square
    {
        public ControlNode topLeft, topRight, bottomRight, bottomLeft;
        public Node centerTop, centerRight, centerBottom, centerLeft;
        public int config;

        public Square(ControlNode topLeft, ControlNode topRight, ControlNode bottomRight, ControlNode bottomLeft)
        {
            this.topLeft = topLeft;
            this.topRight = topRight;
            this.bottomLeft = bottomLeft;
            this.bottomRight = bottomRight;

            centerTop = topLeft.right;
            centerRight = bottomRight.above;
            centerBottom = bottomLeft.right;
            centerLeft = bottomLeft.above;

            if (topLeft.active)
            {
                config += 8;

            }
            if(topRight.active)
            {
                config += 4;
            }
            if(bottomRight.active)
            {
                config += 2;
            }
            if(bottomLeft.active)
            {
                config += 1;
            }
        }
    }

    public class Node
    {
        public Vector3 pos;
        public int vertexInd = -1;

        public Node(Vector3 pos)
        {
            this.pos = pos;
        }

       
    }

    public class ControlNode : Node
    {
        public bool active;
        public Node above, right;

        public ControlNode(Vector3 pos, bool active, float squSize) :base(pos)
        {
            this.active = active;
            above = new Node(pos + Vector3.forward * squSize / 2f);
            right = new Node(pos + Vector3.right * squSize / 2f);
        }
    }
}
