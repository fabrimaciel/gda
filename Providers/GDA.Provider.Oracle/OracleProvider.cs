using System;
using System.Collections.Generic;
using System.Text;
using GDA.Interfaces;
using System.Reflection;
using Oracle.DataAccess.Client;
namespace GDA.Provider.Oracle
{
	public class OracleProvider : Provider, IParameterConverter
	{
		public OracleProvider () : base ("OracleProvider", "Oracle.DataAccess.dll", "Oracle.DataAccess.Client.OracleConnection", "Oracle.DataAccess.Client.OracleDataAdapter", "Oracle.DataAccess.Client.OracleCommand", "Oracle.DataAccess.Client.OracleParameter", ":", true, "")
		{
			base.ExecuteCommandsOneAtATime = true;
			string a = Assembly.GetExecutingAssembly ().EscapedCodeBase;
			Uri b = new Uri (a);
			string c = b.IsFile ? System.IO.Path.GetDirectoryName (b.LocalPath) : null;
			providerAssembly = Assembly.LoadFrom (c + "\\Oracle.DataAccess.dll");
		}
		public override string SqlQueryReturnIdentity {
			get {
				return "SELECT {0}.currval FROM dual;";
			}
		}
		public override char QuoteCharacter {
			get {
				return '"';
			}
		}
		public override string QuoteExpressionBegin {
			get {
				return "\"";
			}
		}
		public override string QuoteExpressionEnd {
			get {
				return "\"";
			}
		}
		public override string QuoteExpression (string a)
		{
			string[] b = a.Split ('.');
			string c = "";
			for (int d = 0; d < b.Length; d++)
				c += "\"" + b [d] + "\"" + ((d + 1) != b.Length ? "." : "");
			return c;
		}
		public override bool SupportSQLCommandLimit {
			get {
				return true;
			}
		}
		public override string ParameterPrefix {
			get {
				return ":";
			}
		}
		public override string GetIdentitySelect (string a, string b)
		{
			string c = (a + "_seq").ToUpper ();
			return String.Format (SqlQueryReturnIdentity, c);
		}
		public override long GetDbType (Type a)
		{
			OracleDbType b = OracleDbType.Int32;
			if (a.Equals (typeof(byte)) || a.Equals (typeof(Byte)))
				b = OracleDbType.Byte;
			else if (a.Equals (typeof(short)) || a.Equals (typeof(Int16)))
				b = OracleDbType.Int16;
			else if (a.Equals (typeof(int)) || a.Equals (typeof(Int32)) || a.IsEnum)
				b = OracleDbType.Int32;
			else if (a.Equals (typeof(long)) || a.Equals (typeof(Int64)))
				b = OracleDbType.Int64;
			else if (a.Equals (typeof(float)) || a.Equals (typeof(Single)))
				b = OracleDbType.Double;
			else if (a.Equals (typeof(double)))
				b = OracleDbType.Double;
			else if (a.Equals (typeof(decimal)) || a.Equals (typeof(Decimal)))
				b = OracleDbType.Decimal;
			else if (a.Equals (typeof(DateTime)))
				b = OracleDbType.Date;
			else if (a.Equals (typeof(bool)))
				b = OracleDbType.Byte;
			else if (a.Equals (typeof(string)))
				b = OracleDbType.Varchar2;
			else if (a.Equals (typeof(TimeSpan)))
				b = OracleDbType.IntervalDS;
			else if (a.Equals (typeof(byte[])))
				b = OracleDbType.Blob;
			else
				throw new GDAException ("Unsupported Property Type");
			return (long)b;
		}
		public override Type GetSystemType (long a)
		{
			switch (a) {
			case (long)OracleDbType.Byte:
				return typeof(bool);
			case (long)OracleDbType.Int16:
				return typeof(Int16);
			case (long)OracleDbType.Int32:
				return typeof(Int32);
			case (long)OracleDbType.Int64:
			case (long)OracleDbType.Long:
				return typeof(Int64);
			case (long)OracleDbType.Single:
				return typeof(float);
			case (long)OracleDbType.Double:
				return typeof(double);
			case (long)OracleDbType.Date:
			case (long)OracleDbType.TimeStamp:
			case (long)OracleDbType.TimeStampLTZ:
			case (long)OracleDbType.TimeStampTZ:
				return typeof(DateTime);
			case (long)OracleDbType.Decimal:
				return typeof(decimal);
			case (long)OracleDbType.NVarchar2:
			case (long)OracleDbType.Varchar2:
			case (long)OracleDbType.NChar:
			case (long)OracleDbType.Char:
			case (long)OracleDbType.Clob:
			case (long)OracleDbType.NClob:
			case (long)OracleDbType.XmlType:
				return typeof(string);
			case (long)OracleDbType.Raw:
			case (long)OracleDbType.LongRaw:
			case (long)OracleDbType.Blob:
			case (long)OracleDbType.BFile:
				return typeof(byte[]);
			case (long)OracleDbType.IntervalDS:
				return typeof(TimeSpan);
			default:
				return typeof(object);
			}
		}
		public override long GetDbType (string a, bool b)
		{
			string c = a.ToLower ();
			switch (c) {
			case "bfile":
				return (long)OracleDbType.BFile;
			case "blob":
				return (long)OracleDbType.Blob;
			case "byte":
				return (long)OracleDbType.Byte;
			case "char":
				return (long)OracleDbType.Char;
			case "clob":
				return (long)OracleDbType.Clob;
			case "date":
			case "datetime":
				return (long)OracleDbType.Date;
			case "decimal":
			case "number":
				return (long)OracleDbType.Decimal;
			case "double":
			case "float":
				return (long)OracleDbType.Double;
			case "int16":
				return (long)OracleDbType.Int16;
			case "int32":
				return (long)OracleDbType.Int32;
			case "int64":
				return (long)OracleDbType.Int64;
			case "intervalds":
			case "intervaldaytosecond":
			case "interval day to second":
				return (long)OracleDbType.IntervalDS;
			case "intervalym":
			case "intervalyeartomonth":
			case "interval year to month":
				return (long)OracleDbType.IntervalYM;
			case "long":
				return (long)OracleDbType.Long;
			case "longraw":
			case "long raw":
				return (long)OracleDbType.LongRaw;
			case "nchar":
				return (long)OracleDbType.NChar;
			case "nclob":
				return (long)OracleDbType.NClob;
			case "nvarchar":
			case "nvarchar2":
				return (long)OracleDbType.NVarchar2;
			case "raw":
				return (long)OracleDbType.Raw;
			case "cursor":
			case "ref cursor":
			case "refcursor":
				return (long)OracleDbType.RefCursor;
			case "single":
				return (long)OracleDbType.Single;
			case "timestamp":
				return (long)OracleDbType.TimeStamp;
			case "timestamplocal":
			case "timestamp with local time zone":
			case "timestampltz":
				return (long)OracleDbType.TimeStampLTZ;
			case "timestampwithtz":
			case "timestamp with time zone":
			case "timestamptz":
				return (long)OracleDbType.TimeStampTZ;
			case "varchar":
			case "varchar2":
				return (long)OracleDbType.Varchar2;
			case "xmltype":
				return (long)OracleDbType.XmlType;
			case "rowid":
				return (long)OracleDbType.Varchar2;
			default:
				return No_DbType;
			}
		}
		public override void SetParameterValue (System.Data.IDbDataParameter a, object b)
		{
			if (b != null && b.GetType ().IsEnum)
				b = (int)b;
			base.SetParameterValue (a, b);
		}
		public System.Data.IDbDataParameter Convert (GDAParameter a)
		{
			var b = this.CreateParameter ();
			b.DbType = a.DbType;
			if (b.Direction != a.Direction)
				b.Direction = a.Direction;
			b.Size = a.Size;
			try {
				if (a.ParameterName [0] == '?')
					b.ParameterName = ParameterPrefix + a.ParameterName.Substring (1) + ParameterSuffix;
				else
					b.ParameterName = a.ParameterName;
			}
			catch (Exception ex) {
				throw new GDAException ("Error on convert parameter name '" + a.ParameterName + "'.", ex);
			}
			SetParameterValue (b, a.Value == null ? DBNull.Value : a.Value);
			return b;
		}
	}
}
