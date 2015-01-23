using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace FullInspector.Internal {
    public static class fiUtility {
        public static string CombinePaths(string a, string b) {
            return Path.Combine(a, b).Replace('\\', '/');
        }
        public static string CombinePaths(string a, string b, string c) {
            return Path.Combine(Path.Combine(a, b), c).Replace('\\', '/');
        }
        public static string CombinePaths(string a, string b, string c, string d) {
            return Path.Combine(Path.Combine(Path.Combine(a, b), c), d).Replace('\\', '/');
        }

        /// <summary>
        /// Creates a dictionary from the given keys and given values.
        /// </summary>
        /// <typeparam name="TKey">The key type of the dictionary.</typeparam>
        /// <typeparam name="TValue">The value type of the dictionary.</typeparam>
        /// <param name="keys">The keys in the dictionary. A null key will be skipped.</param>
        /// <param name="values">The values in the dictionary.</param>
        /// <returns>A dictionary that contains the given key to value mappings.</returns>
        public static Dictionary<TKey, TValue> CreateDictionary<TKey, TValue>(TKey[] keys, TValue[] values) {
            var dict = new Dictionary<TKey, TValue>();

            if (keys != null && values != null) {
                for (int i = 0; i < Mathf.Min(keys.Length, values.Length); ++i) {
                    if (ReferenceEquals(keys[i], null)) continue;

                    dict[keys[i]] = values[i];
                }
            }

            return dict;
        }

        /// <summary>
        /// Swaps two items.
        /// </summary>
        public static void Swap<T>(ref T a, ref T b) {
            T tmp = a;
            a = b;
            b = tmp;
        }
    }
}