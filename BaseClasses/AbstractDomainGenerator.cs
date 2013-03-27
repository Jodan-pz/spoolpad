using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using System.Xml;
using System.CodeDom.Compiler;
using it.jodan.SpoolPad.BaseClasses.Configuration;

namespace it.jodan.SpoolPad.BaseClasses {
	public abstract class AbstractDomainGenerator : IDomainGenerator {
		public abstract CodeDomProvider GetCodeProvider( IDictionary<string, string> options );
		protected abstract StringBuilder CreateClassCode( XElement classMapping );
		protected abstract StringBuilder CreateDomainUsings();
		protected abstract StringBuilder CreateDomainCode( string namespaceName, string classesCode );

		public string GenerateDomain( PadConfig config ) {
			if (!config.DataContext.AutoGen)
				return string.Empty;
			
			StringBuilder classesCode = new StringBuilder();
			
			foreach (MapConfig map in config.Mappings) {
				XDocument mapDoc;
				if (map.IsAssembly) {
					mapDoc = XDocument.Load(new XmlTextReader(Assembly.ReflectionOnlyLoadFrom(map.Assembly).GetManifestResourceStream(map.ResourceName)));
				} else if (map.IsFile) {
					mapDoc = XDocument.Load(map.Map);
				} else
					continue;
				
				if (!mapDoc.Root.HasElements)
					continue;
				
				var clazz = mapDoc.Root.Elements().SingleOrDefault();
				if (clazz == null)
					continue;
				
				classesCode.Append(CreateClassCode(clazz));
			}
			
			StringBuilder domain = new StringBuilder();
			domain.Append(CreateDomainUsings());
			domain.Append(CreateDomainCode(config.DataContextAutoGenNamespace, classesCode.ToString()));
			return domain.ToString();
		}
	}
}