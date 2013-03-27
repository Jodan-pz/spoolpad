using System.Collections;
using System.Text;
using Gtk;
using System;
using System.Reflection;
using it.jodan.SpoolPad.BaseClasses;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Collections.Generic;
using System.Security;

namespace it.jodan.SpoolPad.Services {
	public class HtmlViewSpoolService : ISpoolerService {
				
		readonly WebKit.WebView _webView = new WebKit.WebView();
        int level = 0;

		#region ISpoolerService implementation

		public int GraphDepth { get; set; }

		public Widget GetSpoolerWidget() {
			return _webView;
		}

		public bool AutoFlush {
			get { return false; }
		}

		public string FormatTitle( string title ) {
			return "<h3><font color=green>" + SecurityElement.Escape(title) + "</font></h3>";
		}

		public string ObjectToString( object obj ) {
			return  "<pre>" +  ObjectToHtml(obj, GraphDepth) +  "</pre>";
		}

		public void AppendText( string text ) {
			if (_webView != null) {
				Application.Invoke((s, e) => { _webView.LoadHtmlString(text, null); });
			}
		}

		public void Clear() {
			AppendText(string.Empty);
		}

		#endregion
        		
		string ObjectToHtml( object obj, int depth ) {
			try {
				StringBuilder temp = new StringBuilder();
				
				if (obj == null || obj is ValueType || obj is string) {
					
					temp.AppendLine(GetValue(obj) + "<br/>");
					
				} else {
					
					IEnumerable enumerableElement = obj as IEnumerable;
					
					if (enumerableElement != null) {
						
						Type[] genArg = enumerableElement.GetType().GetGenericArguments();
						
						bool isGenericInterface = genArg.Length == 1 && genArg[0].IsInterface;
						
						bool first = true;
						bool isSimpleType = false;
						
						foreach (var item in enumerableElement) {
							Type innerElementType = isGenericInterface ? genArg[0] : item.GetType();
							PropertyInfo[] properties = innerElementType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
							
							if (first) {
								first = false;
								
								temp.AppendFormat("<hr/><h3 align=\"center\" style=\" background-color:#99CCCC;; color:white\">Items of type: {0}</h3>", IsAnonymousType(innerElementType) ? "[Generated]" : GetObjectTypeName(innerElementType));
								temp.Append("<table style=\"width:100%\" 	border=\"1\">");
								
								isSimpleType = properties.Count() == 0 || innerElementType.IsValueType || innerElementType == typeof(string);
								
								if (!isSimpleType) {
									temp.Append("<thead style=\"background-color: #BDFCC9;\"><tr>");
									if (isGenericInterface) {
										temp.AppendFormat("<th style=\"width:{0}%\">[Type Name]</th>", 100 / properties.Count() + 1);
									}
									foreach (PropertyInfo m in properties) {
										temp.AppendFormat("<th style=\"width:{0}%\">{1}</th>", 100 / properties.Count() + 1, m.Name);
									}
								}
								temp.Append("<tbody>");
							}
							
							temp.Append("<tr>");
							
							if (isGenericInterface) {
								temp.AppendFormat("<td><I>{0}</I></td>", item.GetType().Name);
							}
							
							if (item is IEnumerable && !(item is string)) {
								if (level < depth) {
									level++;
									temp.AppendFormat("{0}", temp.Append(ObjectToHtml(item, depth)));
									level--;
								}
							} else {
								if (!isSimpleType) {
									foreach (PropertyInfo propInfo in properties) {
										if (level < depth) {
											level++;
                                            object propValue = null;
                                            try{
                                                propValue = propInfo.GetValue(item, null);
                                            }catch(Exception ex){
                                                propValue = "[Err:" + ex.Message + "]";
                                            }

											temp.AppendFormat("<td>{0}</td>", ObjectToHtml(propValue, depth));
											level--;
										}
									}
								} else {
									temp.AppendFormat("<td>{0}</td>", item);
								}
							}
							temp.Append("</tr>");
						}
						
						temp.Append("</tbody></table><hr/>");
						
					} else {
						
						Type oType = obj.GetType();
						temp.AppendFormat("<hr/><h3 align=\"center\" style=\" background-color:#99CCCC; color:white\">{0}</h3>", IsAnonymousType(oType) ? "[Generated]" : GetObjectTypeName(obj.GetType()));
						temp.Append("<table style=\"width:100%\" border=\"1\"><thead style=\"background-color: #BDFCC9;\"><tr><th style=\"width:50%\">Name</th><th style=\"width:50%\">Value</th></tr></thead><tbody>");
						
						PropertyInfo[] properties = oType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
						
						foreach (PropertyInfo propInfo in properties) {
                            if (level < depth) {
                                level++;
                                object val = null;
                                try{
    							    val = propInfo.GetValue(obj, null);
                                }catch(Exception ex){
                                    val = "[Err:" + ex.Message + "]";
                                }
    							temp.AppendFormat("<tr><td>{0}</td>", propInfo.Name);
    							if (propInfo.PropertyType.IsValueType || propInfo.PropertyType == typeof(string)) {
    								temp.AppendFormat("<td>{0} (<B>{1}</B>)</td></tr>", val, val.GetType().Name);
    							} else {
    								temp.AppendFormat("<td>{0}</td></tr>", ObjectToHtml(val, depth));
    							}
                                level--;
                            }
						}
						
						temp.Append("</tbody></table><hr/>");
					}
				}
				return temp.ToString();
			} catch (Exception e) {
				return e.ToString();
			}
		}

		string GetObjectTypeName( Type type ) {
			if (type.Name.EndsWith("Proxy") && type.BaseType != typeof(object)) {
				return type.BaseType.Name;
			}
			return type.Name;
		}

		public static bool IsAnonymousType( Type type ) {
			Boolean hasCompilerGeneratedAttribute = type.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Count() > 0;
			Boolean nameContainsAnonymousType = type.FullName.Contains("AnonymousType") || type.FullName.Contains("AnonType");
			Boolean isAnonymousType = hasCompilerGeneratedAttribute && nameContainsAnonymousType;
			
			return isAnonymousType;
		}

		private string GetValue( object o ) {
			if (o == null) {
				return "null";
			} else if (o is DateTime) {
				return ((DateTime)o).ToString("dd-MM-yyyy HH:mm:ss");
			} else if (o is ValueType || o is string) {
				return SecurityElement.Escape(o.ToString());
			} else if (o is IEnumerable) {
				return ("...");
			} else {
				return "{ }";
			}
		}
	}
}