using ExpenseTracker.Data;
using ExpenseTracker.View.Tools;
using ExpenseTracker.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.ViewModels
{
    public class ToolsAndPreferencesViewModel : ViewModel
    {
        #region ViewModels
        private readonly CategoriesEditorViewModel _categoriesEditorViewModel = new CategoriesEditorViewModel();
        public CategoriesEditorViewModel CategoriesEditorViewModel => _categoriesEditorViewModel;
        #endregion
        public ToolsAndPreferencesViewModel()
        {
            AppInstance.Connection.AddViewModel(this);
        }
    }
}
