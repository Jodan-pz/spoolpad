using System;
using System.Data;
using System.Data.SqlClient;
using Common.Logging;
using Equisetum2.Common.Helpers;
using Equisetum2.NHibernate;
using Gtk;

using it.jodan.SpoolPad.BaseClasses;
using it.jodan.SpoolPad.BaseClasses.Configuration;
using it.jodan.SpoolPad.Helpers;


namespace it.jodan.SpoolPad {

	public class SpoolPadProgram {
		public const string APP_NAME = "SpoolPad 1.0";
		//static readonly ILog _log = LogManager.GetLogger(typeof(SpoolPadProgram));
		static ILog _log;

		public static void Main( string[] args ) {
			try{
				_log = LogManager.GetCurrentClassLogger();
			}catch(Exception ex ){
				Console.WriteLine(ex.Message);
			}

			_log.InfoFormat("{0} ready to serve.", APP_NAME);
			// BATCH
			if (args.Length > 0) {
				CodeRunner codeRunner = new CodeRunner();
				try {
					string padFile = args[0];
					_log.InfoFormat("File argument: {0}", padFile);
					
					// load configuration
					PadConfig padConfig = new PadConfig();					
					if (!padConfig.Load(padFile)) {
						_log.ErrorFormat("Error loading pad file!");
						return;
					}
					
					// setup single factory (fast calls optimization)
					if (padConfig.DataContext.Enabled) {
						CustomConfiguration cfg = ServiceHelper.GetService<CustomConfiguration>();
						cfg.FactoryRebuildOnTheFly = bool.FalseString;
						cfg.IsConfigured = true;
					}
					
					// run
					codeRunner.Build(padConfig);
					codeRunner.Run();
					
				} catch (Exception ex) {
					
					_log.ErrorFormat(ex.Message);
					
				} finally {
					
					codeRunner.Release();
				}
				
			} else {
				
				// GUI
				try {
					Application.Init();
					SpoolPadWindow win = new SpoolPadWindow();
					win.Show();
					Application.Run();
				} catch (Exception ex) {
					MessageHelper.ShowError(ex);
				}
				
			}
			
			_log.Info("SpoolPad goes to sleep.");
			
		}
	}
}