using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Core {
    public class PathFinding : MonoBehaviour {
        private Grid _grid;
        private PathRequestManager _pathRequestManager;

        private void Awake() {
            _grid = GetComponent<Grid>();
            _pathRequestManager = GetComponent<PathRequestManager>();
        }

        private IEnumerator FindPath(Vector3 startPos, Vector3 targetPos) {
            //debug timer
            var sw = new Stopwatch();
            sw.Start();

            bool pathSuccess = false;

            var startNode = _grid.NodeFromWorldPoint(startPos);
            var targetNode = _grid.NodeFromWorldPoint(targetPos);

            if (startNode.Walkable && targetNode.Walkable) {
                // var openSet = new List<ANode>();
                var openSet = new Heap<Node>(_grid.MaxSize);

                var closeSet = new HashSet<Node>();

                openSet.Add(startNode);

                while (openSet.Count > 0) {
                    /*
                 * using List deal with openSet
                 */
                    // var currentNode = openSet[0];
                    // foreach (var n in openSet.Where(n =>
                    //     n.FCost < currentNode.FCost || (n.FCost == currentNode.FCost && n.HCost < currentNode.HCost)))
                    // {
                    //     currentNode = n;
                    // }
                    //
                    // openSet.Remove(currentNode);

                    /*
                 * using AHeap
                 */
                    var currentNode = openSet.RemoveFirst();

                    closeSet.Add(currentNode);

                    if (currentNode == targetNode) {
                        pathSuccess = true;
                        sw.Stop();
                        print("Path found in " + sw.ElapsedMilliseconds + " ms");

                        break;
                    }

                    foreach (var neighbour in _grid.GetNeighbours(currentNode)) {
                        if (!neighbour.Walkable || closeSet.Contains(neighbour)) continue;

                        var newMovementCostToNeighbour = currentNode.GCost + GetDistance(currentNode, neighbour) + neighbour.MovementPenalty;
                        if (newMovementCostToNeighbour < neighbour.GCost || !openSet.Contains(neighbour)) {
                            neighbour.GCost = newMovementCostToNeighbour;
                            neighbour.HCost = GetDistance(neighbour, targetNode);
                            neighbour.Parent = currentNode;

                            if (!openSet.Contains(neighbour)) {
                                openSet.Add(neighbour);
                            }
                            else {
                                openSet.UpdateItem(neighbour);
                            }
                        }
                    }
                }
            }


            yield return null;

            if (pathSuccess) {
                //record the path
                var waypoints = RetracePath(startNode, targetNode);
                _pathRequestManager.FinishedProcessingPath(waypoints, true);
            }
        }

        private int GetDistance(Node a, Node b) {
            int x = Mathf.Abs(a.GridX - b.GridX);
            int y = Mathf.Abs(a.GridY - b.GridY);
            return x > y ? y * 14 + (x - y) * 10 : x * 14 + (y - x) * 10;
        }

        private Vector3[] RetracePath(Node start, Node end) {
            var path = new List<Node>();
            var current = end;
            while (current != start) {
                path.Add(current);
                current = current.Parent;
            }

            var waypoints = SimplifyPath(path);
            Array.Reverse(waypoints);
            return waypoints;
        }

        Vector3[] SimplifyPath(List<Node> path) {
            var waypoints = new List<Vector3>();
            var preDirection = Vector2.zero;
            for (int i = 1; i < path.Count; i++) {
                var currentDirection =
                    new Vector2(path[i - 1].GridX - path[i].GridX, path[i - 1].GridY - path[i].GridY);
                if (currentDirection != preDirection) {
                    waypoints.Add(path[i].WorldPosition);
                }

                preDirection = currentDirection;
            }

            return waypoints.ToArray();
        }

        public void StartFindPath(Vector3 startPath, Vector3 endPath) {
            StartCoroutine(FindPath(startPath, endPath));
        }
    }
}