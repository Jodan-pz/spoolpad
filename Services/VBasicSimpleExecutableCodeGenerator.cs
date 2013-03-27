using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Text;
using it.jodan.SpoolPad.BaseClasses;
using Microsoft.CSharp;
using Equisetum2.Common.Helpers;
using Microsoft.VisualBasic;

namespace it.jodan.SpoolPad.Services {
	public class VBasicSimpleExecutableCodeGenerator : IExecutableCodeGenerator {
		public CodeDomProvider GetCodeProvider( IDictionary<string, string> options ) {
			return new VBCodeProvider(options);
		}

		public StringBuilder GetExecutionCode( IEnumerable<string> usingNamespaces, string namespaceName, string baseClassName, string className, CodeType codeType, string execCode ) {
			string code = PrepareCode(codeType, execCode);
			string endCode = (code ?? "").TrimEnd().ToLower().EndsWith("end sub") ? code.ToLower().Contains("class") ? Environment.NewLine : Environment.NewLine + "End Sub" : Environment.NewLine + "End Sub";
			
			StringBuilder usingNs = new StringBuilder();
			foreach (string uns in usingNamespaces)
				usingNs.AppendFormat("Imports {0}{1}", uns, Environment.NewLine);
			
			StringBuilder source = new StringBuilder();
			source.AppendFormat(@"

Imports System
Imports System.IO
Imports System.Collections.Generic
Imports System.Text.RegularExpressions
Imports System.Linq

Imports it.jodan.SpoolPad.DataContext
Imports it.jodan.SpoolPad.Extensions
{0}

namespace {1}
	Public Class {2} 
			Inherits {3} 
		Protected Overrides Sub InternalRun()
		{4}{5}		
	End Class
End namespace
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
				if (temp.StartsWith("'"))
					continue;
				ret += temp + Environment.NewLine;
			}
			
			switch (codeType) {
			case CodeType.VBasicCodeBlock:
				
				{
					break;
				}

			}
			return ret;
		}
	}
}
