using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class FindPathAStar : MonoBehaviour
{
    [SerializeField]
    NavGrid _navGrid;
    [SerializeField]
    Material _closedMaterial;
    [SerializeField]
    Material _openMaterial;
    [SerializeField]
    GameObject _startPrefab;
    [SerializeField]
    GameObject _endPrefab;
    [SerializeField]
    GameObject _pathPrefab;

    PathMarker _startNode;
    PathMarker _goalNode;
    PathMarker _lastPos;
    bool _hasStarted;
    bool done = false;

    List<PathMarker> open = new List<PathMarker>();
    List<PathMarker> closed = new List<PathMarker>();

    Action<NavLocation[]> _donePathSearchCB;

    void RemoveAllMarkers()
    {
        GameObject[] markers = GameObject.FindGameObjectsWithTag("PathVisual");
        foreach (GameObject m in markers) GameObject.Destroy(m);
    }

    public void BeginSearch(int x1, int z1, int x2, int z2, Action<NavLocation[]> cb)
    {
        done = false;
        RemoveAllMarkers();

        Vector3 startLocation = new Vector3(x1, 0.0f, z1);
        _startNode = new PathMarker(new NavLocation(x1, z1),
            0.0f, 0.0f, 0.0f, GameObject.Instantiate(_startPrefab, startLocation, Quaternion.identity), null);

        Vector3 endLocation = new Vector3(x2, 0.0f, z2);
        _goalNode = new PathMarker(new NavLocation(x2, z2),
            0.0f, 0.0f, 0.0f, GameObject.Instantiate(_endPrefab, endLocation, Quaternion.identity), null);

        open.Clear();
        closed.Clear();

        open.Add(_startNode);
        _lastPos = _startNode;

        _hasStarted = true;
        _donePathSearchCB = cb;
    }

    void Update()
    {
        if (_hasStarted && !done)
            Search(_lastPos);
    }

    NavLocation[] GetPath()
    {
        RemoveAllMarkers();
        PathMarker begin = _lastPos;
        Stack<NavLocation> navPath = new Stack<NavLocation>();
        navPath.Push(begin.location);
        while (!_startNode.Equals(begin) && begin != null)
        {
            GameObject.Instantiate(_pathPrefab, new Vector3(begin.location.x, 0, begin.location.z), Quaternion.identity);
            begin = begin.parent;
            navPath.Push(begin.location);
        }

        GameObject.Instantiate(_pathPrefab, new Vector3(_startNode.location.x, 0, _startNode.location.z), Quaternion.identity);
        return navPath.ToArray();
    }

    void Search(PathMarker thisNode)
    {
        if (thisNode.Equals(_goalNode))
        {
            done = true;
            Debug.Log("DONE!");
            _hasStarted = false;
            _donePathSearchCB(GetPath());
            return;
        }

        foreach (NavLocation dir in _navGrid.GetDirection())
        {
            NavLocation neighbour = dir + thisNode.location;

            // edge of the board
            if (neighbour.x < 1 || neighbour.x >= _navGrid.Width || neighbour.z < 1 || neighbour.z >= _navGrid.Depth)
            {
                continue;
            }

            // Blocked
            if (_navGrid.GetMapByte(neighbour.x, neighbour.z) == 1)
            {
                continue;
            }

            // if its already closed previously
            if (IsClosed(neighbour))
            {
                continue;
            }

            float g = Vector2.Distance(thisNode.location.ToVector(), neighbour.ToVector()) + thisNode.G;
            float h = Vector2.Distance(neighbour.ToVector(), _goalNode.location.ToVector());
            float f = g + h;

            GameObject pathBlock = GameObject.Instantiate(_pathPrefab, new Vector3(neighbour.x, 0.0f, neighbour.z), Quaternion.identity);

            TextMeshPro[] values = pathBlock.GetComponentsInChildren<TextMeshPro>();
            values[0].text = "G:" + g.ToString("0.0") + "\nH:" + h.ToString("0.0") + "\nF:" + f.ToString("0.0");

            if (!UpdateMarker(neighbour, g, h, f, thisNode))
            {

                open.Add(new PathMarker(neighbour, g, h, f, pathBlock, thisNode));
            }
        }
        open = open.OrderBy(p => p.F).ThenBy(n=>n.H).ToList<PathMarker>();

        PathMarker pm = (PathMarker)open.ElementAt(0);
        closed.Add(pm);

        open.RemoveAt(0);
        pm.marker.GetComponent<Renderer>().material = _closedMaterial;
        _lastPos = pm;
    }

    bool UpdateMarker(NavLocation pos, float g, float h, float f, PathMarker prt)
    {
        foreach (PathMarker p in open)
        {
            if (p.location.Equals(pos))
            {
                p.G = g;
                p.H = h;
                p.F = f;
                p.parent = prt;
                return true;
            }
        }
        return false;
    }

    bool IsClosed(NavLocation marker)
    {
        foreach (PathMarker p in closed)
        {
            if (p.location.Equals(marker)) return true;
        }
        return false;
    }
}
