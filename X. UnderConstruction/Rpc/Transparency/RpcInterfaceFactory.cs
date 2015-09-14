using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Imps.Services.CommonV4
{
	[Obsolete("NextVersion NotSure", true)]
	public static class RpcInterfaceFactory
	{
		static Dictionary<Type, object> dict = new Dictionary<Type, object>();
		static void OutputResults(CompilerResults results)
		{
			_tracing.Info("NativeCompilerReturnValue=" +
				results.NativeCompilerReturnValue.ToString());
			foreach (string s in results.Output) {
				_tracing.Info(s);
			}
		}
		private static ITracing _tracing = TracingManager.GetTracing(typeof(RpcInterfaceFactory));
		public static TInterface CreateInterface<TInterface>(string ip)
			where TInterface: class
		{
			object obj;
			Type tInterface = typeof(TInterface);
			if (dict.TryGetValue(tInterface, out obj))
				return obj as TInterface;

			MethodInfo[] misInterface = tInterface.GetMethods(BindingFlags.Public
				| BindingFlags.Instance);
			List<MethodInfo> misInterfaceList = new List<MethodInfo>();
			foreach (var item in misInterface) {
				if (item.IsSpecialName == false)
					misInterfaceList.Add(item);
			}

			CodeCompileUnit CompileUnit = new CodeCompileUnit();
			CompileUnit.ReferencedAssemblies.Add(typeof(TInterface).Assembly.ManifestModule.ToString());
			CodeNamespace aName = new CodeNamespace("Imps.Generics.DynamicTypes");
			CompileUnit.Namespaces.Add(aName);
			aName.Imports.Add(new CodeNamespaceImport("System"));
			aName.Imports.Add(new CodeNamespaceImport("Imps.Services.CommonV4"));

			string className = string.Format("{0}_Proxy", tInterface.Name);
			CodeTypeDeclaration proxy = new CodeTypeDeclaration(className);
			aName.Types.Add(proxy);
			proxy.BaseTypes.Add(typeof(TInterface));

			CodeMemberField field_ip = new CodeMemberField(typeof(string), "_ip");
			proxy.Members.Add(field_ip);

			CodeConstructor con = new CodeConstructor();
			con.Attributes = MemberAttributes.Public;
			CodeFieldReferenceExpression field_ip_ref
				= new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "_ip");
			con.Statements.Add(new CodeAssignStatement(field_ip_ref, new CodePrimitiveExpression(ip)));
			proxy.Members.Add(con);


			foreach (var item in misInterfaceList) {
				CreateMethod(aName, proxy, item, typeof(TInterface));
			}
			CompilerParameters opt = new CompilerParameters(new string[]{
                                      "System.dll", 
                                      "IICCommonLibrary.dll"});
			opt.GenerateExecutable = false;
			opt.TreatWarningsAsErrors = true;
			opt.IncludeDebugInformation = true;
			opt.GenerateInMemory = true;

			CompilerResults results;
			CodeDomProvider provider = CodeDomProvider.CreateProvider("cs");

			StringWriter sw = new StringWriter();
			provider.GenerateCodeFromCompileUnit(CompileUnit, sw, null);
			string output = sw.ToString();
			_tracing.Info(output);
			sw.Close();

			results = provider.CompileAssemblyFromDom(opt, CompileUnit);

			OutputResults(results);
			if (results.NativeCompilerReturnValue != 0) {
				_tracing.Warn("Compilation failed.");
				throw new ApplicationException("Compilation failed.");
			} else {
				_tracing.Info("completed successfully.");
			}

			obj = results.CompiledAssembly
				.CreateInstance(string.Format("{0}.{1}", aName.Name, proxy.Name));
			dict[tInterface] = obj;
			return obj as TInterface;

		}

		private static void CreateMethod(CodeNamespace name, CodeTypeDeclaration proxy, MethodInfo item, Type typeInterface)
		{

			string strMethod = @"        public void {0}({1}, RpcResult2<{2}> callback)
        {{
            RpcClientProxy<{3}> clientProxy =
                RpcProxyFactory.GetProxy<{3}>(_ip);
            {4} temp_args = new {4}({5});
            clientProxy.BeginInvoke(""{0}"", temp_args,
                delegate(RpcClientContext context)
                {{                    
                    try
                    {{
                        {2} ret = context.EndInvoke<{2}>();
                        callback.Callback(ret);
                    }}
                    catch (Exception ex)
                    {{
                        Action<Exception> temp = callback.ExceptionHandler;
                        if (temp != null)
                            temp(ex);
                    }}
                }});
        }}";
			ParameterInfo[] pis = item.GetParameters();

			string strArgType = RpcGetArgsHelper.GetTypeStr2(pis);
			string strParams = RpcGetArgsHelper.GetParams(pis);
			string strParams2 = RpcGetArgsHelper.GetParams2(pis);
			Type returnType = pis[pis.Length - 1].ParameterType;
			string strReturnType = returnType.GetGenericArguments()[0].FullName;
			strMethod = string.Format(strMethod, item.Name, strParams, strReturnType,
				typeInterface.FullName, strArgType, strParams2);
			proxy.Members.Add(new CodeSnippetTypeMember(strMethod));
		}
	}
}
