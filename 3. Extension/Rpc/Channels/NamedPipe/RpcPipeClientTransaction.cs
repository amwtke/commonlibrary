using System;
using System.IO;
using System.IO.Pipes;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4.Rpc
{
	public class RpcPipeClientTransaction: IRpcClientTransaction
	{
		private NamedPipeUri _serverUri;
		private RpcPipeContext _context;
		private Action<RpcResponseHeader> _callback;
		private RpcPipeClientChannel _channel;

		private byte[] _bodyBuffer;
		private NamedPipeClientStream _stream;

		public RpcPipeClientTransaction(RpcPipeClientChannel channel, NamedPipeUri serverUri)
		{
			_channel = channel;
			_serverUri = serverUri;
		}

		public void SendRequest<T>(RpcRequestHeader request, T args, Action<RpcResponseHeader> callback, int timeout)
		{
			_callback = callback;

			_context = new RpcPipeContext();

			_context.From = request.ServiceAtComputer;
			_context.To = request.ToUri;
			_context.ServiceName = request.Service;
			_context.MethodName = request.Method;
			_context.HasBody = request.HasBody;

			_stream = new NamedPipeClientStream(_serverUri.Computer, _serverUri.PipeName, PipeDirection.InOut, PipeOptions.Asynchronous);

			try {
				timeout = timeout > 0 ? timeout : _channel.Timeout;
				if (timeout > 0) {
					_stream.Connect(timeout);
				} else {
					_stream.Connect();
				}

				if (request.HasBody) {
					RpcPipeStreamHelper.WriteStream<T>(_stream, _context, args);
				} else {
					RpcPipeStreamHelper.WriteStream<RpcNull>(_stream, _context, null);
				}

				RpcResponseHeader header;
				_context = RpcPipeStreamHelper.ReadStream(_stream, out _bodyBuffer);

				if (_context.RetCode == RpcErrorCode.OK) {
					header = RpcResponseHeader.CreateSuccess(_context.HasBody);
				} else {
					if (_bodyBuffer != null) {
						Exception ex = BinarySerializer.FromByteArray<Exception>(_bodyBuffer);
						header = RpcResponseHeader.CreateError(_context.RetCode, ex);
					} else {
						header = RpcResponseHeader.CreateError(_context.RetCode, null);
					}
				}
				_callback(header);
			} catch (Exception ex) {
				var header = RpcResponseHeader.CreateError(RpcErrorCode.SendFailed, ex);
				_callback(header);
			} finally {
				if (_stream != null) {
					_stream.Close();
				}
			}
		}

		public T ReceiveResponse<T>()
		{
			if (_context.HasBody) {
				return ProtoBufSerializer.FromByteArray<T>(_bodyBuffer);
			} else {
				return default(T);
			}
		}
	}
}
