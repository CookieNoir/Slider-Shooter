using UnityEngine;

public class VoronoiWallHandler : MonoBehaviour
{
    public Vector2[] points;
    public float depth = 1f;
    public Vector2 border1;
    public Vector2 border2;
    public GameObject partPrefab;
    private VoronoiWall voronoiWall;
    private bool created = false;

    public void CreateVoronoiDiagram()
    {
        voronoiWall = new VoronoiWall(points, border1, border2, depth, transform,  partPrefab);
        created = true;
    }

    public void DropDiagram()
    {
        voronoiWall = null;
        created = false;
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (created)
        {
            Gizmos.color = Color.green;
            foreach (VoronoiDiagram.VoronoiEdge edge in voronoiWall.edges)
            {
                Gizmos.DrawLine(transform.position + new Vector3(edge.point1.x, edge.point1.y, 0),
                transform.position + new Vector3(edge.point2.x, edge.point2.y, 0));
            }
        }
        else
        {
            Gizmos.DrawSphere(transform.position + new Vector3(border1.x, border1.y, 0), 0.1f);
            Gizmos.DrawSphere(transform.position + new Vector3(border2.x, border2.y, 0), 0.1f);
        }
        Gizmos.color = Color.blue;
        foreach (Vector2 point in points)
        {
            Gizmos.DrawSphere(transform.position + new Vector3(point.x, point.y, 0), 0.1f);
        }
    }
}
