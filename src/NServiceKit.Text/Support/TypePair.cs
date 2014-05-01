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

namespace NServiceKit.Text.Support
{
    /// <summary>A type pair.</summary>
	public class TypePair
	{
        /// <summary>Gets or sets the arguments 1.</summary>
        /// <value>The arguments 1.</value>
		public Type[] Args1 { get; set; }

        /// <summary>Gets or sets the argument 2.</summary>
        /// <value>The argument 2.</value>
		public Type[] Arg2 { get; set; }

        /// <summary>Initializes a new instance of the NServiceKit.Text.Support.TypePair class.</summary>
        /// <param name="arg1">The first argument.</param>
        /// <param name="arg2">The second argument.</param>
		public TypePair(Type[] arg1, Type[] arg2)
		{
			Args1 = arg1;
			Arg2 = arg2;
		}

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object" /> is equal to the current
        /// <see cref="T:System.Object" />.
        /// </summary>
        /// <param name="other">The type pair to compare to this object.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object" /> is equal to the current
        /// <see cref="T:System.Object" />; otherwise, false.
        /// </returns>
		public bool Equals(TypePair other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.Args1, Args1) && Equals(other.Arg2, Arg2);
		}

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object" /> is equal to the current
        /// <see cref="T:System.Object" />.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object" /> to compare with the current
        /// <see cref="T:System.Object" />.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object" /> is equal to the current
        /// <see cref="T:System.Object" />; otherwise, false.
        /// </returns>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof (TypePair)) return false;
			return Equals((TypePair) obj);
		}

        /// <summary>Serves as a hash function for a particular type.</summary>
        /// <returns>A hash code for the current <see cref="T:System.Object" />.</returns>
		public override int GetHashCode()
		{
			unchecked
			{
				return ((Args1 != null ? Args1.GetHashCode() : 0)*397) ^ (Arg2 != null ? Arg2.GetHashCode() : 0);
			}
		}
	}
}