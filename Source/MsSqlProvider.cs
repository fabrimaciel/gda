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
using System.Data.SqlClient;
using System.Data;

namespace GDA.Provider.MsSql
{
	/// <summary>
	/// Provide da base de dados Microsoft SqlServer.
	/// </summary>
	public class MsSqlProvider : Provider
	{
		private static System.Collections.Generic.List<string> reservedsWords;

		/// <summary>
		/// Dialeto padrão do provedor.
		/// </summary>
		private MsSqlProviderDialects _msSqlDialect = MsSqlProviderDialects.MsSql2000;

		/// <summary>
		/// Dialeto do Provider.
		/// </summary>
		public MsSqlProviderDialects MsSqlDialect
		{
			get
			{
				return _msSqlDialect;
			}
			set
			{
				_msSqlDialect = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public MsSqlProvider() : base("MsSql", typeof(SqlConnection), typeof(SqlDataAdapter), typeof(SqlCommand), typeof(SqlParameter), "@", "select @@IDENTITY;", true)
		{
			if(reservedsWords == null)
			{
				reservedsWords = new List<string>(new string[] {
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

		/// <summary>
		/// Métodos responsável por tratar o comando sql e adapta-lo para executar
		/// estruturas de paginação.
		/// </summary>
		/// <param name="sqlQuery">Clausula SQL que será tratada.</param>
		/// <param name="startRecord">Apartir da qual deve começar o resultado.</param>
		/// <param name="size">Quantidade de registros por resultado.</param>
		/// <returns>Consulta sql tratada</returns>
		public override string SQLCommandLimit(List<Mapper> mapping, string sqlQuery, int startRecord, int size)
		{
			if(startRecord == 0 && size > 0)
			{
				var selectIndex = sqlQuery.IndexOf("SELECT", 0, StringComparison.InvariantCultureIgnoreCase);
				var fromIndex = sqlQuery.IndexOf("FROM", 0, StringComparison.InvariantCultureIgnoreCase);
				if(selectIndex >= 0)
				{
					selectIndex += "SELECT".Length;
					if(fromIndex > selectIndex && sqlQuery.IndexOf("DISTINC", selectIndex, fromIndex - selectIndex, StringComparison.InvariantCultureIgnoreCase) < 0)
						return sqlQuery.Substring(0, selectIndex) + " TOP " + size + " " + sqlQuery.Substring(selectIndex);
				}
			}
			string order = "";
			string[] fieldOrderBy = null;
			string[] directionFieldOrderBy = null;
			if(sqlQuery.IndexOf("ORDER BY", 0, StringComparison.OrdinalIgnoreCase) < 0)
			{
				if(mapping == null)
					throw new GDAException("On MSQL 2000 for paging is required at least one ordered field");
				Mapper field = mapping.Find(delegate(Mapper m) {
					return (m.ParameterType == PersistenceParameterType.IdentityKey || m.ParameterType == PersistenceParameterType.Key);
				});
				if(field == null)
					field = mapping[0];
				var selectIndex = sqlQuery.IndexOf("SELECT", 0, StringComparison.OrdinalIgnoreCase);
				var fromIndex = sqlQuery.IndexOf("FROM", 0, StringComparison.OrdinalIgnoreCase);
				if(selectIndex >= 0 && fromIndex >= "SELECT".Length + 1)
				{
					var columnsPart = sqlQuery.Substring(selectIndex + "SELECT".Length, fromIndex - (selectIndex + "SELECT".Length));
					if(columnsPart.IndexOf("*", 0) < 0 && columnsPart.IndexOf(field.Name, 0, StringComparison.OrdinalIgnoreCase) < 0)
					{
						sqlQuery = sqlQuery.Substring(0, fromIndex) + ", " + QuoteExpression(field.Name) + " " + sqlQuery.Substring(fromIndex);
					}
				}
				var lastIndex = field.Name.LastIndexOf('.');
				if(lastIndex >= 0)
					sqlQuery += " ORDER BY " + QuoteExpression(field.Name.Substring(lastIndex + 1));
				else
					sqlQuery += " ORDER BY " + QuoteExpression(field.Name);
			}
			int orderBy = sqlQuery.IndexOf("ORDER BY", 0, StringComparison.OrdinalIgnoreCase);
			if(orderBy >= 0)
			{
				order = sqlQuery.Substring(orderBy + "ORDER BY".Length, sqlQuery.Length - (orderBy + "ORDER BY".Length));
				order = order.Trim('\r', '\n');
				fieldOrderBy = order.Split(',');
				directionFieldOrderBy = new string[fieldOrderBy.Length];
				for(int i = 0; i < fieldOrderBy.Length; i++)
				{
					int posDi = 0;
					if(fieldOrderBy[i].TrimEnd(' ').EndsWith(" DESC", StringComparison.OrdinalIgnoreCase))
					{
						posDi = fieldOrderBy[i].TrimEnd(' ').LastIndexOf(" DESC", StringComparison.OrdinalIgnoreCase);
						directionFieldOrderBy[i] = "DESC";
						fieldOrderBy[i] = fieldOrderBy[i].Substring(0, posDi).Trim();
					}
					else
					{
						directionFieldOrderBy[i] = "ASC";
						posDi = fieldOrderBy[i].IndexOf(" ASC", 0, StringComparison.OrdinalIgnoreCase);
						if(posDi >= 0)
							fieldOrderBy[i] = fieldOrderBy[i].Substring(0, posDi).Trim();
						else
							fieldOrderBy[i] = fieldOrderBy[i].Trim();
					}
					int dotPos = fieldOrderBy[i].LastIndexOf('.');
					if(dotPos >= 0)
						fieldOrderBy[i] = fieldOrderBy[i].Substring(dotPos + 1);
				}
			}
			if(MsSqlDialect == MsSqlProviderDialects.MsSql2005)
			{
				if(fieldOrderBy.Length > 0)
				{
					string[] orderParts = new string[fieldOrderBy.Length];
					for(int i = 0; i < fieldOrderBy.Length; i++)
						orderParts[i] = fieldOrderBy[i] + " " + directionFieldOrderBy[i];
					order = string.Join(", ", orderParts);
				}
				int pos = sqlQuery.IndexOf("SELECT", 0, StringComparison.OrdinalIgnoreCase);
				sqlQuery = "SELECT * FROM (\r\n" + sqlQuery.Substring(0, pos) + string.Format("SELECT *, [!RowNum] = ROW_NUMBER() OVER (ORDER BY {0}) FROM (", order) + ((orderBy - pos) > 0 ? sqlQuery.Substring(pos, orderBy - pos) : sqlQuery.Substring(pos)) + string.Format("\r\n) tmp1) tmp2 WHERE [!RowNum] > {0}{1}", startRecord, (size > 0 ? string.Format(" AND [!RowNum] <= {0}", startRecord + size) : ""));
				if(orderBy >= 0)
				{
					sqlQuery += " ORDER BY ";
					for(int i = 0; i < fieldOrderBy.Length; i++)
					{
						pos = fieldOrderBy[i].IndexOf('.');
						sqlQuery += (pos > 0 ? fieldOrderBy[i].Substring(pos + 1) : fieldOrderBy[i]) + " " + directionFieldOrderBy[i];
						if((i + 1) != fieldOrderBy.Length)
							sqlQuery += ", ";
					}
				}
				return sqlQuery;
			}
			else
			{
				int pos = sqlQuery.IndexOf("SELECT", 0, StringComparison.OrdinalIgnoreCase);
				int fromPos = sqlQuery.IndexOf("FROM", 0, StringComparison.OrdinalIgnoreCase);
				bool hasDisctint = false;
				if(pos >= 0 && fromPos > pos)
				{
					int distinctPos = sqlQuery.Substring(pos, fromPos - pos).IndexOf("DISTINCT ", 0, StringComparison.OrdinalIgnoreCase);
					if(distinctPos >= 0)
					{
						hasDisctint = true;
						sqlQuery = sqlQuery.Substring(0, distinctPos) + sqlQuery.Substring(distinctPos + "DISTINCT ".Length);
					}
				}
				sqlQuery = String.Format("SELECT{0}TOP {1}", hasDisctint ? " DISTINCT " : " ", (startRecord + size).ToString()) + sqlQuery.Substring(pos + "SELECT".Length);
				sqlQuery = String.Format("SELECT * FROM (SELECT TOP {0} * FROM ({1}) AS inner_tbl", size, sqlQuery);
				if(orderBy >= 0)
				{
					sqlQuery += " ORDER BY ";
					for(int i = 0; i < fieldOrderBy.Length; i++)
					{
						sqlQuery += fieldOrderBy[i] + " " + ((directionFieldOrderBy[i] == "ASC") ? "DESC" : "ASC");
						if((i + 1) != fieldOrderBy.Length)
							sqlQuery += ", ";
					}
				}
				sqlQuery += ") AS outer_tbl";
				if(orderBy >= 0)
				{
					sqlQuery += " ORDER BY ";
					for(int i = 0; i < fieldOrderBy.Length; i++)
					{
						sqlQuery += fieldOrderBy[i] + " " + directionFieldOrderBy[i];
						if((i + 1) != fieldOrderBy.Length)
							sqlQuery += ", ";
					}
				}
				return sqlQuery;
			}
		}

		/// <summary>
		/// Identifica que o provider suporta paginação usando o comando sql.
		/// </summary>
		public override bool SupportSQLCommandLimit
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Quote inicial da expressão.
		/// </summary>
		public override string QuoteExpressionBegin
		{
			get
			{
				return "[";
			}
		}

		/// <summary>
		/// Quote final da expressão.
		/// </summary>
		public override string QuoteExpressionEnd
		{
			get
			{
				return "]";
			}
		}

		/// <summary>
		/// Obtem um número inteiro que corresponde ao tipo da base de dados que representa o tipo
		/// informado. O valor de retorno pode ser convertido em um tipo válido (enum value) para 
		/// o atual provider. Esse method é chamado para traduzir os tipos do sistema para os tipos
		/// do banco de dados que não são convertidos explicitamento.
		/// </summary>
		/// <param name="type">Tipo do sistema.</param>
		/// <returns>Tipo correspondente da base de dados.</returns>
		public override long GetDbType(Type type)
		{
			SqlDbType result = SqlDbType.Int;
			if(type.Equals(typeof(int)) || type.IsEnum)
				result = SqlDbType.Int;
			else if(type.Equals(typeof(long)))
				result = SqlDbType.BigInt;
			else if(type.Equals(typeof(double)))
				result = SqlDbType.Float;
			else if(type.Equals(typeof(DateTime)))
				result = SqlDbType.DateTime;
			else if(type.Equals(typeof(bool)))
				result = SqlDbType.Bit;
			else if(type.Equals(typeof(string)))
				result = SqlDbType.Text;
			else if(type.Equals(typeof(decimal)))
				result = SqlDbType.Decimal;
			else if(type.Equals(typeof(byte[])))
				result = SqlDbType.Image;
			else if(type.Equals(typeof(Guid)))
				result = SqlDbType.UniqueIdentifier;
			return (long)result;
		}

		/// <summary>
		/// Esse método converte a string (extraída da tabelas do banco de dados) para o tipo do system
		/// correspondente.
		/// </summary>
		/// <param name="dbType">Nome do tipo usado no banco de dados.</param>
		/// <param name="isUnsigned">Valor boolean que identifica se o tipo é unsigned.</param>
		/// <returns>Valor do enumerator do tipo correspondente do banco de dados. O retorno é um número
		/// inteiro por causa que em alguns provider o enumerations não seguem o padrão do DbType definido
		/// no System.Data.</returns>
		public override long GetDbType(string dbType, bool isUnsigned)
		{
			switch(dbType)
			{
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

		/// <summary>
		/// Esse método retorna o tipo do sistema correspodente ao tipo specifico indicado no long.
		/// A implementação padrão não retorna exception, mas sim null.
		/// </summary>
		/// <param name="dbType">Tipo especifico do provider.</param>
		/// <returns>Tipo do sistema correspondente.</returns>
		public override Type GetSystemType(long dbType)
		{
			SqlDbType sqlDbType = (SqlDbType)Enum.ToObject(typeof(SqlDbType), dbType);
			switch(sqlDbType)
			{
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

		/// <summary>
		/// Obtem o caracter usado para delimitar os parametros de string.
		/// </summary>
		/// <returns>The quote character.</returns>
		public override char QuoteCharacter
		{
			get
			{
				return '\'';
			}
		}

		/// <summary>
		/// Obtem uma container (Quote) para permitir que colunas com nomes especiais
		/// sejam inseridas na consulta.
		/// </summary>
		/// <param name="word">Nome da coluna ou do paramenrto.</param>
		/// <returns>Expressão com a formatação adequada.</returns>
		public override string QuoteExpression(string word)
		{
			if(word != null)
			{
				var trimWord = word.TrimEnd(' ').TrimStart(' ');
				if(trimWord.Length > 3 && trimWord[0] == '[' && trimWord[trimWord.Length - 1] == ']')
					return word;
			}
			return "[" + word + "]";
		}

		public override bool IsReservedWord(string word)
		{
			return (reservedsWords.BinarySearch(word) >= 0);
		}
	}
}
