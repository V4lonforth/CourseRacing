using System;
using System.Collections.Generic;

namespace Scripts.Utils.Extensions
{
    public static class ListExtensions
    {
        public static T MinValue<T>(this IList<T> self, Func<T, float> selector)
        {
            if (self == null) {
                throw new ArgumentNullException(nameof(self));
            }

            if (self.Count == 0) {
                throw new ArgumentException("List is empty.", nameof(self));
            }

            var min = selector(self[0]);
            var minIndex = 0;

            for (var i = 1; i < self.Count; ++i)
            {
                var value = selector(self[i]);
                if (value < min) {
                    min = value;
                    minIndex = i;
                }
            }

            return self[minIndex];
        }
    }
}