using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using GDA.Sql;
namespace GDA.Interfaces
{
	public interface ISimpleBaseDAO
	{
		IProviderConfiguration Configuration {
			get;
		}
		int Delete (GDASession a, object b);
		int Delete (object a);
		uint Insert (object a, string b, DirectionPropertiesName c);
		uint Insert (GDASession a, object b, string c, DirectionPropertiesName d);
		uint Insert (GDASession a, object b, string c);
		uint Insert (object a, string b);
		uint Insert (GDASession a, object b);
		uint Insert (object a);
		uint InsertOrUpdate (GDASession a, object b);
		uint InsertOrUpdate (object a);
		int Update (GDASession a, object b, string c, DirectionPropertiesName d);
		int Update (object a, string b, DirectionPropertiesName c);
		int Update (GDASession a, object b, string c);
		int Update (object a, string b);
		int Update (GDASession a, object b);
		int Update (object a);
		bool CheckExist (GDASession a, ValidationMode b, string c, object d, object e);
	}
	public interface ISimpleBaseDAO<Model> : ISimpleBaseDAO
	{
		Collections.GDADataRecordCursor<Model> SelectToDataRecord (GDASession a, IQuery b);
	}
}
