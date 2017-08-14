using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
namespace GDA.Helper
{
	internal class TypeHelper
	{
		internal static bool IsNullableType (Type a)
		{
			return (((a != null) && a.IsGenericType) && (a.GetGenericTypeDefinition () == typeof(Nullable<>)));
		}
		internal static Type GetElementType (Type a)
		{
			Type b = FindIEnumerable (a);
			if (b == null) {
				return a;
			}
			return b.GetGenericArguments () [0];
		}
		private static Type FindIEnumerable (Type a)
		{
			if ((a != null) && (a != typeof(string))) {
				if (a.IsArray) {
					return typeof(IEnumerable<>).MakeGenericType (new[] {
						a.GetElementType ()
					});
				}
				if (a.IsGenericType) {
					foreach (Type type in a.GetGenericArguments ()) {
						Type b = typeof(IEnumerable<>).MakeGenericType (new[] {
							type
						});
						if (b.IsAssignableFrom (a)) {
							return b;
						}
					}
				}
				Type[] c = a.GetInterfaces ();
				if ((c != null) && (c.Length > 0)) {
					foreach (Type type3 in c) {
						Type d = FindIEnumerable (type3);
						if (d != null) {
							return d;
						}
					}
				}
				if ((a.BaseType != null) && (a.BaseType != typeof(object))) {
					return FindIEnumerable (a.BaseType);
				}
			}
			return null;
		}
		internal static Type GetMemberType (MemberInfo a)
		{
			FieldInfo b = a as FieldInfo;
			if (b != null)
				return b.FieldType;
			PropertyInfo c = a as PropertyInfo;
			if (c != null)
				return c.PropertyType;
			EventInfo d = a as EventInfo;
			if (d != null)
				return d.EventHandlerType;
			return null;
		}
		internal static void SetMemberValue (object a, MemberInfo b, object c)
		{
			FieldInfo d = b as FieldInfo;
			if (d != null) {
				d.SetValue (a, c, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, null);
				return;
			}
			PropertyInfo e = b as PropertyInfo;
			if (e != null) {
				e.SetValue (a, c, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, null, null);
				return;
			}
			throw new NotSupportedException ("The member type is not supported!");
		}
		internal static object GetMemberValue (object a, MemberInfo b)
		{
			FieldInfo c = b as FieldInfo;
			if (c != null) {
				return c.GetValue (a);
			}
			PropertyInfo d = b as PropertyInfo;
			if (d != null) {
				return d.GetValue (a, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, null, null);
			}
			throw new NotSupportedException ("The member type is not supported!");
		}
	}
}
