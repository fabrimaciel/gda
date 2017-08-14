using System;
using System.Collections.Generic;
using System.Text;
using GDA.Analysis;
using System.Data;
namespace GDA.Provider.Oracle
{
	public class OracleAnalyzer : DatabaseAnalyzer
	{
		private static bool GetBoolean (string a)
		{
			string[] b = new string[] {
				"yes",
				"true",
				"1",
				"y"
			};
			a = a == null ? "false" : a.ToLower ();
			bool c = false;
			foreach (string valid in b) {
				c |= valid.Equals (a);
			}
			return c;
		}
		public OracleAnalyzer (OracleProviderConfiguration a) : base (a)
		{
		}
		public override void Analyze (string a)
		{
			IDbConnection b = ProviderConfiguration.CreateConnection ();
			IDbCommand c = b.CreateCommand ();
			IDbDataAdapter d = ProviderConfiguration.Provider.CreateDataAdapter ();
			d.SelectCommand = c;
			c.Connection = b;
			b.Open ();
			try {
				bool f = a != null;
				c.CommandText = @"select VERSION ""Version"" from PRODUCT_COMPONENT_VERSION where lower(product) like ('%oracle%')";
				string g = c.ExecuteScalar ().ToString ();
				int h = g.IndexOf (".");
				if (h < 0) {
					throw new GDAException ("Unable to determine Oracle database version.");
				}
				int i = Convert.ToInt32 (g.Substring (0, h));
				string j;
				string k;
				if (i < 9) {
					if (g.Substring (0, 5).CompareTo ("8.1.6") == 0) {
						k = @"select O.NAME ""ParentTable"", " + @"	 COL.NAME ""ParentColumn"", " + @"	 OC.NAME ""ConstraintName"", " + @"	 RC.NAME ""ConstraintReference"", " + @"	 BASE.NAME  ""ChildTable"", " + @"	 IDC.NAME ""ChildColumn""	" + @"from SYS.CON$ OC, SYS.CON$ RC, SYS.CDEF$ C, SYS.OBJ$ O, SYS.COL$ COL, " + @"     SYS.CCOL$ CC, SYS.OBJ$ IDX, SYS.IND$ I, SYS.ICOL$ IC, SYS.OBJ$ BASE, SYS.COL$ IDC " + @"where OC.CON# = C.CON# and C.RCON# = RC.CON# and C.TYPE# = 4 and C.OBJ# = O.OBJ# " + @"	and C.CON# = CC.CON# and CC.OBJ# = COL.OBJ# and CC.INTCOL# = COL.INTCOL# " + @"	and IDX.OWNER# = O.OWNER# and RC.NAME = IDX.NAME and IDX.OBJ# = I.OBJ# " + @"	and I.TYPE# IN (1, 2, 3, 4, 6, 7, 9) and IC.OBJ# = IDX.OBJ# and IC.BO# = BASE.OBJ# " + @"  and IDC.OBJ# = BASE.OBJ# and IC.INTCOL# = IDC.INTCOL# and O.OWNER# = USERENV('SCHEMAID') " + @"  and RC.NAME =  '{0}' ";
					}
					else {
						k = @"select CO.TABLE_NAME        ""ParentTable"", " + @"       CO1.COLUMN_NAME      ""ParentColumn"", " + @"       CO.CONSTRAINT_NAME   ""ConstraintName"", " + @"       CO.R_CONSTRAINT_NAME ""ConstraintReference"", " + @"       IDX2.TABLE_NAME      ""ChildTable"", " + @"       IDX2.COLUMN_NAME     ""ChildColumn"" " + @"from USER_CONSTRAINTS CO, USER_CONS_COLUMNS CO1, USER_IND_COLUMNS IDX2 " + @"where CO.TABLE_NAME = CO1.TABLE_NAME AND " + @"      CO.CONSTRAINT_NAME = CO1.CONSTRAINT_NAME AND " + @"      CO.R_CONSTRAINT_NAME = IDX2.INDEX_NAME AND " + @"      CO1.POSITION = IDX2.COLUMN_POSITION AND " + @"      CO.CONSTRAINT_TYPE = 'R' AND " + @"      CO.R_CONSTRAINT_NAME = '{0}'";
					}
					j = @"select TAB.TABLE_NAME       ""TableName"", " + @"       TAB.COLUMN_NAME      ""ColumnName"", " + @"       TAB.DATA_TYPE        ""Type"", " + @"       TAB.DATA_LENGTH      ""Size"", " + @"       TAB.NULLABLE         ""IsNullable"", " + @"       TAB.DATA_DEFAULT     ""DefaultValue"", " + @"       CO.CONSTRAINT_NAME   ""ConstraintName"", " + @"       CO.CONSTRAINT_TYPE   ""ConstraintType"", " + @"       CO.R_CONSTRAINT_NAME ""ConstraintReference"", " + @"       CO.DELETE_RULE       ""DeleteRule"" " + @"from USER_TAB_COLUMNS TAB, USER_CONSTRAINTS CO, USER_CONS_COLUMNS CO1 " + @"where (TAB.TABLE_NAME = CO1.TABLE_NAME(+)) and " + @"      (TAB.COLUMN_NAME = CO1.COLUMN_NAME(+)) and " + @"      (CO1.constraint_name = CO.constraint_name(+)) and " + @"      (CO1.TABLE_NAME = CO.TABLE_NAME(+)) " + @"order by TAB.TABLE_NAME, TAB.COLUMN_NAME, CO1.CONSTRAINT_NAME";
				}
				else {
					j = @"select TAB.TABLE_NAME       ""TableName"",  " + @"		   TAB.COLUMN_NAME      ""ColumnName"",  " + @"		   TAB.DATA_TYPE        ""Type"",  " + @"		   TAB.DATA_LENGTH      ""Size"",  " + @"		   TAB.NULLABLE         ""IsNullable"",  " + @"		   TAB.DATA_DEFAULT     ""DefaultValue"",  " + @"		   CO.CONSTRAINT_NAME   ""ConstraintName"", " + @"		   CO.CONSTRAINT_TYPE   ""ConstraintType"", " + @"		   CO.R_CONSTRAINT_NAME ""ConstraintReference"", " + @"		   CO.DELETE_RULE       ""DeleteRule"", " + @"         CM.COMMENTS 			""TableComment"", " + @"         CC.COMMENTS			""ColumnComment"" " + @"from USER_TAB_COLUMNS TAB LEFT OUTER JOIN " + @"	     (USER_CONSTRAINTS CO INNER JOIN USER_CONS_COLUMNS CO1 ON " + @"		   CO.TABLE_NAME = CO1.TABLE_NAME AND CO.CONSTRAINT_NAME = CO1.CONSTRAINT_NAME) ON " + @"	   TAB.TABLE_NAME = CO.TABLE_NAME AND TAB.COLUMN_NAME = CO1.COLUMN_NAME " + @"     LEFT JOIN USER_TAB_COMMENTS CM ON TAB.TABLE_NAME = CM.TABLE_NAME " + @"     LEFT JOIN USER_COL_COMMENTS CC ON TAB.TABLE_NAME = CC.TABLE_NAME AND TAB.COLUMN_NAME = CC.COLUMN_NAME " + @"order by TAB.TABLE_NAME, TAB.COLUMN_NAME, CO1.CONSTRAINT_NAME";
					k = @"select CO.TABLE_NAME        ""ParentTable"", " + @"		   CO1.COLUMN_NAME      ""ParentColumn"", " + @"		   CO.CONSTRAINT_NAME   ""ConstraintName"", " + @"		   CO.R_CONSTRAINT_NAME ""ConstraintReference"", " + @"		   IDX2.TABLE_NAME      ""ChildTable"", " + @"		   IDX2.COLUMN_NAME     ""ChildColumn"" " + @"from (USER_CONSTRAINTS CO INNER JOIN USER_CONS_COLUMNS CO1 ON " + @"			   CO.TABLE_NAME = CO1.TABLE_NAME AND CO.CONSTRAINT_NAME = CO1.CONSTRAINT_NAME) " + @"		 LEFT OUTER JOIN USER_IND_COLUMNS IDX2 ON " + @"			  CO.R_CONSTRAINT_NAME = IDX2.INDEX_NAME AND CO1.POSITION = IDX2.COLUMN_POSITION " + @"where CO.CONSTRAINT_TYPE = 'R' AND " + @"      CO.R_CONSTRAINT_NAME = '{0}'";
				}
				d.SelectCommand.CommandText = j;
				DataSet l = new DataSet ();
				d.Fill (l);
				DataTable m = l.Tables [0];
				for (int n = 0; n < m.Rows.Count; n++) {
					try {
						string o = (string)m.Rows [n] ["TableName"];
						if (!f || a.ToLower () == o.ToLower ()) {
							TableMap p = GetTableMap (o);
							if (p == null) {
								p = new TableMap (ProviderConfiguration, o);
								if (m.Rows [n] ["TableComment"] != DBNull.Value)
									p.Comment = (string)m.Rows [n] ["TableComment"];
								this.tablesMaps [o.ToLower ()] = p;
							}
							string q = (string)m.Rows [n] ["ColumnName"];
							FieldMap r = p.GetFieldMapFromColumn (q);
							if (r == null) {
								r = new FieldMap (p, q);
								if (m.Rows [n] ["ColumnComment"] != DBNull.Value)
									r.Comment = (string)m.Rows [n] ["ColumnComment"];
								p.Fields.Add (r);
							}
							string s = (string)m.Rows [n] ["Type"];
							int t = s.IndexOf ("(");
							if (t != -1)
								s = s.Substring (0, t);
							r.SetDbType (s, false);
							r.IsNullable = GetBoolean ((string)m.Rows [n] ["IsNullable"]);
							if (m.Rows [n] ["Size"] != DBNull.Value)
								r.Size = Convert.ToInt32 (m.Rows [n] ["Size"]);
							if (m.Rows [n] ["ConstraintName"] != DBNull.Value) {
								string u = (string)m.Rows [n] ["ConstraintType"];
								if (u.ToLower () == "p") {
									r.IsPrimaryKey = true;
								}
								else if (u.ToLower () == "r") {
									string v = (string)m.Rows [n] ["ConstraintReference"];
									c.CommandText = String.Format (k, v);
									using (IDataReader w = c.ExecuteReader ()) {
										if (w.Read ()) {
											r.ForeignKeyTableName = w.GetString (w.GetOrdinal ("ChildTable"));
											r.ForeignKeyColumnName = w.GetString (w.GetOrdinal ("ChildColumn"));
										}
									}
								}
							}
						}
					}
					catch (Exception fe) {
					}
				}
			}
			catch (Exception e) {
				throw new GDAException ("An error occurred while analyzing the database schema.", e);
			}
			finally {
				b.Close ();
			}
		}
	}
}
