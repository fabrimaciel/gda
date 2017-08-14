using System;
using System.Collections.Generic;
using System.Text;
namespace GDA
{
	public class GDAPropertyValue
	{
		private object _value;
		private bool _valueExists;
		public bool ValueExists {
			get {
				return _valueExists;
			}
		}
		public GDAPropertyValue (object a, bool b)
		{
			_value = a == DBNull.Value ? null : a;
			_valueExists = b;
		}
		public int ToInt32 ()
		{
			if (_value is byte[])
				return BitConverter.ToInt32 ((byte[])_value, 0);
			return Convert.ToInt32 (_value);
		}
		public int? ToInt32Nullable ()
		{
			if (_value == null)
				return null;
			else
				return ToInt32 ();
		}
		public uint ToUInt32 ()
		{
			if (_value is byte[])
				return BitConverter.ToUInt32 ((byte[])_value, 0);
			return Convert.ToUInt32 (_value);
		}
		public uint? ToUInt32Nullable ()
		{
			if (_value == null)
				return null;
			else
				return ToUInt32 ();
		}
		public short ToInt16 ()
		{
			if (_value is byte[])
				return BitConverter.ToInt16 ((byte[])_value, 0);
			return Convert.ToInt16 (_value);
		}
		public short? ToInt16Nullable ()
		{
			if (_value == null)
				return null;
			else
				return ToInt16 ();
		}
		public ushort ToUInt16 ()
		{
			if (_value is byte[])
				return BitConverter.ToUInt16 ((byte[])_value, 0);
			return Convert.ToUInt16 (_value);
		}
		public ushort? ToUInt16Nullable ()
		{
			if (_value == null)
				return null;
			else
				return ToUInt16 ();
		}
		public long ToInt64 ()
		{
			if (_value is byte[])
				return BitConverter.ToInt64 ((byte[])_value, 0);
			return Convert.ToInt64 (_value);
		}
		public long? ToInt64Nullable ()
		{
			if (_value == null)
				return null;
			else
				return ToInt64 ();
		}
		public ulong ToUInt64 ()
		{
			if (_value is byte[])
				return BitConverter.ToUInt64 ((byte[])_value, 0);
			return Convert.ToUInt64 (_value);
		}
		public ulong? ToUInt64Nullable ()
		{
			if (_value == null)
				return null;
			else
				return ToUInt64 ();
		}
		public double ToDouble ()
		{
			if (_value is byte[])
				return BitConverter.ToDouble ((byte[])_value, 0);
			return Convert.ToDouble (_value);
		}
		public double? ToDoubleNullable ()
		{
			if (_value == null)
				return null;
			else
				return ToDouble ();
		}
		public float ToSingle ()
		{
			if (_value is byte[])
				return BitConverter.ToSingle ((byte[])_value, 0);
			return Convert.ToSingle (_value);
		}
		public float? ToSingleNullable ()
		{
			if (_value == null)
				return null;
			else
				return ToSingle ();
		}
		public decimal ToDecimal ()
		{
			return Convert.ToDecimal (_value);
		}
		public decimal? ToDecimalNullable ()
		{
			if (_value == null)
				return null;
			else
				return Convert.ToDecimal (_value);
		}
		public DateTime ToDateTime ()
		{
			return Convert.ToDateTime (_value);
		}
		public DateTime? ToDateTimeNullable ()
		{
			if (_value == null)
				return null;
			else
				return Convert.ToDateTime (_value);
		}
		public bool ToBoolean ()
		{
			if (_value is byte[])
				return BitConverter.ToBoolean ((byte[])_value, 0);
			return Convert.ToBoolean (_value);
		}
		public bool? ToBooleanNullable ()
		{
			if (_value == null)
				return null;
			else
				return ToBoolean ();
		}
		public byte[] ToBytes ()
		{
			return _value as byte[];
		}
		public object GetValue ()
		{
			return _value;
		}
		public object GetValue (Type a)
		{
			switch (a.Name) {
			case "System.Int32":
				return (System.Int32)this;
			case "System.Int32?":
				return (System.Int32?)this;
			case "System.UInt32":
				return (System.UInt32)this;
			case "System.UInt32?":
				return (System.UInt32?)this;
			case "System.Int16":
				return (System.Int16)this;
			case "System.Int16?":
				return (System.Int16?)this;
			case "System.UInt16":
				return (System.UInt16)this;
			case "System.UInt16?":
				return (System.UInt16?)this;
			case "System.Int64":
				return (System.Int64)this;
			case "System.Int64?":
				return (System.Int64?)this;
			case "System.UInt64":
				return (System.UInt64)this;
			case "System.UInt64?":
				return (System.UInt64?)this;
			case "System.Single":
				return (System.Single)this;
			case "System.Single?":
				return (System.Single?)this;
			case "System.Double":
				return (System.Double)this;
			case "System.Double?":
				return (System.Double?)this;
			case "System.Decimal":
				return (System.Decimal)this;
			case "System.Decimal?":
				return (System.Decimal?)this;
			case "System.DateTime":
				return (System.DateTime)this;
			case "System.DateTime?":
				return (System.DateTime?)this;
			case "System.Boolean":
				return (System.Boolean)this;
			case "System.Boolean?":
				return (System.Boolean?)this;
			case "System.Byte[]":
				return (System.Byte[])this;
			case "System.String":
				return (System.String)this;
			}
			return _value;
		}
		public override string ToString ()
		{
			if (_value is byte[])
				return BitConverter.ToString ((byte[])_value, 0);
			return Convert.ToString (_value);
		}
		public static implicit operator int (GDAPropertyValue a) {
			if (a == null)
				throw new ArgumentNullException ("value", "The instance is null.");
			return a.ToInt32 ();
		}
		public static implicit operator int? (GDAPropertyValue value) {
			if (value == null)
				throw new ArgumentNullException ("value", "The instance is null.");
			return value.ToInt32Nullable ();
		}
		public static implicit operator uint (GDAPropertyValue value) {
			if (value == null)
				throw new ArgumentNullException ("value", "The instance is null.");
			return value.ToUInt32 ();
		}
		public static implicit operator uint? (GDAPropertyValue value) {
			if (value == null)
				throw new ArgumentNullException ("value", "The instance is null.");
			return value.ToUInt32Nullable ();
		}
		public static implicit operator short (GDAPropertyValue value) {
			if (value == null)
				throw new ArgumentNullException ("value", "The instance is null.");
			return value.ToInt16 ();
		}
		public static implicit operator short? (GDAPropertyValue value) {
			if (value == null)
				throw new ArgumentNullException ("value", "The instance is null.");
			return value.ToInt16Nullable ();
		}
		public static implicit operator ushort (GDAPropertyValue value) {
			if (value == null)
				throw new ArgumentNullException ("value", "The instance is null.");
			return value.ToUInt16 ();
		}
		public static implicit operator ushort? (GDAPropertyValue value) {
			if (value == null)
				throw new ArgumentNullException ("value", "The instance is null.");
			return value.ToUInt16Nullable ();
		}
		public static implicit operator long (GDAPropertyValue value) {
			if (value == null)
				throw new ArgumentNullException ("value", "The instance is null.");
			return value.ToInt64 ();
		}
		public static implicit operator long? (GDAPropertyValue value) {
			if (value == null)
				throw new ArgumentNullException ("value", "The instance is null.");
			return value.ToInt64Nullable ();
		}
		public static implicit operator ulong (GDAPropertyValue value) {
			if (value == null)
				throw new ArgumentNullException ("value", "The instance is null.");
			return value.ToUInt64 ();
		}
		public static implicit operator ulong? (GDAPropertyValue value) {
			if (value == null)
				throw new ArgumentNullException ("value", "The instance is null.");
			return value.ToUInt64Nullable ();
		}
		public static implicit operator float (GDAPropertyValue value) {
			if (value == null)
				throw new ArgumentNullException ("value", "The instance is null.");
			return value.ToSingle ();
		}
		public static implicit operator float? (GDAPropertyValue value) {
			if (value == null)
				throw new ArgumentNullException ("value", "The instance is null.");
			return value.ToSingleNullable ();
		}
		public static implicit operator double (GDAPropertyValue value) {
			if (value == null)
				throw new ArgumentNullException ("value", "The instance is null.");
			return value.ToDouble ();
		}
		public static implicit operator double? (GDAPropertyValue value) {
			if (value == null)
				throw new ArgumentNullException ("value", "The instance is null.");
			return value.ToDoubleNullable ();
		}
		public static implicit operator decimal (GDAPropertyValue value) {
			if (value == null)
				throw new ArgumentNullException ("value", "The instance is null.");
			return value.ToDecimal ();
		}
		public static implicit operator decimal? (GDAPropertyValue value) {
			if (value == null)
				throw new ArgumentNullException ("value", "The instance is null.");
			return value.ToDecimalNullable ();
		}
		public static implicit operator DateTime (GDAPropertyValue value) {
			if (value == null)
				throw new ArgumentNullException ("value", "The instance is null.");
			return value.ToDateTime ();
		}
		public static implicit operator DateTime? (GDAPropertyValue value) {
			if (value == null)
				throw new ArgumentNullException ("value", "The instance is null.");
			return value.ToDateTimeNullable ();
		}
		public static implicit operator bool (GDAPropertyValue value) {
			if (value == null)
				throw new ArgumentNullException ("value", "The instance is null.");
			return value.ToBoolean ();
		}
		public static implicit operator bool? (GDAPropertyValue value) {
			if (value == null)
				throw new ArgumentNullException ("value", "The instance is null.");
			return value.ToBooleanNullable ();
		}
		public static implicit operator byte[] (GDAPropertyValue value) {
			if (value == null)
				throw new ArgumentNullException ("value", "The instance is null.");
			return value.ToBytes ();
		}
		public static implicit operator string (GDAPropertyValue value) {
			if (value == null)
				throw new ArgumentNullException ("value", "The instance is null.");
			return value.ToString ();
		}
	}
}
