using System.Collections.Generic;

public class Vector2RadialComparer : IComparer<VoronoiWall.Vector2Radial>
{
    public int Compare(VoronoiWall.Vector2Radial left, VoronoiWall.Vector2Radial right)
    {
        if (left.angle < right.angle) return -1;
        else if (left.angle == right.angle) return 0;
        else return 1;
    }
}
