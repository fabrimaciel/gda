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

namespace GDA.Mapping
{
	public class TypeInfo
	{
		/// <summary>
		/// Nome do tipo.
		/// </summary>
		public string Name
		{
			get;
			set;
		}

		/// <summary>
		/// Assembly relacionado.
		/// </summary>
		public string Assembly
		{
			get;
			set;
		}

		/// <summary>
		/// Namespace relacionado.
		/// </summary>
		public string Namespace
		{
			get;
			set;
		}

		/// <summary>
		/// Nome completo do tipo
		/// </summary>
		public string FullnameWithAssembly
		{
			get
			{
				return (!string.IsNullOrEmpty(Namespace) ? Namespace + "." : "") + Name + (!string.IsNullOrEmpty(Assembly) ? ", " + Assembly : "");
			}
		}

		/// <summary>
		/// Nome completo do tipo.
		/// </summary>
		public string Fullname
		{
			get
			{
				return (!string.IsNullOrEmpty(Namespace) ? Namespace + "." : "") + Name;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name">Nome do tipo que será processado.</param>
		/// <param name="mappingNamespace">Namespace informado no mapeamento.</param>
		/// <param name="mappingAssembly">Assembly informado no mapeamento.</param>
		public TypeInfo(string name, string mappingNamespace, string mappingAssembly)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException("name");
			if(!string.IsNullOrEmpty(mappingNamespace))
				Namespace = mappingNamespace;
			if(!string.IsNullOrEmpty(mappingAssembly))
				Assembly = mappingAssembly;
			var parts1 = name.Split(',');
			for(int i = 0; i < parts1.Length; i++)
				parts1[i] = parts1[i].Trim();
			var parts2 = parts1[0].Split('.');
			for(int i = 0; i < parts2.Length; i++)
				parts2[i] = parts2[i].Trim();
			if(parts2.Length > 1)
			{
				Name = ElementMapping.Last(parts2);
				var aux = string.Join(".", parts2, 0, parts2.Length - 1);
				if(parts1.Length > 1 || string.IsNullOrEmpty(Namespace))
				{
					Namespace = aux;
					Assembly = parts1[1];
					return;
				}
				else if(aux.IndexOf(Namespace) < 0)
					Namespace += (Namespace.EndsWith(".") ? "" : ".") + aux;
			}
			else
				Name = name;
			if(parts1.Length > 1 && string.IsNullOrEmpty(Assembly))
				Assembly = parts1[1];
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="type"></param>
		public TypeInfo(Type type)
		{
			Name = type.Name;
			Namespace = type.Namespace;
			Assembly = type.Assembly.FullName;
		}

		/// <summary>
		/// Constrói uma instancia com base no nome do tipo.
		/// </summary>
		/// <param name="typeFullname"></param>
		public TypeInfo(string typeFullname)
		{
			if(string.IsNullOrEmpty(typeFullname))
				throw new ArgumentNullException("typeFullname");
			var parts1 = typeFullname.Split(',');
			for(int i = 0; i < parts1.Length; i++)
				parts1[i] = parts1[i].Trim();
			var parts2 = parts1[0].Split('.');
			for(int i = 0; i < parts2.Length; i++)
				parts2[i] = parts2[i].Trim();
			if(parts2.Length > 1)
			{
				Name = ElementMapping.Last(parts2);
				var aux = string.Join(".", parts2, 0, parts2.Length - 1);
				if(parts1.Length > 1 || string.IsNullOrEmpty(Namespace))
				{
					Namespace = aux;
					Assembly = parts1[1];
					return;
				}
				else if(aux.IndexOf(Namespace) < 0)
					Namespace += (Namespace.EndsWith(".") ? "" : ".") + aux;
			}
			else
				Name = typeFullname;
			if(parts1.Length > 1 && string.IsNullOrEmpty(Assembly))
				Assembly = parts1[1];
		}
	}
}
