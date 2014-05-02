#if WINDOWS_PHONE && !WP8
using NServiceKit.Text.WP;
#endif
using System;
using System.Collections.Generic;

namespace NServiceKit.Text
{
    /// <summary>A collection extensions.</summary>
    public static class CollectionExtensions
    {
        /// <summary>Creates and populate.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="ofCollectionType">Type of the of collection.</param>
        /// <param name="withItems">       The with items.</param>
        /// <returns>The new and populate.</returns>
        public static ICollection<T> CreateAndPopulate<T>(Type ofCollectionType, T[] withItems)
        {
            if (ofCollectionType == null) return new List<T>(withItems);

            var genericType = ofCollectionType.GetGenericType();
            var genericTypeDefinition = genericType != null
                ? genericType.GetGenericTypeDefinition()
                : null;
#if !XBOX
            if (genericTypeDefinition == typeof(HashSet<T>))
                return new HashSet<T>(withItems);
#endif
            if (genericTypeDefinition == typeof(LinkedList<T>))
                return new LinkedList<T>(withItems);

            var collection = (ICollection<T>)ofCollectionType.CreateInstance();
            foreach (var item in withItems)
            {
                collection.Add(item);
            }
            return collection;
        }

        /// <summary>
        /// An ICollection&lt;T&gt; extension method that convert this object into an array
        /// representation.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="collection">The collection to act on.</param>
        /// <returns>collection as a T[].</returns>
        public static T[] ToArray<T>(this ICollection<T> collection)
        {
            var to = new T[collection.Count];
            collection.CopyTo(to, 0);
            return to;
        }

        /// <summary>Converts.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="objCollection">   Collection of objects.</param>
        /// <param name="toCollectionType">Type of to collection.</param>
        /// <returns>An object.</returns>
        public static object Convert<T>(object objCollection, Type toCollectionType)
        {
            var collection = (ICollection<T>) objCollection;
            var to = new T[collection.Count];
            collection.CopyTo(to, 0);
            return CreateAndPopulate(toCollectionType, to);
        }
    }
}