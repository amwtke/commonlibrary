using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4
{
	public static class ServiceEnviornment
	{
		private static string _serviceName;
		private static string _computerName;
		private static string _workPath;
		private static string _processInfo;
		private static bool _debug;

		static ServiceEnviornment()
		{
			Process process = Process.GetCurrentProcess();

			_serviceName = process.ProcessName;
			_computerName = Environment.MachineName;
			_workPath = AppDomain.CurrentDomain.BaseDirectory;
			_processInfo = string.Format("{0}-{1}", process.Id, process.ProcessName);
			_debug = false;
		}

		public static string WorkPath
		{
			get { return _workPath; }
		}

		public static string ProcessInfo
		{
			get { return _processInfo; }
		}

		public static string ServiceName
		{
			get { return _serviceName; }
			set { _serviceName = value; }
		}

		public static string ComputerName
		{
			get { return _computerName; }
			set { _computerName = value; }
		}

		public static bool Debug
		{
			get { return _debug; }
			set { _debug = value; }
		}
	}
}
