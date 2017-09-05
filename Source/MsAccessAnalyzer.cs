/* 
 * GDA - Generics Data Access, is framework to object-relational mapping 
 * (a programming technique for converting data between incompatible 
 * type systems in databases and Object-oriented programming languages) using c#.
 * 
 * Copyright (C) 2010  <http://www.colosoft.com.br/gda> - support@colosoft.com.br
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

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
		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="providerConfiguration"></param>
		public MsAccessAnalyzer(MsAccessProviderConfiguration providerConfiguration) : base(providerConfiguration)
		{
		}

		/// <summary>
		/// Recupera a estrutura da tabela.
		/// http://msdn.microsoft.com/library/en-us/oledb/htm/oledbtables_rowset.asp
		/// Restriction columns: TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME, TABLE_TYPE ("TABLE","VIEW")
		/// Schema columns: TABLE_GUID, DESCRIPTION, TABLE_PROPID, DATE_CREATED, DATE_MODIFIED
		/// </summary>
		private DataTable GetTables(string tableName)
		{
			OleDbConnection conn = ProviderConfiguration.CreateConnection() as OleDbConnection;
			GDAConnectionManager.NotifyConnectionCreated(conn);
			if(conn.State != ConnectionState.Open)
			{
				conn.Open();
				GDAConnectionManager.NotifyConnectionOpened(conn);
			}
			try
			{
				return conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] {
					null,
					null,
					tableName,
					null
				});
			}
			finally
			{
				conn.Close();
			}
		}

		/// <summary>
		/// Recupera as informações das colunas da tabela.
		/// http://msdn.microsoft.com/library/en-us/oledb/htm/oledbcolumns_rowset.asp
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/oledb/htm/oledbtype_indicators.asp
		/// Restriction columns: TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME, COLUMN_NAME
		/// Schema columns: DATA_TYPE, ORDINAL_POSITION, COLUMN_HASDEFAULT, COLUMN_DEFAULT, 
		///		COLUMN_FLAGS, IS_NULLABLE, NUMERIC_PRECISION, NUMERIC_SCALE, 
		///		CHARACTER_MAXIMUM_LENGTH, CHARACTER_OCTET_LENGTH
		/// </summary>
		private DataTable GetColumns(string tableName)
		{
			OleDbConnection conn = ProviderConfiguration.CreateConnection() as OleDbConnection;
			GDAConnectionManager.NotifyConnectionCreated(conn);
			if(conn.State != ConnectionState.Open)
			{
				conn.Open();
				GDAConnectionManager.NotifyConnectionOpened(conn);
			}
			try
			{
				return conn.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, new object[] {
					null,
					null,
					tableName,
					null
				});
			}
			finally
			{
				conn.Close();
			}
		}

		/// <summary>
		/// http://msdn.microsoft.com/library/en-us/oledb/htm/oledbprimary_keys_rowset.asp
		/// Restriction columns: TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME
		/// Schema columns: COLUMN_NAME, COLUMN_GUID, COLUMN_PROPID, ORDINAL, PK_NAME
		/// </summary>
		private DataTable GetPrimaryKeys(string tableName)
		{
			OleDbConnection conn = ProviderConfiguration.CreateConnection() as OleDbConnection;
			GDAConnectionManager.NotifyConnectionCreated(conn);
			if(conn.State != ConnectionState.Open)
			{
				conn.Open();
				GDAConnectionManager.NotifyConnectionOpened(conn);
			}
			try
			{
				return conn.GetOleDbSchemaTable(OleDbSchemaGuid.Primary_Keys, new object[] {
					null,
					null,
					tableName
				});
			}
			finally
			{
				conn.Close();
			}
		}

		/// <summary>
		/// Recupera as informações sobre as chaves primárias da tabela.
		/// </summary>
		private DataTable GetPrimaryKeyInfo(string tableName)
		{
			OleDbConnection conn = ProviderConfiguration.CreateConnection() as OleDbConnection;
			GDAConnectionManager.NotifyConnectionCreated(conn);
			if(conn.State != ConnectionState.Open)
			{
				conn.Open();
				GDAConnectionManager.NotifyConnectionOpened(conn);
			}
			try
			{
				OleDbCommand cmd = new OleDbCommand("SELECT * FROM " + tableName, conn);
				OleDbDataReader dr = cmd.ExecuteReader(CommandBehavior.KeyInfo);
				return dr.GetSchemaTable();
			}
			finally
			{
				conn.Close();
			}
		}

		/// <summary>
		/// Recupera as chaves estrangeiras da tabela.
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/oledb/htm/oledbtable_constraints_rowset.asp
		/// Restriction columns: PK_TABLE_CATALOG, PK_TABLE_SCHEMA, PK_TABLE_NAME, 
		/// FK_TABLE_CATALOG, FK_TABLE_SCHEMA, FK_TABLE_NAME
		/// Schema columns: FK_COLUMN_NAME, FK_COLUMN_GUID, FK_COLUMN_PROPID, UPDATE_RULE,
		/// DELETE_RULE, PK_NAME, FK_NAME, DEFERRABILITY 
		/// </summary>
		private DataTable GetForeignKeys(string tableName)
		{
			OleDbConnection conn = ProviderConfiguration.CreateConnection() as OleDbConnection;
			GDAConnectionManager.NotifyConnectionCreated(conn);
			if(conn.State != ConnectionState.Open)
			{
				conn.Open();
				GDAConnectionManager.NotifyConnectionOpened(conn);
			}
			try
			{
				return conn.GetOleDbSchemaTable(OleDbSchemaGuid.Foreign_Keys, new object[] {
					null,
					null,
					null,
					null,
					null,
					tableName
				});
			}
			finally
			{
				conn.Close();
			}
		}

		public override void Analyze(string tableName)
		{
			try
			{
				bool isSingleRun = tableName != null;
				DataTable dt = GetTables(tableName);
				foreach (DataRow row in dt.Rows)
				{
					string dbTableName = (string)row["TABLE_NAME"];
					if(!dbTableName.StartsWith("MSysAccess"))
					{
						if(!isSingleRun || tableName.ToLower().Equals(dbTableName.ToLower()))
						{
							TableMap map = GetTableMap(dbTableName);
							if(map == null)
							{
								map = new TableMap(ProviderConfiguration, dbTableName);
								tablesMaps[dbTableName.ToLower()] = map;
							}
							GetColumnData(map);
							if(isSingleRun)
								break;
						}
					}
				}
				GetPrimaryKeyData();
				GetForeignKeyData();
			}
			catch(Exception ex)
			{
				throw new GDAException("An error occurred while analyzing the database schema.", ex);
			}
		}

		/// <summary>
		/// This enumeration represents the bitmask values of the COLUMN_FLAGS value used below.
		/// </summary>
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

		/// <summary>
		/// Este método preenche os dados das colunas da table.
		/// </summary>
		private void GetColumnData(TableMap map)
		{
			DataTable dt = GetColumns(map.TableName);
			foreach (DataRow row in dt.Rows)
			{
				string columnName = (string)row["COLUMN_NAME"];
				FieldMap fm = map.GetFieldMapFromColumn(columnName);
				if(fm == null)
				{
					fm = new FieldMap(map, columnName);
					map.Fields.Add(fm);
				}
				fm.IsNullable = Convert.ToBoolean(row["IS_NULLABLE"]);
				OleDbType dbType = (OleDbType)row["DATA_TYPE"];
				fm.SetDbType((long)dbType);
				fm.DbTypeName = dbType.ToString();
				if(dbType == OleDbType.Decimal || dbType == OleDbType.Numeric || dbType == OleDbType.VarNumeric)
				{
					fm.Size = Convert.ToInt32(row["NUMERIC_PRECISION"]);
				}
				else if(dbType == OleDbType.LongVarBinary || dbType == OleDbType.LongVarChar || dbType == OleDbType.LongVarWChar || dbType == OleDbType.VarBinary || dbType == OleDbType.VarChar || dbType == OleDbType.VarWChar || dbType == OleDbType.WChar || dbType == OleDbType.Char || dbType == OleDbType.BSTR || dbType == OleDbType.Binary)
				{
					fm.Size = Convert.ToInt32(row["CHARACTER_MAXIMUM_LENGTH"]);
				}
				int columnFlags = Convert.ToInt32(row["COLUMN_FLAGS"]);
				int flags = (int)DBCOLUMNFLAGS.ISNULLABLE + (int)DBCOLUMNFLAGS.MAYBENULL;
				bool isNullableFlag = (columnFlags & flags) != 0;
				flags = (int)DBCOLUMNFLAGS.WRITE + (int)DBCOLUMNFLAGS.WRITEUNKNOWN;
				bool isReadOnly = (columnFlags & flags) == 0;
				fm.IsReadOnly = isReadOnly;
				if(row["DESCRIPTION"] != DBNull.Value && row["DESCRIPTION"] is string)
					fm.Comment = row["DESCRIPTION"].ToString();
			}
		}

		/// <summary>
		/// Recupera os dados das chaves primárias da tabela.
		/// </summary>
		private void GetPrimaryKeyData()
		{
			DataTable dt = GetPrimaryKeys(null);
			foreach (DataRow row in dt.Rows)
			{
				string tableName = (string)row["TABLE_NAME"];
				string columnName = (string)row["COLUMN_NAME"];
				TableMap map = GetTableMap(tableName);
				if(map != null)
				{
					FieldMap fm = map.GetFieldMapFromColumn(columnName);
					if(fm != null)
					{
						fm.IsPrimaryKey = true;
						DataTable pkInfo = GetPrimaryKeyInfo(map.QuotedTableName);
						foreach (DataRow dr in pkInfo.Rows)
						{
							string column = dr["ColumnName"].ToString();
							if(column == columnName)
							{
								bool isAutoGenerated = Convert.ToBoolean(dr["IsAutoIncrement"]);
								fm.IsAutoGenerated = isAutoGenerated;
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Recupera os dados de todas as chaves estrangeiras.
		/// </summary>
		private void GetForeignKeyData()
		{
			DataTable dt = GetForeignKeys(null);
			foreach (DataRow row in dt.Rows)
			{
				string fkTableName = (string)row["FK_TABLE_NAME"];
				string fkColumnName = (string)row["FK_COLUMN_NAME"];
				string pkTableName = (string)row["PK_TABLE_NAME"];
				string pkColumnName = (string)row["PK_COLUMN_NAME"];
				TableMap map = GetTableMap(fkTableName);
				if(map != null)
				{
					FieldMap fm = map.GetFieldMapFromColumn(fkColumnName);
					if(fm != null)
					{
						fm.ForeignKeyTableName = pkTableName;
						fm.ForeignKeyColumnName = pkColumnName;
					}
				}
			}
		}
	}
}
