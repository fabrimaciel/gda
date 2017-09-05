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
	/// Possíveis modos onde é feita a validação.
	/// </summary>
	public enum ValidationMode
	{
		/// <summary>
		/// Quando o registro é para ser inserido.
		/// </summary>
		Insert,
		/// <summary>
		/// Quando o registro é para ser atualizado
		/// </summary>
		Update,
		/// <summary>
		/// Quando o registro é para ser apagado.
		/// </summary>
		Delete
	}
	/// <summary>
	/// Classe singleton usada para executar a perfomace de validação do objeto.
	/// </summary>
	public sealed class ModelValidator
	{
		/// <summary>
		/// Interface estática para validação em um objeto.
		/// </summary>
		/// <param name="obj">Objeto onde será realizada a operação de validação.</param>
		/// <returns>Mensagens da validação.</returns>
		public static ValidationResult Validate(ValidationMode mode, object obj)
		{
			return Validate(null, mode, obj);
		}

		/// <summary>
		/// Interface estática para validação em um objeto.
		/// </summary>
		/// <param name="session">Sessão de conexão do GDA que será usada na validação.</param>
		/// <param name="obj">Objeto onde será realizada a operação de validação.</param>
		/// <returns>Mensagens da validação.</returns>
		public static ValidationResult Validate(GDASession session, ValidationMode mode, object obj)
		{
			if(obj == null)
				throw new ArgumentNullException("obj");
			List<ValidationMessage> messages = new List<ValidationMessage>();
			IList<Mapper> mappers = Caching.MappingManager.GetMappers(obj.GetType());
			foreach (Mapper m in mappers)
			{
				object propValue = m.PropertyMapper.GetValue(obj, null);
				if(m.Validation != null)
					foreach (ValidatorAttribute va in m.Validation.Validators)
					{
						bool currentValid = va.Validate(session, mode, m.PropertyMapperName, propValue, obj);
						if(!currentValid)
						{
							if(va.Message == null)
							{
								ValidationMessage vm = new ValidationMessage();
								vm.Message = va.GetType().Name.Replace("Attribute", "");
								vm.PropertyName = m.PropertyMapperName;
								messages.Add(vm);
							}
							else
								messages.Add(va.Message.Clone(m.PropertyMapperName));
						}
					}
			}
			return new ValidationResult(messages, messages.Count == 0);
		}
	}
}
