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
	/// <summary>
	/// Armazena os dados do mapeamento de uma parametro do GDA.
	/// </summary>
	public class SqlQueryParameterMapping : ElementMapping
	{
		private string _name;

		private string _typeName;

		private string _defaultValue;

		/// <summary>
		/// Nome do parametro.
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}

		/// <summary>
		/// Nome do tipo do parametro no sistema.
		/// </summary>
		public string TypeName
		{
			get
			{
				return _typeName;
			}
			set
			{
				_typeName = value;
			}
		}

		/// <summary>
		/// Valor padrão do parametro.
		/// </summary>
		public string DefaultValue
		{
			get
			{
				return _defaultValue;
			}
			set
			{
				_defaultValue = value;
			}
		}

		/// <summary>
		/// Constrói uma instancia do mapeamento do parametro da consulta.
		/// </summary>
		/// <param name="element"></param>
		public SqlQueryParameterMapping(XmlElement element)
		{
			if(element == null)
				throw new ArgumentNullException("element");
			Name = GetAttributeString(element, "name", true);
			TypeName = GetAttributeString(element, "type", true);
			DefaultValue = GetAttributeString(element, "defaultValue", false);
			ValidateTypeName();
		}

		/// <summary>
		/// Constrói uma instancia do mapeamento do parametro da consulta.
		/// </summary>
		/// <param name="name">Nome do parametro.</param>
		/// <param name="typeName">Nome do tipo dao parametro.</param>
		/// <param name="defaultValue">Valor padrão do parametro.</param>
		public SqlQueryParameterMapping(string name, string typeName, string defaultValue)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException("name");
			else if(string.IsNullOrEmpty(typeName))
				throw new ArgumentNullException("typeName");
			this.Name = name;
			this.TypeName = typeName;
			this.DefaultValue = defaultValue;
		}

		/// <summary>
		/// Valida o nome do tipo do parametro.
		/// </summary>
		private void ValidateTypeName()
		{
			if(!string.IsNullOrEmpty(TypeName))
				switch(TypeName.ToLower())
				{
				case "int32":
				case "int":
					TypeName = "System.Int32";
					break;
				case "int16":
				case "short":
					TypeName = "System.Int16";
					break;
				case "int65":
				case "long":
					TypeName = "System.Int32";
					break;
				case "float":
				case "single":
					TypeName = "System.Single";
					break;
				case "double":
					TypeName = "System.Double";
					break;
				case "datetime":
					TypeName = "System.DateTime";
					break;
				case "string":
					TypeName = "System.String";
					break;
				case "guid":
					TypeName = "System.Guid";
					break;
				}
		}
	}
}
