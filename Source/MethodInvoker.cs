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
using System.Reflection;
using System.Collections;

namespace GDA.Common.Helper
{
	/// <summary>
	/// Helper class usada para armazena a informação sobre um método in partilucar e a abilidade
	/// para executar chamadas.
	/// </summary>
	public class MethodInvoker
	{
		/// <summary>
		/// Método relacionados
		/// </summary>
		private MethodInfo _methodInfo;

		/// <summary>
		/// Parametros relacionados com o método.
		/// </summary>
		private ParameterInfo[] _parameterInfos;

		/// <summary>
		/// Parametros padrões do método
		/// </summary>
		private object[] _parameterDefaultValues;

		/// <summary>
		/// Número de parametros requeridos pelo método.
		/// </summary>
		private int _requiredParameters;

		/// <summary>
		/// Construtor
		/// </summary>
		/// <param name="methodInfo">O <see cref="MethodInfo"/> para o método relacionado a instancia.</param>
		/// <param name="requiredParameters">Número de parametros requeridos para chamar o método.</param>
		public MethodInvoker(MethodInfo methodInfo, int requiredParameters)
		{
			this._methodInfo = methodInfo;
			this._requiredParameters = requiredParameters;
			_parameterInfos = methodInfo.GetParameters();
			_parameterDefaultValues = new object[_parameterInfos.Length];
		}

		/// <summary>
		/// O <see cref="MethodInfo"/> no qual a instancia está relacionada.
		/// </summary>
		public MethodInfo MethodInfo
		{
			get
			{
				return _methodInfo;
			}
		}

		/// <summary>
		/// Número de parametro do método (contados da direita para a esquerda).
		/// </summary>
		public int RequiredParameters
		{
			get
			{
				return _requiredParameters;
			}
		}

		/// <summary>
		/// Armazena os valores padrão do parametros. Esse valor será usado se não houve valor submetido
		/// para o parametro, e se ele não for um parametro requerido.
		/// </summary>
		/// <param name="parameterName">Nome do parametro(case-insensitive).</param>
		/// <param name="value">Valor padrão.</param>
		public void SetDefaultValue(string parameterName, object value)
		{
			int index = FindParameter(parameterName);
			throw new GDAException("Method does not have a parameter named " + parameterName);
		}

		/// <summary>
		/// Prepara um instancia <see cref="MethodInvokable"/> para chamar o método submetendo os parametros.
		/// </summary>
		/// <param name="parameters">Um hashtable contendo o parametro/valor </param>
		/// <returns>Uma instancia do <see cref="MethodInvokable"/> preparado para executar a chamada do método.</returns>
		public MethodInvokable PrepareInvoke(Hashtable parameters)
		{
			int normalCount = 0;
			int defaultCount = 0;
			int nullCount = 0;
			object[] invokeParameters = new object[_parameterInfos.Length];
			for(int i = 0; i < _parameterInfos.Length; i++)
			{
				ParameterInfo pi = _parameterInfos[i];
				string parameterName = pi.Name.ToLower();
				if(parameterName.StartsWith("_"))
					parameterName = parameterName.Substring(1, parameterName.Length - 1);
				if(parameters.ContainsKey(parameterName))
				{
					object val = parameters[parameterName];
					if(val != null && val.GetType() != pi.ParameterType)
						val = TypeConverter.Get(pi.ParameterType, val);
					invokeParameters[i] = val;
					normalCount++;
				}
				else
				{
					if(i >= _requiredParameters)
					{
						if(_parameterDefaultValues[i] != null)
						{
							invokeParameters[i] = _parameterDefaultValues[i];
							defaultCount++;
						}
						else if(TypeConverter.IsNullAssignable(pi.ParameterType))
						{
							invokeParameters[i] = null;
							nullCount++;
						}
					}
				}
			}
			bool isValid = _parameterInfos.Length == normalCount + defaultCount + nullCount;
			isValid &= parameters.Count == normalCount;
			if(!isValid)
				return null;
			int matchIndicator = normalCount << 16 - defaultCount << 8 - nullCount;
			return new MethodInvokable(this, matchIndicator, invokeParameters);
		}

		/// <summary>
		/// Invoke the underlying method on the given target object using the supplied parameter values.
		/// Any exception raised by performing the method call is logged and then exposed as-is.
		/// </summary>
		/// <param name="target">The object on which to invoke the method.</param>
		/// <param name="parameterValues">The parameter values used to invoke the method.</param>
		/// <returns>The return value of the invocation.</returns>
		public object Invoke(object target, object[] parameterValues)
		{
			try
			{
				return MethodInfo.Invoke(target, Helper.ReflectionFlags.InstanceCriteria, null, parameterValues, null);
			}
			catch(Exception e)
			{
				throw e;
			}
		}

		/// <summary>
		/// Invoke the underlying method on the given target object using the supplied parameter values.
		/// Any exception raised by performing the method call is logged and then exposed as-is.
		/// </summary>
		/// <param name="target">The object on which to invoke the method.</param>
		/// <param name="parameters">A hashtable of parameter name/value pairs.</param>
		/// <returns>The return value of the invocation.</returns>
		public object Invoke(object target, Hashtable parameters)
		{
			MethodInvokable mi = PrepareInvoke(parameters);
			if(mi.MatchIndicator >= 0)
			{
				return Invoke(target, mi.ParameterValues);
			}
			else
			{
				throw new GDAException("Unable to invoke method using given parameters.");
				return null;
			}
		}

		/// <summary>
		/// Procura o parametro do método pelo nome.
		/// </summary>
		/// <param name="parameterName">Nome do parametro.</param>
		/// <returns>Index do parametro.</returns>
		private int FindParameter(string parameterName)
		{
			if(_parameterInfos == null || _parameterInfos.Length == 0)
				return -1;
			for(int i = 0; i < _parameterInfos.Length; i++)
			{
				if(_parameterInfos[i].Name == parameterName)
					if(_parameterInfos[i].Name == parameterName)
						return i;
			}
			return -1;
		}
	}
}
