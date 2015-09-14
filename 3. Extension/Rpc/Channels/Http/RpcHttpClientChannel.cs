using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4.Rpc
{
	public class RpcHttpClientChannel: IRpcClientChannel
	{
		private int _timeout = 180000;	// 默认180秒

		public string Protocol
		{
			get { return "http"; }
		}

		public int Timeout
		{
			get { return _timeout; }
			set { _timeout = value; }
		}

		public RpcHttpClientChannel()
		{
		}

		public IRpcClientTransaction CreateTransaction(BaseUri serverUri)
		{
			IICAssert.Is<HttpUri>(serverUri);
			return new RpcHttpClientTransaction(this, serverUri);
		}
	}
}
