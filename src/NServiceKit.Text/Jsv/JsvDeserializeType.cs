using System;
using System.Reflection;
using NServiceKit.Text.Common;

namespace NServiceKit.Text.Jsv
{
    /// <summary>A jsv deserialize type.</summary>
	public static class JsvDeserializeType
	{
        /// <summary>Gets set property method.</summary>
        /// <param name="type">        The type.</param>
        /// <param name="propertyInfo">Information describing the property.</param>
        /// <returns>The set property method.</returns>
		public static SetPropertyDelegate GetSetPropertyMethod(Type type, PropertyInfo propertyInfo)
		{
			return TypeAccessor.GetSetPropertyMethod(type, propertyInfo);
		}

        /// <summary>Gets set field method.</summary>
        /// <param name="type">     The type.</param>
        /// <param name="fieldInfo">Information describing the field.</param>
        /// <returns>The set field method.</returns>
		public static SetPropertyDelegate GetSetFieldMethod(Type type, FieldInfo fieldInfo)
		{
			return TypeAccessor.GetSetFieldMethod(type, fieldInfo);
		}
	}
}