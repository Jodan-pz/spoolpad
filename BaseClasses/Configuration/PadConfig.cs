using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using System.Linq;
using Common.Logging;

namespace it.jodan.SpoolPad.BaseClasses.Configuration {

	public class PadConfig {

		public enum Languages {
			CSHARP,
			VBASIC,
			UNDEFINED
		}

		readonly ILog _log = LogManager.GetLogger(typeof(PadConfig));

		const string SPOOLPAD_DOMAIN = "it.jodan.SpoolPad.Domain";
		const string DATACONTEXT_AUTOGEN_DOMAIN = "it.jodan.{0}.Domain";
		const string BASE_DATA_CONTEXT_CLASS_NAME = "BaseDataContext";
		const string BASE_NO_DATA_CONTEXT_CLASS_NAME = "BaseNoDataContext";

		string _code = null;
		string _name = null;
		CodeType _type = CodeType.CSharpFastCode;
		string _fileName = null;
		List<MapConfig> _mappings = null;
		List<string> _usings = null;
		List<string> _references = null;
		ConnectionConfig _connection = null;
		DataContextConfig _dataContext = null;

		public override string ToString() {
			return string.Format("[PadConfig: Exist={0}, FileName={1}, CodeType={2}, Mappings={3}, Connection={4}, DataContext={5}, References={6}, Usings={7}, Code={8}, Name={9}, DataContextBaseClass={10}, DataContextAutoGenNamespace={11}]", Exist, FileName, CodeType, Mappings, Connection, DataContext, References, Usings, Code,
			Name, DataContextBaseClass, DataContextAutoGenNamespace);
		}

		public PadConfig() {
			Reset();
		}

		public Languages GetCurrentLang() {
			if (CodeType.ToString().ToLowerInvariant().Contains("csharp"))
				return Languages.CSHARP;
			if (CodeType.ToString().ToLowerInvariant().Contains("vbasic"))
				return Languages.VBASIC;
			return Languages.UNDEFINED;
		}

		public void Reset() {
			_code = null;
			_name = null;
			_type = CodeType.CSharpFastCode;
			_fileName = null;
			_mappings = new List<MapConfig>();
			_connection = new ConnectionConfig();
			_dataContext = new DataContextConfig();
			_usings = new List<string>();
			_references = new List<string>();
		}

		public bool Exist {
			get { return _fileName != null; }
		}

		public string FileName {
			get { return this._fileName; }
			set { _fileName = value; }
		}

		public CodeType CodeType {
			get { return this._type; }
			set { _type = value; }
		}

		public List<MapConfig> Mappings {
			get { return this._mappings; }
			set { this._mappings = value; }
		}

		public ConnectionConfig Connection {
			get { return _connection; }
			set { _connection = value; }
		}

		public DataContextConfig DataContext {
			get { return _dataContext; }
			set { _dataContext = value; }
		}

		public IList<string> References {
			get { return _references; }
		}

		public IList<string> Usings {
			get { return _references; }
		}

		public string Code {
			get { return this._code; }
			set { _code = value; }
		}

		public string Name {
			get { return this._name; }
			set { _name = value; }
		}

		public bool Save( string fileName ) {
			_fileName = fileName;
			return Save();
		}

		public bool Load( string fileName ) {
			_fileName = fileName;
			return Load();
		}

		public bool Save() {
			bool ret = false;
			
			XDocument doc = null;
			if (File.Exists(_fileName)) {
				doc = XDocument.Load(_fileName);
				if (doc.Element("spoolpad") != doc.Root) {
					_log.Error("The document root is corrupted! Regenerating new.");
					doc = null;
				}
			}
			
			if (doc == null) {
				doc = new XDocument();
				doc.AddFirst(new XElement("spoolpad"));
			}
			
			XElement nameElement = doc.Root.Element("name");
			XElement codeElement = doc.Root.Element("code");
			XElement codeTypeElement = doc.Root.Element("codetype");
			XElement connectionElement = doc.Root.Element("connection");
			XElement mappingsElement = doc.Root.Element("mappings");
			XElement usingsElement = doc.Root.Element("usings");
			XElement referencesElement = doc.Root.Element("references");
			XElement dataContextElement = doc.Root.Element("datacontext");
			
			if (nameElement == null)
				doc.Root.Add(new XElement("name", _name));
			else
				nameElement.SetValue(_name);
			
			if (connectionElement == null) {
				connectionElement = new XElement("connection");
				doc.Root.Add(connectionElement);
			}
			XElement conStrEle = connectionElement.Elements("connectionstring").SingleOrDefault();
			if (conStrEle == null)
				connectionElement.Add(new XElement("connectionstring", _connection.ConnectionString));
			else
				conStrEle.SetValue(_connection.ConnectionString);
			
			XElement userEle = connectionElement.Elements("user").SingleOrDefault();
			if (userEle == null)
				connectionElement.Add(new XElement("user", _connection.User));
			else
				userEle.SetValue(_connection.User);
			
			XElement pwdEle = connectionElement.Elements("password").SingleOrDefault();
			if (pwdEle == null)
				connectionElement.Add(new XElement("password", _connection.Password));
			else
				pwdEle.SetValue(_connection.Password);
			
			XElement driverEle = connectionElement.Elements("driver").SingleOrDefault();
			if (driverEle == null)
				connectionElement.Add(new XElement("driver", _connection.Driver));
			else
				driverEle.SetValue(_connection.Driver);
			
			XElement dialectEle = connectionElement.Elements("dialect").SingleOrDefault();
			if (dialectEle == null)
				connectionElement.Add(new XElement("dialect", _connection.Dialect));
			else
				dialectEle.SetValue(_connection.Dialect);
			
			if (mappingsElement == null) {
				mappingsElement = new XElement("mappings");
				doc.Root.Add(mappingsElement);
			}
			
			mappingsElement.RemoveAll();
			foreach (MapConfig map in _mappings.Distinct()) {
				if (map.IsFile)
					mappingsElement.Add(new XElement("file", map.Map));
				if (map.IsAssembly) {
					mappingsElement.Add(new XElement("assembly", map.Assembly));
				}
			}
			
			if (usingsElement == null) {
				usingsElement = new XElement("usings");
				doc.Root.Add(usingsElement);
			}
			
			usingsElement.RemoveAll();
			foreach (string usn in _usings)
				usingsElement.Add(new XElement("namespace", usn));
			
			if (referencesElement == null) {
				referencesElement = new XElement("references");
				doc.Root.Add(referencesElement);
			}
			
			referencesElement.RemoveAll();
			foreach (string reference in _references)
				referencesElement.Add(new XElement("assembly", reference));
			
			if (codeTypeElement == null)
				doc.Root.Add(new XElement("codetype", (int)_type));
			else
				codeTypeElement.SetValue((int)_type);
			
			if (codeElement == null)
				doc.Root.Add(new XElement("code", new XCData(_code)));
			else
				codeElement.SetValue(new XCData(_code));
			
			if (dataContextElement == null) {
				dataContextElement = new XElement("datacontext");
				doc.Root.Add(dataContextElement);
			}
			
			dataContextElement.RemoveAll();
			dataContextElement.SetAttributeValue("autogen", _dataContext.AutoGen);
			dataContextElement.SetAttributeValue("enabled", _dataContext.Enabled);
			foreach (string usin in _dataContext.UsingNamespaces) {
				XElement usingToAdd = new XElement("using");
				usingToAdd.SetAttributeValue("namespace", usin);
				dataContextElement.Add(usingToAdd);
			}
			foreach (string reference in _dataContext.References) {
				XElement refToAdd = new XElement("reference");
				refToAdd.SetAttributeValue("assembly", reference);
				dataContextElement.Add(refToAdd);
			}
			
			doc.Save(_fileName);
			ret = true;
			return ret;
		}

		public bool Load() {
			bool ret = false;
			XDocument doc = XDocument.Load(_fileName);
			
			if (doc.Element("spoolpad") != doc.Root) {
				_log.Error("The document root is corrupted!");
				return false;
			}
			
			var dataContext = doc.Root.Elements("datacontext").SingleOrDefault();
			
			if (dataContext != null) {
				_dataContext = new DataContextConfig();
				_dataContext.AutoGen = bool.Parse(dataContext.Attribute("autogen").Value);
				_dataContext.Enabled = bool.Parse(dataContext.Attribute("enabled").Value);
				
				if (dataContext.HasElements) {
					var usingsDataContext = dataContext.Elements("using");
					if (usingsDataContext != null) {
						foreach (XElement u in usingsDataContext) {
							_dataContext.UsingNamespaces.Add(u.Attribute("namespace").Value);
						}
					}
					var referencesDataContext = dataContext.Elements("reference");
					if (referencesDataContext != null) {
						foreach (XElement r in referencesDataContext) {
							string refAsm = r.Attribute("assembly").Value;
							string pattern = Path.GetFileName(refAsm);
							string fileSystemFolderAssembly = Path.GetDirectoryName(refAsm);
							if (string.IsNullOrEmpty(fileSystemFolderAssembly)) {
								_dataContext.References.Add(refAsm);
							} else {
								foreach (string reffile in Directory.GetFiles(Path.GetDirectoryName(refAsm), pattern, SearchOption.TopDirectoryOnly))
									_dataContext.References.Add(reffile);
							}
						}
					}
				}
			}
			
			var name = doc.Root.Elements("name").SingleOrDefault();
			if (name != null)
				_name = name.Value;
			
			var code = doc.Root.Elements("code").SingleOrDefault();
			if (code != null)
				_code = code.Value;
			
			var type = doc.Root.Elements("codetype").SingleOrDefault();
			if (type != null)
				_type = (CodeType)int.Parse(type.Value);
			
			var connInfo = doc.Root.Elements("connection").SingleOrDefault();
			if (connInfo != null) {
				if (connInfo.HasElements) {
					_connection = new ConnectionConfig();
					var val = connInfo.Elements("connectionstring").SingleOrDefault();
					if (val != null)
						_connection.ConnectionString = val.Value;
					val = connInfo.Elements("user").SingleOrDefault();
					if (val != null)
						_connection.User = val.Value;
					val = connInfo.Elements("password").SingleOrDefault();
					if (val != null)
						_connection.Password = val.Value;
					val = connInfo.Elements("driver").SingleOrDefault();
					if (val != null)
						_connection.Driver = val.Value;
					val = connInfo.Elements("dialect").SingleOrDefault();
					if (val != null)
						_connection.Dialect = val.Value;
					val = connInfo.Elements("showsql").SingleOrDefault();
					if (val != null)
						_connection.ShowSql = val.Value;
				}
			}
			
			var mappings = doc.Root.Elements("mappings").SingleOrDefault();
			if (mappings != null) {
				_mappings.Clear();
				
				foreach (XElement ele in mappings.Descendants("assembly")) {
					if (File.Exists(ele.Value)) {
						Assembly asm = Assembly.ReflectionOnlyLoadFrom(ele.Value);
						foreach (string resource in asm.GetManifestResourceNames()) {
							if (!resource.EndsWith("hbm.xml"))
								continue;
							_mappings.Add(MapConfig.FromAssemblyResource(ele.Value, resource));
						}
					}
				}
				
				foreach (XElement ele in mappings.Descendants("file")) {
					string pattern = Path.GetFileName(ele.Value);
					if (!pattern.EndsWith("hbm.xml")) {
						_log.ErrorFormat("Mapping file {0} must match *hbm.xml pattern. Skipping file.", pattern);
						continue;
					}
					foreach (string hbm in Directory.GetFiles(Path.GetDirectoryName(ele.Value), pattern, SearchOption.TopDirectoryOnly))
						_mappings.Add(MapConfig.FromFile(hbm));
				}
			}
			
			var usings = doc.Root.Elements("usings").SingleOrDefault();
			if (usings != null) {
				_usings.Clear();
				foreach (XElement usn in usings.Descendants("namespace")) {
					_usings.Add(usn.Value);
				}
			}
			
			var references = doc.Root.Elements("references").SingleOrDefault();
			if (references != null) {
				_references.Clear();
				foreach (XElement asm in references.Descendants("assembly")) {
					string pattern = Path.GetFileName(asm.Value);
					string fileSystemFolderAssembly = Path.GetDirectoryName(asm.Value);
					if (string.IsNullOrEmpty(fileSystemFolderAssembly)) {
						_references.Add(asm.Value);
					} else {
						foreach (string reffile in Directory.GetFiles(Path.GetDirectoryName(asm.Value), pattern, SearchOption.TopDirectoryOnly))
							_references.Add(reffile);
					}
				}
			}
			
			ret = true;
			return ret;
		}

		public string DataContextBaseClass {
			get { return DataContext.Enabled ? BASE_DATA_CONTEXT_CLASS_NAME : BASE_NO_DATA_CONTEXT_CLASS_NAME; }
		}


		public string DataContextAutoGenNamespace {
			get { return string.Format(DATACONTEXT_AUTOGEN_DOMAIN, Name); }
		}

		public IEnumerable<string> GetAllUsingNamespaces() {
			return GetUsingNamespaces().Union(GetDataContextUsingNamespaces());
		}

		public void AddAllReferences( IList refs ) {
			AddReferences(refs);
			AddDataContextReferences(refs);
		}

		#region DataContext Usings && References
		public IEnumerable<string> GetDataContextUsingNamespaces() {
			// default dummy domain
			if (!DataContext.Enabled)
				return new string[] { SPOOLPAD_DOMAIN };
			// AutoGenerated data-context
			if (DataContext.AutoGen) {
				return new string[] { DataContextAutoGenNamespace };
			} else {
				// user defined...
				return DataContext.UsingNamespaces;
			}
		}

		public void AddDataContextReferences( IList refs ) {
			if (!DataContext.Enabled)
				return;
			if (DataContext.AutoGen)
				refs.Add(Name + ".dll");
			else {
				foreach (string refFile in DataContext.References)
					refs.Add(refFile);
			}
		}
		#endregion

		#region Config Usings && References
		public void AddReferences( IList refs ) {
			foreach (string reference in References)
				refs.Add(reference);
		}

		public IEnumerable<string> GetUsingNamespaces() {
			return Usings;
		}
		#endregion
	}
}
