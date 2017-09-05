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
using System.Xml;

namespace GDA.Mapping
{
	public abstract class ElementMapping
	{
		/// <summary>
		/// Recupera o valor da string contida no atributo.
		/// </summary>
		/// <param name="attr"></param>
		/// <returns></returns>
		internal static string GetAttributeString(XmlElement element, string attributeName)
		{
			var attr = element.Attributes[attributeName];
			if(attr != null)
				return attr.Value;
			return null;
		}

		/// <summary>
		/// Recupera o valor da string contida no atributo.
		/// </summary>
		/// <param name="attr"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		internal static string GetAttributeString(XmlElement element, string attributeName, bool requiredValue)
		{
			var attr = element.Attributes[attributeName];
			if((attr == null || string.IsNullOrEmpty(attr.Value)) && requiredValue)
				throw new GDAMappingException("Attribute \"{0}\" is required in \"{1}\"", attributeName, element.Name);
			if(attr == null)
				return null;
			return attr.Value;
		}

		/// <summary>
		/// Recupera o valor da string contida no atributo.
		/// </summary>
		/// <param name="attr"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		internal static string GetAttributeString(XmlElement element, string attributeName, string defaultValue)
		{
			var attr = element.Attributes[attributeName];
			return attr == null || string.IsNullOrEmpty(attr.Value) ? defaultValue : attr.Value;
		}

		/// <summary>
		/// Recupera o primeiro elemento ou o padrão.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="enumerable"></param>
		/// <returns></returns>
		internal static T FirstOrDefault<T>(System.Collections.IEnumerable enumerable) where T : class
		{
			foreach (var i in enumerable)
				return i as T;
			return default(T);
		}

		internal static T Last<T>(IEnumerable<T> enumerable)
		{
			T item = default(T);
			foreach (var i in enumerable)
				item = i;
			return item;
		}
	}
}
