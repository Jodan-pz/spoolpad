using Gtk;
namespace it.jodan.SpoolPad.BaseClasses {

	public interface ISpoolerService {

		void Clear();
		void AppendText( string text );

		Widget GetSpoolerWidget();

		string FormatTitle( string title );
		string ObjectToString( object obj );

		bool AutoFlush { get; }
		int GraphDepth { get; set; }
		
	}
	
}

