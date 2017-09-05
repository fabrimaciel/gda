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
using System.Collections;
using System.Text;
using System.Data;
using System.Reflection;
using System.Data.Common;
using System.Diagnostics;
using GDA.Interfaces;
using GDA.Collections;
using GDA.Sql;
using GDA.Sql.InterpreterExpression;
using GDA.Sql.InterpreterExpression.Nodes;
using GDA.Caching;

namespace GDA
{
	public class PersistenceObjectBase<Model> : DataAccess, IPersistenceObjectDataAccess<Model>
	{
		/// <summary>
		/// Nome da tabela relacionado com a Model.
		/// </summary>
		private TableName _tableName = null;

		/// <summary>
		/// Identifica que a model relacionado é compatível com a classe Persistence.
		/// </summary>
		protected internal readonly bool CompatibleWithPersistence;

		/// <summary>
		/// Identifica se no mapeamento da model existe alguma propriedade do tipo identidade.
		/// </summary>
		private bool _existsIdentityProperty = false;

		static PersistenceObjectBase()
		{
			var interfaces = typeof(Model).GetInterfaces();
			var type2 = typeof(IObjectDataRecord);
			foreach (var type in interfaces)
				if(type == type2)
				{
					ImplementIObjectDataRecord = true;
					break;
				}
		}

		/// <summary>
		/// Construtor
		/// </summary>
		/// <param name="providerConfig">Provider para acesso aos dados.</param>
		/// <exception cref="ArgumentNullException">Excessão lançada se o argumento providerConfig for nulo.</exception>
		public PersistenceObjectBase(IProviderConfiguration providerConfig) : base(providerConfig)
		{
			CompatibleWithPersistence = typeof(Model).IsSubclassOf(typeof(Persistent));
			_existsIdentityProperty = MappingManager.CheckExistsIdentityKey(typeof(Model));
		}

		/// <summary>
		/// Evento acionado quando alguma message para debug é lançada.
		/// </summary>
		public event DebugTraceDelegate DebugTrace;

		/// <summary>
		/// Evento acionado quando o registro está começando a ser inserido na BD.
		/// </summary>
		public event PersistenceObjectOperation<Model> Inserting;

		/// <summary>
		/// Evento acionado quando um registro é inserido na BD.
		/// </summary>
		public event PersistenceObjectOperation<Model> Inserted;

		/// <summary>
		/// Evento acionado quando um registro está preparando para ser atualizado na BD.
		/// </summary>
		public event PersistenceObjectOperation<Model> Updating;

		/// <summary>
		/// Evento acionado quando um registro é atualizado na BD.
		/// </summary>
		public event PersistenceObjectOperation<Model> Updated;

		/// <summary>
		/// Evento acionado quando um registro está para ser apagado.
		/// </summary>
		public event PersistenceObjectOperation<Model> Deleting;

		/// <summary>
		/// Evento acionado quando um registro é apagado na BD.
		/// </summary>
		public event PersistenceObjectOperation<Model> Deleted;

		/// <summary>
		/// Destrutor
		/// </summary>
		~PersistenceObjectBase()
		{
		}

		/// <summary>
		/// Identifica que a Model implementa a interface IObjectDataRecord.
		/// </summary>
		public static bool ImplementIObjectDataRecord
		{
			get;
			private set;
		}

		/// <summary>
		/// Captura o nome da tabela que a classe Model representa.
		/// </summary>
		[Obsolete("Use TableNameInfo")]
		public string TableName
		{
			get
			{
				var name = TableNameInfo;
				if(name == null)
					return null;
				return name.Name;
			}
		}

		/// <summary>
		/// Captura as informações do nome da tabela.
		/// </summary>
		public GDA.Sql.TableName TableNameInfo
		{
			get
			{
				if(_tableName == null)
				{
					_tableName = MappingManager.GetTableName(typeof(Model));
				}
				return _tableName;
			}
		}

		/// <summary>
		/// Captura o nome da tabela que a classe Model representa no formato usado no sistema.
		/// </summary>
		public string SystemTableName
		{
			get
			{
				return UserProvider.BuildTableName(TableNameInfo);
			}
		}

		/// <summary>
		/// Obtem as chaves do objeto referenciado.
		/// </summary>
		public List<Mapper> Keys
		{
			get
			{
				return MappingManager.GetMappers<Model>(new PersistenceParameterType[] {
					PersistenceParameterType.Key,
					PersistenceParameterType.IdentityKey
				}, null);
			}
		}

		/// <summary>
		/// Identifica se no mapeamento da model existe alguma propriedade do tipo identidade.
		/// </summary>
		public bool ExistsIdentityProperty
		{
			get
			{
				return _existsIdentityProperty;
			}
		}

		/// <summary>
		/// Carrega um mapa com todos os campos contido no datareader.
		/// </summary>
		/// <param name="listAttr">Lista onde serão inserido o atributos que podem ser recuperados no resultado.</param>
		/// <param name="dataReader">DataReader a ser analizado.</param>
		/// <returns>Mapa dos campos.</returns>
		internal static Dictionary<string, int> GetMapDataReader(ref List<Mapper> listAttr, IDataReader dataReader)
		{
			int countFields = dataReader.FieldCount;
			var dic = new Dictionary<string, int>();
			int index;
			string name;
			List<string> fieldsNames = new List<string>();
			for(int i = 0; i < dataReader.FieldCount; i++)
				fieldsNames.Add(dataReader.GetName(i).ToLower());
			for(int i = 0; i < listAttr.Count; i++)
			{
				name = listAttr[i].Name.ToLower();
				index = fieldsNames.FindIndex(delegate(string fn) {
					return fn == name;
				});
				if(index < 0 && (listAttr[i].Direction == DirectionParameter.InputOptional || listAttr[i].Direction == DirectionParameter.InputOptionalOutput || listAttr[i].Direction == DirectionParameter.InputOptionalOutputOnlyInsert))
				{
					SendMessageDebugTrace(null, "Property InputOptional " + listAttr[i].PropertyMapperName + " not found...");
					listAttr.RemoveAt(i);
					i--;
					continue;
				}
				else if(index >= 0)
				{
					try
					{
						dic.Add(fieldsNames[index], index);
					}
					catch(ArgumentException ex)
					{
						throw new GDAException("Ambiguous column name in result.", ex);
					}
				}
				else
				{
					throw new GDAColumnNotFoundException(listAttr[i].Name, "");
				}
			}
			return dic;
		}

		/// <summary>
		/// Recupera os valores do resultado e preenche o objeto submetido.
		/// </summary>
		/// <param name="dReader">DataReader contendo os dados.</param>
		/// <param name="listAttr">Lista dos atributos a serem carregados.</param>
		/// <param name="mapFields">Mapa do campos</param>
		/// <param name="objItem">Objeto que será preenchido.</param>
		/// <param name="mappingInProperties">Identifica se o mapeamento foi feito com base no nomes das propriedades.</param>
		/// <exception cref="GDAColumnNotFoundException"></exception>
		/// <exception cref="GDAException"></exception>
		internal static void RecoverValueOfResult(ref IDataRecord dReader, TranslatorDataInfoCollection mapping, ref Model objItem, bool mappingInProperties)
		{
			object obj = objItem;
			RecoverValueOfResult(ref dReader, mapping, ref obj, ImplementIObjectDataRecord);
		}

		/// <summary>
		/// Recupera os parametros para a inicialização do cursor para a query.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <param name="query">Consulta.</param>
		/// <returns></returns>
		internal GDACursorParameters GetCursorParameters(GDASession session, IQuery query)
		{
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
			return new GDACursorParameters(UserProvider, session, conn, cmd, new TranslatorDataInfoCollection(listAttr), usingPaging, query.SkipCount, query.TakeCount, null);
		}

		/// <summary>
		/// Carrega os dado com o retorno da stored procedure.
		/// </summary>
		/// <param name="session">Sessão onde a procedure será executada.</param>
		/// <param name="procedure">Dados da stored procedure a ser executada.</param>
		/// <param name="selectProperties">Nomes das propriedades que serão recuperadas na consulta separados por vírgula.</param>
		/// <returns>Lista com os dados do retorno.</returns>
		internal GDACursorParameters GetCursorParameters(GDASession session, GDAStoredProcedure procedure, string selectProperties)
		{
			IDbConnection conn = CreateConnection(session);
			IDbCommand cmd = CreateCommand(session, conn);
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandTimeout = procedure.CommandTimeout;
			cmd.CommandText = procedure.Name;
			List<Mapper> listAttr = MappingManager.GetMappers<Model>(null, null);
			if(listAttr.Count == 0)
				throw new GDAException("Not found properties mappers to model {0}.", typeof(Model).FullName);
			if(!string.IsNullOrEmpty(selectProperties))
			{
				List<Mapper> listAux = new List<Mapper>();
				string[] pNames = selectProperties.Split(',');
				bool found = false;
				for(int i = 0; i < pNames.Length; i++)
				{
					string pn = pNames[i].TrimStart().TrimEnd();
					found = false;
					foreach (Mapper m in listAttr)
						if(string.Compare(pn, m.PropertyMapperName, true) == 0)
						{
							listAux.Add(m);
							found = true;
							break;
						}
					if(!found)
						throw new GDAException("Property {0} not found or not found in mapping.", pn);
				}
				listAttr.Clear();
				listAttr.AddRange(listAux);
			}
			foreach (GDAParameter param in procedure)
				cmd.Parameters.Add(GDA.Helper.GDAHelper.ConvertGDAParameter(cmd, param, UserProvider));
			return new GDACursorParameters(UserProvider, session, conn, cmd, new TranslatorDataInfoCollection(listAttr), false, 0, 0, delegate(object sender, EventArgs args) {
				for(int i = 0; i < cmd.Parameters.Count; i++)
				{
					object rVal = ((IDbDataParameter)cmd.Parameters[i]).Value;
					procedure[i] = (rVal == DBNull.Value ? null : rVal);
				}
			});
		}

		/// <summary>
		/// Recupera os parametros para a inicialização do cursos para a consulta.
		/// </summary>
		/// <param name="sqlQuery">Query.</param>
		/// <param name="selectProperties">Nomes das propriedades que serão recuperadas na consulta separados por vírgula.</param>
		/// <param name="sortExpression">Expressão de ordenação do comando sql.</param>
		/// <param name="infoPaging">Informações para paginação do resultado da query.</param>
		/// <param name="parameters">Parametros da query.</param>
		/// <returns>Lista com os dados do retorno da query.</returns>
		/// <exception cref="GDAColumnNotFoundException"></exception>
		/// <exception cref="GDAException"></exception>
		internal GDACursorParameters GetCursorParameters(GDASession session, string sqlQuery, string selectProperties, System.Data.CommandType commandType, InfoSortExpression sortExpression, InfoPaging infoPaging, GDAParameter[] parameters)
		{
			IDbConnection conn = CreateConnection(session);
			IDbCommand cmd = CreateCommand(session, conn);
			cmd.CommandType = commandType;
			List<Mapper> listAttr = MappingManager.GetMappers<Model>(null, null);
			if(!string.IsNullOrEmpty(selectProperties))
			{
				List<Mapper> listAux = new List<Mapper>();
				string[] pNames = selectProperties.Split(',');
				bool found = false;
				for(int i = 0; i < pNames.Length; i++)
				{
					string pn = pNames[i].TrimStart().TrimEnd();
					found = false;
					foreach (Mapper m in listAttr)
						if(string.Compare(pn, m.PropertyMapperName, true) == 0)
						{
							listAux.Add(m);
							found = true;
							break;
						}
					if(!found)
						throw new GDAException("Property {0} not found or not found in mapping.", pn);
				}
				listAttr.Clear();
				listAttr.AddRange(listAux);
			}
			if(listAttr.Count == 0)
				throw new GDAException("Not found properties mappers to model {0}.", typeof(Model).FullName);
			if(sortExpression != null && sortExpression.SortColumn != null && sortExpression.SortColumn != "")
			{
				if(sqlQuery.ToLower().IndexOf("ORDER BY", 0, StringComparison.OrdinalIgnoreCase) != -1)
					throw new GDAException("Found ORDER BY in query, InforSortExpression not allow.");
				else
				{
					Mapper mapperOrderBy = listAttr.Find(delegate(Mapper m) {
						return m.PropertyMapperName == sortExpression.SortColumn;
					});
					if(mapperOrderBy == null)
						throw new GDAException("Property {0} not found in mapping.", sortExpression.SortColumn);
					else
						sqlQuery += " ORDER BY " + ((sortExpression.AliasTable != null && sortExpression.AliasTable != "") ? (sortExpression.AliasTable + ".") : "") + UserProvider.QuoteExpression(mapperOrderBy.Name) + ((sortExpression.Reverse) ? " DESC" : "");
				}
			}
			if(parameters != null)
				for(int i = 0; i < parameters.Length; i++)
				{
					if(parameters[i].Value is IQuery)
						continue;
					try
					{
						string newName = (parameters[i].ParameterName[0] != '?' ? parameters[i].ParameterName : UserProvider.ParameterPrefix + parameters[i].ParameterName.Substring(1) + UserProvider.ParameterSuffix);
						sqlQuery = sqlQuery.Replace(parameters[i].ParameterName, newName);
					}
					catch(Exception ex)
					{
						throw new GDAException("Error on make parameter name '" + parameters[i].ParameterName + "'.", ex);
					}
					cmd.Parameters.Add(GDA.Helper.GDAHelper.ConvertGDAParameter(cmd, parameters[i], UserProvider));
				}
			if(infoPaging != null)
			{
				sqlQuery = UserProvider.SQLCommandLimit(MappingManager.GetMappers<Model>(null, null), sqlQuery, infoPaging.StartRow, infoPaging.PageSize);
			}
			cmd.CommandText = sqlQuery;
			return new GDACursorParameters(UserProvider, session, conn, cmd, new TranslatorDataInfoCollection(listAttr), false, 0, 0, null);
		}

		/// <summary>
		/// Cria os parametros com os dados.
		/// </summary>
		/// <param name="cmd">Command aonde o parametro será relacionado.</param>
		/// <param name="mapper"></param>
		/// <param name="itemData"></param>
		/// <returns>Nome do parametro criado.</returns>
		internal protected string CreateDataParameter(ref IDbCommand cmd, Mapper mapper, ref Model itemData)
		{
			object value = mapper.PropertyMapper.GetValue(itemData, null);
			value = (value == null ? DBNull.Value : value);
			string parameterName = UserProvider.ParameterPrefix + mapper.Name.Replace(" ", "_") + UserProvider.ParameterSuffix;
			SendMessageDebugTrace("Create DataParameter -> Name: " + parameterName + "; Value: " + (value == DBNull.Value ? "{NULL}" : value.ToString()));
			IDbDataParameter dbParam = cmd.CreateParameter();
			dbParam.ParameterName = parameterName;
			if(mapper.PropertyMapper.PropertyType.Name == "Byte[]" && value == DBNull.Value)
				dbParam.DbType = DbType.Binary;
			if(value is Guid)
				value = value.ToString();
			UserProvider.SetParameterValue(dbParam, value);
			cmd.Parameters.Add(dbParam);
			return parameterName;
		}

		/// <summary>
		/// Gera uma chave para os dados informados.
		/// </summary>
		/// <param name="args">Argumentos que serão usados no processamento.</param>
		public void GenerateKey(Mapper identityProperty, GenerateKeyArgs args)
		{
			if(args == null)
				throw new ArgumentNullException("args");
			if(identityProperty == null)
				identityProperty = MappingManager.GetIdentityKey(args.ModelType);
			if(identityProperty != null && identityProperty.GeneratorKey != null)
				identityProperty.GeneratorKey.GenerateKey(this, args);
			else if(GDAOperations.GlobalGenerateKey != null)
				GDAOperations.GlobalGenerateKey(this, args);
			else
				args.Cancel = true;
		}

		/// <summary>
		/// Inseri os dados contidos no objInsert no BD.
		/// </summary>
		/// <param name="objInsert">Objeto com os dados a serem inseridos.</param>
		/// <returns>Chave inserido.</returns>
		/// <exception cref="ArgumentNullException">ObjInsert it cannot be null.</exception>
		/// <exception cref="GDAException"></exception>
		public uint Insert(Model objInsert)
		{
			return Insert(objInsert, null);
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
			return Insert(objInsert, propertiesNamesInsert, DirectionPropertiesName.Inclusion);
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
		public uint Insert(Model objInsert, string propertiesNamesInsert, DirectionPropertiesName direction)
		{
			return Insert(null, objInsert, propertiesNamesInsert, direction);
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
		public uint Insert(GDASession session, Model objInsert, string propertiesNamesInsert, DirectionPropertiesName direction)
		{
			if(objInsert == null)
				throw new ArgumentNullException("ObjInsert it cannot be null.");
			string queryIdentityValue = null;
			Mapper identityKey = MappingManager.GetIdentityKey(typeof(Model));
			if(UserProvider.GenerateIdentity && ExistsIdentityProperty)
			{
				queryIdentityValue = UserProvider.GetIdentitySelect(TableNameInfo, identityKey.Name);
			}
			SendMessageDebugTrace("GDA call method insert.");
			bool localSession = false;
			if(session == null)
			{
				localSession = true;
				session = new GDASession(Configuration);
			}
			IDbConnection conn = CreateConnection(session);
			IDbCommand cmd = CreateCommand(session, conn);
			string[] propNames = null;
			if(propertiesNamesInsert != null && propertiesNamesInsert != "")
			{
				propNames = propertiesNamesInsert.Split(',');
			}
			uint returnValue = 0;
			string fieldsInsert = "", paramsInsert = "";
			if(Inserting != null)
				Inserting(this, ref objInsert);
			List<Mapper> list = MappingManager.GetMappers<Model>(new PersistenceParameterType[] {
				PersistenceParameterType.Field,
				PersistenceParameterType.ForeignKey,
				PersistenceParameterType.Key
			}, new DirectionParameter[] {
				DirectionParameter.Output,
				DirectionParameter.InputOptionalOutput,
				DirectionParameter.InputOutput,
				DirectionParameter.OutputOnlyInsert,
				DirectionParameter.OnlyInsert,
				DirectionParameter.InputOptionalOutputOnlyInsert
			});
			if(propNames != null && propNames.Length > 0)
			{
				int[] indexs = new int[propNames.Length];
				int i = 0;
				for(i = 0; i < propNames.Length; i++)
				{
					string p = propNames[i].Trim();
					int index = list.FindIndex(delegate(Mapper m) {
						return m.PropertyMapperName == p;
					});
					if(index < 0)
						throw new GDAException("Property {0} not found in mapper for insert.", p);
					else
						indexs[i] = index;
				}
				if(direction == DirectionPropertiesName.Inclusion)
				{
					for(i = 0; i < indexs.Length; i++)
					{
						fieldsInsert += UserProvider.QuoteExpression(list[indexs[i]].Name) + ",";
						paramsInsert += CreateDataParameter(ref cmd, list[indexs[i]], ref objInsert) + ",";
					}
					list.Clear();
				}
				else
				{
					for(i = 0; i < indexs.Length; i++)
						list.RemoveAt(indexs[i]);
				}
			}
			var gKeyArgs = new GenerateKeyArgs(session, typeof(Model), identityKey, TableNameInfo.Schema, TableNameInfo.Name, (identityKey != null ? identityKey.Name : null), objInsert);
			if(identityKey != null)
			{
				GenerateKey(identityKey, gKeyArgs);
				if(!gKeyArgs.Cancel)
				{
					returnValue = gKeyArgs.KeyValue;
					try
					{
						object val = typeof(Convert).GetMethod("To" + identityKey.PropertyMapper.PropertyType.Name, new Type[] {
							typeof(uint)
						}).Invoke(null, new object[] {
							returnValue
						});
						identityKey.PropertyMapper.SetValue(objInsert, val, null);
					}
					catch(Exception ex)
					{
						throw new GDAException(string.Format("Fail on define identity value '{0}' to property '{1}' of model '{2}'", gKeyArgs.KeyValue, identityKey.PropertyMapperName, typeof(Model).FullName), ex);
					}
					list.Insert(0, identityKey);
				}
			}
			else
				gKeyArgs.Cancel = true;
			foreach (Mapper mapper in list)
			{
				fieldsInsert += UserProvider.QuoteExpression(mapper.Name) + ",";
				paramsInsert += CreateDataParameter(ref cmd, mapper, ref objInsert) + ",";
			}
			cmd.CommandText = String.Format("INSERT INTO {0} ({1})VALUES({2}){3}", SystemTableName, fieldsInsert.Substring(0, fieldsInsert.Length - 1), paramsInsert.Substring(0, paramsInsert.Length - 1), (UserProvider.ExecuteCommandsOneAtATime ? "" : (Configuration != null && Configuration.Provider != null ? Configuration.Provider.StatementTerminator : "")));
			try
			{
				if(gKeyArgs.Cancel && UserProvider.GenerateIdentity && ExistsIdentityProperty)
				{
					if(!UserProvider.ExecuteCommandsOneAtATime)
					{
						cmd.CommandText += queryIdentityValue;
						SendMessageDebugTrace("CommandText: " + cmd.CommandText);
						object val = null;
						using (var executionHandler = Diagnostics.GDATrace.CreateExecutionHandler(cmd))
							try
							{
								val = cmd.ExecuteScalar();
							}
							catch(Exception ex)
							{
								executionHandler.Fail(ex);
								throw;
							}
						if(val is uint)
							returnValue = (uint)val;
						else
							returnValue = Convert.ToUInt32(val);
					}
					else
					{
						SendMessageDebugTrace("Executing commands one at a time");
						SendMessageDebugTrace("CommandText: " + cmd.CommandText);
						using (var executionHandler = Diagnostics.GDATrace.CreateExecutionHandler(cmd))
							try
							{
								executionHandler.RowsAffects = cmd.ExecuteNonQuery();
							}
							catch(Exception ex)
							{
								executionHandler.Fail(ex);
								throw;
							}
						cmd.Parameters.Clear();
						if(!string.IsNullOrEmpty(queryIdentityValue))
							SendMessageDebugTrace(queryIdentityValue);
						cmd.CommandText = queryIdentityValue;
						object val = null;
						using (var executionHandler = Diagnostics.GDATrace.CreateExecutionHandler(cmd))
							try
							{
								val = cmd.ExecuteScalar();
							}
							catch(Exception ex)
							{
								executionHandler.Fail(ex);
								throw ex;
							}
						if(val is uint)
							returnValue = (uint)val;
						else
							returnValue = Convert.ToUInt32(val);
					}
					if(identityKey != null)
					{
						try
						{
							object val = typeof(Convert).GetMethod("To" + identityKey.PropertyMapper.PropertyType.Name, new Type[] {
								typeof(uint)
							}).Invoke(null, new object[] {
								returnValue
							});
							identityKey.PropertyMapper.SetValue(objInsert, val, null);
						}
						catch
						{
						}
					}
				}
				else
				{
					SendMessageDebugTrace("CommandText: " + cmd.CommandText);
					using (var executionHandler = Diagnostics.GDATrace.CreateExecutionHandler(cmd))
						try
						{
							executionHandler.RowsAffects = cmd.ExecuteNonQuery();
						}
						catch(Exception ex)
						{
							executionHandler.Fail(ex);
							throw ex;
						}
				}
			}
			catch(System.Data.DataException ex)
			{
				throw new GDAException(ex);
			}
			finally
			{
				if(localSession)
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
			if(Inserted != null)
				Inserted(this, ref objInsert);
			return returnValue;
		}

		/// <summary>
		/// <para>Inseri os dados contidos no objInsert não levando em consideração a chave identidade.</para>
		/// <para>Ou seja, constroi um comando sql com todos os dados do objeto que obedenção os parametros
		/// de direção InputOutput, Input, InputOnlyInsert.</para>
		/// </summary>
		/// <param name="objInsert">Objeto com os dados a serem inseridos.</param>
		/// <returns>Número de linhas afetadas.</returns>
		/// <exception cref="GDAException"></exception>
		/// <exception cref="ArgumentNullException">ObjInsert it cannot be null.</exception>
		public uint InsertForced(Model objInsert)
		{
			return InsertForced(null, objInsert);
		}

		/// <summary>
		/// <para>Inseri os dados contidos no objInsert não levando em consideração a chave identidade.</para>
		/// <para>Ou seja, constroi um comando sql com todos os dados do objeto que obedenção os parametros
		/// de direção InputOutput, Input, InputOnlyInsert.</para>
		/// </summary>
		/// <param name="objInsert">Objeto com os dados a serem inseridos.</param>
		/// <returns>Número de linhas afetadas.</returns>
		/// <exception cref="GDAException"></exception>
		/// <exception cref="ArgumentNullException">ObjInsert it cannot be null.</exception>
		public uint InsertForced(GDASession session, Model objInsert)
		{
			if(objInsert == null)
				throw new ArgumentNullException("ObjInsert it cannot be null.");
			if(Inserting != null)
				Inserting(this, ref objInsert);
			IDbConnection conn = CreateConnection(session);
			IDbCommand cmd = CreateCommand(session, conn);
			uint returnValue = 0;
			string fieldsInsert = "", paramsInsert = "";
			List<Mapper> list = MappingManager.GetMappers<Model>(null, new DirectionParameter[] {
				DirectionParameter.Output,
				DirectionParameter.InputOutput,
				DirectionParameter.OutputOnlyInsert,
				DirectionParameter.OnlyInsert
			});
			foreach (Mapper mapper in list)
			{
				fieldsInsert += UserProvider.QuoteExpression(mapper.Name) + ",";
				paramsInsert += CreateDataParameter(ref cmd, mapper, ref objInsert) + ",";
			}
			cmd.CommandText = String.Format("INSERT INTO {0} ({1})VALUES({2}){3}", SystemTableName, fieldsInsert.Substring(0, fieldsInsert.Length - 1), paramsInsert.Substring(0, paramsInsert.Length - 1), UserProvider.StatementTerminator);
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
				SendMessageDebugTrace("CommandText: " + cmd.CommandText);
				using (var executionHandler = Diagnostics.GDATrace.CreateExecutionHandler(cmd))
					try
					{
						returnValue = (uint)cmd.ExecuteNonQuery();
						executionHandler.RowsAffects = (int)returnValue;
					}
					catch(Exception ex)
					{
						executionHandler.Fail(ex);
						throw ex;
					}
			}
			catch(Exception ex)
			{
				throw new GDAException(ex);
			}
			finally
			{
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
			if(Inserted != null)
				Inserted(this, ref objInsert);
			return returnValue;
		}

		/// <summary>
		/// Atualiza os dados contidos no objUpdate no BD.
		/// </summary>
		/// <param name="objUpdate">Objeto com os dados a serem atualizados.</param>
		/// <exception cref="System.ArgumentNullException"></exception>
		/// <exception cref="GDAConditionalClauseException">Parameters do not exist to build the conditional clause.</exception>
		/// <exception cref="GDAException"></exception>
		/// <returns>Número de linhas afetadas.</returns>
		public int Update(Model objUpdate)
		{
			return Update(objUpdate, null);
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
		public int Update(Model objUpdate, string propertiesNamesUpdate)
		{
			return Update(objUpdate, propertiesNamesUpdate, DirectionPropertiesName.Inclusion);
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
		public int Update(Model objUpdate, string propertiesNamesUpdate, DirectionPropertiesName direction)
		{
			return Update(null, objUpdate, propertiesNamesUpdate, direction);
		}

		/// <summary>
		/// Atualiza os dados contidos no objUpdate no BD.
		/// </summary>
		/// <param name="session">Sessão utilizada para a executação do comando de atualização.</param>
		/// <param name="objUpdate">Objeto com os dados a serem atualizados.</param>
		/// <exception cref="System.ArgumentNullException"></exception>
		/// <exception cref="GDAConditionalClauseException">Parameters do not exist to build the conditional clause.</exception>
		/// <exception cref="GDAException"></exception>
		/// <returns>Número de linhas afetadas.</returns>
		public int Update(GDASession session, Model objUpdate)
		{
			return Update(session, objUpdate, null, DirectionPropertiesName.Inclusion);
		}

		/// <summary>
		/// Atualiza os dados contidos no objUpdate no BD.
		/// </summary>
		/// <param name="session">Sessão utilizada para a executação do comando de atualização.</param>
		/// <param name="objUpdate">Objeto com os dados a serem atualizados.</param>
		/// <param name="propertiesNamesUpdate">Nome das propriedades separados por virgula, que serão atualizadas no comando.</param>
		/// <param name="direction">Direção que os nomes das propriedades terão no comando. (Default: DirectionPropertiesName.Inclusion)</param>
		/// <exception cref="System.ArgumentNullException"></exception>
		/// <exception cref="GDAConditionalClauseException">Parameters do not exist to build the conditional clause.</exception>
		/// <exception cref="GDAException"></exception>
		/// <returns>Número de linhas afetadas.</returns>
		public int Update(GDASession session, Model objUpdate, string propertiesNamesUpdate, DirectionPropertiesName direction)
		{
			if(objUpdate == null)
				throw new ArgumentNullException("ObjUpdate it cannot be null.");
			IDbConnection conn = CreateConnection(session);
			IDbCommand cmd = CreateCommand(session, conn);
			string[] propNames = null;
			if(propertiesNamesUpdate != null && propertiesNamesUpdate != "")
			{
				propNames = propertiesNamesUpdate.Split(',');
			}
			if(Updating != null)
				Updating(this, ref objUpdate);
			int returnValue = 0;
			string sqlQuery = "UPDATE " + SystemTableName + " SET ", clauseWhere = "";
			List<Mapper> listAttr = MappingManager.GetMappers<Model>(new PersistenceParameterType[] {
				PersistenceParameterType.Field,
				PersistenceParameterType.ForeignKey
			}, new DirectionParameter[] {
				DirectionParameter.Output,
				DirectionParameter.InputOptionalOutput,
				DirectionParameter.InputOutput
			});
			if(propNames != null && propNames.Length > 0)
			{
				int[] indexs = new int[propNames.Length];
				int i = 0;
				for(i = 0; i < propNames.Length; i++)
				{
					string p = propNames[i].Trim();
					int index = listAttr.FindIndex(delegate(Mapper m) {
						return m.PropertyMapperName == p;
					});
					if(index < 0)
						throw new GDAException("Property {0} not found in mapper for update.", p);
					else
						indexs[i] = index;
				}
				if(direction == DirectionPropertiesName.Inclusion)
				{
					if(indexs.Length == 0)
						return 0;
					for(i = 0; i < indexs.Length; i++)
					{
						sqlQuery += UserProvider.QuoteExpression(listAttr[indexs[i]].Name) + "=" + CreateDataParameter(ref cmd, listAttr[indexs[i]], ref objUpdate) + ",";
					}
					listAttr.Clear();
				}
				else
				{
					if(listAttr.Count == 0)
						return 0;
					for(i = 0; i < indexs.Length; i++)
						listAttr.RemoveAt(indexs[i]);
				}
			}
			else
			{
				if(listAttr.Count == 0)
					return 0;
			}
			foreach (Mapper mapper in listAttr)
			{
				sqlQuery += UserProvider.QuoteExpression(mapper.Name) + "=" + CreateDataParameter(ref cmd, mapper, ref objUpdate) + ",";
			}
			listAttr = MappingManager.GetMappers<Model>(new PersistenceParameterType[] {
				PersistenceParameterType.Key,
				PersistenceParameterType.IdentityKey
			}, new DirectionParameter[] {
				DirectionParameter.Output,
				DirectionParameter.InputOptionalOutput,
				DirectionParameter.InputOutput
			});
			if(listAttr.Count == 0)
				throw new GDAConditionalClauseException("Parameters do not exist to build the conditional clause.");
			foreach (Mapper mapper in listAttr)
			{
				if(clauseWhere.Length != 0)
					clauseWhere += " AND ";
				clauseWhere += UserProvider.QuoteExpression(mapper.Name) + "=" + CreateDataParameter(ref cmd, mapper, ref objUpdate);
			}
			cmd.CommandText = sqlQuery.Substring(0, sqlQuery.Length - 1) + " WHERE " + clauseWhere;
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
				SendMessageDebugTrace("CommandText: " + cmd.CommandText);
				using (var executionHandler = Diagnostics.GDATrace.CreateExecutionHandler(cmd))
					try
					{
						returnValue = cmd.ExecuteNonQuery();
						executionHandler.RowsAffects = (int)returnValue;
					}
					catch(Exception ex)
					{
						executionHandler.Fail(ex);
						throw ex;
					}
			}
			catch(Exception ex)
			{
				throw new GDAException(ex);
			}
			finally
			{
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
			if(returnValue > 0 && Updated != null)
				Updated(this, ref objUpdate);
			return returnValue;
		}

		/// <summary>
		/// Remove o conjunto de itens relacionados com a model com base na consulta fornecida.
		/// </summary>
		/// <param name="query">Filtro para os itens a serem removidos.</param>
		/// <returns>Número de linhas afetadas.</returns>
		/// <exception cref="ArgumentNullException">ObjDelete it cannot be null.</exception>
		/// <exception cref="GDAConditionalClauseException">Parameters do not exist to contruir the conditional clause.</exception>
		/// <exception cref="GDAException"></exception>
		public int Delete(Query query)
		{
			return Delete(null, query);
		}

		/// <summary>
		/// Remove o conjunto de itens relacionados com a model com base na consulta fornecida.
		/// </summary>
		/// <param name="session">Sessão usada para executa o comando para apagar o registro.</param>
		/// <param name="query">Filtro para os itens a serem removidos.</param>
		/// <returns>Número de linhas afetadas.</returns>
		/// <exception cref="ArgumentNullException">ObjDelete it cannot be null.</exception>
		/// <exception cref="GDAConditionalClauseException">Parameters do not exist to contruir the conditional clause.</exception>
		/// <exception cref="GDAException"></exception>
		public int Delete(GDASession session, Query query)
		{
			if(query == null)
				throw new ArgumentNullException("query");
			GDAParameter[] parameters = null;
			StringBuilder buf = new StringBuilder(128).Append("DELETE ");
			DirectionParameter[] dp = new DirectionParameter[] {
				DirectionParameter.Input,
				DirectionParameter.InputOutput,
				DirectionParameter.OutputOnlyInsert
			};
			List<Mapper> columns = MappingManager.GetMappers<Model>(null, dp);
			buf.Append(" FROM ");
			buf.Append(SystemTableName);
			if(!string.IsNullOrEmpty(query.Where))
			{
				Parser parser = new Parser(new Lexer(query.Where));
				WherePart wp = parser.ExecuteWherePart();
				SelectStatement ss = new SelectStatement(wp);
				foreach (ColumnInfo ci in ss.ColumnsInfo)
				{
					Mapper m = columns.Find(delegate(Mapper mp) {
						return string.Compare(mp.PropertyMapperName, ci.ColumnName, true) == 0;
					});
					if(m == null)
						throw new GDAException("Property {0} not exists in {1} or not mapped.", ci.ColumnName, typeof(Model).FullName);
					ci.DBColumnName = m.Name;
					ci.RenameToMapper(this.Configuration.Provider);
				}
				ParserToSqlCommand psc = new ParserToSqlCommand(wp, UserProvider.QuoteExpressionBegin, UserProvider.QuoteExpressionEnd);
				buf.Append(" WHERE ").Append(psc.SqlCommand);
				parameters = query.Parameters.ToArray();
			}
			return ExecuteCommand(session, buf.ToString(), parameters);
		}

		/// <summary>
		/// Remove o item do BD que o objDelete representa.
		/// </summary>
		/// <param name="objDelete">Objeto com os dados a serem Removidos.</param>
		/// <returns>Número de linhas afetadas.</returns>
		/// <exception cref="ArgumentNullException">ObjDelete it cannot be null.</exception>
		/// <exception cref="GDAConditionalClauseException">Parameters do not exist to contruir the conditional clause.</exception>
		/// <exception cref="GDAException"></exception>
		public int Delete(Model objDelete)
		{
			return Delete(null, objDelete);
		}

		/// <summary>
		/// Remove o item do BD que o objDelete representa.
		/// </summary>
		/// <param name="session">Sessão usada para executa o comando para apagar o registro.</param>
		/// <param name="objDelete">Objeto com os dados a serem Removidos.</param>
		/// <returns>Número de linhas afetadas.</returns>
		/// <exception cref="ArgumentNullException">ObjDelete it cannot be null.</exception>
		/// <exception cref="GDAConditionalClauseException">Parameters do not exist to contruir the conditional clause.</exception>
		/// <exception cref="GDAException"></exception>
		public int Delete(GDASession session, Model objDelete)
		{
			if(objDelete == null)
				throw new ArgumentNullException("ObjDelete it cannot be null.");
			IDbConnection conn = CreateConnection(session);
			IDbCommand cmd = CreateCommand(session, conn);
			if(Deleting != null)
				Deleting(this, ref objDelete);
			int returnValue = 0;
			string clauseWhere = "";
			List<Mapper> listAttr = MappingManager.GetMappers<Model>(new PersistenceParameterType[] {
				PersistenceParameterType.Key,
				PersistenceParameterType.IdentityKey
			}, new DirectionParameter[] {
				DirectionParameter.Output,
				DirectionParameter.InputOptionalOutput,
				DirectionParameter.InputOutput
			});
			if(listAttr.Count == 0)
				throw new GDAConditionalClauseException("Parameters do not exist to build the conditional clause.");
			foreach (Mapper mapper in listAttr)
			{
				if(clauseWhere.Length != 0)
					clauseWhere += " AND ";
				clauseWhere += UserProvider.QuoteExpression(mapper.Name) + "=" + CreateDataParameter(ref cmd, mapper, ref objDelete);
			}
			cmd.CommandText = "DELETE FROM " + SystemTableName + " WHERE " + clauseWhere;
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
				SendMessageDebugTrace("CommandText: " + cmd.CommandText);
				using (var executionHandler = Diagnostics.GDATrace.CreateExecutionHandler(cmd))
					try
					{
						executionHandler.RowsAffects = cmd.ExecuteNonQuery();
						returnValue = executionHandler.RowsAffects;
					}
					catch(Exception ex)
					{
						executionHandler.Fail(ex);
						throw ex;
					}
			}
			catch(Exception ex)
			{
				throw new GDAException(ex);
			}
			finally
			{
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
			if(returnValue > 0 && Deleted != null)
				Deleted(this, ref objDelete);
			return returnValue;
		}

		/// <summary>
		/// Se o registro já existir na BD os dados serão atualizados, caso não existe um novo registro é criado.
		/// </summary>
		/// <param name="objData">Objeto conténdo os dados a serem utilizados na transação.</param>
		/// <exception cref="GDAException">Se o tipo de dados utilizado não possuir chaves.</exception>
		/// <exception cref="System.ArgumentNullException"></exception>
		/// <exception cref="GDAConditionalClauseException">Parameters do not exist to build the conditional clause.</exception>
		/// <returns>0 ser o registro estiver sido alterado, senão a identidade do novo registro inserido.</returns>
		public uint InsertOrUpdate(Model objData)
		{
			return InsertOrUpdate(null, objData);
		}

		/// <summary>
		/// Se o registro já existir na BD os dados serão atualizados, caso não existe um novo registro é criado.
		/// </summary>
		/// <param name="session">Sessão para execução do comando.</param>
		/// <param name="objData">Objeto conténdo os dados a serem utilizados na transação.</param>
		/// <exception cref="GDAException">Se o tipo de dados utilizado não possuir chaves.</exception>
		/// <exception cref="System.ArgumentNullException"></exception>
		/// <exception cref="GDAConditionalClauseException">Parameters do not exist to build the conditional clause.</exception>
		/// <returns>0 ser o registro estiver sido alterado, senão a identidade do novo registro inserido.</returns>
		public uint InsertOrUpdate(GDASession session, Model objData)
		{
			List<Mapper> listKeys = Keys;
			Type type = typeof(Model);
			if(listKeys.Count == 0)
				throw new GDAException("Operação ilegal. Objeto do tipo \"" + type.FullName + "\" não possui chaves identificadas, por não é possível executar a ação.");
			int i = 0;
			GDAParameter[] parameters = new GDAParameter[listKeys.Count];
			foreach (Mapper mapper in listKeys)
			{
				parameters[i] = new GDAParameter();
				parameters[i].ParameterName = UserProvider.ParameterPrefix + mapper.PropertyMapperName + UserProvider.ParameterSuffix;
				parameters[i].Value = mapper.PropertyMapper.GetValue(objData, null);
				parameters[i].SourceColumn = mapper.Name;
				i++;
			}
			int numRegFound = GetNumberRegFound(session, parameters);
			if(numRegFound == 0)
				return Insert(session, objData, null, DirectionPropertiesName.Inclusion);
			else if(numRegFound == 1)
			{
				Update(session, objData, null, DirectionPropertiesName.Inclusion);
				return 0;
			}
			else
				throw new GDAException("Existem chaves duplicadas na base de dados.");
		}

		/// <summary>
		/// Recupera o valor da propriedade que se encontrar no resultado da consulta.
		/// </summary>
		/// <param name="session">Sessão para execução do comando.</param>
		/// <param name="query">Comando da consulta que será executada.</param>
		/// <param name="propertyName">Nome da propriedade que será recuperada.</param>
		/// <param name="parameters">Parametros usados na consulta.</param>
		/// <returns>Valor da propriedade encontrada.</returns>
		public GDAPropertyValue GetValue(GDASession session, string query, string propertyName, params GDAParameter[] parameters)
		{
			if(string.IsNullOrEmpty(propertyName))
				throw new ArgumentNullException("propertyName");
			List<Mapper> mappers = MappingManager.GetMappers<Model>(null, null);
			var property = mappers.Find(delegate(Mapper m) {
				return m.PropertyMapperName == propertyName;
			});
			if(property == null)
				throw new GDAException("Property {0} not found in {1}.", propertyName, typeof(Model).FullName);
			foreach (GDADataRecord i in LoadResult(session, query, parameters))
				if(i.GetOrdinal(property.Name) >= 0)
					return new GDAPropertyValue(i.GetValue(property.Name), true);
			return new GDAPropertyValue(null, false);
		}

		/// <summary>
		/// Recupera a quantidade de registros da tabela no banco.
		/// </summary>
		/// <returns>Quantidade de registro encontrados com base na consulta.</returns>
		public long Count()
		{
			GDASession session = null;
			return Count(session);
		}

		/// <summary>
		/// Recupera a quantidade de registros da tabela no banco.
		/// </summary>
		/// <param name="session">Sessão usada para executa o comando.</param>
		/// <returns>Quantidade de registro encontrados com base na consulta.</returns>
		public long Count(GDASession session)
		{
			var sql = string.Format("SELECT COUNT(*) FROM {0}", SystemTableName);
			IDbConnection conn = CreateConnection(session);
			IDbCommand cmd = CreateCommand(session, conn);
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
				SendMessageDebugTrace("CommandText: " + cmd.CommandText);
				long value = 0;
				using (var executionHandler = Diagnostics.GDATrace.CreateExecutionHandler(cmd))
					try
					{
						value = Convert.ToInt64(cmd.ExecuteScalar());
					}
					catch(Exception ex)
					{
						executionHandler.Fail(ex);
						throw ex;
					}
				SendMessageDebugTrace("Return: " + value.ToString());
				return value;
			}
			catch(Exception ex)
			{
				throw new GDAException(ex);
			}
			finally
			{
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
		}

		/// <summary>
		/// Executa o camando que retorna o um número de registros.
		/// </summary>
		/// <param name="parameters">Parametros a serem aplicados.</param>
		/// <returns>Número de registro encontrados com base nos parametros</returns>
		/// <exception cref="GDAException"></exception>
		public int GetNumberRegFound(params GDAParameter[] parameters)
		{
			return GetNumberRegFound(null, parameters);
		}

		/// <summary>
		/// Executa o camando que retorna o um número de registros.
		/// </summary>
		/// <param name="session">Sessão usada para executa o comando.</param>
		/// <param name="parameters">Parametros a serem aplicados.</param>
		/// <returns>Número de registro encontrados com base nos parametros</returns>
		/// <exception cref="GDAException"></exception>
		public int GetNumberRegFound(GDASession session, params GDAParameter[] parameters)
		{
			IDbConnection conn = CreateConnection(session);
			IDbCommand cmd = CreateCommand(session, conn);
			int value = 0;
			string sqlParam = "", sql = "SELECT COUNT(*) FROM " + SystemTableName;
			cmd.Connection = conn;
			if(parameters != null)
				foreach (GDAParameter p in parameters)
				{
					DbParameter param = Configuration.Provider.CreateParameter();
					param.ParameterName = p.ParameterName.Replace("?", UserProvider.ParameterPrefix) + UserProvider.ParameterSuffix;
					param.Value = p.Value;
					if(sqlParam != "")
						sqlParam += " AND ";
					if(p.SourceColumn == null || p.SourceColumn == "")
						throw new GDAException("Nome do campo que o parâmetro \"" + p.ParameterName + "\" representa não foi informado.");
					sqlParam += UserProvider.QuoteExpression(p.SourceColumn) + "=" + param.ParameterName;
					SendMessageDebugTrace("Create DataParameter -> Name: " + param.ParameterName + "; Value: " + param.Value + "; FieldName: " + param.SourceColumn);
					cmd.Parameters.Add(param);
				}
			if(sqlParam != "")
				sql += " WHERE ";
			cmd.CommandText = sql + sqlParam;
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
				SendMessageDebugTrace("CommandText: " + cmd.CommandText);
				using (var executionHandler = Diagnostics.GDATrace.CreateExecutionHandler(cmd))
					try
					{
						value = int.Parse(cmd.ExecuteScalar().ToString());
					}
					catch(Exception ex)
					{
						executionHandler.Fail(ex);
						throw ex;
					}
				SendMessageDebugTrace("Return: " + value.ToString());
			}
			catch(Exception ex)
			{
				throw new GDAException(ex);
			}
			finally
			{
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
			return value;
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
			if(string.IsNullOrEmpty(propertyName))
				throw new ArgumentNullException("properyName");
			else if(parent == null)
				throw new ArgumentNullException("parent");
			string sqlParam = "";
			GDAParameter[] parameters = null;
			Mapper checkPropertyMapper = null;
			foreach (var i in MappingManager.GetMappers<Model>())
				if(i.PropertyMapperName == propertyName)
				{
					checkPropertyMapper = i;
					break;
				}
			if(checkPropertyMapper == null)
				throw new GDAException("Property {0} of model {1} not mapped", propertyName, typeof(Model).FullName);
			StringBuilder buf = new StringBuilder("SELECT COUNT(*)");
			buf.Append(" FROM ").Append(SystemTableName).Append(" ");
			if(mode == ValidationMode.Insert)
			{
				buf.Append(string.Format("WHERE {0}={1}", UserProvider.QuoteExpression(checkPropertyMapper.Name), UserProvider.ParameterPrefix + checkPropertyMapper.Name + UserProvider.ParameterSuffix));
				parameters = new GDAParameter[] {
					new GDAParameter(UserProvider.ParameterPrefix + checkPropertyMapper.Name + UserProvider.ParameterSuffix, propertyValue)
				};
			}
			else
			{
				List<Mapper> properties = Keys;
				if(properties.Count == 0)
					throw new GDAException("In model {0} not found keys for to recover data.", parent.GetType().FullName);
				parameters = new GDAParameter[properties.Count + 1];
				int i = 0;
				foreach (Mapper mapper in properties)
				{
					if(sqlParam != "")
						sqlParam += " AND ";
					parameters[i] = new GDAParameter(UserProvider.ParameterPrefix + mapper.Name + UserProvider.ParameterSuffix, typeof(Model).GetProperty(mapper.PropertyMapper.Name).GetValue(parent, null));
					sqlParam += UserProvider.QuoteExpression(mapper.Name) + "<>" + parameters[i].ParameterName;
					i++;
				}
				if(sqlParam != "")
					sqlParam += " AND ";
				sqlParam += string.Format("{0}={1}", UserProvider.QuoteExpression(checkPropertyMapper.Name), UserProvider.ParameterPrefix + checkPropertyMapper.Name + UserProvider.ParameterSuffix);
				parameters[parameters.Length - 1] = new GDAParameter(UserProvider.ParameterPrefix + checkPropertyMapper.Name + UserProvider.ParameterSuffix, propertyValue);
				buf.Append("WHERE ").Append(sqlParam);
			}
			return ExecuteSqlQueryCount(session, buf.ToString(), parameters) > 0;
		}

		/// <summary>
		/// Busca os dados relacionados com a consulta submetida.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <param name="query">Consulta.</param>
		/// <returns></returns>
		public virtual GDADataRecordCursor<Model> SelectToDataRecord(GDASession session, IQuery query)
		{
			return new GDADataRecordCursor<Model>(GetCursorParameters(session, query));
		}
	}
}
