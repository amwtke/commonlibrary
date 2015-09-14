using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4.Rpc
{
	public interface IRpcServerTransaction
	{
		RpcRequestHeader ReceiveRequestHeader();
		T ReceiceRequest<T>();
		void SendResponse<T>(RpcResponseHeader header, T results);
		void SendError(RpcResponseHeader header);
	}
}
