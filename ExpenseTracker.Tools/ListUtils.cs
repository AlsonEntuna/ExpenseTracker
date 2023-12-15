using System.Collections.ObjectModel;

namespace ExpenseTracker.Tools
{
    public static class ListUtils
    {
        public static ObservableCollection<T> ToObservableCollection<T>(IEnumerable<T> list)
        {
            ObservableCollection<T> newList = new ObservableCollection<T>();
            foreach (T val in list)
            {
                newList.Add(val);
            }
            return newList;
        }
    }
}
