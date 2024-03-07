using System;
using System.Collections.Generic;
using UnityEngine;

public class NavGrid : MonoBehaviour
{
    [SerializeField]
    int _width = 30; //x length
    [SerializeField]
    int _depth = 30; //z length
    [SerializeField]
    int _scale = 6;
    [SerializeField]
    FindPathAStar _aStar;
    [SerializeField]
    GameObject _player;
    [SerializeField]
    GameObject _plane;

    byte[,] _map;
    readonly List<NavLocation> _directions = new List<NavLocation>()
    {
        new NavLocation(1,0), new NavLocation(0,1), new NavLocation(-1,0), new NavLocation(0,-1),
        new NavLocation(1,1), new NavLocation(1,-1), new NavLocation(-1,-1), new NavLocation(-1,1)
    };

    public int Width { get { return _width; } }
    public int Depth { get { return _depth; } }

    /// <summary>
    /// Given the current and desired location, return a path using call back method
    /// </summary>
    public void GetPath(Vector3 origin, Vector3 destination, Action<NavLocation[]> OnSucceedFindingPathCb)
    {
        // start the A* path finding
        _aStar.BeginSearch((int)Math.Round(origin.x), (int)Math.Round(origin.z), (int)Math.Round(destination.x), (int)Math.Round(destination.z), OnSucceedFindingPathCb);
    }

    public List<NavLocation> GetDirection()
    {
        return _directions;
    }

    public byte GetMapByte(int x, int z)
    {
        return _map[x, z];
    }

    void Start()
    {
        InitialiseMap();
        Generate();
        DrawMap();
    }

    void InitialiseMap()
    {
        _map = new byte[_width, _depth];

        for (int z = 0; z < _depth; z++)
            for (int x = 0; x < _width; x++)
            {
                _map[x, z] = 1;     //1 = obj  0 = walkable
            }
        _map[(int)Math.Round(_player.transform.position.x), (int)Math.Round(_player.transform.position.z)] = 0;
    }

    public virtual void Generate()
    {
        for (int z = 0; z < _depth; z++)
            for (int x = 0; x < _width; x++)
            {
                if (UnityEngine.Random.Range(0, 100) < 80)
                    _map[x, z] = 0;     //1 = wall  0 = walkable
            }
    }

    void DrawMap()
    {
        for (int z = 0; z < _depth; z++)
            for (int x = 0; x < _width; x++)
            {
                if (_map[x, z] == 1)
                {
                    Vector3 pos = new Vector3(x * _scale, 0, z * _scale);
                    GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    wall.transform.localScale = new Vector3(_scale, _scale, _scale);
                    wall.transform.position = pos;
                    wall.transform.SetParent(this.transform);
                    wall.GetComponent<Collider>().enabled = false;
                }
            }
    
        _plane.transform.position = new Vector3(_width / 2, 0, _depth / 2);
        _plane.transform.localScale = new Vector3(_width / 10, _plane.transform.localScale.y, _depth / 10);
    }
}
