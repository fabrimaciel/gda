﻿/* 
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

namespace GDA.Provider.MsAccess
{
	public class MsAccessProviderConfiguration : ProviderConfiguration
	{
		/// <summary>
		/// Provider padrão.
		/// </summary>
		private GDA.Interfaces.IProvider currentProvider = new MsAccessProvider();

		/// <summary>
		/// Gets And Sets o provider relacionado.
		/// </summary>
		public override GDA.Interfaces.IProvider Provider
		{
			get
			{
				return currentProvider;
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="connectionString">Connection string para conexão com a BD.</param>
		public MsAccessProviderConfiguration(string connectionString) : base(connectionString, new MsAccessProvider())
		{
		}

		/// <summary>
		/// Analyzer relacionado com o provider.
		/// </summary>
		public override GDA.Analysis.DatabaseAnalyzer GetDatabaseAnalyzer()
		{
			return new MsAccessAnalyzer(this);
		}
	}
}
