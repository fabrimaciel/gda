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

namespace GDA.Provider.MsSql
{
	public class MsSql92Analyzer : DatabaseAnalyzer
	{
		/// <summary>
		/// Consulta para recupera os dados das tabelas.
		/// </summary>
		private const string select = "select c.TABLE_NAME as TableName, c.TABLE_SCHEMA as TableSchema, c.COLUMN_NAME as ColumnName, c.DATA_TYPE as Type, " + " c.CHARACTER_MAXIMUM_LENGTH as Size, c.IS_NULLABLE as IsNullable, " + " c.COLUMN_DEFAULT as DefaultValue, ccu.CONSTRAINT_NAME as ConstraintName, " + " rc.UNIQUE_CONSTRAINT_NAME as ConstraintReference, " + " rc.UPDATE_RULE as UpdateRule, rc.DELETE_RULE as DeleteRule, " + " tc.CONSTRAINT_TYPE as ConstraintType, t.TABLE_TYPE as TableType " + "from INFORMATION_SCHEMA.COLUMNS c " + "inner join INFORMATION_SCHEMA.TABLES t " + " on c.TABLE_NAME = t.TABLE_NAME " + "left join INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE ccu " + " on c.TABLE_NAME = ccu.TABLE_NAME and c.COLUMN_NAME = ccu.COLUMN_NAME " + "left outer join INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS rc " + " on ccu.CONSTRAINT_NAME = rc.CONSTRAINT_NAME " + "left outer join INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc " + " on ccu.CONSTRAINT_NAME = tc.CONSTRAINT_NAME " + "where t.TABLE_NAME != 'dtproperties'";

		/// <summary>
		/// Consulta usada para recuperar todos as chaves estrangeiras das tabelas.
		/// </summary>
		private const string foreignKeysTables = @"SELECT 
                    FK_Table  = FK.TABLE_NAME,
                    FK_Schema  = FK.TABLE_SCHEMA,
                    FK_Column = CU.COLUMN_NAME, 
                    PK_Table  = PK.TABLE_NAME, 
                    PK_Schema  = PK.TABLE_SCHEMA,
                    PK_Column = PT.COLUMN_NAME, 
                    Constraint_Name = C.CONSTRAINT_NAME ,
                    Constraint_Schema = C.CONSTRAINT_SCHEMA
                FROM 
                    INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS C 
                    INNER JOIN 
                    INFORMATION_SCHEMA.TABLE_CONSTRAINTS FK 
                        ON C.CONSTRAINT_NAME = FK.CONSTRAINT_NAME 
                    INNER JOIN 
                    INFORMATION_SCHEMA.TABLE_CONSTRAINTS PK 
                        ON C.UNIQUE_CONSTRAINT_NAME = PK.CONSTRAINT_NAME 
                    INNER JOIN 
                    INFORMATION_SCHEMA.KEY_COLUMN_USAGE CU 
                        ON C.CONSTRAINT_NAME = CU.CONSTRAINT_NAME 
                    INNER JOIN 
                    ( 
                        SELECT 
                            i1.TABLE_NAME, i2.COLUMN_NAME 
                        FROM 
                            INFORMATION_SCHEMA.TABLE_CONSTRAINTS i1 
                            INNER JOIN 
                            INFORMATION_SCHEMA.KEY_COLUMN_USAGE i2 
                            ON i1.CONSTRAINT_NAME = i2.CONSTRAINT_NAME 
                            WHERE i1.CONSTRAINT_TYPE = 'PRIMARY KEY' 
                    ) PT 
                    ON PT.TABLE_NAME = PK.TABLE_NAME";

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="provider"></param>
		public MsSql92Analyzer(MsSqlProviderConfiguration provider) : base(provider)
		{
		}

		/// <summary>
		/// Recuperae o valor booleano contido na string.
		/// </summary>
		/// <param name="boolean"></param>
		/// <returns></returns>
		private static bool GetBoolean(string boolean)
		{
			string[] valids = new string[] {
				"yes",
				"true",
				"1"
			};
			boolean = boolean == null ? "false" : boolean.ToLower();
			bool result = false;
			foreach (string valid in valids)
			{
				result |= valid.Equals(boolean);
			}
			return result;
		}

		/// <summary>
		/// Please refer to the <see cref="DatabaseAnalyzer"/> class and the <see cref="DatabaseAnalyzer"/> 
		/// interface it implements a description of this method.
		/// </summary>
		/// <param name="tableName"></param>
		public override void Analyze(string tableName)
		{
			this.Analyze(tableName, null);
		}

		/// <summary>
		/// Please refer to the <see cref="DatabaseAnalyzer"/> class and the <see cref="DatabaseAnalyzer"/> 
		/// interface it implements a description of this method.
		/// </summary>
		public override void Analyze(string tableName, string tableSchema)
		{
			try
			{
				bool isSingleRun = tableName != null;
				IDbConnection conn = ProviderConfiguration.CreateConnection();
				GDAConnectionManager.NotifyConnectionCreated(conn);
				IDataReader reader = null;
				if(conn.State != ConnectionState.Open)
				{
					conn.Open();
					GDAConnectionManager.NotifyConnectionOpened(conn);
				}
				try
				{
					IDbCommand fkCmd = conn.CreateCommand();
					if(isSingleRun)
					{
						if(!string.IsNullOrEmpty(tableSchema))
							fkCmd.CommandText = foreignKeysTables + string.Format("\r\nWHERE FK.TABLE_NAME='{0}' AND FK.TABLE_SCHEMA = '{1}'", tableName, tableSchema);
						else
							fkCmd.CommandText = foreignKeysTables + string.Format("\r\nWHERE FK.TABLE_NAME='{0}'", tableName);
					}
					else
						fkCmd.CommandText = foreignKeysTables;
					using (reader = fkCmd.ExecuteReader())
					{
						while (reader.Read())
						{
							var fk = ForeignKeys[reader["Constraint_Name"].ToString()];
							if(fk == null)
							{
								ForeignKeys.Add(new ForeignKeyMap() {
									ConstraintName = reader["Constraint_Name"].ToString(),
									ConstraintSchema = reader["Constraint_Schema"].ToString(),
									ForeignKeyTable = reader["FK_Table"].ToString(),
									ForeignKeyTableSchema = reader["FK_Schema"].ToString(),
									ForeignKeyColumn = reader["FK_Column"].ToString(),
									PrimaryKeyTable = reader["PK_Table"].ToString(),
									PrimaryKeyTableSchema = reader["PK_Schema"].ToString(),
									PrimaryKeyColumn = reader["PK_Column"].ToString()
								});
							}
						}
					}
					IDbCommand cmd = conn.CreateCommand();
					cmd.Connection = conn;
					if(isSingleRun)
					{
						if(!string.IsNullOrEmpty(tableSchema))
							cmd.CommandText = select + String.Format(" and t.TABLE_NAME = '{0}' and t.TABLE_SCHEMA = '{1}'", tableName, tableSchema);
						else
							cmd.CommandText = select + String.Format(" and t.TABLE_NAME = '{0}'", tableName);
					}
					else
						cmd.CommandText = select;
					using (reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							string dbTableName = reader["tablename"].ToString();
							string dbTableSchema = (reader["tableschema"] ?? "").ToString();
							if(!isSingleRun || (string.Compare(tableName, dbTableName, true) == 0 && (string.IsNullOrEmpty(tableSchema) || string.Compare(tableSchema, dbTableSchema) == 0)))
							{
								TableMap map = GetTableMap(dbTableName, dbTableSchema);
								if(map == null)
								{
									map = new TableMap(ProviderConfiguration, dbTableName, dbTableSchema);
									tablesMaps[dbTableName.ToLower()] = map;
								}
								map.IsView = reader["TableType"].ToString() == "VIEW";
								string columnName = reader["ColumnName"].ToString();
								FieldMap fm = map.GetFieldMapFromColumn(columnName);
								if(fm == null)
								{
									fm = new FieldMap(map, columnName);
									map.Fields.Add(fm);
								}
								fm.SetDbType(reader["Type"].ToString(), false);
								fm.DbTypeName = reader["Type"].ToString();
								fm.IsNullable = GetBoolean(reader["IsNullable"].ToString());
								if((reader["Size"] != null && reader["Size"] != DBNull.Value) && fm.DbType != (long)SqlDbType.Text)
									fm.Size = int.Parse(reader["Size"].ToString());
								if(reader["ConstraintName"] != null && reader["ConstraintName"] != DBNull.Value)
								{
									string type = reader["ConstraintType"].ToString();
									if(type.ToLower().Equals("primary key"))
									{
										fm.IsPrimaryKey = true;
									}
									else if(type.ToLower().Equals("foreign key"))
									{
										string conref = reader["ConstraintReference"].ToString();
										fm.ForeignKeyConstraintName = reader["ConstraintName"].ToString();
										if(conref.StartsWith("IDX"))
										{
											string fkRef = reader["ConstraintName"].ToString();
											if(fkRef != null && fkRef.StartsWith("FK"))
												conref = fkRef;
										}
										IDbConnection conn2 = ProviderConfiguration.CreateConnection();
										GDAConnectionManager.NotifyConnectionCreated(conn2);
										IDbCommand cmd2 = conn2.CreateCommand();
										cmd2.CommandText = String.Format("select c.TABLE_NAME as TableName, c.TABLE_SCHEMA as TableSchema, c.COLUMN_NAME as ColumnName " + "from INFORMATION_SCHEMA.COLUMNS c " + "inner join INFORMATION_SCHEMA.TABLES t " + " on c.TABLE_NAME = t.TABLE_NAME " + "left join INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE ccu " + " on c.TABLE_NAME = ccu.TABLE_NAME and c.COLUMN_NAME = ccu.COLUMN_NAME " + "where t.TABLE_TYPE = 'BASE TABLE' and ccu.CONSTRAINT_NAME = '{0}'", conref);
										try
										{
											if(conn2.State != ConnectionState.Open)
											{
												conn2.Open();
												GDAConnectionManager.NotifyConnectionOpened(conn2);
											}
											using (IDataReader reader2 = cmd2.ExecuteReader())
											{
												if(reader2.Read())
												{
													fm.ForeignKeyTableName = reader2["TableName"].ToString();
													fm.ForeignKeyTableSchema = (reader2["TableSchema"] ?? "").ToString();
													fm.ForeignKeyColumnName = reader2["ColumnName"].ToString();
												}
											}
										}
										catch(Exception ex)
										{
											throw new GDAException(string.Format("Unable to obtain foreign key information for column {0} of table {1}.", fm.ColumnName, map.TableName), ex);
										}
										finally
										{
											if(conn2.State == ConnectionState.Open)
											{
												conn2.Close();
												conn2.Dispose();
											}
										}
									}
								}
								fm.IsAutoGenerated = (reader["DefaultValue"].ToString().Length > 0 && fm.IsPrimaryKey ? true : false);
								if(map.IsView)
								{
								}
							}
						}
					}
				}
				finally
				{
					conn.Close();
					conn.Dispose();
				}
			}
			catch(Exception ex)
			{
				throw new GDAException("An error occurred while analyzing the database schema.", ex);
			}
		}
	}
}
