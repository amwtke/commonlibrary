using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Imps.Services.CommonV4;
using Imps.Services.CommonV4.Configuration;

namespace Imps.Services.HA
{
	public class HAConfigurationLoader: IConfigurationLoader
	{
		private string _computerName;
		private string _serviceName;
		private RpcClientProxy _proxy;

		public HAConfigurationLoader(string serviceName, string computerName, string centerUrl)
		{
			_computerName = computerName;
			_serviceName = serviceName;
			_proxy = RpcProxyFactory.GetProxy<IHACenterConfigService>(centerUrl);
		}

		public IICConfigFieldBuffer LoadConfigField(string key)
		{
			HAGetConfigArgs args = new HAGetConfigArgs();
			args.ServiceName = _serviceName;
			args.ComputerName = _computerName;
			args.Path = key;

			return _proxy.Invoke<HAGetConfigArgs, IICConfigFieldBuffer>("LoadConfigField", args);
		}

		public IList<IICConfigItemBuffer> LoadConfigSection(string path)
		{
			HAGetConfigArgs args = new HAGetConfigArgs();
			args.ServiceName = _serviceName;
			args.ComputerName = _computerName;
			args.Path = path;

			return _proxy.Invoke<HAGetConfigArgs, List<IICConfigItemBuffer>>("LoadConfigSection", args);
		}

		public IList<IICConfigItemBuffer> LoadConfigItem(string path, string key)
		{
			HAGetConfigArgs args = new HAGetConfigArgs();
			args.ServiceName = _serviceName;
			args.ComputerName = _computerName;
			args.Path = path;
			args.Key = key;

			return _proxy.Invoke<HAGetConfigArgs, List<IICConfigItemBuffer>>("LoadConfigItem", args);
		}

		public IICConfigTableBuffer LoadConfigTable(string tableName)
		{
			HAGetConfigArgs args = new HAGetConfigArgs();
			args.ServiceName = _serviceName;
			args.ComputerName = _computerName;
			args.Path = tableName;

			return _proxy.Invoke<HAGetConfigArgs, IICConfigTableBuffer>("LoadConfigTable", args);
		}

		public HAServiceSettings LoadServiceSettings()
		{
			HAGetConfigArgs args = new HAGetConfigArgs();
			args.ServiceName = _serviceName;
			args.ComputerName = _computerName;

			return _proxy.Invoke<HAGetConfigArgs, HAServiceSettings>("LoadServiceSettings", args);
		}
	}
}
