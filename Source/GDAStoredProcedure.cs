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

namespace GDA
{
	/// <summary>
	/// Armazenas as informações de uma store procedure.
	/// </summary>
	public class GDAStoredProcedure : IEnumerable<GDAParameter>
	{
		private string _name;

		private int _commandTimeOut = GDASession.DefaultCommandTimeout;

		/// <summary>
		/// Lista dos parametros da stored procedure.
		/// </summary>
		private List<GDAParameter> _parameters = new List<GDAParameter>();

		/// <summary>
		/// Nome da stored procedure.
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}

		/// <summary>
		/// Captura e define o tempo de espera depois de terminar a tentativa to executar o comando
		/// e gerar um erro.
		/// O tempo é em segundos. O padrão é 30 segundos.
		/// </summary>
		public int CommandTimeout
		{
			get
			{
				return _commandTimeOut;
			}
			set
			{
				_commandTimeOut = value;
			}
		}

		/// <summary>
		/// Quantidade de parametros da stored procedure.
		/// </summary>
		public int Count
		{
			get
			{
				return _parameters.Count;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="name">Nome da stored procedure.</param>
		public GDAStoredProcedure(string name)
		{
			_name = name;
		}

		/// <summary>
		/// Localiza o parametro da stored procedure.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		private GDAParameter FindParameter(string name)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			GDAParameter p = null;
			if(name[0] == '?')
			{
				name = name.Substring(1);
				p = _parameters.Find(delegate(GDAParameter gp) {
					return gp.ParameterName.Substring(1) == name;
				});
			}
			else
				p = _parameters.Find(delegate(GDAParameter gp) {
					return gp.ParameterName == name;
				});
			if(p == null)
				throw new ItemNotFoundException(String.Format("Parameter {0} not found in stored procedure.", name));
			return p;
		}

		/// <summary>
		/// Adiciona um parametro da saído do banco.
		/// </summary>
		/// <param name="name">Nome do parametro.</param>
		/// <returns>Referência da atual stored procedure.</returns>
		public GDAStoredProcedure AddOutputParameter(string name, DbType parameterType)
		{
			GDAParameter p = new GDAParameter(name, null);
			p.Direction = ParameterDirection.Output;
			p.DbType = parameterType;
			_parameters.Add(p);
			return this;
		}

		/// <summary>
		/// Adiciona um parametro usado na stored procedure.
		/// Com a direção Input por padrão.
		/// </summary>
		/// <param name="name">Nome do parametro.</param>
		/// <param name="value">Valor do parametro</param>
		/// <returns>Referência da atual stored procedure.</returns>
		public GDAStoredProcedure AddParameter(string name, object value)
		{
			return AddParameter(name, value, System.Data.ParameterDirection.Input);
		}

		/// <summary>
		/// Adiciona um parametro usado na stored procedure.
		/// </summary>
		/// <param name="name">Nome do parametro.</param>
		/// <param name="value">Valor do parametro.</param>
		/// <param name="direction">Direção do parametro.</param>
		/// <returns>Referência da atual stored procedure.</returns>
		public GDAStoredProcedure AddParameter(string name, object value, System.Data.ParameterDirection direction)
		{
			_parameters.Add(new GDAParameter(name, value, direction));
			return this;
		}

		/// <summary>
		/// Adiciona um parametro usado na stored procedure.
		/// </summary>
		/// <param name="name">Nome do parametro.</param>
		/// <param name="value">Valor do parametro.</param>
		/// <param name="direction">Direção do parametro.</param>
		/// <param name="dbType">Tipo do parametro.</param>
		/// <returns>Referência da atual stored procedure.</returns>
		public GDAStoredProcedure AddParameter(string name, object value, System.Data.ParameterDirection direction, System.Data.DbType dbType)
		{
			var p = new GDAParameter(name, value, direction);
			p.DbType = dbType;
			_parameters.Add(p);
			return this;
		}

		/// <summary>
		/// Adiciona um parametro para a procedure.
		/// </summary>
		/// <param name="parameter"></param>
		/// <returns></returns>
		public GDAStoredProcedure AddParameter(GDAParameter parameter)
		{
			if(parameter == null)
				throw new ArgumentNullException("parameter");
			_parameters.Add(parameter);
			return this;
		}

		/// <summary>
		/// Adiciona vários parametros para a procedure.
		/// </summary>
		/// <param name="parameters">Instancia dos parametros que serão adicionado.</param>
		/// <returns></returns>
		public GDAStoredProcedure AddParameters(GDAParameter[] parameters)
		{
			if(parameters == null)
				throw new ArgumentNullException("parameters");
			foreach (var i in parameters)
			{
				if(i != null)
					AddParameter(i);
			}
			return this;
		}

		/// <summary>
		/// Remove o parametro da stored procedure.
		/// </summary>
		/// <param name="name">Nome do parametro.</param>
		/// <returns>Referência da atual stored procedure.</returns>
		public GDAStoredProcedure RemoveParameter(string name)
		{
			int index = _parameters.FindIndex(delegate(GDAParameter p) {
				return p.ParameterName == name;
			});
			if(index >= 0)
				_parameters.RemoveAt(index);
			else
				throw new ItemNotFoundException(String.Format("Parameter {0} not found in stored procedure.", name));
			return this;
		}

		/// <summary>
		/// Define a direção do parametro.
		/// </summary>
		/// <param name="name">Nome do parametro.</param>
		/// <returns>Referência da atual stored procedure.</returns>
		public GDAStoredProcedure SetParameterDirection(string name, System.Data.ParameterDirection direction)
		{
			FindParameter(name).Direction = direction;
			return this;
		}

		/// <summary>
		/// Define o tipo do parametro.
		/// </summary>
		/// <param name="name">Nome do parametro.</param>
		/// <param name="type">Tipo do parametro.</param>
		/// <returns>Referência da atual stored procedure.</returns>
		public GDAStoredProcedure SetParameterDbType(string name, System.Data.DbType type)
		{
			FindParameter(name).DbType = type;
			return this;
		}

		/// <summary>
		/// Prepara os dados para executar a stored procedure.
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="provider"></param>
		internal void Prepare(IDbCommand cmd, IProvider provider)
		{
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandTimeout = this.CommandTimeout;
			cmd.CommandText = this.Name;
			foreach (GDAParameter param in this)
			{
				cmd.Parameters.Add(GDA.Helper.GDAHelper.ConvertGDAParameter(cmd, param, provider));
			}
		}

		/// <summary>
		/// Recupera e define o valor do parametro.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public object this[string name]
		{
			get
			{
				object val = FindParameter(name).Value;
				if(val is DBNull)
					return null;
				else
					return val;
			}
			set
			{
				FindParameter(name).Value = value;
			}
		}

		/// <summary>
		/// Recupera e define o valor do parametro.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public object this[int index]
		{
			get
			{
				object val = _parameters[index].Value;
				if(val is DBNull)
					return null;
				else
					return val;
			}
			set
			{
				_parameters[index].Value = value;
			}
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _parameters.GetEnumerator();
		}

		IEnumerator<GDAParameter> IEnumerable<GDAParameter>.GetEnumerator()
		{
			return _parameters.GetEnumerator();
		}
	}
}
