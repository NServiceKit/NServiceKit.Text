﻿using System.Threading;

//Not using it here, but @marcgravell's stuff is too good not to include
namespace NServiceKit.Text.Marc
{
    /// <summary>
    /// Pretty Thread-Safe cache class from: http://code.google.com/p/dapper-dot-
    /// net/source/browse/Dapper/SqlMapper.cs
    /// 
    /// This is a micro-cache; suitable when the number of terms is controllable (a few hundred, for
    /// example), and strictly append-only; you cannot change existing values. All key matches are on
    /// **REFERENCE** equality. The type is fully thread-safe.
    /// </summary>
    /// <typeparam name="TKey">  Type of the key.</typeparam>
    /// <typeparam name="TValue">Type of the value.</typeparam>
	class Link<TKey, TValue> where TKey : class
	{
        /// <summary>Attempts to get from the given data.</summary>
        /// <param name="link"> The link.</param>
        /// <param name="key">  The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>true if it succeeds, false if it fails.</returns>
		public static bool TryGet(Link<TKey, TValue> link, TKey key, out TValue value)
		{
			while (link != null)
			{
				if ((object)key == (object)link.Key)
				{
					value = link.Value;
					return true;
				}
				link = link.Tail;
			}
			value = default(TValue);
			return false;
		}

        /// <summary>Attempts to add from the given data.</summary>
        /// <param name="head"> The head.</param>
        /// <param name="key">  The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>true if it succeeds, false if it fails.</returns>
		public static bool TryAdd(ref Link<TKey, TValue> head, TKey key, ref TValue value)
		{
			bool tryAgain;
			do
			{
				var snapshot = Interlocked.CompareExchange(ref head, null, null);
				TValue found;
				if (TryGet(snapshot, key, out found))
				{ // existing match; report the existing value instead
					value = found;
					return false;
				}
				var newNode = new Link<TKey, TValue>(key, value, snapshot);
				// did somebody move our cheese?
				tryAgain = Interlocked.CompareExchange(ref head, newNode, snapshot) != snapshot;
			} while (tryAgain);
			return true;
		}

        /// <summary>
        /// Initializes a new instance of the NServiceKit.Text.Marc.Link&lt;TKey, TValue&gt; class.
        /// </summary>
        /// <param name="key">  The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="tail"> The tail.</param>
		private Link(TKey key, TValue value, Link<TKey, TValue> tail)
		{
			Key = key;
			Value = value;
			Tail = tail;
		}

        /// <summary>Gets the key.</summary>
        /// <value>The key.</value>
		public TKey Key { get; private set; }

        /// <summary>Gets the value.</summary>
        /// <value>The value.</value>
		public TValue Value { get; private set; }

        /// <summary>Gets the tail.</summary>
        /// <value>The tail.</value>
		public Link<TKey, TValue> Tail { get; private set; }
	}
}