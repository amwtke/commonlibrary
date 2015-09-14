using System;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4
{
	public interface IRpcServiceOperation
	{
		void OnTransactionStart(RpcServerContext context);
	}

	public abstract class RpcServiceBase
	{
        public RpcServiceBase(string serviceName)
        {
            p_serviceName = serviceName;
        }

		public string ServiceName
		{
			get { return p_serviceName; }
		}

		public abstract void OnTransactionStart(RpcServerContext context);

		protected string p_serviceName;
	}
}
