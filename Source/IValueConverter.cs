using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace GDA
{
	public class ValueConverterManager : IEnumerable<IValueConverter>
	{
		private List<IValueConverter> _converters = new List<IValueConverter> ();
		private static ValueConverterManager _instance;
		public static ValueConverterManager Instance {
			get {
				return _instance;
			}
		}
		public int Count {
			get {
				return _converters.Count;
			}
		}
		public IValueConverter this [int a] {
			get {
				return _converters [a];
			}
			set {
				_converters [a] = value;
			}
		}
		static ValueConverterManager ()
		{
			_instance = new ValueConverterManager ();
		}
		internal ValueConverterManager ()
		{
			_converters.Add (new GDAValueConverter ());
		}
		public object Convert (object a, Type b, System.Globalization.CultureInfo c)
		{
			foreach (var converter in _converters) {
				if (converter.CanConvert (a, b, c))
					return converter.Convert (a, b, c);
			}
			return a;
		}
		public void Add (IValueConverter a)
		{
			_converters.Add (a);
		}
		public bool Remove (IValueConverter a)
		{
			return _converters.Remove (a);
		}
		public IEnumerator<IValueConverter> GetEnumerator ()
		{
			return _converters.GetEnumerator ();
		}
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
		{
			return _converters.GetEnumerator ();
		}
	}
	public interface IValueConverter
	{
		bool CanConvert (object a, Type b, System.Globalization.CultureInfo c);
		object Convert (object a, Type b, System.Globalization.CultureInfo c);
	}
	public class GDAValueConverter : IValueConverter
	{
		public bool CanConvert (object a, Type b, System.Globalization.CultureInfo c)
		{
			if (a == null)
				return false;
			var d = a.GetType ();
			if (GDA.Helper.TypeHelper.IsNullableType (b)) {
				if (a == null)
					return false;
				b = Nullable.GetUnderlyingType (b);
			}
			if (b.IsEnum) {
				if (a is string || b == typeof(long) || a is uint || a is ushort || a is ulong || a is short || a is int || a is long || a is byte)
					return true;
			}
			else if (b == typeof(bool))
				return true;
			else if (b == typeof(uint) && d == typeof(int))
				return true;
			else if (b == typeof(ushort) && d == typeof(short))
				return true;
			else if (b == typeof(DateTimeOffset)) {
				if (d == typeof(DateTime))
					return true;
			}
			else if (b == typeof(Guid)) {
				if (d == typeof(string) || d != typeof(Guid))
					return true;
			}
			else if (b != d) {
				if (b == typeof(int))
					return true;
				else if (b == typeof(uint))
					return true;
				else if (d == typeof(decimal)) {
					if (b == typeof(float))
						return true;
					else if (b == typeof(double))
						return true;
					else if (b == typeof(int))
						return true;
					else if (b == typeof(short))
						return true;
					else if (b == typeof(long))
						return true;
				}
				else if (b == typeof(decimal)) {
					if (d == typeof(float))
						return true;
					else if (d == typeof(double))
						return true;
					else if (d == typeof(int))
						return true;
					else if (d == typeof(short))
						return true;
					else if (d == typeof(long))
						return true;
				}
				else if (d == typeof(byte[])) {
					switch (b.Name) {
					case "Int16":
					case "Int32":
					case "Int64":
					case "UInt16":
					case "UInt32":
					case "UInt64":
					case "Single":
					case "String":
					case "Double":
					case "Boolean":
					case "Char":
						return true;
					}
				}
			}
			return false;
		}
		public object Convert (object a, Type b, System.Globalization.CultureInfo c)
		{
			if (a == null)
				return null;
			var d = a.GetType ();
			bool e = false;
			if (GDA.Helper.TypeHelper.IsNullableType (b)) {
				if (a == null)
					return null;
				e = true;
				b = Nullable.GetUnderlyingType (b);
			}
			if (b.IsEnum) {
				var f = Enum.GetUnderlyingType (b);
				var g = a;
				if (a is string) {
					g = Enum.Parse (b, (string)a, true);
				}
				else if (d == typeof(long))
					g = (int)(long)a;
				else if (d == typeof(uint)) {
					if (f == typeof(int))
						g = (int)(uint)a;
					else if (f == typeof(short))
						g = (short)(uint)a;
				}
				else if (d == typeof(ushort)) {
					if (f == typeof(int))
						g = (int)(ushort)a;
					else if (f == typeof(short))
						g = (short)(ushort)a;
				}
				if (e)
					try {
						return Activator.CreateInstance (typeof(Nullable<>).MakeGenericType (b), Enum.ToObject (b, g));
					}
					catch (System.Reflection.TargetInvocationException ex) {
						throw ex.InnerException;
					}
				else
					return g;
			}
			else if (b == typeof(bool))
				return System.Convert.ToBoolean (a);
			else if (b == typeof(uint) && d == typeof(int))
				return uint.Parse (a.ToString ());
			else if (b == typeof(ushort) && d == typeof(short))
				return ushort.Parse (a.ToString ());
			else if (b == typeof(DateTimeOffset)) {
				if (d == typeof(DateTime))
					return new DateTimeOffset ((DateTime)a);
			}
			else if (b == typeof(Guid)) {
				if (d == typeof(string))
					return new Guid ((string)a);
				else if (d != typeof(Guid))
					return new Guid (a.ToString ());
			}
			else if (b != d) {
				if (b == typeof(int))
					return System.Convert.ToInt32 (a);
				else if (b == typeof(uint))
					return System.Convert.ToUInt32 (a);
				else if (d == typeof(decimal)) {
					if (b == typeof(float))
						return decimal.ToSingle ((decimal)a);
					else if (b == typeof(double))
						return decimal.ToDouble ((decimal)a);
					else if (b == typeof(int))
						return decimal.ToInt32 ((decimal)a);
					else if (b == typeof(short))
						return decimal.ToInt16 ((decimal)a);
					else if (b == typeof(long))
						return decimal.ToInt64 ((decimal)a);
				}
				else if (b == typeof(decimal)) {
					if (d == typeof(float))
						return (decimal)(float)a;
					else if (d == typeof(double))
						return (decimal)(double)a;
					else if (d == typeof(int))
						return (decimal)(int)a;
					else if (d == typeof(short))
						return (decimal)(short)a;
					else if (d == typeof(long))
						return (decimal)(long)a;
				}
				else if (b == typeof(bool)) {
					if (a is int)
						return ((int)a) != 0;
					if (a is short)
						return ((short)a) != 0;
					if (a is long)
						return ((long)a) != 0;
				}
				else if (a is uint) {
					if (b == typeof(int))
						return (int)(uint)a;
					else if (b == typeof(short))
						return (short)(uint)a;
					else if (b == typeof(long))
						return (long)(uint)a;
				}
				else if (a is ushort) {
					if (b == typeof(int))
						return (int)(ushort)a;
					else if (b == typeof(short))
						return (short)(ushort)a;
					else if (b == typeof(long))
						return (long)(ushort)a;
				}
				else if (a is ulong) {
					if (b == typeof(int))
						return (int)(ulong)a;
					else if (b == typeof(short))
						return (short)(ulong)a;
					else if (b == typeof(long))
						return (long)(ulong)a;
				}
				else if (d == typeof(byte[])) {
					switch (b.Name) {
					case "Int16":
						return BitConverter.ToInt16 ((byte[])a, 0);
					case "Int32":
						return BitConverter.ToInt32 ((byte[])a, 0);
					case "Int64":
						return BitConverter.ToInt64 ((byte[])a, 0);
					case "UInt16":
						return BitConverter.ToUInt16 ((byte[])a, 0);
					case "UInt32":
						return BitConverter.ToUInt32 ((byte[])a, 0);
					case "UInt64":
						return BitConverter.ToUInt64 ((byte[])a, 0);
					case "Single":
						return BitConverter.ToSingle ((byte[])a, 0);
					case "String":
						return BitConverter.ToString ((byte[])a, 0);
					case "Double":
						return BitConverter.ToDouble ((byte[])a, 0);
					case "Boolean":
						return BitConverter.ToBoolean ((byte[])a, 0);
					case "Char":
						return BitConverter.ToChar ((byte[])a, 0);
					}
				}
			}
			return a;
		}
	}
}
