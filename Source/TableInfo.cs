using System;
using System.Collections.Generic;
using System.Text;
using GDA.Sql.InterpreterExpression;
using System.Reflection;
using GDA.Caching;
namespace GDA.Sql
{
	public class TableInfo
	{
		private InterpreterExpression.Nodes.TableNameExpression _tableNameExpression;
		private Expression _tableAlias;
		private TableName _tableName;
		private List<ColumnInfo> _columns = new List<ColumnInfo> ();
		private Type _classTypeRelated;
		private ISelectStatementReferences _references;
		private Mapping.TypeInfo _typeInfo;
		public Type ClassTypeRelated {
			get {
				return _classTypeRelated;
			}
			set {
				_classTypeRelated = value;
			}
		}
		public List<ColumnInfo> Columns {
			get {
				return _columns;
			}
		}
		public TableName TableName {
			get {
				return _tableName;
			}
		}
		public string TableAlias {
			get {
				if (_tableAlias != null)
					return _tableAlias.Text;
				else
					return null;
			}
		}
		public Mapping.TypeInfo TypeInfo {
			get {
				if (_typeInfo == null)
					_typeInfo = _references.GetTypeInfo (this);
				return _typeInfo;
			}
		}
		internal TableInfo (ISelectStatementReferences a, InterpreterExpression.Nodes.TableNameExpression b, Expression c)
		{
			if (a == null)
				throw new ArgumentNullException ("references");
			_references = a;
			_tableNameExpression = b;
			_tableName = new TableName {
				Name = b.Name,
				Schema = b.Schema
			};
			_tableAlias = c;
		}
		internal bool ExistsColumn (string a)
		{
			return !string.IsNullOrEmpty (_references.GetPropertyMapping (TypeInfo, a));
		}
		internal bool ExistsColumn (ColumnInfo a)
		{
			if (!string.IsNullOrEmpty (a.TableNameOrTableAlias) && (string.Compare (a.TableNameOrTableAlias, this.TableAlias, true) != 0 || string.Compare (a.TableNameOrTableAlias, this.TableName.Name, true) != 0)) {
				return false;
			}
			return ExistsColumn (a.ColumnName);
		}
		internal void AddColumn (ColumnInfo a)
		{
			a.DBColumnName = _references.GetPropertyMapping (TypeInfo, a.ColumnName);
			_columns.Add (a);
		}
		internal void RenameToMapper ()
		{
			var a = _references.GetTableName (TypeInfo);
			if (a == null)
				throw new GDAException ("Table name not found to type \"" + TypeInfo.FullnameWithAssembly + "\"");
			_tableName = a;
			_tableNameExpression.Name = a.Name;
			_tableNameExpression.Schema = a.Schema;
		}
		public override string ToString ()
		{
			return TableName.ToString () + (_tableAlias != null ? " AS " + _tableAlias.Text : "");
		}
	}
}
