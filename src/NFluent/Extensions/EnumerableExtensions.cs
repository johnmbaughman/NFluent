﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableExtensions.cs" company="">
//   Copyright 2013 Thomas PIERRAIN
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//       http://www.apache.org/licenses/LICENSE-2.0
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace NFluent
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using Extensions;
#if !DOTNET_30 && !DOTNET_20
    using System.Linq;
#endif

    /// <summary>
    ///     Extension methods for adding new fluent methods to enumerable.
    /// </summary>
    public static class EnumerableExtensions
    {
        private const string Ellipsis = "...";

        /// <summary>
        ///     Returns the number of items present within the specified enumerable (returns 0 if the enumerable is null).
        /// </summary>
        /// <param name="enumerable">The enumerable.</param>
        /// <returns>The number of items present within the specified enumerable (returns 0 if the enumerable is null).</returns>
        public static long Count(this IEnumerable enumerable)
        {
            return enumerable?.Cast<object>().LongCount() ?? 0;
        }

        /// <summary>
        /// Returns the dimension of the array along one of its axes
        /// </summary>
        /// <param name="array">array</param>
        /// <param name="dimension">axes number</param>
        /// <returns>the size of the requested dimension</returns>
        public static int SizeOfDimension(this Array array, int dimension)
        {
            return array.GetUpperBound(dimension) - array.GetLowerBound(dimension) + 1;
        }

        /// <summary>
        ///     Return a string containing all the <see cref="IEnumerable" /> elements, separated by a comma.
        /// </summary>
        /// <param name="enumerable">The enumerable to transform into a string.</param>
        /// <returns>
        ///     A string containing all the <see cref="IEnumerable" /> elements, separated by a comma.
        /// </returns>
        public static string ToEnumeratedString(this IEnumerable enumerable)
        {
            return ToEnumeratedString(enumerable, 0, -1);
        }

        /// <summary>
        ///     Return a string containing all the elements of an <see cref="IEnumerable" />, separated by a given separator.
        /// </summary>
        /// <param name="enumerable"></param>
        /// <param name="fromIndex"></param>
        /// <param name="len"></param>
        /// <returns>A string containing all the <see cref="IEnumerable" /> elements, separated by the given separator.</returns>
        public static string ToEnumeratedString(this IEnumerable enumerable, long fromIndex, long len)
        {
            if (enumerable is Array)
            {
                return ((Array) enumerable).ArrayToStringProperlyFormatted(", ", fromIndex, len);
            }
            return enumerable.ToEnumeratedStringAdvanced(", ", fromIndex, len, new List<object>());
        }

        private static string ToEnumeratedStringAdvanced(this IEnumerable enumerable, string separator,
            long referenceIndex, long numberOfItems, ICollection<object> seen)
        {
            if (enumerable == null)
            {
                return "null";
            }

            var sb = new StringBuilder();
            sb.Append('{');
            if (seen.Contains(enumerable))
            {
                sb.Append('{');
                sb.Append(Ellipsis);
                sb.Append("}}");
                return sb.ToString();
            }
            var iterator = enumerable.GetEnumerator();
            var copy = new List<object>(seen) {enumerable};
            // we skip the first items
            var firstIndex = Math.Max(0, referenceIndex - (numberOfItems / 2));
            var lastItem = numberOfItems < 1 ? int.MaxValue : firstIndex + numberOfItems;
            if (firstIndex > 0)
            {
                for (var i = 0; i < firstIndex; i++)
                {
                    iterator.MoveNext();
                }
                sb.Append(Ellipsis);
            }

            // items to display
            for (var i = firstIndex; i < lastItem; i++)
            {
                if (!iterator.MoveNext())
                {
                    // end of enumeration
                    break;
                }

                if (i != 0)
                {
                    // add comma
                    sb.Append(separator);
                }

                var item = iterator.Current;
                switch (item)
                {
                    case string s:
                        sb.Append(s.ToStringProperlyFormatted());
                        break;
                    case IEnumerable sub:
                        sb.Append(sub.ToEnumeratedStringAdvanced(separator, referenceIndex, numberOfItems, copy));
                        break;
                    default:
                        sb.Append(item.ToStringProperlyFormatted());
                        break;
                }

                if (i == lastItem-1)
                {
                    sb.Append(Ellipsis);
                }
            }

            sb.Append('}');

            return sb.ToString();
        }

        private static string ArrayToStringProperlyFormatted(this Array array, string separator, long referenceIndex, long numberOfItems)
        {
            var result = new StringBuilder();
            var indices = new long[array.Rank];
            result.Append('{');
            
            var firstIndex = Math.Max(0, referenceIndex - (numberOfItems / 2));
            var lastItem = numberOfItems <= 0 ? array.Length-firstIndex : Math.Min(firstIndex + numberOfItems, array.LongLength()-firstIndex);
            if (firstIndex > 0)
            {
                result.Append(Ellipsis);
                result.Append(separator);
            }
            for (var i = firstIndex; i < lastItem; i++)
            {
                var temp = i;
                var zeroesStrike = true;
                var closingStrikes = true;
                var closing = string.Empty;
                for (var j = array.Rank-1; j >= 0; j--)
                {
                    var currentIndex = temp % array.SizeOfDimension(j);
                    if (currentIndex == 0 && zeroesStrike)
                    {
                        if (j > 0)
                        {
                            result.Append('{');
                        }
                    }
                    else
                    {
                        
                    }
                    {
                        zeroesStrike = false;
                    }

                    if (currentIndex == array.SizeOfDimension(j) - 1 && closingStrikes)
                    {
                        if (j > 0)
                        {
                            closing += '}';
                        }
                    }
                    else
                    {
                        closingStrikes = false;
                    }

                    indices[j] = currentIndex + array.GetLowerBound(j);
                    temp /= array.GetLongLength(j);
                }

                result.Append(array.GetValue(indices).ToStringProperlyFormatted());
                result.Append(closing);
                if (i != array.Length - 1)
                {
                    result.Append(separator);
                }
            }

            if (lastItem < array.Length)
            {
                result.Append(Ellipsis);
            }
            result.Append('}');
            return result.ToString();
        }

    }
}