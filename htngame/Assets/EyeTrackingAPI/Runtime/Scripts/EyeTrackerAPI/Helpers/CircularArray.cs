// Copyright (c) AdHawk Microsystems Inc.
// All rights reserved.

namespace AdhawkApi
{
    public class CircularArray<T>
    {
        private T[] _data;
        private int _index;
        private bool _full;
        public CircularArray(int size)
        {
            _data = new T[size];
            _index = 0;
            _full = false;
        }

        public void Append(T item)
        {
            _data[_index] = item;
            _index = (_index + 1) % _data.Length;
            if (_index == 0)
            {
                // wrapped around
                _full = true;
            }
        }

        public T this[int i]
        {
            get {
                if (_full)
                {
                    return _data[Wrap(i + _index)];
                }
                else if (_index != 0)
                {
                    return _data[Wrap(i)];
                }
                else
                {
                    return default(T);
                }
            }
        }

        private int Wrap(int i)
        {
            return ((i % Length) + Length) % Length;
        }

        public int Length
        {
            get { return _full ? _data.Length : _index; }
        }
    }
}