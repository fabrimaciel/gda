using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
namespace GDA.Mapping
{
	public abstract class ElementMapping
	{
		internal static string GetAttributeString (XmlElement a, string b)
		{
			var c = a.Attributes [b];
			if (c != null)
				return c.Value;
			return null;
		}
		internal static string GetAttributeString (XmlElement a, string b, bool c)
		{
			var d = a.Attributes [b];
			if ((d == null || string.IsNullOrEmpty (d.Value)) && c)
				throw new GDAMappingException ("Attribute \"{0}\" is required in \"{1}\"", b, a.Name);
			if (d == null)
				return null;
			return d.Value;
		}
		internal static string GetAttributeString (XmlElement a, string b, string c)
		{
			var d = a.Attributes [b];
			return d == null || string.IsNullOrEmpty (d.Value) ? c : d.Value;
		}
		internal static T FirstOrDefault<T> (System.Collections.IEnumerable a) where T : class
		{
			foreach (var i in a)
				return i as T;
			return default(T);
		}
		internal static T Last<T> (IEnumerable<T> a)
		{
			T b = default(T);
			foreach (var i in a)
				b = i;
			return b;
		}
	}
}
