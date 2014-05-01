using System;
using System.Linq;
using System.Reflection;

namespace NServiceKit.Text
{
    /// <summary>A type configuration.</summary>
	internal class TypeConfig
	{
        /// <summary>The type.</summary>
		internal readonly Type Type;

        /// <summary>true to enable, false to disable the anonymous field setterses.</summary>
		internal bool EnableAnonymousFieldSetterses;

        /// <summary>The properties.</summary>
		internal PropertyInfo[] Properties;

        /// <summary>The fields.</summary>
		internal FieldInfo[] Fields;

        /// <summary>Initializes a new instance of the NServiceKit.Text.TypeConfig class.</summary>
        /// <param name="type">The type.</param>
		internal TypeConfig(Type type)
		{
			Type = type;
			EnableAnonymousFieldSetterses = false;
			Properties = new PropertyInfo[0];
			Fields = new FieldInfo[0];
		}
	}

    /// <summary>A type configuration.</summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
	public static class TypeConfig<T>
	{
        /// <summary>The configuration.</summary>
		private static readonly TypeConfig config;

        /// <summary>Gets or sets the properties.</summary>
        /// <value>The properties.</value>
		public static PropertyInfo[] Properties
		{
			get { return config.Properties; }
			set { config.Properties = value; }
		}

        /// <summary>Gets or sets the fields.</summary>
        /// <value>The fields.</value>
		public static FieldInfo[] Fields
		{
			get { return config.Fields; }
			set { config.Fields = value; }
		}

        /// <summary>
        /// Gets or sets a value indicating whether the anonymous field setters is enabled.
        /// </summary>
        /// <value>true if enable anonymous field setters, false if not.</value>
		public static bool EnableAnonymousFieldSetters
		{
			get { return config.EnableAnonymousFieldSetterses; }
			set { config.EnableAnonymousFieldSetterses = value; }
		}

        /// <summary>
        /// Initializes static members of the NServiceKit.Text.TypeConfig&lt;T&gt; class.
        /// </summary>
		static TypeConfig()
		{
			config = new TypeConfig(typeof(T));
			
			var excludedProperties = JsConfig<T>.ExcludePropertyNames ?? new string[0];

            var properties = excludedProperties.Any()
                ? config.Type.GetSerializableProperties().Where(x => !excludedProperties.Contains(x.Name))
                : config.Type.GetSerializableProperties();
            Properties = properties.Where(x => x.GetIndexParameters().Length == 0).ToArray();

			Fields = config.Type.GetSerializableFields().ToArray();
		}

        /// <summary>Gets the state.</summary>
        /// <returns>The state.</returns>
		internal static TypeConfig GetState()
		{
			return config;
		}
	}
}