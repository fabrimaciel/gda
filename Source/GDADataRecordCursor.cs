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

namespace GDA.Collections
{
	/// <summary>
	/// Implementação base do cursor.
	/// </summary>
	public abstract class BaseGDADataRecordCursor : IDisposable
	{
		protected GDASession _session;

		private IDbConnection _connection;

		protected IDbCommand _command;

		/// <summary>
		/// Texto de consulta do comando.
		/// </summary>
		protected string _commandText;

		protected IDictionary<string, int> _mapFields;

		protected bool _mapFieldsLoaded = false;

		/// <summary>
		/// Ponto inicial da pagina de resultado.
		/// </summary>
		protected int _startPage = 0;

		/// <summary>
		/// Tamanho da pagina do resultado.
		/// </summary>
		protected int _pageSize = 0;

		/// <summary>
		/// Provider usado para o resultado.
		/// </summary>
		protected IProvider _provider;

		/// <summary>
		/// Identifica se vai se vai ser usado paginacao no resultado.
		/// </summary>
		protected bool _usingPaging;

		/// <summary>
		/// Instancia do eventio que sera acionado quando o cursor comecar a ser processado.
		/// </summary>
		protected EventHandler _startProcess;

		/// <summary>
		/// Identifica se o enumerador foi criado.
		/// </summary>
		private bool _enumeratorCreated = false;

		protected IDbConnection Connection
		{
			get
			{
				return _connection;
			}
		}

		/// <summary>
		/// Instancia do provider relacionada com o cursor.
		/// </summary>
		public GDA.Interfaces.IProvider Provider
		{
			get
			{
				return _provider;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="parameters"></param>
		internal BaseGDADataRecordCursor(GDACursorParameters parameters)
		{
			if(parameters == null)
				throw new ArgumentNullException("parameters");
			_session = parameters.Session;
			_connection = parameters.Connection;
			_command = parameters.Command;
			if(parameters.Command != null)
				_commandText = parameters.Command.CommandText;
			_usingPaging = parameters.UsingPaging;
			_startPage = parameters.StartPage;
			_pageSize = parameters.PageSize;
			_startProcess = parameters.StartProcess;
			_provider = parameters.Provider;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public BaseGDADataRecordCursor()
		{
		}

		/// <summary>
		/// Destrutor.
		/// </summary>
		~BaseGDADataRecordCursor()
		{
			Dispose(false);
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
				GDAOperations.CallDebugTrace(this, message);
		}

		/// <summary>
		/// Método usado para carregar o tradutor dos termos
		/// </summary>
		protected virtual IDictionary<string, int> OnLoadTranslator(IDataReader dataRecord)
		{
			return null;
		}

		/// <summary>
		/// Método usado para cria um registro de dados.
		/// </summary>
		/// <param name="record"></param>
		/// <param name="mapFields"></param>
		protected abstract T CreateDataRecord<T>(IDataRecord record, IDictionary<string, int> mapFields) where T : GDADataRecord;

		/// <summary>
		/// Cria o enumerador.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		protected IEnumerator<T> CreateEnumerator<T>() where T : GDADataRecord
		{
			_enumeratorCreated = true;
			if(_session == null && _connection.State != ConnectionState.Open)
			{
				try
				{
					_connection.Open();
				}
				catch(Exception ex)
				{
					try
					{
						_connection.Dispose();
					}
					catch
					{
					}
					_connection = null;
					throw new GDAException(ex);
				}
				GDAConnectionManager.NotifyConnectionOpened(_connection);
			}
			IDataReader dReader = null;
			try
			{
				StringBuilder sb = new StringBuilder();
				sb.AppendFormat("GDADataRecordCursor: {0}\r\n", _command.CommandText);
				if(_command.Parameters.Count > 0)
				{
					sb.Append("--> Parameters:");
					foreach (IDataParameter parameter in _command.Parameters)
						if(parameter.Value is byte[])
							sb.Append("\r\n").Append(parameter.ParameterName).Append(" = byte[]");
						else
							sb.Append("\r\n").Append(parameter.ParameterName).Append(" = ").Append(parameter.Value == null ? "NULL" : '"' + parameter.Value.ToString() + '"');
				}
				SendMessageDebugTrace(sb.ToString());
				using (var executionHandler = Diagnostics.GDATrace.CreateExecutionHandler(_command))
					try
					{
						dReader = _command.ExecuteReader();
					}
					catch(Exception ex)
					{
						ex = new GDAException(ex);
						executionHandler.Fail(ex);
						throw ex;
					}
				if(!_mapFieldsLoaded)
				{
					_mapFields = OnLoadTranslator(dReader);
					_mapFieldsLoaded = true;
				}
				int startPage = _startPage;
				int countPageSize = 0;
				while (dReader.Read())
				{
					if(countPageSize == 0 && _startProcess != null)
						_startProcess(this, EventArgs.Empty);
					if(_usingPaging && !_provider.SupportSQLCommandLimit && startPage < _pageSize)
					{
						startPage++;
						continue;
					}
					yield return CreateDataRecord<T>(dReader, _mapFields);
					countPageSize++;
					if(_usingPaging && !_provider.SupportSQLCommandLimit && countPageSize >= _pageSize)
						break;
				}
			}
			finally
			{
				try
				{
					if(dReader != null)
					{
						dReader.Close();
						dReader.Dispose();
					}
					if(_command != null)
						_command.Dispose();
					_command = null;
				}
				finally
				{
					if(_session == null)
						try
						{
							_connection.Close();
							_connection.Dispose();
						}
						catch
						{
							SendMessageDebugTrace("Error close connection.");
						}
				}
			}
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			if(_command != null)
				_command.Dispose();
			_command = null;
			if(!_enumeratorCreated && _connection != null)
				if(_session == null)
					try
					{
						_connection.Close();
						_connection.Dispose();
					}
					catch
					{
						SendMessageDebugTrace("Error close connection.");
					}
			_session = null;
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
	/// <summary>
	/// Representa o cursor para os registros de dados do resultado de uma consulta.
	/// </summary>
	public class GDADataRecordCursor : BaseGDADataRecordCursor, IEnumerable<GDADataRecord>
	{
		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="parameters"></param>
		internal GDADataRecordCursor(GDACursorParameters parameters) : base(parameters)
		{
		}

		/// <summary>
		/// Cria o registro de dados.
		/// </summary>
		/// <typeparam name="T">Tipo da Model que será tratado no resultado do consulta.</typeparam>
		/// <param name="record">Instancia do registro dos dados.</param>
		/// <param name="mapFields">Mapeamento para recuperar os campos</param>
		/// <returns></returns>
		protected override T CreateDataRecord<T>(IDataRecord record, IDictionary<string, int> mapFields)
		{
			return (T)new GDADataRecord(record, mapFields);
		}

		/// <summary>
		/// Recupera o enumerador dos registros.
		/// </summary>
		/// <returns></returns>
		public virtual IEnumerator<GDADataRecord> GetEnumerator()
		{
			return CreateEnumerator<GDADataRecord>();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
	/// <summary>
	/// Implementação do cursor para suporte a um tipo especifico.
	/// </summary>
	/// <typeparam name="Model"></typeparam>
	public class GDADataRecordCursor<Model> : GDADataRecordCursor, IEnumerable<GDADataRecord<Model>>
	{
		private TranslatorDataInfoCollection _translatorDataInfos;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="parameters"></param>
		internal GDADataRecordCursor(GDACursorParameters parameters) : base(parameters)
		{
			_translatorDataInfos = parameters.TranslatorDataInfos;
		}

		/// <summary>
		/// Método acionado quando o tradutor for chamado.
		/// </summary>
		/// <param name="dataReader"></param>
		/// <returns></returns>
		protected override IDictionary<string, int> OnLoadTranslator(IDataReader dataReader)
		{
			_translatorDataInfos.ProcessFieldsPositions(dataReader);
			return _translatorDataInfos;
		}

		/// <summary>
		/// Cria um registro com base o map informado.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="record"></param>
		/// <param name="mapFields"></param>
		/// <returns></returns>
		protected override T CreateDataRecord<T>(IDataRecord record, IDictionary<string, int> mapFields)
		{
			return (T)(GDADataRecord)new GDADataRecord<Model>(record, mapFields as TranslatorDataInfoCollection);
		}

		/// <summary>
		/// Converte a coleção para o objeto informado.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public IEnumerable<T> Select<T>() where T : new()
		{
			foreach (var i in new GDADataRecordCursorEx<T>(new GDACursorParameters(_provider, _session, Connection, _command, _translatorDataInfos, _usingPaging, _startPage, _pageSize, _startProcess)))
				yield return i.GetInstance();
		}

		/// <summary>
		/// Recupera o enumerador do cursor.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<GDADataRecord<Model>> GetEnumerator()
		{
			return CreateEnumerator<GDADataRecord<Model>>();
		}

		/// <summary>
		/// Recupera o enumerador do cursor.
		/// </summary>
		/// <returns></returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
	/// <summary>
	/// Implementação de extensão.
	/// </summary>
	/// <typeparam name="Model"></typeparam>
	public class GDADataRecordCursorEx<Model> : GDADataRecordCursor<Model> where Model : new()
	{
		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="parameters"></param>
		internal GDADataRecordCursorEx(GDACursorParameters parameters) : base(parameters)
		{
		}

		/// <summary>
		/// Cria um registro.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="record"></param>
		/// <param name="mapFields"></param>
		/// <returns></returns>
		protected override T CreateDataRecord<T>(IDataRecord record, IDictionary<string, int> mapFields)
		{
			return (T)(GDADataRecord)new GDADataRecordEx<Model>(record, mapFields as TranslatorDataInfoCollection);
		}
	}
}
