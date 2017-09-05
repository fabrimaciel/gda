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
	/// Armazena os dados do mapeamento da propriedade de retorno da consulta.
	/// </summary>
	public class SqlQueryReturnPropertyMapping : ElementMapping
	{
		private string _name;

		private string _column;

		/// <summary>
		/// Nome da propriedade do retorno.
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
		/// Nome da coluna do retorno.
		/// </summary>
		public string Column
		{
			get
			{
				return _column;
			}
			set
			{
				_column = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="element"></param>
		public SqlQueryReturnPropertyMapping(XmlElement element)
		{
			if(element == null)
				throw new ArgumentNullException("element");
			Name = GetAttributeString(element, "name", true);
			Column = GetAttributeString(element, "column", false);
		}
	}
}
