using System;
using System.Threading;
using Equisetum2.Common.Helpers;
using Gdk;
using Gtk;
using GtkSourceView;
using it.jodan.SpoolPad;
using it.jodan.SpoolPad.BaseClasses;
using it.jodan.SpoolPad.BaseClasses.Configuration;
using it.jodan.SpoolPad.Extensions;
using it.jodan.SpoolPad.Helpers;
using Pango;

public partial class SpoolPadWindow : Gtk.Window {
    Cursor WAIT_CURSOR = new Cursor(CursorType.Watch);
    FontDescription _codeFont = FontDescription.FromString("Courier 14");
    FontDescription _resultFont = FontDescription.FromString("Arial 12");
    SourceStyleScheme csharpsourceSchema = SourceStyleSchemeManager.Default.GetScheme("oblivion");
    SourceStyleScheme vbsourceSchema = SourceStyleSchemeManager.Default.GetScheme("kate");
    GtkSourceView.SourceView _codeView = new GtkSourceView.SourceView();
    Widget _resultView;
    PadConfig _currentConfigPad = null;
    CodeRunner _codeRunner = null;
    Thread _thCodeRunner;
    bool _isRunning = false;
    DateTime _startTime;

    public SpoolPadWindow() : base(Gtk.WindowType.Toplevel) {
        _currentConfigPad = new PadConfig();
        _codeRunner = new CodeRunner(_currentConfigPad);
        
        foreach (ISpoolerService spoolerService in SpoolerHelper.Spoolers) {
            Widget temp = spoolerService.GetSpoolerWidget();
            if (temp != null) {
                if (!ExistResultView)
                    _resultView = temp;
                else
                    throw new Exception("The Spooling services MUST be consistent.\nMore than one UI Widget is defined!\nCannot continue.");
            }
        }
        
        Build();
        InitControls();
        Reset();
    }

    protected void OnDeleteEvent( object sender, DeleteEventArgs a ) {
        Quit();
        a.RetVal = true;
    }

    bool ExistResultView {
        get { return _resultView != null; }
    }

    void InitControls() {
        cboCodeType.AppendText("Code Style ->> Fast Code (c#)");
        cboCodeType.AppendText("Code Style ->> Code block (c#)");
        //cboCodeType.AppendText("Code Style ->> Code block (Visual Basic)");
        
        _codeView.ModifyFont(_codeFont);
        _codeView.ShowLineMarks = true;
        _codeView.ShowLineNumbers = true;
        _codeView.ShowAll();
        
        scrollWndCode.Add(_codeView);
        
        if (ExistResultView) {
            _resultView.ModifyFont(_resultFont);
            _resultView.ModifyBase(StateType.Normal, new Gdk.Color(250, 250, 210));
            _resultView.ModifyText(StateType.Normal, new Gdk.Color(0, 0, 10));
            scrollWndResults.Add(_resultView);
            _resultView.ShowAll();
        } else {
            vpanedContainer.Remove(vboxResult);
            // just code.
        }
        
        SetupCode(string.Empty);
        SetupTitle();
    }

    void SetupTitle() {
        Title = SpoolPadProgram.APP_NAME;
        if (!_currentConfigPad.Exist) {
            return;
        }
        Title += " - " + System.IO.Path.GetFileName(_currentConfigPad.FileName);
    }

    void ResetMessage() {
        InfoMessage("Ok");
    }

    void ErrorMessage( Exception ex ) {
        ErrorMessage(ex.Message);
    }

    void ErrorMessage( string error ) {
        StatusMessage(it.jodan.SpoolPad.BaseClasses.MessageType.Error, "Error!", error);
    }

    void InfoMessage( string message ) {
        StatusMessage(it.jodan.SpoolPad.BaseClasses.MessageType.Info, message, null);
    }

    void StatusMessage( it.jodan.SpoolPad.BaseClasses.MessageType type, string msg, string result ) {
        Application.Invoke((s, e) => {
            lblStatus.Text = msg; });
        
        if (result != null)
            result.Spool("[" + type + "]");
        
    }

    void Reset() {
        cboCodeType.Active = 0;
        _codeView.HasFocus = true;
        _codeView.Buffer.Clear();
        this.ClearSpool();
        ResetMessage();
        SetMouseCursor(null);
    }

    void OnCodeExec() {
        try {
            ResetTimer();
            SetupRunningCommands();
            SetMouseCursor(WAIT_CURSOR);
            InfoMessage("Executing...");
            _codeRunner.Run();
            ResetMessage();
        } catch (Exception ex) {
            ErrorMessage(ex);
            ex.FlushSpool();
        } finally {
            _isRunning = false;
            UpdateTimer();
            SetupRunningCommands();
            SetMouseCursor(null);
        }
    }

    void EnableCommands( bool enable ) {
        Application.Invoke((s, e) => {
            runCodeAction.Sensitive = newAction.Sensitive = openAction.Sensitive = saveAction.Sensitive = saveAsAction.Sensitive = quitAction.Sensitive = enable; });
    }

    void SetupRunningCommands() {
        Application.Invoke((s, e) => {
            stopAction.Sensitive = _isRunning;
            runCodeAction.Sensitive = newAction.Sensitive = openAction.Sensitive = saveAction.Sensitive = saveAsAction.Sensitive = quitAction.Sensitive = !_isRunning;
        });
    }

    void ResetTimer() {
        Application.Invoke((s, e) => {
            _startTime = DateTime.Now;
            lblTime.Text = string.Empty;
        });
    }

    void UpdateTimer() {
        Application.Invoke((s, e) => {
            TimeSpan elapsed = (DateTime.Now - _startTime);
            lblTime.Text = string.Format("Execution time: {0:00}:{1:00}:{2:000}", elapsed.TotalMinutes, elapsed.TotalSeconds, elapsed.TotalMilliseconds);
        });
    }

    void OnDCInit() {
        SetMouseCursor(WAIT_CURSOR);
        try {
            EnableCommands(false);
            Application.Invoke((s, e) => {
                lblConnInfo.Text = string.Empty; });
            InfoMessage("Pad is initializing, please wait ...");
            _codeRunner.Build();
            Application.Invoke((s, e) => {
                EnableCommands(true);
                if (!CommonHelper.IsNullOrEmptyOrBlank(_currentConfigPad.Name))
                    lblConnInfo.Text = "(" + _currentConfigPad.Name + ")";
                else
                    lblConnInfo.Text = string.Empty;
            });
            ResetMessage();
        } catch (Exception ex) {
            ErrorMessage(ex);
        }
        SetMouseCursor(null);
        _isRunning = false;
        Application.Invoke((s, e) => {
            EnableCommands(true); });
    }

    void SetMouseCursor( Cursor cursor ) {
        Gtk.Application.Invoke(delegate {
            GdkWindow.Cursor = cursor;
            _codeView.GetWindow(TextWindowType.Text).Cursor = cursor;
        });
    }

    void InternalSave( string file ) {
        if (file != null) {
            _currentConfigPad.Code = _codeView.Buffer.Text;
            _currentConfigPad.Save(file);
            SetupTitle();
        }
    }

    void InternalLoad( string file ) {
        if (file != null) {
            _currentConfigPad.Reset();
            Reset();
            if (_currentConfigPad.Load(file)) {
                SetupCode(_currentConfigPad.Code);
                cboCodeType.Active = (int)_currentConfigPad.CodeType;
                SetupTitle();
                Thread thSetupDC = new Thread(new ThreadStart(OnDCInit));
                thSetupDC.Start();
            }
        }
    }

    void SetupCode( string code ) {
        SourceLanguage language = null;
        SourceStyleScheme scheme = null;
        
        switch (_currentConfigPad.GetCurrentLang()) {
        case PadConfig.Languages.CSHARP:
            {
                language = SourceLanguageManager.Default.GetLanguage("c-sharp");
                scheme = csharpsourceSchema;
                break;
            }
        case PadConfig.Languages.VBASIC:
            {
                language = SourceLanguageManager.Default.GetLanguage("vbnet");
                scheme = vbsourceSchema;
                break;
            }
        default:
            {
                language = SourceLanguageManager.Default.GetLanguage("html");
                break;
            }
        }
        
        SourceBuffer buffer = new SourceBuffer(language);
        buffer.StyleScheme = scheme;
        buffer.MaxUndoLevels = 20;
        buffer.HighlightSyntax = true;
        buffer.HighlightMatchingBrackets = true;
        buffer.Text = code;
        
        _codeView.Buffer = buffer;
    }

    void Quit() {
        _codeRunner.Release();
        Application.Quit();
    }
    #region Menu Actions Handlers

    protected virtual void OnCboCodeTypeChanged( object sender, System.EventArgs e ) {
        _currentConfigPad.CodeType = (CodeType)cboCodeType.Active;
        SetupCode(_codeView.Buffer.Text);
    }

    protected virtual void OnQuitActionActivated( object sender, System.EventArgs e ) {
        Quit();
    }

    protected virtual void OnRunCodeActionActivated( object sender, System.EventArgs e ) {
        if (_isRunning) {
            return;
        }
        
        if (CommonHelper.IsNullOrEmptyOrBlank(_codeView.Buffer.Text)) {
            return;
        }
        
        _isRunning = true;
        
        String temp = null;
        
        if (_codeView.Buffer.HasSelection) {
            Gtk.TextIter cm;
            Gtk.TextIter cme;
            if (_codeView.Buffer.GetSelectionBounds(out cm, out cme)) {
                temp = _codeView.Buffer.GetText(cm, cme, false);
            }
        }
        
        if (temp == null) {
            temp = _codeView.Buffer.Text;
        }
        
        this.ClearSpool();
        _currentConfigPad.Code = temp.ToString();
        
        _thCodeRunner = new Thread(new ThreadStart(OnCodeExec));
        _thCodeRunner.Start();
    }

    protected virtual void OnStopActionActivated( object sender, System.EventArgs e ) {
        if (_thCodeRunner == null || !_isRunning)
            return;
        _codeRunner.Stop();
    }

    protected virtual void OnCopyActionActivated( object sender, System.EventArgs e ) {
        Clipboard clipboad = _codeView.GetClipboard(Gdk.Selection.Clipboard);
        _codeView.Buffer.CopyClipboard(clipboad);
    }

    protected virtual void OnCutActionActivated( object sender, System.EventArgs e ) {
        Clipboard clipboad = _codeView.GetClipboard(Gdk.Selection.Clipboard);
        _codeView.Buffer.CutClipboard(clipboad, true);
    }

    protected virtual void OnPasteActionActivated( object sender, System.EventArgs e ) {
        Clipboard clipboad = _codeView.GetClipboard(Gdk.Selection.Clipboard);
        _codeView.Buffer.PasteClipboard(clipboad);
    }

    protected virtual void OnNewActionActivated( object sender, System.EventArgs e ) {
        if (!_currentConfigPad.Exist && _codeView.Buffer.Text.Trim().Length > 0) {
            
            if (!MessageHelper.ShowYesNo("Current code is not empty. Clear all?")) {
                return;
            }
        }
        _currentConfigPad.Code = string.Empty;
        Reset();
        SetupTitle();
    }

    protected virtual void OnSaveActionActivated( object sender, System.EventArgs e ) {
        if (_codeView.Buffer.Text.Trim().Length > 0) {
            string file = _currentConfigPad.FileName;
            if (file == null) {
                file = FileDialogHelper.SaveFile("SpoolPad - Save", "noname.spad", "SpoolPad File|spad");
            }
            InternalSave(file);
        }
    }

    protected virtual void OnSaveAsActionActivated( object sender, System.EventArgs e ) {
        if (_codeView.Buffer.Text.Trim().Length > 0) {
            string file = "noname.spad";
            if (_currentConfigPad.Exist)
                file = System.IO.Path.GetFileName(_currentConfigPad.FileName);
            file = FileDialogHelper.SaveFile("SpoolPad - Save As", file, "SpoolPad File|spad");
            InternalSave(file);
        }
    }

    protected virtual void OnOpenActionActivated( object sender, System.EventArgs e ) {
        if (_codeView.Buffer.Text.Trim().Length > 0 && !_currentConfigPad.Exist) {
            if (!MessageHelper.ShowYesNo("Current code is not empty. Sure to open a new file?")) {
                return;
            }
        }
        
        string file = FileDialogHelper.OpenFile("SpoolPad - Open", "SpoolPad File|spad");
        InternalLoad(file);
    }

    protected virtual void OnResultLabelEventBoxButtonPressEvent( object o, Gtk.ButtonPressEventArgs args ) {
        if (ExistResultView && args.Event.Button == 1 && args.Event.Type == EventType.TwoButtonPress) {
            vboxCode.Visible = !vboxCode.Visible;
        }
    }

    protected virtual void OnCodeLabelEventBoxButtonPressEvent( object o, Gtk.ButtonPressEventArgs args ) {
        if (ExistResultView && args.Event.Button == 1 && args.Event.Type == EventType.TwoButtonPress) {
            vboxResult.Visible = !vboxResult.Visible;
        }
    }
    #endregion
}
