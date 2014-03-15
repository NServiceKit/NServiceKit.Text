using System;

namespace NServiceKit.Text
{
	public interface ITracer
	{
        void WriteDebug(string error);
        void WriteDebug(string format, params object[] args);
        void WriteWarning(string warning);
        void WriteWarning(string format, params object[] args);
		
		void WriteError(Exception ex);
		void WriteError(string error);
		void WriteError(string format, params object[] args);
	}
}