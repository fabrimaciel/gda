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
using System.Data;
using System.Reflection;
using System.Data.Common;
using System.Diagnostics;
using GDA.Collections;
using GDA.Provider;
using GDA.Helper;
using GDA.Interfaces;
using GDA.Sql;
using GDA.Sql.InterpreterExpression;
using GDA.Sql.InterpreterExpression.Nodes;
using GDA.Caching;

namespace GDA
{
	public class PersistenceObject<Model> : PersistenceObjectBase<Model> where Model : new()
	{
		/// <summary>
		/// Construtor
		/// </summary>
		/// <param name="providerConfig">Provide para acesso aos dados.</param>
		public PersistenceObject(IProviderConfiguration providerConfig) : base(providerConfig)
		{
		}

		/// <summary>
		/// Recupera os valores da propriedade que se encontra no resultado da consulta.
		/// </summary>
		/// <param name="session">Sess�o para execu��o do comando.</param>
		/// <param name="query">Comando da consulta que ser� executada.</param>
		/// <param name="propertyName">Nome da propriedade que ser� recuperada.</param>
		/// <returns>Valores da propriedade encontrada.</returns>
		public IEnumerable<GDAPropertyValue> GetValues(GDASession session, IQuery query, string propertyName)
		{
			if(string.IsNullOrEmpty(propertyName))
				throw new ArgumentNullException("propertyName");
			if(query == null)
				query = new Query();
			query.ReturnTypeQuery = typeof(Model);
			QueryReturnInfo returnInfo = query.BuildResultInfo<Model>(this.Configuration);
			List<Mapper> listAttr = returnInfo.RecoverProperties;
			string sqlQuery = returnInfo.CommandText;
			IDbConnection conn = CreateConnection(session);
			IDbCommand cmd = CreateCommand(session, conn);
			cmd.CommandType = CommandType.Text;
			bool usingPaging = false;
			if(query.SkipCount > 0 || query.TakeCount > 0)
				usingPaging = true;
			if(listAttr.Count == 0)
				throw new GDAException("Not found properties mappers to model {0}.", typeof(Model).FullName);
			if(returnInfo.Parameters != null)
				for(int i = 0; i < returnInfo.Parameters.Count; i++)
				{
					try
					{
						string newName = (returnInfo.Parameters[i].ParameterName[0] != '?' ? returnInfo.Parameters[i].ParameterName : UserProvider.ParameterPrefix + returnInfo.Parameters[i].ParameterName.Substring(1) + UserProvider.ParameterSuffix);
						sqlQuery = sqlQuery.Replace(returnInfo.Parameters[i].ParameterName, newName);
					}
					catch(Exception ex)
					{
						throw new GDAException("Error on make parameter name '" + returnInfo.Parameters[i].ParameterName + "'.", ex);
					}
					cmd.Parameters.Add(GDA.Helper.GDAHelper.ConvertGDAParameter(cmd, returnInfo.Parameters[i], UserProvider));
				}
			if(usingPaging)
			{
				sqlQuery = UserProvider.SQLCommandLimit(listAttr, sqlQuery, query.SkipCount, query.TakeCount);
			}
			cmd.CommandText = sqlQuery;
			var property = listAttr.Find(delegate(Mapper m) {
				return m.PropertyMapperName == propertyName;
			});
			if(property == null)
				throw new GDAException("Property {0} not found in {1}.", propertyName, typeof(Model).FullName);
			IDataReader dReader = null;
			if(session == null)
			{
				try
				{
					conn.Open();
				}
				catch(Exception ex)
				{
					throw new GDAException(ex);
				}
				GDAConnectionManager.NotifyConnectionOpened(conn);
			}
			try
			{
				SendMessageDebugTrace("CommandText: " + cmd.CommandText);
				using (var executionHandler = Diagnostics.GDATrace.CreateExecutionHandler(cmd))
					try
					{
						dReader = cmd.ExecuteReader();
					}
					catch(Exception ex)
					{
						ex = new GDAException(ex);
						executionHandler.Fail(ex);
						throw ex;
					}
				if(dReader.FieldCount == 0)
					throw new GDAException("The query not return any field.");
				var propertyExists = false;
				while (dReader.Read())
				{
					var i = new GDADataRecord(dReader, null);
					if(!propertyExists && i.GetOrdinal(property.Name) < 0)
						yield break;
					propertyExists = true;
					yield return new GDAPropertyValue(i.GetValue(property.Name), true);
				}
			}
			finally
			{
				if(session == null)
					try
					{
						conn.Close();
					}
					catch
					{
						SendMessageDebugTrace("Error close connection.");
					}
				if(dReader != null)
					dReader.Close();
			}
		}

		/// <summary>
		/// Recupera o valor da propriedade que se encontra no resultado da consulta.
		/// </summary>
		/// <param name="session">Sess�o para execu��o do comando.</param>
		/// <param name="query">Comando da consulta que ser� executada.</param>
		/// <param name="propertyName">Nome da propriedade que ser� recuperada.</param>
		/// <returns>Valor da propriedade encontrada.</returns>
		public GDAPropertyValue GetValue(GDASession session, IQuery query, string propertyName)
		{
			foreach (var i in GetValues(session, query, propertyName))
				return i;
			return new GDAPropertyValue(null, false);
		}

		/// <summary>
		/// Busca todos os dados relacionados com a Model.
		/// </summary>
		/// <returns>Lista com todos os registros relacionados com a model.</returns>
		public GDACursor<Model> Select()
		{
			return Select(null, null);
		}

		/// <summary>
		/// Busca todos os dados relacionados com a Model.
		/// </summary>
		/// <returns>Lista com todos os registros relacionados com a model.</returns>
		public GDACursor<Model> Select(GDASession session)
		{
			return Select(session, null);
		}

		/// <summary>
		/// Busca os dados relacionados com a Model com base em uma consulta submetida.
		/// </summary>
		/// <param name="query">Dados da consulta.</param>
		/// <returns></returns>
		public GDACursor<Model> Select(IQuery query)
		{
			return Select(null, query);
		}

		/// <summary>
		/// Busca os dados relacionados com a Model com base em uma consulta submetida.
		/// </summary>
		/// <param name="session">Sess�o utilizada para a execu��o do comando.</param>
		/// <param name="query">Consulta.</param>
		/// <returns></returns>
		public GDACursor<Model> Select(GDASession session, IQuery query)
		{
			return new GDACursor<Model>(GetCursorParameters(session, query));
		}

		/// <summary>
		/// Busca os dados relacionados com a consulta submetida.
		/// </summary>
		/// <param name="session">Sess�o utilizada para a execu��o do comando.</param>
		/// <param name="query">Consulta.</param>
		/// <returns></returns>
		public override GDADataRecordCursor<Model> SelectToDataRecord(GDASession session, IQuery query)
		{
			return new GDADataRecordCursorEx<Model>(GetCursorParameters(session, query));
		}

		/// <summary>
		/// Remove o item do BD baseando na chave passada.
		/// </summary>
		/// <param name="key">Valor da chave da model.</param>
		/// <returns>N�mero de linhas afetadas.</returns>
		/// <exception cref="GDAException"></exception>
		public int DeleteByKey(uint key)
		{
			List<Mapper> listAttr = MappingManager.GetMappers<Model>(new PersistenceParameterType[] {
				PersistenceParameterType.Key,
				PersistenceParameterType.IdentityKey
			}, new DirectionParameter[] {
				DirectionParameter.Output,
				DirectionParameter.InputOptionalOutput,
				DirectionParameter.InputOutput
			});
			if(listAttr.Count == 0)
				throw new GDAException("There isn't more one primary key identify for object \"" + typeof(Model).FullName + "\"");
			else if(listAttr.Count > 1)
				throw new GDAException("There is more one primary key identify for object \"" + typeof(Model).FullName + "\"");
			Model objDelete = new Model();
			object val = null;
			try
			{
				val = typeof(Convert).GetMethod("To" + listAttr[0].PropertyMapper.PropertyType.Name, new Type[] {
					typeof(uint)
				}).Invoke(null, new object[] {
					key
				});
			}
			catch(Exception ex)
			{
				throw new GDAException("Type key not compatible with uint.", ex);
			}
			listAttr[0].PropertyMapper.SetValue(objDelete, val, null);
			return Delete(objDelete);
		}

		/// <summary>
		/// Carrega os dados aplicando pagina��o.
		/// </summary>
		/// <param name="sqlQuery">Consulta usada na recupera��o.</param>
		/// <param name="startRow">Linha inicial da p�gina de registros a ser recuperada.</param>
		/// <param name="pageSize">Tamanho da p�gina de registros.</param>
		/// <param name="parameters">Parametros usados na consulta.</param>
		/// <returns>Lista com os registro da consulta.</returns>
		public GDACursor<Model> LoadDataAndPaging(string sqlQuery, int startRow, int pageSize)
		{
			return LoadDataWithSortExpression(null, sqlQuery, CommandType.Text, null, new InfoPaging(startRow, pageSize), null);
		}

		/// <summary>
		/// Carrega os dados aplicando pagina��o.
		/// </summary>
		/// <param name="sqlQuery">Consulta usada na recupera��o.</param>
		/// <param name="startRow">Linha inicial da p�gina de registros a ser recuperada.</param>
		/// <param name="pageSize">Tamanho da p�gina de registros.</param>
		/// <param name="parameters">Parametros usados na consulta.</param>
		/// <returns>Lista com os registro da consulta.</returns>
		public GDACursor<Model> LoadDataAndPaging(string sqlQuery, int startRow, int pageSize, params GDAParameter[] parameters)
		{
			return LoadDataWithSortExpression(null, sqlQuery, CommandType.Text, null, new InfoPaging(startRow, pageSize), parameters);
		}

		/// <summary>
		/// Carrega os dados aplicando pagina��o.
		/// </summary>
		/// <param name="session"></param>
		/// <param name="sqlQuery">Consulta usada na recupera��o.</param>
		/// <param name="startRow">Linha inicial da p�gina de registros a ser recuperada.</param>
		/// <param name="pageSize">Tamanho da p�gina de registros.</param>
		/// <param name="parameters">Parametros usados na consulta.</param>
		/// <returns>Lista com os registro da consulta.</returns>
		public GDACursor<Model> LoadDataAndPaging(GDASession session, string sqlQuery, int startRow, int pageSize, params GDAParameter[] parameters)
		{
			return LoadDataWithSortExpression(session, sqlQuery, CommandType.Text, null, new InfoPaging(startRow, pageSize), parameters);
		}

		/// <summary>
		/// Carrega os dados com o retorno da query.
		/// </summary>
		/// <param name="sqlQuery">Query.</param>
		/// <param name="sortExpression">Express�o de ordena��o do comando sql.</param>
		/// <param name="infoPaging">Informa��es para pagina��o do resultado da query.</param>
		/// <param name="parameters">Parametros da query.</param>
		/// <returns>Lista com os dados do retorno da query.</returns>
		/// <exception cref="GDAColumnNotFoundException"></exception>
		/// <exception cref="GDAException"></exception>
		public GDACursor<Model> LoadDataWithSortExpression(string sqlQuery, InfoSortExpression sortExpression, InfoPaging infoPaging, GDAParameter[] parameters)
		{
			return LoadDataWithSortExpression(sqlQuery, CommandType.Text, sortExpression, infoPaging, parameters);
		}

		/// <summary>
		/// Carrega os dados com o retorno da query.
		/// </summary>
		/// <param name="sqlQuery">Query.</param>
		/// <param name="commandType">Tipo do comando a ser executado.</param>
		/// <param name="sortExpression">Express�o de ordena��o do comando sql.</param>
		/// <param name="infoPaging">Informa��es para pagina��o do resultado da query.</param>
		/// <param name="parameters">Parametros da query.</param>
		/// <returns>Lista com os dados do retorno da query.</returns>
		/// <exception cref="GDAColumnNotFoundException"></exception>
		/// <exception cref="GDAException"></exception>
		public GDACursor<Model> LoadDataWithSortExpression(string sqlQuery, System.Data.CommandType commandType, InfoSortExpression sortExpression, InfoPaging infoPaging, GDAParameter[] parameters)
		{
			return LoadDataWithSortExpression(null, sqlQuery, commandType, sortExpression, infoPaging, parameters);
		}

		/// <summary>
		/// Carrega os dados com o retorno da query.
		/// </summary>
		/// <param name="sqlQuery">Query.</param>
		/// <param name="sortExpression">Express�o de ordena��o do comando sql.</param>
		/// <param name="infoPaging">Informa��es para pagina��o do resultado da query.</param>
		/// <param name="parameters">Parametros da query.</param>
		/// <returns>Lista com os dados do retorno da query.</returns>
		/// <exception cref="GDAColumnNotFoundException"></exception>
		/// <exception cref="GDAException"></exception>
		public GDACursor<Model> LoadDataWithSortExpression(GDASession session, string sqlQuery, InfoSortExpression sortExpression, InfoPaging infoPaging, GDAParameter[] parameters)
		{
			return LoadDataWithSortExpression(session, sqlQuery, null, CommandType.Text, sortExpression, infoPaging, parameters);
		}

		/// <summary>
		/// Carrega os dados com o retorno da query.
		/// </summary>
		/// <param name="sqlQuery">Query.</param>
		/// <param name="sortExpression">Express�o de ordena��o do comando sql.</param>
		/// <param name="infoPaging">Informa��es para pagina��o do resultado da query.</param>
		/// <param name="parameters">Parametros da query.</param>
		/// <returns>Lista com os dados do retorno da query.</returns>
		/// <exception cref="GDAColumnNotFoundException"></exception>
		/// <exception cref="GDAException"></exception>
		public GDACursor<Model> LoadDataWithSortExpression(GDASession session, string sqlQuery, System.Data.CommandType commandType, InfoSortExpression sortExpression, InfoPaging infoPaging, GDAParameter[] parameters)
		{
			return LoadDataWithSortExpression(session, sqlQuery, null, commandType, sortExpression, infoPaging, parameters);
		}

		/// <summary>
		/// Carrega os dados com o retorno da query.
		/// </summary>
		/// <param name="session">Sess�o que ser� usado para executar a consulta.</param>
		/// <param name="sqlQuery">Query.</param>
		/// <param name="selectProperties">Nomes das propriedades que ser�o recuperadas na consulta separados por v�rgula.</param>
		/// <param name="sortExpression">Express�o de ordena��o do comando sql.</param>
		/// <param name="infoPaging">Informa��es para pagina��o do resultado da query.</param>
		/// <param name="parameters">Parametros da query.</param>
		/// <returns>Lista com os dados do retorno da query.</returns>
		/// <exception cref="GDAColumnNotFoundException"></exception>
		/// <exception cref="GDAException"></exception>
		public GDACursor<Model> LoadDataWithSortExpression(GDASession session, string sqlQuery, string selectProperties, System.Data.CommandType commandType, InfoSortExpression sortExpression, InfoPaging infoPaging, GDAParameter[] parameters)
		{
			return new GDACursor<Model>(GetCursorParameters(session, sqlQuery, selectProperties, commandType, sortExpression, infoPaging, parameters));
		}

		/// <summary>
		/// Carrega os dados com o retorno da query.
		/// </summary>
		/// <param name="session">Sess�o usada para a execu��o da consulta.</param>
		/// <param name="sqlQuery">Query.</param>
		/// <param name="parameters">Parametros da query.</param>
		/// <returns>Lista com os dados do retorno da query.</returns>
		/// <exception cref="GDAColumnNotFoundException"></exception>
		/// <exception cref="GDAException"></exception>
		public GDACursor<Model> LoadData(GDASession session, string sqlQuery, params GDAParameter[] parameters)
		{
			return LoadDataWithSortExpression(session, sqlQuery, CommandType.Text, null, null, parameters);
		}

		/// <summary>
		/// Carrega os dado com o retorno da stored procedure.
		/// </summary>
		/// <param name="procedure">Dados da stored procedure a ser executada.</param>
		/// <returns>Lista com os dados do retorno.</returns>
		public GDACursor<Model> LoadData(GDAStoredProcedure procedure)
		{
			return LoadData(null, procedure);
		}

		/// <summary>
		/// Carrega os dado com o retorno da stored procedure.
		/// </summary>
		/// <param name="session">Sess�o onde a procedure ser� executada.</param>
		/// <param name="procedure">Dados da stored procedure a ser executada.</param>
		/// <returns>Lista com os dados do retorno.</returns>
		public GDACursor<Model> LoadData(GDASession session, GDAStoredProcedure procedure)
		{
			return LoadData(session, procedure, null);
		}

		/// <summary>
		/// Carrega os dado com o retorno da stored procedure.
		/// </summary>
		/// <param name="session">Sess�o onde a procedure ser� executada.</param>
		/// <param name="procedure">Dados da stored procedure a ser executada.</param>
		/// <param name="selectProperties">Nomes das propriedades que ser�o recuperadas na consulta separados por v�rgula.</param>
		/// <returns>Lista com os dados do retorno.</returns>
		public GDACursor<Model> LoadData(GDASession session, GDAStoredProcedure procedure, string selectProperties)
		{
			return new GDACursor<Model>(GetCursorParameters(session, procedure, selectProperties));
		}

		/// <summary>
		/// Carrega os dados com o retorno da query.
		/// </summary>
		/// <param name="session">Sess�o onde a procedure ser� executada.</param>
		/// <param name="sqlQuery">Query.</param>
		/// <param name="selectProperties">Nomes das propriedades que ser�o recuperadas na consulta separados por v�rgula.</param>
		/// <param name="parameters">Parametros da query.</param>
		/// <returns>Lista com os dados do retorno da query.</returns>
		/// <exception cref="GDAColumnNotFoundException"></exception>
		/// <exception cref="GDAException"></exception>
		public GDACursor<Model> LoadData(GDASession session, string sqlQuery, string selectProperties, params GDAParameter[] parameters)
		{
			return LoadDataWithSortExpression(session, sqlQuery, selectProperties, CommandType.Text, null, null, parameters);
		}

		/// <summary>
		/// Carrega os dados com o retorno da query.
		/// </summary>
		/// <param name="sqlQuery">Query.</param>
		/// <param name="selectProperties">Nomes das propriedades que ser�o recuperadas na consulta separados por v�rgula.</param>
		/// <param name="parameters">Parametros da query.</param>
		/// <returns>Lista com os dados do retorno da query.</returns>
		/// <exception cref="GDAColumnNotFoundException"></exception>
		/// <exception cref="GDAException"></exception>
		public GDACursor<Model> LoadData(string sqlQuery, string selectProperties, params GDAParameter[] parameters)
		{
			return LoadDataWithSortExpression(null, sqlQuery, selectProperties, CommandType.Text, null, null, parameters);
		}

		/// <summary>
		/// Carrega os dados com o retorno da query.
		/// </summary>
		/// <param name="sqlQuery">Query.</param>
		/// <param name="selectProperties">Nomes das propriedades que ser�o recuperadas na consulta separados por v�rgula.</param>
		/// <returns>Lista com os dados do retorno da query.</returns>
		/// <exception cref="GDAColumnNotFoundException"></exception>
		/// <exception cref="GDAException"></exception>
		public GDACursor<Model> LoadData(string sqlQuery, string selectProperties)
		{
			return LoadDataWithSortExpression(null, sqlQuery, selectProperties, CommandType.Text, null, null, null);
		}

		/// <summary>
		/// Carrega os dados com o retorno da query.
		/// </summary>
		/// <param name="sqlQuery">Query.</param>
		/// <param name="parameters">Parametros da query.</param>
		/// <returns>Lista com os dados do retorno da query.</returns>
		/// <exception cref="GDAColumnNotFoundException"></exception>
		/// <exception cref="GDAException"></exception>
		public GDACursor<Model> LoadData(string sqlQuery, params GDAParameter[] parameters)
		{
			return LoadDataWithSortExpression(sqlQuery, null, null, parameters);
		}

		/// <summary>
		/// Carrega os dados com o retorno da query.
		/// </summary>
		/// <param name="sqlQuery">Query.</param>
		/// <returns>Lista com os dados do retorno da query.</returns>
		/// <exception cref="GDAColumnNotFoundException"></exception>
		/// <exception cref="GDAException"></exception>
		public GDACursor<Model> LoadData(string sqlQuery)
		{
			return LoadDataWithSortExpression(sqlQuery, null, null, null);
		}

		/// <summary>
		/// Carrega um unico registro.
		/// </summary>
		/// <param name="session">Sess�o onde a procedure ser� executada.</param>
		/// <param name="sqlQuery">Query.</param>
		/// <returns>Objeto contendo o retorno da query.</returns>
		/// <exception cref="GDAColumnNotFoundException"></exception>
		/// <exception cref="GDAException"></exception>
		public Model LoadOneData(GDASession session, string sqlQuery)
		{
			GDAList<Model> list = LoadDataWithSortExpression(session, sqlQuery, null, CommandType.Text, null, null, null);
			if(list.Count > 0)
				return list[0];
			else
				return default(Model);
		}

		/// <summary>
		/// Carrega um unico registro.
		/// </summary>
		/// <param name="sqlQuery">Query.</param>
		/// <returns>Objeto contendo o retorno da query.</returns>
		/// <exception cref="GDAColumnNotFoundException"></exception>
		/// <exception cref="GDAException"></exception>
		public Model LoadOneData(string sqlQuery)
		{
			GDAList<Model> list = LoadDataWithSortExpression(null, sqlQuery, null, CommandType.Text, null, null, null);
			if(list.Count > 0)
				return list[0];
			else
				return default(Model);
		}

		/// <summary>
		/// Carrega um unico registro.
		/// </summary>
		/// <param name="sqlQuery">Query.</param>
		/// <param name="selectProperties">Nomes das propriedades que ser�o recuperadas na consulta separados por v�rgula.</param>
		/// <returns>Objeto contendo o retorno da query.</returns>
		/// <exception cref="GDAColumnNotFoundException"></exception>
		/// <exception cref="GDAException"></exception>
		public Model LoadOneData(string sqlQuery, string selectProperties)
		{
			return LoadOneData(null, sqlQuery, selectProperties, null);
		}

		/// <summary>
		/// Carrega um unico registro.
		/// </summary>
		/// <param name="sqlQuery">Consulta.</param>
		/// <param name="parameters">Paramentros da consulta.</param>
		/// <returns>Item encontrado ou nulo se o item n�o foi encontrado.</returns>
		/// <exception cref="GDAColumnNotFoundException"></exception>
		/// <exception cref="GDAException"></exception>
		public Model LoadOneData(string sqlQuery, params GDAParameter[] parameters)
		{
			return LoadOneData(null, sqlQuery, null, parameters);
		}

		/// <summary>
		/// Carrega um unico registro.
		/// </summary>
		/// <param name="sqlQuery">Consulta.</param>
		/// <param name="selectProperties">Nomes das propriedades que ser�o recuperadas na consulta separados por v�rgula.</param>
		/// <param name="parameters">Paramentros da consulta.</param>
		/// <returns>Item encontrado ou nulo se o item n�o foi encontrado.</returns>
		/// <exception cref="GDAColumnNotFoundException"></exception>
		/// <exception cref="GDAException"></exception>
		public Model LoadOneData(string sqlQuery, string selectProperties, params GDAParameter[] parameters)
		{
			return LoadOneData(null, sqlQuery, selectProperties, parameters);
		}

		/// <summary>
		/// Carrega um unico registro.
		/// </summary>
		/// <param name="session">Sess�o onde a procedure ser� executada.</param>
		/// <param name="sqlQuery">Consulta.</param>
		/// <param name="parameters">Paramentros da consulta.</param>
		/// <returns>Item encontrado ou nulo se o item n�o foi encontrado.</returns>
		/// <exception cref="GDAColumnNotFoundException"></exception>
		/// <exception cref="GDAException"></exception>
		public Model LoadOneData(GDASession session, string sqlQuery, params GDAParameter[] parameters)
		{
			return LoadOneData(session, sqlQuery, null, parameters);
		}

		/// <summary>
		/// Carrega um unico registro.
		/// </summary>
		/// <param name="session">Sess�o onde a procedure ser� executada.</param>
		/// <param name="sqlQuery">Consulta.</param>
		/// <param name="selectProperties">Nomes das propriedades que ser�o recuperadas na consulta separados por v�rgula.</param>
		/// <param name="parameters">Paramentros da consulta.</param>
		/// <returns>Item encontrado ou nulo se o item n�o foi encontrado.</returns>
		/// <exception cref="GDAColumnNotFoundException"></exception>
		/// <exception cref="GDAException"></exception>
		public Model LoadOneData(GDASession session, string sqlQuery, string selectProperties, params GDAParameter[] parameters)
		{
			GDAList<Model> list = LoadData(session, sqlQuery, selectProperties, parameters);
			if(list.Count > 0)
				return list[0];
			else
				return default(Model);
		}

		/// <summary>
		/// Carrega um unico registro.
		/// </summary>
		/// <param name="procedure">Dados da stored procedure a ser executada.</param>
		/// <param name="selectProperties">Nomes das propriedades que ser�o recuperadas na consulta separados por v�rgula.</param>
		/// <returns>Item encontrado ou nulo se o item n�o foi encontrado.</returns>
		/// <exception cref="GDAColumnNotFoundException"></exception>
		/// <exception cref="GDAException"></exception>
		public Model LoadOneData(GDAStoredProcedure procedure, string selectProperties)
		{
			return LoadOneData(null, procedure, selectProperties);
		}

		/// <summary>
		/// Carrega um unico registro.
		/// </summary>
		/// <param name="procedure">Dados da stored procedure a ser executada.</param>
		/// <returns>Item encontrado ou nulo se o item n�o foi encontrado.</returns>
		/// <exception cref="GDAColumnNotFoundException"></exception>
		/// <exception cref="GDAException"></exception>
		public Model LoadOneData(GDAStoredProcedure procedure)
		{
			return LoadOneData(null, procedure, null);
		}

		/// <summary>
		/// Carrega um unico registro.
		/// </summary>
		/// <param name="session">Sess�o onde a procedure ser� executada.</param>
		/// <param name="procedure">Dados da stored procedure a ser executada.</param>
		/// <returns>Item encontrado ou nulo se o item n�o foi encontrado.</returns>
		/// <exception cref="GDAColumnNotFoundException"></exception>
		/// <exception cref="GDAException"></exception>
		public Model LoadOneData(GDASession session, GDAStoredProcedure procedure)
		{
			return LoadOneData(session, procedure, null);
		}

		/// <summary>
		/// Carrega um unico registro.
		/// </summary>
		/// <param name="session">Sess�o onde a procedure ser� executada.</param>
		/// <param name="procedure">Dados da stored procedure a ser executada.</param>
		/// <param name="selectProperties">Nomes das propriedades que ser�o recuperadas na consulta separados por v�rgula.</param>
		/// <returns>Item encontrado ou nulo se o item n�o foi encontrado.</returns>
		/// <exception cref="GDAColumnNotFoundException"></exception>
		/// <exception cref="GDAException"></exception>
		public Model LoadOneData(GDASession session, GDAStoredProcedure procedure, string selectProperties)
		{
			GDAList<Model> list = LoadData(session, procedure, selectProperties);
			if(list.Count > 0)
				return list[0];
			else
				return default(Model);
		}

		/// <summary>
		/// Recupera os dados do objeto submetido com base na consulta fornecida.
		/// </summary>
		/// <param name="objData">Objeto onde os valores ser�o atribuidos.</param>
		/// <param name="sqlQuery">Consulta para a recupera��o dos dados.</param>
		/// <param name="parameters">Parametros utilizados na consulta.</param>
		/// <returns>Objecto com os valores da recupera��o j� atribu�dos.</returns>
		public Model RecoverData(Model objData, string sqlQuery, params GDAParameter[] parameters)
		{
			return RecoverData(null, objData, sqlQuery, parameters);
		}

		/// <summary>
		/// Recupera os dados do objeto submetido com base na consulta fornecida.
		/// </summary>
		/// <param name="session">Sess�o utilizada para a execu��o do comando.</param>
		/// <param name="objData">Objeto onde os valores ser�o atribuidos.</param>
		/// <param name="sqlQuery">Consulta para a recupera��o dos dados.</param>
		/// <param name="parameters">Parametros utilizados na consulta.</param>
		/// <returns>Objecto com os valores da recupera��o j� atribu�dos.</returns>
		public Model RecoverData(GDASession session, Model objData, string sqlQuery, params GDAParameter[] parameters)
		{
			IDbConnection conn = CreateConnection(session);
			IDbCommand cmd = CreateCommand(session, conn);
			string newName = null;
			if(parameters != null)
				for(int i = 0; i < parameters.Length; i++)
				{
					try
					{
						newName = (parameters[i].ParameterName[0] != '?' ? parameters[i].ParameterName : UserProvider.ParameterPrefix + parameters[i].ParameterName.Substring(1) + UserProvider.ParameterSuffix);
					}
					catch(Exception ex)
					{
						throw new GDAException("Error on make parameter name '" + parameters[i].ParameterName + "'.", ex);
					}
					sqlQuery = sqlQuery.Replace(parameters[i].ParameterName, newName);
					cmd.Parameters.Add(GDA.Helper.GDAHelper.ConvertGDAParameter(cmd, parameters[i], UserProvider));
				}
			cmd.CommandText = sqlQuery;
			if(session == null && conn.State != ConnectionState.Open)
			{
				try
				{
					conn.Open();
				}
				catch(Exception ex)
				{
					throw new GDAException(ex);
				}
				GDAConnectionManager.NotifyConnectionOpened(conn);
			}
			IDataReader dReader = null;
			try
			{
				SendMessageDebugTrace("CommandText: " + cmd.CommandText);
				using (var executionHandler = Diagnostics.GDATrace.CreateExecutionHandler(cmd))
					try
					{
						dReader = cmd.ExecuteReader();
					}
					catch(Exception ex)
					{
						ex = new GDAException(ex);
						executionHandler.Fail(ex);
						throw ex;
					}
				if(dReader.Read())
				{
					var mapping = new TranslatorDataInfoCollection(MappingManager.GetMappers<Model>(null, null));
					mapping.ProcessFieldsPositions(dReader);
					IDataRecord record = dReader;
					RecoverValueOfResult(ref record, mapping, ref objData, false);
				}
				else
				{
					throw new ItemNotFoundException("Item not found with submited parameters.");
				}
			}
			finally
			{
				if(dReader != null)
					dReader.Close();
				if(session == null)
					try
					{
						conn.Close();
						conn.Dispose();
					}
					catch
					{
						SendMessageDebugTrace("Error close connection.");
					}
			}
			return objData;
		}

		/// <summary>
		/// Recupera os dados do objeto submetido com base na consulta fornecida.
		/// </summary>
		/// <param name="objData">Objeto onde os valores ser�o atribuidos.</param>
		/// <param name="sqlQuery">Consulta para a recupera��o dos dados.</param>
		/// <returns>Objecto com os valores da recupera��o j� atribu�dos.</returns>
		public Model RecoverData(Model objData, string sqlQuery)
		{
			return RecoverData(objData, sqlQuery, null);
		}

		/// <summary>
		/// Recupera os dados do objeto submetido tendo como base os valores
		/// da chave contidos no objeto submetido.
		/// </summary>
		/// <param name="objData">Objeto contendo os dados das chaves.</param>
		/// <returns>Model com os dados recuperados.</returns>
		public Model RecoverData(Model objData)
		{
			return RecoverData(null, objData);
		}

		/// <summary>
		/// Recupera os dados do objeto submetido tendo como base os valores
		/// da chave contidos no objeto submetido.
		/// </summary>
		/// <param name="session">Sess�o utilizada para a execu��o do comando.</param>
		/// <param name="objData">Objeto contendo os dados das chaves.</param>
		/// <returns>Model com os dados recuperados.</returns>
		public Model RecoverData(GDASession session, Model objData)
		{
			string sqlParam = "";
			StringBuilder buf = new StringBuilder("SELECT ");
			List<Mapper> properties = Keys;
			if(properties.Count == 0)
				throw new GDAException("In model {0} not found keys for to recover data.", objData.GetType().FullName);
			DirectionParameter[] dp = new DirectionParameter[] {
				DirectionParameter.Input,
				DirectionParameter.InputOutput,
				DirectionParameter.OutputOnlyInsert
			};
			List<Mapper> columns = MappingManager.GetMappers<Model>(null, dp);
			foreach (Mapper column in columns)
			{
				buf.Append(UserProvider.QuoteExpression(column.Name)).Append(",");
			}
			buf.Remove(buf.Length - 1, 1);
			buf.Append(" FROM ").Append(SystemTableName).Append(" ");
			GDAParameter[] parameters = new GDAParameter[properties.Count];
			int i = 0;
			foreach (Mapper mapper in properties)
			{
				if(sqlParam != "")
					sqlParam += " AND ";
				parameters[i] = new GDAParameter(UserProvider.ParameterPrefix + mapper.Name + UserProvider.ParameterSuffix, typeof(Model).GetProperty(mapper.PropertyMapper.Name).GetValue(objData, null));
				sqlParam += UserProvider.QuoteExpression(mapper.Name) + "=" + parameters[i].ParameterName;
				i++;
			}
			buf.Append("WHERE ").Append(sqlParam);
			return RecoverData(session, objData, buf.ToString(), parameters);
		}

		/// <summary>
		/// Carrega os dados da tabela filha tem como base os relacionamento entre ela e a atual model, que s�o
		/// identificados pelo atributos <see cref="PersistenceForeignKeyAttribute"/>.
		/// </summary>
		/// <typeparam name="ClassChild">Tipo da classe que representa a tabela filha.</typeparam>
		/// <param name="parentObj">Objeto contendo as informa��es para fazer o relacionamento.</param>
		/// <param name="groupOfRelationship">Nome do grupo de relacionamento.</param>
		/// <param name="sortExpression">Informa��o sobre o propriedade a ser ordenada.</param>
		/// <param name="paging">Informa��es sobre a pagina��o do resultado.</param>
		/// <returns>Lista tipada do tipo da classe que representa a tabela filha.</returns>
		public GDAList<ClassChild> LoadDataForeignKeyParentToChild<ClassChild>(Model parentObj, string groupOfRelationship, InfoSortExpression sortExpression, InfoPaging paging) where ClassChild : new()
		{
			IBaseDAO<ClassChild> dao = GDAOperations.GetDAO<ClassChild>();
			List<GDAParameter> parameters;
			string sql;
			MakeSqlForeignKeyParentToChild<ClassChild>(parentObj, groupOfRelationship, dao, out sql, out parameters, UserProvider);
			MethodInfo mi = dao.GetType().GetMethod("GetSqlData", GDA.Common.Helper.ReflectionFlags.AllCriteria);
			object ret = null;
			try
			{
				ret = mi.Invoke(dao, new object[] {
					sql,
					parameters,
					sortExpression,
					paging
				});
			}
			catch(Exception ex)
			{
				throw new GDAException(ex.InnerException);
			}
			return (GDAList<ClassChild>)ret;
		}

		/// <summary>
		/// Carrega a consulta sql da tabela filha tem como base os relacionamento entre ela e a atual model, que s�o
		/// identificados pelo atributos <see cref="PersistenceForeignKeyAttribute"/>.
		/// </summary>
		/// <typeparam name="ClassChild">Tipo da classe que representa a tabela filha.</typeparam>
		/// <param name="parentObj">Objeto contendo as informa��es para fazer o relacionamento.</param>
		/// <param name="groupOfRelationship">Nome do grupo de relacionamento.</param>
		/// <param name="dao">DAO relacionada a classe filha.</param>
		/// <param name="sql">Consulta sql do relacionamento.</param>
		/// <param name="parametersWhere">Par�metros gerados.</param>
		private static void MakeSqlForeignKeyParentToChild<ClassChild>(Model parentObj, string groupOfRelationship, IBaseDAO<ClassChild> dao, out string sql, out List<GDAParameter> parametersWhere, IProvider provider) where ClassChild : new()
		{
			Type typeOfClassChild = typeof(ClassChild);
			Type typeModel = typeof(Model);
			GroupOfRelationshipInfo groupInfo = new GroupOfRelationshipInfo(typeOfClassChild, typeModel, groupOfRelationship);
			List<ForeignKeyMapper> mapper = MappingManager.LoadRelationships(typeOfClassChild, groupInfo);
			if(mapper.Count == 0)
				throw new GDAException("ForeignKey " + groupInfo.ToString() + " not found in " + typeof(Model).FullName);
			parametersWhere = new List<GDAParameter>();
			string tableNameRelated = provider.BuildTableName(MappingManager.GetTableName(typeOfClassChild));
			sql = String.Format("SELECT * FROM {0} ", tableNameRelated);
			string where = "";
			foreach (ForeignKeyMapper fk in mapper)
			{
				PersistencePropertyAttribute ppaTbl1 = MappingManager.GetPersistenceProperty(fk.PropertyOfClassRelated);
				PersistencePropertyAttribute ppaTbl2 = MappingManager.GetPersistenceProperty(fk.PropertyModel);
				if(ppaTbl1 == null)
					throw new GDAException("PersistencePropertyAttribute not found in property {0}", fk.PropertyOfClassRelated.Name);
				if(ppaTbl2 == null)
					throw new GDAException("PersistencePropertyAttribute not found in property {0}", fk.PropertyModel.Name);
				parametersWhere.Add(new GDAParameter("?" + ppaTbl1.Name.Replace(" ", "_"), fk.PropertyOfClassRelated.GetValue(parentObj, null)));
				if(where != "")
					where += " AND ";
				where += String.Format("{0}=?{1}", provider.QuoteExpression(ppaTbl1.Name), ppaTbl1.Name.Replace(" ", "_"));
			}
			sql += " WHERE " + where;
		}

		/// <summary>
		/// Carrega os dados da tabela filha tem como base os relacionamento entre ela e a atual model, que s�o
		/// identificados pelo atributos <see cref="PersistenceForeignKeyAttribute"/>.
		/// </summary>
		/// <typeparam name="ClassChild">Tipo da classe que representa a tabela filha.</typeparam>
		/// <param name="parentObj">Objeto contendo as informa��es para fazer o relacionamento.</param>
		/// <param name="groupOfRelationship">Nome do grupo de relacionamento.</param>
		/// <param name="sortExpression">Informa��o sobre o propriedade a ser ordenada.</param>
		/// <returns>Lista tipada do tipo da classe que representa a tabela filha.</returns>
		public GDAList<ClassChild> LoadDataForeignKeyParentToChild<ClassChild>(Model parentObj, string groupOfRelationship, InfoSortExpression sortExpression) where ClassChild : new()
		{
			return LoadDataForeignKeyParentToChild<ClassChild>(parentObj, groupOfRelationship, sortExpression, null);
		}

		/// <summary>
		/// Carrega os dados da tabela filha tem como base os relacionamento entre ela e a atual model, que s�o
		/// identificados pelo atributos <see cref="PersistenceForeignKeyAttribute"/>.
		/// </summary>
		/// <typeparam name="ClassChild">Tipo da classe que representa a tabela filha.</typeparam>
		/// <param name="parentObj">Objeto contendo as informa��es para fazer o relacionamento.</param>
		/// <param name="groupOfRelationship">Nome do grupo de relacionamento.</param>
		/// <param name="paging">Informa��es sobre a pagina��o do resultado.</param>
		/// <returns>Lista tipada do tipo da classe que representa a tabela filha.</returns>
		public GDAList<ClassChild> LoadDataForeignKeyParentToChild<ClassChild>(Model parentObj, string groupOfRelationship, InfoPaging paging) where ClassChild : new()
		{
			return LoadDataForeignKeyParentToChild<ClassChild>(parentObj, groupOfRelationship, null, paging);
		}

		/// <summary>
		/// Carrega os dados da tabela filha tem como base os relacionamento entre ela e a atual model, que s�o
		/// identificados pelo atributos <see cref="PersistenceForeignKeyAttribute"/>.
		/// </summary>
		/// <typeparam name="ClassChild">Tipo da classe que representa a tabela filha.</typeparam>
		/// <param name="parentObj">Objeto contendo as informa��es para fazer o relacionamento.</param>
		/// <param name="groupOfRelationship">Nome do grupo de relacionamento.</param>
		/// <returns>Lista tipada do tipo da classe que representa a tabela filha.</returns>
		public GDAList<ClassChild> LoadDataForeignKeyParentToChild<ClassChild>(Model parentObj, string groupOfRelationship) where ClassChild : new()
		{
			return LoadDataForeignKeyParentToChild<ClassChild>(parentObj, groupOfRelationship, null, null);
		}

		/// <summary>
		/// Captura a quantidade de linhas que da tabela filha tem, se baseado no relacionamento com a atual model.
		/// </summary>
		/// <typeparam name="ClassChild">Tipo da classe que representa a tabela filha.</typeparam>
		/// <param name="parentObj">Objeto contendo as informa��es para fazer o relacionamento.</param>
		/// <param name="groupOfRelationship">Nome do grupo de relacionamento.</param>
		/// <returns>Quantidade de linhas encontradas.</returns>
		public int CountRowForeignKeyParentToChild<ClassChild>(Model parentObj, string groupOfRelationship) where ClassChild : new()
		{
			IBaseDAO<ClassChild> dao = GDAOperations.GetDAO<ClassChild>();
			List<GDAParameter> parameters;
			string sql;
			MakeSqlForeignKeyParentToChild<ClassChild>(parentObj, groupOfRelationship, dao, out sql, out parameters, UserProvider);
			sql = sql.Replace("SELECT *", "SELECT COUNT(*)");
			return ExecuteSqlQueryCount(sql, parameters.ToArray());
		}
	}
}
