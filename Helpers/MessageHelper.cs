using System;
using Gtk;
using Common.Logging;

namespace it.jodan.SpoolPad.Helpers {
	public static class MessageHelper {
		static readonly ILog _log = LogManager.GetLogger(typeof(MessageHelper));

		public static bool ShowYesNo( string message, params string[] args ) {
			MessageDialog dlg = new MessageDialog(null, DialogFlags.Modal, Gtk.MessageType.Question, ButtonsType.YesNo, message, args);
			dlg.Title = "SpoolPad";
			ResponseType res = (ResponseType)dlg.Run();
			dlg.Destroy();
			return res == ResponseType.Yes;
		}

		public static void ShowInfo( string message, params string[] args ) {
			MessageDialog dlg = new MessageDialog(null, DialogFlags.Modal, Gtk.MessageType.Info, ButtonsType.Ok, message, args);
			dlg.Title = "SpoolPad";
			dlg.Run();
			dlg.Destroy();
		}

		public static void ShowError( Exception ex ) {
			ShowError(ex.Message);
		}

		public static void ShowError( string message, params string[] args ) {
			_log.ErrorFormat(message, args);
			MessageDialog dlg = new MessageDialog(null, DialogFlags.Modal, Gtk.MessageType.Error, ButtonsType.Ok, message, args);
			dlg.Title = "SpoolPad";
			dlg.Run();
			dlg.Destroy();
		}
	}
}

