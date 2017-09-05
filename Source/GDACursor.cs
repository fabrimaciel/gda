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
using GDA.Interfaces;

namespace GDA.Collections
{
	/// <summary>
	/// Armazena os parametros para inicialização de um curso do GDA.
	/// </summary>
	internal class GDACursorParameters
	{
		public readonly IProvider Provider;

		/// <summary>
		/// Sessao que sera usada.
		/// </summary>
		public readonly GDASession Session;

		/// <summary>
		/// Conexao que sera usada para a recuperacao do resultado.
		/// </summary>
		public readonly IDbConnection Connection;

		/// <summary>
		/// Comando usado para recuperar o resultado.
		/// </summary>
		public readonly IDbCommand Command;

		/// <summary>
		/// Lista dos atributos a serem carregados.
		/// </summary>
		public readonly TranslatorDataInfoCollection TranslatorDataInfos;

		/// <summary>
		/// Identifica se o resultado vai ser paginado.
		/// </summary>
		public readonly bool UsingPaging;

		/// <summary>
		/// Ponto inicial da pagina.
		/// </summary>
		public readonly int StartPage;

		/// <summary>
		/// Tamanho do pagina
		/// </summary>
		public readonly int PageSize;

		/// <summary>
		/// Evento que será acionado no inicio do processamento do cursor.
		/// </summary>
		public readonly EventHandler StartProcess;

		/// <summary>
		/// Construtor padrão
		/// </summary>
		/// <param name="provider"></param>
		/// <param name="session">Sessao que sera usada.</param>
		/// <param name="connection">Conexao que sera usada para a recuperacao do resultado.</param>
		/// <param name="command">Comando usado para recuperar o resultado.</param>
		/// <param name="translatorDataInfos">Lista dos atributos a serem carregados.</param>
		/// <param name="usingPaging">Identifica se o resultado vai ser paginado.</param>
		/// <param name="startPage">Ponto inicial da pagina.</param>
		/// <param name="pageSize">Tamanho do pagina</param>
		/// <param name="startProcess">Evento que será acionado no inicio do processamento do cursor.</param>
		public GDACursorParameters(IProvider provider, GDASession session, IDbConnection connection, IDbCommand command, TranslatorDataInfoCollection translatorDataInfos, bool usingPaging, int startPage, int pageSize, EventHandler startProcess)
		{
			if(command == null)
				throw new ArgumentNullException("command");
			Session = session;
			Connection = connection;
			Command = command;
			TranslatorDataInfos = translatorDataInfos;
			UsingPaging = usingPaging;
			StartPage = startPage;
			PageSize = pageSize;
			StartProcess = startProcess;
			Provider = provider;
		}
	}
	/// <summary>
	/// Representa um cursor usado para navegar pelo resultado de uma consulta.
	/// </summary>
	/// <typeparam name="Model"></typeparam>
	public class GDACursor<Model> : IEnumerable<Model>, IDisposable where Model : new()
	{
		private GDASession _session;

		private IDbConnection _connection;

		private IDbCommand _command;

		private TranslatorDataInfoCollection _translatorDataInfo;

		/// <summary>
		/// Ponto inicial da pagina de resultado.
		/// </summary>
		private int _startPage = 0;

		/// <summary>
		/// Tamanho da pagina do resultado.
		/// </summary>
		private int _pageSize = 0;

		/// <summary>
		/// Provider usado para o resultado.
		/// </summary>
		private IProvider _provider;

		/// <summary>
		/// Identifica se vai se vai ser usado paginacao no resultado.
		/// </summary>
		private bool _usingPaging;

		/// <summary>
		/// Instancia do eventio que sera acionado quando o cursor comecar a ser processado.
		/// </summary>
		private EventHandler _startProcess;

		/// <summary>
		/// Identifica se o cursor foi acionado a partir do método LoadValueS
		/// </summary>
		private bool _isLoadValues;

		/// <summary>
		/// Identifica que o enumerador foi criado.
		/// </summary>
		private bool _enumeratorCreated = false;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="parameters"></param>
		internal GDACursor(GDACursorParameters parameters)
		{
			_session = parameters.Session;
			_connection = parameters.Connection;
			_command = parameters.Command;
			_translatorDataInfo = parameters.TranslatorDataInfos;
			_usingPaging = parameters.UsingPaging;
			_startPage = parameters.StartPage;
			_pageSize = parameters.PageSize;
			_startProcess = parameters.StartProcess;
			_provider = parameters.Provider;
			_isLoadValues = false;
		}

		/// <param name="session">Sessao que sera usada.</param>
		/// <param name="connection">Conexao que sera usada para a recuperacao do resultado.</param>
		/// <param name="command">Comando usado para recuperar o resultado.</param>
		/// <param name="startProcess">Evento que será acionado no inicio do processamento do cursor.</param>
		internal GDACursor(IProvider provider, GDASession session, IDbConnection connection, IDbCommand command, EventHandler startProcess)
		{
			if(_command == null)
				throw new ArgumentNullException("command");
			_session = session;
			_connection = connection;
			_command = command;
			_startProcess = startProcess;
			_provider = provider;
			_isLoadValues = true;
		}

		/// <summary>
		/// Destrutor
		/// </summary>
		~GDACursor()
		{
			Dispose(false);
		}

		/// <summary>
		/// Envia uma mensagem para o debug.
		/// </summary>
		/// <param name="message">Mensagem a ser enviada.</param>
		private void SendMessageDebugTrace(string message)
		{
			#if DEBUG
			            //System.Diagnostics.Debug.WriteLine(message);
#endif
			if(GDASettings.EnabledDebugTrace)
				GDAOperations.CallDebugTrace(this, message);
		}

		/// <summary>
		/// Recupera a lista do cursor.
		/// </summary>
		/// <returns></returns>
		public List<Model> ToList()
		{
			return new List<Model>(this);
		}

		/// <summary>
		/// Recupera o Enumerator dos dados.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<Model> GetEnumerator()
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
				SendMessageDebugTrace("GDACursor: " + _command.CommandText);
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
				if(dReader == null)
					throw new InvalidOperationException(string.Format("Execute Reader result from IDbCommand \"{0}\" couldn't be null.", _command.GetType().FullName));
				_translatorDataInfo.ProcessFieldsPositions(dReader);
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
					if(!_isLoadValues)
					{
						Model objItem = new Model();
						IDataRecord record = dReader;
						PersistenceObject<Model>.RecoverValueOfResult(ref record, _translatorDataInfo, ref objItem, false);
						yield return objItem;
					}
					else
					{
						if(dReader[0] == DBNull.Value)
							yield return default(Model);
						else
							yield return (Model)dReader[0];
					}
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
							_connection = null;
						}
						catch
						{
							SendMessageDebugTrace("Error close connection.");
						}
				}
			}
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		/// <summary>
		/// Converte implicitamente para um lista tipada.
		/// </summary>
		/// <param name="collection"></param>
		/// <returns>Lista tipada.</returns>
		public static implicit operator List<Model>(GDACursor<Model> collection)
		{
			return new List<Model>(collection);
		}

		public static implicit operator GDAItemCollection<Model>(GDACursor<Model> collection)
		{
			return new GDAItemCollection<Model>(collection);
		}

		/// <summary>
		/// Converte implicitamente para uma lista tipada.
		/// </summary>
		/// <param name="collection"></param>
		/// <returns></returns>
		public static implicit operator GDAList<Model>(GDACursor<Model> collection)
		{
			return new GDAList<Model>(collection);
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
						_connection = null;
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
}
