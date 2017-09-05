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
using System.Xml.XPath;

namespace GDA.Common.Configuration.Handlers
{
	public class BaseSectionHandler
	{
		/// <summary>
		/// Nó principal que contem as informações do GDA
		/// </summary>
		protected XmlNode root;

		/// <summary>
		/// Determina se a sessão carregada do arquivo xml é v´lalida
		/// </summary>
		/// <returns>True se estiver usando o arquivo de configuração.</returns>
		public bool IsValid
		{
			get
			{
				return root != null;
			}
		}

		#if !PocketPC
		/// <summary>
		/// Obtem o nó principal do XML de configuração.
		/// </summary>
		public IXPathNavigable XmlRoot
		{
			get
			{
				return root;
			}
		}

		#endif
		public BaseSectionHandler(XmlNode node)
		{
			root = node;
		}

		/// <summary>
		/// Obtem um nó da árvore.
		/// </summary>
		public XmlNode GetNode(string path)
		{
			return root.SelectSingleNode(path);
		}

		/// <summary>
		/// Obtem o nós que estão contidos no nó da árvore.
		/// </summary>
		public XmlNodeList GetNodes(string path)
		{
			return root.SelectNodes(path);
		}
	}
}
