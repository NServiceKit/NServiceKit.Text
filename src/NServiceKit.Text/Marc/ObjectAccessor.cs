using System;
//using System.Dynamic;

//Not using it here, but @marcgravell's stuff is too good not to include
#if !SILVERLIGHT && !MONOTOUCH && !XBOX
namespace NServiceKit.Text.FastMember
{
    /// <summary>Represents an individual object, allowing access to members by-name.</summary>
    public abstract class ObjectAccessor
    {
        /// <summary>Get or Set the value of a named member for the underlying object.</summary>
        /// <param name="name">The name.</param>
        /// <returns>The indexed item.</returns>
        public abstract object this[string name] { get; set; }

        /// <summary>The object represented by this instance.</summary>
        /// <value>The target.</value>
        public abstract object Target { get; }

        /// <summary>Use the target types definition of equality.</summary>
        /// <param name="obj">The <see cref="T:System.Object" /> to compare with the current
        /// <see cref="T:System.Object" />.</param>
        /// <returns>true if the objects are considered equal, false if they are not.</returns>
        public override bool Equals(object obj)
        {
            return Target.Equals(obj);
        }

        /// <summary>Obtain the hash of the target object.</summary>
        /// <returns>A hash code for this object.</returns>
        public override int GetHashCode()
        {
            return Target.GetHashCode();
        }

        /// <summary>Use the target's definition of a string representation.</summary>
        /// <returns>A string that represents this object.</returns>
        public override string ToString()
        {
            return Target.ToString();
        }

        /// <summary>Wraps an individual object, allowing by-name access to that instance.</summary>
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        /// <param name="target">The object represented by this instance.</param>
        /// <returns>An ObjectAccessor.</returns>
        public static ObjectAccessor Create(object target)
        {
            if (target == null) throw new ArgumentNullException("target");
            //IDynamicMetaObjectProvider dlr = target as IDynamicMetaObjectProvider;
            //if (dlr != null) return new DynamicWrapper(dlr); // use the DLR
            return new TypeAccessorWrapper(target, TypeAccessor.Create(target.GetType()));
        }

        /// <summary>A type accessor wrapper.</summary>
        sealed class TypeAccessorWrapper : ObjectAccessor
        {
            /// <summary>Target for the.</summary>
            private readonly object target;

            /// <summary>The accessor.</summary>
            private readonly TypeAccessor accessor;

            /// <summary>
            /// Initializes a new instance of the
            /// NServiceKit.Text.FastMember.ObjectAccessor.TypeAccessorWrapper class.
            /// </summary>
            /// <param name="target">  The target.</param>
            /// <param name="accessor">The accessor.</param>
            public TypeAccessorWrapper(object target, TypeAccessor accessor)
            {
                this.target = target;
                this.accessor = accessor;
            }

            /// <summary>Get or Set the value of a named member for the underlying object.</summary>
            /// <param name="name">The name.</param>
            /// <returns>The indexed item.</returns>
            public override object this[string name]
            {
                get { return accessor[target, name.ToUpperInvariant()]; }
				set { accessor[target, name.ToUpperInvariant()] = value; }
            }

            /// <summary>The object represented by this instance.</summary>
            /// <value>The target.</value>
            public override object Target
            {
                get { return target; }
            }
        }

		//sealed class DynamicWrapper : ObjectAccessor
		//{
		//    private readonly IDynamicMetaObjectProvider target;
		//    public override object Target
		//    {
		//        get { return target; }
		//    }
		//    public DynamicWrapper(IDynamicMetaObjectProvider target)
		//    {
		//        this.target = target;
		//    }
		//    public override object this[string name]
		//    {
		//        get { return CallSiteCache.GetValue(name, target); }
		//        set { CallSiteCache.SetValue(name, target, value); }
		//    }
        //}
    }

}

#endif