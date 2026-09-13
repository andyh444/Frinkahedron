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
    internal sealed class WrappedObservableList<TModel, TViewModel> : IReadOnlyList<TViewModel>, INotifyCollectionChanged
    {
        private List<TModel> sourceList;
        private readonly Func<TModel, TViewModel> map;
        private List<TViewModel> viewModelList;


        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        public TViewModel this[int index] => viewModelList[index];

        public int Count => viewModelList.Count;

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
        }

        public IEnumerator<TViewModel> GetEnumerator() => viewModelList.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
