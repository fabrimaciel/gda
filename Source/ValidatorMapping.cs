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
	/// Armazena os dados do validador que será usado no propriedade.
	/// </summary>
	public class ValidatorMapping : ElementMapping
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
		/// Parametros do validador.
		/// </summary>
		public List<ValidatorParamMapping> Parameters
		{
			get;
			set;
		}

		public ValidatorMapping(XmlElement element)
		{
			if(element == null)
				throw new ArgumentNullException("element");
			Name = GetAttributeString(element, "name", true);
			Parameters = new List<ValidatorParamMapping>();
			foreach (XmlElement i in element.GetElementsByTagName("param"))
			{
				var vp = new ValidatorParamMapping(i);
				if(!Parameters.Exists(f => f.Name == vp.Name))
					Parameters.Add(vp);
			}
		}

		/// <summary>
		/// Constrói uma instancia do mapeamento da validador.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="parameters"></param>
		public ValidatorMapping(string name, IEnumerable<ValidatorParamMapping> parameters)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException("name");
			this.Name = name;
			Parameters = new List<ValidatorParamMapping>();
			if(parameters != null)
				foreach (var i in parameters)
					if(!Parameters.Exists(f => f.Name == i.Name))
						Parameters.Add(i);
		}

		public ValidatorAttribute GetValidator()
		{
			var typeName = Name;
			Type type = null;
			switch(Name)
			{
			case "Unique":
				type = typeof(UniqueAttribute);
				break;
			case "RangeValidator":
			case "Range":
				type = typeof(RangeValidatorAttribute);
				break;
			case "RequiredValidator":
			case "Required":
				type = typeof(RequiredValidatorAttribute);
				break;
			default:
				type = Type.GetType(typeName, false, true);
				break;
			}
			if(type == null)
			{
				var parts = typeName.Split(',');
				typeName = parts[0].Trim() + "Attribute";
				if(parts.Length > 1)
					typeName += ", " + parts[1];
				type = Type.GetType(typeName, false, true);
				if(type == null)
					throw new GDAMappingException("Fail on instance validator type \"{0}\"", Name);
			}
			var instance = Activator.CreateInstance(type) as ValidatorAttribute;
			if(instance == null)
				return null;
			foreach (var i in Parameters)
			{
				var pi = type.GetProperty(i.Name, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
				if(pi != null && pi.CanWrite && !string.IsNullOrEmpty(i.Value))
				{
					try
					{
						if(pi.PropertyType.IsEnum)
							pi.SetValue(instance, Enum.Parse(pi.PropertyType, i.Value, true), null);
						else if(pi.PropertyType == typeof(string))
							pi.SetValue(instance, i.Value, null);
						else
							pi.SetValue(instance, typeof(Convert).GetMethod("To" + pi.PropertyType.Name, new Type[] {
								typeof(string)
							}).Invoke(null, new object[] {
								i.Value
							}), null);
					}
					catch(Exception ex)
					{
						if(ex is System.Reflection.TargetInvocationException)
							ex = ex.InnerException;
						throw new GDAMappingException(string.Format("Fail on set validator \"{0}\" property \"{1}\" value \"{2}\"", Name, pi.Name, i.Value), ex);
					}
				}
			}
			return instance;
		}
	}
}
