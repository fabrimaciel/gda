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

namespace GDA.Sql.InterpreterExpression
{
	static class ArrayHelper
	{
		/// <summary>
		/// Verifica se o elemento tratado pelo Predicate está contido no Array.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="array"></param>
		/// <param name="match"></param>
		/// <returns></returns>
		public static bool Exists<T>(T[] array, Predicate<T> match)
		{
			return (FindIndex<T>(array, match) != -1);
		}

		/// <summary>
		/// Localiza o indice do elemento procurado no Array passado.
		/// </summary>
		/// <typeparam name="T">Tipo do elemento de Array que serà localizado.</typeparam>
		/// <param name="array">Array onde o elemento será procurado.</param>
		/// <param name="match">Predicate que será acionado para verificar o elemento.</param>
		/// <returns>-1 ou a posição do indice encontrado.</returns>
		public static int FindIndex<T>(T[] array, Predicate<T> match)
		{
			if(array == null)
				throw new ArgumentNullException("array");
			return FindIndex<T>(array, 0, array.Length, match);
		}

		/// <summary>
		/// Localiza o indice do elemento procurado no Array passado.
		/// </summary>
		/// <typeparam name="T">Tipo do elemento de Array que serà localizado.</typeparam>
		/// <param name="array">Array onde o elemento será procurado.</param>
		/// <param name="startIndex">Posição inicial da pesquisa.</param>
		/// <param name="match">Predicate que será acionado para verificar o elemento.</param>
		/// <returns>-1 ou a posição do indice encontrado.</returns>
		public static int FindIndex<T>(T[] array, int startIndex, Predicate<T> match)
		{
			if(array == null)
				throw new ArgumentNullException("array");
			return FindIndex<T>(array, startIndex, array.Length - startIndex, match);
		}

		/// <summary>
		/// Localiza o indice do elemento procurado no Array passado.
		/// </summary>
		/// <typeparam name="T">Tipo do elemento de Array que serà localizado.</typeparam>
		/// <param name="array">Array onde o elemento será procurado.</param>
		/// <param name="startIndex">Posição inicial da pesquisa.</param>
		/// <param name="count">Quantidade de itens que deveram ser procurados.</param>
		/// <param name="match">Predicate que será acionado para verificar o elemento.</param>
		/// <returns>-1 ou a posição do indice encontrado.</returns>
		public static int FindIndex<T>(T[] array, int startIndex, int count, Predicate<T> match)
		{
			if(array == null)
				throw new ArgumentNullException("array");
			if((startIndex < 0) || (startIndex > array.Length))
				throw new ArgumentOutOfRangeException("startIndex", "ArgumentOutOfRange Index");
			if((count < 0) || (startIndex > (array.Length - count)))
				throw new ArgumentOutOfRangeException("count", "ArgumentOutOfRange Count");
			if(match == null)
				throw new ArgumentNullException("match");
			int num = startIndex + count;
			for(int i = startIndex; i < num; i++)
			{
				if(match(array[i]))
				{
					return i;
				}
			}
			return -1;
		}
	}
}
