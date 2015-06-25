using UnityEngine;
using System.Collections;

public class RayHitComparer : IComparer
{
    public int Compare(object x, object y)
    {
        return ((RaycastHit)x).distance.CompareTo(((RaycastHit)y).distance);
    }
}
