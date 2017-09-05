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

namespace GDA.Common.Configuration.Targets
{
	using System.Xml;
	using GDA.Common.Configuration.Attributes;

	internal abstract class ElementTarget
	{
		internal readonly string XmlNodePath;

		internal readonly ConfigKeyPresence KeyPresenceRequirement;

		/// <summary>
		/// Construtor.
		/// </summary>
		/// <param name="ca">Atributo de configuração para preecher o objeto.</param>
		public ElementTarget(ConfigurationAttribute ca)
		{
			this.XmlNodePath = ca.XmlNodePath;
			this.KeyPresenceRequirement = ca.KeyPresenceRequirement;
		}

		/// <summary>
		/// Configura os dados lidos do nodo xml no objeto.
		/// </summary>
		/// <param name="target">Objeto.</param>
		/// <param name="node">Nodo xml contendo os dados.</param>
		public abstract void Configure(object target, XmlNode node);

		public void Configure(object target, XmlNodeList nodes)
		{
			foreach (XmlNode node in nodes)
				Configure(target, node);
		}
	}
}
