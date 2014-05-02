//
// https://github.com/NServiceKit/NServiceKit.Text
// NServiceKit.Text: .NET C# POCO JSON, JSV and CSV Text Serializers.
//
// Authors:
//   Demis Bellot (demis.bellot@gmail.com)
//
// Copyright 2012 ServiceStack Ltd.
//
// Licensed under the same terms of ServiceStack: new BSD license.
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Linq;
using NServiceKit.Text.Common;

namespace NServiceKit.Text
{
    /// <summary>A translate list with elements.</summary>
	public static class TranslateListWithElements
	{
        /// <summary>The translate i collection cache.</summary>
        private static Dictionary<Type, ConvertInstanceDelegate> TranslateICollectionCache
            = new Dictionary<Type, ConvertInstanceDelegate>();

        /// <summary>Translate to generic i collection cache.</summary>
        /// <param name="from">            Source for the.</param>
        /// <param name="toInstanceOfType">Type of to instance of.</param>
        /// <param name="elementType">     Type of the element.</param>
        /// <returns>An object.</returns>
		public static object TranslateToGenericICollectionCache(object from, Type toInstanceOfType, Type elementType)
		{
            ConvertInstanceDelegate translateToFn;
            if (TranslateICollectionCache.TryGetValue(toInstanceOfType, out translateToFn))
                return translateToFn(from, toInstanceOfType);

            var genericType = typeof(TranslateListWithElements<>).MakeGenericType(elementType);
            var mi = genericType.GetPublicStaticMethod("LateBoundTranslateToGenericICollection");
            translateToFn = (ConvertInstanceDelegate)mi.MakeDelegate(typeof(ConvertInstanceDelegate));

            Dictionary<Type, ConvertInstanceDelegate> snapshot, newCache;
            do
            {
                snapshot = TranslateICollectionCache;
                newCache = new Dictionary<Type, ConvertInstanceDelegate>(TranslateICollectionCache);
                newCache[elementType] = translateToFn;

            } while (!ReferenceEquals(
                Interlocked.CompareExchange(ref TranslateICollectionCache, newCache, snapshot), snapshot));

			return translateToFn(from, toInstanceOfType);
		}

        /// <summary>The translate convertible i collection cache.</summary>
        private static Dictionary<ConvertibleTypeKey, ConvertInstanceDelegate> TranslateConvertibleICollectionCache
            = new Dictionary<ConvertibleTypeKey, ConvertInstanceDelegate>();

        /// <summary>Translate to convertible generic i collection cache.</summary>
        /// <param name="from">            Source for the.</param>
        /// <param name="toInstanceOfType">Type of to instance of.</param>
        /// <param name="fromElementType"> Type of from element.</param>
        /// <returns>An object.</returns>
		public static object TranslateToConvertibleGenericICollectionCache(
			object from, Type toInstanceOfType, Type fromElementType)
		{
			var typeKey = new ConvertibleTypeKey(toInstanceOfType, fromElementType);
            ConvertInstanceDelegate translateToFn;
            if (TranslateConvertibleICollectionCache.TryGetValue(typeKey, out translateToFn)) return translateToFn(from, toInstanceOfType);

            var toElementType = toInstanceOfType.GetGenericType().GenericTypeArguments()[0];
            var genericType = typeof(TranslateListWithConvertibleElements<,>).MakeGenericType(fromElementType, toElementType);
            var mi = genericType.GetPublicStaticMethod("LateBoundTranslateToGenericICollection");
            translateToFn = (ConvertInstanceDelegate)mi.MakeDelegate(typeof(ConvertInstanceDelegate));

            Dictionary<ConvertibleTypeKey, ConvertInstanceDelegate> snapshot, newCache;
            do
            {
                snapshot = TranslateConvertibleICollectionCache;
                newCache = new Dictionary<ConvertibleTypeKey, ConvertInstanceDelegate>(TranslateConvertibleICollectionCache);
                newCache[typeKey] = translateToFn;

            } while (!ReferenceEquals(
                Interlocked.CompareExchange(ref TranslateConvertibleICollectionCache, newCache, snapshot), snapshot));
            
            return translateToFn(from, toInstanceOfType);
		}

        /// <summary>Try translate to generic i collection.</summary>
        /// <param name="fromPropertyType">Type of from property.</param>
        /// <param name="toPropertyType">  Type of to property.</param>
        /// <param name="fromValue">       from value.</param>
        /// <returns>An object.</returns>
		public static object TryTranslateToGenericICollection(Type fromPropertyType, Type toPropertyType, object fromValue)
		{
			var args = typeof(ICollection<>).GetGenericArgumentsIfBothHaveSameGenericDefinitionTypeAndArguments(
				fromPropertyType, toPropertyType);

			if (args != null)
			{
				return TranslateToGenericICollectionCache(
					fromValue, toPropertyType, args[0]);
			}

			var varArgs = typeof(ICollection<>).GetGenericArgumentsIfBothHaveConvertibleGenericDefinitionTypeAndArguments(
			fromPropertyType, toPropertyType);

			if (varArgs != null)
			{
				return TranslateToConvertibleGenericICollectionCache(
					fromValue, toPropertyType, varArgs.Args1[0]);
			}

			return null;
		}

	}

    /// <summary>A convertible type key.</summary>
	public class ConvertibleTypeKey
	{
        /// <summary>Gets or sets the type of to instance.</summary>
        /// <value>The type of to instance.</value>
		public Type ToInstanceType { get; set; }

        /// <summary>Gets or sets the type of from elemenet.</summary>
        /// <value>The type of from elemenet.</value>
		public Type FromElemenetType { get; set; }

        /// <summary>
        /// Initializes a new instance of the NServiceKit.Text.ConvertibleTypeKey class.
        /// </summary>
		public ConvertibleTypeKey()
		{
		}

        /// <summary>
        /// Initializes a new instance of the NServiceKit.Text.ConvertibleTypeKey class.
        /// </summary>
        /// <param name="toInstanceType">  Type of to instance.</param>
        /// <param name="fromElemenetType">Type of from elemenet.</param>
		public ConvertibleTypeKey(Type toInstanceType, Type fromElemenetType)
		{
			ToInstanceType = toInstanceType;
			FromElemenetType = fromElemenetType;
		}

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object" /> is equal to the current
        /// <see cref="T:System.Object" />.
        /// </summary>
        /// <param name="other">The convertible type key to compare to this object.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object" /> is equal to the current
        /// <see cref="T:System.Object" />; otherwise, false.
        /// </returns>
		public bool Equals(ConvertibleTypeKey other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.ToInstanceType, ToInstanceType) && Equals(other.FromElemenetType, FromElemenetType);
		}

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object" /> is equal to the current
        /// <see cref="T:System.Object" />.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object" /> to compare with the current
        /// <see cref="T:System.Object" />.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object" /> is equal to the current
        /// <see cref="T:System.Object" />; otherwise, false.
        /// </returns>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(ConvertibleTypeKey)) return false;
			return Equals((ConvertibleTypeKey)obj);
		}

        /// <summary>Serves as a hash function for a particular type.</summary>
        /// <returns>A hash code for the current <see cref="T:System.Object" />.</returns>
		public override int GetHashCode()
		{
			unchecked
			{
				return ((ToInstanceType != null ? ToInstanceType.GetHashCode() : 0) * 397)
					^ (FromElemenetType != null ? FromElemenetType.GetHashCode() : 0);
			}
		}
	}

    /// <summary>A translate list with elements.</summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
	public class TranslateListWithElements<T>
	{
        /// <summary>Creates an instance.</summary>
        /// <param name="toInstanceOfType">Type of to instance of.</param>
        /// <returns>The new instance.</returns>
		public static object CreateInstance(Type toInstanceOfType)
		{
            if (toInstanceOfType.IsGeneric())
            {
				if (toInstanceOfType.HasAnyTypeDefinitionsOf(
					typeof(ICollection<>), typeof(IList<>)))
				{
					return ReflectionExtensions.CreateInstance(typeof(List<T>));
				}
			}

			return ReflectionExtensions.CreateInstance(toInstanceOfType);
		}

        /// <summary>Translate to i list.</summary>
        /// <param name="fromList">        List of froms.</param>
        /// <param name="toInstanceOfType">Type of to instance of.</param>
        /// <returns>An IList.</returns>
		public static IList TranslateToIList(IList fromList, Type toInstanceOfType)
		{
			var to = (IList)ReflectionExtensions.CreateInstance(toInstanceOfType);
			foreach (var item in fromList)
			{
				to.Add(item);
			}
			return to;
		}

        /// <summary>Late bound translate to generic i collection.</summary>
        /// <param name="fromList">        List of froms.</param>
        /// <param name="toInstanceOfType">Type of to instance of.</param>
        /// <returns>An object.</returns>
		public static object LateBoundTranslateToGenericICollection(
			object fromList, Type toInstanceOfType)
		{
			if (fromList == null) return null; //AOT

			return TranslateToGenericICollection(
				(ICollection<T>)fromList, toInstanceOfType);
		}

        /// <summary>Translate to generic i collection.</summary>
        /// <param name="fromList">        List of froms.</param>
        /// <param name="toInstanceOfType">Type of to instance of.</param>
        /// <returns>A list of.</returns>
		public static ICollection<T> TranslateToGenericICollection(
			ICollection<T> fromList, Type toInstanceOfType)
		{
			var to = (ICollection<T>)CreateInstance(toInstanceOfType);
			foreach (var item in fromList)
			{
				to.Add(item);
			}
			return to;
		}
	}

    /// <summary>A translate list with convertible elements.</summary>
    /// <typeparam name="TFrom">Type of from.</typeparam>
    /// <typeparam name="TTo">  Type of to.</typeparam>
	public class TranslateListWithConvertibleElements<TFrom, TTo>
	{
        /// <summary>The convert function.</summary>
		private static readonly Func<TFrom, TTo> ConvertFn;

        /// <summary>
        /// Initializes static members of the NServiceKit.Text.TranslateListWithConvertibleElements&lt;
        /// TFrom, TTo&gt; class.
        /// </summary>
		static TranslateListWithConvertibleElements()
		{
			ConvertFn = GetConvertFn();
		}

        /// <summary>Late bound translate to generic i collection.</summary>
        /// <param name="fromList">        List of froms.</param>
        /// <param name="toInstanceOfType">Type of to instance of.</param>
        /// <returns>An object.</returns>
		public static object LateBoundTranslateToGenericICollection(
			object fromList, Type toInstanceOfType)
		{
			return TranslateToGenericICollection(
				(ICollection<TFrom>)fromList, toInstanceOfType);
		}

        /// <summary>Translate to generic i collection.</summary>
        /// <param name="fromList">        List of froms.</param>
        /// <param name="toInstanceOfType">Type of to instance of.</param>
        /// <returns>A list of.</returns>
		public static ICollection<TTo> TranslateToGenericICollection(
			ICollection<TFrom> fromList, Type toInstanceOfType)
		{
			if (fromList == null) return null; //AOT

			var to = (ICollection<TTo>)TranslateListWithElements<TTo>.CreateInstance(toInstanceOfType);

			foreach (var item in fromList)
			{
				var toItem = ConvertFn(item);
				to.Add(toItem);
			}
			return to;
		}

        /// <summary>Gets convert function.</summary>
        /// <returns>The convert function.</returns>
		private static Func<TFrom, TTo> GetConvertFn()
		{
			if (typeof(TTo) == typeof(string))
			{
				return x => (TTo)(object)TypeSerializer.SerializeToString(x);
			}
			if (typeof(TFrom) == typeof(string))
			{
				return x => TypeSerializer.DeserializeFromString<TTo>((string)(object)x);
			}
			return x => TypeSerializer.DeserializeFromString<TTo>(TypeSerializer.SerializeToString(x));
		}
	}
}
