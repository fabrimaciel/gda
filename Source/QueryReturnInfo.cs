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

namespace GDA.Sql
{
	/// <summary>
	/// Representa as informações para o retorno da consulta.
	/// </summary>
	public class QueryReturnInfo
	{
		private List<GDAParameter> _parameters;

		private string _commandText;

		private List<Mapper> _recoverProperties;

		/// <summary>
		/// Lista dos parametros que serão utilizados.
		/// </summary>
		public List<GDAParameter> Parameters
		{
			get
			{
				return _parameters;
			}
		}

		/// <summary>
		/// Comando de texto da consulta.
		/// </summary>
		public string CommandText
		{
			get
			{
				return _commandText;
			}
		}

		/// <summary>
		/// Lista das propriedades mapeadas que devem ser recuperadas.
		/// </summary>
		internal List<Mapper> RecoverProperties
		{
			get
			{
				return _recoverProperties;
			}
		}

		/// <summary>
		/// Construtor interno padrão.
		/// </summary>
		/// <param name="commandText">Comando de texto da consulta.</param>
		/// <param name="parameters">Lista dos parametros que serão utilizados.</param>
		/// <param name="recoverProperties">Lista com as propriedades que devem ser recuperadas na consulta.</param>
		internal QueryReturnInfo(string commandText, IList<GDAParameter> parameters, List<Mapper> recoverProperties)
		{
			_commandText = commandText;
			_parameters = new List<GDAParameter>(parameters);
			_recoverProperties = recoverProperties;
		}
	}
}
