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

namespace GDA.Interfaces
{
	/// <summary>
	/// Assinatura usada nas classe que necessita trabalhar as colunas
	/// do resultado da consulta independente das propriedades da classe.
	/// </summary>
	public interface IObjectDataRecord
	{
		/// <summary>
		/// Insere um campo do registro na classe.
		/// </summary>
		/// <param name="fieldName">Nome do campo.</param>
		/// <param name="value">Valor do campo.</param>
		void InsertRecordField(string fieldName, object value);

		/// <summary>
		/// Identifica se classe deverá ou não carrega os campos do registro que já foram mapeados.
		/// </summary>
		bool LoadMappedsRecordFields
		{
			get;
		}
	}
}
