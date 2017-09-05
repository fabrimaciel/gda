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
using GDA.Sql;
using GDA.Provider;

namespace GDA.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="Model"></typeparam>
	public interface IBaseDAO<Model> : ISimpleBaseDAO<Model> where Model : new()
	{
		/// <summary>
		/// Remove os dados no BD.
		/// </summary>
		/// <param name="objDelete">Objeto contendo os dados a serem removidos.</param>
		/// <returns>Número de linhas afetadas.</returns>
		int Delete(Model objDelete);

		/// <summary>
		/// Remove os dados no BD.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <param name="objDelete">Objeto contendo os dados a serem removidos.</param>
		/// <returns>Número de linhas afetadas.</returns>
		int Delete(GDASession session, Model objDelete);

		/// <summary>
		/// Inseri os dados contidos no objInsert no BD.
		/// </summary>
		/// <param name="objInsert">Objeto com os dados a serem inseridos.</param>
		/// <param name="propertiesNamesInsert">Nome das propriedades separados por virgula, que serão inseridos no comando.</param>
		/// <param name="direction">Direção que os nomes das propriedades terão no comando. (Default: DirectionPropertiesName.Inclusion)</param>
		/// <returns>Chave inserido.</returns>
		uint Insert(Model objInsert, string propertiesNamesInsert, DirectionPropertiesName direction);

		/// <summary>
		/// Inseri os dados contidos no objInsert no BD.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <param name="objInsert">Objeto com os dados a serem inseridos.</param>
		/// <param name="propertiesNamesInsert">Nome das propriedades separados por virgula, que serão inseridos no comando.</param>
		/// <param name="direction">Direção que os nomes das propriedades terão no comando. (Default: DirectionPropertiesName.Inclusion)</param>
		/// <returns>Chave inserido.</returns>
		uint Insert(GDASession session, Model objInsert, string propertiesNamesInsert, DirectionPropertiesName direction);

		/// <summary>
		/// Inseri os dados contidos no objInsert no BD.
		/// </summary>
		/// <param name="objInsert">Objeto com os dados a serem inseridos.</param>
		/// <param name="propertiesNamesInsert">Nome das propriedades separados por virgula, que serão inseridos no comando.</param>
		/// <returns>Chave inserido.</returns>
		uint Insert(Model objInsert, string propertiesNamesInsert);

		/// <summary>
		/// Inseri os dados contidos no objInsert no BD.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <param name="objInsert">Objeto com os dados a serem inseridos.</param>
		/// <param name="propertiesNamesInsert">Nome das propriedades separados por virgula, que serão inseridos no comando.</param>
		/// <returns>Chave inserido.</returns>
		uint Insert(GDASession session, Model objInsert, string propertiesNamesInsert);

		/// <summary>
		/// Insere os dados no BD.
		/// </summary>
		/// <param name="objInsert">Objeto contendo os dados a serem inseridos.</param>
		/// <returns>Identidade gerada.</returns>
		uint Insert(Model objInsert);

		/// <summary>
		/// Insere os dados no BD.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <param name="objInsert">Objeto contendo os dados a serem inseridos.</param>
		/// <returns>Identidade gerada.</returns>
		uint Insert(GDASession session, Model objInsert);

		/// <summary>
		/// Se o registro já existir, atualiza, caso contrário insere.
		/// </summary>
		/// <param name="objUpdate">Objeto contendo os dados.</param>
		uint InsertOrUpdate(Model objUpdate);

		/// <summary>
		/// Se o registro já existir, atualiza, caso contrário insere.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <param name="objUpdate">Objeto contendo os dados.</param>
		uint InsertOrUpdate(GDASession session, Model objUpdate);

		/// <summary>
		/// Atualiza os dados contidos no objUpdate no BD.
		/// </summary>
		/// <param name="objUpdate">Objeto com os dados a serem atualizados.</param>
		/// <param name="propertiesNamesUpdate">Nome das propriedades separados por virgula, que serão atualizadas no comando.</param>
		/// /// <param name="direction">Direção que os nomes das propriedades terão no comando. (Default: DirectionPropertiesName.Inclusion)</param>
		/// <returns>Número de linhas afetadas.</returns>
		int Update(Model objUpdate, string propertiesNamesUpdate, DirectionPropertiesName direction);

		/// <summary>
		/// Atualiza os dados contidos no objUpdate no BD.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <param name="objUpdate">Objeto com os dados a serem atualizados.</param>
		/// <param name="propertiesNamesUpdate">Nome das propriedades separados por virgula, que serão atualizadas no comando.</param>
		/// /// <param name="direction">Direção que os nomes das propriedades terão no comando. (Default: DirectionPropertiesName.Inclusion)</param>
		/// <returns>Número de linhas afetadas.</returns>
		int Update(GDASession session, Model objUpdate, string propertiesNamesUpdate, DirectionPropertiesName direction);

		/// <summary>
		/// Atualiza os dados contidos no objUpdate no BD.
		/// </summary>
		/// <param name="objUpdate">Objeto com os dados a serem atualizados.</param>
		/// <param name="propertiesNamesUpdate">Nome das propriedades separados por virgula, que serão atualizadas no comando.</param>
		/// <returns>Número de linhas afetadas.</returns>
		int Update(Model objUpdate, string propertiesNamesUpdate);

		/// <summary>
		/// Atualiza os dados contidos no objUpdate no BD.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <param name="objUpdate">Objeto com os dados a serem atualizados.</param>
		/// <param name="propertiesNamesUpdate">Nome das propriedades separados por virgula, que serão atualizadas no comando.</param>
		/// <returns>Número de linhas afetadas.</returns>
		int Update(GDASession session, Model objUpdate, string propertiesNamesUpdate);

		/// <summary>
		/// Atualiza os dados no BD.
		/// </summary>
		/// <param name="objUpdate">Objeto contendo os dados a serem atualizados.</param>
		/// <returns>Número de linhas afetadas.</returns>
		int Update(Model objUpdate);

		/// <summary>
		/// Atualiza os dados no BD.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <param name="objUpdate">Objeto contendo os dados a serem atualizados.</param>
		/// <returns>Número de linhas afetadas.</returns>
		int Update(GDASession session, Model objUpdate);

		int CountRowForeignKeyParentToChild<ClassChild>(Model parentObj, string groupOfRelationship) where ClassChild : new();

		int CountRowForeignKeyParentToChild<ClassChild>(Model parentObj) where ClassChild : new();

		GDA.Collections.GDAList<ClassChild> LoadDataForeignKeyParentToChild<ClassChild>(Model parentObj, string groupOfRelationship, InfoSortExpression sortProperty) where ClassChild : new();

		GDA.Collections.GDAList<ClassChild> LoadDataForeignKeyParentToChild<ClassChild>(Model parentObj, string groupOfRelationship) where ClassChild : new();

		GDA.Collections.GDAList<ClassChild> LoadDataForeignKeyParentToChild<ClassChild>(Model parentObj, string groupOfRelationship, InfoSortExpression sortProperty, InfoPaging paging) where ClassChild : new();

		GDA.Collections.GDAList<ClassChild> LoadDataForeignKeyParentToChild<ClassChild>(Model parentObj) where ClassChild : new();

		/// <summary>
		/// Recupera os dados do objeto submetido tem como base os valores
		/// da chave contidos no objeto submetido.
		/// </summary>
		/// <param name="objData">Objeto contendo os dados das chaves.</param>
		/// <returns>Model com os dados recuperados.</returns>
		Model RecoverData(Model objData);

		/// <summary>
		/// Recupera os dados do objeto submetido tem como base os valores
		/// da chave contidos no objeto submetido.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <param name="objData">Objeto contendo os dados das chaves.</param>
		/// <returns>Model com os dados recuperados.</returns>
		Model RecoverData(GDASession session, Model objData);

		/// <summary>
		/// Carrega todos os dados contidos na tabela.
		/// </summary>
		/// <returns>Todos os dados da tabela.</returns>
		GDA.Collections.GDACursor<Model> Select();

		/// <summary>
		/// Carrega todos os dados contidos na tabela.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <returns>Todos os dados da tabela.</returns>
		GDA.Collections.GDACursor<Model> Select(GDASession session);

		/// <summary>
		/// Carrega os dados com base na consulta informada.
		/// </summary>
		/// <param name="query">Dados da consulta.</param>
		/// <returns></returns>
		GDA.Collections.GDACursor<Model> Select(IQuery query);

		/// <summary>
		/// Carrega os dados com base na consulta informada.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <param name="query">Dados da consulta.</param>
		/// <returns></returns>
		GDA.Collections.GDACursor<Model> Select(GDASession session, IQuery query);

		/// <summary>
		/// Recupera a quantidade de registros com base na Query.
		/// </summary>
		/// <param name="query">Consulta usada.</param>
		/// <returns>Quantidade de registro encontrados com base na consulta.</returns>
		long Count(Query query);

		/// <summary>
		/// Recupera a quantidade de registros com base na Query.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <param name="query">Consulta usada.</param>
		/// <returns>Quantidade de registro encontrados com base na consulta.</returns>
		long Count(GDASession session, Query query);

		/// <summary>
		/// Recupera a quantidade de registros da tabela no banco.
		/// </summary>
		/// <returns>Quantidade de registro encontrados com base na consulta.</returns>
		long Count();

		/// <summary>
		/// Recupera a quantidade de registros da tabela no banco.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <returns>Quantidade de registro encontrados com base na consulta.</returns>
		long Count(GDASession session);

		/// <summary>
		/// Efetua a soma de uma determina propriedade da classe T definida.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <param name="query">Consulta usada.</param>
		/// <returns>Soma dos valores.</returns>
		double Sum(GDASession session, Query query);

		/// <summary>
		/// Recupera o item com o maior valor.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <param name="query">Consulta usada.</param>
		/// <returns>Maior valor encontrado ou zero.</returns>
		double Max(GDASession session, Query query);

		/// <summary>
		/// Recupera o item com o menor valor.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <param name="query">Consulta usada.</param>
		/// <returns>Menor valor encontrado ou zero.</returns>
		double Min(GDASession session, Query query);

		/// <summary>
		/// Recupera a média dos valores da propriedade especificada na consulta.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <param name="query">Consulta usada.</param>
		/// <returns>Valor medio encontrado ou zero.</returns>
		double Avg(GDASession session, Query query);

		/// <summary>
		/// Verifica se o valor da propriedade informada existe no banco de dados.
		/// </summary>
		/// <param name="session">Sessão de conexão que será usada na verificação.</param>
		/// <param name="mode">Modo de validação.</param>
		/// <param name="propertyName">Nome da propriedade que será verificada.</param>
		/// <param name="propertyValue">Valor da propriedade que será verificada.</param>
		/// <param name="parent">Elemento que contém a propriedade</param>
		/// <returns>True caso existir.</returns>
		bool CheckExist(GDASession session, ValidationMode mode, string propertyName, object propertyValue, Model parent);
	}
}
