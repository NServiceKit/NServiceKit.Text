using System;

namespace NServiceKit.Text
{
    /// <summary>Interface for tracer.</summary>
	public interface ITracer
	{
        /// <summary>Writes a debug.</summary>
        /// <param name="error">The error.</param>
        void WriteDebug(string error);

        /// <summary>Writes a debug.</summary>
        /// <param name="format">Describes the format to use.</param>
        /// <param name="args">  A variable-length parameters list containing arguments.</param>
        void WriteDebug(string format, params object[] args);

        /// <summary>Writes a warning.</summary>
        /// <param name="warning">The warning.</param>
        void WriteWarning(string warning);

        /// <summary>Writes a warning.</summary>
        /// <param name="format">Describes the format to use.</param>
        /// <param name="args">  A variable-length parameters list containing arguments.</param>
        void WriteWarning(string format, params object[] args);

        /// <summary>Writes an error.</summary>
        /// <param name="ex">The ex.</param>
		void WriteError(Exception ex);

        /// <summary>Writes an error.</summary>
        /// <param name="error">The error.</param>
		void WriteError(string error);

        /// <summary>Writes an error.</summary>
        /// <param name="format">Describes the format to use.</param>
        /// <param name="args">  A variable-length parameters list containing arguments.</param>
		void WriteError(string format, params object[] args);
	}
}