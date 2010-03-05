using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace JinxBot.Views.Chat
{
    internal sealed class CustomDrawnSearchableListBox : CustomDrawnListBox
    {
        #region Searching text box class
        private class SearchingTextBox : TextBox
        {
            public void RoutMessage(ref Message m)
            {
                WndProc(ref m);
            }

            protected override void OnKeyDown(KeyEventArgs e)
            {
                if (!Focused)
                {
                    Focus();
                }
                if (e.KeyCode == Keys.Escape)
                {
                    e.Handled = true;
                }

                base.OnKeyDown(e);
            }

            protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
            {
                base.OnPreviewKeyDown(e);
            }
        }
        #endregion

        #region Custom collection class
        public class SearchableObjectCollection : IList, ICollection, IEnumerable
        {
            private ListBox _owner;
            private ObjectCollection _realCollection;
            private List<object> _allItems;
            private Func<object, bool> _filterExpr;

            internal ObjectCollection RealColllection
            {
                get { return _realCollection; }
                set { _realCollection = value; }
            }

            internal SearchableObjectCollection(ListBox owner, ObjectCollection realCollection)
            {
                _owner = owner;
                _realCollection = realCollection;
                _allItems = new List<object>();
            }

            public void Filter(Func<object, bool> predicate)
            {
                _filterExpr = predicate;
                _realCollection.Clear();
                object[] filteredItems = _allItems.Where(predicate).ToArray();
                _realCollection.AddRange(filteredItems);
            }

            /// <summary>
            /// 
            /// </summary>
            public void EndFilter()
            {
                _filterExpr = null;

                _realCollection.Clear();
                _realCollection.AddRange(_allItems.ToArray());
            }

            #region IEnumerable Members

            public IEnumerator GetEnumerator()
            {
                return _allItems.GetEnumerator();
            }

            #endregion

            #region ICollection Members

            public void CopyTo(Array array, int index)
            {
                ((ICollection)_allItems).CopyTo(array, index);
            }

            public int Count
            {
                get { return _allItems.Count; }
            }

            public bool IsSynchronized
            {
                get { return ((ICollection)_allItems).IsSynchronized; }
            }

            public object SyncRoot
            {
                get { return ((ICollection)_allItems).SyncRoot; }
            }

            #endregion

            #region IList Members

            public int Add(object value)
            {
                int result = ((IList)_allItems).Add(value);
                if (CheckFilter(value))
                    _realCollection.Add(value);
                return result;
            }

            private bool CheckFilter(object value)
            {
                return (_filterExpr == null) || (_filterExpr(value));
            }

            public void Clear()
            {
                _allItems.Clear();
                _realCollection.Clear();
            }

            public bool Contains(object value)
            {
                return _allItems.Contains(value);
            }

            public int IndexOf(object value)
            {
                return _allItems.IndexOf(value);
            }

            public void Insert(int index, object value)
            {
                _allItems.Insert(index, value);
                if (CheckFilter(value))
                    _realCollection.Insert(DetermineRelativeIndex(index), value);
            }

            private int DetermineRelativeIndex(int index)
            {
                if (_filterExpr == null)
                    return index;
                else
                {
                    if (index == 0)
                        return 0;
                    else
                    {
                        for (int i = index - 1; i > 0; i--)
                        {
                            if (_filterExpr(_allItems[i]))
                            {
                                // the item is contained within the display, so get its index.
                                return _realCollection.IndexOf(_allItems[i]) + 1;
                            }
                        }
                    }

                    return -1;
                }
            }

            public bool IsFixedSize
            {
                get { return false; }
            }

            public bool IsReadOnly
            {
                get { return false; }
            }

            public void Remove(object value)
            {
                _allItems.Remove(value);
                if (CheckFilter(value))
                    _realCollection.Remove(value);
            }

            public void RemoveAt(int index)
            {
                object item = _allItems[index];
                Remove(item);
            }

            public object this[int index]
            {
                get
                {
                    return _allItems[index];
                }
                set
                {
                    this._realCollection[this._realCollection.IndexOf(_allItems[index])] = value;
                    _allItems[index] = value;
                }
            }

            #endregion
        }
        #endregion

        private SearchingTextBox _searchBox;
        private SearchableObjectCollection _items;

        protected override ListBox.ObjectCollection CreateItemCollection()
        {
            ObjectCollection objs = base.CreateItemCollection();
            if (_items != null)
                _items.RealColllection = objs;
            return objs;
        }

        public CustomDrawnSearchableListBox()
        {
            Initialize();

            _items = new SearchableObjectCollection(this, base.Items);
        }

        public new SearchableObjectCollection Items
        {
            get
            {
                if (_items == null)
                {
                    _items = new SearchableObjectCollection(this, base.Items);
                }
                return _items;
            }
        }

        private void Initialize()
        {
            this._searchBox = new SearchingTextBox();

            this._searchBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this._searchBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._searchBox.ForeColor = System.Drawing.Color.White;

            this._searchBox.TabIndex = 1;
            this._searchBox.Visible = false;
            //Controls.Add(_searchBox);

            this._searchBox.KeyDown += new KeyEventHandler(_searchBox_KeyDown);
            this._searchBox.TextChanged += new EventHandler(_searchBox_TextChanged);
            this._searchBox.Leave += new EventHandler(_searchBox_Leave);
        }

        void _searchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (_searchBox.Text.Length == 0)
                {
                    _searchBox.Visible = false;
                }
                else
                {
                    _searchBox.Text = "";
                }
                e.Handled = true;
            }
        }

        void _searchBox_TextChanged(object sender, EventArgs e)
        {
            if (_searchBox.Text.Length == 0)
            {
                this.Items.EndFilter();

            }
            else
            {
                this._items.Filter(o =>
                {
                    ItemFilteringEventArgs args = new ItemFilteringEventArgs(o, _searchBox.Text);
                    OnFilteringItem(args);
                    return !args.ShouldBeRemoved;
                });
            }
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            if (Parent != null)
            {
                this._searchBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                    | System.Windows.Forms.AnchorStyles.Right)));
                this._searchBox.Location = new System.Drawing.Point(12, Size.Height - 32);
                this._searchBox.Name = "_searchBox";
                this._searchBox.Size = new System.Drawing.Size(Size.Width - 24, 20);
                Parent.Controls.Add(_searchBox);
                _searchBox.BringToFront();
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (Parent != null && _searchBox.Parent != null)
            {
                this._searchBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                    | System.Windows.Forms.AnchorStyles.Right)));
                this._searchBox.Location = new System.Drawing.Point(24, Size.Height - 32);
                this._searchBox.Name = "_searchBox";
                this._searchBox.Size = new System.Drawing.Size(Size.Width - 48, 20);
                _searchBox.BringToFront();
            }
        }

        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);

            if (_searchBox.Text.Length == 0)
                _searchBox.Visible = false;
        }

        void _searchBox_Leave(object sender, EventArgs e)
        {
            if (_searchBox.Text.Length == 0)
                _searchBox.Visible = false;
        }

        private void OnFilteringItem(ItemFilteringEventArgs e)
        {
            if (FilteringItem != null)
                FilteringItem(this, e);
        }

        public event EventHandler<ItemFilteringEventArgs> FilteringItem;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == (int)WM.KeyDown || m.Msg == (int)WM.KeyUp)
            {
                if (!_searchBox.Focused)
                {
                    _searchBox.Visible = true;
                    _searchBox.Focus();
                }
                return;
            }
            else if (m.Msg == (int)WM.Char || m.Msg == (int)WM.SysChar)
            {
                NativeMethods.SendMessage(_searchBox.Handle, (WM)m.Msg, m.WParam, m.LParam);
            }

            try
            {
                base.WndProc(ref m);
            }
            catch (NullReferenceException) { }
        }
    }
}
