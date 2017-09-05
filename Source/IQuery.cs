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
	/// Representa uma consulta no GDA.
	/// </summary>
	public interface IQuery
	{
		/// <summary>
		/// Tipo do retorno da consulta.
		/// </summary>
		Type ReturnTypeQuery
		{
			get;
			set;
		}

		/// <summary>
		/// Número de registros que deverão ser pulados.
		/// </summary>
		int SkipCount
		{
			get;
		}

		/// <summary>
		/// Quantidade de registros que serão recuperados.
		/// </summary>
		int TakeCount
		{
			get;
		}

		/// <summary>
		/// Nome da propriedade para utiliza uma função de agregação.
		/// </summary>
		string AggregationFunctionProperty
		{
			get;
		}

		/// <summary>
		/// Constrói as informações para o resultado da consulta.
		/// </summary>
		/// <typeparam name="T">Tipo de dados que será tratado na consulta.</typeparam>
		/// <returns>Consulta SQL gerada.</returns>
		QueryReturnInfo BuildResultInfo<T>(GDA.Interfaces.IProviderConfiguration configuration);

		/// <summary>
		/// Constrói as informações para o resultado da consulta.
		/// </summary>
		/// <param name="aggregationFunction">Função de agregação usada para recuperar o resultado.</param>
		/// <returns></returns>
		QueryReturnInfo BuildResultInfo(string aggregationFunction);

		/// <summary>
		/// Constrói as informações para o resultado da consulta.
		/// </summary>
		/// <param name="aggregationFunction">Função de agregação usada para recuperar o resultado.</param>
		/// <param name="provider">Provider que será utilizado no build.</param>
		/// <returns></returns>
		QueryReturnInfo BuildResultInfo2(GDA.Interfaces.IProvider provider, string aggregationFunction);

		/// <summary>
		/// Constrói as informações para o resultado da consulta.
		/// </summary>
		/// <param name="provider">Provider que será utilizado no build.</param>
		/// <param name="aggregationFunction">Função de agregação usada para recuperar o resultado.</param>
		/// <param name="classesDictionary">Dicionário com as classe que já foram processadas.</param>
		/// <returns></returns>
		QueryReturnInfo BuildResultInfo2(GDA.Interfaces.IProvider provider, string aggregationFunction, Dictionary<string, Type> classesDictionary);
	}
}
