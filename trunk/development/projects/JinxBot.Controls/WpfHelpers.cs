using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace JinxBot.Controls
{
    internal static class WpfHelpers
    {
        // The algorythm is really simple, but this extension method provides a HUGE productivity impact
        public static T FindFirstVisualAncestorOfType<T>(this DependencyObject visual)
                          where T : DependencyObject
        {
            var elem = VisualTreeHelper.GetParent(visual);
            while (elem != null)
            {
                var asT = elem as T;
                if (asT != null)
                    return asT;
                elem = VisualTreeHelper.GetParent(elem);
            }
            return null;
        }
        // This one is a bit more complex. The sub-tree can be explored either Vertically or Horizontally.
        // The best choice in this case is "no choice" => let the user decide by providing an enumerated value
        public static T FindFirstVisualDescendantOfType<T>(this DependencyObject visual,
                                          VisualChildExplorationMode explorationMode = VisualChildExplorationMode.Horizontal)
                                            where T : DependencyObject
        {
            switch (explorationMode)
            {
                //Vertical mode : very straight forward. Should be optimized by avoiding recursive calls
                case VisualChildExplorationMode.Vertical:
                    for (int i = 0; i < VisualTreeHelper.GetChildrenCount(visual); i++)
                    {
                        var child = VisualTreeHelper.GetChild(visual, i);
                        if (child is T)
                            return child as T;
                        else
                        {
                            var childTest = child.FindFirstVisualDescendantOfType<T>(VisualChildExplorationMode.Vertical);
                            if (childTest != null)
                                return childTest;
                        }
                    }
                    break;
                // Horizontal mode : let's use the power of Linq
                // at each iteration we have to maintain 2 List<DependencyObject>: the current explored generation,
                // and the next one.
                // The nextGeneration creation using Linq to Objects is really cool:
                // SelectMany flattens a IEnumerable<IEnumerable<T>> to an IEnumerable<T>. I love that.
                // Just imagine the number of lines of code to write this without intermediate collections in 
                // .Net 2 (only iterators).
                case VisualChildExplorationMode.Horizontal:
                    List<DependencyObject> currentGeneration = new List<DependencyObject> { visual }, nextGeneration;
                    nextGeneration = new List<DependencyObject>(currentGeneration.SelectMany(v => v.GetVisualChildren()));
                    while (nextGeneration.Count > 0)
                    {
                        currentGeneration = nextGeneration;
                        var result = currentGeneration.OfType<T>().FirstOrDefault();
                        if (result != null)
                            return result;
                        nextGeneration = new List<DependencyObject>(currentGeneration.SelectMany(v => v.GetVisualChildren()));

                    }
                    break;
            }
            return null;
        }
        public static IEnumerable<DependencyObject> GetVisualChildren(this DependencyObject visual)
        {
            var count = VisualTreeHelper.GetChildrenCount(visual);
            for (int i = 0; i < count; i++)
                yield return VisualTreeHelper.GetChild(visual, i);
        }
    }
    internal enum VisualChildExplorationMode
    {
        Horizontal,
        Vertical
    }
}
