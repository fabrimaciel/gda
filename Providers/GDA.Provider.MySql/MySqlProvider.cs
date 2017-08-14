using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using MySql.Data.MySqlClient;
namespace GDA.Provider.MySql
{
	public class MySqlProvider : global::GDA.Provider.Provider
	{
		public MySqlProvider () : base ("MySqlProvider", "MySql.Data.dll", "MySql.Data.MySqlClient.MySqlConnection", "MySql.Data.MySqlClient.MySqlDataAdapter", "MySql.Data.MySqlClient.MySqlCommand", "MySql.Data.MySqlClient.MySqlParameter", "?", true, "")
		{
			base.ExecuteCommandsOneAtATime = true;
			string a = Assembly.GetExecutingAssembly ().EscapedCodeBase;
			Uri b = new Uri (a);
			string c = b.IsFile ? System.IO.Path.GetDirectoryName (b.LocalPath) : null;
			providerAssembly = Assembly.LoadFrom (c + "\\MySql.Data.dll");
		}
		public override string SqlQueryReturnIdentity {
			get {
				return "select LAST_INSERT_ID()";
			}
		}
		public override char QuoteCharacter {
			get {
				return '`';
			}
		}
		public override string QuoteExpressionBegin {
			get {
				return "`";
			}
		}
		public override string QuoteExpressionEnd {
			get {
				return "`";
			}
		}
		public override string QuoteExpression (string a)
		{
			if (a != null) {
				var b = a.TrimEnd (' ').TrimStart (' ');
				if (b.Length > 3 && b [0] == '[' && b [b.Length - 1] == ']')
					a = a.Substring (1, a.Length - 2);
			}
			return "`" + a + "`";
		}
		public override bool SupportSQLCommandLimit {
			get {
				return true;
			}
		}
		public override string SQLCommandLimit (List<Mapper> a, string b, int c, int d)
		{
			return b + " limit " + c.ToString () + "," + d.ToString ();
		}
		public override long GetDbType (Type a)
		{
			MySqlDbType b = MySqlDbType.Int32;
			if (a.IsEnum) {
				switch (Enum.GetUnderlyingType (a).Name) {
				case "Int16":
					return (long)MySqlDbType.Int16;
				case "UInt16":
					return (long)MySqlDbType.UInt16;
				case "Int32":
					return (long)MySqlDbType.Int32;
				case "UInt32":
					return (long)MySqlDbType.UInt32;
				case "Byte":
					return (long)MySqlDbType.Byte;
				}
			}
			if (a.Equals (typeof(int)))
				b = MySqlDbType.Int32;
			else if (a.Equals (typeof(uint)))
				b = MySqlDbType.UInt32;
			else if (a.Equals (typeof(short)))
				b = MySqlDbType.Int16;
			else if (a.Equals (typeof(ushort)))
				b = MySqlDbType.UInt16;
			else if (a.Equals (typeof(long)))
				b = MySqlDbType.Int64;
			else if (a.Equals (typeof(double)) || a.Equals (typeof(Single)))
				b = MySqlDbType.Double;
			else if (a.Equals (typeof(decimal)))
				b = MySqlDbType.Decimal;
			else if (a.Equals (typeof(DateTime)))
				b = MySqlDbType.Datetime;
			else if (a.Equals (typeof(bool)) || a.Equals (typeof(Byte)) || a.Equals (typeof(byte)))
				b = MySqlDbType.Byte;
			else if (a.Equals (typeof(string)))
				b = MySqlDbType.String;
			else if (a.Equals (typeof(byte[])) || a.Equals (typeof(Byte[]))) {
				b = MySqlDbType.LongBlob;
			}
			return (long)b;
		}
		public override long GetDbType (string a, bool b)
		{
			switch (a) {
			case "byte":
				return (long)MySqlDbType.Byte;
			case "tinyint":
				return (long)MySqlDbType.Bit;
			case "smallint":
				if (b)
					return (long)MySqlDbType.UInt16;
				else
					return (long)MySqlDbType.Int16;
			case "int":
				if (b)
					return (long)MySqlDbType.UInt32;
				else
					return (long)MySqlDbType.Int32;
			case "bigint":
				if (b)
					return (long)MySqlDbType.UInt64;
				else
					return (long)MySqlDbType.Int64;
			case "float":
				return (long)MySqlDbType.Float;
			case "datetime":
				return (long)MySqlDbType.Datetime;
			case "decimal":
			case "numeric":
				return (long)MySqlDbType.Decimal;
			case "char":
				return (long)MySqlDbType.String;
			case "varchar":
				return (long)MySqlDbType.VarChar;
			case "text":
				return (long)MySqlDbType.String;
			case "tinyblob":
				return (long)MySqlDbType.TinyBlob;
			case "blob":
				return (long)MySqlDbType.Blob;
			case "mediumblob":
				return (long)MySqlDbType.MediumBlob;
			case "longblob":
				return (long)MySqlDbType.LongBlob;
			case "enum":
				return (long)MySqlDbType.Enum;
			default:
				return No_DbType;
			}
		}
		public override Type GetSystemType (long a)
		{
			switch (a) {
			case (long)MySqlDbType.Bit:
				return typeof(bool);
			case (long)MySqlDbType.Byte:
				return typeof(byte);
			case (long)MySqlDbType.UInt16:
				return typeof(UInt16);
			case (long)MySqlDbType.Int16:
				return typeof(Int16);
			case (long)MySqlDbType.UInt32:
				return typeof(UInt32);
			case (long)MySqlDbType.Int32:
				return typeof(Int32);
			case (long)MySqlDbType.UInt64:
				return typeof(UInt64);
			case (long)MySqlDbType.Int64:
				return typeof(Int64);
			case (long)MySqlDbType.Float:
				return typeof(float);
			case (long)MySqlDbType.Datetime:
				return typeof(DateTime);
			case (long)MySqlDbType.Decimal:
				return typeof(decimal);
			case (long)MySqlDbType.String:
			case (long)MySqlDbType.VarChar:
				return typeof(string);
			case (long)MySqlDbType.TinyBlob:
			case (long)MySqlDbType.Blob:
			case (long)MySqlDbType.MediumBlob:
			case (long)MySqlDbType.LongBlob:
				return typeof(byte[]);
			case (long)MySqlDbType.Enum:
				return typeof(int);
			default:
				return typeof(object);
			}
		}
	}
}
