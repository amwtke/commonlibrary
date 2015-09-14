//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Imps.Services.CommonV4.Rpc
//{
//    public class RpcDuplexClient<IServer>
//    {
//        public RpcDuplexClient(string from)
//        {
//        }

//        public void BeginConnect()
//        {
//            RpcClientProxy.CreateDuplexClient();
//        }

//        public void BeginCall<TArgs>(string methods, TArgs args, Action<RpcServerContext> endDelegate)
//        {
//            RpcClientProxy<IServer> clientProxy = new RpcClientProxy<IServer>();
//        }

//        public IRpcDuplexClientConnection _connection;
//    }
//}
