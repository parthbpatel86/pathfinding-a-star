using UnityEngine;

public class NavLocation
{
    public int x;
    public int z;

    public NavLocation(int _x, int _z)
    {
        x = _x;
        z = _z;
    }

    public Vector2 ToVector()
    {
        return new Vector2(x, z);
    }

    public static NavLocation operator +(NavLocation a, NavLocation b)
       => new NavLocation(a.x + b.x, a.z + b.z);

    public override bool Equals(object obj)
    {
        if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            return false;
        else
            return x == ((NavLocation)obj).x && z == ((NavLocation)obj).z;
    }

    public override int GetHashCode()
    {
        return 0;
    }
}
