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
using GDA.Interfaces;
using GDA.Collections;
using GDA.Sql;

namespace GDA
{
	/// <summary>
	/// Implementação de uma base DAO simples.
	/// </summary>
	public class SimpleBaseDAO<Model> : ISimpleBaseDAO<Model>
	{
		/// <summary>
		/// PersistenceObject que será responsável por tratar o acesso a dados
		/// </summary>
		private PersistenceObjectBase<Model> currentPersistenceObject;

		/// <summary>
		/// Gets PersistenceObject que será responsável por tratar o acesso a dados
		/// </summary>
		protected PersistenceObjectBase<Model> CurrentPersistenceObject
		{
			get
			{
				return currentPersistenceObject;
			}
		}

		/// <summary>
		/// Gets PersistenceObject que será responsável por tratar o acesso a dados
		/// </summary>
		[Obsolete]
		protected PersistenceObjectBase<Model> ObjPersistence
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
		public SimpleBaseDAO(IProviderConfiguration providerConfig)
		{
			currentPersistenceObject = new PersistenceObjectBase<Model>(providerConfig);
		}

		/// <summary>
		/// Constrói uma instancia de BaseDAO e carrega as configuração do arquivo de configuração.
		/// </summary>
		/// <exception cref="GDAException"></exception>
		public SimpleBaseDAO()
		{
			GDASettings.LoadConfiguration();
			PersistenceProviderAttribute providerAttr = GDA.Caching.MappingManager.GetPersistenceProviderAttribute(typeof(Model));
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
			currentPersistenceObject = new PersistenceObjectBase<Model>(providerConfig);
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
			return CurrentPersistenceObject.Insert(session, (Model)objInsert, propertiesNamesInsert, direction);
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
			return CurrentPersistenceObject.Insert((Model)objInsert, propertiesNamesInsert, direction);
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
			return CurrentPersistenceObject.Insert(session, (Model)objInsert, propertiesNamesInsert, DirectionPropertiesName.Inclusion);
		}

		/// <summary>
		/// Inseri os dados contidos no objInsert no BD.
		/// </summary>
		/// <param name="objInsert">Objeto com os dados a serem inseridos.</param>
		/// <param name="propertiesNamesInsert">Nome das propriedades separados por virgula, que serão inseridos no comando.</param>
		/// <returns>Chave inserido.</returns>
		public virtual uint Insert(object objInsert, string propertiesNamesInsert)
		{
			return CurrentPersistenceObject.Insert((Model)objInsert, propertiesNamesInsert);
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
			return CurrentPersistenceObject.Insert(session, (Model)objInsert, null, DirectionPropertiesName.Inclusion);
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
			return CurrentPersistenceObject.Insert((Model)objInsert);
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
			return CurrentPersistenceObject.Update(session, (Model)objUpdate, propertiesNamesUpdate, direction);
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
			return CurrentPersistenceObject.Update((Model)objUpdate, propertiesNamesUpdate, direction);
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
			return CurrentPersistenceObject.Update(session, (Model)objUpdate, propertiesNamesUpdate, DirectionPropertiesName.Inclusion);
		}

		/// <summary>
		/// Atualiza os dados contidos no objUpdate no BD.
		/// </summary>
		/// <param name="objUpdate">Objeto com os dados a serem atualizados.</param>
		/// <param name="propertiesNamesUpdate">Nome das propriedades separados por virgula, que serão atualizadas no comando.</param>
		/// <returns>Número de linhas afetadas.</returns>
		public virtual int Update(object objUpdate, string propertiesNamesUpdate)
		{
			return CurrentPersistenceObject.Update((Model)objUpdate, propertiesNamesUpdate);
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
			return CurrentPersistenceObject.Update(session, (Model)objUpdate);
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
			return CurrentPersistenceObject.Update((Model)objUpdate);
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
			return CurrentPersistenceObject.Delete((Model)objDelete);
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
			return CurrentPersistenceObject.InsertOrUpdate(session, (Model)objUpdate);
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
			return CurrentPersistenceObject.InsertOrUpdate((Model)objUpdate);
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
	}
}
