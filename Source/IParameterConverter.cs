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

namespace GDA.Interfaces
{
	/// <summary>
	/// Assinatura de um conversor de parametros.
	/// </summary>
	public interface IParameterConverter
	{
		/// <summary>
		/// Converte um parametro do GDA para um parametro de dados.
		/// </summary>
		/// <param name="parameter"></param>
		/// <returns></returns>
		System.Data.IDbDataParameter Convert(GDAParameter parameter);
	}
	/// <summary>
	/// Assinatura de um conversor de parametros que utiliza o IDbCommand como parametro.
	/// </summary>
	public interface IParameterConverter2
	{
		/// <summary>
		/// Converte um parametro do GDA para um parametro de dados.
		/// </summary>
		/// <param name="command">Instancia do comando onde o parametro será utilizado.</param>
		/// <param name="parameter">Instancia do parametro que será convertido.</param>
		/// <returns></returns>
		System.Data.IDbDataParameter Converter(System.Data.IDbCommand command, GDAParameter parameter);
	}
}
