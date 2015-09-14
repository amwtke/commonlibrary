using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4
{
	[Obsolete("暂不支持", true)]
	public class RpcResult
	{
		public void EndInvoke()
		{
			throw new NotImplementedException();
		}

		public static void Return(Action<RpcResult> callback)
		{
			RpcResult result = new RpcResult();
			callback(result);
		}
	}

	[Obsolete("暂不支持", true)]
	public class RpcResult<T>
	{
		public RpcResult()
		{
			throw new NotImplementedException();
		}

		public T EndInvoke()
		{
			throw new NotImplementedException();
		}

		public void Return(T ret)
		{
			throw new NotImplementedException();
		}
	}
}
