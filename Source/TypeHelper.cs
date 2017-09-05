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
using System.Reflection;

namespace GDA.Helper
{
	/// <summary>
	/// Auxilia na manipulação dos tipos.
	/// </summary>
	internal class TypeHelper
	{
		/// <summary>
		/// Verifica se é um tipo Nullable;
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		internal static bool IsNullableType(Type type)
		{
			return (((type != null) && type.IsGenericType) && (type.GetGenericTypeDefinition() == typeof(Nullable<>)));
		}

		/// <summary>
		/// Recupera o tipo do elemento da enumeração.
		/// </summary>
		/// <param name="seqType"></param>
		/// <returns></returns>
		internal static Type GetElementType(Type seqType)
		{
			Type type = FindIEnumerable(seqType);
			if(type == null)
			{
				return seqType;
			}
			return type.GetGenericArguments()[0];
		}

		/// <summary>
		/// Recupera o tipo do elemento da enumeração.
		/// </summary>
		/// <param name="seqType"></param>
		/// <returns></returns>
		private static Type FindIEnumerable(Type seqType)
		{
			if((seqType != null) && (seqType != typeof(string)))
			{
				if(seqType.IsArray)
				{
					return typeof(IEnumerable<>).MakeGenericType(new[] {
						seqType.GetElementType()
					});
				}
				if(seqType.IsGenericType)
				{
					foreach (Type type in seqType.GetGenericArguments())
					{
						Type type2 = typeof(IEnumerable<>).MakeGenericType(new[] {
							type
						});
						if(type2.IsAssignableFrom(seqType))
						{
							return type2;
						}
					}
				}
				Type[] interfaces = seqType.GetInterfaces();
				if((interfaces != null) && (interfaces.Length > 0))
				{
					foreach (Type type3 in interfaces)
					{
						Type type4 = FindIEnumerable(type3);
						if(type4 != null)
						{
							return type4;
						}
					}
				}
				if((seqType.BaseType != null) && (seqType.BaseType != typeof(object)))
				{
					return FindIEnumerable(seqType.BaseType);
				}
			}
			return null;
		}

		/// <summary>
		/// What is the type of the current member?
		/// </summary>
		internal static Type GetMemberType(MemberInfo mi)
		{
			FieldInfo info = mi as FieldInfo;
			if(info != null)
				return info.FieldType;
			PropertyInfo info2 = mi as PropertyInfo;
			if(info2 != null)
				return info2.PropertyType;
			EventInfo info3 = mi as EventInfo;
			if(info3 != null)
				return info3.EventHandlerType;
			return null;
		}

		/// <summary>
		/// If mi is a Property, then its sets it value to value
		/// If mi is a Field, it assigns value to it
		/// </summary>
		internal static void SetMemberValue(object instance, MemberInfo mi, object value)
		{
			FieldInfo info = mi as FieldInfo;
			if(info != null)
			{
				info.SetValue(instance, value, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, null);
				return;
			}
			PropertyInfo info2 = mi as PropertyInfo;
			if(info2 != null)
			{
				info2.SetValue(instance, value, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, null, null);
				return;
			}
			throw new NotSupportedException("The member type is not supported!");
		}

		/// <summary>
		/// If mi is a Property, then it returns its value
		/// If mi is a Field, then it returns its value
		/// </summary>
		internal static object GetMemberValue(object instance, MemberInfo mi)
		{
			FieldInfo info = mi as FieldInfo;
			if(info != null)
			{
				return info.GetValue(instance);
			}
			PropertyInfo info2 = mi as PropertyInfo;
			if(info2 != null)
			{
				return info2.GetValue(instance, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, null, null);
			}
			throw new NotSupportedException("The member type is not supported!");
		}
	}
}
