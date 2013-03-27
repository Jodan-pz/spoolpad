using System;
using System.Collections.Generic;
using System.IO;
namespace it.jodan.SpoolPad.BaseClasses.Configuration {
	public class MapConfig : IEqualityComparer<MapConfig> {
		const string ASM_SUFFIX = "[asm:]";
		string _map = null;
		bool _isAssembly = false;
		string _assembly = null;
		string _resourceName = null;

		public override string ToString() {
			return string.Format("[MapConfig: IsValid={0}, IsFile={1}, IsAssembly={2}, Map={3}, Assembly={4}, ResourceName={5}]", IsValid, IsFile, IsAssembly, Map, Assembly, ResourceName);
		}

		public static MapConfig FromFile( string mapFile ) {
			return new MapConfig(mapFile);
		}

		public static MapConfig FromAssemblyResource( string assembly, string resource ) {
			return new MapConfig(ASM_SUFFIX + assembly + "|" + resource);
		}

		MapConfig( string map ) {
			_map = map;
			_isAssembly = _map != null && _map.StartsWith(ASM_SUFFIX);
			if (_isAssembly) {
				string[] temp = _map.Substring(ASM_SUFFIX.Length).Split('|');
				_assembly = temp[0];
				_resourceName = temp[1];
			}
		}

		public bool IsValid {
			get {
				if (IsFile)
					return File.Exists(Map);
				if (IsAssembly)
					return File.Exists(Assembly);
				return false;
			}
		}

		public bool IsFile {
			get { return !IsAssembly; }
		}

		public bool IsAssembly {
			get { return _isAssembly; }
		}

		public string Map {
			get { return _map; }
		}

		public string Assembly {
			get { return _assembly; }
		}

		public string ResourceName {
			get { return _resourceName; }
		}

		public override bool Equals( object obj ) {
			if (obj == null)
				return false;
			MapConfig other = obj as MapConfig;
			if (other == null)
				return false;
			return ((IEqualityComparer<MapConfig>)this).Equals(this, other);
		}

		public override int GetHashCode() {
			return ((IEqualityComparer<MapConfig>)this).GetHashCode(this);
		}

		#region IEqualityComparer[MapConfig] implementation
		bool IEqualityComparer<MapConfig>.Equals( MapConfig x, MapConfig y ) {
			if (x.IsFile && y.IsFile) {
				return x.Map.Equals(y.Map);
			} else if (x.IsAssembly && y.IsAssembly) {
				return x.Assembly.Equals(y.Assembly);
			}
			return false;
			
		}

		int IEqualityComparer<MapConfig>.GetHashCode( MapConfig obj ) {
			if (obj.IsFile) {
				return obj.Map == null ? 0 : obj.Map.GetHashCode();
			} else if (obj.IsAssembly) {
				return obj.Assembly == null ? 0 : obj.Assembly.GetHashCode();
			}
			return 0;
		}
		#endregion
	}
	
}

