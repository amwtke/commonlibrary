using System;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4
{
	/// <summary>
	///		����IICTracing�Ľӿ�
	/// <seealso cref="TracingManager"/>
	/// <remarks>���TracingManager.GetTracingϵ�з������һ�����õĽӿ�</remarks>
	/// </summary>
    public interface ITracing 
	{
        void Info(string message);
		void Info(string from, string to, string message);
        void Info(Exception exception, string message);
		void Info(Exception exception, string from, string to, string message);
		void InfoFmt(string format, params object[] args);
		void InfoFmt(Exception exception, string format, params object[] args);
		void InfoFmt2(string from, string to, string format, params object[] args);
		void InfoFmt2(Exception exception, string from, string to, string format, params object[] args);

		void Warn(string message);
		void Warn(string from, string to, string message);
		void Warn(Exception exception, string message);
		void Warn(Exception exception, string from, string to, string message);
		void WarnFmt(string format, params object[] args);
		void WarnFmt(Exception exception, string format, params object[] args);
		void WarnFmt2(string from, string to, string format, params object[] args);
		void WarnFmt2(Exception exception, string from, string to, string format, params object[] args);

		void Error(string message);
		void Error(string from, string to, string message);
		void Error(Exception exception, string message);
		void Error(Exception exception, string from, string to, string message);
		void ErrorFmt(string format, params object[] args);
		void ErrorFmt(Exception exception, string format, params object[] args);
		void ErrorFmt2(string from, string to, string format, params object[] args);
		void ErrorFmt2(Exception exception, string from, string to, string format, params object[] args);

		void Info(Action callback);
		void Warn(Action callback);
		void Error(Action callback);

		string LoggerName { get; }
	}
}
