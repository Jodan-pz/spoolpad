using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;

using System.Threading;
using Equisetum2.Common.BaseClasses;
using Equisetum2.Common.Helpers;
using Equisetum2.NHibernate.Extensions;
using it.jodan.SpoolPad.BaseClasses;
using it.jodan.SpoolPad.Extensions;

namespace it.jodan.SpoolPad.DataContext {

	public class BaseDataContext : AbstractContext {
		IUnitOfWork uow = null;

		public BaseDataContext() {
			// get Unit Of Work with updated data context info
			uow = ServiceHelper.GetService<IUnitOfWork>();
		}

		#region implemented abstract members of it.jodan.SpoolPad.AbstractContext
		
        protected override void OnRun() {
			try {
				uow.BeginTransaction();
				InternalRun();
				uow.Commit();
			} finally {
				uow.Rollback();
			}
		}

		#endregion

		[DataContextCommand("List of all the available elements.")]
		protected void DBDomain() {
			"".Spool("Domain elements");
			foreach (string entity in uow.GetDomainEntities())
				entity.Spool();
		}
		
		[DataContextCommand("Describe element. Example: DBDesc<Product>().")]
		protected void DBDesc<T>() {
			uow.GetEntityAttributes(typeof(T)).Spool( typeof(T).Name );
		}

		[DataContextCommand("Create a database.")]
		protected void DBCreate() {
			uow.ExportSchema(false, true, false);
		}

		[DataContextCommand("Query data of generic T element type. Example: A call to DB<Product>() will return all of the Product elements.")]
		protected override IQueryable<T> DB<T>() {
			return uow.Query<T>();
		}

		[DataContextCommand("Reset by clearing the data context session.")]
		protected void DBReset() {
			uow.Clear();
		}

		[DataContextCommand("Save a new element.")]
		protected void DBStore( object item ) {
			uow.Save(item);
		}

		[DataContextCommand("Save an iterable set of new elements.")]
		protected void DBStore<T>( IEnumerable<T> items ) {
			uow.Save(items);
		}

		[DataContextCommand("Save new elements.")]
		protected void DBStore<T>( params T[] items ) {
			if (items != null)
				DBStore((IEnumerable<T>)items.ToList());
		}

		[DataContextCommand("Delete an element.")]
		protected void DBDelete( object item ) {
			uow.Delete(item);
		}

		[DataContextCommand("Delete an iterable set of elements.")]
		protected void DBDelete<T>( IEnumerable<T> items ) {
			uow.Delete(items);
		}

		[DataContextCommand("Delete elements.")]
		protected void DBDelete<T>( params T[] items ) {
			if (items != null)
				DBDelete((IEnumerable<T>)items.ToList());
		}

		[DataContextCommand("Flush current pending operations to the underlying database.")]
		protected void DBSubmitChanges() {
			uow.Flush();
		}
	}
}