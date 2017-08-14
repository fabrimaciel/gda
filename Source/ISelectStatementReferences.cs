using System;
using System.Collections.Generic;
using System.Text;
namespace GDA.Sql
{
	public interface ISelectStatementReferences
	{
		string GetPropertyMapping (Mapping.TypeInfo a, string b);
		string GetFirstKeyPropertyMapping (Mapping.TypeInfo a);
		IEnumerable<Mapping.IPropertyMappingInfo> GetPropertiesMapping (Mapping.TypeInfo a);
		TableName GetTableName (Mapping.TypeInfo a);
		Mapping.TypeInfo GetTypeInfo (TableInfo a);
	}
}
