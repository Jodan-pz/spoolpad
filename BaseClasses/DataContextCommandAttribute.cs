using System;
namespace it.jodan.SpoolPad {
	public class DataContextCommandAttribute : Attribute {
		public string Description { get; set; }

		public DataContextCommandAttribute( string description ) {
			Description = description;
		}
	}
}

