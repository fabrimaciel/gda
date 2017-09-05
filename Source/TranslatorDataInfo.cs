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

namespace GDA
{
	/// <summary>
	/// Representa uma coleção de dados traduzidos
	/// </summary>
	public class TranslatorDataInfoCollection : List<TranslatorDataInfo>, IDictionary<string, int>
	{
		private Dictionary<string, int> _translator;

		/// <summary>
		/// Recupera o tradudor de campos relacionado.
		/// </summary>
		public Dictionary<string, int> Translator
		{
			get
			{
				if(_translator == null)
					throw new GDAException("Translator fields not processed.");
				return _translator;
			}
		}

		public TranslatorDataInfoCollection() : base()
		{
		}

		public TranslatorDataInfoCollection(IEnumerable<TranslatorDataInfo> collection) : base(collection)
		{
		}

		public TranslatorDataInfoCollection(IEnumerable<Mapper> collection) : base()
		{
			if(collection != null)
				foreach (var i in collection)
					if(!string.IsNullOrEmpty(i.Name))
						this.Add(new TranslatorDataInfo(i.Name, i.PropertyMapper));
		}

		/// <summary>
		/// Process as posições dos campos.
		/// </summary>
		/// <param name="dataRecord"></param>
		public void ProcessFieldsPositions(System.Data.IDataRecord dataRecord)
		{
			if(dataRecord == null)
				throw new ArgumentNullException("dataRecord", "Datarecord couldn't be null");
			lock (this)
			{
				List<string> fieldsNames = new List<string>();
				for(int i = 0; i < dataRecord.FieldCount; i++)
				{
					var fieldName = dataRecord.GetName(i);
					if(fieldName != null)
						fieldsNames.Add(fieldName.ToLower());
				}
				var result = new Dictionary<string, int>();
				foreach (TranslatorDataInfo i in this)
				{
					var index = fieldsNames.FindIndex(f => string.Compare(i.FieldName, f, true, System.Globalization.CultureInfo.InvariantCulture) == 0);
					if(index >= 0)
					{
						try
						{
							result.Add(i.PathAddress, index);
						}
						catch(System.ArgumentException ex)
						{
							throw new GDAException(string.Format("PathAddress '{0}' with field name '{1}' duplicated.", i.PathAddress, i.FieldName), ex);
						}
					}
					i.FieldPosition = index;
				}
				_translator = result;
			}
		}

		void IDictionary<string, int>.Add(string key, int value)
		{
			throw new NotSupportedException();
		}

		bool IDictionary<string, int>.ContainsKey(string key)
		{
			return Translator.ContainsKey(key);
		}

		ICollection<string> IDictionary<string, int>.Keys
		{
			get
			{
				return Translator.Keys;
			}
		}

		bool IDictionary<string, int>.Remove(string key)
		{
			throw new NotSupportedException();
		}

		bool IDictionary<string, int>.TryGetValue(string key, out int value)
		{
			return Translator.TryGetValue(key, out value);
		}

		ICollection<int> IDictionary<string, int>.Values
		{
			get
			{
				return Translator.Values;
			}
		}

		int IDictionary<string, int>.this[string key]
		{
			get
			{
				return Translator[key];
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		void ICollection<KeyValuePair<string, int>>.Add(KeyValuePair<string, int> item)
		{
			throw new NotSupportedException();
		}

		void ICollection<KeyValuePair<string, int>>.Clear()
		{
			throw new NotSupportedException();
		}

		bool ICollection<KeyValuePair<string, int>>.Contains(KeyValuePair<string, int> item)
		{
			return ((ICollection<KeyValuePair<string, int>>)Translator).Contains(item);
		}

		void ICollection<KeyValuePair<string, int>>.CopyTo(KeyValuePair<string, int>[] array, int arrayIndex)
		{
			((ICollection<KeyValuePair<string, int>>)Translator).CopyTo(array, arrayIndex);
		}

		int ICollection<KeyValuePair<string, int>>.Count
		{
			get
			{
				return ((ICollection<KeyValuePair<string, int>>)Translator).Count;
			}
		}

		bool ICollection<KeyValuePair<string, int>>.IsReadOnly
		{
			get
			{
				return true;
			}
		}

		bool ICollection<KeyValuePair<string, int>>.Remove(KeyValuePair<string, int> item)
		{
			throw new NotSupportedException();
		}

		IEnumerator<KeyValuePair<string, int>> IEnumerable<KeyValuePair<string, int>>.GetEnumerator()
		{
			return Translator.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return Translator.GetEnumerator();
		}
	}
	/// <summary>
	/// Classe responsável por traduzir os nomes dos dados do registro
	/// do resultado de uma consulta para a propriedade de uma classe.
	/// </summary>
	public class TranslatorDataInfo
	{
		private PropertyInfo[] _path;

		private string _pathAddress;

		/// <summary>
		/// Caminho para chegar até a propriedade final.
		/// </summary>
		public IEnumerable<PropertyInfo> Path
		{
			get
			{
				return _path;
			}
		}

		/// <summary>
		/// Comprimento do caminho.
		/// </summary>
		public int PathLength
		{
			get
			{
				return _path.Length;
			}
		}

		/// <summary>
		/// Endereço do caminho.
		/// </summary>
		public string PathAddress
		{
			get
			{
				return _pathAddress;
			}
		}

		/// <summary>
		/// Propriedade de onde o valor será recuperado.
		/// </summary>
		public PropertyInfo Property
		{
			get
			{
				return _path[_path.Length - 1];
			}
		}

		/// <summary>
		/// Nome do campo no resultado.
		/// </summary>
		public string FieldName
		{
			get;
			set;
		}

		/// <summary>
		/// Posição do campo no resultado.
		/// </summary>
		public int FieldPosition
		{
			get;
			set;
		}

		/// <summary>
		/// Define o valor da propriedade na instancia.
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="value"></param>
		public void SetValue(object instance, object value)
		{
			var i = 0;
			PropertyInfo lastProperty = null;
			foreach (var pi in Path)
			{
				i++;
				if(i == PathLength)
				{
					value = DataAccess.ConvertValue(value, pi.PropertyType);
					pi.SetValue(instance, value, null);
					break;
				}
				if(instance == null && lastProperty != null)
				{
					var nInstance = Activator.CreateInstance(pi.DeclaringType);
					lastProperty.SetValue(instance, nInstance, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, null, System.Globalization.CultureInfo.CurrentCulture);
					instance = nInstance;
				}
				lastProperty = pi;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="fieldName">Nome do campo no banco de dados.</param>
		/// <param name="path">Caminho para chegar até a proprieade final.</param>
		public TranslatorDataInfo(string fieldName, params PropertyInfo[] path)
		{
			if(fieldName == null)
				throw new ArgumentNullException("fieldName");
			FieldName = fieldName;
			_path = path;
			if(path != null)
			{
				var address = new List<string>();
				foreach (var i in path)
					address.Add(i.Name);
				_pathAddress = string.Join(".", address.ToArray());
			}
		}
	}
}
