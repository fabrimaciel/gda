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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDA.Provider.MsSqlv45
{
	/// <summary>
	/// Classe que implementa o Fix para o SqlCommand.
	/// </summary>
	class SqlCommandFix : System.Data.IDbCommand
	{
		private System.Data.SqlClient.SqlCommand _command;

		/// <summary>
		/// Comando que está sendo adaptado.
		/// </summary>
		protected System.Data.SqlClient.SqlCommand Command
		{
			get
			{
				return _command;
			}
		}

		/// <summary>
		/// Texto do comando.
		/// </summary>
		public string CommandText
		{
			get
			{
				return Command.CommandText;
			}
			set
			{
				Command.CommandText = value;
			}
		}

		/// <summary>
		/// Tempo limite de execução do comando.
		/// </summary>
		public int CommandTimeout
		{
			get
			{
				return Command.CommandTimeout;
			}
			set
			{
				Command.CommandTimeout = value;
			}
		}

		/// <summary>
		/// Tipo do comando.
		/// </summary>
		public System.Data.CommandType CommandType
		{
			get
			{
				return Command.CommandType;
			}
			set
			{
				Command.CommandType = value;
			}
		}

		/// <summary>
		/// Conexão associada.
		/// </summary>
		public System.Data.IDbConnection Connection
		{
			get
			{
				return Command.Connection;
			}
			set
			{
				Command.Connection = (System.Data.SqlClient.SqlConnection)value;
			}
		}

		/// <summary>
		/// Relação dos parametros do comando.
		/// </summary>
		public System.Data.IDataParameterCollection Parameters
		{
			get
			{
				return Command.Parameters;
			}
		}

		/// <summary>
		/// Transação associada com o comando.
		/// </summary>
		public System.Data.IDbTransaction Transaction
		{
			get
			{
				return Command.Transaction;
			}
			set
			{
				Command.Transaction = (System.Data.SqlClient.SqlTransaction)value;
			}
		}

		/// <summary>
		/// Origem de linhas atualizadas.
		/// </summary>
		public System.Data.UpdateRowSource UpdatedRowSource
		{
			get
			{
				return Command.UpdatedRowSource;
			}
			set
			{
				Command.UpdatedRowSource = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="command">Instancia do comando que será adaptado.</param>
		public SqlCommandFix(System.Data.SqlClient.SqlCommand command)
		{
			if(command == null)
				throw new ArgumentNullException("command");
			_command = command;
		}

		/// <summary>
		/// Destrutor.
		/// </summary>
		~SqlCommandFix()
		{
			Dispose(false);
		}

		/// <summary>
		/// Recupera o resultado da tarefa que está sendo executada.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="task"></param>
		/// <returns></returns>
		private static T GetTaskResult<T>(Task<T> task)
		{
			Exception exception = null;
			try
			{
				task.Wait();
			}
			catch(Exception ex)
			{
				exception = ex;
			}
			if(exception != null)
			{
				var aggregateException = exception as AggregateException;
				if(aggregateException != null && aggregateException.InnerExceptions.Count == 1)
					exception = aggregateException.InnerExceptions[0];
				throw exception;
			}
			return task.Result;
		}

		/// <summary>
		/// Cancela a execução.
		/// </summary>
		public void Cancel()
		{
			Command.Cancel();
		}

		/// <summary>
		/// Cria um novo parametro.
		/// </summary>
		/// <returns></returns>
		public System.Data.IDbDataParameter CreateParameter()
		{
			return Command.CreateParameter();
		}

		/// <summary>
		/// Executa o comando sem esperar resultado.
		/// </summary>
		/// <returns></returns>
		public int ExecuteNonQuery()
		{
			return GetTaskResult(Command.ExecuteNonQueryAsync());
		}

		/// <summary>
		/// Executa o comando e recupera o leitor dos dados.
		/// </summary>
		/// <param name="behavior"></param>
		/// <returns></returns>
		public System.Data.IDataReader ExecuteReader(System.Data.CommandBehavior behavior)
		{
			return GetTaskResult(Command.ExecuteReaderAsync(behavior));
		}

		/// <summary>
		/// Executa o comando e recupera o leitor dos dados.
		/// </summary>
		/// <returns></returns>
		public System.Data.IDataReader ExecuteReader()
		{
			return GetTaskResult(Command.ExecuteReaderAsync());
		}

		/// <summary>
		/// Realiza uma executa escalar o comando.
		/// </summary>
		/// <returns></returns>
		public object ExecuteScalar()
		{
			return GetTaskResult(Command.ExecuteScalarAsync());
		}

		/// <summary>
		/// Prepara o comando.
		/// </summary>
		public void Prepare()
		{
			Command.Prepare();
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			Command.Dispose();
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
