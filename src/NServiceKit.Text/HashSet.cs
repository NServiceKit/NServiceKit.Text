//
// https://github.com/NServiceKit/NServiceKit.Text
// NServiceKit.Text: .NET C# POCO JSON, JSV and CSV Text Serializers.
//
// Authors:
//   Demis Bellot (demis.bellot@gmail.com)
//	 Mijail Cisneros (cisneros@mijail.ru)
//
// Copyright 2012 Liquidbit Ltd.
//
// Licensed under the same terms of ServiceStack: new BSD license.
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NServiceKit.Text.WP
{
    /// <summary>A hashset implementation that uses an IDictionary.</summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    public class HashSet<T> : ICollection<T>, IEnumerable<T>, IEnumerable
    {
        /// <summary>The dictionary.</summary>
        private readonly Dictionary<T, short> _dict;

        /// <summary>
        /// Initializes a new instance of the NServiceKit.Text.WP.HashSet&lt;T&gt; class.
        /// </summary>
        public HashSet()
        {
            _dict = new Dictionary<T, short>();
        }

        /// <summary>
        /// Initializes a new instance of the NServiceKit.Text.WP.HashSet&lt;T&gt; class.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        /// <param name="collection">The collection.</param>
        public HashSet(IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            _dict = new Dictionary<T, short>(collection.Count());
            foreach (T item in collection)
                Add(item);
        }

        /// <summary>Adds item.</summary>
        /// <param name="item">The item to add.</param>
        public void Add(T item)
        {
            _dict.Add(item, 0);
        }

        /// <summary>Clears this object to its blank/initial state.</summary>
        public void Clear()
        {
            _dict.Clear();
        }

        /// <summary>Query if this object contains the given item.</summary>
        /// <param name="item">The T to test for containment.</param>
        /// <returns>true if the object is in this collection, false if not.</returns>
        public bool Contains(T item)
        {
            return _dict.ContainsKey(item);
        }

        /// <summary>Copies to.</summary>
        /// <param name="array">     The array.</param>
        /// <param name="arrayIndex">Zero-based index of the array.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            _dict.Keys.CopyTo(array, arrayIndex);
        }

        /// <summary>Removes the given item.</summary>
        /// <param name="item">The item to remove.</param>
        /// <returns>true if it succeeds, false if it fails.</returns>
        public bool Remove(T item)
        {
            return _dict.Remove(item);
        }

        /// <summary>Gets the enumerator.</summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _dict.Keys.GetEnumerator();
        }

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through
        /// the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _dict.Keys.GetEnumerator();
        }

        /// <summary>Gets the number of. </summary>
        /// <value>The count.</value>
        public int Count
        {
            get { return _dict.Keys.Count(); }
        }

        /// <summary>Gets a value indicating whether this object is read only.</summary>
        /// <value>true if this object is read only, false if not.</value>
        public bool IsReadOnly
        {
            get { return false; }
        }
    }
}

namespace NServiceKit.Text.WinRT
{
    /// <summary>A hashset implementation that uses an IDictionary.</summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    public class HashSet<T> : ICollection<T>, IEnumerable<T>, IEnumerable
    {
        /// <summary>The dictionary.</summary>
        private readonly Dictionary<T, short> _dict;

        /// <summary>
        /// Initializes a new instance of the NServiceKit.Text.WinRT.HashSet&lt;T&gt; class.
        /// </summary>
        public HashSet()
        {
            _dict = new Dictionary<T, short>();
        }

        /// <summary>
        /// Initializes a new instance of the NServiceKit.Text.WinRT.HashSet&lt;T&gt; class.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        /// <param name="collection">The collection.</param>
        public HashSet(IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            _dict = new Dictionary<T, short>(collection.Count());
            foreach (T item in collection)
                Add(item);
        }

        /// <summary>Adds item.</summary>
        /// <param name="item">The item to add.</param>
        public void Add(T item)
        {
            _dict.Add(item, 0);
        }

        /// <summary>Clears this object to its blank/initial state.</summary>
        public void Clear()
        {
            _dict.Clear();
        }

        /// <summary>Query if this object contains the given item.</summary>
        /// <param name="item">The T to test for containment.</param>
        /// <returns>true if the object is in this collection, false if not.</returns>
        public bool Contains(T item)
        {
            return _dict.ContainsKey(item);
        }

        /// <summary>Copies to.</summary>
        /// <param name="array">     The array.</param>
        /// <param name="arrayIndex">Zero-based index of the array.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            _dict.Keys.CopyTo(array, arrayIndex);
        }

        /// <summary>Removes the given item.</summary>
        /// <param name="item">The item to remove.</param>
        /// <returns>true if it succeeds, false if it fails.</returns>
        public bool Remove(T item)
        {
            return _dict.Remove(item);
        }

        /// <summary>Gets the enumerator.</summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _dict.Keys.GetEnumerator();
        }

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through
        /// the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _dict.Keys.GetEnumerator();
        }

        /// <summary>Gets the number of. </summary>
        /// <value>The count.</value>
        public int Count
        {
            get { return _dict.Keys.Count(); }
        }

        /// <summary>Gets a value indicating whether this object is read only.</summary>
        /// <value>true if this object is read only, false if not.</value>
        public bool IsReadOnly
        {
            get { return false; }
        }
    }
}