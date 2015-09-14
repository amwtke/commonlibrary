using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4.Rpc
{
	public class RpcHttpServerTransaction: IRpcServerTransaction
	{
		private HttpListenerContext _httpContext;

		public RpcHttpServerTransaction(HttpListenerContext context)
		{
			_httpContext = context;
		}

		public RpcRequestHeader ReceiveRequestHeader()
		{
			HttpListenerRequest request = _httpContext.Request;
			string url = _httpContext.Request.Url.ToString();

			var header = new RpcRequestHeader();
			header.ServiceAtComputer = request.Headers["From"];
			
			int l = url.LastIndexOf("/");
			header.ServerUri = url.Substring(0, l);
			header.ServiceDotMethod = url.Substring(l + 1);

			header.HasBody = (request.Headers["Null"] != "true");
			header.ToUri = ObjectHelper.ToString(request.Headers["ToUri"]);

			return header;
		}

		public T ReceiceRequest<T>()
		{
			if (_httpContext.Request.ContentLength64 == 0) {
				return ProtoBufSerializer.Deserialize<T>(RpcNull.EmptyStream);
			} else {
				return ProtoBufSerializer.Deserialize<T>(_httpContext.Request.InputStream);
			}
		}

		public void SendResponse<T>(RpcResponseHeader header, T results)
		{
			HttpListenerResponse response = _httpContext.Response;
			response.StatusCode = 200;
			response.ContentType = "multipart/byteranges";
			if (!header.HasBody) {
				response.Headers.Add("Null", "true");
				response.ContentLength64 = 0;
			} else {
				byte[] buffer = ProtoBufSerializer.ToByteArray<T>(results);
				if (buffer.Length > 0) {
					response.ContentLength64 = buffer.Length;
					response.OutputStream.Write(buffer, 0, buffer.Length);
					response.OutputStream.Close();
				} else {
					response.ContentLength64 = 0;
				}
			}
			response.Close();
		}

		public void SendError(RpcResponseHeader header)
		{
			HttpListenerResponse response = _httpContext.Response;
			response.StatusCode = 200;
			response.ContentType = "multipart/byteranges";

			response.Headers.Add("Warning", header.ErrorCode.ToString());
			if (header.Error == null) {
				response.ContentLength64 = 0;
			} else {
				byte[] buffer = BinarySerializer.ToByteArray(header.Error);
				response.ContentLength64 = buffer.Length;
				response.OutputStream.Write(buffer, 0, buffer.Length);
				response.OutputStream.Close();
			}
			response.Close();
		}
	}
}
