using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
namespace GDA.Common.Helper
{
	public class TypeConverter
	{
		public static object Get (Type a, XmlNode b)
		{
			if (a == typeof(XmlNode))
				return b;
			else
				return Get (a, b.InnerXml);
		}
		public static object Get (Type a, string b)
		{
			if (a.IsEnum) {
				try {
					return Enum.Parse (a, b, true);
				}
				catch {
					return Enum.ToObject (a, Convert.ToInt32 (b));
				}
			}
			if (null != b) {
				return Convert.ChangeType (b, a, System.Globalization.CultureInfo.CurrentCulture);
			}
			return null;
		}
		public static object Get (Type a, object b)
		{
			if (a.IsEnum) {
				try {
					return Enum.Parse (a, b.ToString (), true);
				}
				catch {
					return Enum.ToObject (a, Convert.ToInt32 (b));
				}
			}
			if (null != b) {
				return Convert.ChangeType (b, a, System.Globalization.CultureInfo.GetCultureInfo ("en-US"));
			}
			return null;
		}
		public static bool IsNullAssignable (Type a)
		{
			return !a.IsValueType;
		}
	}
}
