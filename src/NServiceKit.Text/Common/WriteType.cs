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
using System.IO;
using System.Linq;
using System.Reflection;
using NServiceKit.Text.Json;
using NServiceKit.Text.Reflection;

namespace NServiceKit.Text.Common
{
    /// <summary>A write type.</summary>
    /// <typeparam name="T">          Generic type parameter.</typeparam>
    /// <typeparam name="TSerializer">Type of the serializer.</typeparam>
    internal static class WriteType<T, TSerializer>
        where TSerializer : ITypeSerializer
    {
        /// <summary>Set the data member order not belongs to.</summary>
        private const int DataMemberOrderNotSet = -1;

        /// <summary>The serializer.</summary>
        private static readonly ITypeSerializer Serializer = JsWriter.GetTypeSerializer<TSerializer>();

        /// <summary>The cache function.</summary>
        private static readonly WriteObjectDelegate CacheFn;

        /// <summary>The property writers.</summary>
        internal static TypePropertyWriter[] PropertyWriters;

        /// <summary>Information describing the write type.</summary>
        private static readonly WriteObjectDelegate WriteTypeInfo;

        /// <summary>Gets a value indicating whether this object is included.</summary>
        /// <value>true if this object is included, false if not.</value>
        private static bool IsIncluded
        {
            get { return (JsConfig.IncludeTypeInfo || JsConfig<T>.IncludeTypeInfo); }
        }

        /// <summary>Gets a value indicating whether this object is excluded.</summary>
        /// <value>true if this object is excluded, false if not.</value>
        private static bool IsExcluded
        {
            get { return (JsConfig.ExcludeTypeInfo || JsConfig<T>.ExcludeTypeInfo); }
        }

        /// <summary>
        /// Initializes static members of the NServiceKit.Text.Common.WriteType&lt;T, TSerializer&gt;
        /// class.
        /// </summary>
        static WriteType()
        {
            if (typeof(T) == typeof(Object))
            {
                CacheFn = WriteObjectType;
            }
            else
            {
                CacheFn = Init() ? GetWriteFn() : WriteEmptyType;
            }

            if (IsIncluded)
            {
                WriteTypeInfo = TypeInfoWriter;
            }

            if (typeof(T).IsAbstract())
            {
                WriteTypeInfo = TypeInfoWriter;
                if (!JsConfig.PreferInterfaces || !typeof(T).IsInterface())
                {
                    CacheFn = WriteAbstractProperties;
                }
            }
        }

        /// <summary>Type information writer.</summary>
        /// <param name="writer">The writer.</param>
        /// <param name="obj">   The object.</param>
        public static void TypeInfoWriter(TextWriter writer, object obj)
        {
            TryWriteTypeInfo(writer, obj);
        }

        /// <summary>Determine if we should skip type.</summary>
        /// <returns>true if it succeeds, false if it fails.</returns>
        private static bool ShouldSkipType() { return IsExcluded && !IsIncluded; }

        /// <summary>Attempts to write self type from the given data.</summary>
        /// <param name="writer">The writer.</param>
        /// <returns>true if it succeeds, false if it fails.</returns>
        private static bool TryWriteSelfType(TextWriter writer)
        {
            if (ShouldSkipType()) return false;

            Serializer.WriteRawString(writer, JsConfig.TypeAttr);
            writer.Write(JsWriter.MapKeySeperator);
            Serializer.WriteRawString(writer, JsConfig.TypeWriter(typeof(T)));
            return true;
        }

        /// <summary>Attempts to write type information from the given data.</summary>
        /// <param name="writer">The writer.</param>
        /// <param name="obj">   The object.</param>
        /// <returns>true if it succeeds, false if it fails.</returns>
        private static bool TryWriteTypeInfo(TextWriter writer, object obj)
        {
            if (obj == null || ShouldSkipType()) return false;

            Serializer.WriteRawString(writer, JsConfig.TypeAttr);
            writer.Write(JsWriter.MapKeySeperator);
            Serializer.WriteRawString(writer, JsConfig.TypeWriter(obj.GetType()));
            return true;
        }

        /// <summary>Gets the write.</summary>
        /// <value>The write.</value>
        public static WriteObjectDelegate Write
        {
            get { return CacheFn; }
        }

        /// <summary>Gets write function.</summary>
        /// <returns>The write function.</returns>
        private static WriteObjectDelegate GetWriteFn()
        {
            return WriteProperties;
        }

        /// <summary>Gets should serialize method.</summary>
        /// <param name="member">The member.</param>
        /// <returns>The should serialize method.</returns>
        static Func<T, bool> GetShouldSerializeMethod(MemberInfo member)
        {
            var method = member.DeclaringType.GetMethod("ShouldSerialize" + member.Name, BindingFlags.Instance | BindingFlags.Public,
                null, Type.EmptyTypes, null);
            return (method == null || method.ReturnType != typeof(bool)) ? null : (Func<T,bool>)Delegate.CreateDelegate(typeof(Func<T,bool>), method);
        }

        /// <summary>Initialises this object.</summary>
        /// <returns>true if it succeeds, false if it fails.</returns>
        private static bool Init()
        {
            if (!typeof(T).IsClass() && !typeof(T).IsInterface() && !JsConfig.TreatAsRefType(typeof(T))) return false;

            var isDataContract = typeof(T).IsDto();
            var propertyInfos = TypeConfig<T>.Properties;
            var fieldInfos = JsConfig.IncludePublicFields || isDataContract ? TypeConfig<T>.Fields : new FieldInfo[0];
            var propertyNamesLength = propertyInfos.Length;
            var fieldNamesLength = fieldInfos.Length;
            PropertyWriters = new TypePropertyWriter[propertyNamesLength + fieldNamesLength];

            if (propertyNamesLength + fieldNamesLength == 0 && !JsState.IsWritingDynamic)
            {
                return typeof(T).IsDto();
            }

            // NOTE: very limited support for DataContractSerialization (DCS)
            //	NOT supporting Serializable
            //	support for DCS is intended for (re)Name of properties and Ignore by NOT having a DataMember present
            
            for (var i = 0; i < propertyNamesLength; i++)
            {
                var propertyInfo = propertyInfos[i];

                string propertyName, propertyNameCLSFriendly, propertyNameLowercaseUnderscore, propertyReflectedName;
                int propertyOrder = -1;
                var propertyType = propertyInfo.PropertyType;
                var defaultValue = propertyType.GetDefaultValue();
                bool propertySuppressDefaultConfig = defaultValue != null && propertyType.IsValueType() && JsConfig.HasSerializeFn.Contains(propertyType);
                bool propertySuppressDefaultAttribute = false;
                var shouldSerialize = GetShouldSerializeMethod(propertyInfo);
                if (isDataContract)
                {
                    var dcsDataMember = propertyInfo.GetDataMember();
                    if (dcsDataMember == null) continue;

                    propertyName = dcsDataMember.Name ?? propertyInfo.Name;
                    propertyNameCLSFriendly = dcsDataMember.Name ?? propertyName.ToCamelCase();
                    propertyNameLowercaseUnderscore = dcsDataMember.Name ?? propertyName.ToLowercaseUnderscore();
                    propertyReflectedName = dcsDataMember.Name ?? propertyInfo.ReflectedType.Name;

                    // Fields tend to be at topp, push down properties to make it more like common.
                    propertyOrder = dcsDataMember.Order == DataMemberOrderNotSet ? 0 : dcsDataMember.Order;
                    propertySuppressDefaultAttribute = !dcsDataMember.EmitDefaultValue;
                }
                else
                {
                    propertyName = propertyInfo.Name;
                    propertyNameCLSFriendly = propertyName.ToCamelCase();
                    propertyNameLowercaseUnderscore = propertyName.ToLowercaseUnderscore();
                    propertyReflectedName = propertyInfo.ReflectedType.Name;
                }


                PropertyWriters[i] = new TypePropertyWriter
                (
                    propertyName,
                    propertyReflectedName,
                    propertyNameCLSFriendly,
                    propertyNameLowercaseUnderscore,
                    propertyOrder,
                    propertySuppressDefaultConfig,
                    propertySuppressDefaultAttribute,
                    propertyInfo.GetValueGetter<T>(),
                    Serializer.GetWriteFn(propertyType),
                    propertyType.GetDefaultValue(),
                    shouldSerialize
                );
            }

            for (var i = 0; i < fieldNamesLength; i++)
            {
                var fieldInfo = fieldInfos[i];

                string propertyName, propertyNameCLSFriendly, propertyNameLowercaseUnderscore, propertyReflectedName;
                int propertyOrder = -1;
                var propertyType = fieldInfo.FieldType;
                var defaultValue = propertyType.GetDefaultValue();
                bool propertySuppressDefaultConfig = defaultValue != null && propertyType.IsValueType() && JsConfig.HasSerializeFn.Contains(propertyType);
                bool propertySuppressDefaultAttribute = false;
                var shouldSerialize = GetShouldSerializeMethod(fieldInfo);
                if (isDataContract)
                {
                    var dcsDataMember = fieldInfo.GetDataMember();
                    if (dcsDataMember == null) continue;

                    propertyName = dcsDataMember.Name ?? fieldInfo.Name;
                    propertyNameCLSFriendly = dcsDataMember.Name ?? propertyName.ToCamelCase();
                    propertyNameLowercaseUnderscore = dcsDataMember.Name ?? propertyName.ToLowercaseUnderscore();
                    propertyReflectedName = dcsDataMember.Name ?? fieldInfo.ReflectedType.Name;
                    propertyOrder = dcsDataMember.Order;
                    propertySuppressDefaultAttribute = !dcsDataMember.EmitDefaultValue;
                }
                else
                {
                    propertyName = fieldInfo.Name;
                    propertyNameCLSFriendly = propertyName.ToCamelCase();
                    propertyNameLowercaseUnderscore = propertyName.ToLowercaseUnderscore();
                    propertyReflectedName = fieldInfo.ReflectedType.Name;
                }

                PropertyWriters[i + propertyNamesLength] = new TypePropertyWriter
                (
                    propertyName,
                    propertyReflectedName,
                    propertyNameCLSFriendly,
                    propertyNameLowercaseUnderscore,
                    propertyOrder,
                    propertySuppressDefaultConfig,
                    propertySuppressDefaultAttribute,
                    fieldInfo.GetValueGetter<T>(),
                    Serializer.GetWriteFn(propertyType),
                    defaultValue,
                    shouldSerialize
                );
            }
            PropertyWriters = PropertyWriters.OrderBy(x => x.propertyOrder).ToArray();
            return true;
        }

        /// <summary>A type property writer.</summary>
        internal struct TypePropertyWriter
        {
            /// <summary>Gets the name of the property.</summary>
            /// <value>The name of the property.</value>
            internal string PropertyName
            {
                get
                {
                    return (JsConfig<T>.EmitCamelCaseNames || JsConfig.EmitCamelCaseNames)
                        ? propertyNameCLSFriendly
                        : (JsConfig<T>.EmitLowercaseUnderscoreNames || JsConfig.EmitLowercaseUnderscoreNames)
                            ? propertyNameLowercaseUnderscore
                            : propertyName;
                }
            }

            /// <summary>Name of the property.</summary>
            internal readonly string propertyName;

            /// <summary>The property order.</summary>
            internal readonly int propertyOrder;

            /// <summary>true to property suppress default configuration.</summary>
            internal readonly bool propertySuppressDefaultConfig;

            /// <summary>true to property suppress default attribute.</summary>
            internal readonly bool propertySuppressDefaultAttribute;

            /// <summary>Name of the property reflected.</summary>
            internal readonly string propertyReflectedName;

            /// <summary>The property combined name upper.</summary>
            internal readonly string propertyCombinedNameUpper;

            /// <summary>The property name cls friendly.</summary>
            internal readonly string propertyNameCLSFriendly;

            /// <summary>The property name lowercase underscore.</summary>
            internal readonly string propertyNameLowercaseUnderscore;

            /// <summary>The getter function.</summary>
            internal readonly Func<T, object> GetterFn;

            /// <summary>The write function.</summary>
            internal readonly WriteObjectDelegate WriteFn;

            /// <summary>The default value.</summary>
            internal readonly object DefaultValue;

            /// <summary>The should serialize.</summary>
            internal readonly Func<T, bool> shouldSerialize;

            /// <summary>
            /// Initializes a new instance of the NServiceKit.Text.Common.WriteType&lt;T, TSerializer&gt;
            /// class.
            /// </summary>
            /// <param name="propertyName">                    Name of the property.</param>
            /// <param name="propertyReflectedName">           Name of the property reflected.</param>
            /// <param name="propertyNameCLSFriendly">         The property name cls friendly.</param>
            /// <param name="propertyNameLowercaseUnderscore"> The property name lowercase underscore.</param>
            /// <param name="propertyOrder">                   The property order.</param>
            /// <param name="propertySuppressDefaultConfig">   true to property suppress default
            /// configuration.</param>
            /// <param name="propertySuppressDefaultAttribute">true to property suppress default attribute.</param>
            /// <param name="getterFn">                        The getter function.</param>
            /// <param name="writeFn">                         The write function.</param>
            /// <param name="defaultValue">                    The default value.</param>
            /// <param name="shouldSerialize">                 The should serialize.</param>
            public TypePropertyWriter(string propertyName, string propertyReflectedName, string propertyNameCLSFriendly, string propertyNameLowercaseUnderscore, int propertyOrder, bool propertySuppressDefaultConfig,bool propertySuppressDefaultAttribute,
                Func<T, object> getterFn, WriteObjectDelegate writeFn, object defaultValue, Func<T, bool> shouldSerialize)
            {
                this.propertyName = propertyName;
                this.propertyOrder = propertyOrder;
                this.propertySuppressDefaultConfig = propertySuppressDefaultConfig;
                this.propertySuppressDefaultAttribute = propertySuppressDefaultAttribute;
                this.propertyReflectedName = propertyReflectedName;
                this.propertyCombinedNameUpper = propertyReflectedName.ToUpper() + "." + propertyName.ToUpper();
                this.propertyNameCLSFriendly = propertyNameCLSFriendly;
                this.propertyNameLowercaseUnderscore = propertyNameLowercaseUnderscore;
                this.GetterFn = getterFn;
                this.WriteFn = writeFn;
                this.DefaultValue = defaultValue;
                this.shouldSerialize = shouldSerialize;
            }
        }

        /// <summary>Writes an object type.</summary>
        /// <param name="writer">The writer.</param>
        /// <param name="value"> The value.</param>
        public static void WriteObjectType(TextWriter writer, object value)
        {
            writer.Write(JsWriter.EmptyMap);
        }

        /// <summary>Writes an empty type.</summary>
        /// <param name="writer">The writer.</param>
        /// <param name="value"> The value.</param>
        public static void WriteEmptyType(TextWriter writer, object value)
        {
            if (WriteTypeInfo != null || JsState.IsWritingDynamic)
            {
                writer.Write(JsWriter.MapStartChar);
                if (!(JsConfig.PreferInterfaces && TryWriteSelfType(writer)))
                {
                    TryWriteTypeInfo(writer, value);
                }
                writer.Write(JsWriter.MapEndChar);
                return;
            }
            writer.Write(JsWriter.EmptyMap);
        }

        /// <summary>Writes an abstract properties.</summary>
        /// <param name="writer">The writer.</param>
        /// <param name="value"> The value.</param>
        public static void WriteAbstractProperties(TextWriter writer, object value)
        {
            if (value == null)
            {
                writer.Write(JsWriter.EmptyMap);
                return;
            }
            var valueType = value.GetType();
            if (valueType.IsAbstract())
            {
                WriteProperties(writer, value);
                return;
            }

            var writeFn = Serializer.GetWriteFn(valueType);
            if (!JsConfig<T>.ExcludeTypeInfo) JsState.IsWritingDynamic = true;
            writeFn(writer, value);
            if (!JsConfig<T>.ExcludeTypeInfo) JsState.IsWritingDynamic = false;
        }

        /// <summary>Writes the properties.</summary>
        /// <param name="writer">The writer.</param>
        /// <param name="value"> The value.</param>
        public static void WriteProperties(TextWriter writer, object value)
        {
            if (typeof(TSerializer) == typeof(JsonTypeSerializer) && JsState.WritingKeyCount > 0)
                writer.Write(JsWriter.QuoteChar);

            writer.Write(JsWriter.MapStartChar);

            var i = 0;
            if (WriteTypeInfo != null || JsState.IsWritingDynamic)
            {
                if (JsConfig.PreferInterfaces && TryWriteSelfType(writer)) i++;
                else if (TryWriteTypeInfo(writer, value)) i++;
                JsState.IsWritingDynamic = false;
            }

            if (PropertyWriters != null)
            {
                var len = PropertyWriters.Length;
                var exclude = JsConfig.ExcludePropertyReferences ?? new string[0];
                ConvertToUpper(exclude);
                for (int index = 0; index < len; index++)
                {
                    var propertyWriter = PropertyWriters[index];

                    if (propertyWriter.shouldSerialize != null && !propertyWriter.shouldSerialize((T)value)) continue;

                    var propertyValue = value != null
                        ? propertyWriter.GetterFn((T)value)
                        : null;
                    
                    if (propertyWriter.propertySuppressDefaultAttribute && Equals(propertyWriter.DefaultValue, propertyValue))
                    {
                        continue;
                    }
                    if ((propertyValue == null 
                         || (propertyWriter.propertySuppressDefaultConfig && Equals(propertyWriter.DefaultValue, propertyValue)))
                        && !Serializer.IncludeNullValues
                        )
                    {
                        continue;
                    }

                    if (exclude.Any() && exclude.Contains(propertyWriter.propertyCombinedNameUpper)) continue;

                    if (i++ > 0)
                        writer.Write(JsWriter.ItemSeperator);

                    Serializer.WritePropertyName(writer, propertyWriter.PropertyName);
                    writer.Write(JsWriter.MapKeySeperator);

                    if (typeof(TSerializer) == typeof(JsonTypeSerializer)) JsState.IsWritingValue = true;
                    if (propertyValue == null)
                    {
                        writer.Write(JsonUtils.Null);
                    }
                    else
                    {
                        propertyWriter.WriteFn(writer, propertyValue);
                    }
                    if (typeof(TSerializer) == typeof(JsonTypeSerializer)) JsState.IsWritingValue = false;
                }
            }

            writer.Write(JsWriter.MapEndChar);

            if (typeof(TSerializer) == typeof(JsonTypeSerializer) && JsState.WritingKeyCount > 0)
                writer.Write(JsWriter.QuoteChar);
        }

        /// <summary>The array brackets.</summary>
        private static readonly char[] ArrayBrackets = new[] { '[', ']' };

        /// <summary>Writes a query string.</summary>
        /// <param name="writer">The writer.</param>
        /// <param name="value"> The value.</param>
        public static void WriteQueryString(TextWriter writer, object value)
        {
            try
            {
                JsState.QueryStringMode = true;
                var i = 0;
                foreach (var propertyWriter in PropertyWriters)
                {
                    var propertyValue = propertyWriter.GetterFn((T)value);
                    if (propertyValue == null) continue;

                    if (i++ > 0)
                        writer.Write('&');

                    Serializer.WritePropertyName(writer, propertyWriter.PropertyName);
                    writer.Write('=');

                    var isEnumerable = propertyValue != null
                        && !(propertyValue is string)
                        && !(propertyValue.GetType().IsValueType())
                        && propertyValue.GetType().HasInterface(typeof(IEnumerable));

                    if (!isEnumerable)
                    {
                        propertyWriter.WriteFn(writer, propertyValue);
                    }
                    else
                    {                        
                        //Trim brackets in top-level lists in QueryStrings, e.g: ?a=[1,2,3] => ?a=1,2,3
                        using (var ms = new MemoryStream())
                        using (var enumerableWriter = new StreamWriter(ms))
                        {
                            propertyWriter.WriteFn(enumerableWriter, propertyValue); 
                            enumerableWriter.Flush();
                            var output = ms.ToArray().FromUtf8Bytes();
                            output = output.Trim(ArrayBrackets);
                            writer.Write(output);
                        }
                    }
                }
            }
            finally
            {
                JsState.QueryStringMode = false;
            }
        }

        /// <summary>Converts a strArray to an upper.</summary>
        /// <param name="strArray">The array.</param>
        private static void ConvertToUpper(string[] strArray)
        {
            for (var i = 0; i < strArray.Length; ++i)
            {
                if (strArray[i] != null)
                    strArray[i] = strArray[i].ToUpper();
            }
        }
    }
}
