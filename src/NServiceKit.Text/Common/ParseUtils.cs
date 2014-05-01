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
using System.Collections.Generic;
using System.Reflection;

namespace NServiceKit.Text.Common
{
    /// <summary>A parse utilities.</summary>
    internal static class ParseUtils
    {
        /// <summary>Null value type.</summary>
        /// <param name="type">The type.</param>
        /// <returns>An object.</returns>
        public static object NullValueType(Type type)
        {
            return ReflectionExtensions.GetDefaultValue(type);
        }

        /// <summary>Parse object.</summary>
        /// <param name="value">The value.</param>
        /// <returns>An object.</returns>
        public static object ParseObject(string value)
        {
            return value;
        }

        /// <summary>Parse enum.</summary>
        /// <param name="type"> The type.</param>
        /// <param name="value">The value.</param>
        /// <returns>An object.</returns>
        public static object ParseEnum(Type type, string value)
        {
            return Enum.Parse(type, value, false);
        }

        /// <summary>Gets special parse method.</summary>
        /// <param name="type">The type.</param>
        /// <returns>The special parse method.</returns>
        public static ParseStringDelegate GetSpecialParseMethod(Type type)
        {
            if (type == typeof(Uri))
                return x => new Uri(x.FromCsvField());

            //Warning: typeof(object).IsInstanceOfType(typeof(Type)) == True??
            if (type.InstanceOfType(typeof(Type)))
                return ParseType;

            if (type == typeof(Exception))
                return x => new Exception(x);

            if (type.IsInstanceOf(typeof(Exception)))
                return DeserializeTypeUtils.GetParseMethod(type);

            return null;
        }

        /// <summary>Parse type.</summary>
        /// <param name="assemblyQualifiedName">Name of the assembly qualified.</param>
        /// <returns>A Type.</returns>
        public static Type ParseType(string assemblyQualifiedName)
        {
            return AssemblyUtils.FindType(assemblyQualifiedName.FromCsvField());
        }
    }

}