using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using Unity.Jobs;
using Unity.VisualScripting;
using UnityEngine.XR;
using UnityEngine;
using System.ComponentModel;
/*
public class FindPathJob : IJob
{
    PathMarker _startNode;
    PathMarker _goalNode;
    PathMarker _lastPos;
    bool _hasStarted;
    bool done = false;

    List<PathMarker> open = new List<PathMarker>();
    List<PathMarker> closed = new List<PathMarker>();
    public void Execute()
    {
        Search(_lastPos);
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
        open = open.OrderBy(p => p.F).ThenBy(n => n.H).ToList<PathMarker>();

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
*/