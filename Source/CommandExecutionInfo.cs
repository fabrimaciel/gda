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
	/// Armazena as informações de execução de um comando.
	/// </summary>
	public class CommandExecutionInfo : System.Runtime.Serialization.ISerializable, System.Xml.Serialization.IXmlSerializable
	{
		private string _commandText;

		private System.Data.CommandType _commandType;

		private IEnumerable<System.Data.IDataParameter> _parameters;

		private TimeSpan _elapsedTime;

		private int _timeout;

		private bool _success;

		private Exception _error;

		private int _rowsAffects;

		/// <summary>
		/// Texto do comando da execução.
		/// </summary>
		public string CommandText
		{
			get
			{
				return _commandText;
			}
		}

		/// <summary>
		/// Tipo de comando.
		/// </summary>
		public System.Data.CommandType CommandType
		{
			get
			{
				return _commandType;
			}
		}

		/// <summary>
		/// Parametros do comando.
		/// </summary>
		public IEnumerable<System.Data.IDataParameter> Parameters
		{
			get
			{
				return _parameters;
			}
		}

		/// <summary>
		/// Tempo decorrido.
		/// </summary>
		public TimeSpan ElapsedTime
		{
			get
			{
				return _elapsedTime;
			}
		}

		/// <summary>
		/// Identifica se o comando foi executado com sucesso.
		/// </summary>
		public bool Success
		{
			get
			{
				return _success;
			}
		}

		/// <summary>
		/// Erro ocorrido.
		/// </summary>
		public Exception Error
		{
			get
			{
				return _error;
			}
		}

		/// <summary>
		/// Quantidade de linhas afetadas.
		/// </summary>
		public int RowsAffects
		{
			get
			{
				return _rowsAffects;
			}
		}

		/// <summary>
		/// Tempo limite para a execução do comando.
		/// </summary>
		public int Timeout
		{
			get
			{
				return _timeout;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public CommandExecutionInfo()
		{
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="commandText"></param>
		/// <param name="commandType"></param>
		/// <param name="parameters"></param>
		/// <param name="timeout"></param>
		/// <param name="rowsAffects"></param>
		/// <param name="executionTime"></param>
		/// <param name="error"></param>
		public CommandExecutionInfo(string commandText, System.Data.CommandType commandType, IEnumerable<System.Data.IDataParameter> parameters, int timeout, int rowsAffects, TimeSpan executionTime, Exception error)
		{
			_commandText = commandText;
			_commandType = commandType;
			_parameters = parameters;
			_timeout = timeout;
			_rowsAffects = rowsAffects;
			_elapsedTime = executionTime;
			_error = error;
			_success = error == null;
		}

		/// <summary>
		/// Cria a instancia com os dados do comando.
		/// </summary>
		/// <param name="command"></param>
		/// <param name="rowsAffects">Quantidade de linhas afetadas.</param>
		/// <param name="executionTime">Tempo de execução do comando.</param>
		public CommandExecutionInfo(System.Data.IDbCommand command, TimeSpan executionTime, int rowsAffects)
		{
			if(command == null)
				throw new ArgumentNullException("command");
			_commandText = command.CommandText;
			_commandType = command.CommandType;
			if(command.Parameters != null)
			{
				var parameters = new System.Data.IDataParameter[command.Parameters.Count];
				var enumerator = command.Parameters.GetEnumerator();
				for(var i = 0; enumerator.MoveNext(); i++)
					parameters[i] = (System.Data.IDataParameter)enumerator.Current;
				_parameters = parameters;
			}
			else
				_parameters = new System.Data.IDataParameter[0];
			_timeout = command.CommandTimeout;
			_rowsAffects = rowsAffects;
			_elapsedTime = executionTime;
			_success = true;
		}

		/// <summary>
		/// Cria a instancia com base no comando.
		/// </summary>
		/// <param name="command"></param>
		public CommandExecutionInfo(System.Data.IDbCommand command) : this(command, TimeSpan.Zero, 0)
		{
		}

		/// <summary>
		/// Cria a instancia com os dados do erro ocorrido na execução do comando.
		/// </summary>
		/// <param name="command"></param>
		/// <param name="executionTime"></param>
		/// <param name="error"></param>
		public CommandExecutionInfo(System.Data.IDbCommand command, TimeSpan executionTime, Exception error) : this(command, executionTime, 0)
		{
			_error = error;
			_success = false;
		}

		/// <summary>
		/// Construtor usado para deserializar os dados.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected CommandExecutionInfo(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			_commandText = info.GetString("CommandText");
			_commandType = (System.Data.CommandType)info.GetInt16("CommandType");
			_elapsedTime = new TimeSpan(info.GetInt64("ElapsedTime"));
			_timeout = info.GetInt32("Timeout");
			_success = info.GetBoolean("Success");
			_error = (Exception)info.GetValue("Error", typeof(Exception));
			_rowsAffects = info.GetInt32("RowsAffects");
		}

		/// <summary>
		/// Notifica que a execução foi finalizada.
		/// </summary>
		/// <param name="executionTime"></param>
		/// <param name="rowsAffects">Quantidade de linhas afetadas.</param>
		public CommandExecutionInfo Finish(TimeSpan executionTime, int rowsAffects)
		{
			return new CommandExecutionInfo(CommandText, CommandType, Parameters, Timeout, rowsAffects, executionTime, null);
		}

		/// <summary>
		/// Notifica a falha ocorrida na execução do comando.
		/// </summary>
		/// <param name="executionTime"></param>
		/// <param name="error"></param>
		/// <returns></returns>
		public CommandExecutionInfo Fail(TimeSpan executionTime, Exception error)
		{
			return new CommandExecutionInfo(CommandText, CommandType, Parameters, Timeout, 0, executionTime, error);
		}

		/// <summary>
		/// Recupera o texto da instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			var parametersText = string.Empty;
			var parameters = Parameters;
			if(parameters != null)
				parametersText = string.Join(",", parameters.Select(f => string.Format("[Name: {0}, Value: {1}]", f.ParameterName, f.Value)).ToArray());
			if(Success)
				return string.Format("[CommandType: {0}, ExecutionTime: {1}, RowsAffects: {2},\r\nCommandText: {3},\r\nParameters: {4}]", CommandType, ElapsedTime, RowsAffects, CommandText, parametersText);
			else
				return string.Format("[CommandType: {0}, ExecutionTime: {1},\r\nCommandText: {3},\r\nParameters: {4},\r\nError: {5}]", CommandType, ElapsedTime, RowsAffects, CommandText, parametersText, Error != null ? Error.Message : "");
		}

		/// <summary>
		/// Recupera os dados da instancia.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			info.AddValue("CommandText", CommandText);
			info.AddValue("CommandType", (short)CommandType);
			info.AddValue("ElapsedTime", ElapsedTime.Ticks);
			info.AddValue("Timeout", Timeout);
			info.AddValue("Success", Success);
			info.AddValue("Error", Error, typeof(Exception));
			info.AddValue("RowsAffects", RowsAffects);
		}

		System.Xml.Schema.XmlSchema System.Xml.Serialization.IXmlSerializable.GetSchema()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Recupera os dados serializados.
		/// </summary>
		/// <param name="reader"></param>
		void System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
		{
			_commandType = (System.Data.CommandType)Enum.Parse(typeof(System.Data.CommandType), reader.GetAttribute("CommandType"));
			var parts = reader.GetAttribute("ElapsedTime").Split(':');
			_elapsedTime = new TimeSpan(0, int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2].Split('.')[0]), int.Parse(parts[2].Split('.')[1]));
			_timeout = int.Parse(reader.GetAttribute("Timeout"));
			_success = bool.Parse(reader.GetAttribute("Success"));
			_rowsAffects = int.Parse(reader.GetAttribute("RowsAffects"));
			reader.MoveToElement();
			reader.ReadStartElement();
			_commandText = reader.ReadString();
			reader.ReadEndElement();
			if(!reader.IsEmptyElement)
			{
				var parameters = new List<System.Data.IDataParameter>();
				reader.ReadStartElement();
				while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
				{
					var item = new DataParameter();
					((System.Xml.Serialization.IXmlSerializable)item).ReadXml(reader);
					parameters.Add(item);
				}
				reader.ReadEndElement();
			}
			else
				reader.Skip();
		}

		/// <summary>
		/// Serializa os dados.
		/// </summary>
		/// <param name="writer"></param>
		void System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteAttributeString("CommandType", CommandType.ToString());
			writer.WriteAttributeString("ElapsedTime", string.Format("{0:00}:{1:00}:{2:00}.{3:000}", ElapsedTime.TotalHours, ElapsedTime.Minutes, ElapsedTime.Seconds, ElapsedTime.Milliseconds));
			writer.WriteAttributeString("Timeout", Timeout.ToString());
			writer.WriteAttributeString("Success", Success.ToString());
			writer.WriteAttributeString("RowsAffects", RowsAffects.ToString());
			writer.WriteStartElement("CommandText");
			writer.WriteString(CommandText);
			writer.WriteEndElement();
			writer.WriteStartElement("Parameters");
			foreach (System.Xml.Serialization.IXmlSerializable i in Parameters.Select(f => new DataParameter(f)))
			{
				writer.WriteStartElement(i.GetType().Name);
				i.WriteXml(writer);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}

		/// <summary>
		/// Representa uma parametro de dados.
		/// </summary>
		class DataParameter : System.Data.IDataParameter, System.Xml.Serialization.IXmlSerializable
		{
			/// <summary>
			/// Tipo do dado.
			/// </summary>
			public System.Data.DbType DbType
			{
				get;
				set;
			}

			/// <summary>
			/// Direção do parametro.
			/// </summary>
			public System.Data.ParameterDirection Direction
			{
				get;
				set;
			}

			/// <summary>
			/// Identifica se o parametro é nullable.
			/// </summary>
			public bool IsNullable
			{
				get;
				set;
			}

			/// <summary>
			/// Nome do parametro.
			/// </summary>
			public string ParameterName
			{
				get;
				set;
			}

			/// <summary>
			/// Nome da coluna de origem.
			/// </summary>
			public string SourceColumn
			{
				get;
				set;
			}

			/// <summary>
			/// Versão da origem.
			/// </summary>
			public System.Data.DataRowVersion SourceVersion
			{
				get;
				set;
			}

			/// <summary>
			/// Valor do parametro.
			/// </summary>
			public object Value
			{
				get;
				set;
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="parameter"></param>
			public DataParameter(System.Data.IDataParameter parameter)
			{
				if(parameter == null)
					throw new ArgumentNullException("parameter");
				ParameterName = parameter.ParameterName;
				Value = parameter.Value;
			}

			/// <summary>
			/// Cria a instancia com os dados básicos.
			/// </summary>
			/// <param name="name"></param>
			/// <param name="value"></param>
			public DataParameter(string name, object value)
			{
				this.ParameterName = name;
				this.Value = value;
			}

			/// <summary>
			/// Contrutor vazio.
			/// </summary>
			public DataParameter()
			{
			}

			System.Xml.Schema.XmlSchema System.Xml.Serialization.IXmlSerializable.GetSchema()
			{
				throw new NotImplementedException();
			}

			/// <summary>
			/// Recupera os dados serializados.
			/// </summary>
			/// <param name="reader"></param>
			void System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
			{
				ParameterName = reader.GetAttribute("Name");
				;
				reader.MoveToElement();
				reader.ReadStartElement();
				if(!reader.IsEmptyElement)
					Value = reader.ReadString();
				else
					reader.Skip();
				reader.ReadEndElement();
			}

			/// <summary>
			/// Serializa os dados.
			/// </summary>
			/// <param name="writer"></param>
			void System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
			{
				writer.WriteAttributeString("Name", ParameterName);
				if(Value != null && Value != DBNull.Value)
					writer.WriteString(Value.ToString());
			}
		}
	}
}
