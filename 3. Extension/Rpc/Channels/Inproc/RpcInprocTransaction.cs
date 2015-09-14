using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4.Rpc
{
	public class RpcInprocTransaction: IRpcClientTransaction, IRpcServerTransaction
	{
		private RpcInprocChannel _channel;
		private RpcRequestHeader _request;
		private RpcResponseHeader _response;

		private object _args;
		private object _results;

		private Action<RpcResponseHeader> _callback;

		public RpcInprocTransaction(RpcInprocChannel channel)
		{
			_channel = channel;
		}

		public void SendRequest<T>(RpcRequestHeader request, T args, Action<RpcResponseHeader> callback, int timeout)
		{
			_args = args;
			_request = request;
			_callback = callback;
			_channel.OnTransactionStart(this);
		}

		public RpcRequestHeader ReceiveRequestHeader()
		{
			return _request;
		}

		public T ReceiceRequest<T>()
		{
			return (T)_args;
		}

		public void SendResponse<T>(RpcResponseHeader header, T results)
		{
			_response = header;
			_results = results;
			_callback(header);
		}

		public void SendError(RpcResponseHeader header)
		{
			_response = header;
			_callback(header);
		}

		public T ReceiveResponse<T>()
		{
			return (T)_results;
		}
	}
}
