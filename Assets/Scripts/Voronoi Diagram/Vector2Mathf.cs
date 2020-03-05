using UnityEngine;

public class Vector2Mathf
{
    public const double EPS = 1E-9;

    public static void Swap<T>(ref T lhs, ref T rhs)
    {
        T temp = lhs;
        lhs = rhs;
        rhs = temp;
    }

    public static float det(float a, float b, float c, float d)
    {
        return a * d - b * c;
    }

    public static bool between(float a, float b, double c)
    {
        return Mathf.Min(a, b) <= c + EPS && c <= Mathf.Max(a, b) + EPS;
    }

    public static bool intersect_1(float a, float b, float c, float d)
    {
        if (a > b) Swap(ref a, ref b);
        if (c > d) Swap(ref c, ref d);
        return Mathf.Max(a, c) <= Mathf.Min(b, d);
    }

    public static bool intersect(Vector2 a, Vector2 b, Vector2 c, Vector2 d, ref Vector2 intersectionPoint)
    {
        float A1 = a.y - b.y, B1 = b.x - a.x, C1 = -A1 * a.x - B1 * a.y;
        float A2 = c.y - d.y, B2 = d.x - c.x, C2 = -A2 * c.x - B2 * c.y;
        float zn = det(A1, B1, A2, B2);
        if (zn != 0)
        {

            intersectionPoint.x = (float)(det(B1, B2, C1, C2) * 1.0 / zn);
            intersectionPoint.y = (float)(det(A2, A1, C2, C1) * 1.0 / zn);
            return between(a.x, b.x, intersectionPoint.x) && between(a.y, b.y, intersectionPoint.y)
                && between(c.x, d.x, intersectionPoint.x) && between(c.y, d.y, intersectionPoint.y);
        }
        else
            return det(A1, C1, A2, C2) == 0 && det(B1, C1, B2, C2) == 0
                && intersect_1(a.x, b.x, c.x, d.x)
                && intersect_1(a.y, b.y, c.y, d.y);
    }

    public static bool straightLineIntersect(Vector2 a, Vector2 b, Vector2 c, Vector2 d, ref Vector2 intersectionPoint)
    {
        float A1 = a.y - b.y, B1 = b.x - a.x, C1 = -A1 * a.x - B1 * a.y;
        float A2 = c.y - d.y, B2 = d.x - c.x, C2 = -A2 * c.x - B2 * c.y;
        float zn = det(A1, B1, A2, B2);
        if (zn != 0) // Прямые пересекаются
        {

            intersectionPoint.x = (float)(det(B1, B2, C1, C2) * 1.0 / zn);
            intersectionPoint.y = (float)(det(A2, A1, C2, C1) * 1.0 / zn);
            return true;
        }
        else // Прямые параллельны
            return false;
    }

    public static bool insideBox(Vector2 lowerCorner, Vector2 upperCorner, Vector2 point)
    {
        return lowerCorner.x <= point.x && point.x <= upperCorner.x &&
               lowerCorner.y <= point.y && point.y <= upperCorner.y;
    }

    public static float SqrMagnitude(Vector2 vector)
    {
        return vector.x * vector.x + vector.y * vector.y;
    }

    public static bool isFirstHigher(Vector2 first, Vector2 second)
    {
        if (first.y > second.y) return true;
        else if (first.y < second.y) return false;
        else
        {
            if (first.x > second.x) return true;
            else return false;
        }
    }
}
