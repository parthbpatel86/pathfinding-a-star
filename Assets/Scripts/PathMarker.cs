using UnityEngine;

public class PathMarker
{
    public NavLocation location;
    public float G, H, F;
    public PathMarker parent;
    // visual
    public GameObject marker = null;

    public PathMarker(NavLocation l, float g, float h, float f, PathMarker p, GameObject m = null)
    {
        location = l;
        G = g;
        H = h;
        F = f;
        marker = m;
        parent = p;
    }

    public override bool Equals(object obj)
    {
        if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            return false;
        else
            return location.Equals(((PathMarker)obj).location);
    }

    public override int GetHashCode()
    {
        return 0;
    }
}