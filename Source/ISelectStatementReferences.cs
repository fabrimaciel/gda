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
	public interface ISelectStatementReferences
	{
		/// <summary>
		/// Recupera o nome da coluna mapeada para a propriedade do tipo.
		/// </summary>
		/// <param name="typeInfo"></param>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		string GetPropertyMapping(Mapping.TypeInfo typeInfo, string propertyName);

		/// <summary>
		/// Recupera a primeira propriedade chave mapeada para o tipo.
		/// </summary>
		/// <param name="typeInfo"></param>
		/// <returns></returns>
		string GetFirstKeyPropertyMapping(Mapping.TypeInfo typeInfo);

		/// <summary>
		/// Recupera as propriedades mapeadas.
		/// </summary>
		/// <param name="typeInfo"></param>
		/// <returns></returns>
		IEnumerable<Mapping.IPropertyMappingInfo> GetPropertiesMapping(Mapping.TypeInfo typeInfo);

		/// <summary>
		/// Recupera o nome da tabela com base no tipo.
		/// </summary>
		/// <param name="typeInfo"></param>
		/// <returns></returns>
		TableName GetTableName(Mapping.TypeInfo typeInfo);

		/// <summary>
		/// Recupera a informação do tipo.
		/// </summary>
		/// <param name="tableInfo"></param>
		/// <returns></returns>
		Mapping.TypeInfo GetTypeInfo(TableInfo tableInfo);
	}
}
