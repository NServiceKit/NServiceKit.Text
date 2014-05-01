using System;
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
//using System.Dynamic;

//Not using it here, but @marcgravell's stuff is too good not to include
// http://code.google.com/p/fast-member/ Apache License 2.0
#if !SILVERLIGHT && !MONOTOUCH && !XBOX
namespace NServiceKit.Text.FastMember
{
    /// <summary>Provides by-name member-access to objects of a given type.</summary>
    public abstract class TypeAccessor
    {
        /// <summary>hash-table has better read-without-locking semantics than dictionary.</summary>
        private static readonly Hashtable typeLookyp = new Hashtable();

        /// <summary>Does this type support new instances via a parameterless constructor?</summary>
        /// <value>true if create new supported, false if not.</value>
        public virtual bool CreateNewSupported { get { return false; } }

        /// <summary>Create a new instance of this type.</summary>
        /// <exception cref="NotSupportedException">Thrown when the requested operation is not supported.</exception>
        /// <returns>The new new.</returns>
        public virtual object CreateNew() { throw new NotSupportedException();}

        /// <summary>
        /// Provides a type-specific accessor, allowing by-name access for all objects of that type.
        /// </summary>
        /// <remarks>
        /// The accessor is cached internally; a pre-existing accessor may be returned.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        /// <param name="type">The type.</param>
        /// <returns>A TypeAccessor.</returns>
        public static TypeAccessor Create(Type type)
        {
            if(type == null) throw new ArgumentNullException("type");
            TypeAccessor obj = (TypeAccessor)typeLookyp[type];
            if (obj != null) return obj;

            lock(typeLookyp)
            {
                // double-check
                obj = (TypeAccessor)typeLookyp[type];
                if (obj != null) return obj;

                obj = CreateNew(type);

                typeLookyp[type] = obj;
                return obj;
            }
        }

		//sealed class DynamicAccessor : TypeAccessor
		//{
		//    public static readonly DynamicAccessor Singleton = new DynamicAccessor();
		//    private DynamicAccessor(){}
		//    public override object this[object target, string name]
		//    {
		//        get { return CallSiteCache.GetValue(name, target); }
		//        set { CallSiteCache.SetValue(name, target, value); }
		//    }
		//}

        /// <summary>The assembly.</summary>
        private static AssemblyBuilder assembly;

        /// <summary>Gets the module.</summary>
        /// <value>The module.</value>
        private static ModuleBuilder module;

        /// <summary>The counter.</summary>
        private static int counter;

        /// <summary>Writes a getter.</summary>
        /// <param name="il">      The il.</param>
        /// <param name="type">    The type.</param>
        /// <param name="props">   The properties.</param>
        /// <param name="fields">  The fields.</param>
        /// <param name="isStatic">true if this object is static.</param>
        private static void WriteGetter(ILGenerator il, Type type, PropertyInfo[] props, FieldInfo[] fields, bool isStatic)
        {
            LocalBuilder loc = type.IsValueType ? il.DeclareLocal(type) : null;
            OpCode propName = isStatic ? OpCodes.Ldarg_1 : OpCodes.Ldarg_2, target = isStatic ? OpCodes.Ldarg_0 : OpCodes.Ldarg_1;
            foreach (PropertyInfo prop in props)
            {
                if (prop.GetIndexParameters().Length != 0 || !prop.CanRead) continue;

                Label next = il.DefineLabel();
                il.Emit(propName);
                il.Emit(OpCodes.Ldstr, prop.Name);
                il.EmitCall(OpCodes.Call, strinqEquals, null);
                il.Emit(OpCodes.Brfalse_S, next);
                // match:
                il.Emit(target);
                Cast(il, type, loc);
                il.EmitCall(type.IsValueType ? OpCodes.Call : OpCodes.Callvirt, prop.GetGetMethod(), null);
                if (prop.PropertyType.IsValueType)
                {
                    il.Emit(OpCodes.Box, prop.PropertyType);
                }
                il.Emit(OpCodes.Ret);
                // not match:
                il.MarkLabel(next);
            }
            foreach (FieldInfo field in fields)
            {
                Label next = il.DefineLabel();
                il.Emit(propName);
                il.Emit(OpCodes.Ldstr, field.Name);
                il.EmitCall(OpCodes.Call, strinqEquals, null);
                il.Emit(OpCodes.Brfalse_S, next);
                // match:
                il.Emit(target);
                Cast(il, type, loc);
                il.Emit(OpCodes.Ldfld, field);
                if (field.FieldType.IsValueType)
                {
                    il.Emit(OpCodes.Box, field.FieldType);
                }
                il.Emit(OpCodes.Ret);
                // not match:
                il.MarkLabel(next);
            }
            il.Emit(OpCodes.Ldstr, "name");
            il.Emit(OpCodes.Newobj, typeof(ArgumentOutOfRangeException).GetConstructor(new Type[] { typeof(string) }));
            il.Emit(OpCodes.Throw);
        }

        /// <summary>Writes a setter.</summary>
        /// <param name="il">      The il.</param>
        /// <param name="type">    The type.</param>
        /// <param name="props">   The properties.</param>
        /// <param name="fields">  The fields.</param>
        /// <param name="isStatic">true if this object is static.</param>
        private static void WriteSetter(ILGenerator il, Type type, PropertyInfo[] props, FieldInfo[] fields, bool isStatic)
        {
            if (type.IsValueType)
            {
                il.Emit(OpCodes.Ldstr, "Write is not supported for structs");
                il.Emit(OpCodes.Newobj, typeof(NotSupportedException).GetConstructor(new Type[] { typeof(string) }));
                il.Emit(OpCodes.Throw);
            }
            else
            {
                OpCode propName = isStatic ? OpCodes.Ldarg_1 : OpCodes.Ldarg_2,
                       target = isStatic ? OpCodes.Ldarg_0 : OpCodes.Ldarg_1,
                       value = isStatic ? OpCodes.Ldarg_2 : OpCodes.Ldarg_3;
                LocalBuilder loc = type.IsValueType ? il.DeclareLocal(type) : null;
                foreach (PropertyInfo prop in props)
                {
                    if (prop.GetIndexParameters().Length != 0 || !prop.CanWrite) continue;

                    Label next = il.DefineLabel();
                    il.Emit(propName);
                    il.Emit(OpCodes.Ldstr, prop.Name);
                    il.EmitCall(OpCodes.Call, strinqEquals, null);
                    il.Emit(OpCodes.Brfalse_S, next);
                    // match:
                    il.Emit(target);
                    Cast(il, type, loc);
                    il.Emit(value);
                    Cast(il, prop.PropertyType, null);
                    il.EmitCall(type.IsValueType ? OpCodes.Call : OpCodes.Callvirt, prop.GetSetMethod(), null);
                    il.Emit(OpCodes.Ret);
                    // not match:
                    il.MarkLabel(next);
                }
                foreach (FieldInfo field in fields)
                {
                    Label next = il.DefineLabel();
                    il.Emit(propName);
                    il.Emit(OpCodes.Ldstr, field.Name);
                    il.EmitCall(OpCodes.Call, strinqEquals, null);
                    il.Emit(OpCodes.Brfalse_S, next);
                    // match:
                    il.Emit(target);
                    Cast(il, type, loc);
                    il.Emit(value);
                    Cast(il, field.FieldType, null);
                    il.Emit(OpCodes.Stfld, field);
                    il.Emit(OpCodes.Ret);
                    // not match:
                    il.MarkLabel(next);
                }
                il.Emit(OpCodes.Ldstr, "name");
                il.Emit(OpCodes.Newobj, typeof(ArgumentOutOfRangeException).GetConstructor(new Type[] { typeof(string) }));
                il.Emit(OpCodes.Throw);
            }
        }

        /// <summary>The strinq equals.</summary>
        private static readonly MethodInfo strinqEquals = typeof(string).GetMethod("op_Equality", new Type[] { typeof(string), typeof(string) });

        /// <summary>A delegate accessor.</summary>
        sealed class DelegateAccessor : TypeAccessor
        {
            /// <summary>The getter.</summary>
            private readonly Func<object, string, object> getter;

            /// <summary>The setter.</summary>
            private readonly Action<object, string, object> setter;

            /// <summary>The constructor.</summary>
            private readonly Func<object> ctor;

            /// <summary>
            /// Initializes a new instance of the NServiceKit.Text.FastMember.TypeAccessor.DelegateAccessor
            /// class.
            /// </summary>
            /// <param name="getter">The getter.</param>
            /// <param name="setter">The setter.</param>
            /// <param name="ctor">  The constructor.</param>
            public DelegateAccessor(Func<object, string, object> getter, Action<object, string, object> setter, Func<object> ctor)
            {
                this.getter = getter;
                this.setter = setter;
                this.ctor = ctor;
            }

            /// <summary>Does this type support new instances via a parameterless constructor?</summary>
            /// <value>true if create new supported, false if not.</value>
            public override bool CreateNewSupported { get { return ctor != null; } }

            /// <summary>Create a new instance of this type.</summary>
            /// <returns>The new new.</returns>
            public override object CreateNew()
            {
                return ctor != null ? ctor() : base.CreateNew();
            }

            /// <summary>Get or set the value of a named member on the target instance.</summary>
            /// <param name="target">Target for the.</param>
            /// <param name="name">  The name.</param>
            /// <returns>The indexed item.</returns>
            public override object this[object target, string name]
            {
                get { return getter(target, name); }
                set { setter(target, name, value); }
            }
        }

        /// <summary>Query if 'type' is fully public.</summary>
        /// <param name="type">The type.</param>
        /// <returns>true if fully public, false if not.</returns>
        private static bool IsFullyPublic(Type type)
        {
            while (type.IsNestedPublic) type = type.DeclaringType;
            return type.IsPublic;
        }

        /// <summary>Create a new instance of this type.</summary>
        /// <param name="type">The type.</param>
        /// <returns>The new new.</returns>
        static TypeAccessor CreateNew(Type type)
        {
			//if (typeof(IDynamicMetaObjectProvider).IsAssignableFrom(type))
			//{
			//    return DynamicAccessor.Singleton;
			//}

            PropertyInfo[] props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            ConstructorInfo ctor = null;
            if(type.IsClass && !type.IsAbstract)
            {
                ctor = type.GetConstructor(Type.EmptyTypes);
            }
            ILGenerator il;
            if(!IsFullyPublic(type))
            {
                DynamicMethod dynGetter = new DynamicMethod(type.FullName + "_get", typeof(object), new Type[] { typeof(object), typeof(string) }, type, true),
                              dynSetter = new DynamicMethod(type.FullName + "_set", null, new Type[] { typeof(object), typeof(string), typeof(object) }, type, true);
                WriteGetter(dynGetter.GetILGenerator(), type, props, fields, true);
                WriteSetter(dynSetter.GetILGenerator(), type, props, fields, true);
                DynamicMethod dynCtor = null;
                if(ctor != null)
                {
                    dynCtor = new DynamicMethod(type.FullName + "_ctor", typeof(object), Type.EmptyTypes, type, true);
                    il = dynCtor.GetILGenerator();
                    il.Emit(OpCodes.Newobj, ctor);
                    il.Emit(OpCodes.Ret);
                }
                return new DelegateAccessor(
                    (Func<object,string,object>)dynGetter.CreateDelegate(typeof(Func<object,string,object>)),
                    (Action<object,string,object>)dynSetter.CreateDelegate(typeof(Action<object,string,object>)),
                    dynCtor == null ? null : (Func<object>)dynCtor.CreateDelegate(typeof(Func<object>)));
            }

            // note this region is synchronized; only one is being created at a time so we don't need to stress about the builders
            if(assembly == null)
            {
                AssemblyName name = new AssemblyName("FastMember_dynamic");
                assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
                module = assembly.DefineDynamicModule(name.Name);
            }
            TypeBuilder tb = module.DefineType("FastMember_dynamic." + type.Name + "_" + Interlocked.Increment(ref counter),
                (typeof(TypeAccessor).Attributes | TypeAttributes.Sealed) & ~TypeAttributes.Abstract, typeof(TypeAccessor) );

            tb.DefineDefaultConstructor(MethodAttributes.Public);
            PropertyInfo indexer = typeof (TypeAccessor).GetProperty("Item");
            MethodInfo baseGetter = indexer.GetGetMethod(), baseSetter = indexer.GetSetMethod();
            MethodBuilder body = tb.DefineMethod(baseGetter.Name, baseGetter.Attributes & ~MethodAttributes.Abstract, typeof(object), new Type[] {typeof(object), typeof(string)});
            il = body.GetILGenerator();
            WriteGetter(il, type, props, fields, false);
            tb.DefineMethodOverride(body, baseGetter);

            body = tb.DefineMethod(baseSetter.Name, baseSetter.Attributes & ~MethodAttributes.Abstract, null, new Type[] { typeof(object), typeof(string), typeof(object) });
            il = body.GetILGenerator();
            WriteSetter(il, type, props, fields, false);
            tb.DefineMethodOverride(body, baseSetter);

            if(ctor != null)
            {
                MethodInfo baseMethod = typeof (TypeAccessor).GetProperty("CreateNewSupported").GetGetMethod();
                body = tb.DefineMethod(baseMethod.Name, baseMethod.Attributes, typeof (bool), Type.EmptyTypes);
                il = body.GetILGenerator();
                il.Emit(OpCodes.Ldc_I4_1);
                il.Emit(OpCodes.Ret);
                tb.DefineMethodOverride(body, baseMethod);

                baseMethod = typeof (TypeAccessor).GetMethod("CreateNew");
                body = tb.DefineMethod(baseMethod.Name, baseMethod.Attributes, typeof (object), Type.EmptyTypes);
                il = body.GetILGenerator();
                il.Emit(OpCodes.Newobj, ctor);
                il.Emit(OpCodes.Ret);
                tb.DefineMethodOverride(body, baseMethod);
            }

            return (TypeAccessor)Activator.CreateInstance(tb.CreateType());
        }

        /// <summary>Casts.</summary>
        /// <param name="il">  The il.</param>
        /// <param name="type">The type.</param>
        /// <param name="addr">The address.</param>
        private static void Cast(ILGenerator il, Type type, LocalBuilder addr)
        {
            if(type == typeof(object)) {}
            else if(type.IsValueType)
            {
                il.Emit(OpCodes.Unbox_Any, type);
                if (addr != null)
                {
                    il.Emit(OpCodes.Stloc, addr);
                    il.Emit(OpCodes.Ldloca_S, addr);
                }
            }
            else
            {
                il.Emit(OpCodes.Castclass, type);
            }
        }

        /// <summary>Get or set the value of a named member on the target instance.</summary>
        /// <param name="target">Target for the.</param>
        /// <param name="name">  The name.</param>
        /// <returns>The indexed item.</returns>
        public abstract object this[object target, string name]
        {
            get; set;
        }
    }
}

#endif


