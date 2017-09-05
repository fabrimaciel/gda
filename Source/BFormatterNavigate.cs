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
using System.IO;

namespace GDA.Helper.Serialization
{
	/// <summary>
	/// Navegador de registros serializados.
	/// </summary>
	public class BFormatterNavigate
	{
		private Stream navStream;

		private int _count;

		/// <summary>
		/// Tipo base do navegador.
		/// </summary>
		private Type baseType;

		/// <summary>
		/// Tamanho do atual item.
		/// </summary>
		private int sizeCurrentItem = 0;

		private long beginStreamPosition = 0;

		private long currentStreamPosition = 0;

		private int currentPosition = -1;

		private byte[] bufferSize = new byte[sizeof(int)];

		/// <summary>
		/// Informações dos dados de suporte para trabalhar a os dados.
		/// </summary>
		private BFormatter.InfoCoreSupport[] coreSupports;

		/// <summary>
		/// Númer de membros da tipo que permite valores nulos.
		/// </summary>
		private short memberAllowNullCount;

		/// <summary>
		/// Quantidade de itens contidos no navegador.
		/// </summary>
		public int Count
		{
			get
			{
				return _count;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="stream">Stream onde os dados estão armazenados.</param>
		/// <param name="baseType">Tipo base do navegador.</param>
		public BFormatterNavigate(Stream stream, Type baseType)
		{
			beginStreamPosition = stream.Position;
			navStream = stream;
			this.baseType = baseType;
			Initialize();
		}

		/// <summary>
		/// Inicializa a instância.
		/// </summary>
		private void Initialize()
		{
			navStream.Seek(beginStreamPosition, SeekOrigin.Begin);
			currentPosition = -1;
			currentStreamPosition = 0;
			sizeCurrentItem = 0;
			coreSupports = BFormatter.LoadTypeInformation(baseType, out memberAllowNullCount);
			if(navStream.Length > 0)
			{
				_count = BFormatter.ReadArrayLenght(navStream, int.MaxValue);
				currentStreamPosition = navStream.Position;
			}
			else
				_count = 0;
		}

		/// <summary>
		/// Reseta a navegação.
		/// </summary>
		public void Reset()
		{
			Initialize();
		}

		/// <summary>
		/// Recupera o item.
		/// </summary>
		/// <returns></returns>
		public object GetItem()
		{
			if(currentPosition < 0)
				throw new InvalidOperationException("Item not ready.");
			navStream.Seek(currentStreamPosition, SeekOrigin.Begin);
			return BFormatter.DeserializeBase(navStream, baseType, coreSupports, memberAllowNullCount, 0, null);
		}

		/// <summary>
		/// Lê o próximo registro.
		/// </summary>
		/// <returns>True caso o registro tenha sido lido.</returns>
		public bool Read()
		{
			if((currentPosition + 1) >= _count || _count == 0)
				return false;
			navStream.Seek(currentStreamPosition + sizeCurrentItem, SeekOrigin.Begin);
			navStream.Read(bufferSize, 0, bufferSize.Length);
			int size = BitConverter.ToInt32(bufferSize, 0);
			sizeCurrentItem = size;
			currentStreamPosition = navStream.Position;
			currentPosition++;
			return true;
		}
	}
}
