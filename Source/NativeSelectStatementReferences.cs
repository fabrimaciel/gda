using System;
using System.Collections.Generic;
using System.Text;
namespace GDA.Sql
{
	class NativeSelectStatementReferences : ISelectStatementReferences
	{
		private static NativeSelectStatementReferences _instance;
		public static NativeSelectStatementReferences Instance {
			get {
				if (_instance == null)
					_instance = new NativeSelectStatementReferences ();
				return _instance;
			}
		}
		private NativeSelectStatementReferences ()
		{
		}
		public string GetPropertyMapping (GDA.Mapping.TypeInfo a, string b)
		{
			var c = Type.GetType (a.FullnameWithAssembly);
			if (c == null)
				throw new QueryException (string.Format ("Type {0} not found.", a.FullnameWithAssembly));
			var d = Caching.MappingManager.GetMappers (c).Find (e => e.PropertyMapperName == b);
			return d != null ? d.Name : null;
		}
		public string GetFirstKeyPropertyMapping (Mapping.TypeInfo a)
		{
			var b = Caching.MappingManager.GetMappers (Type.GetType (a.FullnameWithAssembly)).Find (c => c.ParameterType == PersistenceParameterType.Key || c.ParameterType == PersistenceParameterType.IdentityKey);
			return b != null ? b.Name : null;
		}
		public IEnumerable<Mapping.IPropertyMappingInfo> GetPropertiesMapping (Mapping.TypeInfo a)
		{
			foreach (var f in Caching.MappingManager.GetMappers (Type.GetType (a.FullnameWithAssembly)))
				yield return (Mapping.IPropertyMappingInfo)new Mapping.PropertyMappingInfo {
					Name = f.PropertyMapperName,
					Column = f.Name,
					Direction = f.Direction,
					ParameterType = f.ParameterType
				};
		}
		public TableName GetTableName (GDA.Mapping.TypeInfo a)
		{
			var b = Caching.MappingManager.GetPersistenceClassAttribute (Type.GetType (a.FullnameWithAssembly));
			return b != null ? new TableName {
				Name = b.Name,
				Schema = b.Schema
			} : null;
		}
		public Mapping.TypeInfo GetTypeInfo (TableInfo a)
		{
			var b = Caching.MappingManager.LoadModel (a.TableName.Name);
			if (b == null) {
				if (string.IsNullOrEmpty (a.TableAlias))
					throw new GDAException ("Not found type info for table {0}.", a.TableName.Name);
				return new Mapping.TypeInfo (a.TableAlias, null, null);
			}
			else {
				Caching.MappingManager.LoadClassMapper (b);
				return new Mapping.TypeInfo (b);
			}
		}
	}
}
