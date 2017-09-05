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

namespace GDA.Diagnostics
{
	/// <summary>
	/// Classe que trata o rastreamento dos eventos do GDA.
	/// </summary>
	public static class GDATrace
	{
		/// <summary>
		/// Listener do trace.
		/// </summary>
		public static GDATraceListenerCollection Listeners
		{
			get
			{
				return GDATraceInternal.Listeners;
			}
		}

		/// <summary>
		/// Cria o manipulador para a execução do comando.
		/// </summary>
		/// <param name="command"></param>
		/// <returns></returns>
		internal static ExecutionHandler CreateExecutionHandler(System.Data.IDbCommand command)
		{
			return new ExecutionHandler(command);
		}

		/// <summary>
		/// Notifica o inicio da execução do comando.
		/// </summary>
		/// <param name="executionInfo"></param>
		public static void NotifyBeginExecution(CommandExecutionInfo executionInfo)
		{
			GDATraceInternal.NotifyBeginExecution(executionInfo);
		}

		/// <summary>
		/// Notificação a execução de um comando no banco de dados.
		/// </summary>
		/// <param name="executionInfo"></param>
		public static void NotifyExecution(CommandExecutionInfo executionInfo)
		{
			GDATraceInternal.NotifyExecution(executionInfo);
		}

		/// <summary>
		/// Manipulador da execução dos comando.
		/// </summary>
		public sealed class ExecutionHandler : IDisposable
		{
			private CommandExecutionInfo _executionInfo;

			private System.Diagnostics.Stopwatch _stopwatch = new System.Diagnostics.Stopwatch();

			private int _rowsAffects;

			/// <summary>
			/// Quantidade de linhas afetadas.
			/// </summary>
			public int RowsAffects
			{
				get
				{
					return _rowsAffects;
				}
				set
				{
					_rowsAffects = value;
				}
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="command"></param>
			public ExecutionHandler(System.Data.IDbCommand command)
			{
				_executionInfo = new CommandExecutionInfo(command);
				_stopwatch.Start();
			}

			/// <summary>
			/// Notifica o erro ocorrido.
			/// </summary>
			/// <param name="exception"></param>
			public void Fail(Exception error)
			{
				_stopwatch.Stop();
				CommandExecutionInfo executionInfo = null;
				try
				{
					executionInfo = _executionInfo.Fail(_stopwatch.Elapsed, error);
				}
				catch(Exception ex)
				{
					throw new GDATraceException(string.Format("An error occurred when get fail details for error \"{0}\".\r\n{1}", error.Message, ex.Message), ex);
				}
				GDATrace.NotifyExecution(executionInfo);
				_executionInfo = null;
			}

			/// <summary>
			/// Libera a instancia.
			/// </summary>
			public void Dispose()
			{
				var info = _executionInfo;
				if(info != null)
				{
					_stopwatch.Stop();
					GDATrace.NotifyExecution(info.Finish(_stopwatch.Elapsed, RowsAffects));
				}
			}
		}
	}
}
