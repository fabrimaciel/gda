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

namespace GDA
{
	/// <summary>
	/// Valida se o da propriedade está entre o intervalo de valor especificado.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
	public class RangeValidatorAttribute : ValidatorAttribute
	{
		private object min;

		private object max;

		/// <summary>
		/// Valor minimo permitido (inclusive). 
		/// </summary>
		public object Min
		{
			get
			{
				return min;
			}
			set
			{
				min = value;
			}
		}

		/// <summary>
		/// Valor máximo permitido (inclusive).
		/// </summary>
		public object Max
		{
			get
			{
				return max;
			}
			set
			{
				max = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public RangeValidatorAttribute() : this(null, null)
		{
		}

		/// <summary>
		/// Construtor que permite especificar o valor minimo e máximo.
		/// </summary>
		/// <param name="max">Valor máximo do intervalo. Null para o maior valor possível.</param>
		/// <param name="min">Valor minimo do intervalo.</param>
		/// <param name="message">Mensagem de validação.</param>
		public RangeValidatorAttribute(object min, object max, string message)
		{
			MessageText = message;
			this.min = min;
			this.max = max;
		}

		/// <summary>
		/// Construtor que permite especificar o valor minimo e máximo.
		/// </summary>
		/// <param name="max">Valor máximo do intervalo. Null para o maior valor possível.</param>
		/// <param name="min">Valor minimo do intervalo.</param>
		public RangeValidatorAttribute(object min, object max)
		{
			this.min = min;
			this.max = max;
		}

		/// <summary>
		/// Esse método deve ser sobreescrito para tratar a valicação da propridade.
		/// </summary>
		/// <param name="propertyName">Nome da propriedade.</param>
		/// <param name="propertyValue">Valor da propriedade.</param>
		/// <param name="parent">Objeto persistente onde a propriedade está inserida.</param>
		/// <returns>O método retorna true se a validação for bem sucedida.</returns>
		public override bool Validate(GDASession session, ValidationMode mode, string propertyName, object propertyValue, object parent)
		{
			if(mode == ValidationMode.Delete)
				return true;
			if(propertyValue == null)
			{
				return false;
			}
			else
			{
				Type type = propertyValue.GetType();
				if(type == typeof(short) || type == typeof(int) || type == typeof(long))
				{
					return ValidateRangeLong(Convert.ToInt64(propertyValue));
				}
				else if(type == typeof(float) || type == typeof(double))
				{
					return ValidateRangeDouble(Convert.ToDouble(propertyValue));
				}
				else if(type == typeof(DateTime))
				{
					return ValidateRangeDateTime((DateTime)propertyValue);
				}
				return false;
			}
		}

		private bool ValidateRangeLong(long number)
		{
			long minValue = min == null ? long.MinValue : Convert.ToInt64(min);
			long maxValue = max == null ? long.MaxValue : Convert.ToInt64(max);
			return number >= minValue && number <= maxValue;
		}

		private bool ValidateRangeDouble(double number)
		{
			double minValue = min == null ? double.MinValue : Convert.ToDouble(min);
			double maxValue = max == null ? double.MaxValue : Convert.ToDouble(max);
			return number >= minValue && number <= maxValue;
		}

		private bool ValidateRangeDateTime(DateTime date)
		{
			DateTime minValue = min == null ? DateTime.MinValue : GetDateTime(min);
			DateTime maxValue = max == null ? DateTime.MaxValue : GetDateTime(max);
			return date >= minValue && date <= maxValue;
		}

		private DateTime GetDateTime(object value)
		{
			if(value.GetType() == typeof(DateTime))
			{
				return (DateTime)value;
			}
			try
			{
				string defaults = "0001-01-01 00:00:00.000";
				string date = (string)value;
				date += defaults.Substring(date.Length, defaults.Length - date.Length);
				return DateTime.Parse(date, System.Globalization.DateTimeFormatInfo.InvariantInfo);
			}
			catch(Exception e)
			{
				throw;
			}
		}
	}
}
