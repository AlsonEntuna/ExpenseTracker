using System.Collections.ObjectModel;

namespace ExpenseTracker.Tools
{
    public static class ListUtils
    {
        public static ObservableCollection<T> ToObservableCollection<T>(IEnumerable<T> list)
        {
            ObservableCollection<T> collection = [.. list];
            return collection;
        }
    }
}
