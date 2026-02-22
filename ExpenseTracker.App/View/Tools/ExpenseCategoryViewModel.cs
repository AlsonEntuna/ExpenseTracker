using CommunityToolkit.Mvvm.Input;
using ExpenseTracker.Data;
using ExpenseTracker.Tools;
using ExpenseTracker.Wpf;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace ExpenseTracker.View.Tools
{
    internal class ExpenseCategoryWrapper : ViewModel
    {
        private string _categoryName;
        public string CategoryName
        {
            get => _categoryName;
            set => SetProperty(ref _categoryName, value);
        }

        private bool _editing;
        public bool Editing
        {
            get => _editing;
            set => SetProperty(ref _editing, value);
        }
        public ExpenseCategoryWrapper() { }
        public ExpenseCategoryWrapper(string categoryName)
        {
            CategoryName = categoryName;
            Editing = false;
        }
    }

    internal class ExpenseCategoriesViewModel : ViewModel, IDataHandler
    {
        private ObservableCollection<ExpenseCategoryWrapper> _expenseCategories;
        public ObservableCollection<ExpenseCategoryWrapper> ExpenseCategories => _expenseCategories;

        private ExpenseCategoryWrapper _selectedExpenseCategory;
        public ExpenseCategoryWrapper SelectedExpenseCategory
        {
            get => _selectedExpenseCategory;
            set => SetProperty(ref _selectedExpenseCategory, value);
        }

        public ICommand AddExpenseCategoryCommand => new RelayCommand(AddExpenseCategory);
        public ICommand RemoveExpenseCategoryCommand => new RelayCommand(RemoveExpenseCategory);


        public ExpenseCategoriesViewModel(List<string> expenseCategories)
        {
            _expenseCategories = ListUtils.ToObservableCollection(expenseCategories.Select(f => new ExpenseCategoryWrapper(f)));
            // Register to the app instance connection
            AppInstance.Connection.AddViewModel(this);
        }

        private void AddExpenseCategory()
        {
            ExpenseCategoryWrapper newCategory = new ExpenseCategoryWrapper("New Category");
            SelectedExpenseCategory = newCategory;
            DataHandler.AddExpenseCategory(newCategory.CategoryName);
        }
        private void RemoveExpenseCategory()
        {
            ExpenseCategories.Remove(_selectedExpenseCategory);
            DataHandler.RemoveExpenseCategory(_selectedExpenseCategory.CategoryName);
        }

        public void Save()
        {
            DataHandler.DataCategories.ExpenseCategories.Clear();
            foreach (ExpenseCategoryWrapper pItem in _expenseCategories)
            {
                DataHandler.AddExpenseCategory(pItem.CategoryName);
            }
        }
    }
}
