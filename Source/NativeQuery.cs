using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using GDA.Caching;
using GDA.Sql.InterpreterExpression.Nodes;
using GDA.Sql.InterpreterExpression;
using GDA.Collections;
namespace GDA.Sql
{
	public class NativeQuery : BaseQuery, IGDAParameterContainer
	{
		private string _commandText;
		private CommandType _commandType = CommandType.Text;
		private int _commandTimeout = GDASession.DefaultCommandTimeout;
		private GDAParameterCollection _parameters = new GDAParameterCollection ();
		private string _orderClause;
		private string _selectProperties;
		private string _keyFieldName;
		public string CommandText {
			get {
				return _commandText;
			}
			set {
				_commandText = value;
			}
		}
		public List<GDAParameter> Parameters {
			get {
				return _parameters;
			}
		}
		public string Order {
			get {
				return _orderClause;
			}
			set {
				_orderClause = value;
			}
		}
		public string SelectProperties {
			get {
				return _selectProperties;
			}
		}
		public CommandType CommandType {
			get {
				return _commandType;
			}
			set {
				_commandType = value;
			}
		}
		public int CommandTimeout {
			get {
				return _commandTimeout;
			}
			set {
				_commandTimeout = value;
			}
		}
		public string KeyFieldName {
			get {
				return _keyFieldName;
			}
			set {
				_keyFieldName = value;
			}
		}
		public NativeQuery ()
		{
		}
		public NativeQuery (string a)
		{
			_commandText = a;
		}
		public NativeQuery Add (GDAParameter a)
		{
			if (a != null) {
				var b = this._parameters.FindIndex (c => c.ParameterName == a.ParameterName);
				if (b >= 0)
					this._parameters.RemoveAt (b);
				this._parameters.Add (a);
			}
			return this;
		}
		public NativeQuery Add (params GDAParameter[] a)
		{
			foreach (var i in a) {
				var b = this._parameters.FindIndex (c => c.ParameterName == i.ParameterName);
				if (b >= 0)
					this._parameters.RemoveAt (b);
				this._parameters.Add (i);
			}
			return this;
		}
		public NativeQuery Add (IEnumerable<GDAParameter> a)
		{
			foreach (var i in a) {
				var b = this._parameters.FindIndex (c => c.ParameterName == i.ParameterName);
				if (b >= 0)
					this._parameters.RemoveAt (b);
				this._parameters.Add (i);
			}
			return this;
		}
		public NativeQuery Add (DbType a, object b)
		{
			return Add ("", a, b);
		}
		public NativeQuery Add (string a, DbType b, object c)
		{
			return Add (a, b, 0, c);
		}
		public NativeQuery Add (string a, object b)
		{
			return Add (new GDAParameter (a, b));
		}
		public NativeQuery Add (DbType a, int b, object c)
		{
			return Add ("", a, b, c);
		}
		public NativeQuery Add (string a, DbType b, int c, object d)
		{
			GDAParameter e = new GDAParameter ();
			e.ParameterName = a;
			e.DbType = b;
			e.Size = c;
			e.Value = d;
			var f = this._parameters.FindIndex (g => g.ParameterName == e.ParameterName);
			if (f >= 0)
				this._parameters.RemoveAt (f);
			this._parameters.Add (e);
			return this;
		}
		public NativeQuery Select (string a)
		{
			_selectProperties = a;
			return this;
		}
		public NativeQuery SetOrder (string a)
		{
			this._orderClause = a;
			return this;
		}
		public NativeQuery SetCommandType (CommandType a)
		{
			_commandType = a;
			return this;
		}
		public NativeQuery SetCommandTimeout (int a)
		{
			_commandTimeout = a;
			return this;
		}
		public long Count ()
		{
			return Count (null);
		}
		public long Count (GDASession a)
		{
			return GDAOperations.Count (a, this);
		}
		public NativeQuery Skip (int a)
		{
			_skipCount = a;
			return this;
		}
		public NativeQuery Take (int a)
		{
			_takeCount = a;
			return this;
		}
		public int Execute ()
		{
			return new DataAccess ().ExecuteCommand (null, this.CommandType, this.CommandTimeout, this.CommandText, this.Parameters.ToArray ());
		}
		public object ExecuteScalar (GDASession a)
		{
			DataAccess b = null;
			if (a != null)
				b = new DataAccess (a.ProviderConfiguration);
			else
				b = new DataAccess ();
			return b.ExecuteScalar (a, this.CommandType, this.CommandTimeout, this.CommandText, this.Parameters.ToArray ());
		}
		public object ExecuteScalar ()
		{
			return new DataAccess ().ExecuteScalar (null, this.CommandType, this.CommandTimeout, this.CommandText, this.Parameters.ToArray ());
		}
		public int Execute (GDASession a)
		{
			DataAccess b = null;
			if (a != null)
				b = new DataAccess (a.ProviderConfiguration);
			else
				b = new DataAccess ();
			return b.ExecuteCommand (a, this.CommandType, this.CommandTimeout, this.CommandText, this.Parameters.ToArray ());
		}
		public ResultList<T> ToResultList<T> (int a) where T : new()
		{
			return new ResultList<T> (this, a);
		}
		public ResultList<T> ToResultList<T> (GDASession a, int b) where T : new()
		{
			return new ResultList<T> (this, a, b);
		}
		public GDADataRecordCursor ToDataRecords ()
		{
			return new DataAccess ().LoadResult (null, this.CommandType, this.CommandTimeout, this.CommandText, this.TakeCount > 0 || this.SkipCount > 0 ? new InfoPaging (this.SkipCount, this.TakeCount) : null, this.Parameters.ToArray ());
		}
		public GDADataRecordCursor ToDataRecords (GDASession a)
		{
			DataAccess b = null;
			if (a != null)
				b = new DataAccess (a.ProviderConfiguration);
			else
				b = new DataAccess ();
			return b.LoadResult (a, this.CommandType, this.CommandTimeout, this.CommandText, this.TakeCount > 0 || this.SkipCount > 0 ? new InfoPaging (this.SkipCount, this.TakeCount) {
				KeyFieldName = this.KeyFieldName
			} : null, this.Parameters.ToArray ());
		}
		public override QueryReturnInfo BuildResultInfo2 (GDA.Interfaces.IProvider a, string b)
		{
			return BuildResultInfo2 (a, b, new Dictionary<string, Type> ());
		}
		public override QueryReturnInfo BuildResultInfo2 (GDA.Interfaces.IProvider a, string b, Dictionary<string, Type> c)
		{
			if (string.IsNullOrEmpty (_commandText))
				throw new QueryException ("Command text not informed.");
			var d = _commandText;
			d = d.TrimStart (' ', '\r', '\n', '\t');
			var e = Regex.Match (_commandText, "SELECT(?<selectpart>(\r|\n|\r\n|.*?)*?)FROM", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
			if (!e.Success)
				throw new QueryException ("Invalid aggregation function.\r\nNot found SELECT ... FROM for substitution in command");
			d = d.Replace (e.Groups ["selectpart"].Value, " " + b + " ");
			var f = d.LastIndexOf ("ORDER BY");
			if (f >= 0)
				d = d.Substring (0, f);
			return new QueryReturnInfo (d, this.Parameters, new List<Mapper> ());
		}
		public void PrepareCommand (GDASession a, IDbCommand b)
		{
			DataAccess c = null;
			if (a != null)
				c = new DataAccess (a.ProviderConfiguration);
			else
				c = new DataAccess ();
			c.PrepareCommand (a, b, this.CommandText, this.TakeCount > 0 || this.SkipCount > 0 ? new InfoPaging (this.SkipCount, this.TakeCount) {
				KeyFieldName = this.KeyFieldName
			} : null, this.Parameters.ToArray ());
		}
		public override QueryReturnInfo BuildResultInfo (string a)
		{
			return BuildResultInfo2 (null, a);
		}
		public override QueryReturnInfo BuildResultInfo<T> (GDA.Interfaces.IProviderConfiguration a)
		{
			if (string.IsNullOrEmpty (_commandText))
				throw new QueryException ("Command text not informed.");
			var b = _commandText;
			if (!string.IsNullOrEmpty (Order)) {
				var c = b.LastIndexOf ("ORDER BY");
				if (c >= 0)
					b = b.Substring (0, c);
				b += " ORDER BY " + Order;
			}
			var d = MappingManager.GetMappers<T> (null, null);
			var e = new List<Mapper> (d);
			if (!string.IsNullOrEmpty (_selectProperties) && _selectProperties != "*") {
				List<string> f = new List<string> ();
				Parser g = new Parser (new Lexer (_selectProperties));
				SelectPart h = g.ExecuteSelectPart ();
				e = new List<Mapper> (h.SelectionExpressions.Count);
				foreach (SelectExpression se in h.SelectionExpressions) {
					if (se.ColumnName.Type == GDA.Sql.InterpreterExpression.Enums.SqlExpressionType.Column) {
						Column j = se.Column;
						foreach (Mapper mp in d) {
							if (string.Compare (se.ColumnName.Value.Text, mp.PropertyMapperName, true) == 0 && (mp.Direction == DirectionParameter.Input || mp.Direction == DirectionParameter.InputOutput || mp.Direction == DirectionParameter.OutputOnlyInsert)) {
								if (!e.Exists (k => k.PropertyMapperName == mp.PropertyMapperName))
									e.Add (mp);
							}
						}
						if (j.Name == "*")
							throw new GDAException ("Invalid expression {0}", se.ColumnName.Value.Text);
					}
					else if (se.ColumnName.Type == GDA.Sql.InterpreterExpression.Enums.SqlExpressionType.Function)
						throw new QueryException ("NativeQuery not support function in select part");
				}
			}
			return new QueryReturnInfo (b, this.Parameters, e);
		}
		public static NativeQuery GetNamedQuery (string a)
		{
			GDA.GDASettings.LoadConfiguration ();
			if (string.IsNullOrEmpty (a))
				throw new ArgumentNullException ("queryName");
			var b = Mapping.MappingData.GetSqlQuery (a);
			if (b == null)
				throw new QueryException ("Query \"{0}\" not found.", a);
			if (!b.UseDatabaseSchema)
				throw new NotSupportedException ("Query not use database schema.");
			var c = new NativeQuery (b.Query);
			var d = new List<string> ();
			if (b.Return != null)
				foreach (var i in b.Return.ReturnProperties)
					d.Add (i.Name);
			foreach (var i in b.Parameters) {
				if (i.DefaultValue != null) {
					var e = Type.GetType (i.TypeName, false);
					if (e != null) {
						#if PocketPC
												#else
						var f = System.ComponentModel.TypeDescriptor.GetConverter (e);
						#endif
						try {
							#if PocketPC
														                             query.Add(i.Name, DataAccess.ConvertValue(i.DefaultValue, pType));
#else
							c.Add (i.Name, f.ConvertFrom (i.DefaultValue));
							#endif
						}
						catch (Exception ex) {
							throw new QueryException (string.Format ("Fail on convert parameter \"{0}\" to \"{1}\" in named query \"{2}\".", i.Name, e.FullName, a), ex);
						}
						continue;
					}
				}
				c.Add (i.Name, null);
			}
			return c;
		}
		void IGDAParameterContainer.Add (GDAParameter parameter)
		{
			if (parameter == null)
				throw new ArgumentNullException ("parameter");
			this._parameters.Add (parameter);
		}
		bool IGDAParameterContainer.TryGet (string a, out GDAParameter b)
		{
			return _parameters.TryGet (a, out b);
		}
		bool IGDAParameterContainer.ContainsKey (string a)
		{
			return _parameters.ContainsKey (a);
		}
		bool IGDAParameterContainer.Remove (string a)
		{
			return _parameters.Remove (a);
		}
		IEnumerator<GDAParameter> IEnumerable<GDAParameter>.GetEnumerator ()
		{
			return this._parameters.GetEnumerator ();
		}
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
		{
			return this._parameters.GetEnumerator ();
		}
	}
}
