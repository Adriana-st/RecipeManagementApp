using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace RecipeApp.MAUI.ViewModels
{
    /// <summary>
    /// Base ViewModel - all ViewModels inherit from this
    /// Provides INotifyPropertyChanged via ObservableObject
    /// </summary>
    public partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private string title = string.Empty;
    }
}
