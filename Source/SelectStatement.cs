using System;
using System.Collections.Generic;
using System.Text;
using GDA.Sql.InterpreterExpression;
using GDA.Sql.InterpreterExpression.Nodes;
using System.Reflection;
using System.Data;
using GDA.Caching;
using GDA.Collections;
namespace GDA.Sql
{
	public class SelectStatement : BaseQuery
	{
		private CommandType _commandType = CommandType.Text;
		private int _commandTimeout = GDASession.DefaultCommandTimeout;
		private int _steps = 0;
		private readonly Parser _parser;
		private Dictionary<string, TableInfo> _tablesInfo = new Dictionary<string, TableInfo> ();
		private Dictionary<string, ColumnInfo> _columnsInfo = new Dictionary<string, ColumnInfo> ();
		private List<TableInfo> _tablesInfoList;
		private List<ColumnInfo> _columnsInfoList;
		private List<VariableInfo> _variablesInfo = new List<VariableInfo> ();
		private GDAParameterCollection parameters = new GDAParameterCollection ();
		private TableInfo _firstTable = null;
		private ISelectStatementReferences _references;
		private string _keyFieldName;
		public List<TableInfo> TablesInfo {
			get {
				return _tablesInfoList;
			}
		}
		internal List<ColumnInfo> ColumnsInfo {
			get {
				return _columnsInfoList;
			}
		}
		internal List<VariableInfo> VariablesInfo {
			get {
				return _variablesInfo;
			}
		}
		public string Command {
			get {
				return _parser.Lex.Command;
			}
		}
		internal int Steps {
			get {
				return _steps;
			}
			set {
				_steps = value;
			}
		}
		public List<GDAParameter> Parameters {
			get {
				return parameters;
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
				if (string.IsNullOrEmpty (_keyFieldName)) {
					TableInfo a = null;
					string b = null;
					foreach (var i in TablesInfo) {
						b = _references.GetFirstKeyPropertyMapping (i.TypeInfo);
						if (!string.IsNullOrEmpty (b)) {
							a = i;
							break;
						}
					}
					if (b != null)
						_keyFieldName = (!string.IsNullOrEmpty (a.TableAlias) ? a.TableAlias + "." : null) + _references.GetPropertyMapping (a.TypeInfo, b);
					else {
						foreach (var i in TablesInfo) {
							foreach (var j in i.Columns) {
								_keyFieldName = (!string.IsNullOrEmpty (i.TableAlias) ? i.TableAlias + "." : null) + _references.GetPropertyMapping (i.TypeInfo, j.ColumnName);
								break;
							}
							if (_keyFieldName != null)
								break;
						}
						if (_keyFieldName == null)
							foreach (var i in TablesInfo) {
								foreach (var j in _references.GetPropertiesMapping (i.TypeInfo)) {
									_keyFieldName = (!string.IsNullOrEmpty (i.TableAlias) ? i.TableAlias + "." : null) + j.Column;
									break;
								}
								if (_keyFieldName != null)
									break;
							}
					}
				}
				return _keyFieldName;
			}
			set {
				_keyFieldName = value;
			}
		}
		internal SelectStatement (ISelectStatementReferences a, Parser b)
		{
			if (a == null)
				throw new ArgumentNullException ("references");
			_references = a;
			_parser = b;
			foreach (Select select in b.SelectParts) {
				GetSelectInfo (select);
			}
			foreach (KeyValuePair<string, ColumnInfo> ci in _columnsInfo) {
				if (ci.Value.TableNameOrTableAlias == null) {
					bool c = false;
					foreach (KeyValuePair<string, TableInfo> ti in _tablesInfo) {
						if (ti.Value.ExistsColumn (ci.Value)) {
							if (!c) {
								ti.Value.AddColumn (ci.Value);
								c = true;
							}
							else {
								_firstTable.AddColumn (ci.Value);
							}
						}
					}
				}
				else {
					foreach (KeyValuePair<string, TableInfo> ti in _tablesInfo) {
						if (ci.Value.TableNameOrTableAlias == ti.Value.TableName.Name || ci.Value.TableNameOrTableAlias == ti.Value.TableAlias) {
							ti.Value.AddColumn (ci.Value);
							break;
						}
					}
				}
			}
			_tablesInfoList = new List<TableInfo> (_tablesInfo.Values);
			_columnsInfoList = new List<ColumnInfo> (_columnsInfo.Values);
		}
		internal SelectStatement (Parser a) : this (NativeSelectStatementReferences.Instance, a)
		{
		}
		internal SelectStatement (ISelectStatementReferences a, WherePart b)
		{
			if (a == null)
				throw new ArgumentNullException ("references");
			_references = a;
			foreach (SqlExpression se in b.Expressions)
				ColumnName (se);
			_columnsInfoList = new List<ColumnInfo> (_columnsInfo.Values);
		}
		internal SelectStatement (WherePart a) : this (NativeSelectStatementReferences.Instance, a)
		{
		}
		internal SelectStatement (ISelectStatementReferences a, OrderByPart b)
		{
			if (a == null)
				throw new ArgumentNullException ("references");
			_references = a;
			foreach (OrderByExpression oe in b.OrderByExpressions)
				ColumnName (oe.Expression);
			_columnsInfoList = new List<ColumnInfo> (_columnsInfo.Values);
		}
		internal SelectStatement (OrderByPart a) : this (NativeSelectStatementReferences.Instance, a)
		{
		}
		internal SelectStatement (ISelectStatementReferences a, GroupByPart b)
		{
			if (a == null)
				throw new ArgumentNullException ("references");
			_references = a;
			foreach (SqlExpression sqle in b.Expressions)
				ColumnName (sqle);
			_columnsInfoList = new List<ColumnInfo> (_columnsInfo.Values);
		}
		internal SelectStatement (GroupByPart a) : this (NativeSelectStatementReferences.Instance, a)
		{
		}
		private void GetSelectInfo (Select a)
		{
			Steps++;
			foreach (SelectExpression se in a.SelectPart.SelectionExpressions) {
				ColumnName (se.ColumnName);
			}
			if (a.FromPart != null) {
				Steps++;
				foreach (TableExpression te in a.FromPart.TableExpressions) {
					if (te.SelectInfo != null) {
						GetSelectInfo (te.SelectInfo);
					}
					else {
						Steps++;
						TableInfo b = new TableInfo (_references, te.TableName, te.TableAlias);
						if (_firstTable == null)
							_firstTable = b;
						if (!_tablesInfo.ContainsKey (b.ToString ())) {
							_tablesInfo.Add (b.ToString (), b);
						}
					}
					if (te.OnExpressions != null) {
						Steps++;
						foreach (SqlExpression se in te.OnExpressions.Expressions)
							ColumnName (se);
					}
				}
			}
			if (a.WherePart != null) {
				Steps++;
				foreach (SqlExpression se in a.WherePart.Expressions)
					ColumnName (se);
			}
			if (a.GroupByPart != null) {
				Steps++;
				foreach (SqlExpression se in a.GroupByPart.Expressions)
					ColumnName (se);
			}
			if (a.HavingPart != null) {
				Steps++;
				foreach (SqlExpression se in a.HavingPart.Expressions)
					ColumnName (se);
			}
			if (a.OrderByPart != null) {
				Steps++;
				foreach (OrderByExpression oe in a.OrderByPart.OrderByExpressions)
					ColumnName (oe.Expression);
			}
		}
		private void ColumnName (SqlExpression a)
		{
			Steps++;
			if (a is ContainerSqlExpression) {
				foreach (SqlExpression se1 in ((ContainerSqlExpression)a).Expressions)
					ColumnName (se1);
			}
			else if (a is Select) {
				GetSelectInfo ((Select)a);
			}
			else if (a.Type == GDA.Sql.InterpreterExpression.Enums.SqlExpressionType.Column)
				AddColumnInfo (a);
			else if (a.Type == GDA.Sql.InterpreterExpression.Enums.SqlExpressionType.Variable)
				AddVariableInfo (a);
			else if (a is SqlFunction) {
				foreach (List<SqlExpression> parameter in ((SqlFunction)a).Parameters) {
					foreach (SqlExpression pSe in parameter) {
						ColumnName (pSe);
					}
				}
			}
			else if (a.Type == GDA.Sql.InterpreterExpression.Enums.SqlExpressionType.Boolean) {
				switch (a.Value.Token) {
				case GDA.Sql.InterpreterExpression.Enums.TokenID.kAnd:
					if (a.Value.Text == "&&")
						a.Value.Text = "AND";
					break;
				case GDA.Sql.InterpreterExpression.Enums.TokenID.kOr:
					if (a.Value.Text == "||")
						a.Value.Text = "OR";
					break;
				}
			}
		}
		private void AddColumnInfo (SqlExpression a)
		{
			ColumnInfo b = new ColumnInfo (a.Value);
			string c = b.ToString ().ToLower ();
			if (_columnsInfo.ContainsKey (c)) {
				_columnsInfo [c].AddColumn (b);
			}
			else {
				_columnsInfo.Add (c, b);
			}
		}
		private void AddVariableInfo (SqlExpression a)
		{
			var b = new VariableInfo (a.Value);
			var c = _variablesInfo.BinarySearch (b, VariableInfo.VariableInfoComparer.Instance);
			if (c < 0)
				_variablesInfo.Insert (~c, b);
		}
		private void ProcessVariablesInfo (IGDAParameterContainer a)
		{
			foreach (var variableInfo in VariablesInfo) {
			}
		}
		public string Parser ()
		{
			return Parser2 (null);
		}
		public string Parser (GDA.Interfaces.IProvider a)
		{
			return Parser2 (a, null);
		}
		public string Parser2 (IGDAParameterContainer a)
		{
			var b = GDASettings.DefaultProviderConfiguration;
			var c = b != null ? b.Provider : null;
			foreach (TableInfo ti in TablesInfo) {
				ti.RenameToMapper ();
				foreach (ColumnInfo ci in ti.Columns)
					ci.RenameToMapper (c);
			}
			foreach (var variableInfo in VariablesInfo)
				variableInfo.Replace (c, a, null);
			return (new ParserToSqlCommand (this._parser)).SqlCommand;
		}
		public string Parser2 (GDA.Interfaces.IProvider a, IGDAParameterContainer b)
		{
			foreach (TableInfo ti in TablesInfo) {
				ti.RenameToMapper ();
				foreach (ColumnInfo ci in ti.Columns)
					ci.RenameToMapper (a);
			}
			foreach (var variableInfo in VariablesInfo)
				variableInfo.Replace (a, b, null);
			return (new ParserToSqlCommand (this._parser, a.QuoteExpressionBegin, a.QuoteExpressionEnd)).SqlCommand;
		}
		public static bool operator == (SelectStatement a, SelectStatement b) {
			return a.Steps == b.Steps;
		}
		public static bool operator != (SelectStatement a, SelectStatement b) {
			return a.Steps != b.Steps;
		}
		public SelectStatement Add (GDAParameter a)
		{
			if (a != null) {
				var b = this.parameters.FindIndex (c => c.ParameterName == a.ParameterName);
				if (b >= 0)
					this.parameters.RemoveAt (b);
				this.parameters.Add (a);
			}
			return this;
		}
		public SelectStatement Add (params GDAParameter[] a)
		{
			this.parameters.AddRange (a);
			return this;
		}
		public SelectStatement Add (IEnumerable<GDAParameter> a)
		{
			foreach (var i in a) {
				var b = this.parameters.FindIndex (c => c.ParameterName == i.ParameterName);
				if (b >= 0)
					this.parameters.RemoveAt (b);
				this.parameters.Add (i);
			}
			return this;
		}
		public SelectStatement Add (DbType a, object b)
		{
			return Add ("", a, b);
		}
		public SelectStatement Add (string a, DbType b, object c)
		{
			return Add (a, b, 0, c);
		}
		public SelectStatement Add (string a, object b)
		{
			return Add (new GDAParameter (a, b));
		}
		public SelectStatement Add (DbType a, int b, object c)
		{
			return Add ("", a, b, c);
		}
		public SelectStatement Add (string a, DbType b, int c, object d)
		{
			GDAParameter e = new GDAParameter ();
			e.ParameterName = a;
			e.DbType = b;
			e.Size = c;
			e.Value = d;
			var f = this.parameters.FindIndex (g => g.ParameterName == e.ParameterName);
			if (f >= 0)
				this.parameters.RemoveAt (f);
			this.parameters.Add (e);
			return this;
		}
		public SelectStatement SetCommandType (CommandType a)
		{
			_commandType = a;
			return this;
		}
		public SelectStatement SetCommandTimeout (int a)
		{
			_commandTimeout = a;
			return this;
		}
		public SelectStatement Skip (int a)
		{
			_skipCount = a;
			return this;
		}
		public SelectStatement Take (int a)
		{
			_takeCount = a;
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
			return new DataAccess ().LoadResult (null, this.CommandType, this.CommandTimeout, Parser2 (GDA.GDASettings.DefaultProviderConfiguration.Provider, parameters), this.TakeCount > 0 || this.SkipCount > 0 ? new InfoPaging (this.SkipCount, this.TakeCount) {
				KeyFieldName = this.KeyFieldName
			} : null, this.Parameters.ToArray ());
		}
		public GDADataRecordCursor ToDataRecords (GDASession a)
		{
			return new DataAccess ().LoadResult (a, this.CommandType, this.CommandTimeout, Parser2 (a.ProviderConfiguration.Provider, parameters), this.TakeCount > 0 || this.SkipCount > 0 ? new InfoPaging (this.SkipCount, this.TakeCount) {
				KeyFieldName = this.KeyFieldName
			} : null, this.Parameters.ToArray ());
		}
		public override QueryReturnInfo BuildResultInfo<T> (GDA.Interfaces.IProviderConfiguration a)
		{
			return new QueryReturnInfo (Parser2 (a.Provider, parameters), parameters, MappingManager.GetMappers<T> (null, null));
		}
		public override QueryReturnInfo BuildResultInfo (string a)
		{
			return BuildResultInfo2 (GDA.GDASettings.DefaultProviderConfiguration.Provider, a);
		}
		public override QueryReturnInfo BuildResultInfo2 (GDA.Interfaces.IProvider a, string b)
		{
			return BuildResultInfo2 (a, b, new Dictionary<string, Type> ());
		}
		public override QueryReturnInfo BuildResultInfo2 (GDA.Interfaces.IProvider a, string b, Dictionary<string, Type> c)
		{
			var d = Parser2 (a, parameters);
			d = d.TrimStart (' ', '\r', '\n', '\t');
			if (d.IndexOf ("SELECT ", StringComparison.InvariantCultureIgnoreCase) == 0) {
				var e = d.IndexOf (" FROM ");
				if (e > 0)
					d = d.Substring (0, "SELECT ".Length) + b + d.Substring (e);
			}
			var f = d.LastIndexOf ("ORDER BY");
			if (f >= 0)
				d = d.Substring (0, f);
			return new QueryReturnInfo (d, parameters, new List<Mapper> ());
		}
		public static SelectStatement GetNamedQuery (string a)
		{
			if (string.IsNullOrEmpty (a))
				throw new ArgumentNullException ("queryName");
			var b = Mapping.MappingData.GetSqlQuery (a);
			if (b == null)
				throw new QueryException ("Query \"{0}\" not found.", a);
			if (b.UseDatabaseSchema)
				throw new NotSupportedException ("Query use database schema.");
			var c = SqlBuilder.P (b.Query);
			var d = new List<string> ();
			if (b.Return != null)
				foreach (var i in b.Return.ReturnProperties)
					d.Add (i.Name);
			foreach (var i in b.Parameters) {
				if (i.DefaultValue != null) {
					var e = Type.GetType (i.TypeName, false);
					if (e != null) {
						#if !PocketPC
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
	}
}
