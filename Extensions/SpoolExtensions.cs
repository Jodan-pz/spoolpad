using System.Collections.Generic;
using System.Text;
using it.jodan.SpoolPad.BaseClasses;

namespace it.jodan.SpoolPad.Extensions {
	internal class SpoolItem {
		internal enum SpoolItemType {
			Value,
			Comment
		}
		public object ItemValue { get; set; }
		public SpoolItemType ItemType { get; set; }
		public SpoolItem( SpoolItemType type, object itemValue ) {
			ItemValue = itemValue;
			ItemType = type;
		}
	}

	public static class SpoolExtensions {
		static object _lock = new object();
		static Dictionary<ISpoolerService, IList<SpoolItem>> _items = new Dictionary<ISpoolerService, IList<SpoolItem>>();

		static SpoolExtensions() {
			lock (_lock) {
				foreach (ISpoolerService service in SpoolerHelper.Spoolers) {
					_items.Add(service, new List<SpoolItem>());
				}
			}
		}

		public static T ClearSpool<T>( this T obj ) {
			lock (_lock) {
				foreach (ISpoolerService service in SpoolerHelper.Spoolers) {
					_items[service].Clear();
					service.Clear();
				}
			}
			return obj;
		}

		public static T Spool<T>( this T obj, string title ) {
			lock (_lock) {
				foreach (ISpoolerService service in SpoolerHelper.Spoolers) {
					if (title != null) {
						_items[service].Add(new SpoolItem(SpoolItem.SpoolItemType.Comment, title));
					}
					_items[service].Add(new SpoolItem(SpoolItem.SpoolItemType.Value, obj));
					
					if (service.AutoFlush) {
						FlushByService(service);
					}
				}
			}
			return obj;
		}

		public static T Spool<T>( this T obj ) {
			return Spool(obj, null);
		}

		public static T FlushSpool<T>( this T obj ) {
			lock (_lock) {
				foreach (ISpoolerService service in SpoolerHelper.Spoolers) {
					if (_items[service].Count > 0)
						FlushByService(service);
				}
			}
			return obj;
		}

		private static void FlushByService( ISpoolerService service ) {
			StringBuilder sb = new StringBuilder();
			
			foreach (SpoolItem item in _items[service]) {
				string toAppend = null;
				switch (item.ItemType) {
				case SpoolItem.SpoolItemType.Comment:
					{
						toAppend = service.FormatTitle(item.ItemValue as string);
						break;
					}
				case SpoolItem.SpoolItemType.Value:
					{
						toAppend = service.ObjectToString(item.ItemValue);
						break;
					}
				}
				if (toAppend != null)
					sb.AppendLine(toAppend);
			}
			
			if (sb.Length > 0)
				service.AppendText(sb.ToString());
			
			_items[service].Clear();
		}
	}
}