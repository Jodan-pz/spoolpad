using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Text;
namespace it.jodan.SpoolPad.BaseClasses {
	public interface IExecutableCodeGenerator {
		CodeDomProvider GetCodeProvider( IDictionary<string, string> options );

		StringBuilder GetExecutionCode( IEnumerable<string> usingNamespaces, string namespaceName, string baseClassName, string className, CodeType codeType, string execCode );
	}
}

