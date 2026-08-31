using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frinkahedron.WinUIEditor.ViewModels
{
    public abstract class ViewModelBase : ObservableObject
    {
        protected void SetModelProperty<T>(T currentValue, T newValue, Action<T> setter, string? name = null) where T : IEquatable<T>
        {
            if (currentValue?.Equals(newValue) == true)
            {
                return;
            }
            setter(newValue);
            if (string.IsNullOrEmpty(name))
            {
                OnPropertyChanged();
            }
            else
            {
                OnPropertyChanged(name);
            }
        }
    }
}
