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
using System.Reflection;

#if !XBOX
using System.Linq.Expressions;
#endif
namespace NServiceKit.Text.Reflection
{
    /// <summary>A static accessors.</summary>
    public static class StaticAccessors
    {
        /// <summary>A PropertyInfo extension method that gets value getter.</summary>
        /// <param name="propertyInfo">The propertyInfo to act on.</param>
        /// <param name="type">        The type.</param>
        /// <returns>The value getter.</returns>
        public static Func<object, object> GetValueGetter(this PropertyInfo propertyInfo, Type type)
        {
#if NETFX_CORE
			var getMethodInfo = propertyInfo.GetMethod;
			if (getMethodInfo == null) return null;
			return x => getMethodInfo.Invoke(x, new object[0]);
#elif (SILVERLIGHT && !WINDOWS_PHONE) || MONOTOUCH || XBOX
			var getMethodInfo = propertyInfo.GetGetMethod();
			if (getMethodInfo == null) return null;
			return x => getMethodInfo.Invoke(x, new object[0]);
#else

            var instance = Expression.Parameter(typeof(object), "i");
            var convertInstance = Expression.TypeAs(instance, propertyInfo.DeclaringType);
            var property = Expression.Property(convertInstance, propertyInfo);
            var convertProperty = Expression.TypeAs(property, typeof(object));
            return Expression.Lambda<Func<object, object>>(convertProperty, instance).Compile();
#endif
        }

        /// <summary>A FieldInfo extension method that gets value getter.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="propertyInfo">The propertyInfo to act on.</param>
        /// <returns>The value getter.</returns>
        public static Func<T, object> GetValueGetter<T>(this PropertyInfo propertyInfo)
        {
#if NETFX_CORE
			var getMethodInfo = propertyInfo.GetMethod;
            if (getMethodInfo == null) return null;
			return x => getMethodInfo.Invoke(x, new object[0]);
#elif (SILVERLIGHT && !WINDOWS_PHONE) || MONOTOUCH || XBOX
			var getMethodInfo = propertyInfo.GetGetMethod();
			if (getMethodInfo == null) return null;
			return x => getMethodInfo.Invoke(x, new object[0]);
#else
            var instance = Expression.Parameter(propertyInfo.DeclaringType, "i");
            var property = Expression.Property(instance, propertyInfo);
            var convert = Expression.TypeAs(property, typeof(object));
            return Expression.Lambda<Func<T, object>>(convert, instance).Compile();
#endif
        }

        /// <summary>A FieldInfo extension method that gets value getter.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="fieldInfo">The fieldInfo to act on.</param>
        /// <returns>The value getter.</returns>
        public static Func<T, object> GetValueGetter<T>(this FieldInfo fieldInfo)
        {
#if (SILVERLIGHT && !WINDOWS_PHONE) || MONOTOUCH || XBOX
            return x => fieldInfo.GetValue(x);
#else
            var instance = Expression.Parameter(fieldInfo.DeclaringType, "i");
            var property = Expression.Field(instance, fieldInfo);
            var convert = Expression.TypeAs(property, typeof(object));
            return Expression.Lambda<Func<T, object>>(convert, instance).Compile();
#endif
        }

#if !XBOX
        /// <summary>A PropertyInfo extension method that gets value setter.</summary>
        /// <exception cref="ArgumentException">Thrown when one or more arguments have unsupported or
        /// illegal values.</exception>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="propertyInfo">The propertyInfo to act on.</param>
        /// <returns>The value setter.</returns>
        public static Action<T, object> GetValueSetter<T>(this PropertyInfo propertyInfo)
        {
            if (typeof(T) != propertyInfo.DeclaringType)
            {
                throw new ArgumentException();
            }

            var instance = Expression.Parameter(propertyInfo.DeclaringType, "i");
            var argument = Expression.Parameter(typeof(object), "a");
            var setterCall = Expression.Call(
                instance,
                propertyInfo.SetMethod(),
                Expression.Convert(argument, propertyInfo.PropertyType));

            return Expression.Lambda<Action<T, object>>
            (
                setterCall, instance, argument
            ).Compile();
        }
#endif

    }
}

