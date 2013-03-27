using System;
using System.Collections.Generic;
namespace it.jodan.SpoolPad.BaseClasses.Configuration {
	public class DataContextConfig {
		public bool AutoGen { get; set; }
		public bool Enabled { get; set; }
		public IList<string> UsingNamespaces { get; set; }
		public IList<string> References { get; set; }

		public DataContextConfig() {
			UsingNamespaces = new List<string>();
			References = new List<string>();
		}

		public override string ToString() {
			return string.Format("[DataContextConfig: AutoGen={0}, Enabled={1}, UsingNamespaces={2}, References={3}]", AutoGen, Enabled, UsingNamespaces, References);
		}
	}
}

