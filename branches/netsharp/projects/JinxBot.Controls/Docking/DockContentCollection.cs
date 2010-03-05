using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace JinxBot.Controls.Docking
{
    /// <summary>
    /// Implements a read-only collection of docking elements.
    /// </summary>
    public class DockContentCollection : ReadOnlyCollection<IDockContent>
    {
        private static List<IDockContent> _emptyList = new List<IDockContent>(0);

        internal DockContentCollection()
            : base(new List<IDockContent>())
        {
        }

        internal DockContentCollection(DockPane pane)
            : base(_emptyList)
        {
            m_dockPane = pane;
        }

        private DockPane m_dockPane;
        private DockPane DockPane
        {
            get { return m_dockPane; }
        }

        /// <summary>
        /// Gets the docking content at the specified index.
        /// </summary>
        /// <param name="index">The index to check.</param>
        /// <returns>The docking content at the specified index.</returns>
        public new IDockContent this[int index]
        {
            get
            {
                if (DockPane == null)
                    return Items[index] as IDockContent;
                else
                    return GetVisibleContent(index);
            }
        }

        internal int Add(IDockContent content)
        {
#if DEBUG
            if (DockPane != null)
                throw new InvalidOperationException();
#endif

            if (Contains(content))
                return IndexOf(content);

            Items.Add(content);
            return Count - 1;
        }

        internal void AddAt(IDockContent content, int index)
        {
#if DEBUG
            if (DockPane != null)
                throw new InvalidOperationException();
#endif

            if (index < 0 || index > Items.Count - 1)
                return;

            if (Contains(content))
                return;

            Items.Insert(index, content);
        }

        /// <summary>
        /// Gets whether the docking content was contained within this collection.
        /// </summary>
        /// <param name="content">The content to check for.</param>
        /// <returns><see langword="true" /> if the content is contained; otherwise <see langword="false" />.</returns>
        public new bool Contains(IDockContent content)
        {
            if (DockPane == null)
                return Items.Contains(content);
            else
                return (GetIndexOfVisibleContents(content) != -1);
        }

        /// <summary>
        /// Gets the number of items in the collection.
        /// </summary>
        public new int Count
        {
            get
            {
                if (DockPane == null)
                    return base.Count;
                else
                    return CountOfVisibleContents;
            }
        }

        /// <summary>
        /// Gets the index of the specified docking content.
        /// </summary>
        /// <param name="content">The content to check for.</param>
        /// <returns>The index of the content, or <c>-1</c> if the content is not found.</returns>
        public new int IndexOf(IDockContent content)
        {
            if (DockPane == null)
            {
                if (!Contains(content))
                    return -1;
                else
                    return Items.IndexOf(content);
            }
            else
                return GetIndexOfVisibleContents(content);
        }


        internal void Remove(IDockContent content)
        {
            if (DockPane != null)
                throw new InvalidOperationException();

            if (!Contains(content))
                return;

            Items.Remove(content);
        }

        private int CountOfVisibleContents
        {
            get
            {
#if DEBUG
                if (DockPane == null)
                    throw new InvalidOperationException();
#endif

                int count = 0;
                foreach (IDockContent content in DockPane.Contents)
                {
                    if (content.DockHandler.DockState == DockPane.DockState)
                        count++;
                }
                return count;
            }
        }

        private IDockContent GetVisibleContent(int index)
        {
#if DEBUG
            if (DockPane == null)
                throw new InvalidOperationException();
#endif

            int currentIndex = -1;
            foreach (IDockContent content in DockPane.Contents)
            {
                if (content.DockHandler.DockState == DockPane.DockState)
                    currentIndex++;

                if (currentIndex == index)
                    return content;
            }
            throw new ArgumentOutOfRangeException("index");
        }

        private int GetIndexOfVisibleContents(IDockContent content)
        {
#if DEBUG
            if (DockPane == null)
                throw new InvalidOperationException();
#endif

            if (content == null)
                return -1;

            int index = -1;
            foreach (IDockContent c in DockPane.Contents)
            {
                if (c.DockHandler.DockState == DockPane.DockState)
                {
                    index++;

                    if (c == content)
                        return index;
                }
            }
            return -1;
        }
    }
}
