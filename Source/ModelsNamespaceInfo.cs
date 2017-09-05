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
using System.Reflection;

namespace GDA
{
	/// <summary>
	/// Armazena as informações dos models do namespace.
	/// </summary>
	public class ModelsNamespaceInfo
	{
		/// <summary>
		/// Namespace das models.
		/// </summary>
		public readonly string Namespace;

		/// <summary>
		/// Assembly do namespace.
		/// </summary>
		public readonly string AssemblyName;

		private Assembly _currentAssembly;

		/// <summary>
		/// Assembly do namespace.
		/// </summary>
		public Assembly CurrentAssembly
		{
			get
			{
				LoadCurrentAssembly();
				return _currentAssembly;
			}
		}

		private void LoadCurrentAssembly()
		{
			if(_currentAssembly == null)
			{
				if(AssemblyName == "*")
					#if PocketPC
					                    // Carrega o assembly da aplicação que está utilizado o gda
                    _currentAssembly = System.Reflection.Assembly.GetExecutingAssembly();
#else
					_currentAssembly = System.Reflection.Assembly.GetEntryAssembly();
				#endif
				else
				{
					#if PocketPC
					                    // Carrega o assembly com os dados completos.
                    _currentAssembly = Assembly.Load(AssemblyName);
#else
					var entry = System.Reflection.Assembly.GetEntryAssembly();
					if(entry != null)
					{
						AssemblyName[] names = System.Reflection.Assembly.GetEntryAssembly().GetReferencedAssemblies();
						foreach (AssemblyName an in names)
						{
							if(an.Name == AssemblyName)
							{
								_currentAssembly = System.Reflection.Assembly.Load(an);
								return;
							}
						}
					}
					if(AssemblyName.IndexOf(',') == -1)
					{
						_currentAssembly = Assembly.LoadWithPartialName(AssemblyName);
					}
					else
					{
						_currentAssembly = Assembly.Load(AssemblyName);
					}
					#endif
				}
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="ns"></param>
		/// <param name="assemblyName"></param>
		public ModelsNamespaceInfo(string ns, string assemblyName)
		{
			if(string.IsNullOrEmpty(assemblyName))
				throw new ArgumentNullException("assemblyName");
			Namespace = ns;
			AssemblyName = assemblyName;
		}

		public ModelsNamespaceInfo(string ns, Assembly currentAssembly)
		{
			if(currentAssembly == null)
				throw new ArgumentNullException("currentAssembly");
			_currentAssembly = currentAssembly;
			Namespace = ns;
		}
	}
}
