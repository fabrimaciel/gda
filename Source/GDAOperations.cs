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
using System.Reflection;
using System.Collections;
using GDA.Collections;
using GDA.Interfaces;
using GDA.Sql;
using GDA.Caching;

namespace GDA
{
	/// <summary>
	/// Classe que disponibiliza as opera��es gerais feitas com as models.
	/// </summary>
	public static class GDAOperations
	{
		/// <summary>
		/// Instancia do m�todo usado para gerar as chaves identidade no sistema.
		/// </summary>
		internal static GenerateKeyHandler GlobalGenerateKey;

		/// <summary>
		/// Instancia do m�todo que � acionado toda vez que um provedor de configura��o
		/// for carregado no sistema.
		/// </summary>
		internal static ProviderConfigurationLoadHandler GlobalProviderConfigurationLoad;

		/// <summary>
		/// Evento acionado quando algum evento de debug de acesso a dados for lan�ado.
		/// </summary>
		public static event DebugTraceDelegate DebugTrace;

		/// <summary>
		/// Adiciona a DAO em mem�ria.
		/// </summary>
		/// <param name="typeModel">Tipo da model da DAO.</param>
		/// <param name="memberDAO"></param>
		internal static void AddMemberDAO(Type typeModel, ISimpleBaseDAO memberDAO)
		{
			try
			{
				MappingManager.MembersDAO[typeModel] = memberDAO;
			}
			catch(NullReferenceException)
			{
			}
		}

		internal static void CallDebugTrace(object sender, string message)
		{
			if(!GDASettings.EnabledDebugTrace)
				return;
			#if PocketPC
			            if (DebugTrace != null)
                DebugTrace(sender, message);
#else
			Helper.ThreadSafeEvents.FireEvent<string>(DebugTrace, sender, message);
			#endif
		}

		/// <summary>
		/// Define o manipulador que ser� respons�vel por gerenciar a cria��o das
		/// chaves identificada no sistema.
		/// </summary>
		/// <param name="handler"></param>
		public static void SetGlobalGenerateKeyHandler(GenerateKeyHandler handler)
		{
			GlobalGenerateKey = handler;
		}

		/// <summary>
		/// Define o manipulador que ser� respons�vel por gerenciar a carga
		/// dos provedores de configura��o no sistema.
		/// </summary>
		/// <param name="handler"></param>
		public static void SetGlobalProviderConfigurationLoadHandler(ProviderConfigurationLoadHandler handler)
		{
			GlobalProviderConfigurationLoad = handler;
		}

		/// <summary>
		/// Recupera as propriedades chave da model.
		/// </summary>
		/// <typeparam name="Model">Tipo a ser usada para recupera as propriedades.</typeparam>
		/// <returns></returns>
		public static List<PropertyInfo> GetPropertiesKey<Model>()
		{
			var result = new List<PropertyInfo>();
			foreach (var i in MappingManager.GetMappers<Model>(new PersistenceParameterType[] {
				PersistenceParameterType.Key,
				PersistenceParameterType.IdentityKey
			}, null))
				result.Add(i.PropertyMapper);
			return result;
		}

		/// <summary>
		/// Recupera as propriedades chave da model.
		/// </summary>
		/// <param name="type">Tipo a ser usada para recupera as propriedades.</param>
		/// <returns></returns>
		public static List<PropertyInfo> GetPropertiesKey(Type type)
		{
			var result = new List<PropertyInfo>();
			foreach (var i in MappingManager.GetMappers(type, new PersistenceParameterType[] {
				PersistenceParameterType.Key,
				PersistenceParameterType.IdentityKey
			}, null))
				result.Add(i.PropertyMapper);
			return result;
		}

		/// <summary>
		/// Recupera do tipo o nome da tabela que ele representa.
		/// </summary>
		/// <typeparam name="Model">Tipo a ser usado para localizar o nome</typeparam>
		/// <returns>Nome da tabela relacionada</returns>
		/// <exception cref="GDATableNameRepresentNotExistsException"></exception>
		public static GDA.Sql.TableName GetTableNameInfo<Model>()
		{
			return MappingManager.GetTableName(typeof(Model));
		}

		/// <summary>
		/// Recupera do tipo o nome da tabela que ele representa.
		/// </summary>
		/// <typeparam name="Model">Tipo a ser usado para localizar o nome</typeparam>
		/// <returns>Nome da tabela relacionada</returns>
		/// <exception cref="GDATableNameRepresentNotExistsException"></exception>
		[Obsolete("Use GetTableNameInfo<Model>")]
		public static string GetTableName<Model>()
		{
			var tableName = MappingManager.GetTableName(typeof(Model));
			return tableName == null ? null : tableName.Name;
		}

		/// <summary>
		/// Recupera do tipo o nome da tabela que ele representa.
		/// </summary>
		/// <param name="type">Tipo a ser usado para localizar o nome</param>
		/// <returns>Nome da tabela relacionada</returns>
		/// <exception cref="GDATableNameRepresentNotExistsException"></exception>
		public static GDA.Sql.TableName GetTableNameInfo(Type type)
		{
			if(type == null)
				throw new ArgumentNullException("type");
			return MappingManager.GetTableName(type);
		}

		/// <summary>
		/// Recupera do tipo o nome da tabela que ele representa.
		/// </summary>
		/// <param name="type">Tipo a ser usado para localizar o nome</param>
		/// <returns>Nome da tabela relacionada</returns>
		/// <exception cref="GDATableNameRepresentNotExistsException"></exception>
		[Obsolete("Use GetTableNameInfo<Model>")]
		public static string GetTableName(Type type)
		{
			if(type == null)
				throw new ArgumentNullException("type");
			var tableName = MappingManager.GetTableName(type);
			return tableName == null ? null : tableName.Name;
		}

		/// <summary>
		/// Inseri o registro no BD.
		/// </summary>
		/// <param name="model">Model contendo os dados a serem inseridos.</param>
		/// <param name="propertiesNamesInsert">Nome das propriedades separados por virgula, que ser�o inseridos no comando.</param>
		/// <param name="direction">Dire��o que os nomes das propriedades ter�o no comando. (Default: DirectionPropertiesName.Inclusion)</param>
		/// <returns>Chave gerada no processo.</returns>
		/// <exception cref="GDAException"></exception>
		/// <exception cref="GDAReferenceDAONotFoundException"></exception>
		public static uint Insert(object model, string propertiesNamesInsert, DirectionPropertiesName direction)
		{
			return Insert(null, model, propertiesNamesInsert, direction);
		}

		/// <summary>
		/// Inseri o registro no BD.
		/// </summary>
		/// <param name="session">Sess�o utilizada para a execu��o do comando.</param>
		/// <param name="model">Model contendo os dados a serem inseridos.</param>
		/// <param name="propertiesNamesInsert">Nome das propriedades separados por virgula, que ser�o inseridos no comando.</param>
		/// <param name="direction">Dire��o que os nomes das propriedades ter�o no comando. (Default: DirectionPropertiesName.Inclusion)</param>
		/// <returns>Chave gerada no processo.</returns>
		/// <exception cref="GDAException"></exception>
		/// <exception cref="GDAReferenceDAONotFoundException"></exception>
		public static uint Insert(GDASession session, object model, string propertiesNamesInsert, DirectionPropertiesName direction)
		{
			return GetDAO(model).Insert(session, model, propertiesNamesInsert, direction);
		}

		/// <summary>
		/// Inseri o registro no BD.
		/// </summary>
		/// <param name="session">Sess�o utilizada para a execu��o do comando.</param>
		/// <param name="model">Model contendo os dados a serem inseridos.</param>
		/// <param name="propertiesNamesInsert">Nome das propriedades separados por virgula, que ser�o inseridos no comando.</param>
		/// <returns>Chave gerada no processo.</returns>
		/// <exception cref="GDAException"></exception>
		/// <exception cref="GDAReferenceDAONotFoundException"></exception>
		public static uint Insert(GDASession session, object model, string propertiesNamesInsert)
		{
			return GetDAO(model).Insert(session, model, propertiesNamesInsert);
		}

		/// <summary>
		/// Inseri o registro no BD.
		/// </summary>
		/// <param name="model">Model contendo os dados a serem inseridos.</param>
		/// <param name="propertiesNamesInsert">Nome das propriedades separados por virgula, que ser�o inseridos no comando.</param>
		/// <returns>Chave gerada no processo.</returns>
		/// <exception cref="GDAException"></exception>
		/// <exception cref="GDAReferenceDAONotFoundException"></exception>
		public static uint Insert(object model, string propertiesNamesInsert)
		{
			return GetDAO(model).Insert(model, propertiesNamesInsert);
		}

		/// <summary>
		/// Inseri o registro no BD.
		/// </summary>
		/// <param name="session">Sess�o utilizada para a execu��o do comando.</param>
		/// <param name="model">Model contendo os dados a serem inseridos.</param>
		/// <returns>Chave gerada no processo.</returns>
		/// <exception cref="GDAException"></exception>
		/// <exception cref="GDAReferenceDAONotFoundException"></exception>
		public static uint Insert(GDASession session, object model)
		{
			return GetDAO(model).Insert(session, model);
		}

		/// <summary>
		/// Inseri o registro no BD.
		/// </summary>
		/// <param name="model">Model contendo os dados a serem inseridos.</param>
		/// <returns>Chave gerada no processo.</returns>
		/// <exception cref="GDAException"></exception>
		/// <exception cref="GDAReferenceDAONotFoundException"></exception>
		public static uint Insert(object model)
		{
			return GetDAO(model).Insert(model);
		}

		/// <summary>
		/// Atualiza os dados contidos no objUpdate no BD.
		/// </summary>
		/// <param name="session">Sess�o utilizada para a execu��o do comando.</param>
		/// <param name="model">Model contendo os dados a serem atualizados.</param>
		/// <param name="propertiesNamesUpdate">Nome das propriedades separadas por virgula, que ser�o atualizadas no comando.</param>
		/// <param name="direction">Dire��o que os nomes das propriedades ter�o no comando. (Default: DirectionPropertiesName.Inclusion)</param>
		/// <exception cref="System.ArgumentNullException"></exception>
		/// <exception cref="GDAConditionalClauseException">Parameters do not exist to build the conditional clause.</exception>
		/// <exception cref="GDAException"></exception>
		/// <returns>N�mero de linhas afetadas.</returns>
		public static int Update(GDASession session, object model, string propertiesNamesUpdate, DirectionPropertiesName direction)
		{
			return GetDAO(model).Update(session, model, propertiesNamesUpdate, direction);
		}

		/// <summary>
		/// Atualiza os dados contidos no objUpdate no BD.
		/// </summary>
		/// <param name="model">Model contendo os dados a serem atualizados.</param>
		/// <param name="propertiesNamesUpdate">Nome das propriedades separadas por virgula, que ser�o atualizadas no comando.</param>
		/// <param name="direction">Dire��o que os nomes das propriedades ter�o no comando. (Default: DirectionPropertiesName.Inclusion)</param>
		/// <exception cref="System.ArgumentNullException"></exception>
		/// <exception cref="GDAConditionalClauseException">Parameters do not exist to build the conditional clause.</exception>
		/// <exception cref="GDAException"></exception>
		/// <returns>N�mero de linhas afetadas.</returns>
		public static int Update(object model, string propertiesNamesUpdate, DirectionPropertiesName direction)
		{
			return GetDAO(model).Update(model, propertiesNamesUpdate, direction);
		}

		#if CLS_3_5
		
        /// <summary>
        /// Recupera o seletor de propriedades que pode ser usado para realizar opera��es de inser��o ou atualiza��o.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model">Instancia da model model que ser� usada.</param>
        /// <returns></returns>
        public static GDAPropertySelector<T> PropertySelector<T>(T model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            return new GDAPropertySelector<T>(model);
        }

        /// <summary>
        /// Recupera o seletor de propriedades que pode ser usado para realizar opera��es de inser��o ou atualiza��o.
        /// </summary>
        /// <typeparam name="T"></typeparam>        
        /// <param name="model">Instancia da model model que ser� usada.</param>
        /// <param name="propertiesSelector">Propriedades que ser�o selecionadas inicialmente.</param>
        /// <returns></returns>
        public static GDAPropertySelector<T> PropertySelector<T>(T model, params System.Linq.Expressions.Expression<Func<T, object>>[] propertiesSelector)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            return new GDAPropertySelector<T>(model).Add(propertiesSelector);
        }
#endif
		/// <summary>
		/// Atualiza os dados contidos no objUpdate no BD.
		/// </summary>
		/// <param name="session">Sess�o utilizada para a execu��o do comando.</param>
		/// <param name="model">Model contendo os dados a serem atualizados.</param>
		/// <param name="propertiesNamesUpdate">Nome das propriedades separadas por virgula, que ser�o atualizadas no comando.</param>
		/// <exception cref="System.ArgumentNullException"></exception>
		/// <exception cref="GDAConditionalClauseException">Parameters do not exist to build the conditional clause.</exception>
		/// <exception cref="GDAException"></exception>
		/// <returns>N�mero de linhas afetadas.</returns>
		public static int Update(GDASession session, object model, string propertiesNamesUpdate)
		{
			return GetDAO(model).Update(session, model, propertiesNamesUpdate);
		}

		/// <summary>
		/// Atualiza os dados contidos no objUpdate no BD.
		/// </summary>
		/// <param name="model">Model contendo os dados a serem atualizados.</param>
		/// <param name="propertiesNamesUpdate">Nome das propriedades separadas por virgula, que ser�o atualizadas no comando.</param>
		/// <exception cref="System.ArgumentNullException"></exception>
		/// <exception cref="GDAConditionalClauseException">Parameters do not exist to build the conditional clause.</exception>
		/// <exception cref="GDAException"></exception>
		/// <returns>N�mero de linhas afetadas.</returns>
		public static int Update(object model, string propertiesNamesUpdate)
		{
			return GetDAO(model).Update(model, propertiesNamesUpdate);
		}

		/// <summary>
		/// Atualiza o registro na BD.
		/// </summary>
		/// <param name="session">Sess�o utilizada para a execu��o do comando.</param>
		/// <param name="model">Model contendo os dados a serem atualizados.</param>
		/// <returns>N�mero de linhas afetadas.</returns>
		/// <exception cref="GDAReferenceDAONotFoundException"></exception>
		/// <exception cref="GDAConditionalClauseException">Parameters do not exist to build the conditional clause.</exception>
		/// <exception cref="GDAException"></exception>
		public static int Update(GDASession session, object model)
		{
			return GetDAO(model).Update(session, model);
		}

		/// <summary>
		/// Atualiza o registro na BD.
		/// </summary>
		/// <param name="model">Model contendo os dados a serem atualizados.</param>
		/// <returns>N�mero de linhas afetadas.</returns>
		/// <exception cref="GDAReferenceDAONotFoundException"></exception>
		/// <exception cref="GDAConditionalClauseException">Parameters do not exist to build the conditional clause.</exception>
		/// <exception cref="GDAException"></exception>
		public static int Update(object model)
		{
			return GetDAO(model).Update(model);
		}

		/// <summary>
		/// Remove o registro da base de dados.
		/// </summary>
		/// <param name="session">Sess�o utilizada para a execu��o do comando.</param>
		/// <param name="model">Model contendo os dados a serem removidos.</param>
		/// <returns>N�mero de linhas afetadas.</returns>
		/// <exception cref="GDAReferenceDAONotFoundException"></exception>
		/// <exception cref="GDAConditionalClauseException">Parameters do not exist to contruir the conditional clause.</exception>
		/// <exception cref="GDAException"></exception>
		public static int Delete(GDASession session, object model)
		{
			return GetDAO(model).Delete(session, model);
		}

		/// <summary>
		/// Remove o registro da base de dados.
		/// </summary>
		/// <param name="model">Model contendo os dados a serem removidos.</param>
		/// <returns>N�mero de linhas afetadas.</returns>
		/// <exception cref="GDAReferenceDAONotFoundException"></exception>
		/// <exception cref="GDAConditionalClauseException">Parameters do not exist to contruir the conditional clause.</exception>
		/// <exception cref="GDAException"></exception>
		public static int Delete(object model)
		{
			return GetDAO(model).Delete(model);
		}

		/// <summary>
		/// Salva os dados na base. Primeiro verifica se o registro existe, se existir ele ser� atualizado
		/// sen�o ele ser� inserido.
		/// </summary>
		/// <param name="session">Sess�o utilizada para a execu��o do comando.</param>
		/// <param name="model">Model contendo os dados a serem salvos.</param>
		/// <returns>A chave do registro inserido ou 0 se ele for atualizado.</returns>
		/// <exception cref="GDAReferenceDAONotFoundException"></exception>
		/// <exception cref="GDAException">Se o tipo de dados utilizado n�o possuir chaves.</exception>
		/// <exception cref="GDAConditionalClauseException">Parameters do not exist to build the conditional clause.</exception>
		public static uint Save(GDASession session, object model)
		{
			return GetDAO(model).InsertOrUpdate(session, model);
		}

		/// <summary>
		/// Salva os dados na base. Primeiro verifica se o registro existe, se existir ele ser� atualizado
		/// sen�o ele ser� inserido.
		/// </summary>
		/// <param name="model">Model contendo os dados a serem salvos.</param>
		/// <returns>A chave do registro inserido ou 0 se ele for atualizado.</returns>
		/// <exception cref="GDAReferenceDAONotFoundException"></exception>
		/// <exception cref="GDAException">Se o tipo de dados utilizado n�o possuir chaves.</exception>
		/// <exception cref="GDAConditionalClauseException">Parameters do not exist to build the conditional clause.</exception>
		public static uint Save(object model)
		{
			return GetDAO(model).InsertOrUpdate(model);
		}

		/// <summary>
		/// Recupera os valores da Model com base nos valores da chaves preenchidas.
		/// </summary>
		/// <param name="session">Sess�o utilizada para a execu��o do comando.</param>
		/// <param name="model">Model contendo os dados que seram usados com base para recuperar os restante dos dados.</param>
		/// <exception cref="GDAColumnNotFoundException"></exception>
		/// <exception cref="GDAException"></exception>
		/// <exception cref="ItemNotFoundException"></exception>
		public static void RecoverData(GDASession session, object model)
		{
			object dao = GetDAO(model);
			MethodInfo mi = dao.GetType().GetMethod("RecoverData", new Type[] {
				typeof(GDASession),
				model.GetType()
			});
			if(mi == null)
				throw new GDAException("Method RecoverData not found in DAO.");
			try
			{
				mi.Invoke(dao, new object[] {
					session,
					model
				});
			}
			catch(TargetInvocationException ex)
			{
				if(ex.InnerException != null)
					throw ex.InnerException;
				else
					throw ex;
			}
		}

		/// <summary>
		/// Recupera os valores da Model com base nos valores da chaves preenchidas.
		/// </summary>
		/// <param name="model">Model contendo os dados que seram usados com base para recuperar os restante dos dados.</param>
		/// <exception cref="GDAColumnNotFoundException"></exception>
		/// <exception cref="GDAException"></exception>
		/// <exception cref="ItemNotFoundException"></exception>
		public static void RecoverData(object model)
		{
			RecoverData(null, model);
		}

		#if CLS_3_5
		
        /// <summary>
        /// Recupera no nome da campo da BD que a propriedade representa.
        /// </summary>
        /// <typeparam name="Model">Tipo da class que contem a propriedade.</typeparam>
        /// <param name="propertyName">Nome da propriedade.</param>
        /// <returns>Nome do campo da BD.</returns>
        public static string GetPropertyDBFieldName<Model>(System.Linq.Expressions.Expression<Func<Model, object>> propertySelector)
        {
             var property = propertySelector.GetMember() as System.Reflection.PropertyInfo;            
             return GetPropertyDBFieldName<Model>(property.Name);
        }

#endif
		/// <summary>
		/// Recupera no nome da campo da BD que a propriedade representa.
		/// </summary>
		/// <typeparam name="Model">Tipo da class que contem a propriedade.</typeparam>
		/// <param name="propertyName">Nome da propriedade.</param>
		/// <returns>Nome do campo da BD.</returns>
		public static string GetPropertyDBFieldName<Model>(string propertyName)
		{
			Type persistenceType = typeof(Model);
			PropertyInfo pi = persistenceType.GetProperty(propertyName);
			if(pi == null)
				throw new GDAException("Property {0} not found in {1}", propertyName, persistenceType.FullName);
			PersistencePropertyAttribute ppa = MappingManager.GetPersistenceProperty(pi);
			if(ppa == null)
				throw new GDAException("DBFieldName not found in Property {0}.", propertyName);
			return ppa.Name;
		}

		/// <summary>
		/// Captura a DAO relacionada com a Model do tipo submetido.
		/// </summary>
		/// <typeparam name="Model">Tipo da Model relacioanda.</typeparam>
		/// <typeparam name="DAO">DAO que representa a model.</typeparam>
		/// <returns></returns>
		/// <exception cref="GDAException"></exception>
		public static DAO GetDAO<Model, DAO>() where Model : new() where DAO : IBaseDAO<Model>
		{
			return (DAO)GetDAO<Model>();
		}

		/// <summary>
		/// Captura a DAO relacionado com a Model do tipo submetido.
		/// </summary>
		/// <typeparam name="T">Model na qual a DAO est� relacionada.</typeparam>
		/// <returns>DAO.</returns>
		/// <exception cref="GDAException"></exception>
		public static IBaseDAO<T> GetDAO<T>() where T : new()
		{
			Type persistenceType = typeof(T);
			if(MappingManager.MembersDAO.ContainsKey(persistenceType))
			{
				return (IBaseDAO<T>)MappingManager.MembersDAO[persistenceType];
			}
			else
			{
				PersistenceBaseDAOAttribute info = MappingManager.GetPersistenceBaseDAOAttribute(persistenceType);
				if(info != null)
				{
					IBaseDAO<T> dao;
					try
					{
						if(info.BaseDAOType.IsGenericType)
						{
							Type t = info.BaseDAOType.MakeGenericType(info.BaseDAOGenericTypes);
							dao = (IBaseDAO<T>)Activator.CreateInstance(t);
						}
						else
							dao = (IBaseDAO<T>)Activator.CreateInstance(info.BaseDAOType);
					}
					catch(InvalidCastException)
					{
						throw new GDAException(String.Format("Invalid cast, type {0} not inherit interface ISimpleBaseDAO.", info.BaseDAOType.FullName));
						;
					}
					catch(Exception ex)
					{
						if(ex is TargetInvocationException)
							throw new GDAException(ex.InnerException);
						else
							throw new GDAException(ex);
					}
					return dao;
				}
				else
				{
					try
					{
						return new BaseDAO<T>();
					}
					catch(Exception ex)
					{
						throw new GDAException("Error to create instance BaseDAO<> for type " + persistenceType.FullName + ".\r\n" + ex.Message, ex);
					}
				}
			}
		}

		/// <summary>
		/// Captura a DAO simples relacionada com a Model do tipo submetido.
		/// </summary>
		/// <typeparam name="T">Model na qual a DAO est� relacionada.</typeparam>
		/// <returns>DAO.</returns>
		/// <exception cref="GDAException"></exception>
		public static ISimpleBaseDAO<T> GetSimpleDAO<T>()
		{
			Type persistenceType = typeof(T);
			if(MappingManager.MembersDAO.ContainsKey(persistenceType))
			{
				return (ISimpleBaseDAO<T>)MappingManager.MembersDAO[persistenceType];
			}
			else
			{
				PersistenceBaseDAOAttribute info = MappingManager.GetPersistenceBaseDAOAttribute(persistenceType);
				if(info != null)
				{
					ISimpleBaseDAO<T> dao;
					try
					{
						if(info.BaseDAOType.IsGenericType)
						{
							Type t = info.BaseDAOType.MakeGenericType(info.BaseDAOGenericTypes);
							dao = (ISimpleBaseDAO<T>)Activator.CreateInstance(t);
						}
						else
							dao = (ISimpleBaseDAO<T>)Activator.CreateInstance(info.BaseDAOType);
					}
					catch(InvalidCastException)
					{
						throw new GDAException(String.Format("Invalid cast, type {0} not inherit interface ISimpleBaseDAO.", info.BaseDAOType.FullName));
						;
					}
					catch(Exception ex)
					{
						if(ex is TargetInvocationException)
							throw new GDAException(ex.InnerException);
						else
							throw new GDAException(ex);
					}
					return dao;
				}
				else
				{
					try
					{
						return new SimpleBaseDAO<T>();
					}
					catch(Exception ex)
					{
						throw new GDAException("Error to create instance SimpleBaseDAO<> for type " + persistenceType.FullName + ".\r\n" + ex.Message, ex);
					}
				}
			}
		}

		/// <summary>
		/// Captura a DAO relacionado com a Model do tipo submetido.
		/// </summary>
		/// <param name="model">Model na qual a DAO est� relacionada.</param>
		/// <returns>DAO.</returns>
		/// <exception cref="GDAException"></exception>
		/// <exception cref="ArgumentNullException"></exception>
		public static ISimpleBaseDAO GetDAO(object model)
		{
			if(model == null)
				throw new ArgumentNullException("model");
			return GetDAO(model.GetType());
		}

		/// <summary>
		/// Captura a DAO relacionado com a Model do tipo submetido.
		/// </summary>
		/// <param name="typeModel">Tipo do model na qual a DAO est� relacionada.</param>
		/// <returns>DAO.</returns>
		/// <exception cref="GDAException"></exception>
		public static ISimpleBaseDAO GetDAO(Type typeModel)
		{
			if(MappingManager.MembersDAO.ContainsKey(typeModel))
			{
				return MappingManager.MembersDAO[typeModel];
			}
			else
			{
				PersistenceBaseDAOAttribute info = MappingManager.GetPersistenceBaseDAOAttribute(typeModel);
				if(info != null)
				{
					ISimpleBaseDAO dao;
					try
					{
						if(info.BaseDAOType.IsGenericType)
						{
							Type t = info.BaseDAOType.MakeGenericType(info.BaseDAOGenericTypes);
							dao = (ISimpleBaseDAO)Activator.CreateInstance(t);
						}
						else
							dao = (ISimpleBaseDAO)Activator.CreateInstance(info.BaseDAOType);
					}
					catch(InvalidCastException)
					{
						throw new GDAException(String.Format("Invalid cast, type {0} not inherit interface ISimpleBaseDAO.", info.BaseDAOType.FullName));
						;
					}
					catch(Exception ex)
					{
						if(ex is TargetInvocationException)
							throw new GDAException(ex.InnerException);
						else
							throw new GDAException(ex);
					}
					return dao;
				}
				else
				{
					try
					{
						return (ISimpleBaseDAO)Activator.CreateInstance(typeof(BaseDAO<object>).GetGenericTypeDefinition().MakeGenericType(typeModel));
					}
					catch(Exception ex)
					{
						throw new GDAException("Error to create instance BaseDAO<> for type " + typeModel.FullName + ".\r\n" + ex.Message, ex);
					}
				}
			}
		}

		/// <summary>
		/// Carrega as lista itens da tabela representada pelo tipo da classe
		/// submetida relacionados com a atual model. Ser� informado tamb�m o grupo
		/// no qual o relacionamento ser� carregado. Utiliza a estrura 1 para N.
		/// </summary>
		/// <typeparam name="ClassRelated">Tipo da classe que representa a tabela do relacionamento.</typeparam>
		/// <param name="model">Model na qual o itens est�o relacionados.</param>
		/// <param name="group">Nome do grupo de relacionamento.</param>
		/// <param name="sortProperty">Informa��o sobre o propriedade a ser ordenada.</param>
		/// <param name="paging">Informa��es sobre a pagina��o do resultado.</param>
		/// <returns>Lista tipada do tipo da classe que representa a tabela do relacionamento.</returns>
		public static GDAList<ClassRelated> LoadRelationship1toN<ClassRelated>(object model, string group, InfoSortExpression sortProperty, InfoPaging paging) where ClassRelated : new()
		{
			object dao = GetDAO(model);
			MethodInfo mi = dao.GetType().GetMethod("LoadDataForeignKeyParentToChild", new Type[] {
				model.GetType(),
				typeof(string),
				typeof(InfoSortExpression),
				typeof(InfoPaging)
			});
			if(mi == null)
				throw new GDAException("DAO of model not suport LoadDataForeignKeyParentToChild.");
			else
				mi = mi.MakeGenericMethod(new Type[] {
					typeof(ClassRelated)
				});
			try
			{
				return (GDAList<ClassRelated>)mi.Invoke(dao, new object[] {
					model,
					group,
					sortProperty,
					paging
				});
			}
			catch(Exception ex)
			{
				throw ex.InnerException;
			}
		}

		/// <summary>
		/// Carrega as lista itens da tabela representada pelo tipo da classe
		/// submetida relacionados com a atual model. Ser� informado tamb�m o grupo
		/// no qual o relacionamento ser� carregado. Utiliza a estrura 1 para N.
		/// </summary>
		/// <typeparam name="ClassRelated">Tipo da classe que representa a tabela do relacionamento.</typeparam>
		/// <param name="model">Model na qual o itens est�o relacionados.</param>
		/// <param name="sortProperty">Informa��o sobre o propriedade a ser ordenada.</param>
		/// <param name="paging">Informa��es sobre a pagina��o do resultado.</param>
		/// <returns>Lista tipada do tipo da classe que representa a tabela do relacionamento.</returns>
		public static GDAList<ClassRelated> LoadRelationship1toN<ClassRelated>(object model, InfoSortExpression sortProperty, InfoPaging paging) where ClassRelated : new()
		{
			return LoadRelationship1toN<ClassRelated>(model, null, sortProperty, paging);
		}

		/// <summary>
		/// Carrega as lista itens da tabela representada pelo tipo da classe
		/// submetida relacionados com a atual model. Ser� informado tamb�m o grupo
		/// no qual o relacionamento ser� carregado. Utiliza a estrura 1 para N.
		/// </summary>
		/// <typeparam name="ClassRelated">Tipo da classe que representa a tabela do relacionamento.</typeparam>
		/// <param name="model">Model na qual o itens est�o relacionados.</param>
		/// <param name="group">Nome do grupo de relacionamento.</param>
		/// <param name="sortProperty">Informa��o sobre o propriedade a ser ordenada.</param>
		/// <returns>Lista tipada do tipo da classe que representa a tabela do relacionamento.</returns>
		public static GDAList<ClassRelated> LoadRelationship1toN<ClassRelated>(object model, string group, InfoSortExpression sortProperty) where ClassRelated : new()
		{
			return LoadRelationship1toN<ClassRelated>(model, group, sortProperty, null);
		}

		/// <summary>
		/// Carrega as lista itens da tabela representada pelo tipo da classe
		/// submetida relacionados com a atual model. Ser� informado tamb�m o grupo
		/// no qual o relacionamento ser� carregado. Utiliza a estrura 1 para N.
		/// </summary>
		/// <typeparam name="ClassRelated">Tipo da classe que representa a tabela do relacionamento.</typeparam>
		/// <param name="model">Model na qual o itens est�o relacionados.</param>
		/// <param name="sortProperty">Informa��o sobre o propriedade a ser ordenada.</param>
		/// <returns>Lista tipada do tipo da classe que representa a tabela do relacionamento.</returns>
		public static GDAList<ClassRelated> LoadRelationship1toN<ClassRelated>(object model, InfoSortExpression sortProperty) where ClassRelated : new()
		{
			return LoadRelationship1toN<ClassRelated>(model, null, sortProperty, null);
		}

		/// <summary>
		/// Carrega as lista itens da tabela representada pelo tipo da classe
		/// submetida relacionados com a atual model. Ser� informado tamb�m o grupo
		/// no qual o relacionamento ser� carregado. Utiliza a estrura 1 para N.
		/// </summary>
		/// <typeparam name="ClassRelated">Tipo da classe que representa a tabela do relacionamento.</typeparam>
		/// <param name="model">Model na qual o itens est�o relacionados.</param>
		/// <param name="group">Nome do grupo de relacionamento.</param>
		/// <param name="paging">Informa��es sobre a pagina��o do resultado.</param>
		/// <returns>Lista tipada do tipo da classe que representa a tabela do relacionamento.</returns>
		public static GDAList<ClassRelated> LoadRelationship1toN<ClassRelated>(object model, string group, InfoPaging paging) where ClassRelated : new()
		{
			return LoadRelationship1toN<ClassRelated>(model, group, null, paging);
		}

		/// <summary>
		/// Carrega as lista itens da tabela representada pelo tipo da classe
		/// submetida relacionados com a atual model. Ser� informado tamb�m o grupo
		/// no qual o relacionamento ser� carregado. Utiliza a estrura 1 para N.
		/// </summary>
		/// <typeparam name="ClassRelated">Tipo da classe que representa a tabela do relacionamento.</typeparam>
		/// <param name="model">Model na qual o itens est�o relacionados.</param>
		/// <param name="paging">Informa��es sobre a pagina��o do resultado.</param>
		/// <returns>Lista tipada do tipo da classe que representa a tabela do relacionamento.</returns>
		public static GDAList<ClassRelated> LoadRelationship1toN<ClassRelated>(object model, InfoPaging paging) where ClassRelated : new()
		{
			return LoadRelationship1toN<ClassRelated>(model, null, null, paging);
		}

		/// <summary>
		/// Carrega as lista itens da tabela representada pelo tipo da classe
		/// submetida relacionados com a atual model. Ser� informado tamb�m o grupo
		/// no qual o relacionamento ser� carregado. Utiliza a estrura 1 para N.
		/// </summary>
		/// <typeparam name="ClassRelated">Tipo da classe que representa a tabela do relacionamento.</typeparam>
		/// <param name="model">Model na qual o itens est�o relacionados.</param>
		/// <param name="group">Nome do grupo de relacionamento.</param>
		/// <returns>Lista tipada do tipo da classe que representa a tabela do relacionamento.</returns>
		public static GDAList<ClassRelated> LoadRelationship1toN<ClassRelated>(object model, string group) where ClassRelated : new()
		{
			return LoadRelationship1toN<ClassRelated>(model, group, null, null);
		}

		/// <summary>
		/// Carrega as lista itens da tabela representada pelo tipo da classe
		/// submetida relacionados com a atual model. Utiliza a estrura 1 para N.
		/// </summary>
		/// <typeparam name="ClassRelated">Tipo da classe que representa a tabela do relacionamento.</typeparam>
		/// <param name="model">Model na qual o itens est�o relacionados.</param>
		/// <returns>Lista tipada do tipo da classe que representa a tabela do relacionamento.</returns>
		public static GDAList<ClassRelated> LoadRelationship1toN<ClassRelated>(object model) where ClassRelated : new()
		{
			return LoadRelationship1toN<ClassRelated>(model, (string)null);
		}

		/// <summary>
		/// Carrega as lista itens da tabela representada pelo tipo da classe
		/// submetida relacionados com a atual model. Ser� informado tamb�m o grupo
		/// no qual o relacionamento ser� carregado. Utiliza a estrura 1 para 1
		/// </summary>
		/// <typeparam name="ClassRelated">Tipo da classe que representa a tabela do relacionamento.</typeparam>
		/// <param name="model">Model na qual o itens est�o relacionados.</param>
		/// <param name="group">Nome do grupo de relacionamento.</param>
		/// <returns>Lista tipada do tipo da classe que representa a tabela do relacionamento.</returns>
		public static ClassRelated LoadRelationship1to1<ClassRelated>(object model, string group) where ClassRelated : new()
		{
			GDAList<ClassRelated> list = LoadRelationship1toN<ClassRelated>(model, group);
			if(list.Count > 1)
				throw new GDAException("There is more one row found for this relationship.");
			else if(list.Count == 1)
				return list[0];
			else
				return default(ClassRelated);
		}

		/// <summary>
		/// Carrega as lista itens da tabela representada pelo tipo da classe
		/// submetida relacionados com a atual model. Utiliza a estrura 1 para 1
		/// </summary>
		/// <typeparam name="ClassRelated">Tipo da classe que representa a tabela do relacionamento.</typeparam>
		/// <param name="model">Model na qual o itens est�o relacionados.</param>
		/// <returns>Lista tipada do tipo da classe que representa a tabela do relacionamento.</returns>
		public static ClassRelated LoadRelationship1to1<ClassRelated>(object model) where ClassRelated : new()
		{
			return LoadRelationship1to1<ClassRelated>(model, null);
		}

		/// <summary>
		/// Carrega a quantidade de itens da tabela representada pelo tipo da classe
		/// submetida relacionados com a atual model. Utiliza a estrura 1 para N.
		/// </summary>
		/// <typeparam name="ClassRelated">Tipo da classe que representa a tabela do relacionamento.</typeparam>
		/// <param name="model">Model na qual o itens est�o relacionados.</param>
		/// <param name="group">Nome do grupo de relacionamento.</param>
		/// <returns>Quantidade de itens tipo da classe que representa a tabela do relacionamento.</returns>
		public static int CountRowRelationship1toN<ClassRelated>(object model, string group) where ClassRelated : new()
		{
			object dao = GetDAO(model);
			MethodInfo mi = dao.GetType().GetMethod("CountRowForeignKeyParentToChild", new Type[] {
				model.GetType(),
				typeof(string)
			});
			if(mi == null)
				throw new GDAException("DAO of model not suport CountRowForeignKeyParentToChild.");
			else
				mi = mi.MakeGenericMethod(new Type[] {
					typeof(ClassRelated)
				});
			try
			{
				return (int)mi.Invoke(dao, new object[] {
					model,
					group
				});
			}
			catch(Exception ex)
			{
				throw ex.InnerException;
			}
		}

		/// <summary>
		/// Carrega a quantidade de itens da tabela representada pelo tipo da classe
		/// submetida relacionados com a atual model. Utiliza a estrura 1 para N.
		/// </summary>
		/// <typeparam name="ClassRelated">Tipo da classe que representa a tabela do relacionamento.</typeparam>
		/// <param name="model">Model na qual o itens est�o relacionados.</param>
		/// <returns>Quantidade de itens tipo da classe que representa a tabela do relacionamento.</returns>
		public static int CountRowRelationship1toN<ClassRelated>(object model) where ClassRelated : new()
		{
			return CountRowRelationship1toN<ClassRelated>(model, null);
		}

		/// <summary>
		/// Carrega os dados com base na consulta informada.
		/// </summary>
		/// <param name="session">Sess�o utilizada para a execu��o do comando.</param>
		/// <returns></returns>
		public static GDACursor<Model> SelectToCursor<Model>(GDASession session) where Model : new()
		{
			return GetDAO<Model>().Select(session);
		}

		/// <summary>
		/// Carrega os dados com base na consulta informada.
		/// </summary>
		/// <returns></returns>
		public static GDACursor<Model> SelectToCursor<Model>() where Model : new()
		{
			return GetDAO<Model>().Select();
		}

		/// <summary>
		/// Carrega os dados com base na consulta informada.
		/// </summary>
		/// <param name="session">Sess�o utilizada para a execu��o do comando.</param>
		/// <returns></returns>
		public static GDAList<Model> Select<Model>(GDASession session) where Model : new()
		{
			return GetDAO<Model>().Select(session);
		}

		/// <summary>
		/// Carrega os dados com base na consulta informada.
		/// </summary>
		/// <returns></returns>
		public static GDAList<Model> Select<Model>() where Model : new()
		{
			return GetDAO<Model>().Select();
		}

		/// <summary>
		/// Recupera a quantidade de registros da tabela no banco.
		/// </summary>
		/// <param name="session">Sess�o utilizada para a execu��o do comando.</param>
		/// <returns>Quantidade de registro encontrados com base na consulta.</returns>
		public static long Count<Model>(GDASession session) where Model : new()
		{
			return GetDAO<Model>().Count(session);
		}

		/// <summary>
		/// Recupera a quantidade de registros da tabela no banco.
		/// </summary>
		/// <returns>Quantidade de registro encontrados com base na consulta.</returns>
		public static long Count<Model>() where Model : new()
		{
			return GetDAO<Model>().Count();
		}

		/// <summary>
		/// Recupera a quantidade de registros com base na Query.
		/// </summary>
		/// <param name="session">Sess�o utilizada para a execu��o do comando.</param>
		/// <param name="query">Consulta usada.</param>
		/// <returns>Quantidade de registro encontrados com base na consulta.</returns>
		public static long Count(GDASession session, IQuery query)
		{
			if(session != null)
				return new DataAccess(session.ProviderConfiguration).Count(session, query);
			else
				return new DataAccess().Count(query);
		}

		/// <summary>
		/// Verifica se o valor da propriedade informada existe no banco de dados.
		/// </summary>
		/// <param name="session">Sess�o de conex�o que ser� usada na verifica��o.</param>
		/// <param name="mode">Modo de valida��o.</param>
		/// <param name="propertyName">Nome da propriedade que ser� verificada.</param>
		/// <param name="propertyValue">Valor da propriedade que ser� verificada.</param>
		/// <param name="parent">Elemento que cont�m a propriedade</param>
		/// <returns>True caso existir.</returns>
		public static bool CheckExist(GDASession session, ValidationMode mode, string propertyName, object propertyValue, object parent)
		{
			return GetDAO(parent).CheckExist(session, mode, propertyName, propertyValue, parent);
		}

		/// <summary>
		/// Verifica se o valor da propriedade informada existe no banco de dados.
		/// </summary>
		/// <param name="mode">Modo de valida��o.</param>
		/// <param name="propertyName">Nome da propriedade que ser� verificada.</param>
		/// <param name="propertyValue">Valor da propriedade que ser� verificada.</param>
		/// <param name="parent">Elemento que cont�m a propriedade</param>
		/// <returns>True caso existir.</returns>
		public static bool CheckExist(ValidationMode mode, string propertyName, object propertyValue, object parent)
		{
			return GetDAO(parent).CheckExist(null, mode, propertyName, propertyValue, parent);
		}
	}
}
