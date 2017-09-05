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
using GDA.Provider;
using GDA.Helper;
using GDA.Common.Configuration;
using GDA.Collections;
using GDA.Interfaces;
using GDA.Sql;
using GDA.Caching;

namespace GDA
{
	public class BaseDAO<Model> : GDA.Interfaces.IBaseDAO<Model> where Model : new()
	{
		/// <summary>
		/// PersistenceObject que será responsável por tratar o acesso a dados
		/// </summary>
		private PersistenceObject<Model> currentPersistenceObject;

		/// <summary>
		/// Gets PersistenceObject que será responsável por tratar o acesso a dados
		/// </summary>
		protected PersistenceObject<Model> CurrentPersistenceObject
		{
			get
			{
				return currentPersistenceObject;
			}
		}

		/// <summary>
		/// Provider de configuração usado na DAO.
		/// </summary>
		public IProviderConfiguration Configuration
		{
			get
			{
				return currentPersistenceObject.Configuration;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="providerConfig">Provider com todo a configuração para acesso a dados.</param>
		public BaseDAO(IProviderConfiguration providerConfig)
		{
			currentPersistenceObject = new PersistenceObject<Model>(providerConfig);
			RegisterCurrentDAOInModel();
		}

		/// <summary>
		/// Constrói uma instancia de BaseDAO e carrega as configuração do arquivo de configuração.
		/// </summary>
		/// <exception cref="GDAException"></exception>
		public BaseDAO()
		{
			GDASettings.LoadConfiguration();
			PersistenceProviderAttribute providerAttr = MappingManager.GetPersistenceProviderAttribute(typeof(Model));
			IProviderConfiguration providerConfig = null;
			if(providerAttr != null)
			{
				if(!string.IsNullOrEmpty(providerAttr.ProviderConfigurationName))
					providerConfig = GDASettings.GetProviderConfiguration(providerAttr.ProviderConfigurationName);
				else
					providerConfig = GDASettings.CreateProviderConfiguration(providerAttr.ProviderName, providerAttr.ConnectionString);
			}
			else
				providerConfig = GDASettings.DefaultProviderConfiguration;
			currentPersistenceObject = new PersistenceObject<Model>(providerConfig);
			RegisterCurrentDAOInModel();
		}

		private void RegisterCurrentDAOInModel()
		{
			GDAOperations.AddMemberDAO(typeof(Model), this);
		}

		internal GDAList<Model> GetSqlData(string sql, List<GDAParameter> parameters, InfoSortExpression sortExpression, InfoPaging paging)
		{
			return CurrentPersistenceObject.LoadDataWithSortExpression(sql, sortExpression, paging, parameters.ToArray());
		}

		/// <summary>
		/// Carrega todos os dados contidos na tabela.
		/// </summary>
		/// <returns>Todos os dados da tabela.</returns>
		/// <exception cref="GDAException"></exception>
		/// <exception cref="GDAColumnNotFoundException"></exception>
		public virtual GDACursor<Model> Select()
		{
			return CurrentPersistenceObject.Select();
		}

		/// <summary>
		/// Carrega todos os dados contidos na tabela.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <returns>Todos os dados da tabela.</returns>
		/// <exception cref="GDAException"></exception>
		/// <exception cref="GDAColumnNotFoundException"></exception>
		public virtual GDACursor<Model> Select(GDASession session)
		{
			return CurrentPersistenceObject.Select(session);
		}

		/// <summary>
		/// Carrega os dados com base na consulta informada.
		/// </summary>
		/// <param name="query">Dados da consulta.</param>
		/// <returns></returns>
		public virtual GDACursor<Model> Select(IQuery query)
		{
			query.ReturnTypeQuery = typeof(Model);
			return CurrentPersistenceObject.Select(query);
		}

		/// <summary>
		/// Carrega os dados com base na consulta informada.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <param name="query">Dados da consulta.</param>
		/// <returns></returns>
		public virtual GDACursor<Model> Select(GDASession session, IQuery query)
		{
			query.ReturnTypeQuery = typeof(Model);
			return CurrentPersistenceObject.Select(session, query);
		}

		/// <summary>
		/// Busca os dados relacionados com a consulta submetida.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <param name="query">Consulta.</param>
		/// <returns></returns>
		public GDADataRecordCursor<Model> SelectToDataRecord(GDASession session, IQuery query)
		{
			query.ReturnTypeQuery = typeof(Model);
			return CurrentPersistenceObject.SelectToDataRecord(session, query);
		}

		/// <summary>
		/// Recupera a quantidade de registros com base na Query.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <param name="query">Consulta usada.</param>
		/// <returns>Quantidade de registro encontrados com base na consulta.</returns>
		public virtual long Count(GDASession session, Query query)
		{
			query.ReturnTypeQuery = typeof(Model);
			return CurrentPersistenceObject.Count(session, query);
		}

		/// <summary>
		/// Recupera a quantidade de registros da tabela no banco.
		/// </summary>
		/// <returns>Quantidade de registro encontrados com base na consulta.</returns>
		public virtual long Count()
		{
			return CurrentPersistenceObject.Count();
		}

		/// <summary>
		/// Recupera a quantidade de registros da tabela no banco.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <returns>Quantidade de registro encontrados com base na consulta.</returns>
		public virtual long Count(GDASession session)
		{
			return CurrentPersistenceObject.Count(session, null);
		}

		/// <summary>
		/// Recupera a quantidade de registros com base na Query.
		/// </summary>
		/// <param name="query">Consulta usada.</param>
		/// <returns>Quantidade de registro encontrados com base na consulta.</returns>
		public virtual long Count(Query query)
		{
			query.ReturnTypeQuery = typeof(Model);
			return CurrentPersistenceObject.Count(query);
		}

		/// <summary>
		/// Efetua a soma de uma determina propriedade da classe T definida.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <param name="query">Consulta usada.</param>
		/// <returns>Soma dos valores ou zero</returns>
		public double Sum(GDASession session, Query query)
		{
			query.ReturnTypeQuery = typeof(Model);
			return CurrentPersistenceObject.Sum(session, query);
		}

		/// <summary>
		/// Recupera o item com o maior valor.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <param name="query">Consulta usada.</param>
		/// <returns>Maior valor encontrado ou zero.</returns>
		public double Max(GDASession session, Query query)
		{
			query.ReturnTypeQuery = typeof(Model);
			return CurrentPersistenceObject.Max(session, query);
		}

		/// <summary>
		/// Recupera o item com o menor valor.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <param name="query">Consulta usada.</param>
		/// <returns>Menor valor encontrado.</returns>
		public double Min(GDASession session, Query query)
		{
			query.ReturnTypeQuery = typeof(Model);
			return CurrentPersistenceObject.Min(session, query);
		}

		/// <summary>
		/// Recupera a média dos valores da propriedade especificada na consulta.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <param name="query">Consulta usada.</param>
		/// <returns>Valor medio encontrado ou zero.</returns>
		public double Avg(GDASession session, Query query)
		{
			query.ReturnTypeQuery = typeof(Model);
			return CurrentPersistenceObject.Avg(session, query);
		}

		/// <summary>
		/// Verifica se o valor da propriedade informada existe no banco de dados.
		/// </summary>
		/// <param name="session">Sessão de conexão que será usada na verificação.</param>
		/// <param name="mode">Modo de validação.</param>
		/// <param name="propertyName">Nome da propriedade que será verificada.</param>
		/// <param name="propertyValue">Valor da propriedade que será verificada.</param>
		/// <param name="parent">Elemento que contém a propriedade</param>
		/// <returns>True caso existir.</returns>
		public bool CheckExist(GDASession session, ValidationMode mode, string propertyName, object propertyValue, Model parent)
		{
			return CurrentPersistenceObject.CheckExist(session, mode, propertyName, propertyValue, parent);
		}

		/// <summary>
		/// Recupera os dados do objeto submetido tem como base os valores
		/// da chave contidos no objeto submetido.
		/// </summary>
		/// <param name="objData">Objeto contendo os dados das chaves.</param>
		/// <returns>Model com os dados recuperados.</returns>
		/// <exception cref="GDAColumnNotFoundException"></exception>
		/// <exception cref="GDAException"></exception>
		public virtual Model RecoverData(Model objData)
		{
			return RecoverData(null, objData);
		}

		/// <summary>
		/// Recupera os dados do objeto submetido tem como base os valores
		/// da chave contidos no objeto submetido.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <param name="objData">Objeto contendo os dados das chaves.</param>
		/// <returns>Model com os dados recuperados.</returns>
		/// <exception cref="GDAColumnNotFoundException"></exception>
		/// <exception cref="GDAException"></exception>
		public virtual Model RecoverData(GDASession session, Model objData)
		{
			return CurrentPersistenceObject.RecoverData(session, objData);
		}

		/// <summary>
		/// Inseri os dados contidos no objInsert no BD.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <param name="objInsert">Objeto com os dados a serem inseridos.</param>
		/// <param name="propertiesNamesInsert">Nome das propriedades separados por virgula, que serão inseridos no comando.</param>
		/// <param name="direction">Direção que os nomes das propriedades terão no comando. (Default: DirectionPropertiesName.Inclusion)</param>
		/// <returns>Chave inserido.</returns>
		/// <exception cref="ArgumentNullException">ObjInsert it cannot be null.</exception>
		/// <exception cref="GDAException"></exception>
		public virtual uint Insert(GDASession session, Model objInsert, string propertiesNamesInsert, DirectionPropertiesName direction)
		{
			return CurrentPersistenceObject.Insert(session, objInsert, propertiesNamesInsert, direction);
		}

		/// <summary>
		/// Inseri os dados contidos no objInsert no BD.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <param name="objInsert">Objeto com os dados a serem inseridos.</param>
		/// <param name="propertiesNamesInsert">Nome das propriedades separados por virgula, que serão inseridos no comando.</param>
		/// <returns>Chave inserido.</returns>
		/// <exception cref="ArgumentNullException">ObjInsert it cannot be null.</exception>
		/// <exception cref="GDAException"></exception>
		public virtual uint Insert(GDASession session, Model objInsert, string propertiesNamesInsert)
		{
			return CurrentPersistenceObject.Insert(session, objInsert, propertiesNamesInsert, DirectionPropertiesName.Inclusion);
		}

		/// <summary>
		/// Inseri os dados contidos no objInsert no BD.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <param name="objInsert">Objeto com os dados a serem inseridos.</param>
		/// <returns>Chave inserido.</returns>
		/// <exception cref="ArgumentNullException">ObjInsert it cannot be null.</exception>
		/// <exception cref="GDAException"></exception>
		public virtual uint Insert(GDASession session, Model objInsert)
		{
			return CurrentPersistenceObject.Insert(session, objInsert, null, DirectionPropertiesName.Inclusion);
		}

		/// <summary>
		/// Inseri os dados contidos no objInsert no BD.
		/// </summary>
		/// <param name="objInsert">Objeto com os dados a serem inseridos.</param>
		/// <param name="propertiesNamesInsert">Nome das propriedades separados por virgula, que serão inseridos no comando.</param>
		/// <param name="direction">Direção que os nomes das propriedades terão no comando. (Default: DirectionPropertiesName.Inclusion)</param>
		/// <returns>Chave inserido.</returns>
		/// <exception cref="ArgumentNullException">ObjInsert it cannot be null.</exception>
		/// <exception cref="GDAException"></exception>
		public virtual uint Insert(Model objInsert, string propertiesNamesInsert, DirectionPropertiesName direction)
		{
			return CurrentPersistenceObject.Insert(objInsert, propertiesNamesInsert, direction);
		}

		/// <summary>
		/// Inseri os dados contidos no objInsert no BD.
		/// </summary>
		/// <param name="objInsert">Objeto com os dados a serem inseridos.</param>
		/// <param name="propertiesNamesInsert">Nome das propriedades separados por virgula, que serão inseridos no comando.</param>
		/// <returns>Chave inserido.</returns>
		/// <exception cref="ArgumentNullException">ObjInsert it cannot be null.</exception>
		/// <exception cref="GDAException"></exception>
		public uint Insert(Model objInsert, string propertiesNamesInsert)
		{
			return CurrentPersistenceObject.Insert(objInsert, propertiesNamesInsert);
		}

		/// <summary>
		/// Insere os dados no BD.
		/// </summary>
		/// <param name="objInsert">Objeto contendo os dados a serem inseridos.</param>
		/// <returns>Identidade gerada.</returns>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="GDAException"></exception>
		public virtual uint Insert(Model objInsert)
		{
			return CurrentPersistenceObject.Insert(objInsert);
		}

		/// <summary>
		/// Atualiza os dados contidos no objUpdate no BD.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <param name="objUpdate">Objeto com os dados a serem atualizados.</param>
		/// <param name="propertiesNamesUpdate">Nome das propriedades separados por virgula, que serão atualizadas no comando.</param>
		/// <param name="direction">Direção que os nomes das propriedades terão no comando. (Default: DirectionPropertiesName.Inclusion)</param>
		/// <exception cref="System.ArgumentNullException"></exception>
		/// <exception cref="GDAConditionalClauseException">Parameters do not exist to build the conditional clause.</exception>
		/// <exception cref="GDAException"></exception>
		/// <returns>Número de linhas afetadas.</returns>
		public virtual int Update(GDASession session, Model objUpdate, string propertiesNamesUpdate, DirectionPropertiesName direction)
		{
			return CurrentPersistenceObject.Update(session, objUpdate, propertiesNamesUpdate, direction);
		}

		/// <summary>
		/// Atualiza os dados contidos no objUpdate no BD.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <param name="objUpdate">Objeto com os dados a serem atualizados.</param>
		/// <param name="propertiesNamesUpdate">Nome das propriedades separados por virgula, que serão atualizadas no comando.</param>
		/// <exception cref="System.ArgumentNullException"></exception>
		/// <exception cref="GDAConditionalClauseException">Parameters do not exist to build the conditional clause.</exception>
		/// <exception cref="GDAException"></exception>
		/// <returns>Número de linhas afetadas.</returns>
		public virtual int Update(GDASession session, Model objUpdate, string propertiesNamesUpdate)
		{
			return CurrentPersistenceObject.Update(session, objUpdate, propertiesNamesUpdate, DirectionPropertiesName.Inclusion);
		}

		/// <summary>
		/// Atualiza os dados contidos no objUpdate no BD.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <param name="objUpdate">Objeto com os dados a serem atualizados.</param>
		/// <exception cref="System.ArgumentNullException"></exception>
		/// <exception cref="GDAConditionalClauseException">Parameters do not exist to build the conditional clause.</exception>
		/// <exception cref="GDAException"></exception>
		/// <returns>Número de linhas afetadas.</returns>
		public virtual int Update(GDASession session, Model objUpdate)
		{
			return CurrentPersistenceObject.Update(session, objUpdate, null, DirectionPropertiesName.Inclusion);
		}

		/// <summary>
		/// Atualiza os dados contidos no objUpdate no BD.
		/// </summary>
		/// <param name="objUpdate">Objeto com os dados a serem atualizados.</param>
		/// <param name="propertiesNamesUpdate">Nome das propriedades separados por virgula, que serão atualizadas no comando.</param>
		/// <param name="direction">Direção que os nomes das propriedades terão no comando. (Default: DirectionPropertiesName.Inclusion)</param>
		/// <exception cref="System.ArgumentNullException"></exception>
		/// <exception cref="GDAConditionalClauseException">Parameters do not exist to build the conditional clause.</exception>
		/// <exception cref="GDAException"></exception>
		/// <returns>Número de linhas afetadas.</returns>
		public virtual int Update(Model objUpdate, string propertiesNamesUpdate, DirectionPropertiesName direction)
		{
			return CurrentPersistenceObject.Update(objUpdate, propertiesNamesUpdate, direction);
		}

		/// <summary>
		/// Atualiza os dados contidos no objUpdate no BD.
		/// </summary>
		/// <param name="objUpdate">Objeto com os dados a serem atualizados.</param>
		/// <param name="propertiesNamesUpdate">Nome das propriedades separados por virgula, que serão atualizadas no comando.</param>
		/// <exception cref="System.ArgumentNullException"></exception>
		/// <exception cref="GDAConditionalClauseException">Parameters do not exist to build the conditional clause.</exception>
		/// <exception cref="GDAException"></exception>
		/// <returns>Número de linhas afetadas.</returns>
		public virtual int Update(Model objUpdate, string propertiesNamesUpdate)
		{
			return CurrentPersistenceObject.Update(objUpdate, propertiesNamesUpdate);
		}

		/// <summary>
		/// Atualiza os dados no BD.
		/// </summary>
		/// <param name="objUpdate">Objeto contendo os dados a serem atualizados.</param>
		/// <returns>Número de linhas afetadas.</returns>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="GDAException"></exception>
		/// <exception cref="GDAConditionalClauseException"></exception>
		public virtual int Update(Model objUpdate)
		{
			return CurrentPersistenceObject.Update(objUpdate);
		}

		/// <summary>
		/// Remove os dados no BD.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <param name="objDelete">Objeto contendo os dados a serem removidos.</param>
		/// <returns>Número de linhas afetadas.</returns>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="GDAException"></exception>
		/// <exception cref="GDAConditionalClauseException"></exception>
		public virtual int Delete(GDASession session, Model objDelete)
		{
			return CurrentPersistenceObject.Delete(session, objDelete);
		}

		/// <summary>
		/// Remove os dados no BD.
		/// </summary>
		/// <param name="objDelete">Objeto contendo os dados a serem removidos.</param>
		/// <returns>Número de linhas afetadas.</returns>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="GDAException"></exception>
		/// <exception cref="GDAConditionalClauseException"></exception>
		public virtual int Delete(Model objDelete)
		{
			return CurrentPersistenceObject.Delete(objDelete);
		}

		/// <summary>
		/// Remove o conjunto de itens relacionados com a model com base na consulta fornecida.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <param name="query">Filtro para os itens a serem removidos.</param>
		/// <returns>Número de linhas afetadas.</returns>
		/// <exception cref="ArgumentNullException">ObjDelete it cannot be null.</exception>
		/// <exception cref="GDAConditionalClauseException">Parameters do not exist to contruir the conditional clause.</exception>
		/// <exception cref="GDAException"></exception>
		public virtual int Delete(GDASession session, Query query)
		{
			return CurrentPersistenceObject.Delete(session, query);
		}

		/// <summary>
		/// Remove o conjunto de itens relacionados com a model com base na consulta fornecida.
		/// </summary>
		/// <param name="query">Filtro para os itens a serem removidos.</param>
		/// <returns>Número de linhas afetadas.</returns>
		/// <exception cref="ArgumentNullException">ObjDelete it cannot be null.</exception>
		/// <exception cref="GDAConditionalClauseException">Parameters do not exist to contruir the conditional clause.</exception>
		/// <exception cref="GDAException"></exception>
		public virtual int Delete(Query query)
		{
			return CurrentPersistenceObject.Delete(query);
		}

		/// <summary>
		/// Se o registro já existir, atualiza, caso contrário insere.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <param name="objUpdate">Objeto contendo os dados.</param>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="GDAException"></exception>
		/// <exception cref="GDAConditionalClauseException"></exception>
		public virtual uint InsertOrUpdate(GDASession session, Model objUpdate)
		{
			return CurrentPersistenceObject.InsertOrUpdate(session, objUpdate);
		}

		/// <summary>
		/// Se o registro já existir, atualiza, caso contrário insere.
		/// </summary>
		/// <param name="objUpdate">Objeto contendo os dados.</param>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="GDAException"></exception>
		/// <exception cref="GDAConditionalClauseException"></exception>
		public virtual uint InsertOrUpdate(Model objUpdate)
		{
			return CurrentPersistenceObject.InsertOrUpdate(objUpdate);
		}

		/// <summary>
		/// Inseri os dados contidos no objInsert no BD.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <param name="objInsert">Objeto com os dados a serem inseridos.</param>
		/// <param name="propertiesNamesInsert">Nome das propriedades separados por virgula, que serão inseridos no comando.</param>
		/// <param name="direction">Direção que os nomes das propriedades terão no comando. (Default: DirectionPropertiesName.Inclusion)</param>
		/// <returns>Chave inserido.</returns>
		/// <exception cref="ArgumentNullException">ObjInsert it cannot be null.</exception>
		/// <exception cref="GDAException"></exception>
		public virtual uint Insert(GDASession session, object objInsert, string propertiesNamesInsert, DirectionPropertiesName direction)
		{
			return Insert(session, (Model)objInsert, propertiesNamesInsert, direction);
		}

		/// <summary>
		/// Inseri os dados contidos no objInsert no BD.
		/// </summary>
		/// <param name="objInsert">Objeto com os dados a serem inseridos.</param>
		/// <param name="propertiesNamesInsert">Nome das propriedades separados por virgula, que serão inseridos no comando.</param>
		/// <param name="direction">Direção que os nomes das propriedades terão no comando. (Default: DirectionPropertiesName.Inclusion)</param>
		/// <returns>Chave inserido.</returns>
		/// <exception cref="ArgumentNullException">ObjInsert it cannot be null.</exception>
		/// <exception cref="GDAException"></exception>
		public virtual uint Insert(object objInsert, string propertiesNamesInsert, DirectionPropertiesName direction)
		{
			return Insert((Model)objInsert, propertiesNamesInsert, direction);
		}

		/// <summary>
		/// Inseri os dados contidos no objInsert no BD.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <param name="objInsert">Objeto com os dados a serem inseridos.</param>
		/// <param name="propertiesNamesInsert">Nome das propriedades separados por virgula, que serão inseridos no comando.</param>
		/// <returns>Chave inserido.</returns>
		public virtual uint Insert(GDASession session, object objInsert, string propertiesNamesInsert)
		{
			return Insert(session, (Model)objInsert, propertiesNamesInsert);
		}

		/// <summary>
		/// Inseri os dados contidos no objInsert no BD.
		/// </summary>
		/// <param name="objInsert">Objeto com os dados a serem inseridos.</param>
		/// <param name="propertiesNamesInsert">Nome das propriedades separados por virgula, que serão inseridos no comando.</param>
		/// <returns>Chave inserido.</returns>
		public virtual uint Insert(object objInsert, string propertiesNamesInsert)
		{
			return Insert((Model)objInsert, propertiesNamesInsert);
		}

		/// <summary>
		/// Insere os dados no BD.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <param name="objInsert">Objeto contendo os dados a serem inseridos.</param>
		/// <returns>Identidade gerada.</returns>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="GDAException"></exception>
		public virtual uint Insert(GDASession session, object objInsert)
		{
			return Insert(session, (Model)objInsert);
		}

		/// <summary>
		/// Insere os dados no BD.
		/// </summary>
		/// <param name="objInsert">Objeto contendo os dados a serem inseridos.</param>
		/// <returns>Identidade gerada.</returns>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="GDAException"></exception>
		public virtual uint Insert(object objInsert)
		{
			return Insert((Model)objInsert);
		}

		/// <summary>
		/// Atualiza os dados contidos no objUpdate no BD.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <param name="objUpdate">Objeto com os dados a serem atualizados.</param>
		/// <param name="propertiesNamesUpdate">Nome das propriedades separados por virgula, que serão atualizadas no comando.</param>
		/// <param name="direction">Direção que os nomes das propriedades terão no comando. (Default: DirectionPropertiesName.Inclusion)</param>
		/// <exception cref="System.ArgumentNullException"></exception>
		/// <exception cref="GDAConditionalClauseException">Parameters do not exist to build the conditional clause.</exception>
		/// <exception cref="GDAException"></exception>
		/// <returns>Número de linhas afetadas.</returns>
		public virtual int Update(GDASession session, object objUpdate, string propertiesNamesUpdate, DirectionPropertiesName direction)
		{
			return Update(session, (Model)objUpdate, propertiesNamesUpdate, direction);
		}

		/// <summary>
		/// Atualiza os dados contidos no objUpdate no BD.
		/// </summary>
		/// <param name="objUpdate">Objeto com os dados a serem atualizados.</param>
		/// <param name="propertiesNamesUpdate">Nome das propriedades separados por virgula, que serão atualizadas no comando.</param>
		/// <param name="direction">Direção que os nomes das propriedades terão no comando. (Default: DirectionPropertiesName.Inclusion)</param>
		/// <exception cref="System.ArgumentNullException"></exception>
		/// <exception cref="GDAConditionalClauseException">Parameters do not exist to build the conditional clause.</exception>
		/// <exception cref="GDAException"></exception>
		/// <returns>Número de linhas afetadas.</returns>
		public virtual int Update(object objUpdate, string propertiesNamesUpdate, DirectionPropertiesName direction)
		{
			return Update((Model)objUpdate, propertiesNamesUpdate, direction);
		}

		/// <summary>
		/// Atualiza os dados contidos no objUpdate no BD.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <param name="objUpdate">Objeto com os dados a serem atualizados.</param>
		/// <param name="propertiesNamesUpdate">Nome das propriedades separados por virgula, que serão atualizadas no comando.</param>
		/// <returns>Número de linhas afetadas.</returns>
		public virtual int Update(GDASession session, object objUpdate, string propertiesNamesUpdate)
		{
			return Update(session, (Model)objUpdate, propertiesNamesUpdate);
		}

		/// <summary>
		/// Atualiza os dados contidos no objUpdate no BD.
		/// </summary>
		/// <param name="objUpdate">Objeto com os dados a serem atualizados.</param>
		/// <param name="propertiesNamesUpdate">Nome das propriedades separados por virgula, que serão atualizadas no comando.</param>
		/// <returns>Número de linhas afetadas.</returns>
		public virtual int Update(object objUpdate, string propertiesNamesUpdate)
		{
			return Update((Model)objUpdate, propertiesNamesUpdate);
		}

		/// <summary>
		/// Atualiza os dados no BD.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <param name="objUpdate">Objeto contendo os dados a serem atualizados.</param>
		/// <returns>Número de linhas afetadas.</returns>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="GDAException"></exception>
		/// <exception cref="GDAConditionalClauseException"></exception>
		public virtual int Update(GDASession session, object objUpdate)
		{
			return Update(session, (Model)objUpdate);
		}

		/// <summary>
		/// Atualiza os dados no BD.
		/// </summary>
		/// <param name="objUpdate">Objeto contendo os dados a serem atualizados.</param>
		/// <returns>Número de linhas afetadas.</returns>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="GDAException"></exception>
		/// <exception cref="GDAConditionalClauseException"></exception>
		public virtual int Update(object objUpdate)
		{
			return Update((Model)objUpdate);
		}

		/// <summary>
		/// Remove os dados no BD.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <param name="objDelete">Objeto contendo os dados a serem removidos.</param>
		/// <returns>Número de linhas afetadas.</returns>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="GDAException"></exception>
		/// <exception cref="GDAConditionalClauseException"></exception>
		public virtual int Delete(GDASession session, object objDelete)
		{
			return CurrentPersistenceObject.Delete(session, (Model)objDelete);
		}

		/// <summary>
		/// Remove os dados no BD.
		/// </summary>
		/// <param name="objDelete">Objeto contendo os dados a serem removidos.</param>
		/// <returns>Número de linhas afetadas.</returns>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="GDAException"></exception>
		/// <exception cref="GDAConditionalClauseException"></exception>
		public virtual int Delete(object objDelete)
		{
			return Delete((Model)objDelete);
		}

		/// <summary>
		/// Se o registro já existir, atualiza, caso contrário insere.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <param name="objUpdate">Objeto contendo os dados.</param>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="GDAException"></exception>
		/// <exception cref="GDAConditionalClauseException"></exception>
		public virtual uint InsertOrUpdate(GDASession session, object objUpdate)
		{
			return InsertOrUpdate(session, (Model)objUpdate);
		}

		/// <summary>
		/// Se o registro já existir, atualiza, caso contrário insere.
		/// </summary>
		/// <param name="objUpdate">Objeto contendo os dados.</param>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="GDAException"></exception>
		/// <exception cref="GDAConditionalClauseException"></exception>
		public virtual uint InsertOrUpdate(object objUpdate)
		{
			return InsertOrUpdate((Model)objUpdate);
		}

		/// <summary>
		/// Verifica se o valor da propriedade informada existe no banco de dados.
		/// </summary>
		/// <param name="session">Sessão de conexão que será usada na verificação.</param>
		/// <param name="mode">Modo de validação.</param>
		/// <param name="propertyName">Nome da propriedade que será verificada.</param>
		/// <param name="propertyValue">Valor da propriedade que será verificada.</param>
		/// <param name="parent">Elemento que contém a propriedade</param>
		/// <returns>True caso existir.</returns>
		public bool CheckExist(GDASession session, ValidationMode mode, string propertyName, object propertyValue, object parent)
		{
			return CurrentPersistenceObject.CheckExist(session, mode, propertyName, propertyValue, (Model)parent);
		}

		public GDAList<ClassChild> LoadDataForeignKeyParentToChild<ClassChild>(Model parentObj, string groupOfRelationship, InfoSortExpression sortProperty, InfoPaging paging) where ClassChild : new()
		{
			return CurrentPersistenceObject.LoadDataForeignKeyParentToChild<ClassChild>(parentObj, groupOfRelationship, sortProperty, paging);
		}

		public GDAList<ClassChild> LoadDataForeignKeyParentToChild<ClassChild>(Model parentObj, string groupOfRelationship, InfoSortExpression sortProperty) where ClassChild : new()
		{
			return LoadDataForeignKeyParentToChild<ClassChild>(parentObj, groupOfRelationship, sortProperty, null);
		}

		public GDAList<ClassChild> LoadDataForeignKeyParentToChild<ClassChild>(Model parentObj, string groupOfRelationship) where ClassChild : new()
		{
			return LoadDataForeignKeyParentToChild<ClassChild>(parentObj, groupOfRelationship, null, null);
		}

		public GDAList<ClassChild> LoadDataForeignKeyParentToChild<ClassChild>(Model parentObj) where ClassChild : new()
		{
			return LoadDataForeignKeyParentToChild<ClassChild>(parentObj, null);
		}

		public int CountRowForeignKeyParentToChild<ClassChild>(Model parentObj, string groupOfRelationship) where ClassChild : new()
		{
			return CurrentPersistenceObject.CountRowForeignKeyParentToChild<ClassChild>(parentObj, groupOfRelationship);
		}

		public int CountRowForeignKeyParentToChild<ClassChild>(Model parentObj) where ClassChild : new()
		{
			return CountRowForeignKeyParentToChild<ClassChild>(parentObj, null);
		}
	}
}
