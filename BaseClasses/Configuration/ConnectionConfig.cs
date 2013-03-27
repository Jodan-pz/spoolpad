using System;
namespace it.jodan.SpoolPad.BaseClasses.Configuration {
	public class ConnectionConfig {
		public string ConnectionString { get; set; }
		public string User { get; set; }
		public string Password { get; set; }
		public string Driver { get; set; }
		public string Dialect { get; set; }
		public string ShowSql { get; set; }

		public override string ToString() {
			return string.Format("[ConnectionConfig: ConnectionString={0}, User={1}, Password={2}, Driver={3}, Dialect={4}, ShowSql={5}]", ConnectionString, User, Password, Driver, Dialect, ShowSql);
		}
	}
	
}

