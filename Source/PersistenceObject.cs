using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Reflection;
using System.Data.Common;
using System.Diagnostics;
using GDA.Collections;
using GDA.Provider;
using GDA.Helper;
using GDA.Interfaces;
using GDA.Sql;
using GDA.Sql.InterpreterExpression;
using GDA.Sql.InterpreterExpression.Nodes;
using GDA.Caching;
namespace GDA
{
	public class PersistenceObject<Model> : PersistenceObjectBase<Model> where Model : new()
	{
		public PersistenceObject (IProviderConfiguration a) : base (a)
		{
		}
		public IEnumerable<GDAPropertyValue> GetValues (GDASession a, IQuery b, string c)
		{
			if (string.IsNullOrEmpty (c))
				throw new ArgumentNullException ("propertyName");
			if (b == null)
				b = new Query ();
			b.ReturnTypeQuery = typeof(Model);
			QueryReturnInfo d = b.BuildResultInfo<Model> (this.Configuration);
			List<Mapper> e = d.RecoverProperties;
			string f = d.CommandText;
			IDbConnection g = CreateConnection (a);
			IDbCommand h = CreateCommand (a, g);
			h.CommandType = CommandType.Text;
			bool j = false;
			if (b.SkipCount > 0 || b.TakeCount > 0)
				j = true;
			if (e.Count == 0)
				throw new GDAException ("Not found properties mappers to model {0}.", typeof(Model).FullName);
			if (d.Parameters != null)
				for (int k = 0; k < d.Parameters.Count; k++) {
					try {
						string l = (d.Parameters [k].ParameterName [0] != '?' ? d.Parameters [k].ParameterName : UserProvider.ParameterPrefix + d.Parameters [k].ParameterName.Substring (1) + UserProvider.ParameterSuffix);
						f = f.Replace (d.Parameters [k].ParameterName, l);
					}
					catch (Exception ex) {
						throw new GDAException ("Error on make parameter name '" + d.Parameters [k].ParameterName + "'.", ex);
					}
					h.Parameters.Add (GDA.Helper.GDAHelper.ConvertGDAParameter (h, d.Parameters [k], UserProvider));
				}
			if (j) {
				f = UserProvider.SQLCommandLimit (e, f, b.SkipCount, b.TakeCount);
			}
			h.CommandText = f;
			var m = e.Find (delegate (Mapper n) {
				return n.PropertyMapperName == c;
			});
			if (m == null)
				throw new GDAException ("Property {0} not found in {1}.", c, typeof(Model).FullName);
			IDataReader o = null;
			if (a == null) {
				try {
					g.Open ();
				}
				catch (Exception ex) {
					throw new GDAException (ex);
				}
				GDAConnectionManager.NotifyConnectionOpened (g);
			}
			try {
				SendMessageDebugTrace ("CommandText: " + h.CommandText);
				using (var p = Diagnostics.GDATrace.CreateExecutionHandler (h))
					try {
						o = h.ExecuteReader ();
					}
					catch (Exception ex) {
						ex = new GDAException (ex);
						p.Fail (ex);
						throw ex;
					}
				if (o.FieldCount == 0)
					throw new GDAException ("The query not return any field.");
				var q = false;
				while (o.Read ()) {
					var k = new GDADataRecord (o, null);
					if (!q && k.GetOrdinal (m.Name) < 0)
						yield break;
					q = true;
					yield return new GDAPropertyValue (k.GetValue (m.Name), true);
				}
			}
			finally {
				if (a == null)
					try {
						g.Close ();
					}
					catch {
						SendMessageDebugTrace ("Error close connection.");
					}
				if (o != null)
					o.Close ();
			}
		}
		public GDAPropertyValue GetValue (GDASession a, IQuery b, string c)
		{
			foreach (var i in GetValues (a, b, c))
				return i;
			return new GDAPropertyValue (null, false);
		}
		public GDACursor<Model> Select ()
		{
			return Select (null, null);
		}
		public GDACursor<Model> Select (GDASession a)
		{
			return Select (a, null);
		}
		public GDACursor<Model> Select (IQuery a)
		{
			return Select (null, a);
		}
		public GDACursor<Model> Select (GDASession a, IQuery b)
		{
			return new GDACursor<Model> (GetCursorParameters (a, b));
		}
		public override GDADataRecordCursor<Model> SelectToDataRecord (GDASession a, IQuery b)
		{
			return new GDADataRecordCursorEx<Model> (GetCursorParameters (a, b));
		}
		public int DeleteByKey (uint a)
		{
			List<Mapper> b = MappingManager.GetMappers<Model> (new PersistenceParameterType[] {
				PersistenceParameterType.Key,
				PersistenceParameterType.IdentityKey
			}, new DirectionParameter[] {
				DirectionParameter.Output,
				DirectionParameter.InputOptionalOutput,
				DirectionParameter.InputOutput
			});
			if (b.Count == 0)
				throw new GDAException ("There isn't more one primary key identify for object \"" + typeof(Model).FullName + "\"");
			else if (b.Count > 1)
				throw new GDAException ("There is more one primary key identify for object \"" + typeof(Model).FullName + "\"");
			Model c = new Model ();
			object d = null;
			try {
				d = typeof(Convert).GetMethod ("To" + b [0].PropertyMapper.PropertyType.Name, new Type[] {
					typeof(uint)
				}).Invoke (null, new object[] {
					a
				});
			}
			catch (Exception ex) {
				throw new GDAException ("Type key not compatible with uint.", ex);
			}
			b [0].PropertyMapper.SetValue (c, d, null);
			return Delete (c);
		}
		public GDACursor<Model> LoadDataAndPaging (string a, int b, int c)
		{
			return LoadDataWithSortExpression (null, a, CommandType.Text, null, new InfoPaging (b, c), null);
		}
		public GDACursor<Model> LoadDataAndPaging (string a, int b, int c, params GDAParameter[] d)
		{
			return LoadDataWithSortExpression (null, a, CommandType.Text, null, new InfoPaging (b, c), d);
		}
		public GDACursor<Model> LoadDataAndPaging (GDASession a, string b, int c, int d, params GDAParameter[] e)
		{
			return LoadDataWithSortExpression (a, b, CommandType.Text, null, new InfoPaging (c, d), e);
		}
		public GDACursor<Model> LoadDataWithSortExpression (string a, InfoSortExpression b, InfoPaging c, GDAParameter[] d)
		{
			return LoadDataWithSortExpression (a, CommandType.Text, b, c, d);
		}
		public GDACursor<Model> LoadDataWithSortExpression (string a, System.Data.CommandType b, InfoSortExpression c, InfoPaging d, GDAParameter[] e)
		{
			return LoadDataWithSortExpression (null, a, b, c, d, e);
		}
		public GDACursor<Model> LoadDataWithSortExpression (GDASession a, string b, InfoSortExpression c, InfoPaging d, GDAParameter[] e)
		{
			return LoadDataWithSortExpression (a, b, null, CommandType.Text, c, d, e);
		}
		public GDACursor<Model> LoadDataWithSortExpression (GDASession a, string b, System.Data.CommandType c, InfoSortExpression d, InfoPaging e, GDAParameter[] f)
		{
			return LoadDataWithSortExpression (a, b, null, c, d, e, f);
		}
		public GDACursor<Model> LoadDataWithSortExpression (GDASession a, string b, string c, System.Data.CommandType d, InfoSortExpression e, InfoPaging f, GDAParameter[] g)
		{
			return new GDACursor<Model> (GetCursorParameters (a, b, c, d, e, f, g));
		}
		public GDACursor<Model> LoadData (GDASession a, string b, params GDAParameter[] c)
		{
			return LoadDataWithSortExpression (a, b, CommandType.Text, null, null, c);
		}
		public GDACursor<Model> LoadData (GDAStoredProcedure a)
		{
			return LoadData (null, a);
		}
		public GDACursor<Model> LoadData (GDASession a, GDAStoredProcedure b)
		{
			return LoadData (a, b, null);
		}
		public GDACursor<Model> LoadData (GDASession a, GDAStoredProcedure b, string c)
		{
			return new GDACursor<Model> (GetCursorParameters (a, b, c));
		}
		public GDACursor<Model> LoadData (GDASession a, string b, string c, params GDAParameter[] d)
		{
			return LoadDataWithSortExpression (a, b, c, CommandType.Text, null, null, d);
		}
		public GDACursor<Model> LoadData (string a, string b, params GDAParameter[] c)
		{
			return LoadDataWithSortExpression (null, a, b, CommandType.Text, null, null, c);
		}
		public GDACursor<Model> LoadData (string a, string b)
		{
			return LoadDataWithSortExpression (null, a, b, CommandType.Text, null, null, null);
		}
		public GDACursor<Model> LoadData (string a, params GDAParameter[] b)
		{
			return LoadDataWithSortExpression (a, null, null, b);
		}
		public GDACursor<Model> LoadData (string a)
		{
			return LoadDataWithSortExpression (a, null, null, null);
		}
		public Model LoadOneData (GDASession a, string b)
		{
			GDAList<Model> c = LoadDataWithSortExpression (a, b, null, CommandType.Text, null, null, null);
			if (c.Count > 0)
				return c [0];
			else
				return default(Model);
		}
		public Model LoadOneData (string a)
		{
			GDAList<Model> b = LoadDataWithSortExpression (null, a, null, CommandType.Text, null, null, null);
			if (b.Count > 0)
				return b [0];
			else
				return default(Model);
		}
		public Model LoadOneData (string a, string b)
		{
			return LoadOneData (null, a, b, null);
		}
		public Model LoadOneData (string a, params GDAParameter[] b)
		{
			return LoadOneData (null, a, null, b);
		}
		public Model LoadOneData (string a, string b, params GDAParameter[] c)
		{
			return LoadOneData (null, a, b, c);
		}
		public Model LoadOneData (GDASession a, string b, params GDAParameter[] c)
		{
			return LoadOneData (a, b, null, c);
		}
		public Model LoadOneData (GDASession a, string b, string c, params GDAParameter[] d)
		{
			GDAList<Model> e = LoadData (a, b, c, d);
			if (e.Count > 0)
				return e [0];
			else
				return default(Model);
		}
		public Model LoadOneData (GDAStoredProcedure a, string b)
		{
			return LoadOneData (null, a, b);
		}
		public Model LoadOneData (GDAStoredProcedure a)
		{
			return LoadOneData (null, a, null);
		}
		public Model LoadOneData (GDASession a, GDAStoredProcedure b)
		{
			return LoadOneData (a, b, null);
		}
		public Model LoadOneData (GDASession a, GDAStoredProcedure b, string c)
		{
			GDAList<Model> d = LoadData (a, b, c);
			if (d.Count > 0)
				return d [0];
			else
				return default(Model);
		}
		public Model RecoverData (Model a, string b, params GDAParameter[] c)
		{
			return RecoverData (null, a, b, c);
		}
		public Model RecoverData (GDASession a, Model b, string c, params GDAParameter[] d)
		{
			IDbConnection e = CreateConnection (a);
			IDbCommand f = CreateCommand (a, e);
			string g = null;
			if (d != null)
				for (int h = 0; h < d.Length; h++) {
					try {
						g = (d [h].ParameterName [0] != '?' ? d [h].ParameterName : UserProvider.ParameterPrefix + d [h].ParameterName.Substring (1) + UserProvider.ParameterSuffix);
					}
					catch (Exception ex) {
						throw new GDAException ("Error on make parameter name '" + d [h].ParameterName + "'.", ex);
					}
					c = c.Replace (d [h].ParameterName, g);
					f.Parameters.Add (GDA.Helper.GDAHelper.ConvertGDAParameter (f, d [h], UserProvider));
				}
			f.CommandText = c;
			if (a == null && e.State != ConnectionState.Open) {
				try {
					e.Open ();
				}
				catch (Exception ex) {
					throw new GDAException (ex);
				}
				GDAConnectionManager.NotifyConnectionOpened (e);
			}
			IDataReader j = null;
			try {
				SendMessageDebugTrace ("CommandText: " + f.CommandText);
				using (var k = Diagnostics.GDATrace.CreateExecutionHandler (f))
					try {
						j = f.ExecuteReader ();
					}
					catch (Exception ex) {
						ex = new GDAException (ex);
						k.Fail (ex);
						throw ex;
					}
				if (j.Read ()) {
					var l = new TranslatorDataInfoCollection (MappingManager.GetMappers<Model> (null, null));
					l.ProcessFieldsPositions (j);
					IDataRecord m = j;
					RecoverValueOfResult (ref m, l, ref b, false);
				}
				else {
					throw new ItemNotFoundException ("Item not found with submited parameters.");
				}
			}
			finally {
				if (j != null)
					j.Close ();
				if (a == null)
					try {
						e.Close ();
						e.Dispose ();
					}
					catch {
						SendMessageDebugTrace ("Error close connection.");
					}
			}
			return b;
		}
		public Model RecoverData (Model a, string b)
		{
			return RecoverData (a, b, null);
		}
		public Model RecoverData (Model a)
		{
			return RecoverData (null, a);
		}
		public Model RecoverData (GDASession a, Model b)
		{
			string c = "";
			StringBuilder d = new StringBuilder ("SELECT ");
			List<Mapper> e = Keys;
			if (e.Count == 0)
				throw new GDAException ("In model {0} not found keys for to recover data.", b.GetType ().FullName);
			DirectionParameter[] f = new DirectionParameter[] {
				DirectionParameter.Input,
				DirectionParameter.InputOutput,
				DirectionParameter.OutputOnlyInsert
			};
			List<Mapper> g = MappingManager.GetMappers<Model> (null, f);
			foreach (Mapper column in g) {
				d.Append (UserProvider.QuoteExpression (column.Name)).Append (",");
			}
			d.Remove (d.Length - 1, 1);
			d.Append (" FROM ").Append (SystemTableName).Append (" ");
			GDAParameter[] h = new GDAParameter[e.Count];
			int j = 0;
			foreach (Mapper mapper in e) {
				if (c != "")
					c += " AND ";
				h [j] = new GDAParameter (UserProvider.ParameterPrefix + mapper.Name + UserProvider.ParameterSuffix, typeof(Model).GetProperty (mapper.PropertyMapper.Name).GetValue (b, null));
				c += UserProvider.QuoteExpression (mapper.Name) + "=" + h [j].ParameterName;
				j++;
			}
			d.Append ("WHERE ").Append (c);
			return RecoverData (a, b, d.ToString (), h);
		}
		public GDAList<ClassChild> LoadDataForeignKeyParentToChild<ClassChild> (Model a, string b, InfoSortExpression c, InfoPaging d) where ClassChild : new()
		{
			IBaseDAO<ClassChild> e = GDAOperations.GetDAO<ClassChild> ();
			List<GDAParameter> f;
			string g;
			MakeSqlForeignKeyParentToChild<ClassChild> (a, b, e, out g, out f, UserProvider);
			MethodInfo h = e.GetType ().GetMethod ("GetSqlData", GDA.Common.Helper.ReflectionFlags.AllCriteria);
			object j = null;
			try {
				j = h.Invoke (e, new object[] {
					g,
					f,
					c,
					d
				});
			}
			catch (Exception ex) {
				throw new GDAException (ex.InnerException);
			}
			return (GDAList<ClassChild>)j;
		}
		private static void MakeSqlForeignKeyParentToChild<ClassChild> (Model a, string b, IBaseDAO<ClassChild> c, out string d, out List<GDAParameter> e, IProvider f) where ClassChild : new()
		{
			Type g = typeof(ClassChild);
			Type h = typeof(Model);
			GroupOfRelationshipInfo j = new GroupOfRelationshipInfo (g, h, b);
			List<ForeignKeyMapper> k = MappingManager.LoadRelationships (g, j);
			if (k.Count == 0)
				throw new GDAException ("ForeignKey " + j.ToString () + " not found in " + typeof(Model).FullName);
			e = new List<GDAParameter> ();
			string l = f.BuildTableName (MappingManager.GetTableName (g));
			d = String.Format ("SELECT * FROM {0} ", l);
			string m = "";
			foreach (ForeignKeyMapper fk in k) {
				PersistencePropertyAttribute n = MappingManager.GetPersistenceProperty (fk.PropertyOfClassRelated);
				PersistencePropertyAttribute o = MappingManager.GetPersistenceProperty (fk.PropertyModel);
				if (n == null)
					throw new GDAException ("PersistencePropertyAttribute not found in property {0}", fk.PropertyOfClassRelated.Name);
				if (o == null)
					throw new GDAException ("PersistencePropertyAttribute not found in property {0}", fk.PropertyModel.Name);
				e.Add (new GDAParameter ("?" + n.Name.Replace (" ", "_"), fk.PropertyOfClassRelated.GetValue (a, null)));
				if (m != "")
					m += " AND ";
				m += String.Format ("{0}=?{1}", f.QuoteExpression (n.Name), n.Name.Replace (" ", "_"));
			}
			d += " WHERE " + m;
		}
		public GDAList<ClassChild> LoadDataForeignKeyParentToChild<ClassChild> (Model a, string b, InfoSortExpression c) where ClassChild : new()
		{
			return LoadDataForeignKeyParentToChild<ClassChild> (a, b, c, null);
		}
		public GDAList<ClassChild> LoadDataForeignKeyParentToChild<ClassChild> (Model a, string b, InfoPaging c) where ClassChild : new()
		{
			return LoadDataForeignKeyParentToChild<ClassChild> (a, b, null, c);
		}
		public GDAList<ClassChild> LoadDataForeignKeyParentToChild<ClassChild> (Model a, string b) where ClassChild : new()
		{
			return LoadDataForeignKeyParentToChild<ClassChild> (a, b, null, null);
		}
		public int CountRowForeignKeyParentToChild<ClassChild> (Model a, string b) where ClassChild : new()
		{
			IBaseDAO<ClassChild> c = GDAOperations.GetDAO<ClassChild> ();
			List<GDAParameter> d;
			string e;
			MakeSqlForeignKeyParentToChild<ClassChild> (a, b, c, out e, out d, UserProvider);
			e = e.Replace ("SELECT *", "SELECT COUNT(*)");
			return ExecuteSqlQueryCount (e, d.ToArray ());
		}
	}
}
