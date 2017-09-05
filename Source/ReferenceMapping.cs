﻿/* 
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
	/// Representa um referencia de mapeamento.
	/// </summary>
	public class ReferenceMapping : ElementMapping, IEquatable<ReferenceMapping>
	{
		/// <summary>
		/// Nome do assembly onde o mapeamento está inserido.
		/// </summary>
		public string AssemblyName
		{
			get;
			set;
		}

		/// <summary>
		/// Nome do resource do mapeamento.
		/// </summary>
		public string ResourceName
		{
			get;
			set;
		}

		/// <summary>
		/// Arquivo aonde o mapemento está inserido.
		/// </summary>
		public string FileName
		{
			get;
			set;
		}

		/// <summary>
		/// Cria uma instancia com base nos dados contidos no elemento informado.
		/// </summary>
		/// <param name="element"></param>
		public ReferenceMapping(XmlElement element)
		{
			AssemblyName = GetAttributeString(element, "assemblyName");
			ResourceName = GetAttributeString(element, "resourceName");
			FileName = GetAttributeString(element, "fileName");
		}

		public bool Equals(ReferenceMapping other)
		{
			if(other == null)
				return false;
			return this.AssemblyName == other.AssemblyName && this.FileName == other.FileName && this.ResourceName == other.ResourceName;
		}
	}
}
