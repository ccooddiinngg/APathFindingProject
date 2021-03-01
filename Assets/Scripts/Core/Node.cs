using UnityEngine;

namespace Core {
    public class Node : IHeapItem<Node> {
        public readonly bool Walkable;

        public Vector3 WorldPosition;
        public readonly int GridX;
        public readonly int GridY;
        public int MovementPenalty;

        public int GCost; //to neighbor node
        public int HCost; //to target
        private int FCost => GCost + HCost; //gCost + hCost
        public int HeapIndex { get; set; }

        public Node Parent;

        public Node(bool walkable, Vector3 worldPosition, int gridX, int gridY, int movementPenalty) {
            Walkable = walkable;
            WorldPosition = worldPosition;
            GridX = gridX;
            GridY = gridY;
            MovementPenalty = movementPenalty;
        }

        public int CompareTo(Node other) {
            int compare = FCost.CompareTo(other.FCost);
            if (compare == 0) {
                compare = HCost.CompareTo(other.HCost);
            }
            return -compare;
        }
    }
}