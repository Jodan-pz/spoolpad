using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using it.jodan.SpoolPad.Extensions;
using System.Threading;
using System.Security;

namespace it.jodan.SpoolPad.BaseClasses {

	public abstract class AbstractContext {
		static Dictionary<Type, IEnumerable<string>> _help = new Dictionary<Type, IEnumerable<string>>();

		protected abstract void OnRun();

		public void _run_() {
			try {
				OnRun();
			} catch (ThreadAbortException) {
			} catch (Exception ex) {
				ex.Message.Spool("[SpoolPad:RunError]");
			}
		}

		protected virtual void InternalRun() {
		}

		[DataContextCommand("Query a data context element. The current data context will do nothing!")]
		protected virtual IQueryable<T> DB<T>() {
			return new T[] {  }.AsQueryable();
		}

		protected string Help() {			
			"".Spool(SpoolPadProgram.APP_NAME + " - Current Data-Context commands: ");
			if (!_help.ContainsKey(GetType().BaseType))
				BuildHelp();
			foreach (String help in _help[GetType().BaseType])
				help.Spool();
			return String.Empty;
		}

		private void BuildHelp() {
			List<string> temp = new List<string>();
			foreach (MethodInfo mi in GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy).OrderBy(m => m.Name)) {
				if (mi.IsDefined(typeof(DataContextCommandAttribute), true)) {
					string help = " * " + mi.ToString().Replace("[T]", "<T>").Replace("`1", "<T>");
					help += "  : " + ((DataContextCommandAttribute)mi.GetCustomAttributes(typeof(DataContextCommandAttribute), true).First()).Description;
					temp.Add(help);
				}
			}
			_help.Add(GetType().BaseType, temp);
		}
	}
}