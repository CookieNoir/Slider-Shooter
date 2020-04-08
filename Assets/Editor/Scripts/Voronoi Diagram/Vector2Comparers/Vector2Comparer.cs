using System.Collections.Generic;
using UnityEngine;

public class Vector2Comparer : IComparer<Vector2>
{
    public int Compare(Vector2 left, Vector2 right)
    {
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
