using System;
using System.Collections.Generic;
using System.Text;
using GDA.Analysis;
using System.Data;
using System.Data.OleDb;
namespace GDA.Provider.MsAccess
{
	public class MsAccessAnalyzer : DatabaseAnalyzer
	{
		public MsAccessAnalyzer (MsAccessProviderConfiguration a) : base (a)
		{
		}
		private DataTable GetTables (string a)
		{
			OleDbConnection b = ProviderConfiguration.CreateConnection () as OleDbConnection;
			GDAConnectionManager.NotifyConnectionCreated (b);
			if (b.State != ConnectionState.Open) {
				b.Open ();
				GDAConnectionManager.NotifyConnectionOpened (b);
			}
			try {
				return b.GetOleDbSchemaTable (OleDbSchemaGuid.Tables, new object[] {
					null,
					null,
					a,
					null
				});
			}
			finally {
				b.Close ();
			}
		}
		private DataTable GetColumns (string a)
		{
			OleDbConnection b = ProviderConfiguration.CreateConnection () as OleDbConnection;
			GDAConnectionManager.NotifyConnectionCreated (b);
			if (b.State != ConnectionState.Open) {
				b.Open ();
				GDAConnectionManager.NotifyConnectionOpened (b);
			}
			try {
				return b.GetOleDbSchemaTable (OleDbSchemaGuid.Columns, new object[] {
					null,
					null,
					a,
					null
				});
			}
			finally {
				b.Close ();
			}
		}
		private DataTable GetPrimaryKeys (string a)
		{
			OleDbConnection b = ProviderConfiguration.CreateConnection () as OleDbConnection;
			GDAConnectionManager.NotifyConnectionCreated (b);
			if (b.State != ConnectionState.Open) {
				b.Open ();
				GDAConnectionManager.NotifyConnectionOpened (b);
			}
			try {
				return b.GetOleDbSchemaTable (OleDbSchemaGuid.Primary_Keys, new object[] {
					null,
					null,
					a
				});
			}
			finally {
				b.Close ();
			}
		}
		private DataTable GetPrimaryKeyInfo (string a)
		{
			OleDbConnection b = ProviderConfiguration.CreateConnection () as OleDbConnection;
			GDAConnectionManager.NotifyConnectionCreated (b);
			if (b.State != ConnectionState.Open) {
				b.Open ();
				GDAConnectionManager.NotifyConnectionOpened (b);
			}
			try {
				OleDbCommand c = new OleDbCommand ("SELECT * FROM " + a, b);
				OleDbDataReader d = c.ExecuteReader (CommandBehavior.KeyInfo);
				return d.GetSchemaTable ();
			}
			finally {
				b.Close ();
			}
		}
		private DataTable GetForeignKeys (string a)
		{
			OleDbConnection b = ProviderConfiguration.CreateConnection () as OleDbConnection;
			GDAConnectionManager.NotifyConnectionCreated (b);
			if (b.State != ConnectionState.Open) {
				b.Open ();
				GDAConnectionManager.NotifyConnectionOpened (b);
			}
			try {
				return b.GetOleDbSchemaTable (OleDbSchemaGuid.Foreign_Keys, new object[] {
					null,
					null,
					null,
					null,
					null,
					a
				});
			}
			finally {
				b.Close ();
			}
		}
		public override void Analyze (string a)
		{
			try {
				bool b = a != null;
				DataTable c = GetTables (a);
				foreach (DataRow row in c.Rows) {
					string d = (string)row ["TABLE_NAME"];
					if (!d.StartsWith ("MSysAccess")) {
						if (!b || a.ToLower ().Equals (d.ToLower ())) {
							TableMap e = GetTableMap (d);
							if (e == null) {
								e = new TableMap (ProviderConfiguration, d);
								tablesMaps [d.ToLower ()] = e;
							}
							GetColumnData (e);
							if (b)
								break;
						}
					}
				}
				GetPrimaryKeyData ();
				GetForeignKeyData ();
			}
			catch (Exception ex) {
				throw new GDAException ("An error occurred while analyzing the database schema.", ex);
			}
		}
		[Flags]
		private enum DBCOLUMNFLAGS
		{
			ISBOOKMARK = 0x1,
			MAYDEFER = 0x2,
			WRITE = 0x4,
			WRITEUNKNOWN = 0x8,
			ISFIXEDLENGTH = 0x10,
			ISNULLABLE = 0x20,
			MAYBENULL = 0x40,
			ISLONG = 0x80,
			ISROWID = 0x100,
			ISROWVER = 0x200,
			CACHEDEFERRED = 0x1000,
			SCALEISNEGATIVE = 0x4000,
			RESERVED = 0x8000,
			ISROWURL = 0x10000,
			ISDEFAULTSTREAM = 0x20000,
			ISCOLLECTION = 0x40000,
			ISSTREAM = 0x80000,
			ISROWSET = 0x100000,
			ISROW = 0x200000,
			ROWSPECIFICCOLUMN = 0x400000
		}
		private void GetColumnData (TableMap a)
		{
			DataTable b = GetColumns (a.TableName);
			foreach (DataRow row in b.Rows) {
				string c = (string)row ["COLUMN_NAME"];
				FieldMap d = a.GetFieldMapFromColumn (c);
				if (d == null) {
					d = new FieldMap (a, c);
					a.Fields.Add (d);
				}
				d.IsNullable = Convert.ToBoolean (row ["IS_NULLABLE"]);
				OleDbType e = (OleDbType)row ["DATA_TYPE"];
				d.SetDbType ((long)e);
				d.DbTypeName = e.ToString ();
				if (e == OleDbType.Decimal || e == OleDbType.Numeric || e == OleDbType.VarNumeric) {
					d.Size = Convert.ToInt32 (row ["NUMERIC_PRECISION"]);
				}
				else if (e == OleDbType.LongVarBinary || e == OleDbType.LongVarChar || e == OleDbType.LongVarWChar || e == OleDbType.VarBinary || e == OleDbType.VarChar || e == OleDbType.VarWChar || e == OleDbType.WChar || e == OleDbType.Char || e == OleDbType.BSTR || e == OleDbType.Binary) {
					d.Size = Convert.ToInt32 (row ["CHARACTER_MAXIMUM_LENGTH"]);
				}
				int f = Convert.ToInt32 (row ["COLUMN_FLAGS"]);
				int g = (int)DBCOLUMNFLAGS.ISNULLABLE + (int)DBCOLUMNFLAGS.MAYBENULL;
				bool h = (f & g) != 0;
				g = (int)DBCOLUMNFLAGS.WRITE + (int)DBCOLUMNFLAGS.WRITEUNKNOWN;
				bool i = (f & g) == 0;
				d.IsReadOnly = i;
				if (row ["DESCRIPTION"] != DBNull.Value && row ["DESCRIPTION"] is string)
					d.Comment = row ["DESCRIPTION"].ToString ();
			}
		}
		private void GetPrimaryKeyData ()
		{
			DataTable a = GetPrimaryKeys (null);
			foreach (DataRow row in a.Rows) {
				string b = (string)row ["TABLE_NAME"];
				string c = (string)row ["COLUMN_NAME"];
				TableMap d = GetTableMap (b);
				if (d != null) {
					FieldMap e = d.GetFieldMapFromColumn (c);
					if (e != null) {
						e.IsPrimaryKey = true;
						DataTable f = GetPrimaryKeyInfo (d.QuotedTableName);
						foreach (DataRow dr in f.Rows) {
							string g = dr ["ColumnName"].ToString ();
							if (g == c) {
								bool h = Convert.ToBoolean (dr ["IsAutoIncrement"]);
								e.IsAutoGenerated = h;
							}
						}
					}
				}
			}
		}
		private void GetForeignKeyData ()
		{
			DataTable a = GetForeignKeys (null);
			foreach (DataRow row in a.Rows) {
				string b = (string)row ["FK_TABLE_NAME"];
				string c = (string)row ["FK_COLUMN_NAME"];
				string d = (string)row ["PK_TABLE_NAME"];
				string e = (string)row ["PK_COLUMN_NAME"];
				TableMap f = GetTableMap (b);
				if (f != null) {
					FieldMap g = f.GetFieldMapFromColumn (c);
					if (g != null) {
						g.ForeignKeyTableName = d;
						g.ForeignKeyColumnName = e;
					}
				}
			}
		}
	}
}
