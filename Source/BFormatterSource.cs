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
	/// Representa uma fonte de dados.
	/// </summary>
	public class BFormatterSource : IDisposable
	{
		/// <summary>
		/// Stream onde os dados serão salvos.
		/// </summary>
		private Stream source;

		private int _count = 0;

		private long sourceBeginPosition = 0;

		private Type baseType = null;

		/// <summary>
		/// Identifica se a stream foi criada na atual instância.
		/// </summary>
		private bool localStream = false;

		/// <summary>
		/// Informações dos dados de suporte para trabalhar a os dados.
		/// </summary>
		private BFormatter.InfoCoreSupport[] coreSupports;

		/// <summary>
		/// Númer de membros da tipo que permite valores nulos.
		/// </summary>
		private short memberAllowNullCount;

		/// <summary>
		/// Quantidade de itens contidos na fonte.
		/// </summary>
		public int Count
		{
			get
			{
				return _count;
			}
		}

		/// <summary>
		/// Instancia uma fonte que será salva na stream informada.
		/// </summary>
		/// <param name="stream">Stream onde os dados serão salvos.</param>
		/// <param name="baseType">Tipo base trabalhado pela fonte.</param>
		public BFormatterSource(Stream stream, Type baseType)
		{
			source = stream;
			localStream = false;
			this.baseType = baseType;
			Initialize();
		}

		/// <summary>
		/// Instancia uma fonte que será salva na stream informada.
		/// </summary>
		/// <param name="stream">Stream onde os dados serão salvos.</param>
		/// <param name="baseType">Tipo base trabalhado pela fonte.</param>
		/// <param name="append">Identifica que a fonte será uma continuação.</param>
		public BFormatterSource(Stream stream, Type baseType, bool append)
		{
			var buffer = new byte[sizeof(int)];
			if(stream.Read(buffer, 0, buffer.Length) == buffer.Length)
				_count = BitConverter.ToInt32(buffer, 0);
			stream.Seek(0, SeekOrigin.End);
			source = stream;
			localStream = false;
			this.baseType = baseType;
			Initialize();
		}

		private void Initialize()
		{
			coreSupports = BFormatter.LoadTypeInformation(baseType, out memberAllowNullCount);
			sourceBeginPosition = source.Position;
			source.Write(BitConverter.GetBytes(_count), 0, sizeof(int));
		}

		/// <summary>
		/// Força o salvamento da informação na stream.
		/// </summary>
		public void Flush()
		{
			long pos = source.Position;
			source.Seek(sourceBeginPosition, SeekOrigin.Begin);
			source.Write(BitConverter.GetBytes((int)_count), 0, sizeof(int));
			source.Seek(pos, SeekOrigin.Begin);
		}

		/// <summary>
		/// Adiciona um item na fonte.
		/// </summary>
		/// <param name="item">Item contendo os dados.</param>
		public void Add(object item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			if(item.GetType() != baseType)
				throw new InvalidOperationException("Invalid item type.");
			long pos = source.Position;
			source.Write(new byte[] {
				0,
				0,
				0,
				0
			}, 0, sizeof(int));
			BFormatter.SerializeBase(source, coreSupports, memberAllowNullCount, 0, item);
			int size = (int)(source.Position - sizeof(int) - pos);
			long endPos = source.Position;
			source.Seek(pos, SeekOrigin.Begin);
			source.Write(BitConverter.GetBytes(size), 0, sizeof(int));
			source.Seek(endPos, SeekOrigin.Begin);
			_count++;
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			Flush();
			if(localStream)
				source.Close();
		}
	}
}
