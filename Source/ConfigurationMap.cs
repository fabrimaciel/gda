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
using System.Collections;
using System.Reflection;
using GDA.Common.Configuration.Attributes;
using GDA.Common.Configuration.Targets;
using GDA.Common.Configuration.Handlers;

namespace GDA.Common.Configuration
{
	internal class ConfigurationMap
	{
		/// <summary>
		/// Lista do alvos a serem chamados para carrega os informações do
		/// arquivo de configuração.
		/// </summary>
		private IList instanceTargets = new ArrayList();

		/// <summary>
		/// Armazena os métodos configurados para serem chamado quando carrega
		/// as informações do arquivo de configuração. Ela contem a relação dos 
		/// nomes do método, que podem ser sobrecarregados.
		/// </summary>
		private Hashtable instanceOverloads = new Hashtable();

		private IList staticTargets = new ArrayList();

		private Hashtable staticOverloads = new Hashtable();

		/// <summary>
		/// Constrói a novo instancia do ConfigurationMap para o tipo submetido. 
		/// Isso irá carrega as informações dos metados contidos no tipo para carregar
		/// o mapeamento do arquivo de configuração <see cref="ConfigurationAttribute"/>.
		/// </summary>
		/// <exception cref="GDA.GDAException"></exception>
		public ConfigurationMap(Type type)
		{
			List<MemberAttributeInfo> memberInfos = LoadMembersConfigurationAttributes(Helper.ReflectionFlags.InstanceCriteria, type);
			foreach (MemberAttributeInfo mai in memberInfos)
			{
				ConfigurationAttribute ca = mai.Attributes[0] as ConfigurationAttribute;
				switch(mai.MemberInfo.MemberType)
				{
				case MemberTypes.Method:
					MethodTarget target = null;
					if(instanceOverloads.ContainsKey(ca.XmlNodePath))
						target = instanceOverloads[ca.XmlNodePath] as MethodTarget;
					if(target != null)
						target.AddCallbackMethod(mai.MemberInfo as MethodInfo, ca.RequiredParameters);
					else
					{
						target = new MethodTarget(ca, mai.MemberInfo as MethodInfo);
						instanceTargets.Add(target);
						instanceOverloads[ca.XmlNodePath] = target;
					}
					break;
				case MemberTypes.Field:
					instanceTargets.Add(new FieldTarget(ca, mai.MemberInfo as FieldInfo));
					break;
				case MemberTypes.Property:
					instanceTargets.Add(new PropertyTarget(ca, mai.MemberInfo as PropertyInfo));
					break;
				default:
					throw new GDAException("Unknown configuration target type for member {0} on class {1}.", mai.MemberInfo.Name, type);
				}
			}
			memberInfos = LoadMembersConfigurationAttributes(Helper.ReflectionFlags.StaticCriteria, type);
			foreach (MemberAttributeInfo mai in memberInfos)
			{
				ConfigurationAttribute ca = mai.Attributes[0] as ConfigurationAttribute;
				switch(mai.MemberInfo.MemberType)
				{
				case MemberTypes.Method:
					MethodTarget target = null;
					if(staticOverloads.ContainsKey(ca.XmlNodePath))
						target = staticOverloads[ca.XmlNodePath] as MethodTarget;
					if(target != null)
						target.AddCallbackMethod(mai.MemberInfo as MethodInfo, ca.RequiredParameters);
					else
					{
						target = new MethodTarget(ca, mai.MemberInfo as MethodInfo);
						staticTargets.Add(target);
						staticOverloads[ca.XmlNodePath] = target;
					}
					break;
				case MemberTypes.Field:
					staticTargets.Add(new FieldTarget(ca, mai.MemberInfo as FieldInfo));
					break;
				case MemberTypes.Property:
					staticTargets.Add(new PropertyTarget(ca, mai.MemberInfo as PropertyInfo));
					break;
				default:
					throw new GDAException("Unknown configuration target type for member {0} on class {1}.", mai.MemberInfo.Name, type);
				}
			}
		}

		/// <summary>
		/// Configure all targets in the specified object using the given ElementTree 
		/// as source. If a Type is passed in obj, static members will be configured.
		/// If an object instance is passed in obj, instance members will be configured.
		/// </summary>
		/// <param name="handlers">The list of handlers providing the source values</param>
		/// <param name="obj">The object to be configured</param>
		public void Configure(IList handlers, object obj)
		{
			IList targets = obj is Type ? staticTargets : instanceTargets;
			foreach (ElementTarget target in targets)
			{
				int maxHandlerIndex = target is MethodTarget ? handlers.Count : 1;
				for(int i = 0; i < maxHandlerIndex; i++)
				{
					BaseSectionHandler handler = handlers[i] as BaseSectionHandler;
					bool keyFound = false;
					if(target is MethodTarget)
					{
						XmlNodeList nodes = handler.GetNodes(target.XmlNodePath);
						if(nodes != null && nodes.Count > 0)
						{
							target.Configure(obj, nodes);
							keyFound = true;
							break;
						}
					}
					else
					{
						XmlNode node = handler.GetNode(target.XmlNodePath);
						if(node != null)
						{
							target.Configure(obj, node);
							keyFound = true;
							break;
						}
					}
					VerifyKeyPresence(target, keyFound);
				}
			}
		}

		private List<MemberAttributeInfo> LoadMembersConfigurationAttributes(BindingFlags flags, Type type)
		{
			List<MemberAttributeInfo> list = new List<MemberAttributeInfo>();
			MemberInfo[] membersInfo = type.GetMembers(flags);
			foreach (MemberInfo mi in membersInfo)
			{
				object[] attributes = mi.GetCustomAttributes(typeof(ConfigurationAttribute), true);
				if(attributes != null && attributes.Length > 0)
				{
					list.Add(new MemberAttributeInfo(mi, attributes));
				}
			}
			return list;
		}

		/// <summary>
		/// Faz a verificação se chave do alvo foi encontrada no arquivo de configuração.
		/// </summary>
		/// <param name="target">Elemento alvo.</param>
		/// <param name="keyFound">Identificador se a a chave foi encontrada.</param>
		private void VerifyKeyPresence(ElementTarget target, bool keyFound)
		{
			if(!keyFound && target.KeyPresenceRequirement != ConfigKeyPresence.Optional)
			{
				throw new GDA.Common.Configuration.Exceptions.MissingConfigurationKeyException(target.XmlNodePath);
			}
		}
	}
}
