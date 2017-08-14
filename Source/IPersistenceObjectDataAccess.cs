using System;
namespace GDA.Interfaces
{
	public interface IPersistenceObjectDataAccess<T>
	{
		int Delete (T a);
		uint Insert (T a);
		uint InsertForced (T a);
		uint InsertOrUpdate (T a);
		System.Collections.Generic.List<Mapper> Keys {
			get;
		}
		string TableName {
			get;
		}
		GDA.Sql.TableName TableNameInfo {
			get;
		}
		string SystemTableName {
			get;
		}
		int Update (T a);
	}
}
