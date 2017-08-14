using System;
using System.Collections.Generic;
using System.Text;
using GDA.Interfaces;
using GDA.Collections;
using GDA.Sql;
namespace GDA
{
	public class SimpleBaseDAO<Model> : ISimpleBaseDAO<Model>
	{
		private PersistenceObjectBase<Model> currentPersistenceObject;
		protected PersistenceObjectBase<Model> CurrentPersistenceObject {
			get {
				return currentPersistenceObject;
			}
		}
		[Obsolete]
		protected PersistenceObjectBase<Model> ObjPersistence {
			get {
				return currentPersistenceObject;
			}
		}
		public IProviderConfiguration Configuration {
			get {
				return currentPersistenceObject.Configuration;
			}
		}
		public SimpleBaseDAO (IProviderConfiguration a)
		{
			currentPersistenceObject = new PersistenceObjectBase<Model> (a);
		}
		public SimpleBaseDAO ()
		{
			GDASettings.LoadConfiguration ();
			PersistenceProviderAttribute a = GDA.Caching.MappingManager.GetPersistenceProviderAttribute (typeof(Model));
			IProviderConfiguration b = null;
			if (a != null) {
				if (!string.IsNullOrEmpty (a.ProviderConfigurationName))
					b = GDASettings.GetProviderConfiguration (a.ProviderConfigurationName);
				else
					b = GDASettings.CreateProviderConfiguration (a.ProviderName, a.ConnectionString);
			}
			else
				b = GDASettings.DefaultProviderConfiguration;
			currentPersistenceObject = new PersistenceObjectBase<Model> (b);
		}
		public virtual uint Insert (GDASession a, object b, string c, DirectionPropertiesName d)
		{
			return CurrentPersistenceObject.Insert (a, (Model)b, c, d);
		}
		public virtual uint Insert (object a, string b, DirectionPropertiesName c)
		{
			return CurrentPersistenceObject.Insert ((Model)a, b, c);
		}
		public virtual uint Insert (GDASession a, object b, string c)
		{
			return CurrentPersistenceObject.Insert (a, (Model)b, c, DirectionPropertiesName.Inclusion);
		}
		public virtual uint Insert (object a, string b)
		{
			return CurrentPersistenceObject.Insert ((Model)a, b);
		}
		public virtual uint Insert (GDASession a, object b)
		{
			return CurrentPersistenceObject.Insert (a, (Model)b, null, DirectionPropertiesName.Inclusion);
		}
		public virtual uint Insert (object a)
		{
			return CurrentPersistenceObject.Insert ((Model)a);
		}
		public virtual int Update (GDASession a, object b, string c, DirectionPropertiesName d)
		{
			return CurrentPersistenceObject.Update (a, (Model)b, c, d);
		}
		public virtual int Update (object a, string b, DirectionPropertiesName c)
		{
			return CurrentPersistenceObject.Update ((Model)a, b, c);
		}
		public virtual int Update (GDASession a, object b, string c)
		{
			return CurrentPersistenceObject.Update (a, (Model)b, c, DirectionPropertiesName.Inclusion);
		}
		public virtual int Update (object a, string b)
		{
			return CurrentPersistenceObject.Update ((Model)a, b);
		}
		public virtual int Update (GDASession a, object b)
		{
			return CurrentPersistenceObject.Update (a, (Model)b);
		}
		public virtual int Update (object a)
		{
			return CurrentPersistenceObject.Update ((Model)a);
		}
		public virtual int Delete (GDASession a, object b)
		{
			return CurrentPersistenceObject.Delete (a, (Model)b);
		}
		public virtual int Delete (object a)
		{
			return CurrentPersistenceObject.Delete ((Model)a);
		}
		public virtual uint InsertOrUpdate (GDASession a, object b)
		{
			return CurrentPersistenceObject.InsertOrUpdate (a, (Model)b);
		}
		public virtual uint InsertOrUpdate (object a)
		{
			return CurrentPersistenceObject.InsertOrUpdate ((Model)a);
		}
		public bool CheckExist (GDASession a, ValidationMode b, string c, object d, object e)
		{
			return CurrentPersistenceObject.CheckExist (a, b, c, d, (Model)e);
		}
		public GDADataRecordCursor<Model> SelectToDataRecord (GDASession a, IQuery b)
		{
			b.ReturnTypeQuery = typeof(Model);
			return CurrentPersistenceObject.SelectToDataRecord (a, b);
		}
	}
}
