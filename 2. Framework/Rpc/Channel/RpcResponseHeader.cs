using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Imps.Services.CommonV4;

namespace Imps.Services.CommonV4.Rpc
{
	public class RpcResponseHeader
	{
		public RpcErrorCode ErrorCode;	// ErrorCode 
		public Exception Error;			// ErrorMessage
		public bool HasBody;			// Body is null

		private RpcResponseHeader(bool hasBody)
		{
			ErrorCode = RpcErrorCode.OK;
			Error = null;
			HasBody = hasBody;
		}

		private RpcResponseHeader(RpcErrorCode code, Exception ex)
		{
			ErrorCode = code;
			Error = ex;
			HasBody = (ex != null);
		}

		public static RpcResponseHeader CreateSuccess(bool hasBody)
		{
			return new RpcResponseHeader(hasBody);
		}

		public static RpcResponseHeader CreateError(RpcErrorCode code, Exception ex)
		{
			return new RpcResponseHeader(code, ex);
		}
	}
}
