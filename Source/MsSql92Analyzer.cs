using System;
using System.Collections.Generic;
using System.Text;
using GDA.Analysis;
using System.Data;
namespace GDA.Provider.MsSql
{
	public class MsSql92Analyzer : DatabaseAnalyzer
	{
		private const string select = "select c.TABLE_NAME as TableName, c.TABLE_SCHEMA as TableSchema, c.COLUMN_NAME as ColumnName, c.DATA_TYPE as Type, " + " c.CHARACTER_MAXIMUM_LENGTH as Size, c.IS_NULLABLE as IsNullable, " + " c.COLUMN_DEFAULT as DefaultValue, ccu.CONSTRAINT_NAME as ConstraintName, " + " rc.UNIQUE_CONSTRAINT_NAME as ConstraintReference, " + " rc.UPDATE_RULE as UpdateRule, rc.DELETE_RULE as DeleteRule, " + " tc.CONSTRAINT_TYPE as ConstraintType, t.TABLE_TYPE as TableType " + "from INFORMATION_SCHEMA.COLUMNS c " + "inner join INFORMATION_SCHEMA.TABLES t " + " on c.TABLE_NAME = t.TABLE_NAME " + "left join INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE ccu " + " on c.TABLE_NAME = ccu.TABLE_NAME and c.COLUMN_NAME = ccu.COLUMN_NAME " + "left outer join INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS rc " + " on ccu.CONSTRAINT_NAME = rc.CONSTRAINT_NAME " + "left outer join INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc " + " on ccu.CONSTRAINT_NAME = tc.CONSTRAINT_NAME " + "where t.TABLE_NAME != 'dtproperties'";
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
		public MsSql92Analyzer (MsSqlProviderConfiguration a) : base (a)
		{
		}
		private static bool GetBoolean (string a)
		{
			string[] b = new string[] {
				"yes",
				"true",
				"1"
			};
			a = a == null ? "false" : a.ToLower ();
			bool c = false;
			foreach (string valid in b) {
				c |= valid.Equals (a);
			}
			return c;
		}
		public override void Analyze (string a)
		{
			this.Analyze (a, null);
		}
		public override void Analyze (string a, string b)
		{
			try {
				bool c = a != null;
				IDbConnection d = ProviderConfiguration.CreateConnection ();
				GDAConnectionManager.NotifyConnectionCreated (d);
				IDataReader e = null;
				if (d.State != ConnectionState.Open) {
					d.Open ();
					GDAConnectionManager.NotifyConnectionOpened (d);
				}
				try {
					IDbCommand f = d.CreateCommand ();
					if (c) {
						if (!string.IsNullOrEmpty (b))
							f.CommandText = foreignKeysTables + string.Format ("\r\nWHERE FK.TABLE_NAME='{0}' AND FK.TABLE_SCHEMA = '{1}'", a, b);
						else
							f.CommandText = foreignKeysTables + string.Format ("\r\nWHERE FK.TABLE_NAME='{0}'", a);
					}
					else
						f.CommandText = foreignKeysTables;
					using (e = f.ExecuteReader ()) {
						while (e.Read ()) {
							var g = ForeignKeys [e ["Constraint_Name"].ToString ()];
							if (g == null) {
								ForeignKeys.Add (new ForeignKeyMap () {
									ConstraintName = e ["Constraint_Name"].ToString (),
									ConstraintSchema = e ["Constraint_Schema"].ToString (),
									ForeignKeyTable = e ["FK_Table"].ToString (),
									ForeignKeyTableSchema = e ["FK_Schema"].ToString (),
									ForeignKeyColumn = e ["FK_Column"].ToString (),
									PrimaryKeyTable = e ["PK_Table"].ToString (),
									PrimaryKeyTableSchema = e ["PK_Schema"].ToString (),
									PrimaryKeyColumn = e ["PK_Column"].ToString ()
								});
							}
						}
					}
					IDbCommand h = d.CreateCommand ();
					h.Connection = d;
					if (c) {
						if (!string.IsNullOrEmpty (b))
							h.CommandText = select + String.Format (" and t.TABLE_NAME = '{0}' and t.TABLE_SCHEMA = '{1}'", a, b);
						else
							h.CommandText = select + String.Format (" and t.TABLE_NAME = '{0}'", a);
					}
					else
						h.CommandText = select;
					using (e = h.ExecuteReader ()) {
						while (e.Read ()) {
							string i = e ["tablename"].ToString ();
							string j = (e ["tableschema"] ?? "").ToString ();
							if (!c || (string.Compare (a, i, true) == 0 && (string.IsNullOrEmpty (b) || string.Compare (b, j) == 0))) {
								TableMap k = GetTableMap (i, j);
								if (k == null) {
									k = new TableMap (ProviderConfiguration, i, j);
									tablesMaps [i.ToLower ()] = k;
								}
								k.IsView = e ["TableType"].ToString () == "VIEW";
								string l = e ["ColumnName"].ToString ();
								FieldMap m = k.GetFieldMapFromColumn (l);
								if (m == null) {
									m = new FieldMap (k, l);
									k.Fields.Add (m);
								}
								m.SetDbType (e ["Type"].ToString (), false);
								m.DbTypeName = e ["Type"].ToString ();
								m.IsNullable = GetBoolean (e ["IsNullable"].ToString ());
								if ((e ["Size"] != null && e ["Size"] != DBNull.Value) && m.DbType != (long)SqlDbType.Text)
									m.Size = int.Parse (e ["Size"].ToString ());
								if (e ["ConstraintName"] != null && e ["ConstraintName"] != DBNull.Value) {
									string n = e ["ConstraintType"].ToString ();
									if (n.ToLower ().Equals ("primary key")) {
										m.IsPrimaryKey = true;
									}
									else if (n.ToLower ().Equals ("foreign key")) {
										string o = e ["ConstraintReference"].ToString ();
										m.ForeignKeyConstraintName = e ["ConstraintName"].ToString ();
										if (o.StartsWith ("IDX")) {
											string p = e ["ConstraintName"].ToString ();
											if (p != null && p.StartsWith ("FK"))
												o = p;
										}
										IDbConnection q = ProviderConfiguration.CreateConnection ();
										GDAConnectionManager.NotifyConnectionCreated (q);
										IDbCommand r = q.CreateCommand ();
										r.CommandText = String.Format ("select c.TABLE_NAME as TableName, c.TABLE_SCHEMA as TableSchema, c.COLUMN_NAME as ColumnName " + "from INFORMATION_SCHEMA.COLUMNS c " + "inner join INFORMATION_SCHEMA.TABLES t " + " on c.TABLE_NAME = t.TABLE_NAME " + "left join INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE ccu " + " on c.TABLE_NAME = ccu.TABLE_NAME and c.COLUMN_NAME = ccu.COLUMN_NAME " + "where t.TABLE_TYPE = 'BASE TABLE' and ccu.CONSTRAINT_NAME = '{0}'", o);
										try {
											if (q.State != ConnectionState.Open) {
												q.Open ();
												GDAConnectionManager.NotifyConnectionOpened (q);
											}
											using (IDataReader s = r.ExecuteReader ()) {
												if (s.Read ()) {
													m.ForeignKeyTableName = s ["TableName"].ToString ();
													m.ForeignKeyTableSchema = (s ["TableSchema"] ?? "").ToString ();
													m.ForeignKeyColumnName = s ["ColumnName"].ToString ();
												}
											}
										}
										catch (Exception ex) {
											throw new GDAException (string.Format ("Unable to obtain foreign key information for column {0} of table {1}.", m.ColumnName, k.TableName), ex);
										}
										finally {
											if (q.State == ConnectionState.Open) {
												q.Close ();
												q.Dispose ();
											}
										}
									}
								}
								m.IsAutoGenerated = (e ["DefaultValue"].ToString ().Length > 0 && m.IsPrimaryKey ? true : false);
								if (k.IsView) {
								}
							}
						}
					}
				}
				finally {
					d.Close ();
					d.Dispose ();
				}
			}
			catch (Exception ex) {
				throw new GDAException ("An error occurred while analyzing the database schema.", ex);
			}
		}
	}
}
