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
        float offsetZ = depth / 2; int index = 0, pos;
        Vector3 normal;
        if (!AssetDatabase.IsValidFolder("Assets\\Temp")) AssetDatabase.CreateFolder("Assets", "Temp");
        GameObject[] selections = new GameObject[cells.Count];
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

            Vector3[] vertices = new Vector3[2 * n + 6 * m];
            Vector3[] normals = new Vector3[2 * n + 6 * m];

            vertices[m].x = cell.center.x;
            vertices[m].y = cell.center.y;
            vertices[m].z = offsetZ;

            normals[m].x = 0;
            normals[m].y = 0;
            normals[m].z = 1;

            vertices[n + m].x = cell.center.x;
            vertices[n + m].y = cell.center.y;
            vertices[n + m].z = -offsetZ;

            normals[n + m].x = 0;
            normals[n + m].y = 0;
            normals[n + m].z = -1;

            for (int i = 0; i < m; ++i)
            {
                vertices[i].x = radialVertices[i].point.x;
                vertices[i].y = radialVertices[i].point.y;
                vertices[i].z = offsetZ;

                normals[i].x = 0;
                normals[i].y = 0;
                normals[i].z = 1;

                vertices[n + i].x = radialVertices[i].point.x;
                vertices[n + i].y = radialVertices[i].point.y;
                vertices[n + i].z = -offsetZ;

                normals[n + i].x = 0;
                normals[n + i].y = 0;
                normals[n + i].z = -1;
            }

            int[] triangles = new int[12 * m];

            for (int i = 0; i < m - 1; ++i)
            {
                triangles[3 * i] = i;
                triangles[3 * i + 1] = i + 1;
                triangles[3 * i + 2] = m;

                triangles[m3 + 3 * i] = n + i;
                triangles[m3 + 3 * i + 1] = n + m;
                triangles[m3 + 3 * i + 2] = n + i + 1;

                pos = 2 * n + 6 * i;
                vertices[pos] = vertices[i];
                vertices[pos + 1] = vertices[i + 1];
                vertices[pos + 2] = vertices[n + i];
                vertices[pos + 3] = vertices[n + i];
                vertices[pos + 4] = vertices[i + 1];
                vertices[pos + 5] = vertices[n + i + 1];
                normals[pos] = LineNormal(vertices[pos], vertices[pos + 1], vertices[m]);
                normals[pos + 1] = normals[pos];
                normals[pos + 2] = normals[pos];
                normals[pos + 3] = normals[pos];
                normals[pos + 4] = normals[pos];
                normals[pos + 5] = normals[pos];

                triangles[m6 + 6 * i] = pos;
                triangles[m6 + 6 * i + 1] = pos + 2;
                triangles[m6 + 6 * i + 2] = pos + 1;
                triangles[m6 + 6 * i + 3] = pos + 3;
                triangles[m6 + 6 * i + 4] = pos + 5;
                triangles[m6 + 6 * i + 5] = pos + 4;
            }
            triangles[m3 - 3] = m - 1;
            triangles[m3 - 2] = 0;
            triangles[m3 - 1] = m;

            triangles[m6 - 3] = m + m;
            triangles[m6 - 2] = n + m;
            triangles[m6 - 1] = n;

            pos = 2 * n + 6 * m - 6;
            vertices[pos] = vertices[m - 1];
            vertices[pos + 1] = vertices[0];
            vertices[pos + 2] = vertices[m + m];
            vertices[pos + 3] = vertices[m + m];
            vertices[pos + 4] = vertices[0];
            vertices[pos + 5] = vertices[n];

            normals[pos] = LineNormal(vertices[pos], vertices[pos + 1], vertices[m]);
            normals[pos + 1] = normals[pos];
            normals[pos + 2] = normals[pos];
            normals[pos + 3] = normals[pos];
            normals[pos + 4] = normals[pos];
            normals[pos + 5] = normals[pos];

            triangles[2 * m6 - 6] = pos;
            triangles[2 * m6 - 5] = pos + 2;
            triangles[2 * m6 - 4] = pos + 1;
            triangles[2 * m6 - 3] = pos + 3;
            triangles[2 * m6 - 2] = pos + 5;
            triangles[2 * m6 - 1] = pos + 4;

            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.triangles = triangles;

            AssetDatabase.CreateAsset(mesh, "Assets\\Temp\\mesh" + index + ".asset");
            GameObject part = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            part.name = part.name + ' ' + index;
            part.transform.parent = parent;
            part.transform.localPosition = Vector3.zero;
            part.GetComponent<MeshFilter>().mesh = mesh;

            selections[index] = part;

            index++;
        }

        ObjExporter.DoExport(selections);
    }
}
