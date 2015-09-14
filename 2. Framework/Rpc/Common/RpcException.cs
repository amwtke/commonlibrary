using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4
{
	[Serializable]
	public class RpcException: Exception
	{
		private RpcErrorCode _code;
		private string _toUri;
		private string _serviceUrl;
		private string _message;

		public RpcException()
		{
		}

		protected RpcException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public RpcException(string message, string url, RpcErrorCode code, Exception ex)
			: this(message, url, null, code, ex)
		{
		}

		public RpcException(string message, string url, BaseUri uri, RpcErrorCode code, Exception ex)
			: base(null, ex)
		{
			_message = message;
			_serviceUrl = url;
			_toUri = ObjectHelper.ToString(uri);
			_code = code;
		}

		public override string ToString()
		{
			return string.Format("RpcException<{0}> on \"{1}\" ({2}) \r\n{3}",
				_code, _serviceUrl, _message, InnerException);
		}

		public RpcErrorCode RpcCode
		{
			get { return _code; }
		}

		public string RpcMessage
		{
			get { return _message; }
		}

		public string ServiceUrl
		{
			get { return _serviceUrl; }
		}
	}
}
