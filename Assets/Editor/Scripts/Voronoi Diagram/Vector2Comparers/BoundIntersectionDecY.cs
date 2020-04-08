using System.Collections.Generic;

public class BoundIntersectionDecY : IComparer<VoronoiDiagram.BoundIntersection>
{
    public int Compare(VoronoiDiagram.BoundIntersection left, VoronoiDiagram.BoundIntersection right)
    {
        if (left.point.y > right.point.y) return -1;
        else if (left.point.y == right.point.y) return 0;
        else return 1;
    }
}
