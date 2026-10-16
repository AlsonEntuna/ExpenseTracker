using ExpenseTracker.Data.Wishlist;
using ExpenseTracker.ViewModels;
using ExpenseTracker.Wpf.Dialog;

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ExpenseTracker.View.Wishlist
{
    public partial class WishlistView : UserControl
    {
        private Point _dragStart;

        public WishlistView()
        {
            InitializeComponent();
        }

        private void WishlistCard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount != 2 || (sender as FrameworkElement)?.DataContext is not WishlistItemViewModel item ||
                DataContext is not WishlistViewModel viewModel || item.State == WishlistState.Achieved)
            {
                return;
            }

            var dialog = new NumDialog("Add to wishlist savings") { Owner = Window.GetWindow(this) };
            if (dialog.ShowDialog() == true
                && double.TryParse(dialog.TextValue, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out var amount))
            {
                viewModel.AddAmount(item, amount);
            }
            e.Handled = true;
        }

        private void WishlistList_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _dragStart = e.GetPosition(null);
        }

        private void WishlistList_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed || sender is not ListView list)
            {
                return;
            }

            var position = e.GetPosition(null);
            if (Math.Abs(position.X - _dragStart.X) < SystemParameters.MinimumHorizontalDragDistance &&
                Math.Abs(position.Y - _dragStart.Y) < SystemParameters.MinimumVerticalDragDistance)
            {
                return;
            }

            var container = ItemsControl.ContainerFromElement(list, e.OriginalSource as DependencyObject) as ListViewItem;
            if (container?.DataContext is WishlistItemViewModel item)
            {
                DragDrop.DoDragDrop(container, item, DragDropEffects.Move);
            }
        }

        private void WishlistList_Drop(object sender, DragEventArgs e)
        {
            if (sender is not ListView list || e.Data.GetData(typeof(WishlistItemViewModel)) is not WishlistItemViewModel item ||
                DataContext is not WishlistViewModel viewModel || !Enum.TryParse(list.Tag?.ToString(), out WishlistState state))
            {
                return;
            }

            viewModel.MoveItem(item, state);
            e.Handled = true;
        }
    }
}
