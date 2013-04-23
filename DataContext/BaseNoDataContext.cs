using it.jodan.SpoolPad.BaseClasses;

namespace it.jodan.SpoolPad.DataContext {

    public class BaseNoDataContext : AbstractContext {
	
        public BaseNoDataContext() {}

		#region implemented abstract members of it.jodan.SpoolPad.BaseContext
		
        protected override void OnRun() {
			InternalRun();
		}
		
		#endregion
	}
}
