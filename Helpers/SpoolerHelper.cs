using System.Collections.Generic;
using System.Collections.Specialized;
using Equisetum2.Common.Helpers;
using it.jodan.SpoolPad.BaseClasses;

using it.jodan.SpoolPad.Extensions;
using it.jodan.SpoolPad.RuntimeConfiguration;

namespace it.jodan.SpoolPad {
	public static class SpoolerHelper {
		const string SECTION = "spoolers";
		const string KEY_SPOOLER = "spooler";
		static readonly IEnumerable<ISpoolerService> _services;

		static SpoolerHelper() {
			NameValueCollection section = NameValueMultipleSectionHandler.GetConfig(SECTION);
						
			List<ISpoolerService> temp = new List<ISpoolerService>();
			
			if (section == null) {
				// not specified, use only the first defined (at least one must exist in core spring configuration)
				temp.Add(ServiceHelper.GetService<ISpoolerService>());
			} else {
				foreach (string spooler in section.GetValues(KEY_SPOOLER)) {
					temp.Add(ServiceHelper.GetService<ISpoolerService>(spooler));
				}
			}
			_services = temp;
		}

		public static IEnumerable<ISpoolerService> Spoolers {
			get { return _services; }
		}
	}
}

