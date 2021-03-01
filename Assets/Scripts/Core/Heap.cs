using System;

namespace Core {
    public class Heap<T> where T : IHeapItem<T> {
        private T[] _items;
        private int _currentItemCount;

        public Heap(int maxHeapSize) {
            _items = new T[maxHeapSize];
        }

        public bool Contains(T item) {
            return Equals(_items[item.HeapIndex], item);
        }

        public int Count => _currentItemCount;

        public void Add(T item) {
            item.HeapIndex = _currentItemCount;
            _items[_currentItemCount] = item;
            SortUp(item);
            _currentItemCount++;
        }

        public void UpdateItem(T item) {
            SortUp(item);
        }

        private void SortUp(T item) {
            int parentIndex = (item.HeapIndex - 1) / 2;
            while (true) {
                T parentItem = _items[parentIndex];
                if (item.CompareTo(parentItem) > 0) {
                    Swap(item, parentItem);
                }
                else {
                    break;
                }
            }
        }

        public T RemoveFirst() {
            T firstItem = _items[0];
            _currentItemCount--;
            _items[0] = _items[_currentItemCount];
            _items[0].HeapIndex = 0;
            SortDown(_items[0]);
            return firstItem;
        }

        private void SortDown(T item) {
            while (true) {
                int childIndexLeft = item.HeapIndex * 2 + 1;
                int childIndexRight = item.HeapIndex * 2 + 2;
                if (childIndexLeft < _currentItemCount) {
                    int swapIndex = childIndexLeft;
                    if (childIndexRight < _currentItemCount) {
                        if (_items[childIndexLeft].CompareTo(_items[childIndexRight]) < 0) {
                            swapIndex = childIndexRight;
                        }
                    }

                    if (item.CompareTo(_items[swapIndex]) < 0) {
                        Swap(item, _items[swapIndex]);
                    }
                    else {
                        break;
                    }
                }
                else {
                    break;
                }
            }
        }

        private void Swap(T a, T b) {
            _items[a.HeapIndex] = b;
            _items[b.HeapIndex] = a;
            int temp = a.HeapIndex;
            a.HeapIndex = b.HeapIndex;
            b.HeapIndex = temp;
        }
    }

    public interface IHeapItem<T> : IComparable<T> {
        int HeapIndex { get; set; }
    }
}