using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
namespace GDA.Provider.MsSql
{
	public class MsSqlProvider : Provider
	{
		private static System.Collections.Generic.List<string> reservedsWords;
		private MsSqlProviderDialects _msSqlDialect = MsSqlProviderDialects.MsSql2000;
		public MsSqlProviderDialects MsSqlDialect {
			get {
				return _msSqlDialect;
			}
			set {
				_msSqlDialect = value;
			}
		}
		public MsSqlProvider () : base ("MsSql", typeof(SqlConnection), typeof(SqlDataAdapter), typeof(SqlCommand), typeof(SqlParameter), "@", "select @@IDENTITY;", true)
		{
			if (reservedsWords == null) {
				reservedsWords = new List<string> (new string[] {
					"ABSOLUTE",
					"ACTION",
					"ADA",
					"ADD",
					"ADMIN",
					"AFTER",
					"AGGREGATE",
					"ALIAS",
					"ALL",
					"ALLOCATE",
					"ALTER",
					"AND",
					"ANY",
					"ARE",
					"ARRAY",
					"AS",
					"ASC",
					"ASSERTION",
					"AT",
					"AUTHORIZATION",
					"AVG",
					"BACKUP",
					"BEFORE",
					"BEGIN",
					"BETWEEN",
					"BINARY",
					"BIT",
					"BIT_LENGTH",
					"BLOB",
					"BOOLEAN",
					"BOTH",
					"BREADTH",
					"BREAK",
					"BROWSE",
					"BULK",
					"BY",
					"CALL",
					"CASCADE",
					"CASCADED",
					"CASE",
					"CAST",
					"CATALOG",
					"CHAR",
					"CHARACTER",
					"CHARACTER_LENGTH",
					"CHAR_LENGTH",
					"CHECK",
					"CHECKPOINT",
					"CLASS",
					"CLOB",
					"CLOSE",
					"CLUSTERED",
					"COALESCE",
					"COLLATE",
					"COLLATION",
					"COLUMN",
					"COMMIT",
					"COMPLETION",
					"COMPUTE",
					"CONNECT",
					"CONNECTION",
					"CONSTRAINT",
					"CONSTRAINTS",
					"CONSTRUCTOR",
					"CONTAINS",
					"CONTAINSTABLE",
					"CONTINUE",
					"CONVERT",
					"CORRESPONDING",
					"COUNT",
					"CREATE",
					"CROSS",
					"CUBE",
					"CURRENT",
					"CURRENT_DATE",
					"CURRENT_PATH",
					"CURRENT_ROLE",
					"CURRENT_TIME",
					"CURRENT_TIMESTAMP",
					"CURRENT_USER",
					"CURSOR",
					"CYCLE",
					"DATA",
					"DATABASE",
					"DATE",
					"DAY",
					"DBCC",
					"DEALLOCATE",
					"DEC",
					"DECIMAL",
					"DECLARE",
					"DEFAULT",
					"DEFERRABLE",
					"DEFERRED",
					"DELETE",
					"DENY",
					"DEPTH",
					"DEREF",
					"DESC",
					"DESCRIBE",
					"DESCRIPTOR",
					"DESTROY",
					"DESTRUCTOR",
					"DETERMINISTIC",
					"DIAGNOSTICS",
					"DICTIONARY",
					"DISCONNECT",
					"DISK",
					"DISTINCT",
					"DISTRIBUTED",
					"DOMAIN",
					"DOUBLE",
					"DROP",
					"DUMMY",
					"DUMP",
					"DYNAMIC",
					"EACH",
					"ELSE",
					"END",
					"END-EXEC",
					"EQUALS",
					"ERRLVL",
					"ESCAPE",
					"EVERY",
					"EXCEPT",
					"EXCEPTION",
					"EXEC",
					"EXECUTE",
					"EXISTS",
					"EXIT",
					"EXTERNAL",
					"EXTRACT",
					"FALSE",
					"FETCH",
					"FILE",
					"FILLFACTOR",
					"FIRST",
					"FLOAT",
					"FOR",
					"FOREIGN",
					"FORTRAN",
					"FOUND",
					"FREE",
					"FREETEXT",
					"FREETEXTTABLE",
					"FROM",
					"FULL",
					"FUNCTION",
					"GENERAL",
					"GET",
					"GLOBAL",
					"GO",
					"GOTO",
					"GRANT",
					"GROUP",
					"GROUPING",
					"HAVING",
					"HOLDLOCK",
					"HOST",
					"HOUR",
					"IDENTITY",
					"IDENTITYCOL",
					"IDENTITY_INSERT",
					"IF",
					"IGNORE",
					"IMMEDIATE",
					"IN",
					"INCLUDE",
					"INDEX",
					"INDICATOR",
					"INITIALIZE",
					"INITIALLY",
					"INNER",
					"INOUT",
					"INPUT",
					"INSENSITIVE",
					"INSERT",
					"INT",
					"INTEGER",
					"INTERSECT",
					"INTERVAL",
					"INTO",
					"IS",
					"ISOLATION",
					"ITERATE",
					"JOIN",
					"KEY",
					"KILL",
					"LANGUAGE",
					"LARGE",
					"LAST",
					"LATERAL",
					"LEADING",
					"LEFT",
					"LESS",
					"LEVEL",
					"LIKE",
					"LIMIT",
					"LINENO",
					"LOAD",
					"LOCAL",
					"LOCALTIME",
					"LOCALTIMESTAMP",
					"LOCATOR",
					"LOWER",
					"MAP",
					"MATCH",
					"MAX",
					"MIN",
					"MINUTE",
					"MODIFIES",
					"MODIFY",
					"MODULE",
					"MONTH",
					"NAMES",
					"NATIONAL",
					"NATURAL",
					"NCHAR",
					"NCLOB",
					"NEW",
					"NEXT",
					"NO",
					"NOCHECK",
					"NONCLUSTERED",
					"NONE",
					"NOT",
					"NULL",
					"NULLIF",
					"NUMERIC",
					"OBJECT",
					"OCTET_LENGTH",
					"OF",
					"OFF",
					"OFFSETS",
					"OLD",
					"ON",
					"ONLY",
					"OPEN",
					"OPENDATASOURCE",
					"OPENQUERY",
					"OPENROWSET",
					"OPENXML",
					"OPERATION",
					"OPTION",
					"OR",
					"ORDER",
					"ORDINALITY",
					"OUT",
					"OUTER",
					"OUTPUT",
					"OVER",
					"OVERLAPS",
					"PAD",
					"PARAMETER",
					"PARAMETERS",
					"PARTIAL",
					"PASCAL",
					"PATH",
					"PERCENT",
					"PLAN",
					"POSITION",
					"POSTFIX",
					"PRECISION",
					"PREFIX",
					"PREORDER",
					"PREPARE",
					"PRESERVE",
					"PRIMARY",
					"PRINT",
					"PRIOR",
					"PRIVILEGES",
					"PROC",
					"PROCEDURE",
					"PUBLIC",
					"RAISERROR",
					"READ",
					"READS",
					"READTEXT",
					"REAL",
					"RECONFIGURE",
					"RECURSIVE",
					"REF",
					"REFERENCES",
					"REFERENCING",
					"RELATIVE",
					"REPLICATION",
					"RESTORE",
					"RESTRICT",
					"RESULT",
					"RETURN",
					"RETURNS",
					"REVOKE",
					"RIGHT",
					"ROLE",
					"ROLLBACK",
					"ROLLUP",
					"ROUTINE",
					"ROW",
					"ROWCOUNT",
					"ROWGUIDCOL",
					"ROWS",
					"RULE",
					"SAVE",
					"SAVEPOINT",
					"SCHEMA",
					"SCOPE",
					"SCROLL",
					"SEARCH",
					"SECOND",
					"SECTION",
					"SELECT",
					"SEQUENCE",
					"SESSION",
					"SESSION_USER",
					"SET",
					"SETS",
					"SETUSER",
					"SHUTDOWN",
					"SIZE",
					"SMALLINT",
					"SOME",
					"SPACE",
					"SPECIFIC",
					"SPECIFICTYPE",
					"SQL",
					"SQLCA",
					"SQLCODE",
					"SQLERROR",
					"SQLEXCEPTION",
					"SQLSTATE",
					"SQLWARNING",
					"START",
					"STATE",
					"STATEMENT",
					"STATIC",
					"STATISTICS",
					"STRUCTURE",
					"SUBSTRING",
					"SUM",
					"SYSTEM_USER",
					"TABLE",
					"TEMPORARY",
					"TERMINATE",
					"TEXTSIZE",
					"THAN",
					"THEN",
					"TIME",
					"TIMESTAMP",
					"TIMEZONE_HOUR",
					"TIMEZONE_MINUTE",
					"TO",
					"TOP",
					"TRAILING",
					"TRAN",
					"TRANSACTION",
					"TRANSLATE",
					"TRANSLATION",
					"TREAT",
					"TRIGGER",
					"TRIM",
					"TRUE",
					"TRUNCATE",
					"TSEQUAL",
					"UNDER",
					"UNION",
					"UNIQUE",
					"UNKNOWN",
					"UNNEST",
					"UPDATE",
					"UPDATETEXT",
					"UPPER",
					"USAGE",
					"USE",
					"USER",
					"USING",
					"VALUE",
					"VALUES",
					"VARCHAR",
					"VARIABLE",
					"VARYING",
					"VIEW",
					"WAITFOR",
					"WHEN",
					"WHENEVER",
					"WHERE",
					"WHILE",
					"WITH",
					"WITHOUT",
					"WORK",
					"WRITE",
					"WRITETEXT",
					"YEAR",
					"ZONE"
				});
			}
		}
		public override string SQLCommandLimit (List<Mapper> a, string b, int c, int d)
		{
			if (c == 0 && d > 0) {
				var e = b.IndexOf ("SELECT", 0, StringComparison.InvariantCultureIgnoreCase);
				var f = b.IndexOf ("FROM", 0, StringComparison.InvariantCultureIgnoreCase);
				if (e >= 0) {
					e += "SELECT".Length;
					if (f > e && b.IndexOf ("DISTINC", e, f - e, StringComparison.InvariantCultureIgnoreCase) < 0)
						return b.Substring (0, e) + " TOP " + d + " " + b.Substring (e);
				}
			}
			string g = "";
			string[] h = null;
			string[] i = null;
			if (b.IndexOf ("ORDER BY", 0, StringComparison.OrdinalIgnoreCase) < 0) {
				if (a == null)
					throw new GDAException ("On MSQL 2000 for paging is required at least one ordered field");
				Mapper j = a.Find (delegate (Mapper k) {
					return (k.ParameterType == PersistenceParameterType.IdentityKey || k.ParameterType == PersistenceParameterType.Key);
				});
				if (j == null)
					j = a [0];
				var e = b.IndexOf ("SELECT", 0, StringComparison.OrdinalIgnoreCase);
				var f = b.IndexOf ("FROM", 0, StringComparison.OrdinalIgnoreCase);
				if (e >= 0 && f >= "SELECT".Length + 1) {
					var l = b.Substring (e + "SELECT".Length, f - (e + "SELECT".Length));
					if (l.IndexOf ("*", 0) < 0 && l.IndexOf (j.Name, 0, StringComparison.OrdinalIgnoreCase) < 0) {
						b = b.Substring (0, f) + ", " + QuoteExpression (j.Name) + " " + b.Substring (f);
					}
				}
				var m = j.Name.LastIndexOf ('.');
				if (m >= 0)
					b += " ORDER BY " + QuoteExpression (j.Name.Substring (m + 1));
				else
					b += " ORDER BY " + QuoteExpression (j.Name);
			}
			int n = b.IndexOf ("ORDER BY", 0, StringComparison.OrdinalIgnoreCase);
			if (n >= 0) {
				g = b.Substring (n + "ORDER BY".Length, b.Length - (n + "ORDER BY".Length));
				g = g.Trim ('\r', '\n');
				h = g.Split (',');
				i = new string[h.Length];
				for (int o = 0; o < h.Length; o++) {
					int p = 0;
					if (h [o].TrimEnd (' ').EndsWith (" DESC", StringComparison.OrdinalIgnoreCase)) {
						p = h [o].TrimEnd (' ').LastIndexOf (" DESC", StringComparison.OrdinalIgnoreCase);
						i [o] = "DESC";
						h [o] = h [o].Substring (0, p).Trim ();
					}
					else {
						i [o] = "ASC";
						p = h [o].IndexOf (" ASC", 0, StringComparison.OrdinalIgnoreCase);
						if (p >= 0)
							h [o] = h [o].Substring (0, p).Trim ();
						else
							h [o] = h [o].Trim ();
					}
					int q = h [o].LastIndexOf ('.');
					if (q >= 0)
						h [o] = h [o].Substring (q + 1);
				}
			}
			if (MsSqlDialect == MsSqlProviderDialects.MsSql2005) {
				if (h.Length > 0) {
					string[] r = new string[h.Length];
					for (int o = 0; o < h.Length; o++)
						r [o] = h [o] + " " + i [o];
					g = string.Join (", ", r);
				}
				int s = b.IndexOf ("SELECT", 0, StringComparison.OrdinalIgnoreCase);
				b = "SELECT * FROM (\r\n" + b.Substring (0, s) + string.Format ("SELECT *, [!RowNum] = ROW_NUMBER() OVER (ORDER BY {0}) FROM (", g) + ((n - s) > 0 ? b.Substring (s, n - s) : b.Substring (s)) + string.Format ("\r\n) tmp1) tmp2 WHERE [!RowNum] > {0}{1}", c, (d > 0 ? string.Format (" AND [!RowNum] <= {0}", c + d) : ""));
				if (n >= 0) {
					b += " ORDER BY ";
					for (int o = 0; o < h.Length; o++) {
						s = h [o].IndexOf ('.');
						b += (s > 0 ? h [o].Substring (s + 1) : h [o]) + " " + i [o];
						if ((o + 1) != h.Length)
							b += ", ";
					}
				}
				return b;
			}
			else {
				int s = b.IndexOf ("SELECT", 0, StringComparison.OrdinalIgnoreCase);
				int t = b.IndexOf ("FROM", 0, StringComparison.OrdinalIgnoreCase);
				bool u = false;
				if (s >= 0 && t > s) {
					int v = b.Substring (s, t - s).IndexOf ("DISTINCT ", 0, StringComparison.OrdinalIgnoreCase);
					if (v >= 0) {
						u = true;
						b = b.Substring (0, v) + b.Substring (v + "DISTINCT ".Length);
					}
				}
				b = String.Format ("SELECT{0}TOP {1}", u ? " DISTINCT " : " ", (c + d).ToString ()) + b.Substring (s + "SELECT".Length);
				b = String.Format ("SELECT * FROM (SELECT TOP {0} * FROM ({1}) AS inner_tbl", d, b);
				if (n >= 0) {
					b += " ORDER BY ";
					for (int o = 0; o < h.Length; o++) {
						b += h [o] + " " + ((i [o] == "ASC") ? "DESC" : "ASC");
						if ((o + 1) != h.Length)
							b += ", ";
					}
				}
				b += ") AS outer_tbl";
				if (n >= 0) {
					b += " ORDER BY ";
					for (int o = 0; o < h.Length; o++) {
						b += h [o] + " " + i [o];
						if ((o + 1) != h.Length)
							b += ", ";
					}
				}
				return b;
			}
		}
		public override bool SupportSQLCommandLimit {
			get {
				return true;
			}
		}
		public override string QuoteExpressionBegin {
			get {
				return "[";
			}
		}
		public override string QuoteExpressionEnd {
			get {
				return "]";
			}
		}
		public override long GetDbType (Type a)
		{
			SqlDbType b = SqlDbType.Int;
			if (a.Equals (typeof(int)) || a.IsEnum)
				b = SqlDbType.Int;
			else if (a.Equals (typeof(long)))
				b = SqlDbType.BigInt;
			else if (a.Equals (typeof(double)))
				b = SqlDbType.Float;
			else if (a.Equals (typeof(DateTime)))
				b = SqlDbType.DateTime;
			else if (a.Equals (typeof(bool)))
				b = SqlDbType.Bit;
			else if (a.Equals (typeof(string)))
				b = SqlDbType.Text;
			else if (a.Equals (typeof(decimal)))
				b = SqlDbType.Decimal;
			else if (a.Equals (typeof(byte[])))
				b = SqlDbType.Image;
			else if (a.Equals (typeof(Guid)))
				b = SqlDbType.UniqueIdentifier;
			return (long)b;
		}
		public override long GetDbType (string a, bool b)
		{
			switch (a) {
			case "bit":
				return (long)SqlDbType.Bit;
			case "tinyint":
				return (long)SqlDbType.TinyInt;
			case "smallint":
				return (long)SqlDbType.SmallInt;
			case "int":
				return (long)SqlDbType.Int;
			case "bigint":
				return (long)SqlDbType.BigInt;
			case "real":
				return (long)SqlDbType.Real;
			case "float":
				return (long)SqlDbType.Float;
			case "smalldatetime":
				return (long)SqlDbType.SmallDateTime;
			case "datetime":
				return (long)SqlDbType.DateTime;
			case "decimal":
			case "numeric":
				return (long)SqlDbType.Decimal;
			case "nchar":
				return (long)SqlDbType.NChar;
			case "nvarchar":
				return (long)SqlDbType.NVarChar;
			case "ntext":
				return (long)SqlDbType.NText;
			case "char":
				return (long)SqlDbType.Char;
			case "varchar":
				return (long)SqlDbType.VarChar;
			case "text":
				return (long)SqlDbType.Text;
			case "binary":
				return (long)SqlDbType.Binary;
			case "varbinary":
				return (long)SqlDbType.VarBinary;
			case "image":
				return (long)SqlDbType.Image;
			case "uniqueidentifier":
				return (long)SqlDbType.UniqueIdentifier;
			case "sysname":
				return No_DbType;
			case "timestamp":
			default:
				return No_DbType;
			}
		}
		public override Type GetSystemType (long a)
		{
			SqlDbType b = (SqlDbType)Enum.ToObject (typeof(SqlDbType), a);
			switch (b) {
			case SqlDbType.BigInt:
				return typeof(long);
			case SqlDbType.Binary:
				return typeof(byte[]);
			case SqlDbType.Bit:
				return typeof(bool);
			case SqlDbType.Char:
				return typeof(string);
			case SqlDbType.DateTime:
				return typeof(DateTime);
			case SqlDbType.Decimal:
				return typeof(decimal);
			case SqlDbType.Float:
				return typeof(double);
			case SqlDbType.Image:
				return typeof(byte[]);
			case SqlDbType.Int:
				return typeof(int);
			case SqlDbType.Money:
				return typeof(object);
			case SqlDbType.NChar:
				return typeof(string);
			case SqlDbType.NText:
				return typeof(string);
			case SqlDbType.NVarChar:
				return typeof(string);
			case SqlDbType.Real:
				return typeof(double);
			case SqlDbType.SmallDateTime:
				return typeof(DateTime);
			case SqlDbType.SmallInt:
				return typeof(short);
			case SqlDbType.SmallMoney:
				return typeof(object);
			case SqlDbType.Text:
				return typeof(string);
			case SqlDbType.Timestamp:
				return typeof(DateTime);
			case SqlDbType.TinyInt:
				return typeof(byte);
			case SqlDbType.UniqueIdentifier:
				return typeof(Guid);
			case SqlDbType.VarBinary:
				return typeof(byte[]);
			case SqlDbType.VarChar:
				return typeof(string);
			case SqlDbType.Variant:
				return typeof(object);
			default:
				return typeof(object);
			}
		}
		public override char QuoteCharacter {
			get {
				return '\'';
			}
		}
		public override string QuoteExpression (string a)
		{
			if (a != null) {
				var b = a.TrimEnd (' ').TrimStart (' ');
				if (b.Length > 3 && b [0] == '[' && b [b.Length - 1] == ']')
					return a;
			}
			return "[" + a + "]";
		}
		public override bool IsReservedWord (string a)
		{
			return (reservedsWords.BinarySearch (a) >= 0);
		}
	}
}
