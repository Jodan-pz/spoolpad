using System;
using Gtk;
using System.IO;
namespace it.jodan.SpoolPad.Helpers {
	public static class FileDialogHelper {
		public static string OpenFile( string title, params string[] extensions ) {
			FileChooserDialog fd = new FileChooserDialog(title, null, FileChooserAction.Open, Gtk.Stock.Cancel, ResponseType.Cancel, Gtk.Stock.Open, ResponseType.Accept);
			foreach (string ext in extensions) {
				string[] filter = ext.Split('|');
				FileFilter temp = new FileFilter();
				temp.Name = filter[0];
				temp.AddPattern("*." + filter[1]);
				fd.AddFilter(temp);
			}
			try {
				if (fd.Run() == (int)ResponseType.Accept) {
					return fd.Filename;
				}
			} finally {
				fd.Destroy();
			}
			return null;
		}

		public static string SaveFile( string title, string fileName, params string[] extensions ) {
			string extension = Path.GetExtension(fileName);
			FileChooserDialog fd = new FileChooserDialog(title, null, FileChooserAction.Save, Gtk.Stock.Cancel, ResponseType.Cancel, Gtk.Stock.Save, ResponseType.Accept);
			fd.CurrentName = fileName;
			foreach (string ext in extensions) {
				string[] filter = ext.Split('|');
				FileFilter temp = new FileFilter();
				temp.Name = filter[0];
				temp.AddPattern("*." + filter[1]);
				fd.AddFilter(temp);
			}
			try {
				if (fd.Run() == (int)ResponseType.Accept) {
					if (!fd.Filename.EndsWith(extension))
						return fd.Filename + "." + extension;
					return fd.Filename;
				}
			} finally {
				fd.Destroy();
			}
			return null;
		}

		public static string ChooseFolder( string title ) {
			FileChooserDialog fd = new FileChooserDialog(title, null, FileChooserAction.SelectFolder, Gtk.Stock.Cancel, ResponseType.Cancel, Gtk.Stock.Save, ResponseType.Accept);
			try {
				if (fd.Run() == (int)ResponseType.Accept) {
					return fd.Filename;
				}
			} finally {
				fd.Destroy();
			}
			return null;
		}
	}
}

