using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4.Rpc
{
	public class RpcPipeClientChannel: IRpcClientChannel
	{
		private int _connectTimeout;

		public string Protocol
		{
			get { return "pipe"; }
		}

		public int Timeout
		{
			get { return _connectTimeout; }
			set { }
		}

		public RpcPipeClientChannel()
		{
			_connectTimeout = 15000;
		}

		public IRpcClientTransaction CreateTransaction(BaseUri serverUri)
		{
			IICAssert.Is<NamedPipeUri>(serverUri);
			return new RpcPipeClientTransaction(this, (NamedPipeUri)serverUri);
		}
	}
}
