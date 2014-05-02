using System;

namespace NServiceKit.Text
{
    /// <summary>Values that represent CsvBehavior.</summary>
    public enum CsvBehavior
    {
        /// <summary>An enum constant representing the first enumerable option.</summary>
        FirstEnumerable
    }

    /// <summary>Attribute for csv.</summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
    public class CsvAttribute : Attribute
    {
        /// <summary>Gets or sets the CSV behavior.</summary>
        /// <value>The CSV behavior.</value>
        public CsvBehavior CsvBehavior { get; set; }

        /// <summary>Initializes a new instance of the NServiceKit.Text.CsvAttribute class.</summary>
        /// <param name="csvBehavior">The CSV behavior.</param>
        public CsvAttribute(CsvBehavior csvBehavior)
        {
            this.CsvBehavior = csvBehavior;
        }
    }
}