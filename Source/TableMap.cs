using System;
using System.Collections.Generic;
using System.Text;
using GDA.Interfaces;
namespace GDA.Analysis
{
	public class TableMap
	{
		private IProviderConfiguration _provider;
		private FieldList _fields;
		private string _tableName = null;
		private string _tableSchema = null;
		private FieldMap _identityMap = null;
		private int _tableId;
		private bool _isView;
		private string _comment;
		public IProviderConfiguration ProviderConfiguration {
			get {
				return _provider;
			}
		}
		public FieldList Fields {
			get {
				return _fields;
			}
		}
		public string TableName {
			get {
				return _tableName;
			}
			set {
				_tableName = value;
			}
		}
		public string TableSchema {
			get {
				return _tableSchema;
			}
			set {
				_tableSchema = value;
			}
		}
		public string QuotedTableName {
			get {
				return ProviderConfiguration.Provider.QuoteExpression (_tableName);
			}
		}
		public FieldMap IdentityMap {
			get {
				return _identityMap;
			}
			set {
				_identityMap = value;
			}
		}
		public int TableId {
			get {
				return _tableId;
			}
			set {
				_tableId = value;
			}
		}
		public bool IsView {
			get {
				return _isView;
			}
			set {
				_isView = value;
			}
		}
		public string Comment {
			get {
				return _comment;
			}
			set {
				_comment = value;
			}
		}
		public TableMap (IProviderConfiguration a, string b)
		{
			_tableName = b;
			_provider = a;
			_fields = new FieldList ();
		}
		public TableMap (IProviderConfiguration a, string b, string c) : this (a, b)
		{
			_tableSchema = c;
		}
		public FieldMap GetFieldMapFromColumn (string a)
		{
			return _fields.FindColumn (a);
		}
		public override string ToString ()
		{
			return TableName;
		}
	}
}
