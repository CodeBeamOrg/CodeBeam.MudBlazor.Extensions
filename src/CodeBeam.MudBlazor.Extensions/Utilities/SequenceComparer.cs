namespace MudExtensions.Utilities
{
    internal class SequenceComparer<T> : IEqualityComparer<IEnumerable<T?>?>
    {
        private readonly IEqualityComparer<T?> _elementComparer;

        /// <summary>
        /// Initializes a new instance of the <see cref="SequenceComparer{T}"/> class.
        /// </summary>
        /// <param name="elementComparer">The comparer to use for individual elements. If null, uses the default comparer.</param>
        public SequenceComparer(IEqualityComparer<T?>? elementComparer = null)
        {
            _elementComparer = elementComparer ?? EqualityComparer<T?>.Default;
        }

        /// <summary>
        /// Determines whether two sequences are equal by comparing each element.
        /// </summary>
        /// <param name="x">The first sequence to compare.</param>
        /// <param name="y">The second sequence to compare.</param>
        /// <returns>True if the sequences are equal; otherwise, false.</returns>
        public bool Equals(IEnumerable<T?>? x, IEnumerable<T?>? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null || y is null) return false;

            using var e1 = x.GetEnumerator();
            using var e2 = y.GetEnumerator();

            while (true)
            {
                var m1 = e1.MoveNext();
                var m2 = e2.MoveNext();
                if (!m1 || !m2) return m1 == m2;
                if (!_elementComparer.Equals(e1.Current, e2.Current)) return false;
            }
        }

        /// <summary>
        /// Returns a hash code for the specified sequence.
        /// </summary>
        /// <param name="obj">The sequence for which to get a hash code.</param>
        /// <returns>A hash code for the sequence.</returns>
        public int GetHashCode(IEnumerable<T?>? obj)
        {
            if (obj is null) return 0;

            unchecked
            {
                var hash = 17;
                foreach (var item in obj)
                {
                    hash = hash * 31 + (item is null ? 0 : _elementComparer.GetHashCode(item));
                }

                return hash;
            }
        }
    }
}
