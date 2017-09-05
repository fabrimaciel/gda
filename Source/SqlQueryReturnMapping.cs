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
	/// Mapeamento do resultado da consulta.
	/// </summary>
	public class SqlQueryReturnMapping : ElementMapping
	{
		private string _classTypeName;

		private List<SqlQueryReturnPropertyMapping> _returnProperties = new List<SqlQueryReturnPropertyMapping>();

		/// <summary>
		/// Nome do tipo da classe do retorno.
		/// </summary>
		public string ClassTypeName
		{
			get
			{
				return _classTypeName;
			}
			set
			{
				_classTypeName = value;
			}
		}

		/// <summary>
		/// Relação das propriedade do retorno da consulta.
		/// </summary>
		public List<SqlQueryReturnPropertyMapping> ReturnProperties
		{
			get
			{
				return _returnProperties;
			}
		}

		/// <summary>
		/// Constrói uma instancia do mapeamento do retorno da consulta.
		/// </summary>
		/// <param name="element"></param>
		public SqlQueryReturnMapping(XmlElement element)
		{
			if(element == null)
				throw new ArgumentNullException("element");
			ClassTypeName = GetAttributeString(element, "class", false);
			foreach (XmlElement i in element.GetElementsByTagName("return-property"))
			{
				var pm = new SqlQueryReturnPropertyMapping(i);
				if(!ReturnProperties.Exists(f => f.Name == pm.Name))
					ReturnProperties.Add(pm);
			}
		}

		/// <summary>
		/// Constrói uma instancia do mapeamento do retorno da consulta.
		/// </summary>
		/// <param name="classTypeName">Nome do tipo da classe do retorno.</param>
		/// <param name="returnProperties">Propriedades do retorno da consulta.</param>
		public SqlQueryReturnMapping(string classTypeName, IEnumerable<SqlQueryReturnPropertyMapping> returnProperties)
		{
			this.ClassTypeName = classTypeName;
			if(returnProperties != null)
				foreach (var i in returnProperties)
					if(!ReturnProperties.Exists(f => f.Name == i.Name))
						ReturnProperties.Add(i);
		}
	}
}
