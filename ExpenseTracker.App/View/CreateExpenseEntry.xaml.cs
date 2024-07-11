﻿using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

using ExpenseTracker.CurrencyConverter;
using ExpenseTracker.Data;
using ExpenseTracker.Wpf.Dialog;

namespace ExpenseTracker.View
{
    /// <summary>
    /// Interaction logic for CreateVariableExpenseWindow.xaml
    /// </summary>
    public partial class CreateExpenseEntry : Window
    {
        public DataEntry Entry { get; set; }
        private readonly CurrencyInfo _mainCurrency = null;
        public CreateExpenseEntry(CurrencyInfo mainCurrency)
        {
            InitializeComponent();
            // Initialize the value
            UpdateCategoryList();
            Combo_Currency.ItemsSource = CurrencyInfo.GenerateCurrencyList();
            Combo_Currency.SelectedItem = mainCurrency;
            _mainCurrency = mainCurrency;
        }

        private void UpdateCategoryList()
        {
            CmbBox_ExpenseCategory.ItemsSource = DataHandler.DataCategories.ExpenseCategories.ToArray();
            CmbBox_PaymentChannel.ItemsSource = DataHandler.DataCategories.PaymentChannels.ToArray();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (TxtBox_Description.Text == string.Empty || TxtBox_Amount.Text == string.Empty)
            {
                MessageBox.Show("Please supply all necessary information", "Input Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            Entry = new DataEntry()
            {
                Description = TxtBox_Description.Text,
                Amount = float.Parse(TxtBox_Amount.Text, CultureInfo.InvariantCulture.NumberFormat),
                PaymentChannel = CmbBox_PaymentChannel.SelectedItem as string,
                ExpenseCategory = CmbBox_ExpenseCategory.SelectedItem as string,
                Currency = Combo_Currency.SelectedItem as CurrencyInfo,
                EntryDate = DateTime.Now.ToString()
            };

            if (_mainCurrency != null)
            {
                if (Entry.Currency != _mainCurrency)
                    Entry.OriginalAmount = Entry.Amount;
            }

            DialogResult = true;
            Close();
        }

        private void BtnCreatePaymentChannel_Click(object sender, RoutedEventArgs e)
        {
            NameDialog dialog = new NameDialog("Enter new PaymentChannel:");
            if (dialog.ShowDialog() ?? true)
            {
                if (DataHandler.AddPaymentChannel(dialog.InputText))
                {
                    UpdateCategoryList();
                }
            }
        }

        private void BtnExpenseCreateCategory_Click(object sender, RoutedEventArgs e)
        {
            NameDialog dialog = new NameDialog("Enter new ExpenseCategory:");
            if (dialog.ShowDialog() ?? true)
            {
                if (DataHandler.AddExpenseCategory(dialog.InputText))
                {
                    UpdateCategoryList();
                }
            }
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private static readonly Regex _regex = new Regex("[^0-9.-]+");
        private static bool IsNumeric(string text)
        {
            return !_regex.IsMatch(text);
        }

        private void TxtBox_Amount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsNumeric(e.Text);
        }
    }
}
