using System;

namespace NServiceKit.Text
{
    /// <summary>A tracer.</summary>
	public class Tracer
	{
        /// <summary>The instance.</summary>
		public static ITracer Instance = new NullTracer();

        /// <summary>A null tracer.</summary>
		public class NullTracer : ITracer
		{
            /// <summary>Writes a debug.</summary>
            /// <param name="error">The error.</param>
			public void WriteDebug(string error) { }

            /// <summary>Writes a debug.</summary>
            /// <param name="format">Describes the format to use.</param>
            /// <param name="args">  A variable-length parameters list containing arguments.</param>
			public void WriteDebug(string format, params object[] args) { }

            /// <summary>Writes a warning.</summary>
            /// <param name="warning">The warning.</param>
            public void WriteWarning(string warning) { }

            /// <summary>Writes a warning.</summary>
            /// <param name="format">Describes the format to use.</param>
            /// <param name="args">  A variable-length parameters list containing arguments.</param>
		    public void WriteWarning(string format, params object[] args) { }

            /// <summary>Writes an error.</summary>
            /// <param name="ex">The ex.</param>
		    public void WriteError(Exception ex) { }

            /// <summary>Writes an error.</summary>
            /// <param name="error">The error.</param>
			public void WriteError(string error) { }

            /// <summary>Writes an error.</summary>
            /// <param name="format">Describes the format to use.</param>
            /// <param name="args">  A variable-length parameters list containing arguments.</param>
			public void WriteError(string format, params object[] args) { }

		}

        /// <summary>A console tracer.</summary>
		public class ConsoleTracer : ITracer
		{
            /// <summary>Writes a debug.</summary>
            /// <param name="error">The error.</param>
			public void WriteDebug(string error)
			{
#if NETFX_CORE
				System.Diagnostics.Debug.WriteLine(error);
#else
				Console.WriteLine(error);
#endif
			}

            /// <summary>Writes a debug.</summary>
            /// <param name="format">Describes the format to use.</param>
            /// <param name="args">  A variable-length parameters list containing arguments.</param>
			public void WriteDebug(string format, params object[] args)
			{
#if NETFX_CORE
                System.Diagnostics.Debug.WriteLine(format, args);
#else
                Console.WriteLine(format, args);
#endif
			}

            /// <summary>Writes a warning.</summary>
            /// <param name="warning">The warning.</param>
		    public void WriteWarning(string warning)
		    {
#if NETFX_CORE
                System.Diagnostics.Debug.WriteLine(warning);                
#else
                Console.WriteLine(warning);                
#endif
		    }

            /// <summary>Writes a warning.</summary>
            /// <param name="format">Describes the format to use.</param>
            /// <param name="args">  A variable-length parameters list containing arguments.</param>
		    public void WriteWarning(string format, params object[] args)
		    {
#if NETFX_CORE
                System.Diagnostics.Debug.WriteLine(format, args);
#else
                Console.WriteLine(format, args);
#endif
            }

            /// <summary>Writes an error.</summary>
            /// <param name="ex">The ex.</param>
		    public void WriteError(Exception ex)
			{
#if NETFX_CORE
                System.Diagnostics.Debug.WriteLine(ex);
#else
                Console.WriteLine(ex);
#endif
			}

            /// <summary>Writes an error.</summary>
            /// <param name="error">The error.</param>
			public void WriteError(string error)
			{
#if NETFX_CORE
                System.Diagnostics.Debug.WriteLine(error);
#else
                Console.WriteLine(error);
#endif
			}

            /// <summary>Writes an error.</summary>
            /// <param name="format">Describes the format to use.</param>
            /// <param name="args">  A variable-length parameters list containing arguments.</param>
			public void WriteError(string format, params object[] args)
			{
#if NETFX_CORE
                System.Diagnostics.Debug.WriteLine(format, args);
#else
                Console.WriteLine(format, args);
#endif
			}
		}
	}
}