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
using System.Data;
using System.Data.Common;
using GDA.Collections;
using GDA.Sql;

namespace GDA
{
	public class DataAccess : IPersistenceObjectBase
	{
		/// <summary>
		/// Provider de configuração para acessar os dados.
		/// </summary>
		private IProviderConfiguration providerConfig;

		/// <summary>
		/// Provider utilizado para conexão com BD.
		/// </summary>
		public IProvider UserProvider
		{
			get
			{
				return providerConfig.Provider;
			}
		}

		/// <summary>
		/// Provider de configuração.
		/// </summary>
		public IProviderConfiguration Configuration
		{
			get
			{
				return providerConfig;
			}
		}

		/// <summary>
		/// Construtor.
		/// </summary>
		/// <param name="providerConfiguration">Provider para acesso aos dados.</param>
		/// <exception cref="ArgumentNullException">Excessão lançada se o argumento providerConfig for nulo.</exception>
		public DataAccess(IProviderConfiguration providerConfiguration)
		{
			if(providerConfiguration == null)
				throw new ArgumentNullException("providerConfig");
			this.providerConfig = providerConfiguration;
		}

		/// <summary>
		/// Construtor padrão que utiliza o ProviderConfiguration padrão do sistema.
		/// </summary>
		public DataAccess() : this(GDASettings.DefaultProviderConfiguration)
		{
		}

		/// <summary>
		/// Envia uma mensagem para o debug.
		/// </summary>
		/// <param name="message">Mensagem a ser enviada.</param>
		protected void SendMessageDebugTrace(string message)
		{
			#if DEBUG
			            //System.Diagnostics.Debug.WriteLine(message);
#endif
			if(GDASettings.EnabledDebugTrace)
			{
				try
				{
					GDAOperations.CallDebugTrace(this, message);
				}
				catch(Exception ex)
				{
					throw new Diagnostics.GDATraceException(ex);
				}
			}
		}

		/// <summary>
		/// Convert o valor do tipo um para o tipo 2.
		/// </summary>
		/// <param name="value">Valor</param>
		/// <param name="sourceType">Tipo de destino.</param>
		/// <param name="destinationType">Tipo de origem.</param>
		/// <returns>Valor convertido para o tipo 2.</returns>
		public static object ConvertType(object value, Type sourceType, Type destinationType)
		{
			return ValueConverterManager.Instance.Convert(value, destinationType, System.Globalization.CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Converte o valor para o tipo de destino.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="destinationType"></param>
		/// <returns></returns>
		internal protected static object ConvertValue(object value, Type destinationType)
		{
			if(value != null)
			{
				var type2 = value.GetType();
				value = ConvertType(value, type2, destinationType);
			}
			return value;
		}

		/// <summary>
		/// Envia uma mensagem para o debug.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="message"></param>
		internal static void SendMessageDebugTrace(object sender, string message)
		{
			#if DEBUG
			            //System.Diagnostics.Debug.WriteLine(message);
#endif
			if(GDASettings.EnabledDebugTrace)
				GDAOperations.CallDebugTrace(sender, message);
		}

		/// <summary>
		/// Cria uma instância de conexão com o BD, 
		/// e caso uma sessão seja informada a conexão é carrega
		/// com base nela.
		/// </summary>
		/// <param name="session">Dados da sessão relacionada.</param>
		/// <returns>Nova instância de conexão.</returns>
		internal protected IDbConnection CreateConnection(GDASession session)
		{
			if(session != null)
			{
				if(session.ProviderConfiguration == null)
					session.DefineConfiguration(this.providerConfig);
				return session.CurrentConnection;
			}
			else
			{
				var connection = Configuration.CreateConnection();
				GDAConnectionManager.NotifyConnectionCreated(connection);
				return connection;
			}
		}

		/// <summary>
		/// Cria uma instância de command. Caso uma sessão for informada o command
		/// é carregado com base na sessão.
		/// </summary>
		/// <param name="session">Dados da sessão relacionada.</param>
		/// <param name="connection">Conexão de onde o comando será executado</param>
		/// <returns>Nova instância do command.</returns>
		public IDbCommand CreateCommand(GDASession session, IDbConnection connection)
		{
			if(session != null)
				return session.CreateCommand();
			else
			{
				IDbCommand cmd = UserProvider.CreateCommand();
				cmd.Connection = connection;
				cmd.CommandTimeout = GDASession.DefaultCommandTimeout;
				return cmd;
			}
		}

		/// <summary>
		/// Recupera os parametros necessário para executar a consulta.
		/// </summary>
		/// <param name="session">Sessão onde será executada a consulta.</param>
		/// <param name="procedure">Procedure que será executada.</param>
		/// <returns></returns>
		internal GDACursorParameters GetLoadResultCursorParameters(GDASession session, GDAStoredProcedure procedure)
		{
			IDbConnection conn = CreateConnection(session);
			IDbCommand cmd = CreateCommand(session, conn);
			PrepareCommand(session, procedure, cmd);
			return new GDACursorParameters(this.UserProvider, session, conn, cmd, null, false, 0, 0, null);
		}

		/// <summary>
		/// Prepara o comando da Storedprocedure para execução.
		/// </summary>
		/// <param name="session"></param>
		/// <param name="procedure">Storedprocedure que será processada.</param>
		/// <param name="cmd">Comando que será preparado.</param>
		public void PrepareCommand(GDASession session, GDAStoredProcedure procedure, IDbCommand cmd)
		{
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandTimeout = procedure.CommandTimeout;
			cmd.CommandText = procedure.Name;
			foreach (GDAParameter param in procedure)
				cmd.Parameters.Add(GDA.Helper.GDAHelper.ConvertGDAParameter(cmd, param, UserProvider));
		}

		/// <summary>
		/// Recupera os parametros necessário para executar a consulta.
		/// </summary>
		/// <param name="session">Sessão onde será executada a consulta.</param>
		/// <param name="commandType"></param>
		/// <param name="commandTimeout"></param>
		/// <param name="paging"></param>
		/// <param name="sqlQuery">Consulta.</param>
		/// <param name="parameters">Parametros para a consulta.</param>
		/// <returns></returns>
		internal GDACursorParameters GetLoadResultCursorParameters(GDASession session, CommandType commandType, int commandTimeout, string sqlQuery, InfoPaging paging, params GDAParameter[] parameters)
		{
			IDbConnection conn = CreateConnection(session);
			IDbCommand cmd = CreateCommand(session, conn);
			cmd.CommandType = commandType;
			if(commandTimeout >= 0)
				cmd.CommandTimeout = commandTimeout;
			sqlQuery = PrepareCommand(session, cmd, sqlQuery, paging, parameters);
			return new GDACursorParameters(UserProvider, session, conn, cmd, null, paging != null, paging == null ? 0 : paging.StartRow, paging == null ? 0 : paging.PageSize, (sender, e) =>  {
				for(int i = 0; i < cmd.Parameters.Count; i++)
				{
					var dataParamter = (IDbDataParameter)cmd.Parameters[i];
					if(dataParamter.Direction == ParameterDirection.Output || dataParamter.Direction == ParameterDirection.ReturnValue)
						parameters[i].Value = ((IDbDataParameter)cmd.Parameters[i]).Value;
				}
			});
		}

		/// <summary>
		/// Prepara o comando que será executado.
		/// </summary>
		/// <param name="session"></param>
		/// <param name="command">Instancia do comando que será preparada.</param>
		/// <param name="commandText">Texto do comando.</param>
		/// <param name="paging">Informações da paginação.</param>
		/// <param name="parameters">Parametros que serão processados.</param>
		/// <returns></returns>
		public string PrepareCommand(GDASession session, IDbCommand command, string commandText, InfoPaging paging, GDAParameter[] parameters)
		{
			if(parameters != null)
				foreach (GDAParameter param in parameters)
				{
					try
					{
						string newName = (param.ParameterName[0] != '?' ? param.ParameterName : UserProvider.ParameterPrefix + param.ParameterName.Substring(1) + UserProvider.ParameterSuffix);
						commandText = commandText.Replace(param.ParameterName, newName);
					}
					catch(Exception ex)
					{
						throw new GDAException("Error on make parameter name '" + param.ParameterName + "'.", ex);
					}
					command.Parameters.Add(GDA.Helper.GDAHelper.ConvertGDAParameter(command, param, UserProvider));
				}
			var provider = session != null ? session.ProviderConfiguration.Provider : GDASettings.DefaultProviderConfiguration.Provider;
			if(provider.SupportSQLCommandLimit && paging != null)
				commandText = provider.SQLCommandLimit(!string.IsNullOrEmpty(paging.KeyFieldName) ? new List<Mapper> {
					new Mapper(null, paging.KeyFieldName, DirectionParameter.InputOutput, PersistenceParameterType.Key, 0, null, null)
				} : null, commandText, paging.StartRow, paging.PageSize);
			command.CommandText = commandText;
			return commandText;
		}

		/// <summary>
		/// Recupera os valores do resultado e preenche o objeto submetido.
		/// </summary>
		/// <param name="dReader">DataReader contendo os dados.</param>
		/// <param name="recoverDataInfos">Lista dos campos a serem carregados.</param>
		/// <param name="objItem">Objeto que será preenchido.</param>
		/// <param name="implementIObjectDataRecord">Identifica se o tipo implementa o <see cref="IObjectDataRecord"/>.</param>
		/// <exception cref="GDAColumnNotFoundException"></exception>
		/// <exception cref="GDAException"></exception>
		public static void RecoverValueOfResult(ref IDataRecord dReader, TranslatorDataInfoCollection recoverDataInfos, ref object objItem, bool implementIObjectDataRecord)
		{
			if(objItem == null)
				return;
			foreach (TranslatorDataInfo rdi in recoverDataInfos)
			{
				if(rdi.FieldPosition < 0)
					continue;
				object value;
				try
				{
					value = dReader[rdi.FieldPosition];
				}
				catch(KeyNotFoundException)
				{
					throw new GDAColumnNotFoundException(rdi.FieldName, "");
				}
				catch(Exception ex)
				{
					throw new GDAException("Error to recover value of field: " + rdi.FieldName + "; Exception: " + ex.Message, ex);
				}
				if(value == DBNull.Value)
					value = null;
				var sourceTypeName = value != null ? value.GetType().Name : "null";
				if(rdi.PathLength > 0)
					try
					{
						rdi.SetValue(objItem, ConvertValue(value, rdi.Property.PropertyType));
					}
					catch(Exception ex)
					{
						if(value != null)
							throw new GDAException(String.Format("Error to convert type {0} to type {1} of field:{2}.", sourceTypeName, rdi.Property.PropertyType.Name, rdi.FieldName), ex);
						else
							throw new GDAException(String.Format("Error to convert type {0} to null", rdi.Property.PropertyType.Name), ex);
					}
			}
			if(implementIObjectDataRecord)
			{
				var objetDataRecord = (IObjectDataRecord)objItem;
				if(objetDataRecord.LoadMappedsRecordFields)
				{
					for(int i = 0; i < dReader.FieldCount; i++)
					{
						var value = dReader[i];
						objetDataRecord.InsertRecordField(dReader.GetName(i), value == DBNull.Value ? null : value);
					}
				}
				else
				{
					for(int i = 0; i < dReader.FieldCount; i++)
					{
						var fieldName = dReader.GetName(i);
						if(recoverDataInfos.FindIndex(f => string.Compare(f.FieldName, fieldName, true) == 0) < 0)
						{
							var value = dReader[i];
							objetDataRecord.InsertRecordField(fieldName, value == DBNull.Value ? null : value);
						}
					}
				}
			}
		}

		/// <summary>
		/// Cria uma instância de conexão com o BD.
		/// </summary>
		/// <returns>Nova instância de conexão.</returns>
		public IDbConnection CreateConnection()
		{
			IDbConnection conn = UserProvider.CreateConnection();
			conn.ConnectionString = this.Configuration.ConnectionString;
			return conn;
		}

		/// <summary>
		/// Executa a stored procedure.
		/// </summary>
		/// <param name="procedure">Dados da stored procedure.</param>
		/// <returns>Número de linhas afetadas.</returns>
		public int ExecuteCommand(GDAStoredProcedure procedure)
		{
			return ExecuteCommand(null, procedure);
		}

		/// <summary>
		/// Executa a stored procedure usando a sessão.
		/// </summary>
		/// <param name="session"></param>
		/// <param name="procedure"></param>
		/// <returns>Número de linhas afetadas.</returns>
		public int ExecuteCommand(GDASession session, GDAStoredProcedure procedure)
		{
			IDbConnection conn = CreateConnection(session);
			IDbCommand cmd = CreateCommand(session, conn);
			int valueReturn = 0;
			try
			{
				procedure.Prepare(cmd, UserProvider);
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
				try
				{
					SendMessageDebugTrace(cmd.CommandText);
					valueReturn = cmd.ExecuteNonQuery();
				}
				catch(Exception ex)
				{
					throw new GDAException("StoredProcedure: " + cmd.CommandText + ". --> " + ex.Message, ex);
				}
				for(int i = 0; i < cmd.Parameters.Count; i++)
					procedure[i] = ((IDbDataParameter)cmd.Parameters[i]).Value;
			}
			finally
			{
				try
				{
					cmd.Dispose();
					cmd = null;
				}
				finally
				{
					if(session == null)
					{
						conn.Close();
						conn.Dispose();
					}
				}
			}
			return valueReturn;
		}

		public object ExecuteScalar(GDAStoredProcedure procedure)
		{
			return ExecuteScalar(null, procedure);
		}

		public object ExecuteScalar(GDASession session, GDAStoredProcedure procedure)
		{
			IDbConnection conn = CreateConnection(session);
			IDbCommand cmd = CreateCommand(session, conn);
			object valueReturn = null;
			try
			{
				procedure.Prepare(cmd, UserProvider);
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
				SendMessageDebugTrace(cmd.CommandText);
				try
				{
					using (var executionHandler = Diagnostics.GDATrace.CreateExecutionHandler(cmd))
						try
						{
							valueReturn = cmd.ExecuteScalar();
						}
						catch(Exception ex)
						{
							executionHandler.Fail(ex);
							throw ex;
						}
				}
				catch(Exception ex)
				{
					throw new GDAException("StoredProcedure: " + cmd.CommandText + "; --> " + ex.Message, ex);
				}
				for(int i = 0; i < cmd.Parameters.Count; i++)
					procedure[i] = ((IDbDataParameter)cmd.Parameters[i]).Value;
			}
			finally
			{
				try
				{
					cmd.Dispose();
					cmd = null;
				}
				finally
				{
					if(session == null)
					{
						conn.Close();
						conn.Dispose();
					}
				}
			}
			return valueReturn;
		}

		/// <summary>
		/// Executa comandos sql.
		/// </summary>
		/// <param name="session">Sessão para execução do comando.</param>
		/// <param name="commandType">Tipo do comando a ser executado.</param>
		/// <param name="commandTimeout">commandTimeout</param>
		/// <param name="sqlQuery">Causa sql a ser executada.</param>
		/// <param name="parameters">Parametros a serem passados para o comando.</param>
		/// <returns>Número de linhas afetadas.</returns>
		/// <exception cref="ArgumentNullException">sqlQuery</exception>
		/// <exception cref="ArgumentException">sqlQuery cannot empty.</exception>
		public int ExecuteCommand(GDASession session, CommandType commandType, int commandTimeout, string sqlQuery, params GDAParameter[] parameters)
		{
			if(sqlQuery == null)
				throw new ArgumentNullException("sqlQuery");
			else if(sqlQuery == "")
				throw new ArgumentException("sqlQuery cannot empty.");
			int valueReturn = 0;
			IDbConnection conn = CreateConnection(session);
			IDbCommand command = CreateCommand(session, conn);
			try
			{
				SendMessageDebugTrace(sqlQuery);
				string newParameterName = null;
				if(parameters != null)
					for(int i = 0; i < parameters.Length; i++)
					{
						newParameterName = parameters[i].ParameterName.Replace("?", UserProvider.ParameterPrefix) + UserProvider.ParameterSuffix;
						sqlQuery = sqlQuery.Replace(parameters[i].ParameterName, newParameterName);
						parameters[i].ParameterName = newParameterName;
						IDbDataParameter p = GDA.Helper.GDAHelper.ConvertGDAParameter(command, parameters[i], UserProvider);
						command.Parameters.Add(p);
					}
				command.CommandText = sqlQuery;
				command.CommandType = commandType;
				command.CommandTimeout = commandTimeout;
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
				try
				{
					SendMessageDebugTrace(command.CommandText);
					using (var executionHandler = Diagnostics.GDATrace.CreateExecutionHandler(command))
						try
						{
							executionHandler.RowsAffects = valueReturn = command.ExecuteNonQuery();
						}
						catch(Exception ex)
						{
							executionHandler.Fail(ex);
							throw ex;
						}
					SendMessageDebugTrace("Return: " + valueReturn.ToString());
				}
				catch(Exception ex)
				{
					throw new GDAException("SqlQuery: " + sqlQuery + "; --> " + ex.Message, ex);
				}
				for(int i = 0; i < command.Parameters.Count; i++)
					parameters[i].Value = ((IDbDataParameter)command.Parameters[i]).Value;
			}
			finally
			{
				try
				{
					command.Dispose();
					command = null;
				}
				finally
				{
					if(session == null)
					{
						conn.Close();
						conn.Dispose();
					}
				}
			}
			return valueReturn;
		}

		/// <summary>
		/// Executa comandos sql.
		/// </summary>
		/// <param name="session">Sessão para execução do comando.</param>
		/// <param name="sqlQuery">Causa sql a ser executada.</param>
		/// <param name="parameters">Parametros a serem passados para o comando.</param>
		/// <returns>Número de linhas afetadas.</returns>
		/// <exception cref="ArgumentNullException">sqlQuery</exception>
		/// <exception cref="ArgumentException">sqlQuery cannot empty.</exception>
		public int ExecuteCommand(GDASession session, string sqlQuery, params GDAParameter[] parameters)
		{
			return ExecuteCommand(session, CommandType.Text, GDASession.DefaultCommandTimeout, sqlQuery, parameters);
		}

		/// <summary>
		/// Executa comandos sql.
		/// </summary>
		/// <param name="sqlQuery">Causa sql a ser executada.</param>
		/// <param name="parameters">Parametros a serem passados para o comando.</param>
		/// <returns>Número de linhas afetadas.</returns>
		/// <exception cref="ArgumentNullException">sqlQuery</exception>
		/// <exception cref="ArgumentException">sqlQuery cannot empty.</exception>
		public int ExecuteCommand(string sqlQuery, params GDAParameter[] parameters)
		{
			return ExecuteCommand(null, CommandType.Text, GDASession.DefaultCommandTimeout, sqlQuery, parameters);
		}

		/// <summary>
		/// Executa comandos sql.
		/// </summary>
		/// <param name="sqlQuery">Causa sql a ser executada.</param>
		public int ExecuteCommand(string sqlQuery)
		{
			return ExecuteCommand(sqlQuery, null);
		}

		/// <summary>
		/// Executa uma consulta que retorna somente um campo.
		/// </summary>
		/// <param name="sqlQuery">Consulta.</param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public int ExecuteSqlQueryCount(GDASession session, string sqlQuery, params GDAParameter[] parameters)
		{
			object value = ExecuteScalar(session, sqlQuery, parameters);
			if(value != null)
				return int.Parse(value.ToString());
			else
				return 0;
		}

		/// <summary>
		/// Executa uma consulta que retorna somente um campo.
		/// </summary>
		/// <param name="sqlQuery">Consulta.</param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public int ExecuteSqlQueryCount(string sqlQuery, params GDAParameter[] parameters)
		{
			return ExecuteSqlQueryCount(null, sqlQuery, parameters);
		}

		/// <summary>
		/// Executa uma consulta que retorna somente um campo.
		/// </summary>
		/// <param name="sqlQuery">Consulta.</param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public object ExecuteScalar(string sqlQuery, params GDAParameter[] parameters)
		{
			return ExecuteScalar(null, sqlQuery, parameters);
		}

		/// <summary>
		/// Executa uma consulta que retorna somente um campo.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <param name="sqlQuery">Consulta.</param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public object ExecuteScalar(GDASession session, CommandType commandType, int commandTimeout, string sqlQuery, params GDAParameter[] parameters)
		{
			object returnValue;
			IDbConnection conn = CreateConnection(session);
			IDbCommand cmd = CreateCommand(session, conn);
			try
			{
				if(parameters != null)
					for(int i = 0; i < parameters.Length; i++)
					{
						string newName = (parameters[i].ParameterName[0] != '?' ? parameters[i].ParameterName : UserProvider.ParameterPrefix + parameters[i].ParameterName.Substring(1) + UserProvider.ParameterSuffix);
						sqlQuery = sqlQuery.Replace(parameters[i].ParameterName, newName);
						cmd.Parameters.Add(GDA.Helper.GDAHelper.ConvertGDAParameter(cmd, parameters[i], UserProvider));
					}
				cmd.CommandText = sqlQuery;
				cmd.CommandType = commandType;
				cmd.CommandTimeout = commandTimeout;
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
				try
				{
					SendMessageDebugTrace(cmd.CommandText);
					using (var executionHandler = Diagnostics.GDATrace.CreateExecutionHandler(cmd))
						try
						{
							returnValue = cmd.ExecuteScalar();
						}
						catch(Exception ex)
						{
							executionHandler.Fail(ex);
							throw ex;
						}
					if(returnValue != DBNull.Value && returnValue != null)
						SendMessageDebugTrace("Return: " + returnValue.ToString());
					else
					{
						returnValue = null;
						SendMessageDebugTrace("Return: null");
					}
				}
				catch(Exception ex)
				{
					throw new GDAException(ex);
				}
			}
			finally
			{
				try
				{
					cmd.Dispose();
					cmd = null;
				}
				finally
				{
					if(session == null)
					{
						conn.Close();
						conn.Dispose();
					}
				}
			}
			return returnValue;
		}

		/// <summary>
		/// Executa uma consulta que retorna somente um campo.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <param name="sqlQuery">Consulta.</param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public object ExecuteScalar(GDASession session, string sqlQuery, params GDAParameter[] parameters)
		{
			return ExecuteScalar(session, CommandType.Text, GDASession.DefaultCommandTimeout, sqlQuery, parameters);
		}

		/// <summary>
		/// Executa uma consulta que retorna somente um campo.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <param name="sqlQuery">Consulta.</param>
		/// <returns></returns>
		public object ExecuteScalar(GDASession session, string sqlQuery)
		{
			return ExecuteScalar(session, sqlQuery, null);
		}

		/// <summary>
		/// Executa uma consulta que retorna somente um campo.
		/// </summary>
		/// <param name="sqlQuery">Consulta.</param>
		/// <returns></returns>
		public object ExecuteScalar(string sqlQuery)
		{
			return ExecuteScalar(sqlQuery, null);
		}

		/// <summary>
		/// Carrega uma lista com os valores da primeira coluna da consulta SQL.
		/// </summary>
		/// <typeparam name="T">Tipo do campo da coluna.</typeparam>
		/// <param name="sqlQuery">Consulta.</param>
		/// <param name="parameters">Parametros para a consulta.</param>
		/// <returns></returns>
		[Obsolete]
		public GDACursor<T> LoadValues<T>(string sqlQuery, params GDAParameter[] parameters) where T : new()
		{
			return LoadValues<T>(null, sqlQuery, parameters);
		}

		/// <summary>
		/// Carrega uma lista com os valores da primeira coluna da consulta SQL.
		/// </summary>
		/// <typeparam name="T">Tipo do campo da coluna.</typeparam>
		/// <param name="procedure">Procedure usado na consulta.</param>
		/// <returns></returns>
		[Obsolete]
		public GDACursor<T> LoadValues<T>(GDAStoredProcedure procedure) where T : new()
		{
			IDbConnection conn = CreateConnection(null);
			IDbCommand cmd = CreateCommand(null, conn);
			cmd.Connection = conn;
			procedure.Prepare(cmd, UserProvider);
			return new GDACursor<T>(UserProvider, null, conn, cmd, (sender, e) =>  {
				for(int i = 0; i < cmd.Parameters.Count; i++)
				{
					var dataParamter = (IDbDataParameter)cmd.Parameters[i];
					if(dataParamter.Direction == ParameterDirection.Output || dataParamter.Direction == ParameterDirection.ReturnValue)
						procedure[i] = ((IDbDataParameter)cmd.Parameters[i]).Value;
				}
			});
		}

		/// <summary>
		/// Carrega uma lista com os valores da primeira coluna da consulta SQL.
		/// </summary>
		/// <typeparam name="T">Tipo do campo da coluna.</typeparam>
		/// <param name="session">Sessão onde será executado o comando.</param>
		/// <param name="procedure">Procedure usado na consulta.</param>
		/// <returns></returns>
		[Obsolete]
		public GDACursor<T> LoadValues<T>(GDASession session, GDAStoredProcedure procedure) where T : new()
		{
			IDbConnection conn = CreateConnection(session);
			IDbCommand cmd = CreateCommand(session, conn);
			cmd.Connection = conn;
			procedure.Prepare(cmd, UserProvider);
			return new GDACursor<T>(UserProvider, session, conn, cmd, (sender, e) =>  {
				for(int i = 0; i < cmd.Parameters.Count; i++)
				{
					var dataParamter = (IDbDataParameter)cmd.Parameters[i];
					if(dataParamter.Direction == ParameterDirection.Output || dataParamter.Direction == ParameterDirection.ReturnValue)
						procedure[i] = ((IDbDataParameter)cmd.Parameters[i]).Value;
				}
			});
		}

		/// <summary>
		/// Carrega uma lista com os valores da primeira coluna da consulta SQL.
		/// </summary>
		/// <typeparam name="T">Tipo do campo da coluna.</typeparam>
		/// <param name="session">Sessão onde será executado o comando.</param>
		/// <param name="sqlQuery">Consulta.</param>
		/// <param name="parameters">Parametros para a consulta.</param>
		/// <returns></returns>
		[Obsolete]
		public GDACursor<T> LoadValues<T>(GDASession session, string sqlQuery, params GDAParameter[] parameters) where T : new()
		{
			IDbConnection conn = CreateConnection(session);
			IDbCommand cmd = CreateCommand(session, conn);
			if(parameters != null)
				foreach (GDAParameter param in parameters)
				{
					cmd.Parameters.Add(GDA.Helper.GDAHelper.ConvertGDAParameter(cmd, param, UserProvider));
				}
			cmd.CommandText = sqlQuery;
			cmd.Connection = conn;
			return new GDACursor<T>(UserProvider, session, conn, cmd, (sender, e) =>  {
				for(int i = 0; i < cmd.Parameters.Count; i++)
				{
					var dataParamter = (IDbDataParameter)cmd.Parameters[i];
					if(dataParamter.Direction == ParameterDirection.Output || dataParamter.Direction == ParameterDirection.ReturnValue)
						parameters[i].Value = ((IDbDataParameter)cmd.Parameters[i]).Value;
				}
			});
		}

		/// <summary>
		/// Executa a consulta e recupera o dados do resultado.
		/// </summary>        
		/// <param name="sqlQuery">Consulta.</param>
		/// <param name="parameters">Parametros para a consulta.</param>
		/// <returns></returns>
		public GDADataRecordCursor LoadResult(string sqlQuery, params GDAParameter[] parameters)
		{
			return LoadResult(null, sqlQuery, parameters);
		}

		/// <summary>
		/// Executa a consulta e recupera o dados do resultado.
		/// </summary>
		/// <param name="session">Sessão onde será executada a consulta.</param>
		/// <param name="sqlQuery">Consulta.</param>
		/// <param name="parameters">Parametros para a consulta.</param>
		/// <returns></returns>
		public GDADataRecordCursor LoadResult(GDASession session, string sqlQuery, params GDAParameter[] parameters)
		{
			return new GDADataRecordCursor(GetLoadResultCursorParameters(session, CommandType.Text, -1, sqlQuery, null, parameters));
		}

		/// <summary>
		/// Executa a consulta e recupera o dados do resultado.
		/// </summary>
		/// <param name="session">Sessão onde será executada a consulta.</param>
		/// <param name="sqlQuery">Consulta.</param>
		/// <param name="parameters">Parametros para a consulta.</param>
		/// <returns></returns>
		public GDADataRecordCursor LoadResult(GDASession session, CommandType commandType, int commandTimeout, string sqlQuery, params GDAParameter[] parameters)
		{
			return new GDADataRecordCursor(GetLoadResultCursorParameters(session, commandType, commandTimeout, sqlQuery, null, parameters));
		}

		/// <summary>
		/// Executa a consulta e recupera o dados do resultado.
		/// </summary>
		/// <param name="session">Sessão onde será executada a consulta.</param>
		/// <param name="commandType">Tipo de comando.</param>
		/// <param name="commandTimeout"></param>
		/// <param name="paging"></param>
		/// <param name="sqlQuery">Consulta.</param>
		/// <param name="parameters">Parametros para a consulta.</param>
		/// <returns></returns>
		public GDADataRecordCursor LoadResult(GDASession session, CommandType commandType, int commandTimeout, string sqlQuery, InfoPaging paging, params GDAParameter[] parameters)
		{
			return new GDADataRecordCursor(GetLoadResultCursorParameters(session, commandType, commandTimeout, sqlQuery, paging, parameters));
		}

		/// <summary>
		/// Executa a Stored Procedure e recupera os dados do resultado.
		/// </summary>
		/// <param name="procedure">Procedure que será executada.</param>
		/// <returns></returns>
		public GDADataRecordCursor LoadResult(GDAStoredProcedure procedure)
		{
			return LoadResult(null, procedure);
		}

		/// <summary>
		/// Executa a Stored Procedure e recupera os dados do resultado.
		/// </summary>
		/// <param name="session">Sessão onde será executada a consulta.</param>
		/// <param name="procedure">Procedure que será executada.</param>
		/// <returns></returns>
		public GDADataRecordCursor LoadResult(GDASession session, GDAStoredProcedure procedure)
		{
			return new GDADataRecordCursor(GetLoadResultCursorParameters(session, procedure));
		}

		/// <summary>
		/// Recupera a quantidade de registros com base na Query.
		/// </summary>
		/// <param name="query">Consulta usada.</param>
		/// <returns>Quantidade de registro encontrados com base na consulta.</returns>
		public long Count(IQuery query)
		{
			return Count(null, query);
		}

		/// <summary>
		/// Recupera a quantidade de registros com base na Query.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <param name="query">Consulta usada.</param>
		/// <returns>Quantidade de registro encontrados com base na consulta.</returns>
		public long Count(GDASession session, IQuery query)
		{
			IProvider provider = session != null && session.ProviderConfiguration != null ? session.ProviderConfiguration.Provider : GDA.GDASettings.DefaultProviderConfiguration.Provider;
			QueryReturnInfo qri = query.BuildResultInfo2(provider, "COUNT(*)");
			return Convert.ToInt64(ExecuteScalar(session, qri.CommandText, qri.Parameters.ToArray()));
		}

		/// <summary>
		/// Efetua a soma de uma determina propriedade da classe T definida.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <param name="query">Consulta usada.</param>
		/// <returns>Soma dos valores.</returns>
		public double Sum(GDASession session, IQuery query)
		{
			IProvider provider = session != null && session.ProviderConfiguration != null ? session.ProviderConfiguration.Provider : GDA.GDASettings.DefaultProviderConfiguration.Provider;
			QueryReturnInfo qri = query.BuildResultInfo2(provider, "SUM(" + query.AggregationFunctionProperty + ")");
			object result = ExecuteScalar(session, qri.CommandText, qri.Parameters.ToArray());
			if(result == null)
				return 0.0d;
			else
				return Convert.ToDouble(result);
		}

		/// <summary>
		/// Recupera o item com o maior valor.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <param name="query">Consulta usada.</param>
		/// <returns>Maior valor encontrado ou zero.</returns>
		public double Max(GDASession session, IQuery query)
		{
			IProvider provider = session != null && session.ProviderConfiguration != null ? session.ProviderConfiguration.Provider : GDA.GDASettings.DefaultProviderConfiguration.Provider;
			QueryReturnInfo qri = query.BuildResultInfo2(provider, "MAX(" + query.AggregationFunctionProperty + ")");
			object result = ExecuteScalar(session, qri.CommandText, qri.Parameters.ToArray());
			if(result == null)
				return 0.0d;
			else
				return Convert.ToDouble(result);
		}

		/// <summary>
		/// Recupera o item com o menor valor.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <param name="query">Consulta usada.</param>
		/// <returns>Menor valor encontrado ou zero.</returns>
		public double Min(GDASession session, IQuery query)
		{
			IProvider provider = session != null && session.ProviderConfiguration != null ? session.ProviderConfiguration.Provider : GDA.GDASettings.DefaultProviderConfiguration.Provider;
			QueryReturnInfo qri = query.BuildResultInfo2(provider, "MIN(" + query.AggregationFunctionProperty + ")");
			object result = ExecuteScalar(session, qri.CommandText, qri.Parameters.ToArray());
			if(result == null)
				return 0.0d;
			else
				return Convert.ToDouble(result);
		}

		/// <summary>
		/// Recupera a média dos valores da propriedade especificada na consulta.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <param name="query">Consulta usada.</param>
		/// <returns>Valor medio encontrado ou zero.</returns>
		public double Avg(GDASession session, IQuery query)
		{
			IProvider provider = session != null && session.ProviderConfiguration != null ? session.ProviderConfiguration.Provider : GDA.GDASettings.DefaultProviderConfiguration.Provider;
			QueryReturnInfo qri = query.BuildResultInfo2(provider, "AVG(" + query.AggregationFunctionProperty + ")");
			object result = ExecuteScalar(session, qri.CommandText, qri.Parameters.ToArray());
			if(result == null)
				return 0.0d;
			else
				return Convert.ToDouble(result);
		}
	}
}
