
using System.Collections;

using UnityEngine;

public class Unit : MonoBehaviour {
    [SerializeField] private Transform target;
    [SerializeField] private float moveSpeed = 5f;

    private Vector3[] _path;
    private int _targetIndex;

    void Start() {
        PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
    }

    private void OnPathFound(Vector3[] newPath, bool success) {
        if (success) {
            _path = newPath;
            StopCoroutine(nameof(FollowPath));
            StartCoroutine(nameof(FollowPath));
        }
    }

    private IEnumerator FollowPath() {
        var waypoint = _path[0];
        while (true) {
            if (transform.position == waypoint) {
                _targetIndex++;
                if (_targetIndex >= _path.Length) {
                    yield break;
                }
                waypoint = _path[_targetIndex];
            }
            transform.position = Vector3.MoveTowards(transform.position, waypoint, moveSpeed * Time.deltaTime);
           
            yield return null;
        }
    }

    private void OnDrawGizmos() {
        if (_path != null) {
            for (int i = _targetIndex; i < _path.Length; i++) {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(_path[i], 0.1f);
                if (i == _targetIndex) {
                    Gizmos.DrawLine(transform.position, _path[i]);
                }
                else {
                    Gizmos.DrawLine(_path[i-1], _path[i]);
                }
            }
        }
    }
}