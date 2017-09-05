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
using System.Collections;
using System.Collections.Specialized;

namespace GDA.Analysis
{
	/// <summary>
	/// Representa a alista para armazenar vários <see cref="FieldMap"/>.
	/// Ela contém métodos para manipular e recupera as colunas contidas.
	/// </summary>
	public sealed class FieldList : IList, IEnumerable
	{
		/// <summary>
		/// Lista onde serão armazenados os campos.
		/// </summary>
		private ArrayList fields;

		/// <summary>
		/// Dicionário para facilitar a recuperação dos dados das colunas.
		/// </summary>
		private HybridDictionary columns;

		internal FieldList() : base()
		{
			fields = new ArrayList();
		}

		/// <summary>
		/// Recupera as informações da coluna com base no nome informado.
		/// </summary>
		/// <param name="name">Nome da coluna.</param>
		/// <returns>FieldMap correspondente.</returns>
		public FieldMap FindColumn(string name)
		{
			if(name == null)
				throw new NullReferenceException("name");
			if(columns == null)
			{
				columns = new HybridDictionary(fields.Count, true);
				foreach (FieldMap fm in fields)
				{
					columns.Add(fm.ColumnName, fm);
				}
			}
			return columns[name] as FieldMap;
		}

		/// <summary>
		/// Recupera os dados da coluna com base no seu identificador.
		/// </summary>
		/// <param name="columnId"></param>
		/// <returns></returns>
		public FieldMap FindColumnById(int columnId)
		{
			foreach (FieldMap fm in this)
			{
				if(columnId != -1 && columnId == fm.ColumnId)
					return fm;
			}
			return null;
		}

		/// <summary>
		/// Recupera o número de campos chave primária da lista.
		/// </summary>
		public int PrimaryKeyCount
		{
			get
			{
				int count = 0;
				foreach (FieldMap fm in this)
				{
					if(fm.IsPrimaryKey)
						count++;
				}
				return count;
			}
		}

		public FieldMap this[int index]
		{
			get
			{
				return fields[index] as FieldMap;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		object IList.this[int index]
		{
			get
			{
				return fields[index];
			}
			set
			{
				fields[index] = value;
			}
		}

		public void RemoveAt(int index)
		{
			FieldMap fm = fields[index] as FieldMap;
			Remove(fm);
			if(columns != null)
				columns.Remove(fm.ColumnName);
		}

		public void Insert(int index, object value)
		{
			FieldMap fm = value as FieldMap;
			fields.Insert(index, fm);
			columns = null;
		}

		public void Remove(object value)
		{
			FieldMap fm = value as FieldMap;
			fields.Remove(fm);
			if(columns != null)
				columns.Remove(fm.ColumnName);
		}

		public bool Contains(object value)
		{
			return fields.Contains(value);
		}

		public void Clear()
		{
			fields.Clear();
			columns = null;
		}

		public int IndexOf(object value)
		{
			return fields.IndexOf(value);
		}

		public int Add(object value)
		{
			Insert(fields.Count, value);
			return fields.Count;
		}

		public bool IsFixedSize
		{
			get
			{
				return false;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public int Count
		{
			get
			{
				return fields.Count;
			}
		}

		public void CopyTo(Array array, int index)
		{
			fields.CopyTo(array, index);
		}

		public object SyncRoot
		{
			get
			{
				return fields.SyncRoot;
			}
		}

		public IEnumerator GetEnumerator()
		{
			return fields.GetEnumerator();
		}
	}
}
