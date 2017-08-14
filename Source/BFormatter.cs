using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Globalization;
using System.Collections;
namespace GDA.Helper.Serialization
{
	public class BNonSerializeAttribute : Attribute
	{
	}
	public class SerializableMaxLenghtAttribute : Attribute
	{
		private int _maxLenght;
		public int MaxLenght {
			get {
				return _maxLenght;
			}
		}
		public SerializableMaxLenghtAttribute (int a)
		{
			_maxLenght = a;
		}
	}
	internal class PropertyInfoComparer : IComparer<PropertyInfo>
	{
		public int Compare (PropertyInfo a, PropertyInfo b)
		{
			return Comparer.Default.Compare (a.Name, b.Name);
		}
	}
	internal class FieldInfoComparer : IComparer<FieldInfo>
	{
		public int Compare (FieldInfo a, FieldInfo b)
		{
			return Comparer.Default.Compare (a.Name, b.Name);
		}
	}
	public class BFormatter
	{
		private readonly static Type[] coreTypes =  {
			typeof(byte[]),
			typeof(byte),
			typeof(bool),
			typeof(char),
			typeof(double),
			typeof(short),
			typeof(int),
			typeof(long),
			typeof(float),
			typeof(ushort),
			typeof(uint),
			typeof(ulong),
			typeof(string),
			typeof(DateTime)
		};
		private static List<Type> fullSerializeTypes = new List<Type> (new Type[] {
			typeof(Guid)
		});
		internal class InfoCoreSupport
		{
			public InfoCoreSupport (bool a, bool b)
			{
				this.coreTypeSupported = a;
				this.allowNullValue = b;
				fieldInfo = null;
				propertyInfo = null;
			}
			public bool coreTypeSupported;
			public bool allowNullValue;
			public FieldInfo fieldInfo;
			public PropertyInfo propertyInfo;
			public int maxLenght;
			public object GetValue (object a)
			{
				if (fieldInfo != null)
					return fieldInfo.GetValue (a);
				else if (propertyInfo != null)
					return propertyInfo.GetValue (a, null);
				else
					throw new InvalidOperationException ();
			}
			public Type GetMemberType ()
			{
				if (fieldInfo != null)
					return fieldInfo.FieldType;
				else if (propertyInfo != null)
					return propertyInfo.PropertyType;
				else
					throw new InvalidOperationException ();
			}
		}
		public static bool IsStruct (Type a)
		{
			return a.IsValueType && !a.IsPrimitive && a.BaseType == typeof(ValueType);
		}
		private static bool IsCoreType (Type a)
		{
			foreach (Type t in coreTypes)
				if (t == a)
					return true;
			return false;
		}
		private static InfoCoreSupport Support (Type a)
		{
			InfoCoreSupport b = new InfoCoreSupport (false, false);
			if (a.Name == "Nullable`1") {
				b.allowNullValue = true;
				a = Nullable.GetUnderlyingType (a);
			}
			if (a.IsEnum) {
				a = Enum.GetUnderlyingType (a);
			}
			if (a.IsArray && a.GetElementType () != typeof(object)) {
				b.coreTypeSupported = true;
				b.allowNullValue = true;
				return b;
			}
			else if (IsStruct (a)) {
				b.coreTypeSupported = true;
				return b;
			}
			foreach (Type supportedType in BFormatter.coreTypes)
				if (supportedType.IsAssignableFrom (a)) {
					b.coreTypeSupported = true;
					if (!b.allowNullValue && (supportedType.IsAssignableFrom (typeof(string)) || supportedType.IsAssignableFrom (typeof(byte[]))))
						b.allowNullValue = true;
					return b;
				}
			b.allowNullValue = true;
			b.coreTypeSupported = true;
			return b;
		}
		private static void WriteData (Stream a, object b, Type c, int d)
		{
			if (c.IsAssignableFrom (typeof(string)) && b == null)
				b = "";
			if (c.Name == "Nullable`1") {
				c = Nullable.GetUnderlyingType (c);
				if (b == null)
					b = Activator.CreateInstance (c);
			}
			if (c.IsEnum) {
				c = Enum.GetUnderlyingType (c);
				switch (c.Name) {
				case "Int16":
					b = (short)b;
					break;
				case "UInt16":
					b = (ushort)b;
					break;
				case "Int32":
					b = (int)b;
					break;
				case "UInt32":
					b = (uint)b;
					break;
				case "Byte":
					b = (byte)b;
					break;
				default:
					b = (int)b;
					break;
				}
			}
			else if (IsStruct (c) && !c.IsAssignableFrom (typeof(DateTime)) && !c.IsAssignableFrom (typeof(decimal))) {
				SerializeBase (a, null, 0, 0, b);
				return;
			}
			if (c.IsAssignableFrom (typeof(string))) {
				string e = (string)b;
				int f = 0;
				int g = e.Length;
				if (d > 0) {
					g = (g > d ? d : g);
					if (d < byte.MaxValue)
						f = 1;
					else if (d < ushort.MaxValue)
						f = sizeof(ushort);
					else if (d < int.MaxValue)
						f = sizeof(uint);
				}
				else {
					f = sizeof(ushort);
					g = e.Length;
				}
				a.Write (BitConverter.GetBytes (g), 0, f);
				a.Write (Encoding.Default.GetBytes (e), 0, g);
			}
			else if (c.IsAssignableFrom (typeof(DateTime))) {
				a.Write (BitConverter.GetBytes (((DateTime)b).Ticks), 0, sizeof(long));
			}
			else if (c.IsAssignableFrom (typeof(decimal))) {
				a.Write (DecimalToBytes (Convert.ToDecimal (b)), 0, sizeof(decimal));
			}
			else if (c.IsAssignableFrom (typeof(byte))) {
				a.WriteByte ((byte)b);
			}
			else if (c.IsAssignableFrom (typeof(byte[]))) {
				byte[] h = (byte[])b;
				int f = 0;
				int g = h.Length;
				if (d > 0) {
					g = (g > d ? d : g);
					if (d < byte.MaxValue)
						f = 1;
					else if (d < ushort.MaxValue)
						f = sizeof(ushort);
					else if (d < int.MaxValue)
						f = sizeof(uint);
				}
				else {
					f = sizeof(ushort);
					g = h.Length;
				}
				a.Write (BitConverter.GetBytes (g), 0, f);
				a.Write (h, 0, g);
			}
			else if (c.IsArray) {
				SerializeBase (a, null, 0, d, b);
			}
			else {
				int f = 0;
				if (c.IsAssignableFrom (typeof(bool)))
					f = sizeof(bool);
				else if (c.IsAssignableFrom (typeof(char)))
					f = sizeof(char);
				else if (!Array.Exists (coreTypes, delegate (Type i) {
					return i == c;
				})) {
					SerializeBase (a, null, 0, 0, b);
					return;
				}
				else {
					try {
						f = System.Runtime.InteropServices.Marshal.SizeOf (c);
					}
					catch (ArgumentException) {
						var h = BFormatter.Serialize (b);
						a.Write (BitConverter.GetBytes (h.Length), 0, sizeof(int));
						a.Write (h, 0, h.Length);
						return;
					}
				}
				MethodInfo j = typeof(BitConverter).GetMethod ("GetBytes", BindingFlags.Public | BindingFlags.Static, null, new Type[] {
					c
				}, null);
				a.Write ((byte[])j.Invoke (null, BindingFlags.Default, null, new object[] {
					b
				}, CultureInfo.CurrentCulture), 0, f);
			}
		}
		private static object ReadData (Stream a, Type b, int c)
		{
			if (b.Name == "Nullable`1")
				b = Nullable.GetUnderlyingType (b);
			Type d = null;
			if (b.IsEnum) {
				d = b;
				b = Enum.GetUnderlyingType (b);
			}
			else if (IsStruct (b) && !b.IsAssignableFrom (typeof(DateTime)) && !b.IsAssignableFrom (typeof(decimal))) {
				object e = Activator.CreateInstance (b);
				DeserializeBase (a, b, null, 0, 0, e);
				return e;
			}
			int f = 0;
			if (b.IsAssignableFrom (typeof(string))) {
				if (c > 0) {
					if (c < byte.MaxValue)
						f = 1;
					else if (c < ushort.MaxValue)
						f = sizeof(ushort);
					else if (c < int.MaxValue)
						f = sizeof(uint);
				}
				else {
					f = sizeof(ushort);
				}
				byte[] g = new byte[f];
				a.Read (g, 0, f);
				if (c > 0) {
					if (c < byte.MaxValue)
						f = (int)g [0];
					else if (c < ushort.MaxValue)
						f = BitConverter.ToInt16 (g, 0);
					else if (c < int.MaxValue)
						f = BitConverter.ToInt32 (g, 0);
				}
				else
					f = BitConverter.ToUInt16 (g, 0);
			}
			else if (b.IsAssignableFrom (typeof(bool)))
				f = sizeof(bool);
			else if (b.IsAssignableFrom (typeof(DateTime)))
				f = sizeof(long);
			else if (b.IsAssignableFrom (typeof(decimal)))
				f = sizeof(decimal);
			else if (b.IsAssignableFrom (typeof(char)))
				f = sizeof(char);
			else if (b.IsAssignableFrom (typeof(byte)))
				f = sizeof(byte);
			else if (b.IsAssignableFrom (typeof(byte[]))) {
				if (c > 0) {
					if (c < byte.MaxValue)
						f = 1;
					else if (c < ushort.MaxValue)
						f = sizeof(ushort);
					else if (c < int.MaxValue)
						f = sizeof(uint);
				}
				else {
					f = sizeof(ushort);
				}
				byte[] g = new byte[f];
				a.Read (g, 0, f);
				if (c > 0) {
					if (c < byte.MaxValue)
						f = (int)g [0];
					else if (c < ushort.MaxValue)
						f = BitConverter.ToInt16 (g, 0);
					else if (c < int.MaxValue)
						f = BitConverter.ToInt32 (g, 0);
				}
				else
					f = BitConverter.ToUInt16 (g, 0);
				g = new byte[f];
				a.Read (g, 0, f);
				return g;
			}
			else if (!b.IsArray && !Array.Exists (coreTypes, delegate (Type h) {
				return h == b;
			})) {
				return DeserializeBase (a, b, null, 0, c, null);
			}
			else {
				try {
					f = System.Runtime.InteropServices.Marshal.SizeOf (b);
				}
				catch (ArgumentException) {
					byte[] i = new byte[sizeof(int)];
					a.Read (i, 0, sizeof(int));
					f = BitConverter.ToInt16 (i, 0);
					i = new byte[f];
					a.Read (i, 0, f);
					return BFormatter.Deserialize (i, b);
				}
			}
			if (b.IsArray) {
				return DeserializeBase (a, b, null, 0, c, null);
			}
			else {
				byte[] i = new byte[f];
				a.Read (i, 0, f);
				if (b.IsAssignableFrom (typeof(string)))
					return Encoding.Default.GetString (i, 0, f);
				else if (b.IsAssignableFrom (typeof(byte[])))
					return i;
				else if (b.IsAssignableFrom (typeof(byte)))
					return i [0];
				else if (b.IsAssignableFrom (typeof(DateTime)))
					return new DateTime (BitConverter.ToInt64 (i, 0));
				else if (b.IsAssignableFrom (typeof(decimal)))
					return BytesToDecimal (i);
				else {
					MethodInfo j = typeof(BitConverter).GetMethod ("To" + b.Name);
					if (d != null) {
						return Enum.ToObject (d, j.Invoke (null, new object[] {
							i,
							0
						}));
					}
					else
						return j.Invoke (null, new object[] {
							i,
							0
						});
				}
			}
		}
		private static void Export (Stream a, InfoCoreSupport[] b, short c, object d)
		{
			int e = (int)a.Position;
			int f = Convert.ToInt32 (Math.Ceiling (c / 8.0d));
			byte[] g = new byte[f];
			a.Write (g, 0, g.Length);
			bool[] h = new bool[c];
			int i = 0;
			foreach (InfoCoreSupport sp in b) {
				object j = sp.GetValue (d);
				if (sp.allowNullValue)
					h [i++] = (j == null);
				if (!sp.allowNullValue || j != null)
					WriteData (a, j, sp.GetMemberType (), sp.maxLenght);
			}
			int k = 0, l = 0;
			for (i = 0; i < f; i++) {
				g [i] = 0x00;
				l = 0;
				for (; (l < 8) && (k < (f * 8)) && k < c; k++) {
					if (h [k]) {
						g [i] = (byte)(g [i] | (byte)Convert.ToInt32 (Math.Pow (2.0d, (double)l)));
					}
					l++;
				}
			}
			int m = (int)a.Position;
			a.Seek (e, SeekOrigin.Begin);
			a.Write (g, 0, g.Length);
			a.Seek (m, SeekOrigin.Begin);
		}
		private static void Import (Stream a, InfoCoreSupport[] b, short c, object d)
		{
			int e = Convert.ToInt32 (Math.Ceiling (c / 8.0d));
			bool[] f = new bool[c];
			if (e > 0) {
				byte[] g = new byte[e];
				a.Read (g, 0, e);
				int h, i = 0;
				for (int j = 0; j < e; j++) {
					h = 0;
					for (; h < 8 && h < c; h++) {
						f [i++] = (((g [j] >> h) % 2) != 0);
					}
					c -= 8;
				}
			}
			c = 0;
			object[] k;
			int l = 0;
			for (int j = 0; j < b.Length; j++) {
				if (b [j].fieldInfo != null) {
					if (b [j].allowNullValue && f [c++]) {
						b [j].fieldInfo.SetValue (d, null);
						continue;
					}
					k = b [j].fieldInfo.GetCustomAttributes (typeof(SerializableMaxLenghtAttribute), true);
					if (k.Length > 0)
						l = (int)((SerializableMaxLenghtAttribute)k [0]).MaxLenght;
					else
						l = 0;
					b [j].fieldInfo.SetValue (d, ReadData (a, b [j].fieldInfo.FieldType, l));
				}
				else {
					if (b [j].allowNullValue && f [c++]) {
						b [j].propertyInfo.SetValue (d, null, null);
						continue;
					}
					k = b [j].propertyInfo.GetCustomAttributes (typeof(SerializableMaxLenghtAttribute), true);
					if (k.Length > 0)
						l = (int)((SerializableMaxLenghtAttribute)k [0]).MaxLenght;
					else
						l = 0;
					b [j].propertyInfo.SetValue (d, ReadData (a, b [j].propertyInfo.PropertyType, l), null);
				}
			}
		}
		internal static InfoCoreSupport[] LoadTypeInformation (Type a, out short b)
		{
			short c = 0;
			List<InfoCoreSupport> d = new List<InfoCoreSupport> ();
			object[] e;
			int f = 0;
			InfoCoreSupport g;
			List<FieldInfo> h = new List<FieldInfo> (fullSerializeTypes.Exists (delegate (Type i) {
				return i == a;
			}) ? a.GetFields (BindingFlags.NonPublic | BindingFlags.Instance) : a.GetFields ());
			h.Sort (0, h.Count, new FieldInfoComparer ());
			foreach (FieldInfo fi in h) {
				e = fi.GetCustomAttributes (typeof(BNonSerializeAttribute), true);
				if (e.Length > 0)
					continue;
				e = fi.GetCustomAttributes (typeof(SerializableMaxLenghtAttribute), true);
				if (e.Length > 0)
					f = (int)((SerializableMaxLenghtAttribute)e [0]).MaxLenght;
				else
					f = 0;
				g = Support (fi.FieldType);
				if (g.coreTypeSupported && !((fi.Attributes & FieldAttributes.Static) == FieldAttributes.Static)) {
					if (g.allowNullValue)
						c++;
					g.fieldInfo = fi;
					g.maxLenght = f;
					d.Add (g);
				}
			}
			List<PropertyInfo> j = new List<PropertyInfo> (fullSerializeTypes.Exists (delegate (Type i) {
				return i == a;
			}) ? a.GetProperties (BindingFlags.NonPublic | BindingFlags.Instance) : a.GetProperties ());
			j.Sort (0, j.Count, new PropertyInfoComparer ());
			foreach (PropertyInfo pi in j) {
				if (pi.GetSetMethod () == null)
					continue;
				e = pi.GetCustomAttributes (typeof(BNonSerializeAttribute), true);
				if (e.Length > 0)
					continue;
				e = pi.GetCustomAttributes (typeof(SerializableMaxLenghtAttribute), true);
				if (e.Length > 0)
					f = (int)((SerializableMaxLenghtAttribute)e [0]).MaxLenght;
				else
					f = 0;
				g = Support (pi.PropertyType);
				if (g.coreTypeSupported) {
					if (g.allowNullValue)
						c++;
					g.propertyInfo = pi;
					g.maxLenght = f;
					d.Add (g);
				}
			}
			b = c;
			return d.ToArray ();
		}
		internal static object DeserializeBase (Stream a, Type b, InfoCoreSupport[] c, short d, int e, object f)
		{
			bool g = b.IsArray;
			if (g)
				b = b.GetElementType ();
			bool h = IsCoreType (b);
			if (h && !g) {
				if (a.Length > 0)
					return ReadData (a, b, 0);
				else
					return null;
			}
			if (!b.IsArray && f == null) {
				ConstructorInfo i = b.GetConstructor (new Type[] {
				});
				if (i != null)
					f = i.Invoke (null);
				else
					f = Activator.CreateInstance (b);
			}
			if (g) {
				int j = ReadArrayLenght (a, e);
				byte[] k = new byte[sizeof(int)];
				Array l = Array.CreateInstance (b, j);
				for (int m = 0; m < j; m++) {
					if (h) {
						l.SetValue (ReadData (a, b, 0), m);
					}
					else {
						a.Read (k, 0, sizeof(int));
						int n = BitConverter.ToInt32 (k, 0);
						l.SetValue (DeserializeBase (a, b, c, d, 0, null), m);
					}
				}
				return l;
			}
			else {
				if (c == null)
					c = LoadTypeInformation (b, out d);
				Import (a, c, d, f);
				return f;
			}
		}
		internal static void SerializeBase (Stream a, InfoCoreSupport[] b, short c, int d, object e)
		{
			if (e == null)
				throw new ArgumentException ("graph");
			Type f = e.GetType ();
			bool g = f.IsArray;
			if (g) {
				f = f.GetElementType ();
			}
			bool h = IsCoreType (f);
			if (h && !g) {
				if (e != null)
					WriteData (a, e, f, 0);
				return;
			}
			if (b == null) {
				if (!h)
					b = LoadTypeInformation (f, out c);
				else
					b = new InfoCoreSupport[0];
			}
			if (g) {
				Array i = (Array)e;
				int j = 0;
				int k = i.Length;
				if (d > 0) {
					k = (k > d ? d : k);
					if (d < byte.MaxValue)
						j = 1;
					else if (d < ushort.MaxValue)
						j = sizeof(ushort);
					else if (d < int.MaxValue)
						j = sizeof(uint);
				}
				else
					j = sizeof(ushort);
				a.Write (BitConverter.GetBytes (k), 0, j);
				for (int l = 0; l < k; l++) {
					object m = i.GetValue (l);
					if (h) {
						WriteData (a, m, f, 0);
					}
					else {
						int n = (int)a.Position;
						a.Write (new byte[sizeof(int)], 0, sizeof(int));
						Export (a, b, c, m);
						int o = (int)a.Position;
						a.Seek (n, SeekOrigin.Begin);
						a.Write (BitConverter.GetBytes ((int)(o - (n + sizeof(int)))), 0, sizeof(int));
						a.Seek (o, SeekOrigin.Begin);
					}
				}
				return;
			}
			else
				Export (a, b, c, e);
		}
		public static void RegisterFullSerializeType (Type a)
		{
			if (!fullSerializeTypes.Exists (delegate (Type b) {
				return b == a;
			}))
				fullSerializeTypes.Add (a);
		}
		public static int ReadArrayLenght (Stream a, int b)
		{
			int c = 0;
			if (b > 0) {
				if (b <= byte.MaxValue)
					c = 1;
				else if (b <= ushort.MaxValue)
					c = sizeof(ushort);
				else if (b <= int.MaxValue)
					c = sizeof(uint);
			}
			else
				c = sizeof(int);
			byte[] d = new byte[c];
			a.Read (d, 0, c);
			if (b > 0) {
				if (b <= byte.MaxValue)
					c = (int)d [0];
				else if (b <= ushort.MaxValue)
					c = BitConverter.ToInt16 (d, 0);
				else if (b <= int.MaxValue)
					c = BitConverter.ToInt32 (d, 0);
			}
			else
				c = BitConverter.ToInt32 (d, 0);
			return c;
		}
		public static byte[] Serialize (object a)
		{
			if (a == null)
				throw new ArgumentException ("graph");
			byte[] b = null;
			using (MemoryStream c = new MemoryStream ()) {
				Serialize (c, a);
				c.Seek (0, SeekOrigin.Begin);
				b = new byte[c.Length];
				c.Read (b, 0, b.Length);
			}
			return b;
		}
		public static void Serialize (Stream a, object b)
		{
			SerializeBase (a, null, 0, 0, b);
		}
		public static object Deserialize (Stream a, Type b)
		{
			return DeserializeBase (a, b, null, 0, 0, null);
		}
		public static object Deserialize (byte[] a, Type b)
		{
			using (Stream c = new MemoryStream (a, 0, a.Length)) {
				return Deserialize (c, b);
			}
		}
		public static object Deserialize (byte[] a, Type b, object c)
		{
			using (Stream d = new MemoryStream (a, 0, a.Length)) {
				return DeserializeBase (d, b, null, 0, 0, c);
			}
		}
		public static T CopyInstance<T> (T a) where T : new()
		{
			T b = new T ();
			CopyInstance<T> (a, b);
			return b;
		}
		public static void CopyInstance<T> (T a, T b) where T : new()
		{
			if (a == null)
				throw new ArgumentException ("source");
			using (MemoryStream c = new MemoryStream ()) {
				Serialize (c, a);
				c.Seek (0, SeekOrigin.Begin);
				DeserializeBase (c, typeof(T), null, 0, 0, b);
			}
		}
		private static decimal BytesToDecimal (byte[] a)
		{
			int[] b = new int[4];
			b [0] = ((a [0] | (a [1] << 8)) | (a [2] << 0x10)) | (a [3] << 0x18);
			b [1] = ((a [4] | (a [5] << 8)) | (a [6] << 0x10)) | (a [7] << 0x18);
			b [2] = ((a [8] | (a [9] << 8)) | (a [10] << 0x10)) | (a [11] << 0x18);
			b [3] = ((a [12] | (a [13] << 8)) | (a [14] << 0x10)) | (a [15] << 0x18);
			return new decimal (b);
		}
		private static byte[] DecimalToBytes (decimal a)
		{
			byte[] b = new byte[16];
			int[] c = decimal.GetBits (a);
			int d = c [0];
			int e = c [1];
			int f = c [2];
			int g = c [3];
			b [0] = (byte)d;
			b [1] = (byte)(d >> 8);
			b [2] = (byte)(d >> 0x10);
			b [3] = (byte)(d >> 0x18);
			b [4] = (byte)e;
			b [5] = (byte)(e >> 8);
			b [6] = (byte)(e >> 0x10);
			b [7] = (byte)(e >> 0x18);
			b [8] = (byte)f;
			b [9] = (byte)(f >> 8);
			b [10] = (byte)(f >> 0x10);
			b [11] = (byte)(f >> 0x18);
			b [12] = (byte)g;
			b [13] = (byte)(g >> 8);
			b [14] = (byte)(g >> 0x10);
			b [15] = (byte)(g >> 0x18);
			return b;
		}
	}
}
