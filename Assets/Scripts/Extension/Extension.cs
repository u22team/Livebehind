using UnityEngine;
using System.Collections;

public static class Extension
{
    //public static Vector3Int forward { get { return new Vector3Int(0, 0, 1); } }

    public static Vector3Int Turn(this Vector3Int md, int i)
    {
        return md = new Vector3Int(-md.z, 0, md.x) * -i;
    }

    public static Vector3Int RoundToInt(this Vector3 vc)
    {
        return new Vector3Int(Mathf.RoundToInt(vc.x), Mathf.RoundToInt(vc.y), Mathf.RoundToInt(vc.z));
    }

    public static int Abs(this int num)
    {
        if (num < 0) num *= -1;
        return num;
    }
}