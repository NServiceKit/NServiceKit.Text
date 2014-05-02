using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;

namespace NServiceKit.Text.Common
{
    /// <summary>A deserialize specialized collections.</summary>
    /// <typeparam name="T">          Generic type parameter.</typeparam>
    /// <typeparam name="TSerializer">Type of the serializer.</typeparam>
    internal static class DeserializeSpecializedCollections<T, TSerializer>
        where TSerializer : ITypeSerializer
    {
        /// <summary>The cache function.</summary>
        private readonly static ParseStringDelegate CacheFn;

        /// <summary>
        /// Initializes static members of the
        /// NServiceKit.Text.Common.DeserializeSpecializedCollections&lt;T, TSerializer&gt; class.
        /// </summary>
        static DeserializeSpecializedCollections()
        {
            CacheFn = GetParseFn();
        }

        /// <summary>Gets the parse.</summary>
        /// <value>The parse.</value>
        public static ParseStringDelegate Parse
        {
            get { return CacheFn; }
        }

        /// <summary>Gets parse function.</summary>
        /// <returns>The parse function.</returns>
        public static ParseStringDelegate GetParseFn()
        {
            if (typeof(T).HasAnyTypeDefinitionsOf(typeof(Queue<>)))
            {
                if (typeof(T) == typeof(Queue<string>))
                    return ParseStringQueue;

                if (typeof(T) == typeof(Queue<int>))
                    return ParseIntQueue;

                return GetGenericQueueParseFn();
            }

            if (typeof(T).HasAnyTypeDefinitionsOf(typeof(Stack<>)))
            {
                if (typeof(T) == typeof(Stack<string>))
                    return ParseStringStack;

                if (typeof(T) == typeof(Stack<int>))
                    return ParseIntStack;

                return GetGenericStackParseFn();
            }

#if !SILVERLIGHT
            if (typeof(T) == typeof(StringCollection))
            {
                return ParseStringCollection<TSerializer>;
            }
#endif
            if (typeof (T) == typeof (IEnumerable) || typeof(T) == typeof(ICollection))
            {
                return GetEnumerableParseFn();
            }

            return GetGenericEnumerableParseFn();
        }

        /// <summary>Parse string queue.</summary>
        /// <param name="value">The value.</param>
        /// <returns>A Queue&lt;string&gt;</returns>
        public static Queue<string> ParseStringQueue(string value)
        {
            var parse = (IEnumerable<string>)DeserializeList<List<string>, TSerializer>.Parse(value);
            return new Queue<string>(parse);
        }

        /// <summary>Parse int queue.</summary>
        /// <param name="value">The value.</param>
        /// <returns>A Queue&lt;int&gt;</returns>
        public static Queue<int> ParseIntQueue(string value)
        {
            var parse = (IEnumerable<int>)DeserializeList<List<int>, TSerializer>.Parse(value);
            return new Queue<int>(parse);
        }

#if !SILVERLIGHT
        /// <summary>Parse string collection.</summary>
        /// <typeparam name="TS">Type of the ts.</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>A StringCollection.</returns>
        public static StringCollection ParseStringCollection<TS>(string value) where TS : ITypeSerializer
        {
            if ((value = DeserializeListWithElements<TS>.StripList(value)) == null) return null;
            return value == String.Empty
                   ? new StringCollection()
                   : ToStringCollection(DeserializeListWithElements<TSerializer>.ParseStringList(value));
        }

        /// <summary>Converts the items to a string collection.</summary>
        /// <param name="items">The items.</param>
        /// <returns>items as a StringCollection.</returns>
        public static StringCollection ToStringCollection(List<string> items)
        {
            var to = new StringCollection();
            foreach (var item in items)
            {
                to.Add(item);
            }
            return to;
        }
#endif

        /// <summary>Gets generic queue parse function.</summary>
        /// <returns>The generic queue parse function.</returns>
        internal static ParseStringDelegate GetGenericQueueParseFn()
        {
            var enumerableInterface = typeof(T).GetTypeWithGenericInterfaceOf(typeof(IEnumerable<>));
            var elementType = enumerableInterface.GenericTypeArguments()[0];
            var genericType = typeof(SpecializedQueueElements<>).MakeGenericType(elementType);
            var mi = genericType.GetPublicStaticMethod("ConvertToQueue");
            var convertToQueue = (ConvertObjectDelegate)mi.MakeDelegate(typeof(ConvertObjectDelegate));

            var parseFn = DeserializeEnumerable<T, TSerializer>.GetParseFn();

            return x => convertToQueue(parseFn(x));
        }

        /// <summary>Parse string stack.</summary>
        /// <param name="value">The value.</param>
        /// <returns>A Stack&lt;string&gt;</returns>
        public static Stack<string> ParseStringStack(string value)
        {
            var parse = (IEnumerable<string>)DeserializeList<List<string>, TSerializer>.Parse(value);
            return new Stack<string>(parse);
        }

        /// <summary>Parse int stack.</summary>
        /// <param name="value">The value.</param>
        /// <returns>A Stack&lt;int&gt;</returns>
        public static Stack<int> ParseIntStack(string value)
        {
            var parse = (IEnumerable<int>)DeserializeList<List<int>, TSerializer>.Parse(value);
            return new Stack<int>(parse);
        }

        /// <summary>Gets generic stack parse function.</summary>
        /// <returns>The generic stack parse function.</returns>
        internal static ParseStringDelegate GetGenericStackParseFn()
        {
            var enumerableInterface = typeof(T).GetTypeWithGenericInterfaceOf(typeof(IEnumerable<>));
            
            var elementType = enumerableInterface.GenericTypeArguments()[0];
            var genericType = typeof(SpecializedQueueElements<>).MakeGenericType(elementType);
            var mi = genericType.GetPublicStaticMethod("ConvertToStack");
            var convertToQueue = (ConvertObjectDelegate)mi.MakeDelegate(typeof(ConvertObjectDelegate));

            var parseFn = DeserializeEnumerable<T, TSerializer>.GetParseFn();

            return x => convertToQueue(parseFn(x));
        }

        /// <summary>Gets enumerable parse function.</summary>
        /// <returns>The enumerable parse function.</returns>
        public static ParseStringDelegate GetEnumerableParseFn()
        {
            return DeserializeListWithElements<TSerializer>.ParseStringList;
        }

        /// <summary>Gets generic enumerable parse function.</summary>
        /// <returns>The generic enumerable parse function.</returns>
        public static ParseStringDelegate GetGenericEnumerableParseFn()
        {
            var enumerableInterface = typeof(T).GetTypeWithGenericInterfaceOf(typeof(IEnumerable<>));
            if (enumerableInterface == null) return null; 
            var elementType = enumerableInterface.GenericTypeArguments()[0];
            var genericType = typeof(SpecializedEnumerableElements<,>).MakeGenericType(typeof(T), elementType);
            var fi = genericType.GetPublicStaticField("ConvertFn");

            var convertFn = fi.GetValue(null) as ConvertObjectDelegate;
            if (convertFn == null) return null;

            var parseFn = DeserializeEnumerable<T, TSerializer>.GetParseFn();

            return x => convertFn(parseFn(x));
        }
    }

    /// <summary>A specialized queue elements.</summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    internal class SpecializedQueueElements<T>
    {
        /// <summary>Converts an enumerable to a queue.</summary>
        /// <param name="enumerable">The enumerable.</param>
        /// <returns>The given data converted to a queue.</returns>
        public static Queue<T> ConvertToQueue(object enumerable)
        {
            if (enumerable == null) return null;
            return new Queue<T>((IEnumerable<T>)enumerable);
        }

        /// <summary>Converts an enumerable to a stack.</summary>
        /// <param name="enumerable">The enumerable.</param>
        /// <returns>The given data converted to a stack.</returns>
        public static Stack<T> ConvertToStack(object enumerable)
        {
            if (enumerable == null) return null;
            return new Stack<T>((IEnumerable<T>)enumerable);
        }
    }

    /// <summary>A specialized enumerable elements.</summary>
    /// <typeparam name="TCollection">Type of the collection.</typeparam>
    /// <typeparam name="T">          Generic type parameter.</typeparam>
    internal class SpecializedEnumerableElements<TCollection, T>
    {
        /// <summary>The convert function.</summary>
        public static ConvertObjectDelegate ConvertFn;

        /// <summary>
        /// Initializes static members of the NServiceKit.Text.Common.SpecializedEnumerableElements&lt;
        /// TCollection, T&gt; class.
        /// </summary>
        static SpecializedEnumerableElements()
        {
            foreach (var ctorInfo in typeof(TCollection).DeclaredConstructors())
            {
                var ctorParams = ctorInfo.GetParameters();
                if (ctorParams.Length != 1) continue;
                var ctorParam = ctorParams[0];

                if (typeof(IEnumerable).AssignableFrom(ctorParam.ParameterType)
                    || ctorParam.ParameterType.IsOrHasGenericInterfaceTypeOf(typeof(IEnumerable<>)))
                {
                    ConvertFn = fromObject =>
                    {
                        var to = Activator.CreateInstance(typeof(TCollection), fromObject);
                        return to;
                    };
                    return;
                }
            }

            if (typeof(TCollection).IsOrHasGenericInterfaceTypeOf(typeof(ICollection<>)))
            {
                ConvertFn = ConvertFromCollection;
            }
        }

        /// <summary>Converts the given enumerable.</summary>
        /// <param name="enumerable">The enumerable.</param>
        /// <returns>An object.</returns>
        public static object Convert(object enumerable)
        {
            return ConvertFn(enumerable);
        }

        /// <summary>Initializes this object from the given convert from collection.</summary>
        /// <param name="enumerable">The enumerable.</param>
        /// <returns>from converted collection.</returns>
        public static object ConvertFromCollection(object enumerable)
        {
            var to = (ICollection<T>)typeof(TCollection).CreateInstance();
            var from = (IEnumerable<T>)enumerable;
            foreach (var item in from)
            {
                to.Add(item);
            }
            return to;
        }
    }
}
