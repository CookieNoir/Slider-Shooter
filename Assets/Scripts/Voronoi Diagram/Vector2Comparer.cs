using System.Collections;
using UnityEngine;

public class Vector2Comparer : IComparer
{
    public int Compare(object x, object y)
    {
        Vector2 left = (Vector2)x;
        Vector2 right = (Vector2)y;

        if (left.x < right.x) return -1;
        else if (left.x > right.x) return 1;
        else
        {
            if (left.y < right.y) return -1;
            else if (left.y == right.y) return 0;
            else return 1;
        }
    }
}
