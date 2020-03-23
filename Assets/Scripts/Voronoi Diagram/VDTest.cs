using UnityEngine;

public class VDTest : MonoBehaviour
{
    public Vector2[] points;
    public Vector2 border1;
    public Vector2 border2;
    private VoronoiDiagram voronoiDiagram;
    private bool created = false;

    public void CreateVoronoiDiagram()
    {
        voronoiDiagram = new VoronoiDiagram(points, border1, border2);
        created = true;
    }

    public void DropDiagram()
    {
        voronoiDiagram = null;
        created = false;
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (created)
        {
            Gizmos.DrawLine(transform.position + new Vector3(voronoiDiagram.lowerLeftCorner.x, voronoiDiagram.lowerLeftCorner.y, 0),
                transform.position + new Vector3(voronoiDiagram.lowerRightCorner.x, voronoiDiagram.lowerRightCorner.y, 0));
            Gizmos.DrawLine(transform.position + new Vector3(voronoiDiagram.lowerLeftCorner.x, voronoiDiagram.lowerLeftCorner.y, 0),
                transform.position + new Vector3(voronoiDiagram.upperLeftCorner.x, voronoiDiagram.upperLeftCorner.y, 0));
            Gizmos.DrawLine(transform.position + new Vector3(voronoiDiagram.upperLeftCorner.x, voronoiDiagram.upperLeftCorner.y, 0),
                transform.position + new Vector3(voronoiDiagram.upperRightCorner.x, voronoiDiagram.upperRightCorner.y, 0));
            Gizmos.DrawLine(transform.position + new Vector3(voronoiDiagram.upperRightCorner.x, voronoiDiagram.upperRightCorner.y, 0),
                transform.position + new Vector3(voronoiDiagram.lowerRightCorner.x, voronoiDiagram.lowerRightCorner.y, 0));

            Gizmos.color = Color.green;
            foreach (VoronoiDiagram.VoronoiEdge edge in voronoiDiagram.edges)
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
