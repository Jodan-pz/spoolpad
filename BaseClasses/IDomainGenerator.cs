using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using it.jodan.SpoolPad.BaseClasses.Configuration;
namespace it.jodan.SpoolPad.BaseClasses {
	public interface IDomainGenerator {
		CodeDomProvider GetCodeProvider( IDictionary<string, string> options );

		string GenerateDomain( PadConfig config );
	}
}
