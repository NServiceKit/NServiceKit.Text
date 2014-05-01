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
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;
using NServiceKit.Text.Support;
#if WINDOWS_PHONE
using System.Linq.Expressions;
#endif

namespace NServiceKit.Text
{
    /// <summary>Empty constructor factory delegate.</summary>
    /// <param name="type">The type.</param>
    /// <returns>An EmptyCtorDelegate.</returns>
    public delegate EmptyCtorDelegate EmptyCtorFactoryDelegate(Type type);

    /// <summary>Empty constructor delegate.</summary>
    /// <returns>An object.</returns>
    public delegate object EmptyCtorDelegate();

    /// <summary>A reflection extensions.</summary>
    public static class ReflectionExtensions
    {
        /// <summary>The default value types.</summary>
        private static Dictionary<Type, object> DefaultValueTypes = new Dictionary<Type, object>();

        /// <summary>A Type extension method that gets default value.</summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>The default value.</returns>
        public static object GetDefaultValue(this Type type)
        {
            if (!type.IsValueType()) return null;

            object defaultValue;
            if (DefaultValueTypes.TryGetValue(type, out defaultValue)) return defaultValue;

            defaultValue = Activator.CreateInstance(type);

            Dictionary<Type, object> snapshot, newCache;
            do
            {
                snapshot = DefaultValueTypes;
                newCache = new Dictionary<Type, object>(DefaultValueTypes);
                newCache[type] = defaultValue;

            } while (!ReferenceEquals(
                Interlocked.CompareExchange(ref DefaultValueTypes, newCache, snapshot), snapshot));

            return defaultValue;
        }

        /// <summary>A Type extension method that query if 'type' is instance of.</summary>
        /// <param name="type">          The type to act on.</param>
        /// <param name="thisOrBaseType">Type of this or base.</param>
        /// <returns>true if instance of, false if not.</returns>
        public static bool IsInstanceOf(this Type type, Type thisOrBaseType)
        {
            while (type != null)
            {
                if (type == thisOrBaseType)
                    return true;

                type = type.BaseType();
            }
            return false;
        }

        /// <summary>A Type extension method that query if 'type' has generic type.</summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>true if generic type, false if not.</returns>
        public static bool HasGenericType(this Type type)
        {
            while (type != null)
            {
                if (type.IsGeneric())
                    return true;

                type = type.BaseType();
            }
            return false;
        }

        /// <summary>A Type extension method that gets generic type.</summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>The generic type.</returns>
        public static Type GetGenericType(this Type type)
        {
            while (type != null)
            {
                if (type.IsGeneric())
                    return type;

                type = type.BaseType();
            }
            return null;
        }

        /// <summary>
        /// A Type extension method that gets type with generic type definition of any.
        /// </summary>
        /// <param name="type">                  The type to act on.</param>
        /// <param name="genericTypeDefinitions">A variable-length parameters list containing generic type
        /// definitions.</param>
        /// <returns>The type with generic type definition of any.</returns>
        public static Type GetTypeWithGenericTypeDefinitionOfAny(this Type type, params Type[] genericTypeDefinitions)
        {
            foreach (var genericTypeDefinition in genericTypeDefinitions)
            {
                var genericType = type.GetTypeWithGenericTypeDefinitionOf(genericTypeDefinition);
                if (genericType == null && type == genericTypeDefinition)
                {
                    genericType = type;
                }

                if (genericType != null)
                    return genericType;
            }
            return null;
        }

        /// <summary>
        /// A Type extension method that query if 'type' is or has generic interface type of.
        /// </summary>
        /// <param name="type">                 The type to act on.</param>
        /// <param name="genericTypeDefinition">The generic type definition.</param>
        /// <returns>true if or has generic interface type of, false if not.</returns>
        public static bool IsOrHasGenericInterfaceTypeOf(this Type type, Type genericTypeDefinition)
        {
            return (type.GetTypeWithGenericTypeDefinitionOf(genericTypeDefinition) != null)
                || (type == genericTypeDefinition);
        }

        /// <summary>A Type extension method that gets type with generic type definition of.</summary>
        /// <param name="type">                 The type to act on.</param>
        /// <param name="genericTypeDefinition">The generic type definition.</param>
        /// <returns>The type with generic type definition of.</returns>
        public static Type GetTypeWithGenericTypeDefinitionOf(this Type type, Type genericTypeDefinition)
        {
            foreach (var t in type.GetTypeInterfaces())
            {
                if (t.IsGeneric() && t.GetGenericTypeDefinition() == genericTypeDefinition)
                {
                    return t;
                }
            }

            var genericType = type.GetGenericType();
            if (genericType != null && genericType.GetGenericTypeDefinition() == genericTypeDefinition)
            {
                return genericType;
            }

            return null;
        }

        /// <summary>A Type extension method that gets type with interface of.</summary>
        /// <param name="type">         The type to act on.</param>
        /// <param name="interfaceType">Type of the interface.</param>
        /// <returns>The type with interface of.</returns>
        public static Type GetTypeWithInterfaceOf(this Type type, Type interfaceType)
        {
            if (type == interfaceType) return interfaceType;

            foreach (var t in type.GetTypeInterfaces())
            {
                if (t == interfaceType)
                    return t;
            }

            return null;
        }

        /// <summary>A Type extension method that query if 'type' has interface.</summary>
        /// <param name="type">         The type to act on.</param>
        /// <param name="interfaceType">Type of the interface.</param>
        /// <returns>true if interface, false if not.</returns>
        public static bool HasInterface(this Type type, Type interfaceType)
        {
            foreach (var t in type.GetTypeInterfaces())
            {
                if (t == interfaceType)
                    return true;
            }
            return false;
        }

        /// <summary>A Type extension method that all have interfaces of type.</summary>
        /// <param name="assignableFromType">The assignableFromType to act on.</param>
        /// <param name="types">             A variable-length parameters list containing types.</param>
        /// <returns>true if it succeeds, false if it fails.</returns>
        public static bool AllHaveInterfacesOfType(
            this Type assignableFromType, params Type[] types)
        {
            foreach (var type in types)
            {
                if (assignableFromType.GetTypeWithInterfaceOf(type) == null) return false;
            }
            return true;
        }

        /// <summary>A Type extension method that query if 'type' is numeric type.</summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>true if numeric type, false if not.</returns>
        public static bool IsNumericType(this Type type)
        {
            if (type == null) return false;

            if (type.IsEnum) //TypeCode can be TypeCode.Int32
            {
                return JsConfig.TreatEnumAsInteger || type.IsEnumFlags();
            }

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;

                case TypeCode.Object:
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        return IsNumericType(Nullable.GetUnderlyingType(type));
                    }
                    if (type.IsEnum)
                    {
                        return JsConfig.TreatEnumAsInteger || type.IsEnumFlags();
                    }
                    return false;
            }
            return false;
        }

        /// <summary>A Type extension method that query if 'type' is integer type.</summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>true if integer type, false if not.</returns>
        public static bool IsIntegerType(this Type type)
        {
            if (type == null) return false;

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;

                case TypeCode.Object:
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        return IsNumericType(Nullable.GetUnderlyingType(type));
                    }
                    return false;
            }
            return false;
        }

        /// <summary>A Type extension method that query if 'type' is real number type.</summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>true if real number type, false if not.</returns>
        public static bool IsRealNumberType(this Type type)
        {
            if (type == null) return false;

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;

                case TypeCode.Object:
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        return IsNumericType(Nullable.GetUnderlyingType(type));
                    }
                    return false;
            }
            return false;
        }

        /// <summary>A Type extension method that gets type with generic interface of.</summary>
        /// <param name="type">                The type to act on.</param>
        /// <param name="genericInterfaceType">Type of the generic interface.</param>
        /// <returns>The type with generic interface of.</returns>
        public static Type GetTypeWithGenericInterfaceOf(this Type type, Type genericInterfaceType)
        {
            foreach (var t in type.GetTypeInterfaces())
            {
                if (t.IsGeneric() && t.GetGenericTypeDefinition() == genericInterfaceType) 
                    return t;
            }

            if (!type.IsGeneric()) return null;

            var genericType = type.GetGenericType();
            return genericType.GetGenericTypeDefinition() == genericInterfaceType
                    ? genericType
                    : null;
        }

        /// <summary>
        /// A Type extension method that query if 'genericType' has any type definitions of.
        /// </summary>
        /// <param name="genericType">      The genericType to act on.</param>
        /// <param name="theseGenericTypes">List of types of the these generics.</param>
        /// <returns>true if any type definitions of, false if not.</returns>
        public static bool HasAnyTypeDefinitionsOf(this Type genericType, params Type[] theseGenericTypes)
        {
            if (!genericType.IsGeneric()) return false;

            var genericTypeDefinition = genericType.GenericTypeDefinition();

            foreach (var thisGenericType in theseGenericTypes)
            {
                if (genericTypeDefinition == thisGenericType)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// A Type extension method that gets generic arguments if both have same generic definition type
        /// and arguments.
        /// </summary>
        /// <param name="assignableFromType">The assignableFromType to act on.</param>
        /// <param name="typeA">             The type a.</param>
        /// <param name="typeB">             The type b.</param>
        /// <returns>An array of type.</returns>
        public static Type[] GetGenericArgumentsIfBothHaveSameGenericDefinitionTypeAndArguments(
            this Type assignableFromType, Type typeA, Type typeB)
        {
            var typeAInterface = typeA.GetTypeWithGenericInterfaceOf(assignableFromType);
            if (typeAInterface == null) return null;

            var typeBInterface = typeB.GetTypeWithGenericInterfaceOf(assignableFromType);
            if (typeBInterface == null) return null;

            var typeAGenericArgs = typeAInterface.GetTypeGenericArguments();
            var typeBGenericArgs = typeBInterface.GetTypeGenericArguments();

            if (typeAGenericArgs.Length != typeBGenericArgs.Length) return null;

            for (var i = 0; i < typeBGenericArgs.Length; i++)
            {
                if (typeAGenericArgs[i] != typeBGenericArgs[i])
                {
                    return null;
                }
            }

            return typeAGenericArgs;
        }

        /// <summary>
        /// A Type extension method that gets generic arguments if both have convertible generic
        /// definition type and arguments.
        /// </summary>
        /// <param name="assignableFromType">The assignableFromType to act on.</param>
        /// <param name="typeA">             The type a.</param>
        /// <param name="typeB">             The type b.</param>
        /// <returns>
        /// The generic arguments if both have convertible generic definition type and arguments.
        /// </returns>
        public static TypePair GetGenericArgumentsIfBothHaveConvertibleGenericDefinitionTypeAndArguments(
            this Type assignableFromType, Type typeA, Type typeB)
        {
            var typeAInterface = typeA.GetTypeWithGenericInterfaceOf(assignableFromType);
            if (typeAInterface == null) return null;

            var typeBInterface = typeB.GetTypeWithGenericInterfaceOf(assignableFromType);
            if (typeBInterface == null) return null;

            var typeAGenericArgs = typeAInterface.GetTypeGenericArguments();
            var typeBGenericArgs = typeBInterface.GetTypeGenericArguments();

            if (typeAGenericArgs.Length != typeBGenericArgs.Length) return null;

            for (var i = 0; i < typeBGenericArgs.Length; i++)
            {
                if (!AreAllStringOrValueTypes(typeAGenericArgs[i], typeBGenericArgs[i]))
                {
                    return null;
                }
            }

            return new TypePair(typeAGenericArgs, typeBGenericArgs);
        }

        /// <summary>Determine if we are all string or value types.</summary>
        /// <param name="types">A variable-length parameters list containing types.</param>
        /// <returns>true if all string or value types, false if not.</returns>
        public static bool AreAllStringOrValueTypes(params Type[] types)
        {
            foreach (var type in types)
            {
                if (!(type == typeof(string) || type.IsValueType())) return false;
            }
            return true;
        }

        /// <summary>The constructor methods.</summary>
        static Dictionary<Type, EmptyCtorDelegate> ConstructorMethods = new Dictionary<Type, EmptyCtorDelegate>();

        /// <summary>Gets constructor method.</summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>The constructor method.</returns>
        public static EmptyCtorDelegate GetConstructorMethod(Type type)
        {
            EmptyCtorDelegate emptyCtorFn;
            if (ConstructorMethods.TryGetValue(type, out emptyCtorFn)) return emptyCtorFn;

            emptyCtorFn = GetConstructorMethodToCache(type);

            Dictionary<Type, EmptyCtorDelegate> snapshot, newCache;
            do
            {
                snapshot = ConstructorMethods;
                newCache = new Dictionary<Type, EmptyCtorDelegate>(ConstructorMethods);
                newCache[type] = emptyCtorFn;

            } while (!ReferenceEquals(
                Interlocked.CompareExchange(ref ConstructorMethods, newCache, snapshot), snapshot));

            return emptyCtorFn;
        }

        /// <summary>The type names map.</summary>
        static Dictionary<string, EmptyCtorDelegate> TypeNamesMap = new Dictionary<string, EmptyCtorDelegate>();

        /// <summary>Gets constructor method.</summary>
        /// <param name="typeName">Name of the type.</param>
        /// <returns>The constructor method.</returns>
        public static EmptyCtorDelegate GetConstructorMethod(string typeName)
        {
            EmptyCtorDelegate emptyCtorFn;
            if (TypeNamesMap.TryGetValue(typeName, out emptyCtorFn)) return emptyCtorFn;

            var type = JsConfig.TypeFinder.Invoke(typeName);
            if (type == null) return null;
            emptyCtorFn = GetConstructorMethodToCache(type);

            Dictionary<string, EmptyCtorDelegate> snapshot, newCache;
            do
            {
                snapshot = TypeNamesMap;
                newCache = new Dictionary<string, EmptyCtorDelegate>(TypeNamesMap);
                newCache[typeName] = emptyCtorFn;

            } while (!ReferenceEquals(
                Interlocked.CompareExchange(ref TypeNamesMap, newCache, snapshot), snapshot));

            return emptyCtorFn;
        }

        /// <summary>Gets constructor method to cache.</summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>The constructor method to cache.</returns>
        public static EmptyCtorDelegate GetConstructorMethodToCache(Type type)
        {
            if (type.IsInterface)
            {
                if (type.HasGenericType())
                {
                    var genericType = type.GetTypeWithGenericTypeDefinitionOfAny(
                        typeof(IDictionary<,>));

                    if (genericType != null)
                    {
                        var keyType = genericType.GenericTypeArguments()[0];
                        var valueType = genericType.GenericTypeArguments()[1];
                        return GetConstructorMethodToCache(typeof(Dictionary<,>).MakeGenericType(keyType, valueType));
                    }

                    genericType = type.GetTypeWithGenericTypeDefinitionOfAny(
                        typeof(IEnumerable<>),
                        typeof(ICollection<>),
                        typeof(IList<>));

                    if (genericType != null)
                    {
                        var elementType = genericType.GenericTypeArguments()[0];
                        return GetConstructorMethodToCache(typeof(List<>).MakeGenericType(elementType));
                    }
                }
            }
            else if (type.IsArray)
            {
                return () => Array.CreateInstance(type.GetElementType(), 0);
            }

            var emptyCtor = type.GetEmptyConstructor();
            if (emptyCtor != null)
            {

#if MONOTOUCH || c|| XBOX || NETFX_CORE
				return () => Activator.CreateInstance(type);
#elif WINDOWS_PHONE
                return Expression.Lambda<EmptyCtorDelegate>(Expression.New(type)).Compile();
#else
#if SILVERLIGHT
                var dm = new System.Reflection.Emit.DynamicMethod("MyCtor", type, Type.EmptyTypes);
#else
                var dm = new System.Reflection.Emit.DynamicMethod("MyCtor", type, Type.EmptyTypes, typeof(ReflectionExtensions).Module, true);
#endif
                var ilgen = dm.GetILGenerator();
                ilgen.Emit(System.Reflection.Emit.OpCodes.Nop);
                ilgen.Emit(System.Reflection.Emit.OpCodes.Newobj, emptyCtor);
                ilgen.Emit(System.Reflection.Emit.OpCodes.Ret);

                return (EmptyCtorDelegate)dm.CreateDelegate(typeof(EmptyCtorDelegate));
#endif
            }

#if (SILVERLIGHT && !WINDOWS_PHONE) || XBOX
            return () => Activator.CreateInstance(type);
#elif WINDOWS_PHONE
            return Expression.Lambda<EmptyCtorDelegate>(Expression.New(type)).Compile();
#else
            if (type == typeof(string))
                return () => String.Empty;

            //Anonymous types don't have empty constructors
            return () => FormatterServices.GetUninitializedObject(type);
#endif
        }

        /// <summary>A type meta.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        private static class TypeMeta<T>
        {
            /// <summary>The empty constructor function.</summary>
            public static readonly EmptyCtorDelegate EmptyCtorFn;

            /// <summary>
            /// Initializes static members of the NServiceKit.Text.ReflectionExtensions.TypeMeta&lt;T&gt;
            /// class.
            /// </summary>
            static TypeMeta()
            {
                EmptyCtorFn = GetConstructorMethodToCache(typeof(T));
            }
        }

        /// <summary>Creates the instance.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <returns>The new instance.</returns>
        public static object CreateInstance<T>()
        {
            return TypeMeta<T>.EmptyCtorFn();
        }

        /// <summary>Creates an instance.</summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>The new instance.</returns>
        public static object CreateInstance(this Type type)
        {
            var ctorFn = GetConstructorMethod(type);
            return ctorFn();
        }

        /// <summary>Creates an instance.</summary>
        /// <param name="typeName">Name of the type.</param>
        /// <returns>The new instance.</returns>
        public static object CreateInstance(string typeName)
        {
            var ctorFn = GetConstructorMethod(typeName);
            return ctorFn();
        }

        /// <summary>A Type extension method that gets public properties.</summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>An array of property information.</returns>
        public static PropertyInfo[] GetPublicProperties(this Type type)
        {
            if (type.IsInterface())
            {
                var propertyInfos = new List<PropertyInfo>();

                var considered = new List<Type>();
                var queue = new Queue<Type>();
                considered.Add(type);
                queue.Enqueue(type);

                while (queue.Count > 0)
                {
                    var subType = queue.Dequeue();
                    foreach (var subInterface in subType.GetTypeInterfaces())
                    {
                        if (considered.Contains(subInterface)) continue;

                        considered.Add(subInterface);
                        queue.Enqueue(subInterface);
                    }

                    var typeProperties = subType.GetTypesPublicProperties();

                    var newPropertyInfos = typeProperties
                        .Where(x => !propertyInfos.Contains(x));

                    propertyInfos.InsertRange(0, newPropertyInfos);
                }

                return propertyInfos.ToArray();
            }

            return type.GetTypesPublicProperties()
                .Where(t => t.GetIndexParameters().Length == 0) // ignore indexed properties
                .ToArray();
        }

        /// <summary>The data contract.</summary>
        const string DataContract = "DataContractAttribute";

        /// <summary>The data member.</summary>
        const string DataMember = "DataMemberAttribute";

        /// <summary>The ignore data member.</summary>
        const string IgnoreDataMember = "IgnoreDataMemberAttribute";

        /// <summary>A Type extension method that gets serializable properties.</summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>An array of property information.</returns>
        public static PropertyInfo[] GetSerializableProperties(this Type type)
        {
            var publicProperties = GetPublicProperties(type);
            var publicReadableProperties = publicProperties.Where(x => x.PropertyGetMethod() != null);

            if (type.IsDto())
            {
                return !Env.IsMono
                    ? publicReadableProperties.Where(attr => 
                        attr.IsDefined(typeof(DataMemberAttribute), false)).ToArray()
                    : publicReadableProperties.Where(attr => 
                        attr.CustomAttributes(false).Any(x => x.GetType().Name == DataMember)).ToArray();
            }

            // else return those properties that are not decorated with IgnoreDataMember
            return publicReadableProperties.Where(prop => !prop.CustomAttributes(false).Any(attr => attr.GetType().Name == IgnoreDataMember)).ToArray();
        }

        /// <summary>A Type extension method that gets serializable fields.</summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>An array of field information.</returns>
        public static FieldInfo[] GetSerializableFields(this Type type)
        {
            if (type.IsDto()) {
                var allFields = type.GetAllFields();

                // The contract must be honered 
                return allFields.Where(prop => prop.CustomAttributes(false).Any(attr => attr.GetType().Name == DataMember)).ToArray();
            }
            
            var publicFields = type.GetPublicFields();

            // else return those properties that are not decorated with IgnoreDataMember
            return publicFields.Where(prop => !prop.CustomAttributes(false).Any(attr => attr.GetType().Name == IgnoreDataMember)).ToArray();
        }

        /// <summary>A Type extension method that query if 'type' has attribute.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="type">The type to act on.</param>
        /// <returns>true if attribute, false if not.</returns>
        public static bool HasAttr<T>(this Type type) where T : Attribute
        {
            return type.HasAttribute<T>();
        }

#if !SILVERLIGHT && !MONOTOUCH 
        /// <summary>The type accessor map.</summary>
        static readonly Dictionary<Type, FastMember.TypeAccessor> typeAccessorMap 
            = new Dictionary<Type, FastMember.TypeAccessor>();
#endif

        /// <summary>A Type extension method that gets data contract.</summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>The data contract.</returns>
        public static DataContractAttribute GetDataContract(this Type type)
        {
            var dataContract = type.FirstAttribute<DataContractAttribute>();

#if !SILVERLIGHT && !MONOTOUCH && !XBOX
            if (dataContract == null && Env.IsMono)
                return type.GetWeakDataContract();
#endif
            return dataContract;
        }

        /// <summary>A FieldInfo extension method that gets data member.</summary>
        /// <param name="pi">The pi to act on.</param>
        /// <returns>The data member.</returns>
        public static DataMemberAttribute GetDataMember(this PropertyInfo pi)
        {
            var dataMember = pi.CustomAttributes(typeof(DataMemberAttribute), false)
                .FirstOrDefault() as DataMemberAttribute;

#if !SILVERLIGHT && !MONOTOUCH && !XBOX
            if (dataMember == null && Env.IsMono)
                return pi.GetWeakDataMember();
#endif
            return dataMember;
        }

        /// <summary>A FieldInfo extension method that gets data member.</summary>
        /// <param name="pi">The pi to act on.</param>
        /// <returns>The data member.</returns>
        public static DataMemberAttribute GetDataMember(this FieldInfo pi)
        {
            var dataMember = pi.CustomAttributes(typeof(DataMemberAttribute), false)
                .FirstOrDefault() as DataMemberAttribute;

#if !SILVERLIGHT && !MONOTOUCH && !XBOX
            if (dataMember == null && Env.IsMono)
                return pi.GetWeakDataMember();
#endif
            return dataMember;
        }

#if !SILVERLIGHT && !MONOTOUCH && !XBOX
        /// <summary>A Type extension method that gets weak data contract.</summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>The weak data contract.</returns>
        public static DataContractAttribute GetWeakDataContract(this Type type)
        {
            var attr = type.CustomAttributes().FirstOrDefault(x => x.GetType().Name == DataContract);
            if (attr != null)
            {
                var attrType = attr.GetType();

                FastMember.TypeAccessor accessor;
                lock (typeAccessorMap)
                {
                    if (!typeAccessorMap.TryGetValue(attrType, out accessor))
                        typeAccessorMap[attrType] = accessor = FastMember.TypeAccessor.Create(attr.GetType());
                }

                return new DataContractAttribute {
                    Name = (string)accessor[attr, "Name"],
                    Namespace = (string)accessor[attr, "Namespace"],
                };
            }
            return null;
        }

        /// <summary>A FieldInfo extension method that gets weak data member.</summary>
        /// <param name="pi">The pi to act on.</param>
        /// <returns>The weak data member.</returns>
        public static DataMemberAttribute GetWeakDataMember(this PropertyInfo pi)
        {
            var attr = pi.CustomAttributes().FirstOrDefault(x => x.GetType().Name == DataMember);
            if (attr != null)
            {
                var attrType = attr.GetType();

                FastMember.TypeAccessor accessor;
                lock (typeAccessorMap)
                {
                    if (!typeAccessorMap.TryGetValue(attrType, out accessor))
                        typeAccessorMap[attrType] = accessor = FastMember.TypeAccessor.Create(attr.GetType());
                }

                var newAttr = new DataMemberAttribute {
                    Name = (string) accessor[attr, "Name"],
                    EmitDefaultValue = (bool)accessor[attr, "EmitDefaultValue"],
                    IsRequired = (bool)accessor[attr, "IsRequired"],
                };

                var order = (int)accessor[attr, "Order"];
                if (order >= 0)
                    newAttr.Order = order; //Throws Exception if set to -1

                return newAttr;
            }
            return null;
        }

        /// <summary>A FieldInfo extension method that gets weak data member.</summary>
        /// <param name="pi">The pi to act on.</param>
        /// <returns>The weak data member.</returns>
        public static DataMemberAttribute GetWeakDataMember(this FieldInfo pi)
        {
            var attr = pi.CustomAttributes().FirstOrDefault(x => x.GetType().Name == DataMember);
            if (attr != null)
            {
                var attrType = attr.GetType();

                FastMember.TypeAccessor accessor;
                lock (typeAccessorMap)
                {
                    if (!typeAccessorMap.TryGetValue(attrType, out accessor))
                        typeAccessorMap[attrType] = accessor = FastMember.TypeAccessor.Create(attr.GetType());
                }

                var newAttr = new DataMemberAttribute
                {
                    Name = (string)accessor[attr, "Name"],
                    EmitDefaultValue = (bool)accessor[attr, "EmitDefaultValue"],
                    IsRequired = (bool)accessor[attr, "IsRequired"],
                };

                var order = (int)accessor[attr, "Order"];
                if (order >= 0)
                    newAttr.Order = order; //Throws Exception if set to -1

                return newAttr;
            }
            return null;
        }
#endif
    }

    /// <summary>A platform extensions.</summary>
    public static class PlatformExtensions //Because WinRT is a POS
    {
        /// <summary>A Type extension method that query if 'type' is interface.</summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>true if interface, false if not.</returns>
        public static bool IsInterface(this Type type)
        {
#if NETFX_CORE
            return type.GetTypeInfo().IsInterface;
#else
            return type.IsInterface;
#endif
        }

        /// <summary>A Type extension method that query if 'type' is array.</summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>true if array, false if not.</returns>
        public static bool IsArray(this Type type)
        {
#if NETFX_CORE
            return type.GetTypeInfo().IsArray;
#else
            return type.IsArray;
#endif
        }

        /// <summary>A Type extension method that query if 'type' is value type.</summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>true if value type, false if not.</returns>
        public static bool IsValueType(this Type type)
        {
#if NETFX_CORE
            return type.GetTypeInfo().IsValueType;
#else
            return type.IsValueType;
#endif
        }

        /// <summary>A Type extension method that query if 'type' is generic.</summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>true if generic, false if not.</returns>
        public static bool IsGeneric(this Type type)
        {
#if NETFX_CORE
            return type.GetTypeInfo().IsGenericType;
#else
            return type.IsGenericType;
#endif
        }

        /// <summary>A Type extension method that base type.</summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>A Type.</returns>
        public static Type BaseType(this Type type)
        {
#if NETFX_CORE
            return type.GetTypeInfo().BaseType;
#else
            return type.BaseType;
#endif
        }

        /// <summary>A FieldInfo extension method that reflected type.</summary>
        /// <param name="pi">The pi to act on.</param>
        /// <returns>A Type.</returns>
        public static Type ReflectedType(this PropertyInfo pi)
        {
#if NETFX_CORE
            return pi.PropertyType;
#else
            return pi.ReflectedType;
#endif
        }

        /// <summary>A FieldInfo extension method that reflected type.</summary>
        /// <param name="fi">The fi to act on.</param>
        /// <returns>A Type.</returns>
        public static Type ReflectedType(this FieldInfo fi)
        {
#if NETFX_CORE
            return fi.FieldType;
#else
            return fi.ReflectedType;
#endif
        }

        /// <summary>A Type extension method that generic type definition.</summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>A Type.</returns>
        public static Type GenericTypeDefinition(this Type type)
        {
#if NETFX_CORE
            return type.GetTypeInfo().GetGenericTypeDefinition();
#else
            return type.GetGenericTypeDefinition();
#endif
        }

        /// <summary>A Type extension method that gets type interfaces.</summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>An array of type.</returns>
        public static Type[] GetTypeInterfaces(this Type type)
        {
#if NETFX_CORE
            return type.GetTypeInfo().ImplementedInterfaces.ToArray();
#else
            return type.GetInterfaces();
#endif
        }

        /// <summary>A Type extension method that gets type generic arguments.</summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>An array of type.</returns>
        public static Type[] GetTypeGenericArguments(this Type type)
        {
#if NETFX_CORE
            return type.GenericTypeArguments;
#else
            return type.GetGenericArguments();
#endif
        }

        /// <summary>A Type extension method that gets empty constructor.</summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>The empty constructor.</returns>
        public static ConstructorInfo GetEmptyConstructor(this Type type)
        {
#if NETFX_CORE
            return type.GetTypeInfo().DeclaredConstructors.FirstOrDefault(c => c.GetParameters().Count() == 0);
#else
            return type.GetConstructor(Type.EmptyTypes);
#endif
        }

        /// <summary>A Type extension method that gets types public properties.</summary>
        /// <param name="subType">The subType to act on.</param>
        /// <returns>An array of property information.</returns>
        internal static PropertyInfo[] GetTypesPublicProperties(this Type subType)
        {
#if NETFX_CORE 
            return subType.GetRuntimeProperties().ToArray();
#else
            return subType.GetProperties(
                BindingFlags.FlattenHierarchy |
                BindingFlags.Public |
                BindingFlags.Instance);
#endif
        }

        /// <summary>A Type extension method that properties the given type.</summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>A PropertyInfo[].</returns>
        public static PropertyInfo[] Properties(this Type type)
        {
#if NETFX_CORE 
            return type.GetRuntimeProperties().ToArray();
#else
            return type.GetProperties();
#endif
        }

        /// <summary>A Type extension method that gets all fields.</summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>An array of field information.</returns>
        public static FieldInfo[] GetAllFields(this Type type)
        {
            if (type.IsInterface())
            {
                return new FieldInfo[0];
            }

#if NETFX_CORE
            return type.GetRuntimeFields().Where(p => !p.IsStatic).ToArray();
#else
            return type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .ToArray();
#endif
        }

        /// <summary>A Type extension method that gets public fields.</summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>An array of field information.</returns>
        public static FieldInfo[] GetPublicFields(this Type type)
        {
            if (type.IsInterface())
            {
                return new FieldInfo[0];
            }

#if NETFX_CORE
            return type.GetRuntimeFields().Where(p => p.IsPublic && !p.IsStatic).ToArray();
#else
            return type.GetFields(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance)
            .ToArray();
#endif
        }

        /// <summary>A Type extension method that gets public members.</summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>An array of member information.</returns>
        public static MemberInfo[] GetPublicMembers(this Type type)
        {

#if NETFX_CORE
            var members = new List<MemberInfo>();
            members.AddRange(type.GetRuntimeFields().Where(p => p.IsPublic && !p.IsStatic));
            members.AddRange(type.GetPublicProperties());
            return members.ToArray();
#else
            return type.GetMembers(BindingFlags.Public | BindingFlags.Instance);
#endif
        }

        /// <summary>A Type extension method that gets all public members.</summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>An array of member information.</returns>
        public static MemberInfo[] GetAllPublicMembers(this Type type)
        {

#if NETFX_CORE
            var members = new List<MemberInfo>();
            members.AddRange(type.GetRuntimeFields().Where(p => p.IsPublic && !p.IsStatic));
            members.AddRange(type.GetPublicProperties());
            return members.ToArray();
#else
            return type.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
#endif
        }

        /// <summary>A Type extension method that query if 'type' has attribute.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="type">   The type to act on.</param>
        /// <param name="inherit">true to inherit.</param>
        /// <returns>true if attribute, false if not.</returns>
        public static bool HasAttribute<T>(this Type type, bool inherit = true) where T : Attribute
        {
            return type.CustomAttributes(inherit).Any(x => x.GetType() == typeof(T));
        }

        /// <summary>Enumerates attributes of type in this collection.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="type">   The type to act on.</param>
        /// <param name="inherit">true to inherit.</param>
        /// <returns>
        /// An enumerator that allows foreach to be used to process attributes of type in this collection.
        /// </returns>
        public static IEnumerable<T> AttributesOfType<T>(this Type type, bool inherit = true) where T : Attribute
        {
#if NETFX_CORE
            return type.GetTypeInfo().GetCustomAttributes<T>(inherit);
#else
            return type.GetCustomAttributes(inherit).OfType<T>();
#endif
        }

        /// <summary>The data contract.</summary>
        const string DataContract = "DataContractAttribute";

        /// <summary>A Type extension method that query if 'type' is dto.</summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>true if dto, false if not.</returns>
        public static bool IsDto(this Type type)
        {
#if NETFX_CORE
            return type.GetTypeInfo().IsDefined(typeof(DataContractAttribute), false);
#else
            return !Env.IsMono
                   ? type.IsDefined(typeof(DataContractAttribute), false)
                   : type.GetCustomAttributes(true).Any(x => x.GetType().Name == DataContract);
#endif
        }

        /// <summary>A PropertyInfo extension method that property get method.</summary>
        /// <param name="pi">       The pi to act on.</param>
        /// <param name="nonPublic">true to non public.</param>
        /// <returns>A MethodInfo.</returns>
        public static MethodInfo PropertyGetMethod(this PropertyInfo pi, bool nonPublic = false)
        {
#if NETFX_CORE
            return pi.GetMethod;
#else
            return pi.GetGetMethod(false);
#endif
        }

        /// <summary>A Type extension method that interfaces the given type.</summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>A Type[].</returns>
        public static Type[] Interfaces(this Type type)
        {
#if NETFX_CORE
            return type.GetTypeInfo().ImplementedInterfaces.ToArray();
            //return type.GetTypeInfo().ImplementedInterfaces
            //    .FirstOrDefault(x => !x.GetTypeInfo().ImplementedInterfaces
            //        .Any(y => y.GetTypeInfo().ImplementedInterfaces.Contains(y)));
#else
            return type.GetInterfaces();
#endif
        }

        /// <summary>A Type extension method that all properties.</summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>A PropertyInfo[].</returns>
        public static PropertyInfo[] AllProperties(this Type type)
        {
#if NETFX_CORE
            return type.GetRuntimeProperties().ToArray();
#else
            return type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
#endif
        }

        /// <summary>A Type extension method that custom attributes.</summary>
        /// <param name="propertyInfo">The propertyInfo to act on.</param>
        /// <param name="inherit">     true to inherit.</param>
        /// <returns>An object[].</returns>
        public static object[] CustomAttributes(this PropertyInfo propertyInfo, bool inherit = true)
        {
#if NETFX_CORE
            return propertyInfo.GetCustomAttributes(inherit).ToArray();
#else
            return propertyInfo.GetCustomAttributes(inherit);
#endif
        }

        /// <summary>A Type extension method that custom attributes.</summary>
        /// <param name="propertyInfo">The propertyInfo to act on.</param>
        /// <param name="attrType">    Type of the attribute.</param>
        /// <param name="inherit">     true to inherit.</param>
        /// <returns>An object[].</returns>
        public static object[] CustomAttributes(this PropertyInfo propertyInfo, Type attrType, bool inherit = true)
        {
#if NETFX_CORE
            return propertyInfo.GetCustomAttributes(inherit).Where(x => x.GetType() == attrType).ToArray();
#else
            return propertyInfo.GetCustomAttributes(attrType, inherit);
#endif
        }

        /// <summary>A Type extension method that custom attributes.</summary>
        /// <param name="fieldInfo">The fieldInfo to act on.</param>
        /// <param name="inherit">  true to inherit.</param>
        /// <returns>An object[].</returns>
        public static object[] CustomAttributes(this FieldInfo fieldInfo, bool inherit = true)
        {
#if NETFX_CORE
            return fieldInfo.GetCustomAttributes(inherit).ToArray();
#else
            return fieldInfo.GetCustomAttributes(inherit);
#endif
        }

        /// <summary>A Type extension method that custom attributes.</summary>
        /// <param name="fieldInfo">The fieldInfo to act on.</param>
        /// <param name="attrType"> Type of the attribute.</param>
        /// <param name="inherit">  true to inherit.</param>
        /// <returns>An object[].</returns>
        public static object[] CustomAttributes(this FieldInfo fieldInfo, Type attrType, bool inherit = true)
        {
#if NETFX_CORE
            return fieldInfo.GetCustomAttributes(inherit).Where(x => x.GetType() == attrType).ToArray();
#else
            return fieldInfo.GetCustomAttributes(attrType, inherit);
#endif
        }

        /// <summary>A Type extension method that custom attributes.</summary>
        /// <param name="type">   The type to act on.</param>
        /// <param name="inherit">true to inherit.</param>
        /// <returns>An object[].</returns>
        public static object[] CustomAttributes(this Type type, bool inherit = true)
        {
#if NETFX_CORE
            return type.GetTypeInfo().GetCustomAttributes(inherit).ToArray();
#else
            return type.GetCustomAttributes(inherit);
#endif
        }

        /// <summary>A Type extension method that custom attributes.</summary>
        /// <param name="type">    The type to act on.</param>
        /// <param name="attrType">Type of the attribute.</param>
        /// <param name="inherit"> true to inherit.</param>
        /// <returns>An object[].</returns>
        public static object[] CustomAttributes(this Type type, Type attrType, bool inherit = true)
        {
#if NETFX_CORE
            return type.GetTypeInfo().GetCustomAttributes(inherit).Where(x => x.GetType() == attrType).ToArray();
#else
            return type.GetCustomAttributes(attrType, inherit);
#endif
        }

        /// <summary>A PropertyInfo extension method that first attribute.</summary>
        /// <typeparam name="TAttr">Type of the attribute.</typeparam>
        /// <param name="type">   The type to act on.</param>
        /// <param name="inherit">true to inherit.</param>
        /// <returns>A TAttribute.</returns>
        public static TAttr FirstAttribute<TAttr>(this Type type, bool inherit = true) where TAttr : Attribute
        {
#if NETFX_CORE
            return type.GetTypeInfo().GetCustomAttributes(typeof(TAttr), inherit)
                .FirstOrDefault() as TAttr;
#else
            return type.GetCustomAttributes(typeof(TAttr), inherit)
                   .FirstOrDefault() as TAttr;
#endif
        }

        /// <summary>A PropertyInfo extension method that first attribute.</summary>
        /// <typeparam name="TAttribute">Type of the attribute.</typeparam>
        /// <param name="propertyInfo">The propertyInfo to act on.</param>
        /// <returns>A TAttribute.</returns>
        public static TAttribute FirstAttribute<TAttribute>(this PropertyInfo propertyInfo)
            where TAttribute : Attribute
        {
            return propertyInfo.FirstAttribute<TAttribute>(true);
        }

        /// <summary>A PropertyInfo extension method that first attribute.</summary>
        /// <typeparam name="TAttribute">Type of the attribute.</typeparam>
        /// <param name="propertyInfo">The propertyInfo to act on.</param>
        /// <param name="inherit">     true to inherit.</param>
        /// <returns>A TAttribute.</returns>
        public static TAttribute FirstAttribute<TAttribute>(this PropertyInfo propertyInfo, bool inherit)
            where TAttribute : Attribute
        {
#if NETFX_CORE
            var attrs = propertyInfo.GetCustomAttributes<TAttribute>(inherit);
            return (TAttribute)(attrs.Count() > 0 ? attrs.ElementAt(0) : null);
#else
            var attrs = propertyInfo.GetCustomAttributes(typeof(TAttribute), inherit);
            return (TAttribute)(attrs.Length > 0 ? attrs[0] : null);
#endif
        }

        /// <summary>A Type extension method that first generic type definition.</summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>A Type.</returns>
        public static Type FirstGenericTypeDefinition(this Type type)
        {
            while (type != null)
            {
                if (type.HasGenericType())
                    return type.GenericTypeDefinition();

                type = type.BaseType();
            }

            return null;
        }

        /// <summary>An Assembly extension method that query if 'assembly' is dynamic.</summary>
        /// <param name="assembly">The assembly to act on.</param>
        /// <returns>true if dynamic, false if not.</returns>
        public static bool IsDynamic(this Assembly assembly)
        {
#if MONOTOUCH || WINDOWS_PHONE || NETFX_CORE
            return false;
#else
            try
            {
                var isDyanmic = assembly is System.Reflection.Emit.AssemblyBuilder
                    || string.IsNullOrEmpty(assembly.Location);
                return isDyanmic;
            }
            catch (NotSupportedException)
            {
                //Ignore assembly.Location not supported in a dynamic assembly.
                return true;
            }
#endif
        }

        /// <summary>A Type extension method that gets public static method.</summary>
        /// <param name="type">      The type to act on.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="types">     The types.</param>
        /// <returns>The public static method.</returns>
        public static MethodInfo GetPublicStaticMethod(this Type type, string methodName, Type[] types = null)
        {
#if NETFX_CORE
            return type.GetRuntimeMethod(methodName, types);
#else
            return types == null
                ? type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static)
                : type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static, null, types, null);
#endif
        }

        /// <summary>A PropertyInfo extension method that gets method information.</summary>
        /// <param name="type">      The type to act on.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="types">     The types.</param>
        /// <returns>The method information.</returns>
        public static MethodInfo GetMethodInfo(this Type type, string methodName, Type[] types = null)
        {
#if NETFX_CORE
            return type.GetRuntimeMethods().First(p => p.Name.Equals(methodName));
#else
            return types == null
                ? type.GetMethod(methodName)
                : type.GetMethod(methodName, types);
#endif
        }

        /// <summary>
        /// A Delegate extension method that executes the method on a different thread, and waits for the
        /// result.
        /// </summary>
        /// <param name="fn">        The fn to act on.</param>
        /// <param name="instance">  The instance.</param>
        /// <param name="parameters">Options for controlling the operation.</param>
        /// <returns>An object.</returns>
        public static object InvokeMethod(this Delegate fn, object instance, object[] parameters = null)
        {
#if NETFX_CORE
            return fn.GetMethodInfo().Invoke(instance, parameters ?? new object[] { });
#else
            return fn.Method.Invoke(instance, parameters ?? new object[] { });
#endif
        }

        /// <summary>A Type extension method that gets public static field.</summary>
        /// <param name="type">     The type to act on.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>The public static field.</returns>
        public static FieldInfo GetPublicStaticField(this Type type, string fieldName)
        {
#if NETFX_CORE
            return type.GetRuntimeField(fieldName);
#else
            return type.GetField(fieldName, BindingFlags.Public | BindingFlags.Static);
#endif
        }

        /// <summary>A MethodInfo extension method that makes a delegate.</summary>
        /// <param name="mi">                The mi to act on.</param>
        /// <param name="delegateType">      Type of the delegate.</param>
        /// <param name="throwOnBindFailure">true to throw on bind failure.</param>
        public static Delegate MakeDelegate(this MethodInfo mi, Type delegateType, bool throwOnBindFailure=true)
        {
#if NETFX_CORE
            return mi.CreateDelegate(delegateType);
#else
            return Delegate.CreateDelegate(delegateType, mi, throwOnBindFailure);
#endif
        }

        /// <summary>A Type extension method that generic type arguments.</summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>A Type[].</returns>
        public static Type[] GenericTypeArguments(this Type type)
        {
#if NETFX_CORE
            return type.GenericTypeArguments;
#else
            return type.GetGenericArguments();
#endif
        }

        /// <summary>A Type extension method that declared constructors.</summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>A ConstructorInfo[].</returns>
        public static ConstructorInfo[] DeclaredConstructors(this Type type)
        {
#if NETFX_CORE
            return type.GetTypeInfo().DeclaredConstructors.ToArray();
#else
            return type.GetConstructors();
#endif
        }

        /// <summary>A Type extension method that assignable from.</summary>
        /// <param name="type">    The type to act on.</param>
        /// <param name="fromType">Type of from.</param>
        /// <returns>true if it succeeds, false if it fails.</returns>
        public static bool AssignableFrom(this Type type, Type fromType)
        {
#if NETFX_CORE
            return type.GetTypeInfo().IsAssignableFrom(fromType.GetTypeInfo());
#else
            return type.IsAssignableFrom(fromType);
#endif
        }

        /// <summary>A Type extension method that query if 'type' is standard class.</summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>true if standard class, false if not.</returns>
        public static bool IsStandardClass(this Type type)
        {
#if NETFX_CORE
            var typeInfo = type.GetTypeInfo();
            return typeInfo.IsClass && !typeInfo.IsAbstract && !typeInfo.IsInterface;
#else
            return type.IsClass && !type.IsAbstract && !type.IsInterface;
#endif
        }

        /// <summary>A Type extension method that query if 'type' is abstract.</summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>true if abstract, false if not.</returns>
        public static bool IsAbstract(this Type type)
        {
#if NETFX_CORE
            return type.GetTypeInfo().IsAbstract;
#else
            return type.IsAbstract;
#endif
        }

        /// <summary>A Type extension method that gets property information.</summary>
        /// <param name="type">        The type to act on.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>The property information.</returns>
        public static PropertyInfo GetPropertyInfo(this Type type, string propertyName)
        {
#if NETFX_CORE
            return type.GetRuntimeProperty(propertyName);
#else
            return type.GetProperty(propertyName);
#endif
        }

        /// <summary>A Type extension method that gets field information.</summary>
        /// <param name="type">     The type to act on.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>The field information.</returns>
        public static FieldInfo GetFieldInfo(this Type type, string fieldName)
        {
#if NETFX_CORE
            return type.GetRuntimeField(fieldName);
#else
            return type.GetField(fieldName);
#endif
        }

        /// <summary>A Type extension method that gets writable fields.</summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>An array of field information.</returns>
        public static FieldInfo[] GetWritableFields(this Type type)
        {
#if NETFX_CORE
            return type.GetRuntimeFields().Where(p => !p.IsPublic && !p.IsStatic).ToArray();
#else
            return type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.SetField);
#endif
        }

        /// <summary>A PropertyInfo extension method that sets a method.</summary>
        /// <param name="pi">       The pi to act on.</param>
        /// <param name="nonPublic">true to non public.</param>
        /// <returns>A MethodInfo.</returns>
        public static MethodInfo SetMethod(this PropertyInfo pi, bool nonPublic = true)
        {
#if NETFX_CORE
            return pi.SetMethod;
#else
            return pi.GetSetMethod(nonPublic);
#endif
        }

        /// <summary>A PropertyInfo extension method that gets method information.</summary>
        /// <param name="pi">       The pi to act on.</param>
        /// <param name="nonPublic">true to non public.</param>
        /// <returns>The method information.</returns>
        public static MethodInfo GetMethodInfo(this PropertyInfo pi, bool nonPublic = true)
        {
#if NETFX_CORE
            return pi.GetMethod;
#else
            return pi.GetGetMethod(nonPublic);
#endif
        }

        /// <summary>A Type extension method that instance of type.</summary>
        /// <param name="type">    The type to act on.</param>
        /// <param name="instance">The instance.</param>
        /// <returns>true if it succeeds, false if it fails.</returns>
        public static bool InstanceOfType(this Type type, object instance)
        {
#if NETFX_CORE
            return type.IsInstanceOf(instance.GetType());
#else
            return type.IsInstanceOfType(instance);
#endif
        }

        /// <summary>A Type extension method that query if 'type' is class.</summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>true if class, false if not.</returns>
        public static bool IsClass(this Type type)
        {
#if NETFX_CORE
            return type.GetTypeInfo().IsClass;
#else
            return type.IsClass;
#endif
        }

        /// <summary>A Type extension method that query if 'type' is enum.</summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>true if enum, false if not.</returns>
        public static bool IsEnum(this Type type)
        {
#if NETFX_CORE
            return type.GetTypeInfo().IsEnum;
#else
            return type.IsEnum;
#endif
        }

        /// <summary>A Type extension method that query if 'type' is enum flags.</summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>true if enum flags, false if not.</returns>
        public static bool IsEnumFlags(this Type type)
        {
#if NETFX_CORE
            return type.GetTypeInfo().IsEnum && type.FirstAttribute<FlagsAttribute>(false) != null;
#else
            return type.IsEnum && type.FirstAttribute<FlagsAttribute>(false) != null;
#endif
        }

        /// <summary>A Type extension method that query if 'type' is underlying enum.</summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>true if underlying enum, false if not.</returns>
        public static bool IsUnderlyingEnum(this Type type)
        {
#if NETFX_CORE
            return type.GetTypeInfo().IsEnum;
#else
            return type.IsEnum || type.UnderlyingSystemType.IsEnum;
#endif
        }

        /// <summary>A Type extension method that gets method infos.</summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>An array of method information.</returns>
        public static MethodInfo[] GetMethodInfos(this Type type)
        {
#if NETFX_CORE
            return type.GetRuntimeMethods().ToArray();
#else
            return type.GetMethods();
#endif
        }

        /// <summary>A Type extension method that gets property infos.</summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>An array of property information.</returns>
        public static PropertyInfo[] GetPropertyInfos(this Type type)
        {
#if NETFX_CORE
            return type.GetRuntimeProperties().ToArray();
#else
            return type.GetProperties();
#endif
        }

#if SILVERLIGHT || NETFX_CORE
        /// <summary>A List&lt;T&gt; extension method that convert all.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <typeparam name="U">Generic type parameter.</typeparam>
        /// <param name="list">     The list to act on.</param>
        /// <param name="converter">The converter.</param>
        /// <returns>all converted.</returns>
        public static List<U> ConvertAll<T, U>(this List<T> list, Func<T, U> converter)
        {
            var result = new List<U>();
            foreach (var element in list)
            {
                result.Add(converter(element));
            }
            return result;
        }
#endif
 
    
    }

}