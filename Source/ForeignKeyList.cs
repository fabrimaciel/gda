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
	public class ForeignKeyList : IList<ForeignKeyMap>, IEnumerable<ForeignKeyMap>
	{
		/// <summary>
		/// Lista onde serão armazenados os campos.
		/// </summary>
		private List<ForeignKeyMap> _foreignKeys;

		/// <summary>
		/// Dicionário para facilitar a recuperação dos dados.
		/// </summary>
		private HybridDictionary _constraints;

		/// <summary>
		/// Recupera a instancia na posição informada.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public ForeignKeyMap this[int index]
		{
			get
			{
				return _foreignKeys[index];
			}
			set
			{
			}
		}

		/// <summary>
		/// Recupera a instancia com base no nome da constraint informada.
		/// </summary>
		/// <param name="constraintName"></param>
		/// <returns></returns>
		public ForeignKeyMap this[string constraintName]
		{
			get
			{
				if(_constraints == null)
				{
					_constraints = new HybridDictionary(_foreignKeys.Count, true);
					foreach (ForeignKeyMap fk in _foreignKeys)
					{
						_constraints.Add(fk.ConstraintName, fk);
					}
				}
				return _constraints[constraintName] as ForeignKeyMap;
			}
		}

		internal ForeignKeyList() : base()
		{
			_foreignKeys = new List<ForeignKeyMap>();
		}

		/// <summary>
		/// Recupera as chaves estrangeiras com base
		/// no nome da tabela de destino.
		/// </summary>
		/// <param name="name">Nome da tabela.</param>
		/// <returns>Referencias encontradas.</returns>
		public IEnumerable<ForeignKeyMap> FindForeignKeyTable(string name)
		{
			if(name == null)
				throw new NullReferenceException("name");
			foreach (ForeignKeyMap fkm in _foreignKeys)
				if(fkm.ForeignKeyTable == name)
					yield return fkm;
		}

		/// <summary>
		/// Recupera as chaves estrangeiras com base
		/// no nome da tabela de origem.
		/// </summary>
		/// <param name="name">Nome da tabel.</param>
		/// <returns>Referencias encontradas.</returns>
		public IEnumerable<ForeignKeyMap> FindPrimaryKeyTable(string name)
		{
			if(name == null)
				throw new NullReferenceException("name");
			foreach (ForeignKeyMap fkm in _foreignKeys)
				if(fkm.PrimaryKeyColumn == name)
					yield return fkm;
		}

		public bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		public void RemoveAt(int index)
		{
			ForeignKeyMap fk = _foreignKeys[index];
			Remove(fk);
			if(_constraints != null)
				_constraints.Remove(fk.ConstraintName);
		}

		public void Insert(int index, ForeignKeyMap value)
		{
			_foreignKeys.Insert(index, value);
			_constraints = null;
		}

		public bool Remove(ForeignKeyMap value)
		{
			bool r = _foreignKeys.Remove(value);
			if(_constraints != null)
				_constraints.Remove(value.ConstraintName);
			return r;
		}

		public bool Contains(ForeignKeyMap value)
		{
			return _foreignKeys.Contains(value);
		}

		public void Clear()
		{
			_foreignKeys.Clear();
			_constraints = null;
		}

		public int IndexOf(ForeignKeyMap value)
		{
			return _foreignKeys.IndexOf(value);
		}

		public void Add(ForeignKeyMap value)
		{
			Insert(_foreignKeys.Count, value);
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
				return _foreignKeys.Count;
			}
		}

		public void CopyTo(ForeignKeyMap[] array, int index)
		{
			_foreignKeys.CopyTo(array, index);
		}

		public object SyncRoot
		{
			get
			{
				return _foreignKeys;
			}
		}

		public IEnumerator GetEnumerator()
		{
			return _foreignKeys.GetEnumerator();
		}

		IEnumerator<ForeignKeyMap> IEnumerable<ForeignKeyMap>.GetEnumerator()
		{
			return _foreignKeys.GetEnumerator();
		}
	}
}
