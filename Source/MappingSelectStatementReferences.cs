using System;
using System.Collections.Generic;
using System.Text;
using GDA.Mapping;
namespace GDA.Sql
{
	public class MappingSelectStatementReferences : ISelectStatementReferences
	{
		private Dictionary<string, ClassMapping> _mappings = new Dictionary<string, ClassMapping> ();
		public ClassMapping GetMapping (string a)
		{
			ClassMapping b = null;
			if (_mappings.TryGetValue (a, out b))
				return b;
			return null;
		}
		public IEnumerable<ClassMapping> GetMappings ()
		{
			return _mappings.Values;
		}
		public void AddMapping (ClassMapping a)
		{
			if (a == null)
				throw new ArgumentNullException ("mapping");
			if (_mappings.ContainsKey (a.TypeInfo.Fullname))
				_mappings.Remove (a.TypeInfo.Name);
			_mappings.Add (a.TypeInfo.Fullname, a);
		}
		public string GetPropertyMapping (GDA.Mapping.TypeInfo a, string b)
		{
			ClassMapping c = null;
			if (_mappings.TryGetValue (a.Fullname, out c)) {
				var d = c.Properties.Find (e => e.Name == b);
				return d != null ? d.Column : null;
			}
			return null;
		}
		public string GetFirstKeyPropertyMapping (Mapping.TypeInfo a)
		{
			ClassMapping b = null;
			if (_mappings.TryGetValue (a.Fullname, out b)) {
				var c = b.Properties.Find (d => d.ParameterType == PersistenceParameterType.Key || d.ParameterType == PersistenceParameterType.IdentityKey);
				return c != null ? c.Column : null;
			}
			return null;
		}
		public IEnumerable<IPropertyMappingInfo> GetPropertiesMapping (Mapping.TypeInfo a)
		{
			ClassMapping b = null;
			if (_mappings.TryGetValue (a.Fullname, out b)) {
				foreach (var i in b.Properties)
					yield return (IPropertyMappingInfo)i;
			}
		}
		public TableName GetTableName (GDA.Mapping.TypeInfo a)
		{
			ClassMapping b = null;
			if (_mappings.TryGetValue (a.Fullname, out b))
				return new TableName {
					Name = b.Table,
					Schema = b.Schema
				};
			return null;
		}
		public GDA.Mapping.TypeInfo GetTypeInfo (TableInfo a)
		{
			return new TypeInfo (a.TableName.Name);
		}
	}
}
