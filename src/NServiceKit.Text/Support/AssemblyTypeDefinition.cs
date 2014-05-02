using System;

namespace NServiceKit.Common.Support
{
    /// <summary>An assembly type definition.</summary>
	internal class AssemblyTypeDefinition
	{
        /// <summary>The type definition seperator.</summary>
		private const char TypeDefinitionSeperator = ',';

        /// <summary>Zero-based index of the type name.</summary>
		private const int TypeNameIndex = 0;

        /// <summary>Zero-based index of the assembly name.</summary>
		private const int AssemblyNameIndex = 1;

        /// <summary>
        /// Initializes a new instance of the NServiceKit.Common.Support.AssemblyTypeDefinition class.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        /// <param name="typeDefinition">The type definition.</param>
		public AssemblyTypeDefinition(string typeDefinition)
		{
			if (string.IsNullOrEmpty(typeDefinition))
			{
				throw new ArgumentNullException();
			}
			var parts = typeDefinition.Split(TypeDefinitionSeperator);
			TypeName = parts[TypeNameIndex].Trim();
			AssemblyName = (parts.Length > AssemblyNameIndex) ? parts[AssemblyNameIndex].Trim() : null;
		}

        /// <summary>Gets or sets the name of the type.</summary>
        /// <value>The name of the type.</value>
		public string TypeName { get; set; }

        /// <summary>Gets or sets the name of the assembly.</summary>
        /// <value>The name of the assembly.</value>
		public string AssemblyName { get; set; }
	}
}