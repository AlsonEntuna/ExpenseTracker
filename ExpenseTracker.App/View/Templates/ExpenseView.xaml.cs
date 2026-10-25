using ExpenseTracker.Data;
using ExpenseTracker.Wpf;
using ExpenseTracker.Wpf.Controls;

using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace ExpenseTracker.View.Templates
{
    public partial class ExpenseView : UserControl
    {
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ExpenseVm"
                , typeof(ExpenseViewModel)
                , typeof(ExpenseView)
                , new PropertyMetadata(null));

        public ExpenseViewModel ExpenseVm
        {
            get => (ExpenseViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        public ExpenseView()
        {
            InitializeComponent();
        }

        private void GridSplitter_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            GridSplitter gridSplitter = sender as GridSplitter;

            if (gridSplitter != null)
            {
                ((DataGridCell)gridSplitter.Tag).Column.Width
                    = ((DataGridCell)gridSplitter.Tag).Column.ActualWidth +
                      e.HorizontalChange;
            }
        }

        /// <summary>
        /// Starts editing a dropdown cell on the first click, so the plain-text cell feels like a live ComboBox.
        /// </summary>
        private void ComboCell_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not DataGridCell cell || cell.IsEditing || cell.IsReadOnly)
            {
                return;
            }

            // Leave Ctrl/Shift clicks to the grid so multi-select still works
            if (Keyboard.Modifiers != ModifierKeys.None)
            {
                return;
            }

            // Don't start editing when the user is grabbing the in-cell splitter
            if (e.OriginalSource is GridSplitter
                || (e.OriginalSource is Visual source && WpfHelpers.FindVisualParentOfType<GridSplitter>(source) != null))
            {
                return;
            }

            if (!cell.IsFocused)
            {
                cell.Focus();
            }

            ExpenseDataGrid.BeginEdit(e);
        }

        private void ExpenseDataGrid_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            // Open the dropdown once the editing template is in the visual tree
            // ContextIdle runs after the click and the grid's own focus handling finish,
            // so focus stays in the editor instead of going back to the cell.
            Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, () =>
            {
                SearchableComboBox comboBox = e.EditingElement as SearchableComboBox
                    ?? WpfHelpers.FindVisualChildOfType<SearchableComboBox>(e.EditingElement);
                if (comboBox == null)
                {
                    return;
                }

                comboBox.FocusEditor();
                comboBox.IsDropDownOpen = true;
            });
        }

        private void ExpenseDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ExpenseVm != null)
            {
                ExpenseVm.SelectedDataEntries = ExpenseDataGrid.SelectedItems.OfType<DataEntry>().ToList();
            }
        }

        private void ExpenseDataGrid_OnCopy(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            if (ExpenseVm != null)
            {
                ExpenseVm.CopyEntriesToClipboard();
            }
        }

        // TODO: change and make this as a NumericTextBox
        private static readonly Regex _regex = new Regex("[^0-9.-]+");
        private static bool IsNumeric(string text)
        {
            return !_regex.IsMatch(text);
        }

        private void Txt_Amount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsNumeric(e.Text);
        }
    }
}