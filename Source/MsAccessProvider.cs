using System;
using System.Collections.Generic;
using System.Text;
using System.Data.OleDb;
namespace GDA.Provider.MsAccess
{
	public class MsAccessProvider : Provider
	{
		public MsAccessProvider () : base ("MsAccess", typeof(OleDbConnection), typeof(OleDbDataAdapter), typeof(OleDbCommand), typeof(OleDbParameter), "@", "select @@IDENTITY;", true)
		{
			base.ExecuteCommandsOneAtATime = true;
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
		public override string SQLCommandLimit (List<Mapper> a, string b, int c, int d)
		{
			string e = "";
			string[] f = null;
			string[] g = null;
			if (b.IndexOf ("ORDER BY", 0, StringComparison.OrdinalIgnoreCase) < 0) {
				if (a == null)
					throw new GDAException ("Mapping not found");
				Mapper h = a.Find (delegate (Mapper i) {
					return (i.ParameterType == PersistenceParameterType.IdentityKey || i.ParameterType == PersistenceParameterType.Key);
				});
				if (h == null) {
					h = a [0];
				}
				b += " ORDER BY " + QuoteExpression (h.Name);
			}
			int j = b.IndexOf ("ORDER BY", 0, StringComparison.OrdinalIgnoreCase);
			if (j >= 0) {
				e = b.Substring (j + "ORDER BY".Length, b.Length - (j + "ORDER BY".Length));
				f = e.Split (',');
				g = new string[f.Length];
				for (int k = 0; k < f.Length; k++) {
					int l = f [k].IndexOf (" DESC", 0, StringComparison.OrdinalIgnoreCase);
					if (l >= 0) {
						g [k] = "DESC";
						f [k] = f [k].Substring (0, l).Trim ();
					}
					else {
						g [k] = "ASC";
						l = f [k].IndexOf (" ASC", 0, StringComparison.OrdinalIgnoreCase);
						if (l >= 0)
							f [k] = f [k].Substring (0, l).Trim ();
						else
							f [k] = f [k].Trim ();
					}
				}
			}
			int m = b.IndexOf ("SELECT", 0, StringComparison.OrdinalIgnoreCase);
			b = String.Format ("SELECT TOP {0}", (c + d).ToString ()) + b.Substring (m + "SELECT".Length);
			b = String.Format ("SELECT * FROM (SELECT TOP {0} * FROM ({1}) AS inner_tbl", d, b);
			if (j >= 0) {
				b += " ORDER BY ";
				for (int k = 0; k < f.Length; k++) {
					b += f [k] + " " + ((g [k] == "ASC") ? "DESC" : "ASC");
					if ((k + 1) != f.Length)
						b += ", ";
				}
			}
			b += ") AS outer_tbl";
			if (j >= 0) {
				b += " ORDER BY ";
				for (int k = 0; k < f.Length; k++) {
					b += f [k] + " " + g [k];
					if ((k + 1) != f.Length)
						b += ", ";
				}
			}
			return b;
		}
		public override bool SupportSQLCommandLimit {
			get {
				return true;
			}
		}
		public override char QuoteCharacter {
			get {
				return '\'';
			}
		}
		public override string QuoteExpression (string a)
		{
			return "[" + a + "]";
		}
		public override long GetDbType (Type a)
		{
			if (a.IsEnum) {
				switch (Enum.GetUnderlyingType (a).Name) {
				case "Int16":
					return (long)OleDbType.SmallInt;
				case "UInt16":
					return (long)OleDbType.SmallInt;
				case "Int32":
					return (long)OleDbType.Integer;
				case "UInt32":
					return (long)OleDbType.Integer;
				case "Byte":
					return (long)OleDbType.Char;
				}
			}
			if (a == typeof(string))
				return (long)OleDbType.VarChar;
			else if (a == typeof(int) || a == typeof(uint))
				return (long)OleDbType.Integer;
			else if (a == typeof(short) || a == typeof(ushort))
				return (long)OleDbType.SmallInt;
			else if (a == typeof(bool))
				return (long)OleDbType.Boolean;
			else if (a == typeof(DateTime))
				return (long)OleDbType.Date;
			else if (a == typeof(char))
				return (long)OleDbType.Char;
			else if (a == typeof(decimal))
				return (long)OleDbType.Decimal;
			else if (a == typeof(double))
				return (long)OleDbType.Double;
			else if (a == typeof(float))
				return (long)OleDbType.Single;
			else if (a == typeof(byte[]))
				return (long)OleDbType.Binary;
			else if (a == typeof(Guid))
				return (long)OleDbType.Guid;
			else if (a == typeof(byte))
				return (long)OleDbType.Char;
			return (long)OleDbType.Empty;
		}
		public override Type GetSystemType (long a)
		{
			switch (a) {
			case (long)OleDbType.Boolean:
				return typeof(bool);
			case (long)OleDbType.Char:
				return typeof(byte);
			case (long)OleDbType.SmallInt:
				return typeof(Int16);
			case (long)OleDbType.Integer:
				return typeof(Int32);
			case (long)OleDbType.Single:
				return typeof(float);
			case (long)OleDbType.Date:
				return typeof(DateTime);
			case (long)OleDbType.Decimal:
				return typeof(decimal);
			case (long)OleDbType.VarChar:
				return typeof(string);
			case (long)OleDbType.Binary:
				return typeof(byte[]);
			default:
				return typeof(object);
			}
		}
	}
}
