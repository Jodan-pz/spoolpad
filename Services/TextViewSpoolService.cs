using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Common.Logging;
using Gtk;
using System.Text;
using System.Runtime.CompilerServices;
using System.Collections;
using it.jodan.SpoolPad.BaseClasses;

namespace it.jodan.SpoolPad.Services {
	public class TextViewSpoolService : ISpoolerService {
		readonly TextView _textView = new TextView();

		#region ISpoolerService implementation

		public int GraphDepth { get; set; }

		public Widget GetSpoolerWidget() {
			return _textView;
		}

		public bool AutoFlush {
			get { return true; }
		}

		public string FormatTitle( string title ) {
			return "*** " + title;
		}

		public string ObjectToString( object obj ) {
			StringBuilder temp = new StringBuilder();
			if (obj != null) {
				
				if (obj is string)
					temp.Append(MakeSpool(obj, null, false));
				else {
					IEnumerable en = obj as IEnumerable;
					if (en != null) {
						object first = en.Cast<object>().FirstOrDefault();
						if (first == null)
							return null;
						temp.AppendLine(GetHeaders(first.GetType(), true));
						foreach (object item in (IEnumerable)obj) {
							temp.AppendLine(ObjectToString(item));
						}
					} else {
						temp.Append(MakeSpool(obj, null, false));
					}
				}
			}
			return temp.ToString();
		}

		public void AppendText( string text ) {
			if (_textView != null) {
				Application.Invoke((s, e) => { _textView.Buffer.InsertAtCursor(text); });
			}
		}

		public void Clear() {
			if (_textView != null) {
				Application.Invoke((s, e) => { _textView.Buffer.Clear(); });
			}
		}

		#endregion

		private string MakeSpool( object source, string text, bool createHeader ) {
			StringBuilder toSpool = new StringBuilder();
			
			Type sType = source.GetType();
			if (createHeader)
				toSpool.Append(GetHeaders(sType, false));
			
			if (sType != typeof(string) && !sType.IsValueType) {
				foreach (PropertyInfo pi in source.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)) {
					toSpool.AppendFormat("{0} | ", pi.GetValue(source, null));
				}
			} else {
				toSpool.Append(source.ToString());
			}
			return toSpool.ToString();
		}

		private string GetHeaders( Type type, bool enumerable ) {
			StringBuilder toSpool = new StringBuilder();
			if (type != typeof(string) && !type.IsValueType) {
				
				if (enumerable)
					toSpool.Append("Items Of Type ");
				
				if (CheckIfAnonymousType(type)) {
					toSpool.Append("[...]");
				} else {
					toSpool.AppendFormat("[{0}]", type.Name);
				}
				
				toSpool.Length = 0;
				foreach (PropertyInfo pi in type.GetProperties(BindingFlags.Public | BindingFlags.Instance)) {
					toSpool.AppendFormat("{0} | ", pi.Name);
				}
			}
			return toSpool.ToString();
		}

		private static bool CheckIfAnonymousType( Type type ) {
			if (type == null)
				throw new ArgumentNullException("type");
			return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false) && type.IsGenericType && type.Name.Contains("Anon") && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"));
		}
	}
	
}
