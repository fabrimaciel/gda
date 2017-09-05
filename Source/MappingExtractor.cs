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
using System.Linq;
using System.Text;

namespace GDA.Mapping
{
	/// <summary>
	/// Classe responsável por realizar a extração dos mapeamento.
	/// </summary>
	public class MappingExtractor
	{
		/// <summary>
		/// Extrai o mapeamento xml contido no assembly informado.
		/// </summary>
		/// <param name="assembly"></param>
		/// <param name="outStream">Stream de saída.</param>
		/// <returns></returns>
		public void ExtractXmlMapping(System.Reflection.Assembly assembly, System.IO.Stream outStream)
		{
			if(assembly == null)
				throw new ArgumentNullException("assembly");
			if(outStream == null)
				throw new ArgumentNullException("outStream");
			var types = assembly.GetTypes();
			var document = new System.Xml.XmlDocument();
			var root = document.CreateElement("gda-mapping");
			document.AppendChild(root);
			root.SetAttribute("namespace", "");
			root.SetAttribute("assembly", assembly.GetName().Name);
			foreach (var type in types)
			{
				var customAttributes = type.GetCustomAttributes(typeof(global::GDA.PersistenceClassAttribute), false);
				if(customAttributes.Length > 0)
				{
					var persistenceClass = (global::GDA.PersistenceClassAttribute)customAttributes[0];
					var classElement = document.CreateElement("class");
					classElement.SetAttribute("name", type.FullName);
					classElement.SetAttribute("table", persistenceClass.Name);
					classElement.SetAttribute("schema", persistenceClass.Schema);
					root.AppendChild(classElement);
					foreach (var property in type.GetProperties())
					{
						var persistenceProperty = (global::GDA.PersistencePropertyAttribute)property.GetCustomAttributes(typeof(global::GDA.PersistencePropertyAttribute), false).FirstOrDefault();
						if(persistenceProperty != null)
						{
							var propertyElement = document.CreateElement("property");
							propertyElement.SetAttribute("name", property.Name);
							if(!StringComparer.InvariantCultureIgnoreCase.Equals(property.Name, persistenceProperty.Name))
								propertyElement.SetAttribute("column", persistenceProperty.Name);
							if(persistenceProperty.ParameterType != PersistenceParameterType.Field)
								propertyElement.SetAttribute("parameterType", persistenceProperty.ParameterType.ToString());
							if(persistenceProperty.Size > 0)
								propertyElement.SetAttribute("size", persistenceProperty.Size.ToString());
							if(persistenceProperty.Direction != DirectionParameter.InputOutput)
								propertyElement.SetAttribute("direction", persistenceProperty.Direction.ToString());
							if(persistenceProperty.IsNotNull)
								propertyElement.SetAttribute("not-null", persistenceProperty.IsNotNull.ToString().ToLower());
							classElement.AppendChild(propertyElement);
						}
					}
				}
			}
			document.Save(outStream);
		}
	}
}
