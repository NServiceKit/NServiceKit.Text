using System;
using System.Linq;
using System.Reflection;
#if !SILVERLIGHT && !MONOTOUCH
using System.Reflection.Emit;

namespace NServiceKit.Text {

    /// <summary>A dynamic proxy.</summary>
	public static class DynamicProxy {

        /// <summary>Gets instance for.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <returns>The instance for.</returns>
		public static T GetInstanceFor<T> () {
			return (T)GetInstanceFor(typeof(T));
		}

        /// <summary>The module builder.</summary>
		static readonly ModuleBuilder ModuleBuilder;

        /// <summary>The dynamic assembly.</summary>
		static readonly AssemblyBuilder DynamicAssembly;

        /// <summary>Gets instance for.</summary>
        /// <param name="targetType">Type of the target.</param>
        /// <returns>The instance for.</returns>
		public static object GetInstanceFor (Type targetType) {
			lock (DynamicAssembly)
			{
				var constructedType = DynamicAssembly.GetType(ProxyName(targetType)) ?? GetConstructedType(targetType);
				var instance = Activator.CreateInstance(constructedType);
				return instance;
			}
		}

        /// <summary>Proxy name.</summary>
        /// <param name="targetType">Type of the target.</param>
        /// <returns>A string.</returns>
		static string ProxyName(Type targetType)
		{
			return targetType.Name + "Proxy";
		}

        /// <summary>Initializes static members of the NServiceKit.Text.DynamicProxy class.</summary>
		static DynamicProxy () {
			var assemblyName = new AssemblyName("DynImpl");
			DynamicAssembly = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
			ModuleBuilder = DynamicAssembly.DefineDynamicModule("DynImplModule");
		}

        /// <summary>Gets constructed type.</summary>
        /// <param name="targetType">Type of the target.</param>
        /// <returns>The constructed type.</returns>
		static Type GetConstructedType (Type targetType) {
			var typeBuilder = ModuleBuilder.DefineType(targetType.Name + "Proxy", TypeAttributes.Public);

			var ctorBuilder = typeBuilder.DefineConstructor(
				MethodAttributes.Public,
				CallingConventions.Standard,
				new Type[] { });
			var ilGenerator = ctorBuilder.GetILGenerator();
			ilGenerator.Emit(OpCodes.Ret);

			IncludeType(targetType, typeBuilder);

			foreach (var face in targetType.GetInterfaces())
				IncludeType(face, typeBuilder);

			return typeBuilder.CreateType();
		}

        /// <summary>Include type.</summary>
        /// <param name="typeOfT">    Type of the t.</param>
        /// <param name="typeBuilder">The type builder.</param>
		static void IncludeType (Type typeOfT, TypeBuilder typeBuilder) {
			var methodInfos = typeOfT.GetMethods();
			foreach (var methodInfo in methodInfos) {
                if (methodInfo.Name.StartsWith("set_", StringComparison.Ordinal)) continue; // we always add a set for a get.

                if (methodInfo.Name.StartsWith("get_", StringComparison.Ordinal))
                {
					BindProperty(typeBuilder, methodInfo);
				} else {
					BindMethod(typeBuilder, methodInfo);
				}
			}

			typeBuilder.AddInterfaceImplementation(typeOfT);
		}

        /// <summary>Bind method.</summary>
        /// <param name="typeBuilder">The type builder.</param>
        /// <param name="methodInfo"> Information describing the method.</param>
		static void BindMethod (TypeBuilder typeBuilder, MethodInfo methodInfo) {
			var methodBuilder = typeBuilder.DefineMethod(
				methodInfo.Name,
				MethodAttributes.Public | MethodAttributes.Virtual,
				methodInfo.ReturnType,
				methodInfo.GetParameters().Select(p => p.GetType()).ToArray()
				);
			var methodILGen = methodBuilder.GetILGenerator();
			if (methodInfo.ReturnType == typeof(void)) {
				methodILGen.Emit(OpCodes.Ret);
			} else {
				if (methodInfo.ReturnType.IsValueType || methodInfo.ReturnType.IsEnum) {
					MethodInfo getMethod = typeof(Activator).GetMethod("CreateInstance",
																	   new[] { typeof(Type) });
					LocalBuilder lb = methodILGen.DeclareLocal(methodInfo.ReturnType);
					methodILGen.Emit(OpCodes.Ldtoken, lb.LocalType);
					methodILGen.Emit(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle"));
					methodILGen.Emit(OpCodes.Callvirt, getMethod);
					methodILGen.Emit(OpCodes.Unbox_Any, lb.LocalType);
				} else {
					methodILGen.Emit(OpCodes.Ldnull);
				}
				methodILGen.Emit(OpCodes.Ret);
			}
			typeBuilder.DefineMethodOverride(methodBuilder, methodInfo);
		}

        /// <summary>Bind property.</summary>
        /// <param name="typeBuilder">The type builder.</param>
        /// <param name="methodInfo"> Information describing the method.</param>
		public static void BindProperty (TypeBuilder typeBuilder, MethodInfo methodInfo) {
			// Backing Field
			string propertyName = methodInfo.Name.Replace("get_", "");
			Type propertyType = methodInfo.ReturnType;
			FieldBuilder backingField = typeBuilder.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);

			//Getter
			MethodBuilder backingGet = typeBuilder.DefineMethod("get_" + propertyName, MethodAttributes.Public |
				MethodAttributes.SpecialName | MethodAttributes.Virtual |
				MethodAttributes.HideBySig, propertyType, Type.EmptyTypes);
			ILGenerator getIl = backingGet.GetILGenerator();

			getIl.Emit(OpCodes.Ldarg_0);
			getIl.Emit(OpCodes.Ldfld, backingField);
			getIl.Emit(OpCodes.Ret);


			//Setter
			MethodBuilder backingSet = typeBuilder.DefineMethod("set_" + propertyName, MethodAttributes.Public |
				MethodAttributes.SpecialName | MethodAttributes.Virtual |
				MethodAttributes.HideBySig, null, new[] { propertyType });

			ILGenerator setIl = backingSet.GetILGenerator();

			setIl.Emit(OpCodes.Ldarg_0);
			setIl.Emit(OpCodes.Ldarg_1);
			setIl.Emit(OpCodes.Stfld, backingField);
			setIl.Emit(OpCodes.Ret);

			// Property
			PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.None, propertyType, null);
			propertyBuilder.SetGetMethod(backingGet);
			propertyBuilder.SetSetMethod(backingSet);
		}
	}
}
#endif
