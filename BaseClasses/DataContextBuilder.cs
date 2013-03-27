using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using Common.Logging;

using Equisetum2.Common.Helpers;
using Equisetum2.NHibernate;

using Microsoft.CSharp;
using System.Reflection;
using System.Xml;
using it.jodan.SpoolPad.BaseClasses.Configuration;

namespace it.jodan.SpoolPad.BaseClasses {
	public class DataContextBuilder {
		readonly ILog _log = LogManager.GetLogger(typeof(DataContextBuilder));
		static string _rebuildFactory = "N";
		string dcAutoGenFile;
		PadConfig _config = null;

		public DataContextBuilder() {
		}

		public DataContextBuilder( PadConfig config ) {
			_config = config;
		}

		public void Build( PadConfig config ) {
			_config = config;
			Build();
		}

		public void Build() {
			if (_config == null || !_config.DataContext.Enabled)
				return;
			
			string domainGeneratorName = string.Format("{0}_SpoolPadDomainGenerator", _config.GetCurrentLang());
			IDomainGenerator dgen = ServiceHelper.GetService<IDomainGenerator>(domainGeneratorName);
			if (dgen == null) {
				_log.ErrorFormat("Domain generator cannot be resolved: {0}", domainGeneratorName);
				return;
			}
			String dataContextCode = dgen.GenerateDomain(_config);
			
			Dictionary<string, string> options = new Dictionary<string, string> { { "CompilerVersion", "v3.5" } };
			CodeDomProvider codeProvider = dgen.GetCodeProvider(options);
			
			if (Compile(codeProvider, dataContextCode, _config, _config.Name)) {
				// setup props...
				CustomConfiguration cfg = ServiceHelper.GetService<CustomConfiguration>();
				cfg.ConnectionString = _config.Connection.ConnectionString;
				cfg.Dialect = _config.Connection.Dialect;
				cfg.Driver = _config.Connection.Driver;
				cfg.User = _config.Connection.User;
				cfg.Password = _config.Connection.Password;
				cfg.AddMappings(_config.Name);
				cfg.IsConfigured = true;
				
				// load assemblies before factory build...
				foreach (MapConfig mc in _config.Mappings.Distinct()) {
					if (mc.IsValid && mc.IsAssembly) {
						Assembly.LoadFile(mc.Assembly);
					}
				}
				
				SessionFactory factory = ServiceHelper.GetService<SessionFactory>();
				lock (_rebuildFactory) {
					if (_rebuildFactory == "N") {
						_rebuildFactory = "S";
						
					} else {
						// build factory
						factory.Build();
					}
				}
			}
		}

		public void Release() {
			if (File.Exists(dcAutoGenFile)) {
				try {
					File.Delete(dcAutoGenFile);
				} catch {
				}
			}
		}

		bool Compile( CodeDomProvider provider, string dataContextCode, PadConfig config, String dllFile ) {
			if (!dllFile.EndsWith(".dll"))
				dllFile += ".dll";
			
			dcAutoGenFile = dllFile;
			
			CompilerParameters cp = new CompilerParameters();
			
			// Generate a class library.
			cp.GenerateExecutable = false;
			
			// Generate debug information.
			cp.IncludeDebugInformation = false;
			
			// Add assembly references.
			cp.ReferencedAssemblies.Add("System.dll");
			cp.ReferencedAssemblies.Add("Iesi.Collections.dll");
			
			// Save the assembly as a physical file.
			cp.GenerateInMemory = false;
			
			cp.OutputAssembly = dcAutoGenFile;
			
			// Set the level at which the compiler 
			// should start displaying warnings.
			cp.WarningLevel = 3;
			
			// Set whether to treat all warnings as errors.
			cp.TreatWarningsAsErrors = false;
			
			// Set compiler argument to optimize output.
			cp.CompilerOptions = "/optimize";
			
			string workDir = Path.Combine(Path.GetTempPath(), "SpoolPad$temp$" + config.Name);
			
			if (!Directory.Exists(workDir))
				Directory.CreateDirectory(workDir);
			
			foreach (MapConfig map in config.Mappings) {
				
				if (map.IsValid) {
					if (provider.Supports(GeneratorSupport.Resources)) {
						if (!config.DataContext.AutoGen) {
							if (map.IsFile)
								cp.EmbeddedResources.Add(map.Map); else if (map.IsAssembly) {
								
								string tempFile = Path.Combine(workDir, Path.GetFileName(map.ResourceName));
								TextReader tr = new StreamReader(Assembly.ReflectionOnlyLoadFrom(map.Assembly).GetManifestResourceStream(map.ResourceName));
								File.WriteAllText(tempFile, tr.ReadToEnd());
								cp.EmbeddedResources.Add(tempFile);
								
							} else
								continue;
							
						} else {
							
							XDocument doc = null;
							if (map.IsAssembly) {
								doc = XDocument.Load(new XmlTextReader(Assembly.ReflectionOnlyLoadFrom(map.Assembly).GetManifestResourceStream(map.ResourceName)));
							} else if (map.IsFile) {
								doc = XDocument.Load(map.Map);
							} else
								continue;
							
							var hibmap = doc.Root;
							if (hibmap != null) {
								hibmap.SetAttributeValue("assembly", config.Name);
								hibmap.SetAttributeValue("namespace", config.DataContextAutoGenNamespace);
								
								string tempFile = Path.Combine(workDir, Path.GetFileName(map.Map));
								doc.Save(tempFile);
								cp.EmbeddedResources.Add(tempFile);
							}
						}
					}
				}
			}
			
			
			CompilerResults cr = provider.CompileAssemblyFromSource(cp, dataContextCode);
			
			Directory.Delete(workDir, true);
			
			if (cr.Errors.Count > 0) {
				// Display compilation errors.
				_log.ErrorFormat("Errors building {0}", cr.PathToAssembly);
				foreach (CompilerError ce in cr.Errors) {
					_log.DebugFormat("  {0}", ce.ToString());
				}
			} else {
				_log.DebugFormat("Source built into {0} successfully.", cr.PathToAssembly);
			}
			
			// Return the results of compilation.
			if (cr.Errors.Count > 0) {
				return false;
			} else {
				return true;
			}
		}
	}
}