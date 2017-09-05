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
	/// 
	/// </summary>
	public class ValidationMapper
	{
		/// <summary>
		/// Mapeamento pai.
		/// </summary>
		private Mapper _mapperOwner;

		/// <summary>
		/// Validadores.
		/// </summary>
		private ValidatorAttribute[] _validators;

		/// <summary>
		/// Mapeamento pai.
		/// </summary>
		public Mapper MapperOwner
		{
			get
			{
				return _mapperOwner;
			}
		}

		/// <summary>
		/// Validadores.
		/// </summary>
		public ValidatorAttribute[] Validators
		{
			get
			{
				return _validators;
			}
		}

		/// <summary>
		/// Instancia o validador do mapeamento.
		/// </summary>
		/// <param name="mapperOwner">Mapeamento pai.</param>
		/// <param name="validators">Validadores do membro.</param>
		public ValidationMapper(Mapper mapperOwner, ValidatorAttribute[] validators)
		{
			if(mapperOwner == null)
				throw new ArgumentNullException("mapperOwner");
			else if(validators == null)
				throw new ArgumentNullException("validators");
			_mapperOwner = mapperOwner;
			_validators = validators;
		}
	}
}
