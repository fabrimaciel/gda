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

namespace GDA.Common.Configuration.Exceptions
{
	/// <summary>
	/// Armazena as informações do erro ocorrido ao carregar as configurações do GDA.
	/// </summary>
	public class LoadConfigurationException : GDAException
	{
		public LoadConfigurationException(string message) : base(message)
		{
		}

		public LoadConfigurationException(Exception innerException) : base(innerException.Message, innerException)
		{
		}

		public LoadConfigurationException(string message, Exception innerException) : base(message, innerException)
		{
		}

		public LoadConfigurationException(string message, params object[] args) : base(message, args)
		{
		}
	}
}
