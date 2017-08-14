using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Data;
using System.Reflection;
using System.Data.Common;
using System.Diagnostics;
using GDA.Interfaces;
using GDA.Collections;
using GDA.Sql;
using GDA.Sql.InterpreterExpression;
using GDA.Sql.InterpreterExpression.Nodes;
using GDA.Caching;
namespace GDA
{
	public class PersistenceObjectBase<Model> : DataAccess, IPersistenceObjectDataAccess<Model>
	{
		private TableName _tableName = null;
		protected internal readonly bool CompatibleWithPersistence;
		private bool _existsIdentityProperty = false;
		static PersistenceObjectBase ()
		{
			var a = typeof(Model).GetInterfaces ();
			var b = typeof(IObjectDataRecord);
			foreach (var type in a)
				if (type == b) {
					ImplementIObjectDataRecord = true;
					break;
				}
		}
		public PersistenceObjectBase (IProviderConfiguration a) : base (a)
		{
			CompatibleWithPersistence = typeof(Model).IsSubclassOf (typeof(Persistent));
			_existsIdentityProperty = MappingManager.CheckExistsIdentityKey (typeof(Model));
		}
		public event DebugTraceDelegate DebugTrace;
		public event PersistenceObjectOperation<Model> Inserting;
		public event PersistenceObjectOperation<Model> Inserted;
		public event PersistenceObjectOperation<Model> Updating;
		public event PersistenceObjectOperation<Model> Updated;
		public event PersistenceObjectOperation<Model> Deleting;
		public event PersistenceObjectOperation<Model> Deleted;
		~PersistenceObjectBase ()
		{
		}
		public static bool ImplementIObjectDataRecord {
			get;
			private set;
		}
		[Obsolete ("Use TableNameInfo")]
		public string TableName {
			get {
				var a = TableNameInfo;
				if (a == null)
					return null;
				return a.Name;
			}
		}
		public GDA.Sql.TableName TableNameInfo {
			get {
				if (_tableName == null) {
					_tableName = MappingManager.GetTableName (typeof(Model));
				}
				return _tableName;
			}
		}
		public string SystemTableName {
			get {
				return UserProvider.BuildTableName (TableNameInfo);
			}
		}
		public List<Mapper> Keys {
			get {
				return MappingManager.GetMappers<Model> (new PersistenceParameterType[] {
					PersistenceParameterType.Key,
					PersistenceParameterType.IdentityKey
				}, null);
			}
		}
		public bool ExistsIdentityProperty {
			get {
				return _existsIdentityProperty;
			}
		}
		internal static Dictionary<string, int> GetMapDataReader (ref List<Mapper> a, IDataReader b)
		{
			int c = b.FieldCount;
			var d = new Dictionary<string, int> ();
			int e;
			string f;
			List<string> g = new List<string> ();
			for (int h = 0; h < b.FieldCount; h++)
				g.Add (b.GetName (h).ToLower ());
			for (int h = 0; h < a.Count; h++) {
				f = a [h].Name.ToLower ();
				e = g.FindIndex (delegate (string j) {
					return j == f;
				});
				if (e < 0 && (a [h].Direction == DirectionParameter.InputOptional || a [h].Direction == DirectionParameter.InputOptionalOutput || a [h].Direction == DirectionParameter.InputOptionalOutputOnlyInsert)) {
					SendMessageDebugTrace (null, "Property InputOptional " + a [h].PropertyMapperName + " not found...");
					a.RemoveAt (h);
					h--;
					continue;
				}
				else if (e >= 0) {
					try {
						d.Add (g [e], e);
					}
					catch (ArgumentException ex) {
						throw new GDAException ("Ambiguous column name in result.", ex);
					}
				}
				else {
					throw new GDAColumnNotFoundException (a [h].Name, "");
				}
			}
			return d;
		}
		internal static void RecoverValueOfResult (ref IDataRecord a, TranslatorDataInfoCollection b, ref Model c, bool d)
		{
			object e = c;
			RecoverValueOfResult (ref a, b, ref e, ImplementIObjectDataRecord);
		}
		internal GDACursorParameters GetCursorParameters (GDASession a, IQuery b)
		{
			if (b == null)
				b = new Query ();
			b.ReturnTypeQuery = typeof(Model);
			QueryReturnInfo c = b.BuildResultInfo<Model> (this.Configuration);
			List<Mapper> d = c.RecoverProperties;
			string e = c.CommandText;
			IDbConnection f = CreateConnection (a);
			IDbCommand g = CreateCommand (a, f);
			g.CommandType = CommandType.Text;
			bool h = false;
			if (b.SkipCount > 0 || b.TakeCount > 0)
				h = true;
			if (d.Count == 0)
				throw new GDAException ("Not found properties mappers to model {0}.", typeof(Model).FullName);
			if (c.Parameters != null)
				for (int j = 0; j < c.Parameters.Count; j++) {
					try {
						string k = (c.Parameters [j].ParameterName [0] != '?' ? c.Parameters [j].ParameterName : UserProvider.ParameterPrefix + c.Parameters [j].ParameterName.Substring (1) + UserProvider.ParameterSuffix);
						e = e.Replace (c.Parameters [j].ParameterName, k);
					}
					catch (Exception ex) {
						throw new GDAException ("Error on make parameter name '" + c.Parameters [j].ParameterName + "'.", ex);
					}
					g.Parameters.Add (GDA.Helper.GDAHelper.ConvertGDAParameter (g, c.Parameters [j], UserProvider));
				}
			if (h) {
				e = UserProvider.SQLCommandLimit (d, e, b.SkipCount, b.TakeCount);
			}
			g.CommandText = e;
			return new GDACursorParameters (UserProvider, a, f, g, new TranslatorDataInfoCollection (d), h, b.SkipCount, b.TakeCount, null);
		}
		internal GDACursorParameters GetCursorParameters (GDASession a, GDAStoredProcedure b, string c)
		{
			IDbConnection d = CreateConnection (a);
			IDbCommand e = CreateCommand (a, d);
			e.CommandType = CommandType.StoredProcedure;
			e.CommandTimeout = b.CommandTimeout;
			e.CommandText = b.Name;
			List<Mapper> f = MappingManager.GetMappers<Model> (null, null);
			if (f.Count == 0)
				throw new GDAException ("Not found properties mappers to model {0}.", typeof(Model).FullName);
			if (!string.IsNullOrEmpty (c)) {
				List<Mapper> g = new List<Mapper> ();
				string[] h = c.Split (',');
				bool j = false;
				for (int k = 0; k < h.Length; k++) {
					string l = h [k].TrimStart ().TrimEnd ();
					j = false;
					foreach (Mapper m in f)
						if (string.Compare (l, m.PropertyMapperName, true) == 0) {
							g.Add (m);
							j = true;
							break;
						}
					if (!j)
						throw new GDAException ("Property {0} not found or not found in mapping.", l);
				}
				f.Clear ();
				f.AddRange (g);
			}
			foreach (GDAParameter param in b)
				e.Parameters.Add (GDA.Helper.GDAHelper.ConvertGDAParameter (e, param, UserProvider));
			return new GDACursorParameters (UserProvider, a, d, e, new TranslatorDataInfoCollection (f), false, 0, 0, delegate (object n, EventArgs o) {
				for (int k = 0; k < e.Parameters.Count; k++) {
					object q = ((IDbDataParameter)e.Parameters [k]).Value;
					b [k] = (q == DBNull.Value ? null : q);
				}
			});
		}
		internal GDACursorParameters GetCursorParameters (GDASession a, string b, string c, System.Data.CommandType d, InfoSortExpression e, InfoPaging f, GDAParameter[] g)
		{
			IDbConnection h = CreateConnection (a);
			IDbCommand j = CreateCommand (a, h);
			j.CommandType = d;
			List<Mapper> k = MappingManager.GetMappers<Model> (null, null);
			if (!string.IsNullOrEmpty (c)) {
				List<Mapper> l = new List<Mapper> ();
				string[] n = c.Split (',');
				bool o = false;
				for (int q = 0; q < n.Length; q++) {
					string r = n [q].TrimStart ().TrimEnd ();
					o = false;
					foreach (Mapper m in k)
						if (string.Compare (r, m.PropertyMapperName, true) == 0) {
							l.Add (m);
							o = true;
							break;
						}
					if (!o)
						throw new GDAException ("Property {0} not found or not found in mapping.", r);
				}
				k.Clear ();
				k.AddRange (l);
			}
			if (k.Count == 0)
				throw new GDAException ("Not found properties mappers to model {0}.", typeof(Model).FullName);
			if (e != null && e.SortColumn != null && e.SortColumn != "") {
				if (b.ToLower ().IndexOf ("ORDER BY", 0, StringComparison.OrdinalIgnoreCase) != -1)
					throw new GDAException ("Found ORDER BY in query, InforSortExpression not allow.");
				else {
					Mapper s = k.Find (delegate (Mapper t) {
						return t.PropertyMapperName == e.SortColumn;
					});
					if (s == null)
						throw new GDAException ("Property {0} not found in mapping.", e.SortColumn);
					else
						b += " ORDER BY " + ((e.AliasTable != null && e.AliasTable != "") ? (e.AliasTable + ".") : "") + UserProvider.QuoteExpression (s.Name) + ((e.Reverse) ? " DESC" : "");
				}
			}
			if (g != null)
				for (int q = 0; q < g.Length; q++) {
					if (g [q].Value is IQuery)
						continue;
					try {
						string u = (g [q].ParameterName [0] != '?' ? g [q].ParameterName : UserProvider.ParameterPrefix + g [q].ParameterName.Substring (1) + UserProvider.ParameterSuffix);
						b = b.Replace (g [q].ParameterName, u);
					}
					catch (Exception ex) {
						throw new GDAException ("Error on make parameter name '" + g [q].ParameterName + "'.", ex);
					}
					j.Parameters.Add (GDA.Helper.GDAHelper.ConvertGDAParameter (j, g [q], UserProvider));
				}
			if (f != null) {
				b = UserProvider.SQLCommandLimit (MappingManager.GetMappers<Model> (null, null), b, f.StartRow, f.PageSize);
			}
			j.CommandText = b;
			return new GDACursorParameters (UserProvider, a, h, j, new TranslatorDataInfoCollection (k), false, 0, 0, null);
		}
		internal protected string CreateDataParameter (ref IDbCommand a, Mapper b, ref Model c)
		{
			object d = b.PropertyMapper.GetValue (c, null);
			d = (d == null ? DBNull.Value : d);
			string e = UserProvider.ParameterPrefix + b.Name.Replace (" ", "_") + UserProvider.ParameterSuffix;
			SendMessageDebugTrace ("Create DataParameter -> Name: " + e + "; Value: " + (d == DBNull.Value ? "{NULL}" : d.ToString ()));
			IDbDataParameter f = a.CreateParameter ();
			f.ParameterName = e;
			if (b.PropertyMapper.PropertyType.Name == "Byte[]" && d == DBNull.Value)
				f.DbType = DbType.Binary;
			if (d is Guid)
				d = d.ToString ();
			UserProvider.SetParameterValue (f, d);
			a.Parameters.Add (f);
			return e;
		}
		public void GenerateKey (Mapper a, GenerateKeyArgs b)
		{
			if (b == null)
				throw new ArgumentNullException ("args");
			if (a == null)
				a = MappingManager.GetIdentityKey (b.ModelType);
			if (a != null && a.GeneratorKey != null)
				a.GeneratorKey.GenerateKey (this, b);
			else if (GDAOperations.GlobalGenerateKey != null)
				GDAOperations.GlobalGenerateKey (this, b);
			else
				b.Cancel = true;
		}
		public uint Insert (Model a)
		{
			return Insert (a, null);
		}
		public uint Insert (Model a, string b)
		{
			return Insert (a, b, DirectionPropertiesName.Inclusion);
		}
		public uint Insert (Model a, string b, DirectionPropertiesName c)
		{
			return Insert (null, a, b, c);
		}
		public uint Insert (GDASession a, Model b, string c, DirectionPropertiesName d)
		{
			if (b == null)
				throw new ArgumentNullException ("ObjInsert it cannot be null.");
			string e = null;
			Mapper f = MappingManager.GetIdentityKey (typeof(Model));
			if (UserProvider.GenerateIdentity && ExistsIdentityProperty) {
				e = UserProvider.GetIdentitySelect (TableNameInfo, f.Name);
			}
			SendMessageDebugTrace ("GDA call method insert.");
			bool g = false;
			if (a == null) {
				g = true;
				a = new GDASession (Configuration);
			}
			IDbConnection h = CreateConnection (a);
			IDbCommand j = CreateCommand (a, h);
			string[] k = null;
			if (c != null && c != "") {
				k = c.Split (',');
			}
			uint l = 0;
			string n = "", o = "";
			if (Inserting != null)
				Inserting (this, ref b);
			List<Mapper> q = MappingManager.GetMappers<Model> (new PersistenceParameterType[] {
				PersistenceParameterType.Field,
				PersistenceParameterType.ForeignKey,
				PersistenceParameterType.Key
			}, new DirectionParameter[] {
				DirectionParameter.Output,
				DirectionParameter.InputOptionalOutput,
				DirectionParameter.InputOutput,
				DirectionParameter.OutputOnlyInsert,
				DirectionParameter.OnlyInsert,
				DirectionParameter.InputOptionalOutputOnlyInsert
			});
			if (k != null && k.Length > 0) {
				int[] r = new int[k.Length];
				int s = 0;
				for (s = 0; s < k.Length; s++) {
					string t = k [s].Trim ();
					int u = q.FindIndex (delegate (Mapper v) {
						return v.PropertyMapperName == t;
					});
					if (u < 0)
						throw new GDAException ("Property {0} not found in mapper for insert.", t);
					else
						r [s] = u;
				}
				if (d == DirectionPropertiesName.Inclusion) {
					for (s = 0; s < r.Length; s++) {
						n += UserProvider.QuoteExpression (q [r [s]].Name) + ",";
						o += CreateDataParameter (ref j, q [r [s]], ref b) + ",";
					}
					q.Clear ();
				}
				else {
					for (s = 0; s < r.Length; s++)
						q.RemoveAt (r [s]);
				}
			}
			var w = new GenerateKeyArgs (a, typeof(Model), f, TableNameInfo.Schema, TableNameInfo.Name, (f != null ? f.Name : null), b);
			if (f != null) {
				GenerateKey (f, w);
				if (!w.Cancel) {
					l = w.KeyValue;
					try {
						object x = typeof(Convert).GetMethod ("To" + f.PropertyMapper.PropertyType.Name, new Type[] {
							typeof(uint)
						}).Invoke (null, new object[] {
							l
						});
						f.PropertyMapper.SetValue (b, x, null);
					}
					catch (Exception ex) {
						throw new GDAException (string.Format ("Fail on define identity value '{0}' to property '{1}' of model '{2}'", w.KeyValue, f.PropertyMapperName, typeof(Model).FullName), ex);
					}
					q.Insert (0, f);
				}
			}
			else
				w.Cancel = true;
			foreach (Mapper mapper in q) {
				n += UserProvider.QuoteExpression (mapper.Name) + ",";
				o += CreateDataParameter (ref j, mapper, ref b) + ",";
			}
			j.CommandText = String.Format ("INSERT INTO {0} ({1})VALUES({2}){3}", SystemTableName, n.Substring (0, n.Length - 1), o.Substring (0, o.Length - 1), (UserProvider.ExecuteCommandsOneAtATime ? "" : (Configuration != null && Configuration.Provider != null ? Configuration.Provider.StatementTerminator : "")));
			try {
				if (w.Cancel && UserProvider.GenerateIdentity && ExistsIdentityProperty) {
					if (!UserProvider.ExecuteCommandsOneAtATime) {
						j.CommandText += e;
						SendMessageDebugTrace ("CommandText: " + j.CommandText);
						object x = null;
						using (var y = Diagnostics.GDATrace.CreateExecutionHandler (j))
							try {
								x = j.ExecuteScalar ();
							}
							catch (Exception ex) {
								y.Fail (ex);
								throw;
							}
						if (x is uint)
							l = (uint)x;
						else
							l = Convert.ToUInt32 (x);
					}
					else {
						SendMessageDebugTrace ("Executing commands one at a time");
						SendMessageDebugTrace ("CommandText: " + j.CommandText);
						using (var y = Diagnostics.GDATrace.CreateExecutionHandler (j))
							try {
								y.RowsAffects = j.ExecuteNonQuery ();
							}
							catch (Exception ex) {
								y.Fail (ex);
								throw;
							}
						j.Parameters.Clear ();
						if (!string.IsNullOrEmpty (e))
							SendMessageDebugTrace (e);
						j.CommandText = e;
						object x = null;
						using (var y = Diagnostics.GDATrace.CreateExecutionHandler (j))
							try {
								x = j.ExecuteScalar ();
							}
							catch (Exception ex) {
								y.Fail (ex);
								throw ex;
							}
						if (x is uint)
							l = (uint)x;
						else
							l = Convert.ToUInt32 (x);
					}
					if (f != null) {
						try {
							object x = typeof(Convert).GetMethod ("To" + f.PropertyMapper.PropertyType.Name, new Type[] {
								typeof(uint)
							}).Invoke (null, new object[] {
								l
							});
							f.PropertyMapper.SetValue (b, x, null);
						}
						catch {
						}
					}
				}
				else {
					SendMessageDebugTrace ("CommandText: " + j.CommandText);
					using (var y = Diagnostics.GDATrace.CreateExecutionHandler (j))
						try {
							y.RowsAffects = j.ExecuteNonQuery ();
						}
						catch (Exception ex) {
							y.Fail (ex);
							throw ex;
						}
				}
			}
			catch (System.Data.DataException ex) {
				throw new GDAException (ex);
			}
			finally {
				if (g)
					try {
						h.Close ();
						h.Dispose ();
					}
					catch {
						SendMessageDebugTrace ("Error close connection.");
					}
			}
			if (Inserted != null)
				Inserted (this, ref b);
			return l;
		}
		public uint InsertForced (Model a)
		{
			return InsertForced (null, a);
		}
		public uint InsertForced (GDASession a, Model b)
		{
			if (b == null)
				throw new ArgumentNullException ("ObjInsert it cannot be null.");
			if (Inserting != null)
				Inserting (this, ref b);
			IDbConnection c = CreateConnection (a);
			IDbCommand d = CreateCommand (a, c);
			uint e = 0;
			string f = "", g = "";
			List<Mapper> h = MappingManager.GetMappers<Model> (null, new DirectionParameter[] {
				DirectionParameter.Output,
				DirectionParameter.InputOutput,
				DirectionParameter.OutputOnlyInsert,
				DirectionParameter.OnlyInsert
			});
			foreach (Mapper mapper in h) {
				f += UserProvider.QuoteExpression (mapper.Name) + ",";
				g += CreateDataParameter (ref d, mapper, ref b) + ",";
			}
			d.CommandText = String.Format ("INSERT INTO {0} ({1})VALUES({2}){3}", SystemTableName, f.Substring (0, f.Length - 1), g.Substring (0, g.Length - 1), UserProvider.StatementTerminator);
			if (a == null && c.State != ConnectionState.Open) {
				try {
					c.Open ();
				}
				catch (Exception ex) {
					throw new GDAException (ex);
				}
				GDAConnectionManager.NotifyConnectionOpened (c);
			}
			try {
				SendMessageDebugTrace ("CommandText: " + d.CommandText);
				using (var j = Diagnostics.GDATrace.CreateExecutionHandler (d))
					try {
						e = (uint)d.ExecuteNonQuery ();
						j.RowsAffects = (int)e;
					}
					catch (Exception ex) {
						j.Fail (ex);
						throw ex;
					}
			}
			catch (Exception ex) {
				throw new GDAException (ex);
			}
			finally {
				if (a == null)
					try {
						c.Close ();
						c.Dispose ();
					}
					catch {
						SendMessageDebugTrace ("Error close connection.");
					}
			}
			if (Inserted != null)
				Inserted (this, ref b);
			return e;
		}
		public int Update (Model a)
		{
			return Update (a, null);
		}
		public int Update (Model a, string b)
		{
			return Update (a, b, DirectionPropertiesName.Inclusion);
		}
		public int Update (Model a, string b, DirectionPropertiesName c)
		{
			return Update (null, a, b, c);
		}
		public int Update (GDASession a, Model b)
		{
			return Update (a, b, null, DirectionPropertiesName.Inclusion);
		}
		public int Update (GDASession a, Model b, string c, DirectionPropertiesName d)
		{
			if (b == null)
				throw new ArgumentNullException ("ObjUpdate it cannot be null.");
			IDbConnection e = CreateConnection (a);
			IDbCommand f = CreateCommand (a, e);
			string[] g = null;
			if (c != null && c != "") {
				g = c.Split (',');
			}
			if (Updating != null)
				Updating (this, ref b);
			int h = 0;
			string j = "UPDATE " + SystemTableName + " SET ", k = "";
			List<Mapper> l = MappingManager.GetMappers<Model> (new PersistenceParameterType[] {
				PersistenceParameterType.Field,
				PersistenceParameterType.ForeignKey
			}, new DirectionParameter[] {
				DirectionParameter.Output,
				DirectionParameter.InputOptionalOutput,
				DirectionParameter.InputOutput
			});
			if (g != null && g.Length > 0) {
				int[] n = new int[g.Length];
				int o = 0;
				for (o = 0; o < g.Length; o++) {
					string q = g [o].Trim ();
					int r = l.FindIndex (delegate (Mapper s) {
						return s.PropertyMapperName == q;
					});
					if (r < 0)
						throw new GDAException ("Property {0} not found in mapper for update.", q);
					else
						n [o] = r;
				}
				if (d == DirectionPropertiesName.Inclusion) {
					if (n.Length == 0)
						return 0;
					for (o = 0; o < n.Length; o++) {
						j += UserProvider.QuoteExpression (l [n [o]].Name) + "=" + CreateDataParameter (ref f, l [n [o]], ref b) + ",";
					}
					l.Clear ();
				}
				else {
					if (l.Count == 0)
						return 0;
					for (o = 0; o < n.Length; o++)
						l.RemoveAt (n [o]);
				}
			}
			else {
				if (l.Count == 0)
					return 0;
			}
			foreach (Mapper mapper in l) {
				j += UserProvider.QuoteExpression (mapper.Name) + "=" + CreateDataParameter (ref f, mapper, ref b) + ",";
			}
			l = MappingManager.GetMappers<Model> (new PersistenceParameterType[] {
				PersistenceParameterType.Key,
				PersistenceParameterType.IdentityKey
			}, new DirectionParameter[] {
				DirectionParameter.Output,
				DirectionParameter.InputOptionalOutput,
				DirectionParameter.InputOutput
			});
			if (l.Count == 0)
				throw new GDAConditionalClauseException ("Parameters do not exist to build the conditional clause.");
			foreach (Mapper mapper in l) {
				if (k.Length != 0)
					k += " AND ";
				k += UserProvider.QuoteExpression (mapper.Name) + "=" + CreateDataParameter (ref f, mapper, ref b);
			}
			f.CommandText = j.Substring (0, j.Length - 1) + " WHERE " + k;
			if (a == null && e.State != ConnectionState.Open) {
				try {
					e.Open ();
				}
				catch (Exception ex) {
					throw new GDAException (ex);
				}
				GDAConnectionManager.NotifyConnectionOpened (e);
			}
			try {
				SendMessageDebugTrace ("CommandText: " + f.CommandText);
				using (var t = Diagnostics.GDATrace.CreateExecutionHandler (f))
					try {
						h = f.ExecuteNonQuery ();
						t.RowsAffects = (int)h;
					}
					catch (Exception ex) {
						t.Fail (ex);
						throw ex;
					}
			}
			catch (Exception ex) {
				throw new GDAException (ex);
			}
			finally {
				if (a == null)
					try {
						e.Close ();
						e.Dispose ();
					}
					catch {
						SendMessageDebugTrace ("Error close connection.");
					}
			}
			if (h > 0 && Updated != null)
				Updated (this, ref b);
			return h;
		}
		public int Delete (Query a)
		{
			return Delete (null, a);
		}
		public int Delete (GDASession a, Query b)
		{
			if (b == null)
				throw new ArgumentNullException ("query");
			GDAParameter[] c = null;
			StringBuilder d = new StringBuilder (128).Append ("DELETE ");
			DirectionParameter[] e = new DirectionParameter[] {
				DirectionParameter.Input,
				DirectionParameter.InputOutput,
				DirectionParameter.OutputOnlyInsert
			};
			List<Mapper> f = MappingManager.GetMappers<Model> (null, e);
			d.Append (" FROM ");
			d.Append (SystemTableName);
			if (!string.IsNullOrEmpty (b.Where)) {
				Parser g = new Parser (new Lexer (b.Where));
				WherePart h = g.ExecuteWherePart ();
				SelectStatement j = new SelectStatement (h);
				foreach (ColumnInfo ci in j.ColumnsInfo) {
					Mapper k = f.Find (delegate (Mapper l) {
						return string.Compare (l.PropertyMapperName, ci.ColumnName, true) == 0;
					});
					if (k == null)
						throw new GDAException ("Property {0} not exists in {1} or not mapped.", ci.ColumnName, typeof(Model).FullName);
					ci.DBColumnName = k.Name;
					ci.RenameToMapper (this.Configuration.Provider);
				}
				ParserToSqlCommand n = new ParserToSqlCommand (h, UserProvider.QuoteExpressionBegin, UserProvider.QuoteExpressionEnd);
				d.Append (" WHERE ").Append (n.SqlCommand);
				c = b.Parameters.ToArray ();
			}
			return ExecuteCommand (a, d.ToString (), c);
		}
		public int Delete (Model a)
		{
			return Delete (null, a);
		}
		public int Delete (GDASession a, Model b)
		{
			if (b == null)
				throw new ArgumentNullException ("ObjDelete it cannot be null.");
			IDbConnection c = CreateConnection (a);
			IDbCommand d = CreateCommand (a, c);
			if (Deleting != null)
				Deleting (this, ref b);
			int e = 0;
			string f = "";
			List<Mapper> g = MappingManager.GetMappers<Model> (new PersistenceParameterType[] {
				PersistenceParameterType.Key,
				PersistenceParameterType.IdentityKey
			}, new DirectionParameter[] {
				DirectionParameter.Output,
				DirectionParameter.InputOptionalOutput,
				DirectionParameter.InputOutput
			});
			if (g.Count == 0)
				throw new GDAConditionalClauseException ("Parameters do not exist to build the conditional clause.");
			foreach (Mapper mapper in g) {
				if (f.Length != 0)
					f += " AND ";
				f += UserProvider.QuoteExpression (mapper.Name) + "=" + CreateDataParameter (ref d, mapper, ref b);
			}
			d.CommandText = "DELETE FROM " + SystemTableName + " WHERE " + f;
			if (a == null && c.State != ConnectionState.Open) {
				try {
					c.Open ();
				}
				catch (Exception ex) {
					throw new GDAException (ex);
				}
				GDAConnectionManager.NotifyConnectionOpened (c);
			}
			try {
				SendMessageDebugTrace ("CommandText: " + d.CommandText);
				using (var h = Diagnostics.GDATrace.CreateExecutionHandler (d))
					try {
						h.RowsAffects = d.ExecuteNonQuery ();
						e = h.RowsAffects;
					}
					catch (Exception ex) {
						h.Fail (ex);
						throw ex;
					}
			}
			catch (Exception ex) {
				throw new GDAException (ex);
			}
			finally {
				if (a == null)
					try {
						c.Close ();
						c.Dispose ();
					}
					catch {
						SendMessageDebugTrace ("Error close connection.");
					}
			}
			if (e > 0 && Deleted != null)
				Deleted (this, ref b);
			return e;
		}
		public uint InsertOrUpdate (Model a)
		{
			return InsertOrUpdate (null, a);
		}
		public uint InsertOrUpdate (GDASession a, Model b)
		{
			List<Mapper> c = Keys;
			Type d = typeof(Model);
			if (c.Count == 0)
				throw new GDAException ("Operação ilegal. Objeto do tipo \"" + d.FullName + "\" não possui chaves identificadas, por não é possível executar a ação.");
			int e = 0;
			GDAParameter[] f = new GDAParameter[c.Count];
			foreach (Mapper mapper in c) {
				f [e] = new GDAParameter ();
				f [e].ParameterName = UserProvider.ParameterPrefix + mapper.PropertyMapperName + UserProvider.ParameterSuffix;
				f [e].Value = mapper.PropertyMapper.GetValue (b, null);
				f [e].SourceColumn = mapper.Name;
				e++;
			}
			int g = GetNumberRegFound (a, f);
			if (g == 0)
				return Insert (a, b, null, DirectionPropertiesName.Inclusion);
			else if (g == 1) {
				Update (a, b, null, DirectionPropertiesName.Inclusion);
				return 0;
			}
			else
				throw new GDAException ("Existem chaves duplicadas na base de dados.");
		}
		public GDAPropertyValue GetValue (GDASession a, string b, string c, params GDAParameter[] d)
		{
			if (string.IsNullOrEmpty (c))
				throw new ArgumentNullException ("propertyName");
			List<Mapper> e = MappingManager.GetMappers<Model> (null, null);
			var f = e.Find (delegate (Mapper g) {
				return g.PropertyMapperName == c;
			});
			if (f == null)
				throw new GDAException ("Property {0} not found in {1}.", c, typeof(Model).FullName);
			foreach (GDADataRecord i in LoadResult (a, b, d))
				if (i.GetOrdinal (f.Name) >= 0)
					return new GDAPropertyValue (i.GetValue (f.Name), true);
			return new GDAPropertyValue (null, false);
		}
		public long Count ()
		{
			GDASession a = null;
			return Count (a);
		}
		public long Count (GDASession a)
		{
			var b = string.Format ("SELECT COUNT(*) FROM {0}", SystemTableName);
			IDbConnection c = CreateConnection (a);
			IDbCommand d = CreateCommand (a, c);
			if (a == null && c.State != ConnectionState.Open) {
				try {
					c.Open ();
				}
				catch (Exception ex) {
					throw new GDAException (ex);
				}
				GDAConnectionManager.NotifyConnectionOpened (c);
			}
			try {
				SendMessageDebugTrace ("CommandText: " + d.CommandText);
				long e = 0;
				using (var f = Diagnostics.GDATrace.CreateExecutionHandler (d))
					try {
						e = Convert.ToInt64 (d.ExecuteScalar ());
					}
					catch (Exception ex) {
						f.Fail (ex);
						throw ex;
					}
				SendMessageDebugTrace ("Return: " + e.ToString ());
				return e;
			}
			catch (Exception ex) {
				throw new GDAException (ex);
			}
			finally {
				if (a == null)
					try {
						c.Close ();
						c.Dispose ();
					}
					catch {
						SendMessageDebugTrace ("Error close connection.");
					}
			}
		}
		public int GetNumberRegFound (params GDAParameter[] a)
		{
			return GetNumberRegFound (null, a);
		}
		public int GetNumberRegFound (GDASession a, params GDAParameter[] b)
		{
			IDbConnection c = CreateConnection (a);
			IDbCommand d = CreateCommand (a, c);
			int e = 0;
			string f = "", g = "SELECT COUNT(*) FROM " + SystemTableName;
			d.Connection = c;
			if (b != null)
				foreach (GDAParameter p in b) {
					DbParameter h = Configuration.Provider.CreateParameter ();
					h.ParameterName = p.ParameterName.Replace ("?", UserProvider.ParameterPrefix) + UserProvider.ParameterSuffix;
					h.Value = p.Value;
					if (f != "")
						f += " AND ";
					if (p.SourceColumn == null || p.SourceColumn == "")
						throw new GDAException ("Nome do campo que o parâmetro \"" + p.ParameterName + "\" representa não foi informado.");
					f += UserProvider.QuoteExpression (p.SourceColumn) + "=" + h.ParameterName;
					SendMessageDebugTrace ("Create DataParameter -> Name: " + h.ParameterName + "; Value: " + h.Value + "; FieldName: " + h.SourceColumn);
					d.Parameters.Add (h);
				}
			if (f != "")
				g += " WHERE ";
			d.CommandText = g + f;
			if (a == null && c.State != ConnectionState.Open) {
				try {
					c.Open ();
				}
				catch (Exception ex) {
					throw new GDAException (ex);
				}
				GDAConnectionManager.NotifyConnectionOpened (c);
			}
			try {
				SendMessageDebugTrace ("CommandText: " + d.CommandText);
				using (var j = Diagnostics.GDATrace.CreateExecutionHandler (d))
					try {
						e = int.Parse (d.ExecuteScalar ().ToString ());
					}
					catch (Exception ex) {
						j.Fail (ex);
						throw ex;
					}
				SendMessageDebugTrace ("Return: " + e.ToString ());
			}
			catch (Exception ex) {
				throw new GDAException (ex);
			}
			finally {
				if (a == null)
					try {
						c.Close ();
						c.Dispose ();
					}
					catch {
						SendMessageDebugTrace ("Error close connection.");
					}
			}
			return e;
		}
		public bool CheckExist (GDASession a, ValidationMode b, string c, object d, Model e)
		{
			if (string.IsNullOrEmpty (c))
				throw new ArgumentNullException ("properyName");
			else if (e == null)
				throw new ArgumentNullException ("parent");
			string f = "";
			GDAParameter[] g = null;
			Mapper h = null;
			foreach (var i in MappingManager.GetMappers<Model> ())
				if (i.PropertyMapperName == c) {
					h = i;
					break;
				}
			if (h == null)
				throw new GDAException ("Property {0} of model {1} not mapped", c, typeof(Model).FullName);
			StringBuilder j = new StringBuilder ("SELECT COUNT(*)");
			j.Append (" FROM ").Append (SystemTableName).Append (" ");
			if (b == ValidationMode.Insert) {
				j.Append (string.Format ("WHERE {0}={1}", UserProvider.QuoteExpression (h.Name), UserProvider.ParameterPrefix + h.Name + UserProvider.ParameterSuffix));
				g = new GDAParameter[] {
					new GDAParameter (UserProvider.ParameterPrefix + h.Name + UserProvider.ParameterSuffix, d)
				};
			}
			else {
				List<Mapper> k = Keys;
				if (k.Count == 0)
					throw new GDAException ("In model {0} not found keys for to recover data.", e.GetType ().FullName);
				g = new GDAParameter[k.Count + 1];
				int l = 0;
				foreach (Mapper mapper in k) {
					if (f != "")
						f += " AND ";
					g [l] = new GDAParameter (UserProvider.ParameterPrefix + mapper.Name + UserProvider.ParameterSuffix, typeof(Model).GetProperty (mapper.PropertyMapper.Name).GetValue (e, null));
					f += UserProvider.QuoteExpression (mapper.Name) + "<>" + g [l].ParameterName;
					l++;
				}
				if (f != "")
					f += " AND ";
				f += string.Format ("{0}={1}", UserProvider.QuoteExpression (h.Name), UserProvider.ParameterPrefix + h.Name + UserProvider.ParameterSuffix);
				g [g.Length - 1] = new GDAParameter (UserProvider.ParameterPrefix + h.Name + UserProvider.ParameterSuffix, d);
				j.Append ("WHERE ").Append (f);
			}
			return ExecuteSqlQueryCount (a, j.ToString (), g) > 0;
		}
		public virtual GDADataRecordCursor<Model> SelectToDataRecord (GDASession a, IQuery b)
		{
			return new GDADataRecordCursor<Model> (GetCursorParameters (a, b));
		}
	}
}
