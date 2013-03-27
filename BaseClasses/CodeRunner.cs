using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Common.Logging;
using System.CodeDom.Compiler;
using Equisetum2.Common.Helpers;
using System.Threading;
using it.jodan.SpoolPad.Extensions;
using it.jodan.SpoolPad.BaseClasses.Configuration;

namespace it.jodan.SpoolPad.BaseClasses {
	public class CodeRunner {
		ILog _log = LogManager.GetLogger(typeof(CodeRunner));

		PadConfig _config = null;
		DataContextBuilder _dcBuilder = new DataContextBuilder();
		bool _isStopRequest = false;
		Thread _thCodeRun;

		public PadConfig Config {
			get { return _config; }
			set { _config = value; }
		}

		public CodeRunner() {
			_config = null;
		}

		public CodeRunner( PadConfig config ) {
			_config = config;
		}

		public void Build( PadConfig config ) {
			_config = config;
			Build();
		}

		public void Build() {
			_dcBuilder.Release();
			_dcBuilder.Build(_config);
		}

		public void Release() {
			_isStopRequest = false;
			_dcBuilder.Release();
		}

		public void Run( PadConfig config ) {
			_config = config;
			Run();
		}

		public void Stop() {
			_isStopRequest = true;
			if (_thCodeRun != null && _thCodeRun.IsAlive)
				_thCodeRun.Abort();
		}

		public bool IsRunning {
			get { return _thCodeRun != null && _thCodeRun.IsAlive; }
		}

		public void Run() {
			_isStopRequest = false;
			
			if (_config == null)
				return;
			
			if (CommonHelper.IsNullOrEmptyOrBlank(_config.Code))
				return;
			
			string codeExecName = string.Format("{0}_SpoolPadCodeExecGenerator", _config.GetCurrentLang());
			IExecutableCodeGenerator codeExecGen = ServiceHelper.GetService<IExecutableCodeGenerator>(codeExecName);
			if (codeExecGen == null) {
				_log.ErrorFormat("Executable code generator cannot be resolved: {0}", codeExecName);
				return;
			}
			
			Dictionary<string, string> options = new Dictionary<string, string> { { "CompilerVersion", "v3.5" } };
			CodeDomProvider codeProvider = codeExecGen.GetCodeProvider(options);
			CompilerParameters loParams = new CompilerParameters();
			loParams.ReferencedAssemblies.Add("System.dll");
			loParams.ReferencedAssemblies.Add("System.Core.dll");
			loParams.ReferencedAssemblies.Add("System.dll");
			loParams.ReferencedAssemblies.Add("Equisetum2.Common.dll");
			loParams.ReferencedAssemblies.Add("Equisetum2.NHibernate.dll");
			loParams.ReferencedAssemblies.Add("SpoolPad.exe");
			
			_config.AddAllReferences(loParams.ReferencedAssemblies);
			
			string unqMethodId = DateTime.Now.ToString("yyyyMMddHHmmss");
						
			StringBuilder source = codeExecGen.GetExecutionCode(_config.GetAllUsingNamespaces(), 
			                                                    "____jodan._code_exec", 
			                                                    _config.DataContextBaseClass, 
			                                                    "____jodan_class_" + unqMethodId, 
			                                                    _config.CodeType, 
			                                                    _config.Code);
			_log.Debug(source.ToString());
			
			loParams.GenerateInMemory = true;
			
			if (_isStopRequest)
				return;
			
			CompilerResults loCompiled = codeProvider.CompileAssemblyFromSource(loParams, source.ToString());
			if (loCompiled.Errors.HasErrors) {
				string lcErrorMsg = "";
				lcErrorMsg = loCompiled.Errors.Count.ToString() + " Errors:";
				for (int x = 0; x < loCompiled.Errors.Count; x++)
					lcErrorMsg = lcErrorMsg + "\r\n -> " + loCompiled.Errors[x].ErrorText;
				throw new Exception(lcErrorMsg);
			}
			
			Assembly loAssembly = loCompiled.CompiledAssembly;
			object loObject = loAssembly.CreateInstance("____jodan._code_exec.____jodan_class_" + unqMethodId);
			if (loObject == null) {
				throw new Exception("Error. Cannot create class.");
			}
			
			if (_isStopRequest)
				return;
			_thCodeRun = new Thread(new ParameterizedThreadStart(DoCode));
			_thCodeRun.Start(loObject);
			_thCodeRun.Join();
		}

		private void DoCode( object arg ) {
			if (arg == null)
				return;
			object[] loCodeParms = new object[] {  };
			try {
				arg.GetType().InvokeMember("_run_", BindingFlags.InvokeMethod, null, arg, loCodeParms);
			} catch {
			} finally {
				arg.FlushSpool();
			}
		}
	}
}