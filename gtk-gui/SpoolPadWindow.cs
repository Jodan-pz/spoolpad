
// This file has been generated by the GUI designer. Do not modify.

public partial class SpoolPadWindow
{
	private global::Gtk.UIManager UIManager;
	private global::Gtk.Action FileAction;
	private global::Gtk.Action EditAction;
	private global::Gtk.Action newAction;
	private global::Gtk.Action openAction;
	private global::Gtk.Action saveAction;
	private global::Gtk.Action saveAsAction;
	private global::Gtk.Action quitAction;
	private global::Gtk.Action cutAction;
	private global::Gtk.Action copyAction;
	private global::Gtk.Action pasteAction;
	private global::Gtk.Action runCodeAction;
	private global::Gtk.Action stopAction;
	private global::Gtk.VBox vboxMain;
	private global::Gtk.MenuBar menubar;
	private global::Gtk.Toolbar toolbarOptions;
	private global::Gtk.ComboBox cboCodeType;
	private global::Gtk.VPaned vpanedContainer;
	private global::Gtk.VBox vboxCode;
	private global::Gtk.EventBox codeLabelEventBox;
	private global::Gtk.Label lblCode;
	private global::Gtk.ScrolledWindow scrollWndCode;
	private global::Gtk.VBox vboxResult;
	private global::Gtk.EventBox resultLabelEventBox;
	private global::Gtk.Label lblResult;
	private global::Gtk.ScrolledWindow scrollWndResults;
	private global::Gtk.Statusbar statusbar;
	private global::Gtk.Label lblTime;
	private global::Gtk.Label lblConnInfo;
	private global::Gtk.Label lblStatus;
	
	protected virtual void Build ()
	{
		global::Stetic.Gui.Initialize (this);
		// Widget SpoolPadWindow
		this.UIManager = new global::Gtk.UIManager ();
		global::Gtk.ActionGroup w1 = new global::Gtk.ActionGroup ("Default");
		this.FileAction = new global::Gtk.Action ("FileAction", global::Mono.Unix.Catalog.GetString ("File"), null, null);
		this.FileAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("File");
		w1.Add (this.FileAction, null);
		this.EditAction = new global::Gtk.Action ("EditAction", global::Mono.Unix.Catalog.GetString ("Edit"), null, null);
		this.EditAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("Edit");
		w1.Add (this.EditAction, null);
		this.newAction = new global::Gtk.Action ("newAction", global::Mono.Unix.Catalog.GetString ("_New"), "", "gtk-new");
		this.newAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("_New");
		w1.Add (this.newAction, null);
		this.openAction = new global::Gtk.Action ("openAction", global::Mono.Unix.Catalog.GetString ("_Open"), "", "gtk-open");
		this.openAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("_Open");
		w1.Add (this.openAction, null);
		this.saveAction = new global::Gtk.Action ("saveAction", global::Mono.Unix.Catalog.GetString ("_Save"), "", "gtk-save");
		this.saveAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("_Save");
		w1.Add (this.saveAction, null);
		this.saveAsAction = new global::Gtk.Action ("saveAsAction", global::Mono.Unix.Catalog.GetString ("Save _As"), "", "gtk-save-as");
		this.saveAsAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("Save _As");
		w1.Add (this.saveAsAction, null);
		this.quitAction = new global::Gtk.Action ("quitAction", global::Mono.Unix.Catalog.GetString ("_Quit"), null, "gtk-quit");
		this.quitAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("_Quit");
		w1.Add (this.quitAction, null);
		this.cutAction = new global::Gtk.Action ("cutAction", global::Mono.Unix.Catalog.GetString ("Cu_t"), "", "gtk-cut");
		this.cutAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("Cu_t");
		w1.Add (this.cutAction, null);
		this.copyAction = new global::Gtk.Action ("copyAction", global::Mono.Unix.Catalog.GetString ("_Copy"), "", "gtk-copy");
		this.copyAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("_Copy");
		w1.Add (this.copyAction, null);
		this.pasteAction = new global::Gtk.Action ("pasteAction", global::Mono.Unix.Catalog.GetString ("_Paste"), "", "gtk-paste");
		this.pasteAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("_Paste");
		w1.Add (this.pasteAction, null);
		this.runCodeAction = new global::Gtk.Action ("runCodeAction", global::Mono.Unix.Catalog.GetString ("_Run"), "", "gtk-go-forward");
		this.runCodeAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("_Run");
		w1.Add (this.runCodeAction, "F5");
		this.stopAction = new global::Gtk.Action ("stopAction", global::Mono.Unix.Catalog.GetString ("_Stop"), null, "gtk-stop");
		this.stopAction.Sensitive = false;
		this.stopAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("_Stop");
		w1.Add (this.stopAction, "<Shift>F5");
		this.UIManager.InsertActionGroup (w1, 0);
		this.AddAccelGroup (this.UIManager.AccelGroup);
		this.Name = "SpoolPadWindow";
		this.Title = global::Mono.Unix.Catalog.GetString ("SpoolPad");
		this.WindowPosition = ((global::Gtk.WindowPosition)(1));
		this.BorderWidth = ((uint)(2));
		// Container child SpoolPadWindow.Gtk.Container+ContainerChild
		this.vboxMain = new global::Gtk.VBox ();
		this.vboxMain.Name = "vboxMain";
		this.vboxMain.Spacing = 6;
		// Container child vboxMain.Gtk.Box+BoxChild
		this.UIManager.AddUiFromString ("<ui><menubar name='menubar'><menu name='FileAction' action='FileAction'><menuitem name='newAction' action='newAction'/><menuitem name='openAction' action='openAction'/><menuitem name='saveAction' action='saveAction'/><menuitem name='saveAsAction' action='saveAsAction'/><separator/><menuitem name='quitAction' action='quitAction'/></menu><menu name='EditAction' action='EditAction'><menuitem name='cutAction' action='cutAction'/><menuitem name='copyAction' action='copyAction'/><menuitem name='pasteAction' action='pasteAction'/><separator/><menuitem name='runCodeAction' action='runCodeAction'/><menuitem name='stopAction' action='stopAction'/></menu></menubar></ui>");
		this.menubar = ((global::Gtk.MenuBar)(this.UIManager.GetWidget ("/menubar")));
		this.menubar.Name = "menubar";
		this.vboxMain.Add (this.menubar);
		global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.vboxMain [this.menubar]));
		w2.Position = 0;
		w2.Expand = false;
		w2.Fill = false;
		// Container child vboxMain.Gtk.Box+BoxChild
		this.UIManager.AddUiFromString ("<ui><toolbar name='toolbarOptions'><toolitem name='newAction' action='newAction'/><toolitem name='openAction' action='openAction'/><toolitem name='saveAction' action='saveAction'/><toolitem name='saveAsAction' action='saveAsAction'/><separator/><toolitem name='cutAction' action='cutAction'/><toolitem name='copyAction' action='copyAction'/><toolitem name='pasteAction' action='pasteAction'/><separator/><toolitem name='runCodeAction' action='runCodeAction'/><toolitem name='stopAction' action='stopAction'/><separator/></toolbar></ui>");
		this.toolbarOptions = ((global::Gtk.Toolbar)(this.UIManager.GetWidget ("/toolbarOptions")));
		this.toolbarOptions.Name = "toolbarOptions";
		this.toolbarOptions.ShowArrow = false;
		this.toolbarOptions.ToolbarStyle = ((global::Gtk.ToolbarStyle)(2));
		this.toolbarOptions.IconSize = ((global::Gtk.IconSize)(2));
		this.vboxMain.Add (this.toolbarOptions);
		global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.vboxMain [this.toolbarOptions]));
		w3.Position = 1;
		w3.Expand = false;
		w3.Fill = false;
		// Container child vboxMain.Gtk.Box+BoxChild
		this.cboCodeType = global::Gtk.ComboBox.NewText ();
		this.cboCodeType.Name = "cboCodeType";
		this.vboxMain.Add (this.cboCodeType);
		global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.vboxMain [this.cboCodeType]));
		w4.Position = 2;
		w4.Expand = false;
		w4.Fill = false;
		// Container child vboxMain.Gtk.Box+BoxChild
		this.vpanedContainer = new global::Gtk.VPaned ();
		this.vpanedContainer.CanFocus = true;
		this.vpanedContainer.Name = "vpanedContainer";
		this.vpanedContainer.Position = 219;
		// Container child vpanedContainer.Gtk.Paned+PanedChild
		this.vboxCode = new global::Gtk.VBox ();
		this.vboxCode.Name = "vboxCode";
		this.vboxCode.Spacing = 6;
		// Container child vboxCode.Gtk.Box+BoxChild
		this.codeLabelEventBox = new global::Gtk.EventBox ();
		this.codeLabelEventBox.Events = ((global::Gdk.EventMask)(256));
		this.codeLabelEventBox.Name = "codeLabelEventBox";
		// Container child codeLabelEventBox.Gtk.Container+ContainerChild
		this.lblCode = new global::Gtk.Label ();
		this.lblCode.Name = "lblCode";
		this.lblCode.LabelProp = global::Mono.Unix.Catalog.GetString ("Code");
		this.codeLabelEventBox.Add (this.lblCode);
		this.vboxCode.Add (this.codeLabelEventBox);
		global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.vboxCode [this.codeLabelEventBox]));
		w6.Position = 0;
		w6.Expand = false;
		w6.Fill = false;
		// Container child vboxCode.Gtk.Box+BoxChild
		this.scrollWndCode = new global::Gtk.ScrolledWindow ();
		this.scrollWndCode.CanFocus = true;
		this.scrollWndCode.Name = "scrollWndCode";
		this.scrollWndCode.ShadowType = ((global::Gtk.ShadowType)(1));
		this.vboxCode.Add (this.scrollWndCode);
		global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(this.vboxCode [this.scrollWndCode]));
		w7.Position = 1;
		this.vpanedContainer.Add (this.vboxCode);
		global::Gtk.Paned.PanedChild w8 = ((global::Gtk.Paned.PanedChild)(this.vpanedContainer [this.vboxCode]));
		w8.Resize = false;
		// Container child vpanedContainer.Gtk.Paned+PanedChild
		this.vboxResult = new global::Gtk.VBox ();
		this.vboxResult.Name = "vboxResult";
		this.vboxResult.Spacing = 6;
		// Container child vboxResult.Gtk.Box+BoxChild
		this.resultLabelEventBox = new global::Gtk.EventBox ();
		this.resultLabelEventBox.Events = ((global::Gdk.EventMask)(256));
		this.resultLabelEventBox.Name = "resultLabelEventBox";
		// Container child resultLabelEventBox.Gtk.Container+ContainerChild
		this.lblResult = new global::Gtk.Label ();
		this.lblResult.Events = ((global::Gdk.EventMask)(256));
		this.lblResult.Name = "lblResult";
		this.lblResult.LabelProp = global::Mono.Unix.Catalog.GetString ("Result");
		this.resultLabelEventBox.Add (this.lblResult);
		this.vboxResult.Add (this.resultLabelEventBox);
		global::Gtk.Box.BoxChild w10 = ((global::Gtk.Box.BoxChild)(this.vboxResult [this.resultLabelEventBox]));
		w10.Position = 0;
		w10.Expand = false;
		w10.Fill = false;
		// Container child vboxResult.Gtk.Box+BoxChild
		this.scrollWndResults = new global::Gtk.ScrolledWindow ();
		this.scrollWndResults.CanFocus = true;
		this.scrollWndResults.Name = "scrollWndResults";
		this.scrollWndResults.ShadowType = ((global::Gtk.ShadowType)(1));
		this.vboxResult.Add (this.scrollWndResults);
		global::Gtk.Box.BoxChild w11 = ((global::Gtk.Box.BoxChild)(this.vboxResult [this.scrollWndResults]));
		w11.Position = 1;
		this.vpanedContainer.Add (this.vboxResult);
		this.vboxMain.Add (this.vpanedContainer);
		global::Gtk.Box.BoxChild w13 = ((global::Gtk.Box.BoxChild)(this.vboxMain [this.vpanedContainer]));
		w13.Position = 3;
		// Container child vboxMain.Gtk.Box+BoxChild
		this.statusbar = new global::Gtk.Statusbar ();
		this.statusbar.Name = "statusbar";
		this.statusbar.Spacing = 6;
		// Container child statusbar.Gtk.Box+BoxChild
		this.lblTime = new global::Gtk.Label ();
		this.lblTime.Name = "lblTime";
		this.statusbar.Add (this.lblTime);
		global::Gtk.Box.BoxChild w14 = ((global::Gtk.Box.BoxChild)(this.statusbar [this.lblTime]));
		w14.Position = 0;
		w14.Expand = false;
		w14.Fill = false;
		// Container child statusbar.Gtk.Box+BoxChild
		this.lblConnInfo = new global::Gtk.Label ();
		this.lblConnInfo.Name = "lblConnInfo";
		this.statusbar.Add (this.lblConnInfo);
		global::Gtk.Box.BoxChild w15 = ((global::Gtk.Box.BoxChild)(this.statusbar [this.lblConnInfo]));
		w15.Position = 2;
		w15.Expand = false;
		w15.Fill = false;
		// Container child statusbar.Gtk.Box+BoxChild
		this.lblStatus = new global::Gtk.Label ();
		this.lblStatus.Name = "lblStatus";
		this.lblStatus.LabelProp = global::Mono.Unix.Catalog.GetString ("Ready");
		this.statusbar.Add (this.lblStatus);
		global::Gtk.Box.BoxChild w16 = ((global::Gtk.Box.BoxChild)(this.statusbar [this.lblStatus]));
		w16.Position = 3;
		w16.Expand = false;
		w16.Fill = false;
		this.vboxMain.Add (this.statusbar);
		global::Gtk.Box.BoxChild w17 = ((global::Gtk.Box.BoxChild)(this.vboxMain [this.statusbar]));
		w17.Position = 4;
		w17.Expand = false;
		w17.Fill = false;
		this.Add (this.vboxMain);
		if ((this.Child != null)) {
			this.Child.ShowAll ();
		}
		this.DefaultWidth = 757;
		this.DefaultHeight = 482;
		this.Show ();
		this.DeleteEvent += new global::Gtk.DeleteEventHandler (this.OnDeleteEvent);
		this.newAction.Activated += new global::System.EventHandler (this.OnNewActionActivated);
		this.openAction.Activated += new global::System.EventHandler (this.OnOpenActionActivated);
		this.saveAction.Activated += new global::System.EventHandler (this.OnSaveActionActivated);
		this.saveAsAction.Activated += new global::System.EventHandler (this.OnSaveAsActionActivated);
		this.quitAction.Activated += new global::System.EventHandler (this.OnQuitActionActivated);
		this.cutAction.Activated += new global::System.EventHandler (this.OnCutActionActivated);
		this.copyAction.Activated += new global::System.EventHandler (this.OnCopyActionActivated);
		this.pasteAction.Activated += new global::System.EventHandler (this.OnPasteActionActivated);
		this.runCodeAction.Activated += new global::System.EventHandler (this.OnRunCodeActionActivated);
		this.stopAction.Activated += new global::System.EventHandler (this.OnStopActionActivated);
	}
}
