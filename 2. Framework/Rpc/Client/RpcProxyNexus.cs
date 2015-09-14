using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4.Rpc
{
	class RpcProxyNexus
	{
		private RpcProtocolVersion _version;
		private IRpcClientChannel _channel;
		private BaseUri _serverUri;
		private string _serviceRole;

		public BaseUri ServerUri
		{
			get { return _serverUri; }
		}

		public string ServiceRole
		{
			get { return _serviceRole; }
		}

		public RpcProtocolVersion Version
		{
			get { return _version; }
		}

		public bool AutoBatch
		{
			get { throw new NotImplementedException(); }
		}

		public RpcProxyNexus(IRpcClientChannel channel, BaseUri uri, string serviceRole)
		{
			_channel = channel;
			_serverUri = uri;
			_version = RpcProtocolVersion.V4;
			_serviceRole = serviceRole;
		}

		public IRpcClientTransaction CreateTransaction()
		{
			return _channel.CreateTransaction(_serverUri);
		}

		public void EnqueueBatchTransaction(RpcClientContext context)
		{
		}
	}
}
