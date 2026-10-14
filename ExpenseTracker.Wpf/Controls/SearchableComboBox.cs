#nullable enable

using System.Collections;
using System.ComponentModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

namespace ExpenseTracker.Wpf.Controls;

/// <summary>
/// A themed ComboBox that filters its items as the user types.
/// </summary>
public class SearchableComboBox : ComboBox
{
    private ICollectionView? _itemsView;
    private bool _isApplyingFilter;
    private bool _isReplacingItemsSource;
    private bool _isUserEditing;
    private TextBox? _editableTextBox;

    public SearchableComboBox()
    {
        IsEditable = true;
        IsTextSearchEnabled = false;
        StaysOpenOnEdit = true;
        IsSynchronizedWithCurrentItem = false;
        AddHandler(TextBoxBase.TextChangedEvent, new TextChangedEventHandler(EditableTextChanged));
    }

    public override void OnApplyTemplate()
    {
        if (_editableTextBox is not null)
        {
            _editableTextBox.PreviewTextInput -= EditableTextInput;
            _editableTextBox.PreviewKeyDown -= EditableKeyDown;
            DataObject.RemovePastingHandler(_editableTextBox, EditablePasting);
        }

        base.OnApplyTemplate();

        _editableTextBox = GetTemplateChild("PART_EditableTextBox") as TextBox;
        if (_editableTextBox is not null)
        {
            _editableTextBox.PreviewTextInput += EditableTextInput;
            _editableTextBox.PreviewKeyDown += EditableKeyDown;
            DataObject.AddPastingHandler(_editableTextBox, EditablePasting);
        }
    }

    protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
    {
        base.OnItemsSourceChanged(oldValue, newValue);
        if (_isReplacingItemsSource)
            return;

        // Each ComboBox needs its own view. WPF's default view is shared for a
        // source collection, so filtering or moving its current item can affect
        // other rows bound to the same collection.
        _itemsView = newValue switch
        {
            null => null,
            IList list => new ListCollectionView(list),
            _ => CollectionViewSource.GetDefaultView(newValue)
        };

        if (_itemsView is not null && !ReferenceEquals(_itemsView, newValue))
        {
            _isReplacingItemsSource = true;
            try
            {
                SetCurrentValue(ItemsSourceProperty, _itemsView);
            }
            finally
            {
                _isReplacingItemsSource = false;
            }
        }

        if (_itemsView is not null)
        {
            _itemsView.Filter = null;
            _itemsView.Refresh();
        }
    }

    private void EditableTextChanged(object sender, TextChangedEventArgs e)
    {
        if (_isApplyingFilter || !_isUserEditing || e.OriginalSource is not TextBox searchBox)
            return;

        _isUserEditing = false;
        var searchText = searchBox.Text;
        var selectionStart = searchBox.SelectionStart;
        var selectionLength = searchBox.SelectionLength;
        _isApplyingFilter = true;
        try
        {
            ApplyFilter(searchText);

            // Refreshing the view can make ComboBox rewrite its editable text from
            // the current selection. Keep the user's query in the editor instead.
            if (searchBox.Text != searchText)
                searchBox.Text = searchText;

            selectionStart = Math.Min(selectionStart, searchBox.Text.Length);
            selectionLength = Math.Min(selectionLength, searchBox.Text.Length - selectionStart);
            searchBox.Select(selectionStart, selectionLength);
        }
        finally
        {
            _isApplyingFilter = false;
        }

        if (!IsDropDownOpen)
            IsDropDownOpen = true;
    }

    private void EditableTextInput(object sender, TextCompositionEventArgs e) => _isUserEditing = true;

    private void EditableKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key is Key.Back or Key.Delete
            || (Keyboard.Modifiers.HasFlag(ModifierKeys.Control) && (e.Key is Key.V or Key.X)))
            _isUserEditing = true;
    }

    private void EditablePasting(object sender, DataObjectPastingEventArgs e) => _isUserEditing = true;

    protected override void OnDropDownClosed(EventArgs e)
    {
        base.OnDropDownClosed(e);
        _isUserEditing = false;

        if (_itemsView is not null)
        {
            _itemsView.Filter = null;
            _itemsView.Refresh();
        }
    }

    private void ApplyFilter(string? text = null)
    {
        if (_itemsView is null)
            return;

        var searchText = (text ?? Text)?.Trim();
        _itemsView.Filter = string.IsNullOrEmpty(searchText)
            ? null
            : item => Equals(item, SelectedItem)
                || GetItemText(item).Contains(searchText, StringComparison.CurrentCultureIgnoreCase);
        _itemsView.Refresh();
    }

    private string GetItemText(object? item)
    {
        if (item is null)
            return string.Empty;

        var path = DisplayMemberPath;
        if (string.IsNullOrEmpty(path))
            return item.ToString() ?? string.Empty;

        object? value = item;
        foreach (var segment in path.Split('.'))
        {
            if (value is null)
                return string.Empty;

            value = TypeDescriptor.GetProperties(value)[segment]?.GetValue(value);
        }

        return value?.ToString() ?? string.Empty;
    }
}
