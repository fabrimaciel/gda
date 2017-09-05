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
using System.Configuration;
using System.Xml;

namespace GDA.Common.Configuration.Handlers
{
	/// <summary>
	/// Essa classe é um handler para os arquivo de configuração padrão do .NET (App.config ou Web.config)
	/// O nome da seção para usar é GDA.
	/// </summary>
	#if PocketPC
	    public class GDASectionHandler : BaseSectionHandler
#else
	public class GDASectionHandler : BaseSectionHandler, IConfigurationSectionHandler
	#endif
	{
		/// <summary>
		/// Construtor.
		/// </summary>
		public GDASectionHandler() : base(null)
		{
		}

		/// <summary>
		/// Método chamado pelo .NET passando as informações sobre a sessão
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="configContext"></param>
		/// <param name="section"></param>
		/// <returns></returns>
		public object Create(object parent, object configContext, XmlNode section)
		{
			root = section;
			return this;
		}
	}
}
