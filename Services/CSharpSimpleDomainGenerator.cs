using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Xml.Linq;

using it.jodan.SpoolPad.BaseClasses;

using Microsoft.CSharp;

namespace it.jodan.SpoolPad.Services {

	public class CSharpSimpleDomainGenerator : AbstractDomainGenerator {
		public override CodeDomProvider GetCodeProvider( IDictionary<string, string> options ) {
			return new CSharpCodeProvider(options);
		}

		protected override StringBuilder CreateDomainUsings() {
			return new StringBuilder(@"using System;
										using Iesi.Collections.Generic;");
		}

		protected override StringBuilder CreateDomainCode( string namespaceName, string classesCode ) {
			StringBuilder dc = new StringBuilder();
			dc.AppendFormat(@"namespace {0}{{
									{1}
								}}", namespaceName, classesCode);
			return dc;
		}

		protected override StringBuilder CreateClassCode( XElement classMapping ) {
			StringBuilder code = new StringBuilder();
			
			code.AppendFormat(@" public class {0} {{ ", classMapping.Attribute("name").Value);
			
			foreach (var temp in classMapping.Elements()) {
				
				string locName = temp.Name.LocalName;
				
				if (locName.Equals("many-to-one")) {
					code.AppendFormat(@"public virtual {0} {1} {{ get; set; }}", temp.Attribute("class").Value, temp.Attribute("name").Value);
				} else if (locName.Equals("property") || locName.Equals("id")) {
					var pName = temp.Attribute("name");
					var pType = temp.Attribute("type");
					if ( pName != null ){
						string type = pType != null?pType.Value:null;
						code.AppendFormat(@"public virtual {0} {1} {{ get; set; }}", DecodeHibType( type ) , pName.Value);
					}
				} else if (locName.Equals("set") || locName.Equals("bag")) {
					var setType = (from e in temp.Elements()
						where e.Name.LocalName.Equals("one-to-many")
						select e.Attribute("class")).SingleOrDefault();
					if (setType != null) {
						code.AppendFormat(@"public virtual {0}<{1}> {2} {{ get; set; }}", locName.Equals("set") ? "ISet" : "IList", setType.Value, temp.Attribute("name").Value);
					}
				}
			}
			
			code.Append(" }");
			
			return code;
		}

		string DecodeHibType( string hibType ) {		
			if ( String.IsNullOrEmpty(hibType )) return "System.String";
			return "System." + hibType;
		}
		
	}
}

