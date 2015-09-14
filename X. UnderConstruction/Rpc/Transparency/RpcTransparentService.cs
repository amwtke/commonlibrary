using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Reflection.Emit;

using Imps.Services.CommonV4;

namespace Imps.Services.CommonV4 {

	[Obsolete("NextVersion NotSure", true)]
    public static class RpcGetArgsHelper {
        static readonly Dictionary<string, RpcServiceMethod> dict
            = new Dictionary<string, RpcServiceMethod>();
        private static readonly ITracing _tracing
            = TracingManager.GetTracing(typeof(RpcGetArgsHelper));
        static readonly CodeCompileUnit CompileUnit;
        static readonly CodeTypeDeclaration Methodhelper;

        static RpcGetArgsHelper() {
            CompileUnit = new CodeCompileUnit();
            var aName = new CodeNamespace("Imps.Generics.DynamicTypes");
            CompileUnit.Namespaces.Add(aName);
            aName.Imports.Add(new CodeNamespaceImport("System"));
            aName.Imports.Add(new CodeNamespaceImport("System.Reflection"));
            aName.Imports.Add(new CodeNamespaceImport("Imps.Services.CommonV4"));


            Methodhelper = new CodeTypeDeclaration("TransparentMethodHelper");
            aName.Types.Add(Methodhelper);
        }

        public static void Build(string[] dlls) {
            CompilerParameters opt = new CompilerParameters(new string[]{
                "System.dll", "IICCommonLibrary.dll"});
            if (dlls != null) {
                foreach (var dll in dlls) {
                    opt.ReferencedAssemblies.Add(dll);
                }
            }
            opt.GenerateExecutable = false;
            opt.TreatWarningsAsErrors = true;
            opt.IncludeDebugInformation = true;
            opt.GenerateInMemory = true;

            CodeDomProvider provider = CodeDomProvider.CreateProvider("cs");

            StringWriter sw = new StringWriter();
            provider.GenerateCodeFromCompileUnit(CompileUnit, sw, null);
            string output = sw.ToString();
            _tracing.Info(output);
            sw.Close();

            CompilerResults results = provider.CompileAssemblyFromDom(opt, CompileUnit);
            OutputResults(results);
            if (results.NativeCompilerReturnValue != 0) {
                _tracing.Warn("Compilation failed.");
                throw new ApplicationException("Compilation failed.");
            }
            else {
                _tracing.Info("completed successfully.");
            }

            Type typeMethodHelper = results.CompiledAssembly
                .GetType("Imps.Generics.DynamicTypes.TransparentMethodHelper");
            foreach (var pair in dict) {
                RpcGetArgsDelegate getArgs;
                MethodInfo mi = typeMethodHelper.GetMethod(pair.Key);
                getArgs = (RpcGetArgsDelegate)RpcGetArgsDelegate.CreateDelegate(
                    typeof(RpcGetArgsDelegate), mi);
                pair.Value.GetArgs = getArgs;
            }
        }

        static void OutputResults(CompilerResults results) {
            _tracing.Info("NativeCompilerReturnValue=" +
                results.NativeCompilerReturnValue.ToString());
            foreach (string s in results.Output) {
                _tracing.Info(s);
            }
        }

        internal static void RegisterMethod(string service, RpcServiceMethod method) {

            if (dict.ContainsKey(service + method.Method.Name))
                throw new ApplicationException("已经注册过");
            ParameterInfo[] pis = method.Method.GetParameters();
            string strArgType = GetTypeStr(pis);
            Type returnType = pis[pis.Length - 1].ParameterType;
            string strReturnType = returnType.GetGenericArguments()[0].FullName;
            string strObjs = getStrObjs(pis);

            string strMethod = @"       public static object[] {0}{1}(RpcServerContext context) {{
            Action<{2}> action 
                = delegate({2} ret) {{ context.Return(ret); }};
            RpcResult2<{2}> result 
                = new RpcResult2<{2}>(action);
            result.Context = context;
            {3} arg 
                = context.GetArgs<{3}>();
            return new object[] {{ {4} }};
        }}";
            strMethod = string.Format(strMethod, service,
                method.Method.Name, strReturnType, strArgType, strObjs);
            Methodhelper.Members.Add(new CodeSnippetTypeMember(strMethod));
            dict[service + method.Method.Name] = method;

        }

        private static string getStrObjs(ParameterInfo[] pis) {
            switch (pis.Length)
            {
                case 2:
                    return "arg, result";
                case 3:
                    return "arg.Value1,arg.Value2, result";
                case 4:
                    return "arg.Value1,arg.Value2,arg.Value3, result";
                case 5:
                    return "arg.Value1,arg.Value2,arg.Value3,arg.Value4 result";
                default:
                    throw new ApplicationException("参数个数不对");
            }
        }

        public static string GetTypeStr(ParameterInfo[] pis) {
            switch (pis.Length) {
                case 2:
                    return GetTypeStr(pis[0].ParameterType);
                case 3:
                    return string.Format("RpcClass<{0},{1}>",
                        GetTypeStr(pis[0].ParameterType),
                        GetTypeStr(pis[1].ParameterType));
                case 4:
                    return string.Format("RpcClass<{0},{1},{2}>",
                        GetTypeStr(pis[0].ParameterType),
                        GetTypeStr(pis[1].ParameterType),
                        GetTypeStr(pis[2].ParameterType));
                case 5:
                    return string.Format("RpcClass<{0},{1},{2},{3}>",
                        GetTypeStr(pis[0].ParameterType),
                        GetTypeStr(pis[1].ParameterType),
                        GetTypeStr(pis[2].ParameterType), 
                        GetTypeStr(pis[3].ParameterType));
                default:
                    throw new ApplicationException("参数个数不对");
            }
        }
        public static string GetTypeStr2(ParameterInfo[] pis) {
            switch (pis.Length) {
                case 2:
                    return string.Format("RpcClass<{0}>",GetTypeStr(pis[0].ParameterType));
                case 3:
                    return string.Format("RpcClass<{0},{1}>",
                        GetTypeStr(pis[0].ParameterType),
                        GetTypeStr(pis[1].ParameterType));
                case 4:
                    return string.Format("RpcClass<{0},{1},{2}>",
                        GetTypeStr(pis[0].ParameterType),
                        GetTypeStr(pis[1].ParameterType),
                        GetTypeStr(pis[2].ParameterType));
                case 5:
                    return string.Format("RpcClass<{0},{1},{2},{3}>",
                        GetTypeStr(pis[0].ParameterType),
                        GetTypeStr(pis[1].ParameterType),
                        GetTypeStr(pis[2].ParameterType),
                        GetTypeStr(pis[3].ParameterType));
                default:
                    throw new ApplicationException("参数个数不对");
            }
        }

        public static string GetTypeStr(Type argType) {
            if (!argType.IsGenericType)
                return argType.FullName;
            int pos = argType.Name.IndexOf("`");
            string className = argType.Name.Substring(0, pos);
            Type[] argTypes = argType.GetGenericArguments();
            switch (argTypes.Length) {
                case 1:
                    return string.Format("{0}.{1}<{2}>", argType.Namespace,
                        className, GetTypeStr(argTypes[0]));
                case 2:
                    return string.Format("{0}.{1}<{2},{3}>",
                        argType.Namespace, className, GetTypeStr(argTypes[0]),
                        GetTypeStr(argTypes[1]));
                case 3:
                    return string.Format("{0}.{1}<{2},{3},{4}>",
                        argType.Namespace, className, GetTypeStr(argTypes[0]),
                        GetTypeStr(argTypes[1]), GetTypeStr(argTypes[2]));
                case 4:
                    return string.Format("{0}.{1}<{2},{3},{4},{5}>",
                         argType.Namespace, className, GetTypeStr(argTypes[0])
                         , GetTypeStr(argTypes[1]), GetTypeStr(argTypes[2]),
                         GetTypeStr(argTypes[3]));
                default:
                    throw new ApplicationException("参数过多");
            }
        }

        internal static string GetParams(ParameterInfo[] pis) {
            switch (pis.Length) {
                case 2:
                    return string.Format("{0} {1}",
                        GetTypeStr(pis[0].ParameterType),pis[0].Name);
                case 3:
                    return string.Format("{0} {1}, {2} {3}",
                        GetTypeStr(pis[0].ParameterType),pis[0].Name,
                        GetTypeStr(pis[1].ParameterType),pis[1].Name);
                case 4:
                    return string.Format("{0} {1}, {2} {3}, {4} {5}",
                        GetTypeStr(pis[0].ParameterType),pis[0].Name,
                        GetTypeStr(pis[1].ParameterType),pis[1].Name,
                        GetTypeStr(pis[2].ParameterType), pis[2].Name);
                case 5:
                    return string.Format("{0} {1}, {2} {3}, {4} {5}, {6} {7}",
                        GetTypeStr(pis[0].ParameterType),pis[0].Name,
                        GetTypeStr(pis[1].ParameterType),pis[1].Name,
                        GetTypeStr(pis[2].ParameterType),pis[2].Name,
                        GetTypeStr(pis[3].ParameterType),pis[3].Name);
                default:
                    throw new ApplicationException("参数个数不对");
            }
        }

        internal static string GetParams2(ParameterInfo[] pis) {
            switch (pis.Length) {
                case 2:
                    return string.Format("{0}",pis[0].Name);
                case 3:
                    return string.Format("{0}, {1}",pis[0].Name, pis[1].Name);
                case 4:
                    return string.Format("{0}, {1}, {2}",
                        pis[0].Name,pis[1].Name,pis[2].Name);
                case 5:
                    return string.Format("{0}, {1}, {2}, {3}",
                        pis[0].Name,pis[1].Name,pis[2].Name,pis[3].Name);
                default:
                    throw new ApplicationException("参数个数不对");
            }
        }
    }
	[Obsolete("NextVersion NotSure", true)]
    public class RpcTransparentService<T> : RpcServiceBase {
        internal RpcTransparentService(T serviceObj, string serviceUrl)
            : base(string.Empty) {
            Type intf = typeof(T);
            if (!intf.IsInterface)
                throw new NotSupportedException();

            RpcServiceAttribute serviceAttr = AttributeHelper.GetAttribute<RpcServiceAttribute>(intf);
            p_serviceName = serviceAttr.ServiceName;
            _serviceObj = serviceObj;
            _serviceUrl = serviceUrl;

            IICPerformanceCounterCategory category =
                new IICPerformanceCounterCategory("rpc:" + p_serviceName,
                    PerformanceCounterCategoryType.MultiInstance);
            foreach (MethodInfo method in intf.GetMethods()) {
                string methodName = method.Name;


                DynamicMethod dm = new DynamicMethod("fun", typeof(object[]),
                    new[] { typeof(RpcServerContext) });

                RpcServiceMethod m = new RpcServiceMethod();
                m.RatePerSecond = category.CreateCounter(methodName + " /sec.",
                    PerformanceCounterType.RateOfCountsPerSecond32);
                m.TotalCount = category.CreateCounter(methodName + " Total.",
                    PerformanceCounterType.NumberOfItems32);
                m.TotalFailed = category.CreateCounter(methodName + " Failed.",
                    PerformanceCounterType.NumberOfItems32);
                m.Concurrent = category.CreateCounter(methodName + " Concurrent.",
                    PerformanceCounterType.NumberOfItems32);
                m.Method = method;

                RpcGetArgsHelper.RegisterMethod(p_serviceName, m);

                _methods.Add(methodName, m);
            }
            IICPerformanceCounterFactory.GetCounters(category);
        }



        public override void OnTransactionStart(RpcServerContext context) {
            RpcServiceMethod method;
            if (!_methods.TryGetValue(context.MethodName, out method)) {
                throw new RpcException("TransactionStart", _serviceUrl, RpcErrorCode.MethodNotFound, null);
            }
            try {
                method.RatePerSecond.Increment();
                method.TotalCount.Increment();
                method.Concurrent.Increment();

                object[] objs = method.GetArgs(context);
                method.Method.Invoke(_serviceObj, objs);
            }
            catch (Exception ex) {
                context.ReturnError(RpcErrorCode.ServerError, ex);
                method.TotalFailed.Increment();
            }
            finally {
                method.Concurrent.Decrement();
            }
        }

        private string _serviceUrl;
        private object _serviceObj;
        private Dictionary<string, RpcServiceMethod> _methods = new Dictionary<string, RpcServiceMethod>();

    }
    internal class RpcServiceMethod {
        public IICPerformanceCounter RatePerSecond;
        public IICPerformanceCounter TotalCount;
        public IICPerformanceCounter TotalFailed;
        public IICPerformanceCounter Concurrent;
        public RpcGetArgsDelegate GetArgs;
        public MethodInfo Method;
    }
    public delegate object[] RpcGetArgsDelegate(RpcServerContext context);

	[Obsolete("NextVersion NotSure", true)]
    public class RpcResult2<T> {
        public RpcResult2(Action<T> action) {
            Callback = action;
        }
        public Action<T> Callback;
        public RpcServerContext Context;

        public static implicit operator RpcResult2<T>(Action<T> action) {
            return new RpcResult2<T>(action);
        }

        public Action<Exception> ExceptionHandler { get; set; }
    }
}
