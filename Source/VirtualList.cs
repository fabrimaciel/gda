using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
namespace GDA.Collections
{
    /// <summary>
    /// Delegate que representa o método usado para carregar os dados da lista virtual.
    /// </summary>
    /// <typeparam name="Model">Model de dados da lista.</typeparam>
    /// <param name="startRow">Linha inicial para a recuperação dos dados.</param>
    /// <param name="pageSize">Tamanho da página de dados que será recuperada.</param>
    /// <returns></returns>
    public delegate IEnumerable<Model> VirtualListLoadMethod<Model>(int startRow, int pageSize);
    /// <summary>
    /// Método usado para recupera a quantidade de elementos que a lista virtual contém.
    /// </summary>
    /// <returns></returns>
    public delegate int VirtualListCountMethod();
    /// <summary>
    /// Estrutura que representa uma lista virtual, ou seja,
    /// ela contêm itens que ainda não foram carregados.
    /// </summary>
    public class VirtualList<Model> : 
        IEnumerable<Model>, IEnumerable, IList<Model>, IList, ICollection<Model>, ICollection
    {
        #region Variáveis Locais
        /// <summary>
        /// Tamanho da página de cada sessão.
        /// </summary>
        private int _pageSize;
        /// <summary>
        /// Quantidade de elementos da lista.
        /// </summary>
        private int _count;
        /// <summary>
        /// Sessões onde são armazenados os elementos carregados na lista
        /// </summary>
        private IList<Model>[] _sessions;
        /// <summary>
        /// Versão da lista.
        /// </summary>
        internal int _version;
        /// <summary>
        /// Objeto usadao para garantir a sincronização da lista.
        /// </summary>
        private object _syncRoot;
        /// <summary>
        /// Instancia do método usado para recuperar os itens da lista virtual.
        /// </summary>
        private VirtualListLoadMethod<Model> _loadMethod;
        /// <summary>
        /// Instancia do método usado para recuperar a quantidade de itens da lista virtual.
        /// </summary>
        private VirtualListCountMethod _countMethod;
        private Func<Model, Model> _processMethod;
        private Action<IEnumerable<Model>> _processItemsMethod;
        #endregion
        #region Propriedades Públicas
        /// <summary>
        /// Quantidade de itens na lista.
        /// </summary>
        public int Count
        {
            get { return _count; }
        }
        public bool IsReadOnly
        {
            get { return true; }
        }
        #endregion
        #region Construtores
        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="pageSize">Tamanho da página da lista virtual.</param>
        /// <param name="loadMethod">Instancia do método que será usado para carregar os dados.</param>
        /// <param name="countMethod">Instancia do método que será usado para recuperar quantidade de dados da lista.</param>
        public VirtualList(int pageSize, VirtualListLoadMethod<Model> loadMethod, VirtualListCountMethod countMethod)
        {
            if (loadMethod == null)
                throw new ArgumentNullException("loadMethod");
            else if (countMethod == null)
                throw new ArgumentNullException("countMethod");
            _pageSize = pageSize;
            _loadMethod = loadMethod;
            _countMethod = countMethod;
            Refresh();
        }
        #endregion
        #region Métodos Privados
        /// <summary>
        /// Recupera o item na posição informada.
        /// </summary>
        /// <param name="index">Posição do item.</param>
        /// <returns></returns>
        internal protected virtual Model GetItem(int index)
        {
            // Verifica se o item está no intervalo da lista
            if (index >= this.Count)
            {
                throw new ArgumentOutOfRangeException();
            }
            // Recupera a sessão que a posição pertence
            int indexSession = (int)Math.Floor(index / (double)_pageSize);
            // Verifica se a sessão não foi carregada
            if (_sessions[indexSession] == null)
            {
                List<Model> items = null;
                if (_processMethod != null)
                {
                    var method = _processMethod;
                    items = new List<Model>();
                    foreach (var item in _loadMethod(indexSession * _pageSize, _pageSize))
                        // Processa o item
                        items.Add(method(item));
                }
                else
                    // Recupera a lista dos itens da sessão
                    items = new List<Model>(_loadMethod(indexSession * _pageSize, _pageSize));
                if (_processItemsMethod != null)
                    _processItemsMethod(items);
                _sessions[indexSession] = items;
            }
            return _sessions[indexSession][index - (indexSession * _pageSize)];
        }
        /// <summary>
        /// Define o item na posição informada.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        internal protected virtual void SetItem(int index, Model value)
        {
            if (index >= _count)
            {
                throw new ArgumentOutOfRangeException();
            }
            // Recupera a sessão que a posição pertence
            int indexSession = (int)Math.Floor(index / (double)_pageSize);
            OnSet(index, GetItem(index), value);
            _sessions[indexSession][index - (indexSession * _pageSize)] = value;
            this._version++;
            OnSetComplete(index, value);
        }
        #endregion
        #region Métodos Protegidos
        /// <summary>
        /// Método acionado quando o valor de um item é definido.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected virtual void OnSet(int index, Model oldValue, Model newValue)
        {
        }
        protected virtual void OnSetComplete(int index, Model newValue)
        {
        }
        /// <summary>
        /// Método acionado quando se dar um refresh na lista.
        /// </summary>
        protected virtual void OnRefresh()
        {
        }
        #endregion
        #region Métodos Públicos
        /// <summary>
        /// Registra o método que será responsável por processar os itens carregados na coleção.
        /// </summary>
        /// <param name="method"></param>
        public void RegisterProcessMethod(Func<Model, Model> method)
        {
            _processMethod = method;
        }
        /// <summary>
        /// Registra o método que será responsável por processar os itens carregados.
        /// </summary>
        /// <param name="method"></param>
        public void RegisterItemsProcessMethod(Action<IEnumerable<Model>> method)
        {
            _processItemsMethod = method;
        }
        /// <summary>
        /// Atualiza os dados da lista.
        /// </summary>
        public void Refresh()
        {
            // Recupera a quantidade de elementos da lista
            _count = _countMethod();
            // Recupera o número de sessões necessárias
            int numberSessions = (int)Math.Ceiling(_count / (double)_pageSize);
            if (_sessions != null)
            {
                for (int i = 0; i < _sessions.Length; i++)
                {
                    if (_sessions[i] != null)
                    {
                        _sessions[i].Clear();
                        _sessions[i] = null;
                    }
                }
            }
            // Instância as sessões
            _sessions = new IList<Model>[numberSessions];
            _version++;
            OnRefresh();
        }
        /// <summary>
        /// Recupera o item na posição informada.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Model this[int index]
        {
            get
            {
                return GetItem(index);
            }
            set
            {
                SetItem(index, value);
            }
        }
        #endregion
        #region Estruturas internas
        /// <summary>
        /// Enumerator usado na lista.
        /// </summary>
        /// <typeparam name="Model"></typeparam>
        public struct Enumerator<Model> : IEnumerator<Model>, IDisposable, IEnumerator
        {
            /// <summary>
            /// Lista em questão.
            /// </summary>
            private VirtualList<Model> list;
            /// <summary>
            /// Atual index.
            /// </summary>
            private int index;
            /// <summary>
            /// Versão atual da lista
            /// </summary>
            private int version;
            /// <summary>
            /// Objeto atualmente selecionado.
            /// </summary>
            private Model current;
            internal Enumerator(VirtualList<Model> list)
            {
                this.list = list;
                this.index = 0;
                this.version = list._version;
                this.current = default(Model);
            }
            public void Dispose()
            {
            }
            /// <summary>
            /// Movimenta para o proximo objeto.
            /// </summary>
            /// <returns></returns>
            public bool MoveNext()
            {
                if (this.version != this.list._version)
                {
                    throw new InvalidOperationException("InvalidOperation_EnumFailedVersion");
                }
                if (this.index < this.list.Count)
                {
                    this.current = this.list[this.index];
                    this.index++;
                    return true;
                }
                this.index = this.list.Count + 1;
                this.current = default(Model);
                return false;
            }
            /// <summary>
            /// Atual objeto selecionado.
            /// </summary>
            public Model Current
            {
                get
                {
                    return this.current;
                }
            }
            /// <summary>
            /// Atual objeto selecionado.
            /// </summary>
            object IEnumerator.Current
            {
                get
                {
                    if ((this.index == 0) || (this.index == (this.list.Count + 1)))
                    {
                        throw new InvalidOperationException("InvalidOperation_EnumOpCantHappen");
                    }
                    return this.Current;
                }
            }
            /// <summary>
            /// Reseta a lista.
            /// </summary>
            void IEnumerator.Reset()
            {
                if (this.version != this.list._version)
                {
                    throw new InvalidOperationException("InvalidOperation_EnumFailedVersion");
                }
                this.index = 0;
                this.current = default(Model);
            }
        }
        #endregion
        #region IEnumerable<Model> Members
        /// <summary>
        /// Recupera o enumerator da lista.
        /// </summary>
        /// <returns></returns>
        IEnumerator<Model> IEnumerable<Model>.GetEnumerator()
        {
            return new Enumerator<Model>(this);
        }
        #endregion
        #region IEnumerable Members
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator<Model>(this);
        }
        #endregion
        #region ICollection Members
        public void CopyTo(Array array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            for (int i = 0; i < array.Length - arrayIndex && i < this.Count; i++)
                array.SetValue(this[i], i + arrayIndex);
        }
        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }
        public object SyncRoot
        {
            get
            {
                if (_syncRoot == null)
                {
                    System.Threading.Interlocked.CompareExchange(ref _syncRoot, new object(), null);
                }
                return _syncRoot;
            }
        }
        #endregion
        #region Membros de ICollection<Model>
        void ICollection<Model>.Add(Model item)
        {
            throw new NotSupportedException("Not supported readonly collection");
        }
        void ICollection<Model>.Clear()
        {
            throw new NotSupportedException("Not supported readonly collection");
        }
        bool ICollection<Model>.Contains(Model item)
        {
            throw new NotSupportedException("Not supported readonly collection");
        }
        void ICollection<Model>.CopyTo(Model[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            for (int i = 0; i < array.Length - arrayIndex && i < this.Count; i++)
                array[i + arrayIndex] = this[i];
        }
        int ICollection<Model>.Count
        {
            get { return this.Count; }
        }
        bool ICollection<Model>.IsReadOnly
        {
            get { return true; }
        }
        bool ICollection<Model>.Remove(Model item)
        {
            throw new NotSupportedException("Not supported readonly collection");
        }
        #endregion
        #region Membros de IList<Model>
        int IList<Model>.IndexOf(Model item)
        {
            throw new NotSupportedException("Not supported IndexOf");
        }
        void IList<Model>.Insert(int index, Model item)
        {
            throw new NotSupportedException("Not supported readonly collection");
        }
        void IList<Model>.RemoveAt(int index)
        {
            throw new NotSupportedException("Not supported readonly collection");
        }
        Model IList<Model>.this[int index]
        {
            get
            {
                return GetItem(index);
            }
            set
            {
                SetItem(index, value);
            }
        }
        #endregion
        #region Membros de IList
        int IList.Add(object value)
        {
            throw new NotSupportedException("Not supported readonly collection");
        }
        void IList.Clear()
        {
            throw new NotSupportedException("Not supported readonly collection");
        }
        bool IList.Contains(object value)
        {
            throw new NotSupportedException("Not supported Contains");
        }
        int IList.IndexOf(object value)
        {
            throw new NotSupportedException("Not supported IndexOf");
        }
        void IList.Insert(int index, object value)
        {
            throw new NotSupportedException("Not supported readonly collection");
        }
        bool IList.IsFixedSize
        {
            get { return false; }
        }
        bool IList.IsReadOnly
        {
            get { return true; }
        }
        void IList.Remove(object value)
        {
            throw new NotSupportedException("Not supported readonly collection");
        }
        void IList.RemoveAt(int index)
        {
            throw new NotSupportedException("Not supported readonly collection");
        }
        object IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                this[index] = (Model)value;
            }
        }
        #endregion
        #region Membros de ICollection
        void ICollection.CopyTo(Array array, int index)
        {
            this.CopyTo(array, index);
        }
        int ICollection.Count
        {
            get { return this.Count; }
        }
        bool ICollection.IsSynchronized
        {
            get { return this.IsSynchronized; }
        }
        object ICollection.SyncRoot
        {
            get { return this.SyncRoot; }
        }
        #endregion
    }
}
