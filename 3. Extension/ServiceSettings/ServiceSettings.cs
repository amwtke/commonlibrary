using System;
using System.Collections.Generic;
using System.Web.Configuration;
using System.Configuration;
using System.Text;

using Imps.Services.HA;
using Imps.Services.CommonV4.Rpc;
using Imps.Services.CommonV4.Configuration;

namespace Imps.Services.CommonV4
{
	public static class ServiceSettings
	{
		#region Private Fields
		private const string NULL = "NULL";
		private static object _syncRoot = new object();
		private static ServiceSettingsConfigProxy _current = null;
		#endregion

		#region Public Static Properties
		public static string ProcessInfo
		{
			get { return Current.ProcessInfo; }
		}

		public static string ComputerName
		{
			get { return Current.ComputerName; }
		}

		public static string ServiceName
		{
			get { return Current.ServiceName; }
		}

		public static ServiceRunMode RunMode
		{
			get { return Current.RunMode; }
		}

		public static ServiceSettingsConfigProxy Current
		{
			get
			{
				if (_current == null) {
					lock (_syncRoot) {
						if (_current == null) {
							_current = new ServiceSettingsConfigProxy(NULL);
						}
					}
				}
				return _current;
			}
		}

		public static string BasePath
		{
			get { return AppDomain.CurrentDomain.BaseDirectory; }
		}
		#endregion

		#region Public Initialize Methods
		/// <summary>本地初始化服务, 默认初始化为本地配置, HA会覆盖这个配置</summary>
		public static void InitService(string serviceName)
		{
			if (_current != null && _current.ServiceName != NULL)
				return;

			lock (_syncRoot) {
				IICConfigurationManager.Loader = new LocalConfigurationLoader();

				_current = new ServiceSettingsConfigProxy(serviceName);
				_current.UpdateConfig(ServiceRunMode.LocalService, null);
				TracingManager.UpdateConfig();
			}
		}

		public static void InitServiceHa(string serviceName, string centerUrl)
		{
			InitServiceHa(serviceName, Environment.MachineName, centerUrl);
		}

		public static void InitServiceHa(string serviceName, string computerName, string centerUrl)
		{
			if (_current != null && _current.ServiceName != NULL)
				return;

			lock (_syncRoot) {
				_current = new ServiceSettingsConfigProxy(serviceName, computerName);

				RpcHttpClientChannel channel = new RpcHttpClientChannel();
				RpcProxyFactory.RegisterClientChannel(channel);

				HAGetConfigArgs args = new HAGetConfigArgs();
				args.ServiceName = serviceName;
				args.ComputerName = computerName;

				var proxy = RpcProxyFactory.GetProxy<IHACenterConfigService>(centerUrl);
				HAServiceSettings settings = proxy.Invoke<HAGetConfigArgs, HAServiceSettings>(
					"LoadServiceSettings",
					args);

				IICConfigurationManager.Loader = new HAConfigurationLoader(serviceName, computerName, centerUrl);

				_current.UpdateConfig(ServiceRunMode.HAService, settings);
				TracingManager.UpdateConfig();
			}
		}

		public static void InitWeb()
		{
			if (_current != null && _current.ServiceName != NULL)
				return;

			lock (_syncRoot) {
				string runMode = WebConfigurationManager.AppSettings["RunMode"];
				string centerUrl = WebConfigurationManager.AppSettings["CenterUrl"];
				string serviceName = WebConfigurationManager.AppSettings["ServiceName"];
				string computerName = WebConfigurationManager.AppSettings["ComputerName"] ?? Environment.MachineName;

				if (runMode == "LocalWeb") {
					_current = new ServiceSettingsConfigProxy(serviceName);
					IICConfigurationManager.Loader = new LocalConfigurationLoader();
					_current.UpdateConfig(ServiceRunMode.LocalWeb, null);
					RpcProxyFactory.RegisterClientChannel(new RpcPipeClientChannel());
					TracingManager.UpdateConfig();
				} else {
					_current = new ServiceSettingsConfigProxy(serviceName, computerName);
					RpcProxyFactory.RegisterClientChannel(new RpcHttpClientChannel());
					RpcProxyFactory.RegisterClientChannel(new RpcPipeClientChannel());
					HAConfigurationLoader loader = new HAConfigurationLoader(serviceName, computerName, centerUrl);
					HAServiceSettings settings = loader.LoadServiceSettings();
					IICConfigurationManager.Loader = loader;
					_current.UpdateConfig(ServiceRunMode.HAWeb, settings);
					TracingManager.UpdateConfig();
				}
			}
		}
		#endregion
	}
}
