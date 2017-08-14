using System;
using GDA.Sql;
using GDA.Provider;
namespace GDA.Interfaces
{
	public interface IBaseDAO<Model> : ISimpleBaseDAO<Model> where Model : new()
	{
		int Delete (Model a);
		int Delete (GDASession a, Model b);
		uint Insert (Model a, string b, DirectionPropertiesName c);
		uint Insert (GDASession a, Model b, string c, DirectionPropertiesName d);
		uint Insert (Model a, string b);
		uint Insert (GDASession a, Model b, string c);
		uint Insert (Model a);
		uint Insert (GDASession a, Model b);
		uint InsertOrUpdate (Model a);
		uint InsertOrUpdate (GDASession a, Model b);
		int Update (Model a, string b, DirectionPropertiesName c);
		int Update (GDASession a, Model b, string c, DirectionPropertiesName d);
		int Update (Model a, string b);
		int Update (GDASession a, Model b, string c);
		int Update (Model a);
		int Update (GDASession a, Model b);
		int CountRowForeignKeyParentToChild<ClassChild> (Model a, string b) where ClassChild : new();
		int CountRowForeignKeyParentToChild<ClassChild> (Model a) where ClassChild : new();
		GDA.Collections.GDAList<ClassChild> LoadDataForeignKeyParentToChild<ClassChild> (Model a, string b, InfoSortExpression c) where ClassChild : new();
		GDA.Collections.GDAList<ClassChild> LoadDataForeignKeyParentToChild<ClassChild> (Model a, string b) where ClassChild : new();
		GDA.Collections.GDAList<ClassChild> LoadDataForeignKeyParentToChild<ClassChild> (Model a, string b, InfoSortExpression c, InfoPaging d) where ClassChild : new();
		GDA.Collections.GDAList<ClassChild> LoadDataForeignKeyParentToChild<ClassChild> (Model a) where ClassChild : new();
		Model RecoverData (Model a);
		Model RecoverData (GDASession a, Model b);
		GDA.Collections.GDACursor<Model> Select ();
		GDA.Collections.GDACursor<Model> Select (GDASession a);
		GDA.Collections.GDACursor<Model> Select (IQuery a);
		GDA.Collections.GDACursor<Model> Select (GDASession a, IQuery b);
		long Count (Query a);
		long Count (GDASession a, Query b);
		long Count ();
		long Count (GDASession a);
		double Sum (GDASession a, Query b);
		double Max (GDASession a, Query b);
		double Min (GDASession a, Query b);
		double Avg (GDASession a, Query b);
		bool CheckExist (GDASession a, ValidationMode b, string c, object d, Model e);
	}
}
