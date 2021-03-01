using System;
using System.Collections.Generic;
using Core;
using UnityEngine;

public class PathRequestManager : MonoBehaviour {
    private Queue<PathRequest> _pathRequestsQueue = new Queue<PathRequest>();
    private PathRequest _currentPathRequest;

    private static PathRequestManager _instance;
    private PathFinding _pathFinding;
    private bool _isProcessingPath;

    private void Awake() {
        _instance = this;
        _pathFinding = GetComponent<PathFinding>();
    }

    public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback) {
        var newRequest = new PathRequest(pathStart, pathEnd, callback);
        _instance._pathRequestsQueue.Enqueue(newRequest);
        _instance.TryProcessNext();
    }

    private void TryProcessNext() {
        if (!_isProcessingPath && _pathRequestsQueue.Count> 0) {
            _currentPathRequest = _pathRequestsQueue.Dequeue();
            _isProcessingPath = true;
            _pathFinding.StartFindPath(_currentPathRequest.StartPath, _currentPathRequest.EndPath);
        }
    }

    public void FinishedProcessingPath(Vector3[] path, bool success) {
        _currentPathRequest.CallBack(path, success);
        _isProcessingPath = false;
        TryProcessNext();
    }

    struct PathRequest {
        public Vector3 StartPath;
        public Vector3 EndPath;
        public Action<Vector3[], bool> CallBack;

        public PathRequest(Vector3 startPath, Vector3 endPath, Action<Vector3[], bool> callBack) {
            StartPath = startPath;
            EndPath = endPath;
            CallBack = callBack;
        }
    }
}