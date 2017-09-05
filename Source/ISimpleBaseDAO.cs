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
using System.Collections;
using GDA.Sql;

namespace GDA.Interfaces
{
	/// <summary>
	/// Interface que possui os método simples relacionados com a DAO.
	/// </summary>
	public interface ISimpleBaseDAO
	{
		/// <summary>
		/// Provider de configuração usado na DAO.
		/// </summary>
		IProviderConfiguration Configuration
		{
			get;
		}

		/// <summary>
		/// Remove os dados no BD.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <param name="objDelete">Objeto contendo os dados a serem removidos.</param>
		/// <returns>Número de linhas afetadas.</returns>
		int Delete(GDASession session, object objDelete);

		/// <summary>
		/// Remove os dados no BD.
		/// </summary>
		/// <param name="objDelete">Objeto contendo os dados a serem removidos.</param>
		/// <returns>Número de linhas afetadas.</returns>
		int Delete(object objDelete);

		/// <summary>
		/// Inseri os dados contidos no objInsert no BD.
		/// </summary>
		/// <param name="objInsert">Objeto com os dados a serem inseridos.</param>
		/// <param name="propertiesNamesInsert">Nome das propriedades separados por virgula, que serão inseridos no comando.</param>
		/// <param name="direction">Direção que os nomes das propriedades terão no comando. (Default: DirectionPropertiesName.Inclusion)</param>
		/// <returns>Chave inserido.</returns>
		uint Insert(object objInsert, string propertiesNamesInsert, DirectionPropertiesName direction);

		/// <summary>
		/// Inseri os dados contidos no objInsert no BD.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <param name="objInsert">Objeto com os dados a serem inseridos.</param>
		/// <param name="propertiesNamesInsert">Nome das propriedades separados por virgula, que serão inseridos no comando.</param>
		/// <param name="direction">Direção que os nomes das propriedades terão no comando. (Default: DirectionPropertiesName.Inclusion)</param>
		/// <returns>Chave inserido.</returns>
		uint Insert(GDASession session, object objInsert, string propertiesNamesInsert, DirectionPropertiesName direction);

		/// <summary>
		/// Inseri os dados contidos no objInsert no BD.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <param name="objInsert">Objeto com os dados a serem inseridos.</param>
		/// <param name="propertiesNamesInsert">Nome das propriedades separados por virgula, que serão inseridos no comando.</param>
		/// <returns>Chave inserido.</returns>
		uint Insert(GDASession session, object objInsert, string propertiesNamesInsert);

		/// <summary>
		/// Inseri os dados contidos no objInsert no BD.
		/// </summary>
		/// <param name="objInsert">Objeto com os dados a serem inseridos.</param>
		/// <param name="propertiesNamesInsert">Nome das propriedades separados por virgula, que serão inseridos no comando.</param>
		/// <returns>Chave inserido.</returns>
		uint Insert(object objInsert, string propertiesNamesInsert);

		/// <summary>
		/// Insere os dados no BD.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <param name="objInsert">Objeto contendo os dados a serem inseridos.</param>
		/// <returns>Identidade gerada.</returns>
		uint Insert(GDASession session, object objInsert);

		/// <summary>
		/// Insere os dados no BD.
		/// </summary>
		/// <param name="objInsert">Objeto contendo os dados a serem inseridos.</param>
		/// <returns>Identidade gerada.</returns>
		uint Insert(object objInsert);

		/// <summary>
		/// Se o registro já existir, atualiza, caso contrário insere.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <param name="objUpdate">Objeto contendo os dados.</param>
		uint InsertOrUpdate(GDASession session, object objUpdate);

		/// <summary>
		/// Se o registro já existir, atualiza, caso contrário insere.
		/// </summary>
		/// <param name="objUpdate">Objeto contendo os dados.</param>
		uint InsertOrUpdate(object objUpdate);

		/// <summary>
		/// Atualiza os dados contidos no objUpdate no BD.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <param name="objUpdate">Objeto com os dados a serem atualizados.</param>
		/// <param name="propertiesNamesUpdate">Nome das propriedades separados por virgula, que serão atualizadas no comando.</param>
		/// <param name="direction">Direção que os nomes das propriedades terão no comando. (Default: DirectionPropertiesName.Inclusion)</param>
		/// <returns>Número de linhas afetadas.</returns>
		int Update(GDASession session, object objUpdate, string propertiesNamesUpdate, DirectionPropertiesName direction);

		/// <summary>
		/// Atualiza os dados contidos no objUpdate no BD.
		/// </summary>
		/// <param name="objUpdate">Objeto com os dados a serem atualizados.</param>
		/// <param name="propertiesNamesUpdate">Nome das propriedades separados por virgula, que serão atualizadas no comando.</param>
		/// <param name="direction">Direção que os nomes das propriedades terão no comando. (Default: DirectionPropertiesName.Inclusion)</param>
		/// <returns>Número de linhas afetadas.</returns>
		int Update(object objUpdate, string propertiesNamesUpdate, DirectionPropertiesName direction);

		/// <summary>
		/// Atualiza os dados contidos no objUpdate no BD.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <param name="objUpdate">Objeto com os dados a serem atualizados.</param>
		/// <param name="propertiesNamesUpdate">Nome das propriedades separados por virgula, que serão atualizadas no comando.</param>
		/// <returns>Número de linhas afetadas.</returns>
		int Update(GDASession session, object objUpdate, string propertiesNamesUpdate);

		/// <summary>
		/// Atualiza os dados contidos no objUpdate no BD.
		/// </summary>
		/// <param name="objUpdate">Objeto com os dados a serem atualizados.</param>
		/// <param name="propertiesNamesUpdate">Nome das propriedades separados por virgula, que serão atualizadas no comando.</param>
		/// <returns>Número de linhas afetadas.</returns>
		int Update(object objUpdate, string propertiesNamesUpdate);

		/// <summary>
		/// Atualiza os dados no BD.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <param name="objUpdate">Objeto contendo os dados a serem atualizados.</param>
		/// <returns>Número de linhas afetadas.</returns>
		int Update(GDASession session, object objUpdate);

		/// <summary>
		/// Atualiza os dados no BD.
		/// </summary>
		/// <param name="objUpdate">Objeto contendo os dados a serem atualizados.</param>
		/// <returns>Número de linhas afetadas.</returns>
		int Update(object objUpdate);

		/// <summary>
		/// Verifica se o valor da propriedade informada existe no banco de dados.
		/// </summary>
		/// <param name="session">Sessão de conexão que será usada na verificação.</param>
		/// <param name="mode">Modo de validação.</param>
		/// <param name="propertyName">Nome da propriedade que será verificada.</param>
		/// <param name="propertyValue">Valor da propriedade que será verificada.</param>
		/// <param name="parent">Elemento que contém a propriedade</param>
		/// <returns>True caso existir.</returns>
		bool CheckExist(GDASession session, ValidationMode mode, string propertyName, object propertyValue, object parent);
	}
	/// <summary>
	/// Interface que possui os método simples relacionados com a DAO.
	/// </summary>
	public interface ISimpleBaseDAO<Model> : ISimpleBaseDAO
	{
		/// <summary>
		/// Busca os dados relacionados com a consulta submetida.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <param name="query">Consulta.</param>
		/// <returns></returns>
		Collections.GDADataRecordCursor<Model> SelectToDataRecord(GDASession session, IQuery query);
	}
}
