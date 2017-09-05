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
using GDA.Common.Helper;
using System.Xml;
using System.Collections;

namespace GDA.Common.Configuration.Targets
{
	internal class MethodTarget : ElementTarget
	{
		/// <summary>
		/// Um simples objeto para ser usado garantindo acesso único a dispacher.
		/// </summary>
		private object _lock = new object();

		/// <summary>
		/// Dispacher que irá conter os método a serem chamados.
		/// </summary>
		private MethodDispatcher dispatcher;

		/// <summary>
		/// Construtor.
		/// </summary>
		/// <param name="ca"></param>
		/// <param name="methodInfo"></param>
		public MethodTarget(ConfigurationAttribute ca, MethodInfo methodInfo) : base(ca)
		{
			dispatcher = new MethodDispatcher(new MethodInvoker(methodInfo, ca.RequiredParameters));
		}

		public void AddCallbackMethod(MethodInfo methodInfo, int requiredParameters)
		{
			lock (_lock)
			{
				dispatcher.AddInvoker(new MethodInvoker(methodInfo, requiredParameters));
			}
		}

		private Hashtable ExtractAttributes(XmlNode node)
		{
			Hashtable result = new Hashtable();
			foreach (XmlAttribute attr in node.Attributes)
			{
				result[attr.Name.ToLower()] = attr.Value;
			}
			return result;
		}

		/// <summary>
		/// Use the supplied XmlNode to configure the target object. This configuration target 
		/// performs a method callback on the target object, and uses the attributes of the XmlNode
		/// as parameters. The method parameter names must match the names of the node attributes 
		/// (a leading underscore will be stripped to permit using C# reserved words in the XML file). 
		/// This method does not check whether the node matches the requested environment.
		/// </summary>
		/// <param name="target">The object to cofigure.</param>
		/// <param name="node">The node containing the configuration value(s).</param>
		public override void Configure(object target, XmlNode node)
		{
			Hashtable parameters = ExtractAttributes(node);
			dispatcher.Invoke(target, parameters);
		}
	}
}
