using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JinxBot.Views.Chat
{
    public class ItemFilteringEventArgs : EventArgs
    {
        private object _item;
        private string _filter;

        /// <summary>
        /// Creates a new <see>ItemFilteringEventArgs</see>.
        /// </summary>
        /// <param name="item"></param>
        public ItemFilteringEventArgs(object item, string filterExpression)
        {
            _item = item;
            _filter = filterExpression;
        }

        /// <summary>
        /// Gets the item being evaluated.
        /// </summary>
        public object Item
        {
            get { return _item; }
        }

        /// <summary>
        /// Gets the filtering expression from the user.
        /// </summary>
        public string Filter
        {
            get { return _filter; }
        }

        /// <summary>
        /// Specifies whether the item should be removed.
        /// </summary>
        public bool ShouldBeRemoved { get; set; }
    }
}
