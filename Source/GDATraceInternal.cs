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
	class GDATraceInternal
	{
		internal static readonly object _critSec;

		private static bool _useGlobalLock;

		private static bool _autoFlush;

		private static GDATraceListenerCollection _listeners;

		/// <summary>
		/// Identifica se é para usar o lock global.
		/// </summary>
		public static bool UseGlobalLock
		{
			get
			{
				InitializeSettings();
				return _useGlobalLock;
			}
			set
			{
				InitializeSettings();
				_useGlobalLock = value;
			}
		}

		/// <summary>
		/// Relação dos listener registrados.
		/// </summary>
		public static GDATraceListenerCollection Listeners
		{
			get
			{
				InitializeSettings();
				if(_listeners == null)
				{
					lock (_critSec)
					{
						if(_listeners == null)
						{
							_listeners = new GDATraceListenerCollection();
						}
					}
				}
				return _listeners;
			}
		}

		/// <summary>
		/// Identifica se é para auto liberar os dados.
		/// </summary>
		public static bool AutoFlush
		{
			get
			{
				InitializeSettings();
				return _autoFlush;
			}
			set
			{
				InitializeSettings();
				_autoFlush = value;
			}
		}

		/// <summary>
		/// Construtor estático.
		/// </summary>
		static GDATraceInternal()
		{
			_critSec = new object();
		}

		/// <summary>
		/// Inicializa as configurações.
		/// </summary>
		private static void InitializeSettings()
		{
			_useGlobalLock = true;
		}

		/// <summary>
		/// Notifica o inicio de execução de uma comando.
		/// </summary>
		/// <param name="executionInfo"></param>
		public static void NotifyBeginExecution(CommandExecutionInfo executionInfo)
		{
			try
			{
				if(UseGlobalLock)
				{
					lock (_critSec)
					{
						foreach (GDATraceListener listener in Listeners)
						{
							listener.NotifyBeginExecution(executionInfo);
							if(AutoFlush)
								listener.Flush();
						}
						return;
					}
				}
				foreach (GDATraceListener listener2 in Listeners)
				{
					if(!listener2.IsThreadSafe)
					{
						lock (listener2)
						{
							listener2.NotifyBeginExecution(executionInfo);
							if(AutoFlush)
								listener2.Flush();
							continue;
						}
					}
					listener2.NotifyBeginExecution(executionInfo);
					if(AutoFlush)
						listener2.Flush();
				}
			}
			catch(Exception ex)
			{
				throw new GDATraceException(string.Format("GDA Trace notify begin execution error. {0}", ex.Message), ex);
			}
		}

		/// <summary>
		/// Notificação a execução de um comando no banco de dados.
		/// </summary>
		/// <param name="executionInfo"></param>
		public static void NotifyExecution(CommandExecutionInfo executionInfo)
		{
			try
			{
				if(UseGlobalLock)
				{
					lock (_critSec)
					{
						foreach (GDATraceListener listener in Listeners)
						{
							listener.NotifyExecution(executionInfo);
							if(AutoFlush)
								listener.Flush();
						}
						return;
					}
				}
				foreach (GDATraceListener listener2 in Listeners)
				{
					if(!listener2.IsThreadSafe)
					{
						lock (listener2)
						{
							listener2.NotifyExecution(executionInfo);
							if(AutoFlush)
								listener2.Flush();
							continue;
						}
					}
					listener2.NotifyExecution(executionInfo);
					if(AutoFlush)
						listener2.Flush();
				}
			}
			catch(Exception ex)
			{
				throw new GDATraceException(string.Format("GDA Trace notify execution error. {0}", ex.Message), ex);
			}
		}
	}
}
