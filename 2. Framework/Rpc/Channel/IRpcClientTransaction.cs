using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4.Rpc
{
	public interface IRpcClientTransaction
	{
		void SendRequest<T>(RpcRequestHeader request, T args, Action<RpcResponseHeader> callback, int timeout);

		T ReceiveResponse<T>();
	}
}
