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

namespace GDA.Provider.Oracle
{
	public class OracleAnalyzer : DatabaseAnalyzer
	{
		private static bool GetBoolean(string boolean)
		{
			string[] valids = new string[] {
				"yes",
				"true",
				"1",
				"y"
			};
			boolean = boolean == null ? "false" : boolean.ToLower();
			bool result = false;
			foreach (string valid in valids)
			{
				result |= valid.Equals(boolean);
			}
			return result;
		}

		public OracleAnalyzer(OracleProviderConfiguration provider) : base(provider)
		{
		}

		public override void Analyze(string tableName)
		{
			IDbConnection conn = ProviderConfiguration.CreateConnection();
			IDbCommand cmd = conn.CreateCommand();
			IDbDataAdapter da = ProviderConfiguration.Provider.CreateDataAdapter();
			da.SelectCommand = cmd;
			cmd.Connection = conn;
			conn.Open();
			try
			{
				bool isSingleRun = tableName != null;
				cmd.CommandText = @"select VERSION ""Version"" from PRODUCT_COMPONENT_VERSION where lower(product) like ('%oracle%')";
				string ver = cmd.ExecuteScalar().ToString();
				int indexOfDot = ver.IndexOf(".");
				if(indexOfDot < 0)
				{
					throw new GDAException("Unable to determine Oracle database version.");
				}
				int version = Convert.ToInt32(ver.Substring(0, indexOfDot));
				string select;
				string selectReferences;
				if(version < 9)
				{
					if(ver.Substring(0, 5).CompareTo("8.1.6") == 0)
					{
						selectReferences = @"select O.NAME ""ParentTable"", " + @"	 COL.NAME ""ParentColumn"", " + @"	 OC.NAME ""ConstraintName"", " + @"	 RC.NAME ""ConstraintReference"", " + @"	 BASE.NAME  ""ChildTable"", " + @"	 IDC.NAME ""ChildColumn""	" + @"from SYS.CON$ OC, SYS.CON$ RC, SYS.CDEF$ C, SYS.OBJ$ O, SYS.COL$ COL, " + @"     SYS.CCOL$ CC, SYS.OBJ$ IDX, SYS.IND$ I, SYS.ICOL$ IC, SYS.OBJ$ BASE, SYS.COL$ IDC " + @"where OC.CON# = C.CON# and C.RCON# = RC.CON# and C.TYPE# = 4 and C.OBJ# = O.OBJ# " + @"	and C.CON# = CC.CON# and CC.OBJ# = COL.OBJ# and CC.INTCOL# = COL.INTCOL# " + @"	and IDX.OWNER# = O.OWNER# and RC.NAME = IDX.NAME and IDX.OBJ# = I.OBJ# " + @"	and I.TYPE# IN (1, 2, 3, 4, 6, 7, 9) and IC.OBJ# = IDX.OBJ# and IC.BO# = BASE.OBJ# " + @"  and IDC.OBJ# = BASE.OBJ# and IC.INTCOL# = IDC.INTCOL# and O.OWNER# = USERENV('SCHEMAID') " + @"  and RC.NAME =  '{0}' ";
					}
					else
					{
						selectReferences = @"select CO.TABLE_NAME        ""ParentTable"", " + @"       CO1.COLUMN_NAME      ""ParentColumn"", " + @"       CO.CONSTRAINT_NAME   ""ConstraintName"", " + @"       CO.R_CONSTRAINT_NAME ""ConstraintReference"", " + @"       IDX2.TABLE_NAME      ""ChildTable"", " + @"       IDX2.COLUMN_NAME     ""ChildColumn"" " + @"from USER_CONSTRAINTS CO, USER_CONS_COLUMNS CO1, USER_IND_COLUMNS IDX2 " + @"where CO.TABLE_NAME = CO1.TABLE_NAME AND " + @"      CO.CONSTRAINT_NAME = CO1.CONSTRAINT_NAME AND " + @"      CO.R_CONSTRAINT_NAME = IDX2.INDEX_NAME AND " + @"      CO1.POSITION = IDX2.COLUMN_POSITION AND " + @"      CO.CONSTRAINT_TYPE = 'R' AND " + @"      CO.R_CONSTRAINT_NAME = '{0}'";
					}
					select = @"select TAB.TABLE_NAME       ""TableName"", " + @"       TAB.COLUMN_NAME      ""ColumnName"", " + @"       TAB.DATA_TYPE        ""Type"", " + @"       TAB.DATA_LENGTH      ""Size"", " + @"       TAB.NULLABLE         ""IsNullable"", " + @"       TAB.DATA_DEFAULT     ""DefaultValue"", " + @"       CO.CONSTRAINT_NAME   ""ConstraintName"", " + @"       CO.CONSTRAINT_TYPE   ""ConstraintType"", " + @"       CO.R_CONSTRAINT_NAME ""ConstraintReference"", " + @"       CO.DELETE_RULE       ""DeleteRule"" " + @"from USER_TAB_COLUMNS TAB, USER_CONSTRAINTS CO, USER_CONS_COLUMNS CO1 " + @"where (TAB.TABLE_NAME = CO1.TABLE_NAME(+)) and " + @"      (TAB.COLUMN_NAME = CO1.COLUMN_NAME(+)) and " + @"      (CO1.constraint_name = CO.constraint_name(+)) and " + @"      (CO1.TABLE_NAME = CO.TABLE_NAME(+)) " + @"order by TAB.TABLE_NAME, TAB.COLUMN_NAME, CO1.CONSTRAINT_NAME";
				}
				else
				{
					select = @"select TAB.TABLE_NAME       ""TableName"",  " + @"		   TAB.COLUMN_NAME      ""ColumnName"",  " + @"		   TAB.DATA_TYPE        ""Type"",  " + @"		   TAB.DATA_LENGTH      ""Size"",  " + @"		   TAB.NULLABLE         ""IsNullable"",  " + @"		   TAB.DATA_DEFAULT     ""DefaultValue"",  " + @"		   CO.CONSTRAINT_NAME   ""ConstraintName"", " + @"		   CO.CONSTRAINT_TYPE   ""ConstraintType"", " + @"		   CO.R_CONSTRAINT_NAME ""ConstraintReference"", " + @"		   CO.DELETE_RULE       ""DeleteRule"", " + @"         CM.COMMENTS 			""TableComment"", " + @"         CC.COMMENTS			""ColumnComment"" " + @"from USER_TAB_COLUMNS TAB LEFT OUTER JOIN " + @"	     (USER_CONSTRAINTS CO INNER JOIN USER_CONS_COLUMNS CO1 ON " + @"		   CO.TABLE_NAME = CO1.TABLE_NAME AND CO.CONSTRAINT_NAME = CO1.CONSTRAINT_NAME) ON " + @"	   TAB.TABLE_NAME = CO.TABLE_NAME AND TAB.COLUMN_NAME = CO1.COLUMN_NAME " + @"     LEFT JOIN USER_TAB_COMMENTS CM ON TAB.TABLE_NAME = CM.TABLE_NAME " + @"     LEFT JOIN USER_COL_COMMENTS CC ON TAB.TABLE_NAME = CC.TABLE_NAME AND TAB.COLUMN_NAME = CC.COLUMN_NAME " + @"order by TAB.TABLE_NAME, TAB.COLUMN_NAME, CO1.CONSTRAINT_NAME";
					selectReferences = @"select CO.TABLE_NAME        ""ParentTable"", " + @"		   CO1.COLUMN_NAME      ""ParentColumn"", " + @"		   CO.CONSTRAINT_NAME   ""ConstraintName"", " + @"		   CO.R_CONSTRAINT_NAME ""ConstraintReference"", " + @"		   IDX2.TABLE_NAME      ""ChildTable"", " + @"		   IDX2.COLUMN_NAME     ""ChildColumn"" " + @"from (USER_CONSTRAINTS CO INNER JOIN USER_CONS_COLUMNS CO1 ON " + @"			   CO.TABLE_NAME = CO1.TABLE_NAME AND CO.CONSTRAINT_NAME = CO1.CONSTRAINT_NAME) " + @"		 LEFT OUTER JOIN USER_IND_COLUMNS IDX2 ON " + @"			  CO.R_CONSTRAINT_NAME = IDX2.INDEX_NAME AND CO1.POSITION = IDX2.COLUMN_POSITION " + @"where CO.CONSTRAINT_TYPE = 'R' AND " + @"      CO.R_CONSTRAINT_NAME = '{0}'";
				}
				da.SelectCommand.CommandText = select;
				DataSet ds = new DataSet();
				da.Fill(ds);
				DataTable dt = ds.Tables[0];
				for(int i = 0; i < dt.Rows.Count; i++)
				{
					try
					{
						string dbTableName = (string)dt.Rows[i]["TableName"];
						if(!isSingleRun || tableName.ToLower() == dbTableName.ToLower())
						{
							TableMap map = GetTableMap(dbTableName);
							if(map == null)
							{
								map = new TableMap(ProviderConfiguration, dbTableName);
								if(dt.Rows[i]["TableComment"] != DBNull.Value)
									map.Comment = (string)dt.Rows[i]["TableComment"];
								this.tablesMaps[dbTableName.ToLower()] = map;
							}
							string columnName = (string)dt.Rows[i]["ColumnName"];
							FieldMap fm = map.GetFieldMapFromColumn(columnName);
							if(fm == null)
							{
								fm = new FieldMap(map, columnName);
								if(dt.Rows[i]["ColumnComment"] != DBNull.Value)
									fm.Comment = (string)dt.Rows[i]["ColumnComment"];
								map.Fields.Add(fm);
							}
							string typeInfo = (string)dt.Rows[i]["Type"];
							int pos = typeInfo.IndexOf("(");
							if(pos != -1)
								typeInfo = typeInfo.Substring(0, pos);
							fm.SetDbType(typeInfo, false);
							fm.IsNullable = GetBoolean((string)dt.Rows[i]["IsNullable"]);
							if(dt.Rows[i]["Size"] != DBNull.Value)
								fm.Size = Convert.ToInt32(dt.Rows[i]["Size"]);
							if(dt.Rows[i]["ConstraintName"] != DBNull.Value)
							{
								string typ = (string)dt.Rows[i]["ConstraintType"];
								if(typ.ToLower() == "p")
								{
									fm.IsPrimaryKey = true;
								}
								else if(typ.ToLower() == "r")
								{
									string conref = (string)dt.Rows[i]["ConstraintReference"];
									cmd.CommandText = String.Format(selectReferences, conref);
									using (IDataReader dr = cmd.ExecuteReader())
									{
										if(dr.Read())
										{
											fm.ForeignKeyTableName = dr.GetString(dr.GetOrdinal("ChildTable"));
											fm.ForeignKeyColumnName = dr.GetString(dr.GetOrdinal("ChildColumn"));
										}
									}
								}
							}
						}
					}
					catch(Exception fe)
					{
					}
				}
			}
			catch(Exception e)
			{
				throw new GDAException("An error occurred while analyzing the database schema.", e);
			}
			finally
			{
				conn.Close();
			}
		}
	}
}
