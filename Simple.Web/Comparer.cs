namespace Simple.Web
{
    using System;
    using System.Collections.Generic;

    sealed class Comparer<T> : IComparer<T>
    {
        private readonly Func<T, T, int> _compare; 
        public Comparer(Func<T, T, int> compare)
        {
            if (compare == null) throw new ArgumentNullException("compare");
            _compare = compare;
        }

        public int Compare(T x, T y)
        {
            return _compare(x, y);
        }
    }
}