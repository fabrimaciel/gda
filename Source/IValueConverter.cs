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

namespace GDA
{
	/// <summary>
	/// Representa o gerenciador de conversão.
	/// </summary>
	public class ValueConverterManager : IEnumerable<IValueConverter>
	{
		private List<IValueConverter> _converters = new List<IValueConverter>();

		private static ValueConverterManager _instance;

		/// <summary>
		/// Instancia unica do gerenciador.
		/// </summary>
		public static ValueConverterManager Instance
		{
			get
			{
				return _instance;
			}
		}

		/// <summary>
		/// Quantidade de conversores.
		/// </summary>
		public int Count
		{
			get
			{
				return _converters.Count;
			}
		}

		/// <summary>
		/// Acesso o conversão na posição informada.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public IValueConverter this[int index]
		{
			get
			{
				return _converters[index];
			}
			set
			{
				_converters[index] = value;
			}
		}

		/// <summary>
		/// Construtor estático.
		/// </summary>
		static ValueConverterManager()
		{
			_instance = new ValueConverterManager();
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		internal ValueConverterManager()
		{
			_converters.Add(new GDAValueConverter());
		}

		/// <summary>
		/// Realiza a conversão do valor para o tipo alvo.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="targetType"></param>
		/// <param name="cultureInfo"></param>
		/// <returns></returns>
		public object Convert(object value, Type targetType, System.Globalization.CultureInfo culture)
		{
			foreach (var converter in _converters)
			{
				if(converter.CanConvert(value, targetType, culture))
					return converter.Convert(value, targetType, culture);
			}
			return value;
		}

		/// <summary>
		/// Adiciona o conversor para a instancia.
		/// </summary>
		/// <param name="converter"></param>
		public void Add(IValueConverter converter)
		{
			_converters.Add(converter);
		}

		/// <summary>
		/// Remove o conversor informado.
		/// </summary>
		/// <param name="converter"></param>
		/// <returns></returns>
		public bool Remove(IValueConverter converter)
		{
			return _converters.Remove(converter);
		}

		/// <summary>
		/// Recupera o enumerador dos conversores.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<IValueConverter> GetEnumerator()
		{
			return _converters.GetEnumerator();
		}

		/// <summary>
		/// Recupera o enumerador dos conversores.
		/// </summary>
		/// <returns></returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _converters.GetEnumerator();
		}
	}
	/// <summary>
	/// Assinatura da classe usada para converter o valor.
	/// </summary>
	public interface IValueConverter
	{
		/// <summary>
		/// Verifica se pode converter o valor informado.
		/// </summary>
		/// <param name="value">Valor que será convertido.</param>
		/// <param name="targetType">Tipo alvo da conversão.</param>
		/// <param name="culture">Culture que será usada na conversão.</param>
		/// <returns></returns>
		bool CanConvert(object value, Type targetType, System.Globalization.CultureInfo culture);

		/// <summary>
		/// Converte o valor informado
		/// </summary>
		/// <param name="value">Valor que será convertido.</param>
		/// <param name="targetType">Tipo alvo da conversão.</param>
		/// <param name="culture">Cultura que será utilizada na conversão.</param>
		/// <returns></returns>
		object Convert(object value, Type targetType, System.Globalization.CultureInfo culture);
	}
	/// <summary>
	/// Conversor padrão.
	/// </summary>
	public class GDAValueConverter : IValueConverter
	{
		/// <summary>
		/// Verifica se pode converter
		/// </summary>
		/// <param name="value"></param>
		/// <param name="targetType"></param>
		/// <param name="culture"></param>
		/// <returns></returns>
		public bool CanConvert(object value, Type targetType, System.Globalization.CultureInfo culture)
		{
			if(value == null)
				return false;
			var sourceType = value.GetType();
			if(GDA.Helper.TypeHelper.IsNullableType(targetType))
			{
				if(value == null)
					return false;
				targetType = Nullable.GetUnderlyingType(targetType);
			}
			if(targetType.IsEnum)
			{
				if(value is string || targetType == typeof(long) || value is uint || value is ushort || value is ulong || value is short || value is int || value is long || value is byte)
					return true;
			}
			else if(targetType == typeof(bool))
				return true;
			else if(targetType == typeof(uint) && sourceType == typeof(int))
				return true;
			else if(targetType == typeof(ushort) && sourceType == typeof(short))
				return true;
			else if(targetType == typeof(DateTimeOffset))
			{
				if(sourceType == typeof(DateTime))
					return true;
			}
			else if(targetType == typeof(Guid))
			{
				if(sourceType == typeof(string) || sourceType != typeof(Guid))
					return true;
			}
			else if(targetType != sourceType)
			{
				if(targetType == typeof(int))
					return true;
				else if(targetType == typeof(uint))
					return true;
				else if(sourceType == typeof(decimal))
				{
					if(targetType == typeof(float))
						return true;
					else if(targetType == typeof(double))
						return true;
					else if(targetType == typeof(int))
						return true;
					else if(targetType == typeof(short))
						return true;
					else if(targetType == typeof(long))
						return true;
				}
				else if(targetType == typeof(decimal))
				{
					if(sourceType == typeof(float))
						return true;
					else if(sourceType == typeof(double))
						return true;
					else if(sourceType == typeof(int))
						return true;
					else if(sourceType == typeof(short))
						return true;
					else if(sourceType == typeof(long))
						return true;
				}
				else if(sourceType == typeof(byte[]))
				{
					switch(targetType.Name)
					{
					case "Int16":
					case "Int32":
					case "Int64":
					case "UInt16":
					case "UInt32":
					case "UInt64":
					case "Single":
					case "String":
					case "Double":
					case "Boolean":
					case "Char":
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Converte o valor.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="targetType"></param>
		/// <param name="culture"></param>
		/// <returns></returns>
		public object Convert(object value, Type targetType, System.Globalization.CultureInfo culture)
		{
			if(value == null)
				return null;
			var sourceType = value.GetType();
			bool isNullable = false;
			if(GDA.Helper.TypeHelper.IsNullableType(targetType))
			{
				if(value == null)
					return null;
				isNullable = true;
				targetType = Nullable.GetUnderlyingType(targetType);
			}
			if(targetType.IsEnum)
			{
				var underlyngType = Enum.GetUnderlyingType(targetType);
				var enumValue = value;
				if(value is string)
				{
					enumValue = Enum.Parse(targetType, (string)value, true);
				}
				else if(sourceType == typeof(long))
					enumValue = (int)(long)value;
				else if(sourceType == typeof(uint))
				{
					if(underlyngType == typeof(int))
						enumValue = (int)(uint)value;
					else if(underlyngType == typeof(short))
						enumValue = (short)(uint)value;
				}
				else if(sourceType == typeof(ushort))
				{
					if(underlyngType == typeof(int))
						enumValue = (int)(ushort)value;
					else if(underlyngType == typeof(short))
						enumValue = (short)(ushort)value;
				}
				if(isNullable)
					try
					{
						return Activator.CreateInstance(typeof(Nullable<>).MakeGenericType(targetType), Enum.ToObject(targetType, enumValue));
					}
					catch(System.Reflection.TargetInvocationException ex)
					{
						throw ex.InnerException;
					}
				else
					return enumValue;
			}
			else if(targetType == typeof(bool))
				return System.Convert.ToBoolean(value);
			else if(targetType == typeof(uint) && sourceType == typeof(int))
				return uint.Parse(value.ToString());
			else if(targetType == typeof(ushort) && sourceType == typeof(short))
				return ushort.Parse(value.ToString());
			else if(targetType == typeof(DateTimeOffset))
			{
				if(sourceType == typeof(DateTime))
					return new DateTimeOffset((DateTime)value);
			}
			else if(targetType == typeof(Guid))
			{
				if(sourceType == typeof(string))
					return new Guid((string)value);
				else if(sourceType != typeof(Guid))
					return new Guid(value.ToString());
			}
			else if(targetType != sourceType)
			{
				if(targetType == typeof(int))
					return System.Convert.ToInt32(value);
				else if(targetType == typeof(uint))
					return System.Convert.ToUInt32(value);
				else if(sourceType == typeof(decimal))
				{
					if(targetType == typeof(float))
						return decimal.ToSingle((decimal)value);
					else if(targetType == typeof(double))
						return decimal.ToDouble((decimal)value);
					else if(targetType == typeof(int))
						return decimal.ToInt32((decimal)value);
					else if(targetType == typeof(short))
						return decimal.ToInt16((decimal)value);
					else if(targetType == typeof(long))
						return decimal.ToInt64((decimal)value);
				}
				else if(targetType == typeof(decimal))
				{
					if(sourceType == typeof(float))
						return (decimal)(float)value;
					else if(sourceType == typeof(double))
						return (decimal)(double)value;
					else if(sourceType == typeof(int))
						return (decimal)(int)value;
					else if(sourceType == typeof(short))
						return (decimal)(short)value;
					else if(sourceType == typeof(long))
						return (decimal)(long)value;
				}
				else if(targetType == typeof(bool))
				{
					if(value is int)
						return ((int)value) != 0;
					if(value is short)
						return ((short)value) != 0;
					if(value is long)
						return ((long)value) != 0;
				}
				else if(value is uint)
				{
					if(targetType == typeof(int))
						return (int)(uint)value;
					else if(targetType == typeof(short))
						return (short)(uint)value;
					else if(targetType == typeof(long))
						return (long)(uint)value;
				}
				else if(value is ushort)
				{
					if(targetType == typeof(int))
						return (int)(ushort)value;
					else if(targetType == typeof(short))
						return (short)(ushort)value;
					else if(targetType == typeof(long))
						return (long)(ushort)value;
				}
				else if(value is ulong)
				{
					if(targetType == typeof(int))
						return (int)(ulong)value;
					else if(targetType == typeof(short))
						return (short)(ulong)value;
					else if(targetType == typeof(long))
						return (long)(ulong)value;
				}
				else if(sourceType == typeof(byte[]))
				{
					switch(targetType.Name)
					{
					case "Int16":
						return BitConverter.ToInt16((byte[])value, 0);
					case "Int32":
						return BitConverter.ToInt32((byte[])value, 0);
					case "Int64":
						return BitConverter.ToInt64((byte[])value, 0);
					case "UInt16":
						return BitConverter.ToUInt16((byte[])value, 0);
					case "UInt32":
						return BitConverter.ToUInt32((byte[])value, 0);
					case "UInt64":
						return BitConverter.ToUInt64((byte[])value, 0);
					case "Single":
						return BitConverter.ToSingle((byte[])value, 0);
					case "String":
						return BitConverter.ToString((byte[])value, 0);
					case "Double":
						return BitConverter.ToDouble((byte[])value, 0);
					case "Boolean":
						return BitConverter.ToBoolean((byte[])value, 0);
					case "Char":
						return BitConverter.ToChar((byte[])value, 0);
					}
				}
			}
			return value;
		}
	}
}
