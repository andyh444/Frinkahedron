using Microsoft.UI.Xaml.Interop;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frinkahedron.WinUIEditor.Utilities
{
    /// <summary>
    /// An alternative to ObservableCollection that wraps an existing collection rather than creating a new one
    /// </summary>
    internal sealed class WrappedObservableList<TModel, TViewModel> : IReadOnlyList<TViewModel>, INotifyCollectionChanged, INotifyPropertyChanged, IBindableObservableVector
    {
        // Note: IBindableObservableVector seems to be necessary to get WinUI to subscribe to the CollectionChanged event
        // (even though the documentation says implementing IList and INotifyCollectionChanged is enough)

        private List<TModel> sourceList;
        private readonly Func<TModel, TViewModel> map;
        private List<TViewModel> viewModelList;

        public event BindableVectorChangedEventHandler? VectorChanged;
        public event NotifyCollectionChangedEventHandler? CollectionChanged;
        public event PropertyChangedEventHandler? PropertyChanged;

        public TViewModel this[int index]
        {
            get => viewModelList[index];
        }

        public int Count => viewModelList.Count;

        public bool IsFixedSize => false;

        public bool IsReadOnly => false;

        public bool IsSynchronized => false;

        public object SyncRoot => this;

        object? IList.this[int index] { get => this[index]; set => throw new NotSupportedException(); }

        public WrappedObservableList(List<TModel> sourceList, Func<TModel, TViewModel> map)
        {
            this.sourceList = sourceList;
            this.map = map;
            viewModelList = sourceList.Select(x => map(x)).ToList();
        }

        public void Add(TModel item)
        {
            int index = sourceList.Count;
            var viewModel = map(item);

            sourceList.Add(item);
            viewModelList.Add(viewModel);

            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, viewModel, index));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
            VectorChanged?.Invoke(this, EventArgs.Empty);
        }

        public IEnumerator<TViewModel> GetEnumerator() => viewModelList.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        int IList.Add(object? value) => throw new NotSupportedException();

        public void Clear()
        {
            sourceList.Clear();
            viewModelList.Clear();
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public bool Contains(TViewModel item) => viewModelList.Contains(item);

        public bool Contains(object? value) => value is TViewModel vm && Contains(vm);

        public int IndexOf(object? value)
            => value is TViewModel vm ? viewModelList.IndexOf(vm) : -1;

        void IList.Insert(int index, object? value) => throw new NotSupportedException();

        void IList.Remove(object? value) => throw new NotSupportedException();

        void IList.RemoveAt(int index) => throw new NotSupportedException();

        void ICollection.CopyTo(Array array, int index) => throw new NotSupportedException();
    }
}
