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
            GetBounds(element, out var minX, out var minY, out var maxX, out var maxY);

            for (int x = minX; x < maxX; x++)
                for (int y = minY; y < maxY; y++)
                {
                    var key = GetKey(x, y);
                    if (!_parition.ContainsKey(key))
                        _parition[key] = new List<T>(8);

                    _parition[key].Add(element);
                }
        }

        public void Remove(in T element)
        {
            GetBounds(element, out var minX, out var minY, out var maxX, out var maxY);

            for (int x = minX; x < maxX; x++)
                for (int y = minY; y < maxY; y++)
                {
                    var key = GetKey(x, y);
                    if (_parition.TryGetValue(key, out var list))
                        Remove(list, element);
                }
        }
        void Remove(List<T> list, T element) => list.Remove(element);
        int GetKey(in int gridX, in int gridY) => gridX + 3 * gridY;
        void GetBounds(in T element, out int minX, out int minY, out int maxX, out int maxY)
        {
            minX = (int)Math.Floor(element.MinX / _gridSizeX);
            minY = (int)Math.Floor(element.MinY / _gridSizeY);
            maxX = (int)Math.Floor(element.MaxX / _gridSizeX);
            maxY = (int)Math.Floor(element.MaxY / _gridSizeY);
        }
    }
}
