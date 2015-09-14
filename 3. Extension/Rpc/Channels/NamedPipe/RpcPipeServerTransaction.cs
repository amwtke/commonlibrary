using System;
using System.IO;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4.Rpc
{
	public class RpcPipeServerTransaction: IRpcServerTransaction
	{
		private byte[] _buffer;
		private RpcPipeContext _context;
		private RpcPipeServerChannel _channel;
		private NamedPipeServerStream _stream;

		public RpcPipeServerTransaction(NamedPipeServerStream stream, RpcPipeServerChannel channel)
		{
			_stream = stream;
			_channel = channel;
		}

		public RpcRequestHeader ReceiveRequestHeader()
		{
			_context = RpcPipeStreamHelper.ReadStream(_stream, out _buffer);

			RpcRequestHeader header = new RpcRequestHeader();
			header.ServiceAtComputer = _context.From;
			header.Service = _context.ServiceName;
			header.Method = _context.MethodName;
			header.HasBody = _context.HasBody;
			header.ToUri = _context.To;
			return header;
		}

		public T ReceiceRequest<T>()
		{
			if (_context.HasBody) {
				return ProtoBufSerializer.FromByteArray<T>(_buffer);
			} else {
				return default(T);
			}
		}

		public void SendResponse<T>(RpcResponseHeader header, T results)
		{
			_context.RetCode = RpcErrorCode.OK;
			_context.HasBody = header.HasBody;

			if (header.HasBody) {
				RpcPipeStreamHelper.WriteStream<T>(_stream, _context, results);
				_stream.Disconnect();
				_channel.RecycleServerStream(_stream);
			} else {
				RpcPipeStreamHelper.WriteStream<RpcNull>(_stream, _context, null);
				_stream.Disconnect();
				_channel.RecycleServerStream(_stream);
			}
		}

		public void SendError(RpcResponseHeader header)
		{
			_context.RetCode = header.ErrorCode;

			if (header.Error != null) {
				_context.HasBody = true;
				RpcPipeStreamHelper.WriteStreamEx(_stream, _context, header.Error);
			} else {
				_context.HasBody = false;
				RpcPipeStreamHelper.WriteStream<RpcNull>(_stream, _context, null);
			}
			_stream.Disconnect();
			_channel.RecycleServerStream(_stream);
		}
	}
}
