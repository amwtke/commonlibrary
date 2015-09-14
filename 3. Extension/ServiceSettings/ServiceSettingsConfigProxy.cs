using System;
using System.Web;
using System.Web.Configuration;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Imps.Services.HA;
using Imps.Services.CommonV4.Configuration;

namespace Imps.Services.CommonV4
{
	public enum ServiceRunMode
	{
		Unknown,			// 默认配置
		LocalService,		// 使用LocalConfigurator, 本地app.config, 服务启动
		LocalWeb,			// 使用LocalConfigurator, 本机app.config, 服务启动
		HAService,			// 使用HAConfigurator, 使用Service进入
		HAWeb,				// ASP.NET应用程序池, 使用web.config, 使用appSettings["CenterUrl"], 访问HACenter

		//
		// NextVersion
		//LocalWinform,
		//LocalConsole,		// 使用LocalConfigurator, 使用app.config, Console启动
		//HAConsole,		// 使用HAConfigurator, 使用命令行参数通过Console进入
		//HATask,			// HACenter管理的TASK程序, 可以动态下载的插件
	}

	public sealed class ServiceSettingsConfigProxy
	{
		private ServiceRunMode _runMode;
		private ServiceSettingsConfigSection _section = null;

		private string _serviceName = string.Empty;
		private string _serviceRoleName = string.Empty;
		private string _computerName = string.Empty;
		private string _processInfo = string.Empty;
		private string _workPath = string.Empty;
		private string _domain = string.Empty;
		private string _site = string.Empty;
		private int _poolId = 0;
		private List<int> _pools = null;
		

		//
		// Default Constructors
		public ServiceSettingsConfigProxy(string serviceName)
			: this(serviceName, Environment.MachineName)
		{
		}

		//
		// Nothing in
		public ServiceSettingsConfigProxy(string serviceName, string computerName)
		{
			_runMode = ServiceRunMode.Unknown;
			_section = IICConfigSection.CreateDefault<ServiceSettingsConfigSection>();
			_serviceName = serviceName;
			_section.ServiceName = serviceName;

			Process process = Process.GetCurrentProcess();
			_computerName = computerName.ToUpper();
			_workPath = AppDomain.CurrentDomain.BaseDirectory;
		}

		public void UpdateConfig(ServiceRunMode runMode, HAServiceSettings serviceSettings)
		{
			_runMode = runMode;
			_section = IICConfigurationManager.Configurator.GetConfigSecion<ServiceSettingsConfigSection>("ServiceSettings", null);

			//
			// 优先使用本地配置
			_domain = _section.Domain;
			_site = _section.Site;

			int.TryParse(_section.PoolID, out _poolId);
			_pools = new List<int>();
			foreach (string a in _section.Pools.Split(',')) {
				int p;
				if (int.TryParse(a, out p)) {
					_pools.Add(p);
				}
			}

			//
			// HA模式下, 如果ServiceSettings没有配置, 会使用来自HA_Computer与HA_Deployment的配置覆盖
			// Pool, Site, Domain三项配置
			//
			// BTW domain is not usable
			switch (_runMode) {
				case ServiceRunMode.LocalWeb:
				case ServiceRunMode.LocalService:
					if (string.IsNullOrEmpty(_serviceRoleName)) {
						_serviceRoleName = _section.ServiceName;
					}
					break;
				case ServiceRunMode.HAService:
				case ServiceRunMode.HAWeb:
					//
					// 
					if (serviceSettings != null) {
						_serviceRoleName = serviceSettings.ServiceOriginName;

						if (string.IsNullOrEmpty(_domain)) {
							_domain = serviceSettings.Domain;
						}

						if (string.IsNullOrEmpty(_section.Site)) {
							_site = serviceSettings.Site;
						}

						if (string.IsNullOrEmpty(_section.PoolID)) {
							_poolId = serviceSettings.PoolId;
						}
					}
					break;
				default:
					throw new NotSupportedException("Unexcepted RunMode:" + _runMode);
			}

			ServiceEnviornment.ComputerName = _computerName;
			ServiceEnviornment.ServiceName = _serviceName;
		}

		#region Fixed in HAMode
		public ServiceRunMode RunMode
		{
			get { return _runMode; }
		}

		public string ProcessInfo
		{
			get { return _processInfo; }
		}

		public string ComputerName
		{
			get { return _computerName; }
		}

		public string ServiceRoleName
		{
			get { return _serviceRoleName; }
		}

		public string ServiceName
		{
			get { return _serviceName; }
		}

		public string BasePath
		{
			get { return _workPath; }
		}

		public string Domain
		{
			get { return _domain; }
		}

		public int PoolID
		{
			get { return _poolId; }
		}

		public List<int> Pools
		{
			get { return _pools; }
		}

		public string SiteName
		{
			get { return _site; }
		}
		#endregion

		#region <ServiceSettings> ConfigSection Fields
		public bool Debug
		{
			get { return _section.Debug; }
		}

		public int SipcServerPort
		{
			get { return _section.SipcServerPort; }
		}

		public int HttpServerPort
		{
			get { return _section.HttpServerPort; }
		}

		public string HttpServicePrefix
		{
			get { return _section.HttpServicePrefix; }
		}

		public int RpcServerPort
		{
			get { return _section.RpcServerPort; }
		}

		public int RemotingServerPort
		{
			get { return _section.RemotingServerPort; }
		}

		public int MaxWorkerThread
		{
			get { return _section.MaxWorkerThread; }
		}

		public int MinWorkerThread
		{
			get { return _section.MinWorkerThread; }
		}

		public int WorkerThreadTimeout
		{
			get { return _section.WorkerThreadTimeout; }
		}

		public string ServerAddress
		{
			get { return _section.ServerAddress; }
		}
		#endregion
	}
}
