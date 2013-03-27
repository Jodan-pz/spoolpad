using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Text;
using it.jodan.SpoolPad.BaseClasses;
using Microsoft.CSharp;
using Equisetum2.Common.Helpers;
namespace it.jodan.SpoolPad.Services {
    public class CSharpSimpleExecutableCodeGenerator : IExecutableCodeGenerator {
        public CodeDomProvider GetCodeProvider( IDictionary<string, string> options ) {
            return new CSharpCodeProvider(options);
        }

        public StringBuilder GetExecutionCode( IEnumerable<string> usingNamespaces, string namespaceName, string baseClassName, string className, CodeType codeType, string execCode ) {
            string code = PrepareCode(codeType, execCode);
            string endCode = (code ?? "").TrimEnd().EndsWith("}") ? code.Contains("class") ? ";" : ";}" : ";}";
            
            StringBuilder usingNs = new StringBuilder();
            foreach (string uns in usingNamespaces)
                usingNs.AppendFormat("using {0};", uns);
            
            StringBuilder source = new StringBuilder();
            source.AppendFormat(@"

using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

using it.jodan.SpoolPad.DataContext;
using it.jodan.SpoolPad.Extensions;
{0}

namespace {1}{{
    public class {2} : {3} {{
        protected override void InternalRun(){{
        {4}{5}      
    }}
}}
", usingNs, namespaceName, className, baseClassName, code, endCode);
            
            return source;
        }


        private string PrepareCode( CodeType codeType, string source ) {
            if (CommonHelper.IsNullOrEmptyOrBlank(source))
                return null;
            
            string ret = string.Empty;
            string[] lines = source.Trim().Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            
            foreach (string line in lines) {
                string temp = line.Trim();
                if (temp.StartsWith("//"))
                    continue;
                ret += temp + "\n";
            }
            
            switch (codeType) {
                case CodeType.CSharpFastCode:{
                        if (CommonHelper.IsNullOrEmptyOrBlank(ret))
                            return string.Empty;
                        
                        if (!ret.EndsWith("/")) {
                            if (ret.StartsWith("from") && !ret.EndsWith(")"))
                                ret = "(" + ret + ")";
                            
                            if (!ret.Contains(".Spool("))
                                ret += ".Spool();";
                        }
                        break;
                }
            }
            return ret;
        }
    }
}