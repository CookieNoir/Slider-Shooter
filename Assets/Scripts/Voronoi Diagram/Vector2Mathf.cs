using UnityEngine;

public class Vector2Mathf
{
    public const double EPS = 1E-6;
    public const double EPSEXT = 1E-8;

    public static void Swap<T>(ref T lhs, ref T rhs)
    {
        T temp = lhs;
        lhs = rhs;
        rhs = temp;
    }

    public static float Det(float a, float b, float c, float d)
    {
        return a * d - b * c;
    }

    public static bool Between(float a, float b, double c)
    {
        return Mathf.Min(a, b) <= c + EPS && c <= Mathf.Max(a, b) + EPS;
    }

    public static bool Intersect_1(float a, float b, float c, float d)
    {
        if (a > b) Swap(ref a, ref b);
        if (c > d) Swap(ref c, ref d);
        return Mathf.Max(a, c) <= Mathf.Min(b, d);
    }

    public static bool SegmentIntersect(Vector2 a, Vector2 b, Vector2 c, Vector2 d, ref Vector2 intersectionPoint)
    {
        float A1 = a.y - b.y, B1 = b.x - a.x, C1 = -A1 * a.x - B1 * a.y;
        float A2 = c.y - d.y, B2 = d.x - c.x, C2 = -A2 * c.x - B2 * c.y;
        float zn = Det(A1, B1, A2, B2);
        if (zn != 0)
        {

            intersectionPoint.x = (float)(Det(B1, B2, C1, C2) * 1.0 / zn);
            intersectionPoint.y = (float)(Det(A2, A1, C2, C1) * 1.0 / zn);
            return Between(a.x, b.x, intersectionPoint.x) && Between(a.y, b.y, intersectionPoint.y)
                && Between(c.x, d.x, intersectionPoint.x) && Between(c.y, d.y, intersectionPoint.y);
        }
        else
            return Det(A1, C1, A2, C2) == 0 && Det(B1, C1, B2, C2) == 0
                && Intersect_1(a.x, b.x, c.x, d.x)
                && Intersect_1(a.y, b.y, c.y, d.y);
    }

    public static bool StraightLineIntersect(Vector2 a, Vector2 b, Vector2 c, Vector2 d, ref Vector2 intersectionPoint)
    {
        float A1 = a.y - b.y, B1 = b.x - a.x, C1 = -A1 * a.x - B1 * a.y;
        float A2 = c.y - d.y, B2 = d.x - c.x, C2 = -A2 * c.x - B2 * c.y;
        float zn = Det(A1, B1, A2, B2);
        if (zn != 0) // Прямые пересекаются
        {

            intersectionPoint.x = (float)(Det(B1, B2, C1, C2) * 1.0 / zn);
            intersectionPoint.y = (float)(Det(A2, A1, C2, C1) * 1.0 / zn);
            return true;
        }
        else // Прямые параллельны
            return Det(A1, C1, A2, C2) == 0 && Det(B1, C1, B2, C2) == 0;
    }

    // Типы:
    // 0 - прямая, 1 - луч с началом в первой точке, 2 - луч с началом во второй точке, 3 - отрезок
    public static bool UniversalIntersect(Vector2 a, Vector2 b, int type1, Vector2 c, Vector2 d, int type2, ref Vector2 intersectionPoint)
    {
        float A1 = a.y - b.y, B1 = b.x - a.x, C1 = -A1 * a.x - B1 * a.y;
        float A2 = c.y - d.y, B2 = d.x - c.x, C2 = -A2 * c.x - B2 * c.y;
        float zn = Det(A1, B1, A2, B2);
        switch (type1)
        {
            case 0:
                {
                    float bx = b.x, by = b.y;
                    if (b.x - a.x != 0) bx = (b.x - a.x) / 0;
                    if (b.y - a.y != 0) by = (b.y - a.y) / 0;
                    if (a.x - b.x != 0) a.x = (a.x - b.x) / 0;
                    if (a.y - b.y != 0) a.y = (a.y - b.y) / 0;
                    b.x = bx;
                    b.y = by;
                    break;
                }
            case 1:
                {
                    if (b.x - a.x != 0) b.x = (b.x - a.x) / 0;
                    if (b.y - a.y != 0) b.y = (b.y - a.y) / 0;
                    break;
                }
            case 2:
                {
                    if (a.x - b.x != 0) a.x = (a.x - b.x) / 0;
                    if (a.y - b.y != 0) a.y = (a.y - b.y) / 0;
                    break;
                }
        }
        switch (type2)
        {
            case 0:
                {
                    float dx = d.x, dy = d.y;
                    if (d.x - c.x != 0) dx = (d.x - c.x) / 0;
                    if (d.y - c.y != 0) dy = (d.y - c.y) / 0;
                    if (c.x - d.x != 0) c.x = (c.x - d.x) / 0;
                    if (c.y - d.y != 0) c.y = (c.y - d.y) / 0;
                    d.x = dx;
                    d.y = dy;
                    break;
                }
            case 1:
                {
                    if (d.x - c.x != 0) d.x = (d.x - c.x) / 0;
                    if (d.y - c.y != 0) d.y = (d.y - c.y) / 0;
                    break;
                }
            case 2:
                {
                    if (c.x - d.x != 0) c.x = (c.x - d.x) / 0;
                    if (c.y - d.y != 0) c.y = (c.y - d.y) / 0;
                    break;
                }
        }
        if (zn != 0) // Прямые пересекаются
        {

            intersectionPoint.x = (float)(Det(B1, B2, C1, C2) * 1.0 / zn);
            intersectionPoint.y = (float)(Det(A2, A1, C2, C1) * 1.0 / zn);
            return Between(a.x, b.x, intersectionPoint.x) && Between(a.y, b.y, intersectionPoint.y)
                && Between(c.x, d.x, intersectionPoint.x) && Between(c.y, d.y, intersectionPoint.y);
        }
        else // Прямые параллельны
        {
            return Det(A1, C1, A2, C2) == 0 && Det(B1, C1, B2, C2) == 0
                && Intersect_1(a.x, b.x, c.x, d.x)
                && Intersect_1(a.y, b.y, c.y, d.y);
        }
    }

    public static bool InsideBox(Vector2 lowerCorner, Vector2 upperCorner, Vector2 point)
    {
        return lowerCorner.x <= point.x && point.x <= upperCorner.x &&
               lowerCorner.y <= point.y && point.y <= upperCorner.y;
    }

    public static float SqrMagnitude(Vector2 vector)
    {
        return vector.x * vector.x + vector.y * vector.y;
    }

    public static bool IsFirstHigher(Vector2 first, Vector2 second, bool isLeft)
    {
        if (first.y > second.y) return true;
        else if (first.y < second.y) return false;
        else
        {
            if ((first.x > second.x) == isLeft) return true;
            else return false;
        }
    }
}
