using System.Collections.Generic;
using UnityEngine;

namespace Core {
    public class Grid : MonoBehaviour {
        [SerializeField] private LayerMask obstacle;
        [SerializeField] private Vector2 gridWorldSize;
        [SerializeField] private float nodeRadius;
        [SerializeField] private TerrainType[] walkableRegions;
        
        private Node[,] _grid;
        private float _nodeDiameter;
        private int _gridSizeX, _gridSizeY;
        private LayerMask _walkableMask;
        private Dictionary<int, int> _walkableRegionsDictionary = new Dictionary<int, int>();
        public int MaxSize => _gridSizeX * _gridSizeY;

        [SerializeField] private bool displayGridGizmos;

        private void Awake() {
            _nodeDiameter = nodeRadius * 2;
            _gridSizeX = Mathf.RoundToInt(gridWorldSize.x / _nodeDiameter);
            _gridSizeY = Mathf.RoundToInt(gridWorldSize.y / _nodeDiameter);
            //Add all terrain types to layer mask.For example: layer 9 is 10 0000 0000, layer 8 is 1 0000 0000, so use OR to add them together.
            foreach (var region in walkableRegions) {
                _walkableMask.value |= region.terrainMask.value;
                _walkableRegionsDictionary.Add((int) Mathf.Log(region.terrainMask.value, 2), region.terrainPenalty);
                
            }
            
            CreateGrid();
        }

        private void OnDrawGizmos() {
            Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 0.2f, gridWorldSize.y));

            if (_grid != null && displayGridGizmos) {
                foreach (var n in _grid) {
                    Gizmos.color = n.Walkable ? Color.white : Color.red;
                    Gizmos.DrawCube(n.WorldPosition, Vector3.one * (_nodeDiameter - 0.01f));
                }
            }
        }

        private void CreateGrid() {
            _grid = new Node[_gridSizeX, _gridSizeY];
            var worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 -
                                  Vector3.forward * gridWorldSize.y / 2;

            for (int x = 0; x < _gridSizeX; x++) {
                for (int y = 0; y < _gridSizeY; y++) {
                    var worldPoint = worldBottomLeft + Vector3.right * (x * _nodeDiameter + nodeRadius) +
                                     Vector3.forward * (y * _nodeDiameter + nodeRadius);
                    bool walkable = !Physics.CheckSphere(worldPoint, nodeRadius, obstacle);

                    int movementPenalty = 0;
                    //TODO raycast set movementPenalty
                    if (walkable) {
                        var ray = new Ray(worldPoint + Vector3.up * 50, Vector3.down);
                        if (Physics.Raycast(ray, out var hit, 100, _walkableMask)) {
                            _walkableRegionsDictionary.TryGetValue(hit.collider.gameObject.layer, out movementPenalty);
                        }
                    }
                    
                    _grid[x, y] = new Node(walkable, worldPoint, x, y, movementPenalty);
                }
            }
        }

        public Node NodeFromWorldPoint(Vector3 worldPosition) {
            float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
            float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
            percentX = Mathf.Clamp01(percentX);
            percentY = Mathf.Clamp01(percentY);
            int x = Mathf.RoundToInt((_gridSizeX - 1) * percentX);
            int y = Mathf.RoundToInt((_gridSizeY - 1) * percentY);

            return _grid[x, y];
        }

        public List<Node> GetNeighbours(Node node) {
            var neighbours = new List<Node>();
            for (int i = -1; i <= 1; i++) {
                for (int j = -1; j <= 1; j++) {
                    if (i == 0 && j == 0) continue;

                    int checkX = node.GridX + i;
                    int checkY = node.GridY + j;
                    if (checkX >= 0 && checkX < _gridSizeX && checkY >= 0 && checkY < _gridSizeY) {
                        neighbours.Add(_grid[checkX, checkY]);
                    }
                }
            }

            return neighbours;
        }

        [System.Serializable]
        public class TerrainType {
            public LayerMask terrainMask;
            public int terrainPenalty;
        }
    }
}