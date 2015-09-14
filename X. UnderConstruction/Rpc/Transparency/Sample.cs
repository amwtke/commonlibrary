//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Imps.Services.CommonV4.Rpc
//{
//    [RpcService("TransparencyRpc", IsTransparent = true)]
//    public interface ITransparencyRpc
//    {
//        void Somebody(int a, int b, RpcReturn<int> ctx);
//        int Somebody(int a, int b, Action<RpcTransparentContext> callback);
//        int Somebody(int a, int b, Action<RpcTransparentContext> callback);
//    }

//    class Sample
//    {
//        int Somebody(RpcTransparentContext ctx, int a, int b)
//        {
//            return a + b;
//        }

//        void main()
//        {
//            ITransparencyRpc rpc = RpcProxyFactory.GetProxy<ITransparencyRpc>(url, );

//            var ret = RpcResult.New(
//            rpc.Somebody(10, 20, ret);
//            PrintRet;
//        }
//    }
//}
