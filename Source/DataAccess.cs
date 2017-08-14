using System;
using System.Collections.Generic;
using System.Text;
using GDA.Interfaces;
using System.Data;
using System.Data.Common;
using GDA.Collections;
using GDA.Sql;
namespace GDA
{
	public class DataAccess : IPersistenceObjectBase
	{
		private IProviderConfiguration providerConfig;
		public IProvider UserProvider {
			get {
				return providerConfig.Provider;
			}
		}
		public IProviderConfiguration Configuration {
			get {
				return providerConfig;
			}
		}
		public DataAccess (IProviderConfiguration a)
		{
			if (a == null)
				throw new ArgumentNullException ("providerConfig");
			this.providerConfig = a;
		}
		public DataAccess () : this (GDASettings.DefaultProviderConfiguration)
		{
		}
		protected void SendMessageDebugTrace (string a)
		{
			#if DEBUG
						            //System.Diagnostics.Debug.WriteLine(message);
#endif
			if (GDASettings.EnabledDebugTrace) {
				try {
					GDAOperations.CallDebugTrace (this, a);
				}
				catch (Exception ex) {
					throw new Diagnostics.GDATraceException (ex);
				}
			}
		}
		public static object ConvertType (object a, Type b, Type c)
		{
			return ValueConverterManager.Instance.Convert (a, c, System.Globalization.CultureInfo.InvariantCulture);
		}
		internal protected static object ConvertValue (object a, Type b)
		{
			if (a != null) {
				var c = a.GetType ();
				a = ConvertType (a, c, b);
			}
			return a;
		}
		internal static void SendMessageDebugTrace (object a, string b)
		{
			#if DEBUG
						            //System.Diagnostics.Debug.WriteLine(message);
#endif
			if (GDASettings.EnabledDebugTrace)
				GDAOperations.CallDebugTrace (a, b);
		}
		internal protected IDbConnection CreateConnection (GDASession a)
		{
			if (a != null) {
				if (a.ProviderConfiguration == null)
					a.DefineConfiguration (this.providerConfig);
				return a.CurrentConnection;
			}
			else {
				var b = Configuration.CreateConnection ();
				GDAConnectionManager.NotifyConnectionCreated (b);
				return b;
			}
		}
		public IDbCommand CreateCommand (GDASession a, IDbConnection b)
		{
			if (a != null)
				return a.CreateCommand ();
			else {
				IDbCommand c = UserProvider.CreateCommand ();
				c.Connection = b;
				c.CommandTimeout = GDASession.DefaultCommandTimeout;
				return c;
			}
		}
		internal GDACursorParameters GetLoadResultCursorParameters (GDASession a, GDAStoredProcedure b)
		{
			IDbConnection c = CreateConnection (a);
			IDbCommand d = CreateCommand (a, c);
			PrepareCommand (a, b, d);
			return new GDACursorParameters (this.UserProvider, a, c, d, null, false, 0, 0, null);
		}
		public void PrepareCommand (GDASession a, GDAStoredProcedure b, IDbCommand c)
		{
			c.CommandType = CommandType.StoredProcedure;
			c.CommandTimeout = b.CommandTimeout;
			c.CommandText = b.Name;
			foreach (GDAParameter param in b)
				c.Parameters.Add (GDA.Helper.GDAHelper.ConvertGDAParameter (c, param, UserProvider));
		}
		internal GDACursorParameters GetLoadResultCursorParameters (GDASession a, CommandType b, int c, string d, InfoPaging e, params GDAParameter[] f)
		{
			IDbConnection g = CreateConnection (a);
			IDbCommand h = CreateCommand (a, g);
			h.CommandType = b;
			if (c >= 0)
				h.CommandTimeout = c;
			d = PrepareCommand (a, h, d, e, f);
			return new GDACursorParameters (UserProvider, a, g, h, null, e != null, e == null ? 0 : e.StartRow, e == null ? 0 : e.PageSize, (i, j) =>  {
				for (int k = 0; k < h.Parameters.Count; k++) {
					var l = (IDbDataParameter)h.Parameters [k];
					if (l.Direction == ParameterDirection.Output || l.Direction == ParameterDirection.ReturnValue)
						f [k].Value = ((IDbDataParameter)h.Parameters [k]).Value;
				}
			});
		}
		public string PrepareCommand (GDASession a, IDbCommand b, string c, InfoPaging d, GDAParameter[] e)
		{
			if (e != null)
				foreach (GDAParameter param in e) {
					try {
						string f = (param.ParameterName [0] != '?' ? param.ParameterName : UserProvider.ParameterPrefix + param.ParameterName.Substring (1) + UserProvider.ParameterSuffix);
						c = c.Replace (param.ParameterName, f);
					}
					catch (Exception ex) {
						throw new GDAException ("Error on make parameter name '" + param.ParameterName + "'.", ex);
					}
					b.Parameters.Add (GDA.Helper.GDAHelper.ConvertGDAParameter (b, param, UserProvider));
				}
			var g = a != null ? a.ProviderConfiguration.Provider : GDASettings.DefaultProviderConfiguration.Provider;
			if (g.SupportSQLCommandLimit && d != null)
				c = g.SQLCommandLimit (!string.IsNullOrEmpty (d.KeyFieldName) ? new List<Mapper> {
					new Mapper (null, d.KeyFieldName, DirectionParameter.InputOutput, PersistenceParameterType.Key, 0, null, null)
				} : null, c, d.StartRow, d.PageSize);
			b.CommandText = c;
			return c;
		}
		public static void RecoverValueOfResult (ref IDataRecord a, TranslatorDataInfoCollection b, ref object c, bool d)
		{
			if (c == null)
				return;
			foreach (TranslatorDataInfo rdi in b) {
				if (rdi.FieldPosition < 0)
					continue;
				object e;
				try {
					e = a [rdi.FieldPosition];
				}
				catch (KeyNotFoundException) {
					throw new GDAColumnNotFoundException (rdi.FieldName, "");
				}
				catch (Exception ex) {
					throw new GDAException ("Error to recover value of field: " + rdi.FieldName + "; Exception: " + ex.Message, ex);
				}
				if (e == DBNull.Value)
					e = null;
				var f = e != null ? e.GetType ().Name : "null";
				if (rdi.PathLength > 0)
					try {
						rdi.SetValue (c, ConvertValue (e, rdi.Property.PropertyType));
					}
					catch (Exception ex) {
						if (e != null)
							throw new GDAException (String.Format ("Error to convert type {0} to type {1} of field:{2}.", f, rdi.Property.PropertyType.Name, rdi.FieldName), ex);
						else
							throw new GDAException (String.Format ("Error to convert type {0} to null", rdi.Property.PropertyType.Name), ex);
					}
			}
			if (d) {
				var g = (IObjectDataRecord)c;
				if (g.LoadMappedsRecordFields) {
					for (int h = 0; h < a.FieldCount; h++) {
						var e = a [h];
						g.InsertRecordField (a.GetName (h), e == DBNull.Value ? null : e);
					}
				}
				else {
					for (int h = 0; h < a.FieldCount; h++) {
						var i = a.GetName (h);
						if (b.FindIndex (j => string.Compare (j.FieldName, i, true) == 0) < 0) {
							var e = a [h];
							g.InsertRecordField (i, e == DBNull.Value ? null : e);
						}
					}
				}
			}
		}
		public IDbConnection CreateConnection ()
		{
			IDbConnection a = UserProvider.CreateConnection ();
			a.ConnectionString = this.Configuration.ConnectionString;
			return a;
		}
		public int ExecuteCommand (GDAStoredProcedure a)
		{
			return ExecuteCommand (null, a);
		}
		public int ExecuteCommand (GDASession a, GDAStoredProcedure b)
		{
			IDbConnection c = CreateConnection (a);
			IDbCommand d = CreateCommand (a, c);
			int e = 0;
			try {
				b.Prepare (d, UserProvider);
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
					SendMessageDebugTrace (d.CommandText);
					e = d.ExecuteNonQuery ();
				}
				catch (Exception ex) {
					throw new GDAException ("StoredProcedure: " + d.CommandText + ". --> " + ex.Message, ex);
				}
				for (int f = 0; f < d.Parameters.Count; f++)
					b [f] = ((IDbDataParameter)d.Parameters [f]).Value;
			}
			finally {
				try {
					d.Dispose ();
					d = null;
				}
				finally {
					if (a == null) {
						c.Close ();
						c.Dispose ();
					}
				}
			}
			return e;
		}
		public object ExecuteScalar (GDAStoredProcedure a)
		{
			return ExecuteScalar (null, a);
		}
		public object ExecuteScalar (GDASession a, GDAStoredProcedure b)
		{
			IDbConnection c = CreateConnection (a);
			IDbCommand d = CreateCommand (a, c);
			object e = null;
			try {
				b.Prepare (d, UserProvider);
				if (a == null && c.State != ConnectionState.Open) {
					try {
						c.Open ();
					}
					catch (Exception ex) {
						throw new GDAException (ex);
					}
					GDAConnectionManager.NotifyConnectionOpened (c);
				}
				SendMessageDebugTrace (d.CommandText);
				try {
					using (var f = Diagnostics.GDATrace.CreateExecutionHandler (d))
						try {
							e = d.ExecuteScalar ();
						}
						catch (Exception ex) {
							f.Fail (ex);
							throw ex;
						}
				}
				catch (Exception ex) {
					throw new GDAException ("StoredProcedure: " + d.CommandText + "; --> " + ex.Message, ex);
				}
				for (int g = 0; g < d.Parameters.Count; g++)
					b [g] = ((IDbDataParameter)d.Parameters [g]).Value;
			}
			finally {
				try {
					d.Dispose ();
					d = null;
				}
				finally {
					if (a == null) {
						c.Close ();
						c.Dispose ();
					}
				}
			}
			return e;
		}
		public int ExecuteCommand (GDASession a, CommandType b, int c, string d, params GDAParameter[] e)
		{
			if (d == null)
				throw new ArgumentNullException ("sqlQuery");
			else if (d == "")
				throw new ArgumentException ("sqlQuery cannot empty.");
			int f = 0;
			IDbConnection g = CreateConnection (a);
			IDbCommand h = CreateCommand (a, g);
			try {
				SendMessageDebugTrace (d);
				string i = null;
				if (e != null)
					for (int j = 0; j < e.Length; j++) {
						i = e [j].ParameterName.Replace ("?", UserProvider.ParameterPrefix) + UserProvider.ParameterSuffix;
						d = d.Replace (e [j].ParameterName, i);
						e [j].ParameterName = i;
						IDbDataParameter k = GDA.Helper.GDAHelper.ConvertGDAParameter (h, e [j], UserProvider);
						h.Parameters.Add (k);
					}
				h.CommandText = d;
				h.CommandType = b;
				h.CommandTimeout = c;
				if (a == null && g.State != ConnectionState.Open) {
					try {
						g.Open ();
					}
					catch (Exception ex) {
						throw new GDAException (ex);
					}
					GDAConnectionManager.NotifyConnectionOpened (g);
				}
				try {
					SendMessageDebugTrace (h.CommandText);
					using (var l = Diagnostics.GDATrace.CreateExecutionHandler (h))
						try {
							l.RowsAffects = f = h.ExecuteNonQuery ();
						}
						catch (Exception ex) {
							l.Fail (ex);
							throw ex;
						}
					SendMessageDebugTrace ("Return: " + f.ToString ());
				}
				catch (Exception ex) {
					throw new GDAException ("SqlQuery: " + d + "; --> " + ex.Message, ex);
				}
				for (int j = 0; j < h.Parameters.Count; j++)
					e [j].Value = ((IDbDataParameter)h.Parameters [j]).Value;
			}
			finally {
				try {
					h.Dispose ();
					h = null;
				}
				finally {
					if (a == null) {
						g.Close ();
						g.Dispose ();
					}
				}
			}
			return f;
		}
		public int ExecuteCommand (GDASession a, string b, params GDAParameter[] c)
		{
			return ExecuteCommand (a, CommandType.Text, GDASession.DefaultCommandTimeout, b, c);
		}
		public int ExecuteCommand (string a, params GDAParameter[] b)
		{
			return ExecuteCommand (null, CommandType.Text, GDASession.DefaultCommandTimeout, a, b);
		}
		public int ExecuteCommand (string a)
		{
			return ExecuteCommand (a, null);
		}
		public int ExecuteSqlQueryCount (GDASession a, string b, params GDAParameter[] c)
		{
			object d = ExecuteScalar (a, b, c);
			if (d != null)
				return int.Parse (d.ToString ());
			else
				return 0;
		}
		public int ExecuteSqlQueryCount (string a, params GDAParameter[] b)
		{
			return ExecuteSqlQueryCount (null, a, b);
		}
		public object ExecuteScalar (string a, params GDAParameter[] b)
		{
			return ExecuteScalar (null, a, b);
		}
		public object ExecuteScalar (GDASession a, CommandType b, int c, string d, params GDAParameter[] e)
		{
			object f;
			IDbConnection g = CreateConnection (a);
			IDbCommand h = CreateCommand (a, g);
			try {
				if (e != null)
					for (int i = 0; i < e.Length; i++) {
						string j = (e [i].ParameterName [0] != '?' ? e [i].ParameterName : UserProvider.ParameterPrefix + e [i].ParameterName.Substring (1) + UserProvider.ParameterSuffix);
						d = d.Replace (e [i].ParameterName, j);
						h.Parameters.Add (GDA.Helper.GDAHelper.ConvertGDAParameter (h, e [i], UserProvider));
					}
				h.CommandText = d;
				h.CommandType = b;
				h.CommandTimeout = c;
				if (a == null && g.State != ConnectionState.Open) {
					try {
						g.Open ();
					}
					catch (Exception ex) {
						throw new GDAException (ex);
					}
					GDAConnectionManager.NotifyConnectionOpened (g);
				}
				try {
					SendMessageDebugTrace (h.CommandText);
					using (var k = Diagnostics.GDATrace.CreateExecutionHandler (h))
						try {
							f = h.ExecuteScalar ();
						}
						catch (Exception ex) {
							k.Fail (ex);
							throw ex;
						}
					if (f != DBNull.Value && f != null)
						SendMessageDebugTrace ("Return: " + f.ToString ());
					else {
						f = null;
						SendMessageDebugTrace ("Return: null");
					}
				}
				catch (Exception ex) {
					throw new GDAException (ex);
				}
			}
			finally {
				try {
					h.Dispose ();
					h = null;
				}
				finally {
					if (a == null) {
						g.Close ();
						g.Dispose ();
					}
				}
			}
			return f;
		}
		public object ExecuteScalar (GDASession a, string b, params GDAParameter[] c)
		{
			return ExecuteScalar (a, CommandType.Text, GDASession.DefaultCommandTimeout, b, c);
		}
		public object ExecuteScalar (GDASession a, string b)
		{
			return ExecuteScalar (a, b, null);
		}
		public object ExecuteScalar (string a)
		{
			return ExecuteScalar (a, null);
		}
		[Obsolete]
		public GDACursor<T> LoadValues<T> (string a, params GDAParameter[] b) where T : new()
		{
			return LoadValues<T> (null, a, b);
		}
		[Obsolete]
		public GDACursor<T> LoadValues<T> (GDAStoredProcedure a) where T : new()
		{
			IDbConnection b = CreateConnection (null);
			IDbCommand c = CreateCommand (null, b);
			c.Connection = b;
			a.Prepare (c, UserProvider);
			return new GDACursor<T> (UserProvider, null, b, c, (d, e) =>  {
				for (int f = 0; f < c.Parameters.Count; f++) {
					var g = (IDbDataParameter)c.Parameters [f];
					if (g.Direction == ParameterDirection.Output || g.Direction == ParameterDirection.ReturnValue)
						a [f] = ((IDbDataParameter)c.Parameters [f]).Value;
				}
			});
		}
		[Obsolete]
		public GDACursor<T> LoadValues<T> (GDASession a, GDAStoredProcedure b) where T : new()
		{
			IDbConnection c = CreateConnection (a);
			IDbCommand d = CreateCommand (a, c);
			d.Connection = c;
			b.Prepare (d, UserProvider);
			return new GDACursor<T> (UserProvider, a, c, d, (e, f) =>  {
				for (int g = 0; g < d.Parameters.Count; g++) {
					var h = (IDbDataParameter)d.Parameters [g];
					if (h.Direction == ParameterDirection.Output || h.Direction == ParameterDirection.ReturnValue)
						b [g] = ((IDbDataParameter)d.Parameters [g]).Value;
				}
			});
		}
		[Obsolete]
		public GDACursor<T> LoadValues<T> (GDASession a, string b, params GDAParameter[] c) where T : new()
		{
			IDbConnection d = CreateConnection (a);
			IDbCommand e = CreateCommand (a, d);
			if (c != null)
				foreach (GDAParameter param in c) {
					e.Parameters.Add (GDA.Helper.GDAHelper.ConvertGDAParameter (e, param, UserProvider));
				}
			e.CommandText = b;
			e.Connection = d;
			return new GDACursor<T> (UserProvider, a, d, e, (f, g) =>  {
				for (int h = 0; h < e.Parameters.Count; h++) {
					var i = (IDbDataParameter)e.Parameters [h];
					if (i.Direction == ParameterDirection.Output || i.Direction == ParameterDirection.ReturnValue)
						c [h].Value = ((IDbDataParameter)e.Parameters [h]).Value;
				}
			});
		}
		public GDADataRecordCursor LoadResult (string a, params GDAParameter[] b)
		{
			return LoadResult (null, a, b);
		}
		public GDADataRecordCursor LoadResult (GDASession a, string b, params GDAParameter[] c)
		{
			return new GDADataRecordCursor (GetLoadResultCursorParameters (a, CommandType.Text, -1, b, null, c));
		}
		public GDADataRecordCursor LoadResult (GDASession a, CommandType b, int c, string d, params GDAParameter[] e)
		{
			return new GDADataRecordCursor (GetLoadResultCursorParameters (a, b, c, d, null, e));
		}
		public GDADataRecordCursor LoadResult (GDASession a, CommandType b, int c, string d, InfoPaging e, params GDAParameter[] f)
		{
			return new GDADataRecordCursor (GetLoadResultCursorParameters (a, b, c, d, e, f));
		}
		public GDADataRecordCursor LoadResult (GDAStoredProcedure a)
		{
			return LoadResult (null, a);
		}
		public GDADataRecordCursor LoadResult (GDASession a, GDAStoredProcedure b)
		{
			return new GDADataRecordCursor (GetLoadResultCursorParameters (a, b));
		}
		public long Count (IQuery a)
		{
			return Count (null, a);
		}
		public long Count (GDASession a, IQuery b)
		{
			IProvider c = a != null && a.ProviderConfiguration != null ? a.ProviderConfiguration.Provider : GDA.GDASettings.DefaultProviderConfiguration.Provider;
			QueryReturnInfo d = b.BuildResultInfo2 (c, "COUNT(*)");
			return Convert.ToInt64 (ExecuteScalar (a, d.CommandText, d.Parameters.ToArray ()));
		}
		public double Sum (GDASession a, IQuery b)
		{
			IProvider c = a != null && a.ProviderConfiguration != null ? a.ProviderConfiguration.Provider : GDA.GDASettings.DefaultProviderConfiguration.Provider;
			QueryReturnInfo d = b.BuildResultInfo2 (c, "SUM(" + b.AggregationFunctionProperty + ")");
			object e = ExecuteScalar (a, d.CommandText, d.Parameters.ToArray ());
			if (e == null)
				return 0.0d;
			else
				return Convert.ToDouble (e);
		}
		public double Max (GDASession a, IQuery b)
		{
			IProvider c = a != null && a.ProviderConfiguration != null ? a.ProviderConfiguration.Provider : GDA.GDASettings.DefaultProviderConfiguration.Provider;
			QueryReturnInfo d = b.BuildResultInfo2 (c, "MAX(" + b.AggregationFunctionProperty + ")");
			object e = ExecuteScalar (a, d.CommandText, d.Parameters.ToArray ());
			if (e == null)
				return 0.0d;
			else
				return Convert.ToDouble (e);
		}
		public double Min (GDASession a, IQuery b)
		{
			IProvider c = a != null && a.ProviderConfiguration != null ? a.ProviderConfiguration.Provider : GDA.GDASettings.DefaultProviderConfiguration.Provider;
			QueryReturnInfo d = b.BuildResultInfo2 (c, "MIN(" + b.AggregationFunctionProperty + ")");
			object e = ExecuteScalar (a, d.CommandText, d.Parameters.ToArray ());
			if (e == null)
				return 0.0d;
			else
				return Convert.ToDouble (e);
		}
		public double Avg (GDASession a, IQuery b)
		{
			IProvider c = a != null && a.ProviderConfiguration != null ? a.ProviderConfiguration.Provider : GDA.GDASettings.DefaultProviderConfiguration.Provider;
			QueryReturnInfo d = b.BuildResultInfo2 (c, "AVG(" + b.AggregationFunctionProperty + ")");
			object e = ExecuteScalar (a, d.CommandText, d.Parameters.ToArray ());
			if (e == null)
				return 0.0d;
			else
				return Convert.ToDouble (e);
		}
	}
}
