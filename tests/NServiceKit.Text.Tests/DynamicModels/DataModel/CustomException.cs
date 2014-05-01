using System;
using System.Runtime.Serialization;

namespace NServiceKit.Text.Tests.DynamicModels.DataModel
{
    /// <summary>Exception for signalling custom errors.</summary>
	public class CustomException
		: Exception
	{
        /// <summary>
        /// Initializes a new instance of the
        /// NServiceKit.Text.Tests.DynamicModels.DataModel.CustomException class.
        /// </summary>
		public CustomException()
		{
		}

        /// <summary>
        /// Initializes a new instance of the
        /// NServiceKit.Text.Tests.DynamicModels.DataModel.CustomException class.
        /// </summary>
        /// <param name="message">The message.</param>
		public CustomException(string message) : base(message)
		{
		}

        /// <summary>
        /// Initializes a new instance of the
        /// NServiceKit.Text.Tests.DynamicModels.DataModel.CustomException class.
        /// </summary>
        /// <param name="message">       The message.</param>
        /// <param name="innerException">The inner exception.</param>
		public CustomException(string message, Exception innerException) : base(message, innerException)
		{
		}

        /// <summary>
        /// Initializes a new instance of the
        /// NServiceKit.Text.Tests.DynamicModels.DataModel.CustomException class.
        /// </summary>
        /// <param name="info">   The information.</param>
        /// <param name="context">The context.</param>
		protected CustomException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}