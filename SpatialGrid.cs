using System;
using System.Collections.Generic;

namespace Spatial
{
    public sealed class SpatialGrid<T> where T : IBounds
    {
        double _gridSizeX;
        double _gridSizeY;
        Dictionary<int, List<T>> _parition;

        public T this[int gridX, int gridY, int index] => _parition[GetKey(gridX, gridY)][index];
        public IReadOnlyList<T> this[int gridX, int gridY] => _parition[GetKey(gridX, gridY)];
        public SpatialGrid(in double gridSizeX, in double gridSizeY)
        {
            if (gridSizeX <= 0 || gridSizeY <= 0)
                throw new Exception("Provided grid sizes must be greater than zero! (" + gridSizeX + "," + gridSizeY + ")");

            _gridSizeX = gridSizeX;
            _gridSizeY = gridSizeY;
            _parition = new Dictionary<int, List<T>>(8);
        }

        public void Add(T element)
        {
            GetBounds(element, out var mX, out var mY, out var mxX, out var mxY);

            for (int x = mX; x <= mxX; x++)
                for (int y = mY; y <= mxY; y++)
                {
                    var key = GetKey(x, y);

                    if (!_parition.ContainsKey(key))
                        _parition[key] = new List<T>(8);

                    _parition[key].Add(element);
                }
        }
        public void Remove(in T element)
        {
            GetBounds(element, out var mX, out var mY, out var mxX, out var mxY);

            for (int x = mX; x < mxX + 1; x++)
                for (int y = mY; y < mxY + 1; y++)
                {
                    var key = GetKey(x, y);
                    if (_parition.TryGetValue(key, out var list))
                        list.Remove(element);
                }
        }


        /// <summary>
        /// Get all data at this world position
        /// </summary>
        public int GetElementsAtWorldPosition(double x, double y, out IReadOnlyList<T> data)
        {
            int X = (int)Math.Floor(x / _gridSizeX);
            int Y = (int)Math.Floor(y / _gridSizeY);
            var key = GetKey(X, Y);
            if (_parition.TryGetValue(key, out var d))
            {
                data = d;
                return data.Count;
            }
            data = default;
            return 0;
        }
        /// <summary>
        /// Get all data at this cell
        /// </summary>
        public int GetElementsAtCell(int cellX, int cellY, out IReadOnlyList<T> data)
        {
            var key = GetKey(cellX, cellY);
            if (_parition.TryGetValue(key, out var d))
            {
                data = d;
                return data.Count;
            }
            data = default;
            return 0;
        }

        /// <summary>
        /// Clear all data.
        /// </summary>
        public void Clear() => _parition.Clear();

        /// <summary>
        /// Delete all references to this element in the data structure
        /// [Warning] This is a slow method and should be avoided
        /// If the position of the element has changed and no longer matches the data in this spatial grid
        /// this method will make sure it is removed.
        /// </summary>
        /// <param name="element"></param>
        public void Delete(T element)
        {
            foreach (var kvp in _parition)
                kvp.Value.Remove(element);
        }
        public int GetKey(in int gridX, in int gridY)
        {
            unchecked
            {
                int hashcode = 3;
                hashcode = (hashcode * 7) + gridX;
                hashcode = (hashcode * 7) + gridY;
                return hashcode;
            }
        }
        void GetBounds(T element, out int mX, out int mY, out int mxX, out int mxY)
        {
            mX = (int)Math.Floor(element.MinX / _gridSizeX);
            mY = (int)Math.Floor(element.MinY / _gridSizeY);
            mxX = (int)Math.Floor(element.MaxX / _gridSizeX);
            mxY = (int)Math.Floor(element.MaxY / _gridSizeY);
        }
    }
}
