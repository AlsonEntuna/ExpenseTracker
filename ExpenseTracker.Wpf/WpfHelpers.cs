using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace ExpenseTracker.Wpf
{
    public static class WpfHelpers
    {
        public static T FindVisualChildOfType<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T correctlyTyped)
                    return correctlyTyped;

                var result = FindVisualChildOfType<T>(child);
                if (result != null)
                    return result;
            }
            return null;
        }

        public static List<T> FindVisualChildrenOfTypeRecursive<T>(DependencyObject parent) where T : DependencyObject
        {
            List<T> children = new List<T>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T correctlyTyped)
                {
                    children.Add(correctlyTyped);
                }

                var recursedChildren = FindVisualChildrenOfTypeRecursive<T>(child);
                children.AddRange(recursedChildren);
            }
            return children;
        }

        public static T FindVisualParentOfType<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            while (parentObject != null)
            {
                if (parentObject is T _parent)
                    return _parent;

                parentObject = VisualTreeHelper.GetParent(parentObject);
            }

            return null;
        }
    }
}
