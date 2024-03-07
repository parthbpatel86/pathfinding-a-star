using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using TMPro;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
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

    List<PathMarker> _open = new List<PathMarker>();
    Dictionary<NavLocation, PathMarker> _closed = new Dictionary<NavLocation, PathMarker>();

    Action<NavLocation[]> _donePathSearchCB;

    void RemoveAllMarkers()
    {
        GameObject[] markers = GameObject.FindGameObjectsWithTag("PathVisual");
        foreach (GameObject m in markers) GameObject.Destroy(m);
    }

    public void BeginSearch(int x1, int z1, int x2, int z2, Action<NavLocation[]> OnSucceedCB)
    {
        done = false;
        RemoveAllMarkers();

        Vector3 startLocation = new Vector3(x1, 0.0f, z1);
        _startNode = new PathMarker(new NavLocation(x1, z1),
            0.0f, 0.0f, 0.0f, null, GameObject.Instantiate(_startPrefab, startLocation, Quaternion.identity));

        _goalNode = new PathMarker(new NavLocation(x2, z2),
            0.0f, 0.0f, 0.0f, null, GameObject.Instantiate(_startPrefab, startLocation, Quaternion.identity));

        _open.Clear();
        _closed.Clear();

        _open.Add(_startNode);
        _lastPos = _startNode;

        _hasStarted = true;
        _donePathSearchCB = OnSucceedCB;
    }

    void Update()
    {
        if (_hasStarted && !done)
            Search(_lastPos);
    }

    NavLocation[] GetPath()
    {
        PathMarker begin = _lastPos;
        Stack<NavLocation> navPath = new Stack<NavLocation>();
        navPath.Push(begin.location);
        while (!_startNode.Equals(begin) && begin != null)
        {
            #if VISUAL
            begin.marker.GetComponent<Renderer>().material.color = Color.cyan;
            #endif

            begin = begin.parent;
            navPath.Push(begin.location);
        }

        #if VISUAL
        GameObject.Instantiate(_pathPrefab, new Vector3(_startNode.location.x, 0, _startNode.location.z), Quaternion.identity);
        #endif
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

        // check to see if this node is valid and added or updated to the list
        CalculateMarkers(thisNode);

        // sort the list by F and if tied on F then sort by H value
        _open = _open.OrderBy(p => p.F).ThenBy(n => n.H).ToList<PathMarker>();

        PathMarker pm = _open.ElementAt(0);
        _closed.Add(pm.location, pm);

        _open.RemoveAt(0);
        #if VISUAL
        pm.marker.GetComponent<Renderer>().material = _closedMaterial;
        #endif
        _lastPos = pm;
    }

    private void CalculateMarkers(PathMarker thisNode)
    {
        List<NavLocation> list = _navGrid.GetDirection();
        for (int i = 0; i < list.Count; i++)
        {
            NavLocation dir = list[i];
            NavLocation neighbour = dir + thisNode.location;

            // has wall or out of bound
            if (!_navGrid.IsPassable(neighbour.x, neighbour.z))
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

            // visuals
            GameObject pathBlock = null;
            #if VISUAL
            pathBlock = GameObject.Instantiate(_pathPrefab, new Vector3(neighbour.x, 0.0f, neighbour.z), Quaternion.identity);
            TextMeshPro[] values = pathBlock.GetComponentsInChildren<TextMeshPro>();
            values[0].text = "G:" + g.ToString("0.00") + "\nH:" + h.ToString("0.00") + "\nF:" + f.ToString("0.00");
            #endif
            if (!UpdateMarker(neighbour, g, h, f, thisNode))
            {
                #if VISUAL
                pathBlock.GetComponent<Renderer>().material = _openMaterial;
                #endif
                _open.Add(new PathMarker(neighbour, g, h, f, thisNode, pathBlock));
            }
        }
    }

    bool UpdateMarker(NavLocation pos, float g, float h, float f, PathMarker prt)
    {
        for (int i = 0; i < _open.Count; i++)
        {
            PathMarker p = _open[i];
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
        return _closed.ContainsKey(marker);
    }
}