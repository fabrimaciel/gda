using System;
using System.Collections.Generic;
using System.Text;
using GDA.Provider;
using GDA.Helper;
using GDA.Common.Configuration;
using GDA.Collections;
using GDA.Interfaces;
using GDA.Sql;
using GDA.Caching;
namespace GDA
{
	public class BaseDAO<Model> : GDA.Interfaces.IBaseDAO<Model> where Model : new()
	{
		private PersistenceObject<Model> currentPersistenceObject;
		protected PersistenceObject<Model> CurrentPersistenceObject {
			get {
				return currentPersistenceObject;
			}
		}
		public IProviderConfiguration Configuration {
			get {
				return currentPersistenceObject.Configuration;
			}
		}
		public BaseDAO (IProviderConfiguration a)
		{
			currentPersistenceObject = new PersistenceObject<Model> (a);
			RegisterCurrentDAOInModel ();
		}
		public BaseDAO ()
		{
			GDASettings.LoadConfiguration ();
			PersistenceProviderAttribute a = MappingManager.GetPersistenceProviderAttribute (typeof(Model));
			IProviderConfiguration b = null;
			if (a != null) {
				if (!string.IsNullOrEmpty (a.ProviderConfigurationName))
					b = GDASettings.GetProviderConfiguration (a.ProviderConfigurationName);
				else
					b = GDASettings.CreateProviderConfiguration (a.ProviderName, a.ConnectionString);
			}
			else
				b = GDASettings.DefaultProviderConfiguration;
			currentPersistenceObject = new PersistenceObject<Model> (b);
			RegisterCurrentDAOInModel ();
		}
		private void RegisterCurrentDAOInModel ()
		{
			GDAOperations.AddMemberDAO (typeof(Model), this);
		}
		internal GDAList<Model> GetSqlData (string a, List<GDAParameter> b, InfoSortExpression c, InfoPaging d)
		{
			return CurrentPersistenceObject.LoadDataWithSortExpression (a, c, d, b.ToArray ());
		}
		public virtual GDACursor<Model> Select ()
		{
			return CurrentPersistenceObject.Select ();
		}
		public virtual GDACursor<Model> Select (GDASession a)
		{
			return CurrentPersistenceObject.Select (a);
		}
		public virtual GDACursor<Model> Select (IQuery a)
		{
			a.ReturnTypeQuery = typeof(Model);
			return CurrentPersistenceObject.Select (a);
		}
		public virtual GDACursor<Model> Select (GDASession a, IQuery b)
		{
			b.ReturnTypeQuery = typeof(Model);
			return CurrentPersistenceObject.Select (a, b);
		}
		public GDADataRecordCursor<Model> SelectToDataRecord (GDASession a, IQuery b)
		{
			b.ReturnTypeQuery = typeof(Model);
			return CurrentPersistenceObject.SelectToDataRecord (a, b);
		}
		public virtual long Count (GDASession a, Query b)
		{
			b.ReturnTypeQuery = typeof(Model);
			return CurrentPersistenceObject.Count (a, b);
		}
		public virtual long Count ()
		{
			return CurrentPersistenceObject.Count ();
		}
		public virtual long Count (GDASession a)
		{
			return CurrentPersistenceObject.Count (a, null);
		}
		public virtual long Count (Query a)
		{
			a.ReturnTypeQuery = typeof(Model);
			return CurrentPersistenceObject.Count (a);
		}
		public double Sum (GDASession a, Query b)
		{
			b.ReturnTypeQuery = typeof(Model);
			return CurrentPersistenceObject.Sum (a, b);
		}
		public double Max (GDASession a, Query b)
		{
			b.ReturnTypeQuery = typeof(Model);
			return CurrentPersistenceObject.Max (a, b);
		}
		public double Min (GDASession a, Query b)
		{
			b.ReturnTypeQuery = typeof(Model);
			return CurrentPersistenceObject.Min (a, b);
		}
		public double Avg (GDASession a, Query b)
		{
			b.ReturnTypeQuery = typeof(Model);
			return CurrentPersistenceObject.Avg (a, b);
		}
		public bool CheckExist (GDASession a, ValidationMode b, string c, object d, Model e)
		{
			return CurrentPersistenceObject.CheckExist (a, b, c, d, e);
		}
		public virtual Model RecoverData (Model a)
		{
			return RecoverData (null, a);
		}
		public virtual Model RecoverData (GDASession a, Model b)
		{
			return CurrentPersistenceObject.RecoverData (a, b);
		}
		public virtual uint Insert (GDASession a, Model b, string c, DirectionPropertiesName d)
		{
			return CurrentPersistenceObject.Insert (a, b, c, d);
		}
		public virtual uint Insert (GDASession a, Model b, string c)
		{
			return CurrentPersistenceObject.Insert (a, b, c, DirectionPropertiesName.Inclusion);
		}
		public virtual uint Insert (GDASession a, Model b)
		{
			return CurrentPersistenceObject.Insert (a, b, null, DirectionPropertiesName.Inclusion);
		}
		public virtual uint Insert (Model a, string b, DirectionPropertiesName c)
		{
			return CurrentPersistenceObject.Insert (a, b, c);
		}
		public uint Insert (Model a, string b)
		{
			return CurrentPersistenceObject.Insert (a, b);
		}
		public virtual uint Insert (Model a)
		{
			return CurrentPersistenceObject.Insert (a);
		}
		public virtual int Update (GDASession a, Model b, string c, DirectionPropertiesName d)
		{
			return CurrentPersistenceObject.Update (a, b, c, d);
		}
		public virtual int Update (GDASession a, Model b, string c)
		{
			return CurrentPersistenceObject.Update (a, b, c, DirectionPropertiesName.Inclusion);
		}
		public virtual int Update (GDASession a, Model b)
		{
			return CurrentPersistenceObject.Update (a, b, null, DirectionPropertiesName.Inclusion);
		}
		public virtual int Update (Model a, string b, DirectionPropertiesName c)
		{
			return CurrentPersistenceObject.Update (a, b, c);
		}
		public virtual int Update (Model a, string b)
		{
			return CurrentPersistenceObject.Update (a, b);
		}
		public virtual int Update (Model a)
		{
			return CurrentPersistenceObject.Update (a);
		}
		public virtual int Delete (GDASession a, Model b)
		{
			return CurrentPersistenceObject.Delete (a, b);
		}
		public virtual int Delete (Model a)
		{
			return CurrentPersistenceObject.Delete (a);
		}
		public virtual int Delete (GDASession a, Query b)
		{
			return CurrentPersistenceObject.Delete (a, b);
		}
		public virtual int Delete (Query a)
		{
			return CurrentPersistenceObject.Delete (a);
		}
		public virtual uint InsertOrUpdate (GDASession a, Model b)
		{
			return CurrentPersistenceObject.InsertOrUpdate (a, b);
		}
		public virtual uint InsertOrUpdate (Model a)
		{
			return CurrentPersistenceObject.InsertOrUpdate (a);
		}
		public virtual uint Insert (GDASession a, object b, string c, DirectionPropertiesName d)
		{
			return Insert (a, (Model)b, c, d);
		}
		public virtual uint Insert (object a, string b, DirectionPropertiesName c)
		{
			return Insert ((Model)a, b, c);
		}
		public virtual uint Insert (GDASession a, object b, string c)
		{
			return Insert (a, (Model)b, c);
		}
		public virtual uint Insert (object a, string b)
		{
			return Insert ((Model)a, b);
		}
		public virtual uint Insert (GDASession a, object b)
		{
			return Insert (a, (Model)b);
		}
		public virtual uint Insert (object a)
		{
			return Insert ((Model)a);
		}
		public virtual int Update (GDASession a, object b, string c, DirectionPropertiesName d)
		{
			return Update (a, (Model)b, c, d);
		}
		public virtual int Update (object a, string b, DirectionPropertiesName c)
		{
			return Update ((Model)a, b, c);
		}
		public virtual int Update (GDASession a, object b, string c)
		{
			return Update (a, (Model)b, c);
		}
		public virtual int Update (object a, string b)
		{
			return Update ((Model)a, b);
		}
		public virtual int Update (GDASession a, object b)
		{
			return Update (a, (Model)b);
		}
		public virtual int Update (object a)
		{
			return Update ((Model)a);
		}
		public virtual int Delete (GDASession a, object b)
		{
			return CurrentPersistenceObject.Delete (a, (Model)b);
		}
		public virtual int Delete (object a)
		{
			return Delete ((Model)a);
		}
		public virtual uint InsertOrUpdate (GDASession a, object b)
		{
			return InsertOrUpdate (a, (Model)b);
		}
		public virtual uint InsertOrUpdate (object a)
		{
			return InsertOrUpdate ((Model)a);
		}
		public bool CheckExist (GDASession a, ValidationMode b, string c, object d, object e)
		{
			return CurrentPersistenceObject.CheckExist (a, b, c, d, (Model)e);
		}
		public GDAList<ClassChild> LoadDataForeignKeyParentToChild<ClassChild> (Model a, string b, InfoSortExpression c, InfoPaging d) where ClassChild : new()
		{
			return CurrentPersistenceObject.LoadDataForeignKeyParentToChild<ClassChild> (a, b, c, d);
		}
		public GDAList<ClassChild> LoadDataForeignKeyParentToChild<ClassChild> (Model a, string b, InfoSortExpression c) where ClassChild : new()
		{
			return LoadDataForeignKeyParentToChild<ClassChild> (a, b, c, null);
		}
		public GDAList<ClassChild> LoadDataForeignKeyParentToChild<ClassChild> (Model a, string b) where ClassChild : new()
		{
			return LoadDataForeignKeyParentToChild<ClassChild> (a, b, null, null);
		}
		public GDAList<ClassChild> LoadDataForeignKeyParentToChild<ClassChild> (Model a) where ClassChild : new()
		{
			return LoadDataForeignKeyParentToChild<ClassChild> (a, null);
		}
		public int CountRowForeignKeyParentToChild<ClassChild> (Model a, string b) where ClassChild : new()
		{
			return CurrentPersistenceObject.CountRowForeignKeyParentToChild<ClassChild> (a, b);
		}
		public int CountRowForeignKeyParentToChild<ClassChild> (Model a) where ClassChild : new()
		{
			return CountRowForeignKeyParentToChild<ClassChild> (a, null);
		}
	}
}
