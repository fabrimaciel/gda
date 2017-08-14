using System;
using System.Collections.Generic;
using System.Text;
using GDA.Sql.InterpreterExpression;
namespace GDA.Sql
{
	public class ColumnInfo
	{
		private Expression _expression;
		private List<ColumnInfo> _columnExpressionsRef;
		private string _realTableNameOrTablesAlias;
		private string _oldTableNameOrTableAlias;
		private string _tableNameOrTableAlias;
		private string _columnName;
		private string _dbColumnName;
		private string _originalColumnName;
		public string TableNameOrTableAlias {
			get {
				return _tableNameOrTableAlias;
			}
			internal set {
				_tableNameOrTableAlias = value;
			}
		}
		public string ColumnName {
			get {
				return _columnName;
			}
		}
		public string DBColumnName {
			get {
				return _dbColumnName;
			}
			set {
				_dbColumnName = value;
			}
		}
		internal ColumnInfo (Expression a)
		{
			_expression = a;
			string[] b = a.Text.Split ('.');
			_columnName = b [b.Length - 1];
			_originalColumnName = _columnName;
			if (b.Length > 1) {
				_tableNameOrTableAlias = a.Text.Substring (0, a.Text.Length - (_columnName.Length + 1));
				_realTableNameOrTablesAlias = _tableNameOrTableAlias;
				if (_tableNameOrTableAlias [0] == '`' || _tableNameOrTableAlias [0] == '[')
					_tableNameOrTableAlias = _tableNameOrTableAlias.Substring (1, _tableNameOrTableAlias.Length - 2);
				_oldTableNameOrTableAlias = _tableNameOrTableAlias;
			}
			if (_columnName [0] == '`' || _columnName [0] == '[')
				_columnName = _columnName.Substring (1, _columnName.Length - 2);
		}
		internal void AddColumn (ColumnInfo a)
		{
			if (_columnExpressionsRef == null)
				_columnExpressionsRef = new List<ColumnInfo> ();
			_columnExpressionsRef.Add (a);
		}
		internal void Rename (string a)
		{
			if (a == null)
				throw new ArgumentNullException ("newColumnName");
			if (_tableNameOrTableAlias != _oldTableNameOrTableAlias) {
				if (_realTableNameOrTablesAlias != null) {
					string b = _realTableNameOrTablesAlias.Replace (_oldTableNameOrTableAlias, _tableNameOrTableAlias);
					_expression.Text = _expression.Text.Replace (_realTableNameOrTablesAlias + ".", b + ".");
					_realTableNameOrTablesAlias = b;
				}
				else {
					_expression.Text = _tableNameOrTableAlias + "." + _expression.Text;
					_realTableNameOrTablesAlias = _tableNameOrTableAlias;
				}
				_oldTableNameOrTableAlias = _tableNameOrTableAlias;
			}
			_expression.Text = _expression.Text.Replace (_originalColumnName, a);
			_originalColumnName = a;
			_columnName = a;
			if (_columnExpressionsRef != null)
				foreach (ColumnInfo col in _columnExpressionsRef) {
					col.TableNameOrTableAlias = this.TableNameOrTableAlias;
					col.Rename (a);
				}
		}
		internal void RenameToMapper (GDA.Interfaces.IProvider a)
		{
			if (!string.IsNullOrEmpty (DBColumnName))
				Rename (a != null ? a.QuoteExpression (DBColumnName) : DBColumnName);
		}
		public override string ToString ()
		{
			return (_tableNameOrTableAlias != null ? _tableNameOrTableAlias + "." : "") + _columnName;
		}
	}
}
