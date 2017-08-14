using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
namespace GDA.Collections
{
    /// <summary>
    /// Delegate usado para converter o tipo de saída.
    /// </summary>
    /// <typeparam name="TOutput"></typeparam>
    /// <param name="input"></param>
    /// <returns></returns>
    public delegate TOutput ConverterOutput<TOutput>(object input);
    /// <summary>
    /// Delegate usado para converter o tipo de saída para o tipo de entrada.
    /// </summary>
    /// <typeparam name="TOutput"></typeparam>
    /// <param name="output"></param>
    /// <returns></returns>
    public delegate object ConverterInput<TOutput>(TOutput output);
    /// <summary>
    /// Representa uma lista que é uma proxy para uma lista interna.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ProxyIList<T> : IEnumerable<T>, IEnumerable, IList<T>, IList, ICollection<T>, ICollection
    {
        #region Local Variables
        /// <summary>
        /// Instancia da lista cliente.
        /// </summary>
        private IList _client;
        /// <summary>
        /// Instancia do delegate usado para converter o item de entrada para o item de saída.
        /// </summary>
        private ConverterOutput<T> _converterOutput;
        /// <summary>
        /// Instancia do delegate usado para converter o item de saída para o item de entrada.
        /// </summary>
        private ConverterInput<T> _converterInput;
        /// <summary>
        /// Comparador usados na instancia.
        /// </summary>
        private Comparer _comparer;
        #endregion
        #region Properties
        /// <summary>
        /// Instancia do delegate usado para converter o item de entrada para o item de saída.
        /// </summary>
        public ConverterOutput<T> ConverterOutput
        {
            get { return _converterOutput; }
            set { _converterOutput = value; }
        }
        /// <summary>
        /// Instancia do delegate usado para converter o item de saída para o item de entrada.
        /// </summary>
        public ConverterInput<T> ConverterInput
        {
            get { return _converterInput; }
            set { _converterInput = value; }
        }
        /// <summary>
        /// Instancia do cliente do proxy
        /// </summary>
        public virtual IList Client
        {
            get { return _client; }
        }
        #endregion
        #region Constructors
        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="client">Instancia do cliente do proxy.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public ProxyIList(IList client)
        {            
            if (client == null)
                throw new ArgumentNullException("client");
            _client = client;
        }
        /// <summary>
        /// Constrói uma instancia já tendo o método de conversão do item de saída.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="converterOutput"></param>
        public ProxyIList(IList client, ConverterOutput<T> converterOutput)
            : this(client)
        {
            _converterOutput = converterOutput;
        }
        /// <summary>
        /// Constrói uma instancia já tendo os métodos de conversão.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="converterOutput"></param>
        /// <param name="converterInput"></param>
        /// <param name="comparer">Comparador que será usado na instancia.</param>
        public ProxyIList(IList client, ConverterOutput<T> converterOutput, ConverterInput<T> converterInput, Comparer comparer)
            : this(client)
        {
            _converterOutput = converterOutput;
            _converterInput = converterInput;
        }
        #endregion
        #region Membros de IList<T>
        public int IndexOf(T item)
        {
            if (_comparer != null)
            {
                for (int i = 0; i < Client.Count; i++)
                    if (_comparer.Compare(Client[i], item) == 0)
                        return i;
                return -1;
            }
            return Client.IndexOf(item);
        }
        public void Insert(int index, T item)
        {
            if (_converterInput != null)
                Client.Insert(index, _converterInput(item));
            else
                Client.Insert(index, item);
        }
        public void RemoveAt(int index)
        {
            Client.RemoveAt(index);
        }
        public T this[int index]
        {
            get
            {
                if (_converterOutput != null)
                    return _converterOutput(Client[index]);
                return (T)Client[index];
            }
            set
            {
                if (_converterInput != null)
                    Client[index] = _converterInput(value);
                else
                    Client[index] = value;
            }
        }
        #endregion
        #region Membros de ICollection<T>
        public void Add(T item)
        {
            if (_converterInput != null)
                Client.Add(_converterInput(item));
            else
                Client.Add(item);
        }
        public void Clear()
        {
            Client.Clear();
        }
        public bool Contains(T item)
        {
            if (_comparer != null)
            {
                for (int i = 0; i < Client.Count; i++)
                    if (_comparer.Compare(Client[i], item) == 0)
                        return true;
                return false;
            }
            return Client.Contains(item);
        }
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (_converterOutput != null)
            {
                if (array == null)
                    throw new ArgumentNullException("array");
                for (var i = 0; i < array.Length - arrayIndex && i < this.Count; i++)
                    array[arrayIndex + i] = _converterOutput(Client[i]);
            }
            else
                Client.CopyTo(array, arrayIndex);
        }
        public int Count
        {
            get { return Client.Count; }
        }
        public bool IsReadOnly
        {
            get { return Client.IsReadOnly; }
        }
        public bool Remove(T item)
        {
            var comparer = _comparer == null ? Comparer.Default : _comparer;
            for (int i = 0; i < Client.Count; i++)
                if (_comparer.Compare(Client[i], item) == 0)
                {
                    Client.RemoveAt(i);
                    return true;
                }
            return false;
        }
        #endregion
        #region Membros de IEnumerable<T>
        public IEnumerator<T> GetEnumerator()
        {
            if (_converterOutput != null)
                foreach (object i in Client)
                    yield return _converterOutput(i);
            else
                foreach (T i in Client)
                    yield return i;
        }
        #endregion
        #region Membros de IEnumerable
        IEnumerator IEnumerable.GetEnumerator()
        {
            if (_converterOutput != null)
                foreach (object i in Client)
                    yield return _converterOutput(i);
            else
                foreach (T i in Client)
                    yield return i;
        }
        #endregion
        #region Membros de IList
        public int Add(object value)
        {
            if (value is T && _converterInput != null)
                return Client.Add(_converterInput((T)value));
            else
                return Client.Add(value);
        }
        public bool Contains(object value)
        {
            if (_comparer != null)
            {
                for (int i = 0; i < Client.Count; i++)
                    if (_comparer.Compare(Client[i], value) == 0)
                        return true;
                return false;
            }
            return Client.Contains(value);
        }
        public int IndexOf(object value)
        {
            if (_comparer != null)
            {
                for (int i = 0; i < Client.Count; i++)
                    if (_comparer.Compare(Client[i], value) == 0)
                        return i;
                return -1;
            }
            return Client.IndexOf(value);
        }
        public void Insert(int index, object value)
        {
            if (value is T && _converterInput != null)
                Client.Insert(index, _converterInput((T)value));
            else
                Client.Insert(index, value);
        }
        public bool IsFixedSize
        {
            get { return Client.IsFixedSize; }
        }
        public void Remove(object value)
        {
            if (_comparer != null)
            {
                for (int i = 0; i < Client.Count; i++)
                    if (_comparer.Compare(Client[i], value) == 0)
                    {
                        Client.RemoveAt(i);
                        return;
                    }
            }
            else
                Client.Remove(value);
        }
        object IList.this[int index]
        {
            get
            {
                if (_converterOutput != null)
                    return _converterOutput(Client[index]);
                return (T)Client[index];
            }
            set
            {
                if (value is T && _converterInput != null)
                    Client[index] = _converterInput((T)value);
                else
                    Client[index] = value;
            }
        }
        #endregion
        #region Membros de ICollection
        public void CopyTo(Array array, int index)
        {
            if (_converterOutput != null)
            {
                if (array == null)
                    throw new ArgumentNullException("array");
                for (var i = 0; i < array.Length - index && i < this.Count; i++)
                    array.SetValue(this[i], index + i);
            }
            else
                Client.CopyTo(array, index);
        }
        public bool IsSynchronized
        {
            get { return Client.IsSynchronized; }
        }
        public object SyncRoot
        {
            get { return Client.SyncRoot; }
        }
        #endregion
    }
}
