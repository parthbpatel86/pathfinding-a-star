using System;
using UnityEngine;

public class Player : MonoBehaviour
{    
    [SerializeField]
    NavGrid _grid;
    [SerializeField]
    float _speed = 10.0f;

    bool _serachingPath;
    NavLocation[] _currentPath = Array.Empty<NavLocation>();
    int _currentPathIndex = 0;

    void Update()
    {
        // Check Input
        if (Input.GetMouseButtonUp(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hitInfo))
            {
                if (hitInfo.transform.tag == "Floor")
                {
                    _grid.GetPath(transform.position, hitInfo.point, OnFinishSearchPath);
                    _currentPathIndex = 0;
                    _serachingPath = true;
                }
            }
        }

        // Traverse
        if (!_serachingPath && _currentPathIndex < _currentPath.Length)
        {
            var currentNode = _currentPath[_currentPathIndex];

            var nextNodeLocation = new Vector3(currentNode.x, transform.position.y, currentNode.z);
            var maxDistance = _speed * Time.deltaTime;
            var vectorToDestination = nextNodeLocation - transform.position;
            var moveDistance = Mathf.Min(vectorToDestination.magnitude, maxDistance);

            var moveVector = vectorToDestination.normalized * moveDistance;
            moveVector.y = 0f; // Ignore Y
            transform.position += moveVector;

            if (transform.position == nextNodeLocation)
                _currentPathIndex++;
        }
    }

    void OnFinishSearchPath(NavLocation[] navLocation)
    {
        _currentPath = navLocation;
        _serachingPath = false;        
    }
}
