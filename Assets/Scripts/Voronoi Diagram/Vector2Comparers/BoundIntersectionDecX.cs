using System.Collections.Generic;

public class BoundIntersectionDecX : IComparer<VoronoiDiagram.BoundIntersection>
{
    public int Compare(VoronoiDiagram.BoundIntersection left, VoronoiDiagram.BoundIntersection right)
    {
        if (left.point.x > right.point.x) return -1;
        else if (left.point.x == right.point.x) return 0;
        else return 1;
    }
}
