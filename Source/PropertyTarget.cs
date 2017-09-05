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
using GDA.Common.Configuration.Attributes;
using System.Reflection;

namespace GDA.Common.Configuration.Targets
{
	internal class PropertyTarget : ElementTarget
	{
		/// <summary>
		/// Representa a propriedade alvo.
		/// </summary>
		public readonly PropertyInfo RepresentPropertyInfo;

		/// <summary>
		/// Construtor
		/// </summary>
		/// <param name="ca">Metadado relacionado com a propriedade.</param>
		/// <param name="pi">Propriedade</param>
		public PropertyTarget(ConfigurationAttribute ca, PropertyInfo pi) : base(ca)
		{
			this.RepresentPropertyInfo = pi;
		}

		public override void Configure(object target, System.Xml.XmlNode node)
		{
			object value = GDA.Common.Helper.TypeConverter.Get(RepresentPropertyInfo.PropertyType, node);
			RepresentPropertyInfo.SetValue(target, value, Helper.ReflectionFlags.InstanceCriteria, null, null, null);
		}
	}
}
