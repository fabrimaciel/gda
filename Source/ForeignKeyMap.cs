using System;
using System.Collections.Generic;
using System.Text;
namespace GDA.Analysis
{
	public class ForeignKeyMap
	{
		private string _constraintName;
		private string _constraintSchema;
		private string _foreignKeyTable;
		private string _foreignKeyTableSchema;
		private string _foreignKeyColumn;
		private string _primaryKeyTable;
		private string _primaryKeyTableSchema;
		private string _primaryKeyColumn;
		public string ConstraintName {
			get {
				return _constraintName;
			}
			set {
				_constraintName = value;
			}
		}
		public string ConstraintSchema {
			get {
				return _constraintSchema;
			}
			set {
				_constraintSchema = value;
			}
		}
		public string ForeignKeyTable {
			get {
				return _foreignKeyTable;
			}
			set {
				_foreignKeyTable = value;
			}
		}
		public string ForeignKeyTableSchema {
			get {
				return _foreignKeyTableSchema;
			}
			set {
				_foreignKeyTableSchema = value;
			}
		}
		public string ForeignKeyColumn {
			get {
				return _foreignKeyColumn;
			}
			set {
				_foreignKeyColumn = value;
			}
		}
		public string PrimaryKeyTable {
			get {
				return _primaryKeyTable;
			}
			set {
				_primaryKeyTable = value;
			}
		}
		public string PrimaryKeyTableSchema {
			get {
				return _primaryKeyTableSchema;
			}
			set {
				_primaryKeyTableSchema = value;
			}
		}
		public string PrimaryKeyColumn {
			get {
				return _primaryKeyColumn;
			}
			set {
				_primaryKeyColumn = value;
			}
		}
		public override string ToString ()
		{
			return !string.IsNullOrEmpty (ConstraintSchema) ? string.Format ("[{0}].[{1}]", ConstraintSchema, ConstraintName) : string.Format ("[{0}]", ConstraintName);
		}
	}
}
