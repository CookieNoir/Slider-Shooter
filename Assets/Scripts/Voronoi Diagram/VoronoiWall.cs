using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[Serializable]
public class VoronoiWall : VoronoiDiagram
{
    [Serializable]
    public class Vector2Radial
    {
        public Vector2 point;
        public float angle;

        public Vector2Radial(Vector2 newPoint, Vector2 radialCenter)
        {
            point = newPoint;
            angle = Mathf.Atan2(point.y - radialCenter.y, point.x - radialCenter.x);
        }
    }

    public VoronoiWall(Vector2[] points, Vector2 point1, Vector2 point2, float depth, Transform parent, GameObject prefab) : base(points, point1, point2)
    {
        BuildEdgesOfBounds();
        CreateMeshes(depth, prefab, parent);
    }

    private void BuildEdgesOfBounds()
    {
        if (cells.Count < 1) return;
        IComparer<BoundIntersection> comparer = new BoundIntersectionIncX();
        intersectionsUp.Sort(comparer);
        comparer = new BoundIntersectionDecY();
        intersectionsRight.Sort(comparer);
        comparer = new BoundIntersectionDecX();
        intersectionsDown.Sort(comparer);
        comparer = new BoundIntersectionIncY();
        intersectionsLeft.Sort(comparer);

        int cell = 0;
        float sqrMag = SqrMagnitude(cells[0].center - upperLeftCorner), sqr;

        for (int i = 1; i < cells.Count; ++i)
        {
            sqr = SqrMagnitude(cells[i].center - upperLeftCorner);
            if (sqr < sqrMag || (Mathf.Abs(sqr - sqrMag) < EPSEXT && cells[i].center.y > cells[cell].center.y))
            {
                cell = i;
                sqrMag = sqr;
            }
        }
        // От левого верхнего угла к правому верхнему
        LineBuilder(upperLeftCorner, upperRightCorner, ref cell, intersectionsUp);
        LineBuilder(upperRightCorner, lowerRightCorner, ref cell, intersectionsRight);
        LineBuilder(lowerRightCorner, lowerLeftCorner, ref cell, intersectionsDown);
        LineBuilder(lowerLeftCorner, upperLeftCorner, ref cell, intersectionsLeft);
    }

    private void LineBuilder(Vector2 startPoint, Vector2 endPoint, ref int cell, List<BoundIntersection> list)
    {
        Vector2 midPoint = startPoint;
        VoronoiEdge newEdge = null;
        for (int i = 0; i < list.Count; ++i)
        {
            newEdge = new VoronoiEdge(midPoint, list[i].point, -1, -1);
            edges.Add(newEdge);
            cells[cell].edges.Add(newEdge);
            if (cell == list[i].edge.leftCell)
                cell = list[i].edge.rightCell;
            else
                cell = list[i].edge.leftCell;
            midPoint = list[i].point;
        }
        newEdge = new VoronoiEdge(midPoint, endPoint, -1, -1);
        edges.Add(newEdge);
        cells[cell].edges.Add(newEdge);
    }

    private void CreateMeshes(float depth, GameObject prefab, Transform parent)
    {
        IComparer<Vector2Radial> comparer = new Vector2RadialComparer();
        List<Vector2> frontVertices = new List<Vector2>();
        List<Vector2Radial> radialVertices = new List<Vector2Radial>();
        float offsetZ = depth / 2; int index = 0;
        if (!AssetDatabase.IsValidFolder("Assets\\Temp")) AssetDatabase.CreateFolder("Assets", "Temp");
        foreach (VoronoiCell cell in cells)
        {
            Mesh mesh = new Mesh();
            frontVertices.Clear();
            foreach (VoronoiEdge edge in cell.edges)
            {
                if (!frontVertices.Contains(edge.point1)) frontVertices.Add(edge.point1);
                if (!frontVertices.Contains(edge.point2)) frontVertices.Add(edge.point2);
            }
            radialVertices.Clear();
            foreach (Vector2 vert in frontVertices)
            {
                radialVertices.Add(new Vector2Radial(vert, cell.center));
            }
            radialVertices.Sort(comparer);

            int m = radialVertices.Count, n = m + 1, m3 = 3 * m, m6 = 6 * m;
            Debug.Log(m);
            Vector3[] vertices = new Vector3[2 * n];
            vertices[m].x = cell.center.x;
            vertices[m].y = cell.center.y;
            vertices[m].z = offsetZ;

            vertices[n + m].x = cell.center.x;
            vertices[n + m].y = cell.center.y;
            vertices[n + m].z = -offsetZ;

            for (int i = 0; i < m; ++i)
            {
                vertices[i].x = radialVertices[i].point.x;
                vertices[i].y = radialVertices[i].point.y;
                vertices[i].z = offsetZ;

                vertices[n + i].x = radialVertices[i].point.x;
                vertices[n + i].y = radialVertices[i].point.y;
                vertices[n + i].z = -offsetZ;
            }

            int[] triangles = new int[12 * m];

            for (int i = 0; i < m - 1; ++i)
            {
                triangles[3 * i] = i;
                triangles[3 * i + 1] = i + 1;
                triangles[3 * i + 2] = m;

                triangles[m3 + 3 * i] = n + i;
                triangles[m3 + 3 * i + 1] = n + i + 1;
                triangles[m3 + 3 * i + 2] = n + m;

                triangles[m6 + 6 * i] = i;
                triangles[m6 + 6 * i + 1] = i + 1;
                triangles[m6 + 6 * i + 2] = n + i;
                triangles[m6 + 6 * i + 3] = n + i;
                triangles[m6 + 6 * i + 4] = i + 1;
                triangles[m6 + 6 * i + 5] = n + i + 1;
            }
            triangles[m3 - 3] = m - 1;
            triangles[m3 - 2] = 0;
            triangles[m3 - 1] = m;

            triangles[m6 - 3] = m + m;
            triangles[m6 - 2] = n;
            triangles[m6 - 1] = n + m;

            triangles[2 * m6 - 6] = m - 1;
            triangles[2 * m6 - 5] = 0;
            triangles[2 * m6 - 4] = m + m;
            triangles[2 * m6 - 3] = m + m;
            triangles[2 * m6 - 2] = 0;
            triangles[2 * m6 - 1] = n;

            mesh.vertices = vertices;
            mesh.triangles = triangles;

            AssetDatabase.CreateAsset(mesh, "Assets\\Temp\\mesh" + index + ".asset");
            GameObject part = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            part.transform.parent = parent;
            part.transform.localPosition = Vector3.zero;
            part.GetComponent<MeshFilter>().mesh = mesh;

            index++;
        }
    }
}
